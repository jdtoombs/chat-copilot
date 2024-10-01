// Copyright (c) Microsoft. All rights reserved.

import {
    Link,
    Toast,
    Toaster,
    ToastIntent,
    ToastTitle,
    ToastTrigger,
    useId,
    useToastController,
} from '@fluentui/react-components';
import { Dismiss12Regular } from '@fluentui/react-icons';
import { useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '../../redux/app/hooks';
import { RootState } from '../../redux/app/store';
import { removeAlert } from '../../redux/features/app/appSlice';

const Alerts = () => {
    const dispatch = useAppDispatch();

    const toasterId = useId('toaster');
    const { dispatchToast } = useToastController(toasterId);

    const { alerts } = useAppSelector((state: RootState) => state.app);

    useEffect(() => {
        alerts.forEach((alert, index) => {
            console.log(alert.type);
            dispatchToast(
                <Toast>
                    <ToastTitle
                        action={
                            <ToastTrigger>
                                <Link>
                                    <Dismiss12Regular />
                                </Link>
                            </ToastTrigger>
                        }
                    >
                        {alert.message}
                    </ToastTitle>
                </Toast>,
                {
                    position: 'top-end',
                    intent: alert.type as ToastIntent,
                    onStatusChange: (_e, { status: toastStatus }) => {
                        if (toastStatus === 'dismissed') {
                            dispatch(removeAlert(index));
                        }
                    },
                },
            );
        });
    }, [alerts, dispatch, dispatchToast]);

    return <Toaster toasterId={toasterId} />;
};

export default Alerts;
