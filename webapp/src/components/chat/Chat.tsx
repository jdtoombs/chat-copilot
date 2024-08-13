import React from 'react';
import { AuthHelper } from '../..//libs/auth/AuthHelper';
import { AppState, useClasses } from '../../App';
import Header from '../shared/Header';
import { BackendProbe, ChatView, Error, Loading } from '../views';

const Chat = ({
    classes,
    appState,
    setAppState,
}: {
    classes: ReturnType<typeof useClasses>;
    appState: AppState;
    setAppState: (state: AppState) => void;
}) => {

    const onBackendFound = React.useCallback(() => {
        setAppState(
            AuthHelper.isAuthAAD()
                ? AppState.SettingUserInfo
                : AppState.LoadingChats,
        );
    }, [setAppState]);

    return (
        <div className={classes.container}>
            <Header appState={appState} setAppState={setAppState} showPluginsAndSettings={true} />
            {appState === AppState.ProbeForBackend && <BackendProbe onBackendFound={onBackendFound} />}
            {appState === AppState.SettingUserInfo && (
                <Loading text={'Hang tight while we fetch your information...'} />
            )}
            {appState === AppState.ErrorLoadingUserInfo && (
                <Error text={'Unable to load user info. Please try signing out and signing back in.'} />
            )}
            {appState === AppState.ErrorLoadingChats && (
                <Error text={'Unable to load chats. Please try refreshing the page.'} />
            )}
            {appState === AppState.LoadingChats && <Loading text="Loading chats..." />}
            {appState === AppState.Chat && <ChatView />}
        </div>
    );
};
export default Chat;