import { useMsal } from '@azure/msal-react';
import { useDispatch } from 'react-redux';
import { getErrorDetails } from '../../components/utils/TextUtils';
import {
    addAISearchDeployment,
    editAISearchDeployment,
    removeAISearchDeployment,
    setAISearchDeployments,
} from '../../redux/features/admin/adminSlice';
import { addAlert } from '../../redux/features/app/appSlice';
import { AuthHelper } from '../auth/AuthHelper';
import { IAISearchDeployment } from '../models/AISearchDeployment';
import { AlertType } from '../models/AlertType';
import { AISearchDeploymentService } from '../services/AISearchDeploymentService';

export const useAISearchDeployment = () => {
    const dispatch = useDispatch();
    const { instance, inProgress } = useMsal();
    const openAIDeploymentService = new AISearchDeploymentService();

    const loadAISearchDeployments = async (): Promise<IAISearchDeployment[] | undefined> => {
        try {
            const accessToken = await AuthHelper.getSKaaSAccessToken(instance, inProgress);
            const deployments = await openAIDeploymentService.getAllAISearchDeploymentsAsync(accessToken);
            dispatch(setAISearchDeployments(deployments));
            return deployments;
        } catch (e: any) {
            return undefined;
        }
    };

    const saveAISearchDeployment = async (body: IAISearchDeployment): Promise<IAISearchDeployment | undefined> => {
        try {
            const accessToken = await AuthHelper.getSKaaSAccessToken(instance, inProgress);
            const deployment = await openAIDeploymentService.createAISearchDeployment(body, accessToken);
            dispatch(addAISearchDeployment(deployment));
            dispatch(
                addAlert({
                    message: `AI Search Deployment ${deployment.name} was created successfully.`,
                    type: AlertType.Success,
                }),
            );
            return deployment;
        } catch (e: any) {
            return undefined;
        }
    };

    const updateAISearchDeployment = async (
        id: string,
        body: IAISearchDeployment,
    ): Promise<IAISearchDeployment | undefined> => {
        try {
            const accessToken = await AuthHelper.getSKaaSAccessToken(instance, inProgress);
            const deployment = await openAIDeploymentService.updateAISearchDeployment(id, body, accessToken);
            dispatch(editAISearchDeployment(deployment));
            dispatch(
                addAlert({
                    message: `AI Search Deployment ${deployment.name} was updated successfully.`,
                    type: AlertType.Success,
                }),
            );
            return deployment;
        } catch (e: any) {
            return undefined;
        }
    };

    const deleteAISearchDeployment = async (id: string): Promise<boolean> => {
        try {
            const accessToken = await AuthHelper.getSKaaSAccessToken(instance, inProgress);
            const success = await openAIDeploymentService.deleteAISearchDeployment(id, accessToken);
            dispatch(removeAISearchDeployment(id));
            dispatch(
                addAlert({
                    message: `AI Search Deployment was deleted successfully.`,
                    type: AlertType.Success,
                }),
            );
            return success;
        } catch (e: any) {
            return false;
        }
    };

    const setAISearchDeploymentOrder = async (deployments: IAISearchDeployment[]) => {
        dispatch(setAISearchDeployments(deployments));
        try {
            const accessToken = await AuthHelper.getSKaaSAccessToken(instance, inProgress);
            await openAIDeploymentService.setAISearchDeploymentOrder(deployments, accessToken);
        } catch (e: any) {
            dispatch(
                addAlert({
                    message: `Failed to swap deployment order: Details: ${getErrorDetails(e)}`,
                    type: AlertType.Error,
                }),
            );
        }
    };

    return {
        loadAISearchDeployments,
        saveAISearchDeployment,
        updateAISearchDeployment,
        deleteAISearchDeployment,
        setAISearchDeploymentOrder,
    };
};
