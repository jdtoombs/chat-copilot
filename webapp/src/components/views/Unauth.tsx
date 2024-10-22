import { Body1, Title3 } from '@fluentui/react-components';
import React from 'react';
import { useSharedClasses } from '../../styles';

/** Simple component to display when the user is not authorized to view the page. */
export const Unauth: React.FC = () => {
    const classes = useSharedClasses();

    return (
        <div className={classes.informativeView}>
            <Title3 className="title">Your account does not have the appropriate roles.</Title3>
            <Body1>{'Contact your administrator to verify your access to QPilot.'}</Body1>
        </div>
    );
};
