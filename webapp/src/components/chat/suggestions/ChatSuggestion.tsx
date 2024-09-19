import { Card, CardHeader, Text, makeStyles, tokens } from "@fluentui/react-components";
import React from "react";

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
  onClick: (message: string) => void;
  suggestionMainText: string;
}

export const ChatSuggestion: React.FC<ChatSuggestionProps> = ({ suggestionMainText, onClick }) => {
  const styles = useStyles();
  const suggestionDivID = React.useId();
  const cardId = React.useId();

  return (
    <div className={ styles.root } key={suggestionDivID}>
      <Card className={styles.card} data-testid="chatSuggestionItem" onClick={() => { onClick(suggestionMainText) } } key={cardId}>
        <CardHeader header={<Text weight="semibold">{suggestionMainText}</Text>}/>
      </Card>
    </div>
  )
}