import { Button, makeStyles, shorthands, tokens } from '@fluentui/react-components';
import { FC } from 'react';
import { DndProvider } from 'react-dnd';
import { HTML5Backend } from 'react-dnd-html5-backend';
import { useAISearchDeployment } from '../../../libs/hooks/useAISearchDeployment';
import { useAppDispatch, useAppSelector } from '../../../redux/app/hooks';
import { RootState } from '../../../redux/app/store';
import { setSelectedAISearchDeploymentKey } from '../../../redux/features/admin/adminSlice';
import { Breakpoints } from '../../../styles';
import { Add20 } from '../../shared/BundledIcons';
import { AdminItemListSection } from '../shared/list/AdminListItemSection';

const useClasses = makeStyles({
    root: {
        display: 'flex',
        flexShrink: 0,
        width: '320px',
        backgroundColor: tokens.colorNeutralBackground4,
        boxShadow: 'rgba(0, 0, 0, 0.25) 0px 0.2rem 0.4rem -0.075rem',
        flexDirection: 'column',
        ...shorthands.overflow('hidden'),
        ...Breakpoints.small({
            width: '64px',
        }),
    },
    list: {
        overflowY: 'auto',
        overflowX: 'hidden',
        alignItems: 'stretch',
    },
    header: {
        display: 'flex',
        flexDirection: 'row',
        justifyContent: 'space-between',
        marginRight: tokens.spacingVerticalM,
        marginLeft: tokens.spacingHorizontalXL,
        alignItems: 'center',
        height: '60px',
        ...Breakpoints.small({
            justifyContent: 'center',
        }),
    },
});

export const AISearchList: FC = () => {
    const classes = useClasses();
    const dispatch = useAppDispatch();
    const deploymentServices = useAISearchDeployment();
    const { aiSearchDeployments, selectedAISearchDeploymentId } = useAppSelector((state: RootState) => state.admin);
    const onAddOpenAIClick = () => {
        dispatch(setSelectedAISearchDeploymentKey(''));
    };

    return (
        <>
            <DndProvider backend={HTML5Backend}>
                <div className={classes.root}>
                    <div className={classes.header}>
                        <Button
                            data-testid="createNewAISearchDeploymentButton"
                            icon={<Add20 />}
                            appearance="primary"
                            onClick={() => {
                                onAddOpenAIClick();
                            }}
                        >
                            New Deployment
                        </Button>
                    </div>
                    <div aria-label={'ai search list'} className={classes.list}>
                        <AdminItemListSection
                            header="All"
                            items={aiSearchDeployments}
                            onItemCollectionReorder={(deployments) => {
                                void deploymentServices.setAISearchDeploymentOrder(deployments);
                            }}
                            onItemSelected={(id: string) => {
                                dispatch(setSelectedAISearchDeploymentKey(id));
                            }}
                            selectedId={selectedAISearchDeploymentId}
                        />
                    </div>
                </div>
            </DndProvider>
        </>
    );
};
