import {
    Link,
    Toast,
    ToastBody,
    Toaster,
    ToastIntent,
    ToastTitle,
    ToastTrigger,
    useId,
    useToastController,
} from '@fluentui/react-components';
import { useEffect } from 'react';
import { useAppSelector } from '../../redux/app/hooks';
import { RootState } from '../../redux/app/store';
import { Dismiss16 } from '../shared/BundledIcons';

const ToastContainer = () => {
    const toasterId = useId('toaster');
    const currentToast = useAppSelector((state: RootState) => state.toast.currentToast);

    const { dispatchToast } = useToastController(toasterId);

    useEffect(() => {
        if (currentToast) {
            const { title, body, intent } = currentToast;
            dispatchToast(
                <Toast>
                    <ToastTitle
                        action={
                            <ToastTrigger>
                                <Link>
                                    <Dismiss16 />
                                </Link>
                            </ToastTrigger>
                        }
                    >
                        {title}
                    </ToastTitle>
                    <ToastBody>{body}</ToastBody>
                </Toast>,
                { position: 'top-end', intent: intent as ToastIntent },
            );
        }
    }, [currentToast, dispatchToast]);

    return <Toaster toasterId={toasterId} />;
};

export default ToastContainer;
