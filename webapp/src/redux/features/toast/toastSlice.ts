import { createSlice, PayloadAction } from '@reduxjs/toolkit';
import { initialState, ToastData, ToastState } from './ToastState';

export const toastSlice = createSlice({
    name: 'toast',
    initialState,
    reducers: {
        // Set a new toast
        setToast: (state: ToastState, action: PayloadAction<ToastData | null>) => {
            state.currentToast = action.payload;
        },
    },
});

export const { setToast } = toastSlice.actions;

export default toastSlice.reducer;
