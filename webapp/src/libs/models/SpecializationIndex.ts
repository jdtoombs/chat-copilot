export interface ISpecializationIndex {
    id: string;
    name: string;
    label: string;
    queryType: string;
    aiSearchDeploymentConnection: string;
    openAIDeploymentConnection: string;
    embeddingDeployment: string;
    order: number;
}
