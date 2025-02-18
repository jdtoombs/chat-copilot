import { Body1, Card, CardPreview, makeStyles, Subtitle2, tokens } from '@fluentui/react-components';
import * as React from 'react';
import { useChat } from '../../libs/hooks';
import { AlertType } from '../../libs/models/AlertType';
import { ISpecialization } from '../../libs/models/Specialization';
import { useAppDispatch, useAppSelector } from '../../redux/app/hooks';
import { RootState } from '../../redux/app/store';
import { setChatSpecialization } from '../../redux/features/admin/adminSlice';
import { addAlert, hideSpinner, showSpinner } from '../../redux/features/app/appSlice';
import {
    editConversationSpecialization,
    editConversationSystemDescription,
} from '../../redux/features/conversations/conversationsSlice';

const useStyles = makeStyles({
    main: {
        display: 'flex',
        flexWrap: 'wrap',
    },

    card: {
        display: 'flex',
        flexDirection: 'column',
    },

    root: {
        padding: tokens.spacingHorizontalS,
    },

    caption: {
        color: tokens.colorNeutralForeground3,
        overflow: 'hidden',
        textOverflow: 'ellipsis',
        marginTop: '8px',
    },

    cardImage: { borderRadius: tokens.borderRadiusSmall, objectFit: 'contain' },

    cardPreview: {
        backgroundColor: tokens.colorNeutralBackground3,
        height: '100px',
    },

    cardContent: {
        padding: '1.5rem',
        display: 'flex',
        flexDirection: 'column',
    },

    cardTitle: {
        display: 'block',
        overflow: 'hidden',
        textOverflow: 'ellipsis',
    },

    cardDesc: {
        marginTop: '16px',
        overflow: 'hidden',
        textOverflow: 'ellipsis',
    },

    logoBadge: {
        padding: '5px',
        borderRadius: tokens.borderRadiusSmall,
        backgroundColor: '#FFF',
        boxShadow: '0px 1px 2px rgba(0, 0, 0, 0.14), 0px 0px 2px rgba(0, 0, 0, 0.12)',
    },

    showTooltip: {
        display: 'show',
    },

    hideTooltip: {
        display: 'none',
    },
});

interface SpecializationItemProps {
    specialization: ISpecialization;
}

export const SpecializationCard: React.FC<SpecializationItemProps> = ({ specialization }) => {
    const styles = useStyles();
    const chat = useChat();
    const cardId = React.useId();
    const dispatch = useAppDispatch();
    const { selectedId } = useAppSelector((state: RootState) => state.conversations);
    const { specializations } = useAppSelector((state: RootState) => state.admin);
    // eslint-disable-next-line @typescript-eslint/no-unsafe-return
    const onAddChat = () => {
        dispatch(showSpinner());
        void chat
            .selectSpecializationAndBeginChat(specialization.id, selectedId)
            .then(() => {
                const specializationMatch = specializations.find((spec) => spec.id === specialization.id);
                if (specializationMatch) {
                    dispatch(setChatSpecialization(specializationMatch));
                }
                dispatch(editConversationSpecialization({ id: selectedId, specializationId: specialization.id }));
                dispatch(
                    editConversationSystemDescription({
                        id: selectedId,
                        newSystemDescription: specialization.roleInformation,
                    }),
                );
            })
            .catch(() => {
                dispatch(
                    addAlert({ message: 'Unable to select the specified specialization.', type: AlertType.Error }),
                );
            })
            .finally(() => {
                dispatch(hideSpinner());
            });
    };

    const truncate = (str: string) => {
        return str.length > 250 ? str.substring(0, 250) : str;
    };

    const getimagefilepath = (str: any): string => {
        // eslint-disable-next-line @typescript-eslint/no-unsafe-return
        return str;
    };

    return (
        <Card className={styles.card} data-testid="addNewBotMenuItem" onClick={onAddChat} key={cardId}>
            <CardPreview className={styles.cardPreview}>
                <img
                    className={styles.cardImage}
                    src={getimagefilepath(specialization.imageFilePath)}
                    alt="Presentation Preview"
                />
            </CardPreview>

            <div className={styles.cardContent}>
                <Subtitle2>{specialization.name}</Subtitle2>
                <Body1 className={styles.caption}>{truncate(specialization.description)}</Body1>
            </div>
        </Card>
    );
};
