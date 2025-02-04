import { makeStyles, shorthands } from '@fluentui/react-components';
import { FC } from 'react';
import { useAppSelector } from '../../redux/app/hooks';
import { RootState } from '../../redux/app/store';
import { AdminScreen } from '../../redux/features/admin/AdminState';
import { AISearchList } from '../admin/ai-search-deployments/AISearchList';
import { AISearchManager } from '../admin/ai-search-deployments/AISearchManager';
import { OpenAIList } from '../admin/open-ai-deployments/OpenAIList';
import { OpenAIManager } from '../admin/open-ai-deployments/OpenAIManager';
import { AdminScreenWrapper } from '../admin/shared/AdminScreenWrapper';
import { SpecializationIndexList } from '../admin/specialization-index/SpecializationIndexList';
import { SpecializationIndexManager } from '../admin/specialization-index/SpecializationIndexManager';
import { SpecializationList } from '../admin/specialization/SpecializationList';
import { SpecializationManager } from '../admin/specialization/SpecializationManager';
import { UserFeedbackManager } from '../admin/user-feedback/UserFeedbackManager';
import { ChatWindow } from '../chat/ChatWindow';
import { ChatType } from '../chat/chat-list/ChatType';
import { SearchWindow } from '../search/SearchWindow';

const useClasses = makeStyles({
    container: {
        ...shorthands.overflow('hidden'),
        display: 'flex',
        flexDirection: 'row',
        alignContent: 'start',
        height: '100%',
    },
});

export const ChatView: FC = () => {
    const classes = useClasses();
    const { selectedAdminScreen } = useAppSelector((state: RootState) => state.admin);
    const renderCurrentScreen = (screen: AdminScreen): JSX.Element => {
        switch (screen) {
            case AdminScreen.NONE:
                return <ChatWindow />;
            case AdminScreen.SEARCH:
                return <SearchWindow />;
            case AdminScreen.SPECIALIZATION:
                return <AdminScreenWrapper sidebar={<SpecializationList />} adminScreen={<SpecializationManager />} />;
            case AdminScreen.INDEX:
                return (
                    <AdminScreenWrapper
                        sidebar={<SpecializationIndexList />}
                        adminScreen={<SpecializationIndexManager />}
                    />
                );
            case AdminScreen.AISEARCHDEPLOYMENT:
                return <AdminScreenWrapper sidebar={<AISearchList />} adminScreen={<AISearchManager />} />;
            case AdminScreen.OPENAIDEPLOYMENT:
                return <AdminScreenWrapper sidebar={<OpenAIList />} adminScreen={<OpenAIManager />} />;
            case AdminScreen.FEEDBACK:
                return <AdminScreenWrapper adminScreen={<UserFeedbackManager />} />;
            default:
                return <></>;
        }
    };
    return (
        <div className={classes.container}>
            <ChatType />
            {renderCurrentScreen(selectedAdminScreen)}
        </div>
    );
};
