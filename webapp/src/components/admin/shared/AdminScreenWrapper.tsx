import React from 'react';
import { AdminWindow } from './AdminWindow';

interface IAdminScreenWrapperProps {
    sidebar?: JSX.Element;
    adminScreen: JSX.Element;
}

export const AdminScreenWrapper: React.FC<IAdminScreenWrapperProps> = (props: IAdminScreenWrapperProps) => {
    return (
        <>
            {props.sidebar}
            <AdminWindow>{props.adminScreen}</AdminWindow>
        </>
    );
};
