export interface ISpecialization {
    id: string;
    label: string;
    name: string;
    description: string;
    roleInformation: string;
    indexId: string;
    openAIDeploymentId: string;
    completionDeploymentName: string;
    imageFilePath: string;
    iconFilePath: string;
    isActive: boolean;
    groupMemberships: string[];
    isDefault: boolean;
    restrictResultScope: boolean | null;
    strictness: number | null;
    documentCount: number | null;
    pastMessagesIncludedCount: number | null;
    maxResponseTokenLimit: number | null;
    initialChatMessage: string;
    order: number;
    suggestions: string[];
    canGenImages: boolean;
    enableKernelMemoryMultiIndex: boolean;
    indexIds?: string[];
}

/**
 * Specialization request interface.
 *
 */
export interface ISpecializationRequest {
    label: string;
    name: string;
    description: string;
    roleInformation: string;
    indexId: string;
    openAIDeploymentId: string;
    completionDeploymentName: string;
    groupMemberships: string[];
    initialChatMessage: string;
    isDefault: boolean;
    restrictResultScope: boolean | null;
    strictness: number | null;
    documentCount: number | null;
    pastMessagesIncludedCount: number | null;
    maxResponseTokenLimit: number | null;
    order: number;
    suggestions: string[];
    canGenImages: boolean;
    enableKernelMemoryMultiIndex: boolean;
    indexIds?: string[];
}

export interface ISpecializationToggleRequest {
    isActive: boolean;
}

export interface IChatCompletionDeployment {
    name: string;
    completionTokenLimit: number;
    outputTokens: number;
}
