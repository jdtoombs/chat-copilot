import { IAISearchDeployment } from '../models/AISearchDeployment';
import { BaseService } from './BaseService';

export class AISearchDeploymentService extends BaseService {
    public getAllAISearchDeploymentsAsync = async (accessToken: string): Promise<IAISearchDeployment[]> => {
        const result = await this.getResponseAsync<IAISearchDeployment[]>(
            { commandPath: 'aiSearchDeployments', method: 'GET' },
            accessToken,
        );
        return result;
    };

    public createAISearchDeployment = async (
        body: IAISearchDeployment,
        accessToken: string,
    ): Promise<IAISearchDeployment> => {
        const formData = new FormData();
        formData.append('name', body.name);
        formData.append('endpoint', body.endpoint);
        formData.append('secretName', body.secretName);
        formData.append('label', body.label);

        const result = await this.getResponseAsync<IAISearchDeployment>(
            { commandPath: 'aiSearchDeployments', method: 'POST', body: formData },
            accessToken,
        );
        return result;
    };

    public updateAISearchDeployment = async (id: string, body: IAISearchDeployment, accessToken: string) => {
        const formData = new FormData();
        formData.append('name', body.name);
        formData.append('endpoint', body.endpoint);
        formData.append('secretName', body.secretName);
        formData.append('label', body.label);

        const result = await this.getResponseAsync<IAISearchDeployment>(
            { commandPath: `aiSearchDeployments/${id}`, method: 'PATCH', body: formData },
            accessToken,
        );
        return result;
    };

    public deleteAISearchDeployment = async (id: string, accessToken: string) => {
        const result = await this.getResponseAsync<boolean>(
            { commandPath: `aiSearchDeployments/${id}`, method: 'DELETE' },
            accessToken,
        );
        return result;
    };

    public setAISearchDeploymentOrder = async (body: IAISearchDeployment[], accessToken: string): Promise<void> => {
        const deploymentsOrder = {
            ordering: body.reduce<Record<string, number>>((acc, index) => {
                acc[index.id] = index.order;
                return acc;
            }, {}),
        };

        await this.getResponseAsync(
            {
                commandPath: `aiSearchDeployments/order`,
                method: 'POST',
                body: deploymentsOrder,
            },
            accessToken,
        );
    };
}
