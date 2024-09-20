import { makeStyles } from '@fluentui/react-components';
import React from 'react';
import { useAppSelector } from '../../../redux/app/hooks';
import { RootState } from '../../../redux/app/store';
import { ChatSuggestion } from './ChatSuggestion';

const useClasses = makeStyles({
    root: {
        display: 'flex',
        flexDirection: 'row',
        width: '100%',
        maxWidth: '105em',
        justifyContent: 'center',
    },
});

interface ChatSuggestionListProps {
    onClickSuggestion: (message: string) => void;
}

export const ChatSuggestionList: React.FC<ChatSuggestionListProps> = ({
    onClickSuggestion,
}: ChatSuggestionListProps) => {
    const conversation = useAppSelector((state: RootState) => state.conversations);
    const classes = useClasses();
    return (
        <div className={classes.root}>
            {conversation.conversations[conversation.selectedId].suggestions.current.map((suggestion, idx) => (
                <ChatSuggestion
                    onClick={onClickSuggestion}
                    key={`suggestions-${idx}`}
                    suggestionMainText={suggestion}
                />
            ))}
        </div>
    );
};
