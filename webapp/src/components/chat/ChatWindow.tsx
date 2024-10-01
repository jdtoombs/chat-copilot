// Copyright (c) Microsoft. All rights reserved.

import {
    Label,
    makeStyles,
    Persona,
    SelectTabEventHandler,
    shorthands,
    Tab,
    TabList,
    TabValue,
    tokens,
} from '@fluentui/react-components';
import { Map16Regular } from '@fluentui/react-icons';
import React from 'react';
import { useAppSelector } from '../../redux/app/hooks';
import { RootState } from '../../redux/app/store';
import { FeatureKeys } from '../../redux/features/app/AppState';
import { ChatRoom } from './ChatRoom';
import { DeleteChatHistory } from './controls/DeleteChatHistory';
import { ParticipantsList } from './controls/ParticipantsList';
import { ShareBotMenu } from './controls/ShareBotMenu';
import { DocumentsTab } from './tabs/DocumentsTab';
import { PersonaTab } from './tabs/PersonaTab';
import { PlansTab } from './tabs/PlansTab';

// Enum for chat window tabs
enum ChatWindowTabEnum {
    CHAT = 'CHAT',
    DOCUMENTS = 'DOCUMENTS',
    PERSONA = 'PERSONA',
    PLANS = 'PLANS',
}

const useClasses = makeStyles({
    root: {
        display: 'flex',
        flexDirection: 'column',
        width: '100%',
        backgroundColor: tokens.colorNeutralBackground3,
        boxShadow: 'rgb(0 0 0 / 25%) 0 0.2rem 0.4rem -0.075rem',
    },
    header: {
        ...shorthands.borderBottom('1px', 'solid', 'rgb(0 0 0 / 10%)'),
        ...shorthands.padding(tokens.spacingVerticalS, tokens.spacingHorizontalM),
        backgroundColor: tokens.colorNeutralBackground4,
        display: 'flex',
        flexDirection: 'row',
        boxSizing: 'border-box',
        width: '100%',
        justifyContent: 'space-between',
    },
    title: {
        ...shorthands.gap(tokens.spacingHorizontalM),
        alignItems: 'center',
        display: 'flex',
        flexDirection: 'row',
    },
    controls: {
        display: 'flex',
        alignItems: 'center',
    },
});

export const ChatWindow: React.FC = () => {
    const classes = useClasses();
    const { features } = useAppSelector((state: RootState) => state.app);
    const { conversations, selectedId } = useAppSelector((state: RootState) => state.conversations);
    const [selectedTab, setSelectedTab] = React.useState<TabValue>(ChatWindowTabEnum.CHAT);

    const showShareBotMenu = features[FeatureKeys.BotAsDocs].enabled || features[FeatureKeys.MultiUserChat].enabled;
    const chatName = conversations[selectedId].title;
    const { chatSpecialization } = useAppSelector((state: RootState) => state.admin);

    const onTabSelect: SelectTabEventHandler = (_event, data) => {
        setSelectedTab(data.value);
    };

    return (
        <div className={classes.root}>
            <div className={classes.header}>
                <div className={classes.title}>
                    <Persona
                        key={'Semantic Kernel Bot'}
                        size="medium"
                        avatar={{
                            image: {
                                src: chatSpecialization?.iconFilePath
                                    ? chatSpecialization.iconFilePath
                                    : conversations[selectedId].botProfilePicture,
                            },
                        }}
                        presence={{ status: 'available' }}
                    />
                    <Label size="large" weight="semibold">
                        {chatSpecialization?.name}
                    </Label>
                    <TabList selectedValue={selectedTab} onTabSelect={onTabSelect}>
                        <Tab
                            data-testid="chatTab"
                            id="chat"
                            value={ChatWindowTabEnum.CHAT}
                            aria-label="Chat Tab"
                            title="Chat Tab"
                        >
                            Chat
                        </Tab>
                        <Tab
                            data-testid="documentsTab"
                            id="documents"
                            value={ChatWindowTabEnum.DOCUMENTS}
                            aria-label="Documents Tab"
                            title="Documents Tab"
                        >
                            Documents
                        </Tab>
                        {features[FeatureKeys.PluginsPlannersAndPersonas].enabled && (
                            <>
                                <Tab
                                    data-testid="plansTab"
                                    id="plans"
                                    value={ChatWindowTabEnum.PLANS}
                                    icon={<Map16Regular />}
                                    aria-label="Plans Tab"
                                    title="Plans Tab"
                                >
                                    Plans
                                </Tab>
                                {/* <Tab
                                    data-testid="personaTab"
                                    id="persona"
                                    value={ChatWindowTabEnum.PERSONA}
                                    icon={<Person16Regular />}
                                    aria-label="Persona Tab"
                                    title="Persona Tab"
                                >
                                    Persona
                                </Tab> */}
                            </>
                        )}
                    </TabList>
                </div>
                <div className={classes.controls}>
                    <div>
                        <DeleteChatHistory chatId={selectedId} />
                    </div>
                    {!features[FeatureKeys.SimplifiedExperience].enabled && (
                        <div data-testid="chatParticipantsView">
                            <ParticipantsList participants={conversations[selectedId].users} />
                        </div>
                    )}
                    {showShareBotMenu && (
                        <div>
                            <ShareBotMenu chatId={selectedId} chatTitle={chatName} />
                        </div>
                    )}
                </div>
            </div>
            {selectedTab === ChatWindowTabEnum.CHAT && <ChatRoom />}
            {selectedTab === ChatWindowTabEnum.DOCUMENTS && <DocumentsTab />}
            {selectedTab === ChatWindowTabEnum.PLANS && (
                <PlansTab
                    setChatTab={() => {
                        setSelectedTab(ChatWindowTabEnum.CHAT);
                    }}
                />
            )}
            {selectedTab === ChatWindowTabEnum.PERSONA && <PersonaTab />}
        </div>
    );
};
