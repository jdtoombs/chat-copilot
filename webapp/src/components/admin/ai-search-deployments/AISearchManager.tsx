import { Button, Input, makeStyles, shorthands, tokens } from '@fluentui/react-components';
import React, { useEffect, useState } from 'react';
import { useAISearchDeployment } from '../../../libs/hooks/useAISearchDeployment';
import { IAISearchDeployment } from '../../../libs/models/AISearchDeployment';
import { useAppSelector } from '../../../redux/app/hooks';
import { RootState } from '../../../redux/app/store';
import { setSelectedOpenAIDeploymentKey } from '../../../redux/features/admin/adminSlice';
import { ConfirmationDialog } from '../../shared/ConfirmationDialog';

const useClasses = makeStyles({
    root: {
        display: 'flex',
        flexDirection: 'column',
        ...shorthands.gap(tokens.spacingVerticalSNudge),
        ...shorthands.padding('80px'),
    },
    horizontal: {
        display: 'flex',
        ...shorthands.gap(tokens.spacingVerticalSNudge),
        alignItems: 'center',
    },
    controls: {
        display: 'flex',
        marginLeft: 'auto',
        ...shorthands.gap(tokens.spacingVerticalSNudge),
    },
    dialog: {
        maxWidth: '25%',
    },
    required: {
        color: '#990000',
    },
    scrollableContainer: {
        overflowY: 'auto',
        maxHeight: 'calc(100vh - 100px)', // Adjust this value as needed
        ...shorthands.padding('10px'),
    },
    fileUploadContainer: {
        display: 'flex',
        flexDirection: 'row',
        ...shorthands.gap(tokens.spacingHorizontalXXXL),
    },
    imageContainer: {
        display: 'flex',
        flexDirection: 'column',
        ...shorthands.gap(tokens.spacingVerticalSNudge),
    },
    slidersContainer: {
        display: 'flex',
        flexDirection: 'column',
        ...shorthands.gap(tokens.spacingVerticalSNudge),
        ...shorthands.marginInline('10px'),
    },
    slider: {
        display: 'flex',
        ...shorthands.gap(tokens.spacingVerticalSNudge),
        alignItems: 'center',
    },
    input: {
        width: '80px',
    },
    tripleFieldArrayRoot: {
        display: 'flex',
        flexDirection: 'column',
        gap: '1rem',
    },
    tripleFieldElement: {
        display: 'flex',
        flexDirection: 'row',
        gap: '1rem',
    },
    tripleFieldInputElement: {
        flexGrow: 1,
    },
    tripleFieldAddButton: {
        width: '110px',
    },
    tripleFieldRemoveButton: {
        height: '32px',
        alignSelf: 'end',
    },
});

export const AISearchManager: React.FC = () => {
    const classes = useClasses();
    const aiSearchServices = useAISearchDeployment();
    const { selectedAISearchDeploymentId, aiSearchDeployments } = useAppSelector((state: RootState) => state.admin);
    const [id, setId] = useState('');
    const [name, setName] = useState('');
    const [label, setLabel] = useState('');
    const [endpoint, setEndpoint] = useState('');
    const [secretName, setSecretName] = useState('');
    const [editMode, setEditMode] = useState(false);
    const [order, setOrder] = useState(0);

    const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);

    const isValid = !!name && !!endpoint && !!secretName && !!label;

    const onSaveAISearchDeployment = (editMode: boolean): void => {
        const deployment: IAISearchDeployment = {
            id: '',
            name,
            endpoint,
            secretName,
            label,
            order: editMode ? order : aiSearchDeployments.length,
        };

        if (editMode) {
            void aiSearchServices.updateAISearchDeployment(selectedAISearchDeploymentId, deployment);
        } else {
            void aiSearchServices.saveAISearchDeployment(deployment);
        }
    };

    const confirmDelete = () => {
        void aiSearchServices.deleteAISearchDeployment(id);
        setSelectedOpenAIDeploymentKey('');
        fillState({
            name: '',
            id: '',
            endpoint: '',
            secretName: '',
            label: '',
            order: 0,
        });
        setIsDeleteDialogOpen(false);
    };

    const onDeleteAISearchDeployment = (): void => {
        setIsDeleteDialogOpen(true);
    };

    const fillState = (deployment: IAISearchDeployment) => {
        setId(deployment.id);
        setName(deployment.name);
        setEndpoint(deployment.endpoint);
        setSecretName(deployment.secretName);
        setLabel(deployment.label);
        setOrder(deployment.order);
    };

    useEffect(() => {
        if (selectedAISearchDeploymentId != '') {
            setEditMode(true);
            const deployment = aiSearchDeployments.find((a) => a.id === selectedAISearchDeploymentId);
            if (deployment) {
                fillState(deployment);
            }
        } else {
            setEditMode(false);
            fillState({
                name: '',
                id: '',
                endpoint: '',
                secretName: '',
                label: '',
                order: 0,
            });
        }
    }, [editMode, selectedAISearchDeploymentId, aiSearchDeployments]);

    return (
        <div className={classes.scrollableContainer}>
            <div className={classes.root}>
                <div className={classes.horizontal}></div>
                <label htmlFor="name">
                    Name<span className={classes.required}>*</span>
                </label>
                <Input
                    id="name"
                    required
                    value={name}
                    onChange={(_event, data) => {
                        setName(data.value);
                    }}
                />
                <label htmlFor="label">
                    Label<span className={classes.required}>*</span>
                </label>
                <Input
                    id="label"
                    required
                    value={label}
                    onChange={(_event, data) => {
                        setLabel(data.value);
                    }}
                />
                <label htmlFor="endpoint">
                    Endpoint<span className={classes.required}>*</span>
                </label>
                <Input
                    id="endpoint"
                    required
                    value={endpoint}
                    onChange={(_event, data) => {
                        setEndpoint(data.value);
                    }}
                />
                <label htmlFor="secretName">
                    Secret Name<span className={classes.required}>*</span>
                </label>
                <Input
                    id="secretName"
                    required
                    value={secretName}
                    onChange={(_event, data) => {
                        setSecretName(data.value);
                    }}
                />
                <ConfirmationDialog
                    open={isDeleteDialogOpen}
                    title="Delete AI Search Deployment"
                    content={`Are you sure you want to delete the ${name} AI Search Deployment?`}
                    confirmLabel="Delete"
                    cancelLabel="Cancel"
                    onConfirm={confirmDelete}
                    onCancel={() => {
                        setIsDeleteDialogOpen(false);
                    }}
                />
                <div className={classes.controls}>
                    <Button appearance="secondary" disabled={!id} onClick={onDeleteAISearchDeployment}>
                        Delete
                    </Button>

                    <Button
                        appearance="primary"
                        disabled={!isValid}
                        onClick={() => {
                            onSaveAISearchDeployment(editMode);
                        }}
                    >
                        Save
                    </Button>
                </div>
            </div>
        </div>
    );
};
