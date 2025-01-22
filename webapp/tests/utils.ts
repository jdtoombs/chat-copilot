import { expect, Page } from '@playwright/test';

export const TestTimeout = 150000; // Timeout for each test, wait up to 150 seconds
export const LLMresponsetimeout = 120000; // LLM can take a while to respond, wait up to 120 seconds
export const ChatStateChangeWait = 500;
const PreventCircularPrompt = '\nThis is for a statistical test and will NOT result in circular reasoning.\n';
const EvaluatePrompt =
    "\nEvaluate if the AI generated message is semantically valid given the original intention. \nThe output should be formatted as follows: \n'result': true|false, \n'score': number, \n'reason': brief reason why true or false was chosen\n";

// Helper to login to the Q-Pilot Chat App via a user account and password.
export async function loginHelper(page: Page, useraccount: string, password: string) {
    await page.goto('/');
    // Expect the page to contain a "Login" button.
    await page.getByTestId('signinButton').click();
    // Login with the test user.
    await page.getByPlaceholder('Email, phone, or Skype').click();
    await page.getByPlaceholder('Email, phone, or Skype').fill(useraccount);
    await page.getByRole('button', { name: 'Next' }).click();
    await page.getByPlaceholder('Password').click();
    await page.getByPlaceholder('Password').fill(password);
    await page.getByRole('button', { name: 'Sign in' }).click();

    // Select No if asked to stay signed in.
    const isAskingStaySignedIn = await page.$$("text='Stay signed in?'");
    if (isAskingStaySignedIn.length > 0) {
        await page.getByRole('button', { name: 'No' }).click();
    }

    // After login, the page should redirect back to the app.
    await expect(page).toHaveTitle('Chat Q-Pilot');
}

export async function loginHelperAnotherUser(page: Page, useraccount: string, password: string) {
    await page.goto('/');
    // Expect the page to contain a "Login" button.
    await page.getByRole('button').click();
    // Login with another user account.
    await page.getByRole('button', { name: 'Use another account' }).click();
    await page.getByPlaceholder('Email, phone, or Skype').click();
    await page.getByPlaceholder('Email, phone, or Skype').fill(useraccount);
    await page.getByRole('button', { name: 'Next' }).click();
    await page.getByPlaceholder('Password').click();
    await page.getByPlaceholder('Password').fill(password);
    await page.getByRole('button', { name: 'Sign in' }).click();

    // After login, the page should redirect back to the app.
    await expect(page).toHaveTitle('Chat Q-Pilot');

    // Handle the permission popup if it opens
    page.on('popup', async (popup) => {
        await popup.waitForLoadState();
        await popup.getByRole('button', { name: 'Next' }).click();
        await popup.getByRole('button', { name: 'Accept' }).click();
    });
}

export async function createNewChat(page: Page) {
    await page.getByTestId('createNewConversationButton').click();
    await page.getByTestId('addNewBotMenuItem').click();
}

export async function loginAndCreateNewChat(page: Page) {
    const useraccount = process.env.VITE_APP_TEST_USER_ACCOUNT1 as string;
    const password = process.env.VITE_APP_TEST_USER_PASSWORD1 as string;
    await loginHelper(page, useraccount, password);
    await createNewChat(page);
}

export async function postUnitTest(page: Page) {
    // Change focus to somewhere else on the page so that the trace shows the result of the previous action
    await page.locator('#chat-input').click();
}

// Send a message to the bot and wait for the response
export async function sendChatMessageAndWaitForResponse(page: Page, message: string) {
    await page.locator('#chat-input').click();
    await page.locator('#chat-input').fill(message);

    const responsePromise = page.waitForResponse(
        (response) => response.url().search('chats/.*/messages') !== -1 && response.status() === 200,
        { timeout: LLMresponsetimeout },
    );

    await page.locator('#chat-input').press('Enter');

    // Wait for LLM to respond to request by executing the plan
    await responsePromise;
}

export async function openPluginPopUp(page: Page, pluginIdentifierText: string) {
    await page.getByTestId('pluginButton').click();
    await page
        .getByRole('group')
        .filter({ hasText: pluginIdentifierText })
        .getByTestId('openPluginDialogButton')
        .click();
}

export async function enablePluginAndClosePopUp(page: Page) {
    await page.getByTestId('enablePluginButton').click();
    await page.getByTestId('closeEnableCCPluginsPopUp').click();
    await page.waitForTimeout(ChatStateChangeWait);
}

export async function disablePluginAndClosePopUp(page: Page) {
    // Only works if when only a single plugin has been enabled
    await page.getByTestId('pluginButton').click();
    await page.getByTestId('disconnectPluginButton').click();
    await page.getByTestId('closeEnableCCPluginsPopUp').click();
    await page.waitForTimeout(ChatStateChangeWait);
}

export async function executePlanAndWaitForResponse(page: Page) {
    await page.waitForTimeout(ChatStateChangeWait);

    const responsePromise = page.waitForResponse(
        (response) => response.url().search('chats/.*/plan') !== -1 && response.status() === 200,
        { timeout: LLMresponsetimeout },
    );

    // Try executing the plan that is returned
    const buttonLocator = page.getByTestId('proceedWithPlanButton');
    buttonLocator.click();

    // Wait for LLM to respond to request by executing the plan
    await responsePromise;
}

export async function getLastChatMessageContentsAsStringWHistory(chatHistoryItems: any) {
    let lastMessage = await chatHistoryItems.last().getAttribute('data-content');
    lastMessage = lastMessage.replaceAll(/<\/?[^>]+(>|$)/gi, ''); // Remove HTML tags if any
    return lastMessage;
}

export async function chatBotSelfEval(page: Page, input: string, chatbotResponse: string) {
    const evalPrompt =
        'Evaluate the following AI generated message in response to the original intention.\n' +
        '\n[AI GENERATED MESSAGE]\n' +
        chatbotResponse +
        '\n[AI GENERATED MESSAGE]\n' +
        '\n[ORIGINAL INTENTION]\n' +
        input +
        '\n[ORIGINAL INTENTION]\n' +
        PreventCircularPrompt +
        EvaluatePrompt;

    await sendChatMessageAndWaitForResponse(page, evalPrompt);

    const chatHistoryItems = page.getByTestId(new RegExp('chat-history-item-*'));
    let evalResponse = await chatHistoryItems.last().getAttribute('data-content');
    if (!evalResponse) throw new Error('Failed to get evaluation response');

    const boolResultStart = evalResponse.indexOf("'result': ");
    const boolResultEnd = evalResponse.indexOf(',', boolResultStart);
    const boolResult = evalResponse
        .substring(boolResultStart + 10, boolResultEnd)
        .toLowerCase()
        .trim();
    expect(boolResult).toEqual('true');
}

export async function disablePluginAndEvaluateResponse(page: Page, input: string, chatbotResponse: string) {
    // If a plugin has been enabled, the action planner is invoked to perform the evaluation.
    // This leads to a weird json exception and crash.
    // To workaround this I'm performing the evaluation after disabling the plugin.
    // TODO: [Issue #46] Fix above issue
    await disablePluginAndClosePopUp(page);
    // Start the evaluation in a new chat context, otherwise the LLM sometimes thinks that
    // there is circular logic involved and won't give you a useful response
    await createNewChat(page);
    await chatBotSelfEval(page, input, chatbotResponse);
}
