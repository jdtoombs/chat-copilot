import { Button, Tooltip } from '@fluentui/react-components';
import { ChatDismissRegular } from '@fluentui/react-icons';
import debug from 'debug';
import { FC } from 'react';
import { Constants } from '../../../Constants';
import { useChat } from '../../../libs/hooks';
import { AlertType } from '../../../libs/models/AlertType';
import { useAppDispatch, useAppSelector } from '../../../redux/app/hooks';
import { RootState } from '../../../redux/app/store';
import { addAlert } from '../../../redux/features/app/appSlice';

const log = debug(Constants.debug.root).extend('chat-input');

interface DeleteChatHistoryProps {
    chatId: string;
}

export const DeleteChatHistory: FC<DeleteChatHistoryProps> = ({ chatId }) => {
    const chat = useChat();
    const dispatch = useAppDispatch();

    const { conversations } = useAppSelector((state: RootState) => state.conversations);
    const botResponseStatus = conversations[chatId].botResponseStatus;

    const handleDeleteChatHistory = () => {
        chat.deleteChatHistory(chatId).catch((error) => {
            const message = `Error deleting chat history: ${(error as Error).message}`;
            log(message);
            dispatch(
                addAlert({
                    type: AlertType.Error,
                    message,
                }),
            );
        });
    };

    return (
        <div>
            <Tooltip content={'Clear chat history'} relationship="label">
                <Button
                    onClick={handleDeleteChatHistory}
                    icon={<ChatDismissRegular />}
                    appearance="transparent"
                    disabled={botResponseStatus != null}
                />
            </Tooltip>
        </div>
    );
};