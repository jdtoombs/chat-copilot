export interface ToastData {
    id: string;
    title?: string;
    body?: string;
    intent?: string;
}

export interface ToastState {
    currentToast: ToastData | null;
}

export const initialState: ToastState = {
    currentToast: null,
};
