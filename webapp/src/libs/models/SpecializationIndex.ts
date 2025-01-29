export interface ISpecializationIndex {
    id: string;
    name: string;
    label: string;
    queryType: string;
    aiSearchDeploymentId: string;
    openAIDeploymentConnection: string;
    embeddingDeployment: string;
    order: number;
}
