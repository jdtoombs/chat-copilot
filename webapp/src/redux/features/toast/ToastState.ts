export interface ToastData {
    title?: string;
    body?: string;
    intent?: AppToastIntent;
}

export enum AppToastIntent {
    Success = 'success',
    Error = 'error',
    Warning = 'warning',
    Info = 'info',
}

export interface ToastState {
    currentToast: ToastData | null;
}

export const initialState: ToastState = {
    currentToast: null,
};
