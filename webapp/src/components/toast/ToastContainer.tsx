import {
    Toast,
    ToastBody,
    Toaster,
    ToastIntent,
    ToastTitle,
    useId,
    useToastController,
} from '@fluentui/react-components';
import { useEffect } from 'react';
import { useAppSelector } from '../../redux/app/hooks';
import { RootState } from '../../redux/app/store';

const ToastContainer = () => {
    const toasterId = useId('toaster');
    const currentToast = useAppSelector((state: RootState) => state.toast.currentToast);

    const { dispatchToast } = useToastController(toasterId);

    useEffect(() => {
        if (currentToast) {
            const { title, body, intent, id } = currentToast;
            if (title ?? body) {
                dispatchToast(
                    <Toast key={id}>
                        {title && <ToastTitle>{title}</ToastTitle>}
                        {body && <ToastBody>{body}</ToastBody>}
                    </Toast>,
                    { intent: intent as ToastIntent },
                );
            }
        }
    }, [currentToast, dispatchToast]);

    return <Toaster toasterId={toasterId} />;
};

export default ToastContainer;
