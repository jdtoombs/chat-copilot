import { IOpenAIDeployment } from '../models/OpenAIDeployment';
import { ISpecialization, ISpecializationRequest, ISpecializationToggleRequest } from '../models/Specialization';
import { BaseService } from './BaseService';

export class SpecializationService extends BaseService {
    public getAllSpecializationsAsync = async (accessToken: string): Promise<ISpecialization[]> => {
        const result = await this.getResponseAsync<ISpecialization[]>(
            {
                commandPath: 'specializations',
                method: 'GET',
            },
            accessToken,
        );
        return result;
    };

    public getAllSpecializationIndexesAsync = async (accessToken: string): Promise<string[]> => {
        const result = await this.getResponseAsync<string[]>(
            {
                commandPath: 'specialization/indexes',
                method: 'GET',
            },
            accessToken,
        );
        return result;
    };

    public getAllChatCompletionDeploymentsAsync = async (accessToken: string): Promise<IOpenAIDeployment[]> => {
        const result = await this.getResponseAsync<IOpenAIDeployment[]>(
            {
                commandPath: 'openAIDeployments',
                method: 'GET',
            },
            accessToken,
        );
        return result;
    };

    /**
     * Create specialization.
     *
     * Note: The backend endpoint expects FormData which only accepts string values.
     *
     * @async
     * @param {ISpecializationRequest} body - The specialization request body.
     * @param {string} accessToken
     * @returns {Promise<ISpecialization>}
     */
    public createSpecializationAsync = async (
        body: ISpecializationRequest,
        accessToken: string,
    ): Promise<ISpecialization> => {
        const result = await this.getResponseAsync<ISpecialization>(
            {
                commandPath: 'specializations',
                method: 'POST',
                body,
            },
            accessToken,
        );
        return result;
    };

    public updateSpecializationImage = async (
        specializationId: string,
        image: File,
        accessToken: string,
    ): Promise<void> => {
        const formData = new FormData();
        formData.append('image', image);

        await this.getResponseAsync<ISpecialization>(
            {
                commandPath: `specializations/${specializationId}/image`,
                method: 'PATCH',
                body: formData,
            },
            accessToken,
        );
    };

    public deleteSpecializationImage = async (specializationId: string, accessToken: string): Promise<void> => {
        await this.getResponseAsync<ISpecialization>(
            {
                commandPath: `specializations/${specializationId}/image`,
                method: 'DELETE',
            },
            accessToken,
        );
    };

    public updateSpecializationIcon = async (
        specializationId: string,
        icon: File,
        accessToken: string,
    ): Promise<void> => {
        const formData = new FormData();
        formData.append('icon', icon);

        await this.getResponseAsync<ISpecialization>(
            {
                commandPath: `specializations/${specializationId}/icon`,
                method: 'PATCH',
                body: formData,
            },
            accessToken,
        );
    };

    public deleteSpecializationIcon = async (specializationId: string, accessToken: string): Promise<void> => {
        await this.getResponseAsync<ISpecialization>(
            {
                commandPath: `specializations/${specializationId}/icon`,
                method: 'DELETE',
            },
            accessToken,
        );
    };

    /**
     * Update specialization.
     *
     * Note: The backend endpoint expects FormData which only accepts string values.
     *
     * @async
     * @param {string} specializationId
     * @param {ISpecializationRequest} specialization - Specialization request body.
     * @param {string} accessToken
     * @returns {Promise<ISpecialization>}
     */
    public updateSpecializationAsync = async (
        specializationId: string,
        specialization: ISpecializationRequest,
        accessToken: string,
    ): Promise<ISpecialization> => {
        const body = [];

        body.push({ op: 'replace', path: '/label', value: specialization.label });
        body.push({ op: 'replace', path: '/name', value: specialization.name });
        body.push({ op: 'replace', path: '/description', value: specialization.description });
        body.push({ op: 'replace', path: '/roleInformation', value: specialization.roleInformation });
        body.push({ op: 'replace', path: '/openAIDeploymentId', value: specialization.openAIDeploymentId });
        body.push({ op: 'replace', path: '/completionDeploymentName', value: specialization.completionDeploymentName });
        body.push({ op: 'replace', path: '/initialChatMessage', value: specialization.initialChatMessage });
        body.push({ op: 'replace', path: '/indexId', value: specialization.indexId });
        body.push({ op: 'replace', path: '/groupMemberships', value: specialization.groupMemberships });
        body.push({ op: 'replace', path: '/order', value: specialization.order.toString() });
        body.push({ op: 'replace', path: '/isDefault', value: specialization.isDefault.toString() });
        body.push({ op: 'replace', path: '/suggestions', value: specialization.suggestions });
        body.push({ op: 'replace', path: '/canGenImages', value: specialization.canGenImages.toString() });

        if (specialization.restrictResultScope != null) {
            body.push({
                op: 'replace',
                path: '/restrictResultScope',
                value: specialization.restrictResultScope.toString(),
            });
        }
        if (specialization.strictness) {
            body.push({ op: 'replace', path: '/strictness', value: specialization.strictness.toString() });
        }
        if (specialization.documentCount) {
            body.push({ op: 'replace', path: '/documentCount', value: specialization.documentCount.toString() });
        }
        if (specialization.pastMessagesIncludedCount) {
            body.push({
                op: 'replace',
                path: '/pastMessagesIncludedCount',
                value: specialization.pastMessagesIncludedCount.toString(),
            });
        }
        if (specialization.maxResponseTokenLimit) {
            body.push({
                op: 'replace',
                path: '/maxResponseTokenLimit',
                value: specialization.maxResponseTokenLimit.toString(),
            });
        }

        const result = await this.getResponseAsync<ISpecialization>(
            {
                commandPath: `specializations/${specializationId}`,
                method: 'PATCH',
                body: body,
            },
            accessToken,
        );
        return result;
    };

    /**
     * Toggle specialization on or off.
     *
     * Note: The backend endpoint expects FormData which only accepts string values.
     *
     * @async
     * @param {string} specializationId
     * @param {boolean} isActive - Is the specialization active?
     * @param {string} accessToken
     * @returns {Promise<ISpecialization>}
     */
    public onOffSpecializationAsync = async (
        specializationId: string,
        request: ISpecializationToggleRequest,
        accessToken: string,
    ): Promise<ISpecialization> => {
        const formData = new FormData();

        formData.append('isActive', request.isActive.toString());

        const result = await this.getResponseAsync<ISpecialization>(
            {
                commandPath: `specializations/${specializationId}`,
                method: 'PATCH',
                body: formData,
            },
            accessToken,
        );
        return result;
    };

    public deleteSpecializationAsync = async (specializationId: string, accessToken: string): Promise<object> => {
        const result = await this.getResponseAsync<object>(
            {
                commandPath: `specializations/${specializationId}`,
                method: 'DELETE',
            },
            accessToken,
        );

        return result;
    };

    /**
     * Sets the order of specializations on the server by converting an array of specializations into a format
     * suitable for the backend API, then posts this data to update the specialization order.
     *
     * @param {ISpecialization[]} body - An array of specializations where each object includes an `id` and an `order`.
     * @param {string} accessToken - The access token for authentication with the API.
     * @returns {Promise<void>} A promise that resolves when the order has been successfully updated or rejects with an error.
     * @throws Will throw an error if the API request fails or if there's an issue during data conversion.
     */
    async setSpecializationsOrder(body: ISpecialization[], accessToken: string): Promise<void> {
        const specializationOrder = {
            ordering: body.reduce<Record<string, number>>((acc, specialization) => {
                acc[specialization.id] = specialization.order;
                return acc;
            }, {}),
        };

        await this.getResponseAsync(
            {
                commandPath: `specializations/order`,
                method: 'POST',
                body: specializationOrder,
            },
            accessToken,
        );
    }
}
