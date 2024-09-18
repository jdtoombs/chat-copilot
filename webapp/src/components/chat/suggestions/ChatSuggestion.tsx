import { Card, CardHeader, Text, makeStyles, tokens } from "@fluentui/react-components";
import React from "react";
import { useChat } from "../../../libs/hooks";
import { GetResponseOptions } from "../../../libs/hooks/useChat";
import { ChatMessageType } from "../../../libs/models/ChatMessage";
import { useAppSelector } from "../../../redux/app/hooks";
import { RootState } from "../../../redux/app/store";

const useStyles = makeStyles({
    main: {
        display: 'flex',
        flexWrap: 'wrap',
    },

    card: {
        maxWidth: '250px',
        height: '100px',
    },

    root: {
        padding: tokens.spacingHorizontalS,
    },

    caption: {
        color: tokens.colorNeutralForeground3,
        overflow: 'hidden',
        textOverflow: 'ellipsis',
    },

    smallRadius: { borderRadius: tokens.borderRadiusSmall },

    grayBackground: {
        backgroundColor: tokens.colorNeutralBackground3,
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

interface ChatSuggestionProps {
  suggestionTitleText: string;
  suggestionMainText: string;
}

export const ChatSuggestion: React.FC<ChatSuggestionProps> = ({ suggestionMainText}) => {
  const styles = useStyles();
  const chat = useChat();
  const converstationState = useAppSelector((state: RootState) => state.conversations);
  const suggestionDivID = React.useId();
  const cardId = React.useId();
  const onSendSuggestion = () => {
    const messageBody: GetResponseOptions = {
      messageType: ChatMessageType.Message,
      value: suggestionMainText,
      chatId: converstationState.selectedId
    }
    void chat.getResponse(messageBody);
  }
  return (
    <div className={ styles.root } key={suggestionDivID}>
      <Card className={styles.card} data-testid="chatSuggestionItem" onClick={onSendSuggestion} key={cardId}>
        <CardHeader header={<Text weight="semibold">{suggestionMainText}</Text>}/>
      </Card>
    </div>
  )
}