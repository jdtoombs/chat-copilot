import { useAppDispatch } from '../../redux/app/hooks';
import { setToast } from '../../redux/features/toast/toastSlice';
import { ToastData } from '../../redux/features/toast/ToastState';

export const useToast = () => {
    const dispatch = useAppDispatch();

    const showToast = (toastData: ToastData) => {
        dispatch(setToast(toastData));
    };

    return {
        showToast,
    };
};
