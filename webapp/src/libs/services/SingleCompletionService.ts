// Copyright (c) Microsoft. All rights reserved.
import { IAsk } from '../semantic-kernel/model/Ask';
import { IAskResult } from '../semantic-kernel/model/AskResult';
import { BaseService } from './BaseService';

/**
 * A class for interactging with chat bot WITHOUT a particular chat. or specialization
 */
export class SingleCompletionService extends BaseService {
    /**
     * getBotResponseNoChat - Calling this with a valid ask object will query the chatbot through a POST request,
     * chat id and other chat parameters not required as this is simply a message and response
     * @param ask query for the chat bot
     * @param accessToken valid access token
     * @param enabledPlugins plugins, if any
     */
    public getBotResponseNoChat = async (
        ask: IAsk,
        accessToken: string,
        specializationId?: string,
    ): Promise<IAskResult> => {
        const result = await this.getResponseAsync<IAskResult>(
            {
                commandPath: `/completions/chats`,
                query: new URLSearchParams({ specializationId: specializationId ?? '' }),
                method: 'POST',
                body: ask,
            },
            accessToken,
        );
        return result;
    };
}
