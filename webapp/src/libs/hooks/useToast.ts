import { useAppDispatch } from '../../redux/app/hooks';
import { setToast } from '../../redux/features/toast/toastSlice';

export const useToast = () => {
    const dispatch = useAppDispatch();

    /**
     * Displays a toast notification.
     *
     * @param {Object} params - The parameters for the toast.
     * @param {string} [params.title] - The title of the toast message.
     * @param {string} [params.body] - The body content of the toast message.
     * @param {'success' | 'error' | 'informative' | 'warning'} [params.intent] - The intent of the toast.
     *        Must be one of 'success', 'error', 'informative', or 'warning'.
     */
    const showToast = ({ title, body, intent }: { title?: string; body?: string; intent?: string }) => {
        // Generate a unique ID
        const id = `toast-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
        dispatch(setToast({ id, title, body, intent }));
    };

    return {
        showToast,
    };
};
