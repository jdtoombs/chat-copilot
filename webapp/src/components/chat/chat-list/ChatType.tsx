import { makeStyles, SelectTabEventHandler, shorthands, Tab, TabList, tokens } from '@fluentui/react-components';
import { FC, useEffect, useState } from 'react';
import { useAppDispatch, useAppSelector } from '../../../redux/app/hooks';
import { RootState } from '../../../redux/app/store';
import { setAdminSelected } from '../../../redux/features/admin/adminSlice';
import { AdminScreen } from '../../../redux/features/admin/AdminState';
import { Breakpoints } from '../../../styles';
import { SearchList } from '../../search/search-list/SearchList';
import { ChatList } from './ChatList';

const useClasses = makeStyles({
    root: {
        display: 'flex',
        flexShrink: 0,
        width: '320px',
        backgroundColor: tokens.colorNeutralBackground4,
        flexDirection: 'column',
        ...shorthands.overflow('hidden'),
        ...Breakpoints.small({
            width: '64px',
        }),
    },
    innerTabs: {
        marginLeft: '1rem',
        marginRight: '1rem',
        marginTop: '2rem',
    },
});

export const ChatType: FC = () => {
    const classes = useClasses();
    const dispatch = useAppDispatch();
    const { selectedAdminScreen } = useAppSelector((state: RootState) => state.admin);
    const activeUserInfo = useAppSelector((state: RootState) => state.app.activeUserInfo);
    const [hasAdmin, setHasAdmin] = useState(false);

    const onTabSelect: SelectTabEventHandler = (_event, data) => {
        dispatch(setAdminSelected(data.value as AdminScreen));
    };

    const resolveMainTabValue = (screen: AdminScreen) => {
        if ([AdminScreen.NONE, AdminScreen.SEARCH].includes(screen)) {
            return screen;
        } else {
            return AdminScreen.ADMIN;
        }
    };

    useEffect(() => {
        if (activeUserInfo) {
            setHasAdmin(activeUserInfo.hasAdmin);
        }
    }, [activeUserInfo]);

    return (
        <div className={classes.root}>
            <TabList selectedValue={resolveMainTabValue(selectedAdminScreen)} onTabSelect={onTabSelect}>
                <Tab data-testid="chatTab" id="chat" value={AdminScreen.NONE} aria-label="Chat Tab" title="Chat Tab">
                    Chat
                </Tab>
                <Tab
                    data-testid="searchTab"
                    id="search"
                    value={AdminScreen.SEARCH}
                    aria-label="Search Tab"
                    title="Search Tab"
                >
                    Search
                </Tab>
                <Tab
                    disabled={!hasAdmin}
                    data-testid="adminTab"
                    id="admin"
                    value={AdminScreen.ADMIN}
                    aria-label="admin Tab"
                    title="Admin Tab"
                >
                    Admin
                </Tab>
            </TabList>
            {selectedAdminScreen === AdminScreen.NONE && <ChatList />}
            {selectedAdminScreen === AdminScreen.SEARCH && <SearchList />}
            {![AdminScreen.NONE, AdminScreen.SEARCH].includes(selectedAdminScreen) && (
                <div className={classes.innerTabs}>
                    <TabList vertical selectedValue={selectedAdminScreen} onTabSelect={onTabSelect}>
                        <Tab id="specializations" value={AdminScreen.SPECIALIZATION}>
                            Specializations
                        </Tab>
                        <Tab id="indexes" value={AdminScreen.INDEX}>
                            Indexes
                        </Tab>
                        <Tab id="userFeedback" value={AdminScreen.FEEDBACK}>
                            User Feedback
                        </Tab>
                        <Tab id="openAIDeployments" value={AdminScreen.OPENAIDEPLOYMENT}>
                            Open AI Deployments
                        </Tab>
                        <Tab id="aiSearchDeployments" value={AdminScreen.AISEARCHDEPLOYMENT}>
                            AI Search Deployments
                        </Tab>
                    </TabList>
                </div>
            )}
        </div>
    );
};
