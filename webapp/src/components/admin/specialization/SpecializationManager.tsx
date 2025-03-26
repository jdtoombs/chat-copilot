import {
    Button,
    Checkbox,
    Dropdown,
    Input,
    makeStyles,
    Option,
    OptionOnSelectData,
    SelectionEvents,
    shorthands,
    Slider,
    Tag,
    TagPicker,
    TagPickerControl,
    TagPickerGroup,
    TagPickerInput,
    TagPickerList,
    TagPickerOnOptionSelectData,
    TagPickerOption,
    Textarea,
    tokens,
    Tooltip,
} from '@fluentui/react-components';
import { Info20Regular } from '@fluentui/react-icons';
import '@mdxeditor/editor/style.css';
import React, { useCallback, useMemo, useState } from 'react';
import { useSpecialization } from '../../../libs/hooks';
import { AlertType } from '../../../libs/models/AlertType';
import { useAppDispatch, useAppSelector } from '../../../redux/app/hooks';
import { RootState } from '../../../redux/app/store';
import { addAlert } from '../../../redux/features/app/appSlice';
import { ImageUploaderPreview } from '../../files/ImageUploaderPreview';
import { ConfirmationDialog } from '../../shared/ConfirmationDialog';
import FieldArray from '../../shared/FieldArray';
import { Row } from '../../shared/Row';

import { useMsal } from '@azure/msal-react';
import { AuthHelper } from '../../../libs/auth/AuthHelper';
import { ChatMessageType } from '../../../libs/models/ChatMessage';
import { IAsk } from '../../../libs/semantic-kernel/model/Ask';
import { IAskResult } from '../../../libs/semantic-kernel/model/AskResult';
import { SingleCompletionService } from '../../../libs/services/SingleCompletionService';
import { extractJsonArray } from '../../../libs/utils/HelperMethods';
import MarkDownEditor from './MarkDownEditor';

interface ISpecializationFile {
    file: File | null;
    src: string | null;
}

const useClasses = makeStyles({
    root: {
        display: 'flex',
        flexDirection: 'column',
        ...shorthands.gap(tokens.spacingVerticalSNudge),
        ...shorthands.padding('80px'),
    },
    horizontal: {
        display: 'flex',
        ...shorthands.gap(tokens.spacingVerticalSNudge),
        alignItems: 'center',
    },
    controls: {
        display: 'flex',
        marginLeft: 'auto',
        ...shorthands.gap(tokens.spacingVerticalSNudge),
    },
    dialog: {
        maxWidth: '25%',
    },
    required: {
        color: '#990000',
    },
    scrollableContainer: {
        overflowY: 'auto',
        maxHeight: 'calc(100vh - 100px)', // Adjust this value as needed
        ...shorthands.padding('10px'),
    },
    fileUploadContainer: {
        display: 'flex',
        flexDirection: 'row',
        ...shorthands.gap(tokens.spacingHorizontalXXXL),
    },
    imageContainer: {
        display: 'flex',
        flexDirection: 'column',
        ...shorthands.gap(tokens.spacingVerticalSNudge),
    },
    slidersContainer: {
        display: 'flex',
        flexDirection: 'column',
        ...shorthands.gap(tokens.spacingVerticalSNudge),
        ...shorthands.marginInline('10px'),
    },
    slider: {
        display: 'flex',
        ...shorthands.gap(tokens.spacingVerticalSNudge),
        alignItems: 'center',
    },
    input: {
        width: '80px',
    },
    needsAttention: {
        backgroundColor: '#FFCCCB',
        flexGrow: 1,
    },
});

/**
 * Specialization Manager component.
 *
 * @returns {*}
 */
export const SpecializationManager: React.FC = () => {
    const { instance, inProgress } = useMsal();
    const classes = useClasses();
    const specialization = useSpecialization();
    const dispatch = useAppDispatch();
    const {
        specializations,
        specializationIndexes,
        openAIDeployments: chatCompletionDeployments,
        selectedId,
    } = useAppSelector((state: RootState) => state.admin);
    const defaultSpecializationRequest = useMemo(() => {
        return {
            label: '',
            name: '',
            description: '',
            roleInformation: '',
            indexId: '',
            openAIDeploymentId: '',
            completionDeploymentName: '',
            groupMemberships: [''],
            initialChatMessage: '',
            isDefault: false,
            restrictResultScope: false,
            strictness: 3,
            documentCount: 5,
            pastMessagesIncludedCount: 10,
            maxResponseTokenLimit: 1024,
            order: 0,
            suggestions: [''],
            canGenImages: false,
            enableKernelMemoryMultiIndex: false,
            indexIds: [] as string[],
        };
    }, []);

    interface FormattedOpenAIDeployment {
        id: string;
        deploymentName: string;
        completionName: string;
    }

    const completionDeploymentsFormatted = useMemo(() => {
        const formatted: FormattedOpenAIDeployment[] = [];
        chatCompletionDeployments.forEach((dep) =>
            formatted.push(
                ...dep.chatCompletionDeployments.map((comp) => ({
                    id: dep.id,
                    deploymentName: dep.name,
                    completionName: comp.name,
                })),
            ),
        );
        return formatted;
    }, [chatCompletionDeployments]);

    const [specializationRequest, setSpecializationRequest] = useState(defaultSpecializationRequest);
    const [isLoadingSuggestions, setIsLoadingSuggestions] = useState(false);
    const [isLoadingContextFormatting, setIsLoadingContextFormatting] = useState(false);
    const [editMode, setEditMode] = useState(false);
    const [id, setId] = useState('');
    const [deploymentOutputTokens, setDeploymentOutputTokens] = useState(0);
    const [imageFile, setImageFile] = useState<ISpecializationFile>({ file: null, src: null });
    const [iconFile, setIconFile] = useState<ISpecializationFile>({ file: null, src: null });
    const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
    const [saveAttempted, setSaveAttempted] = useState(false);

    const isValid =
        !!specializationRequest.label &&
        !!specializationRequest.name &&
        !!specializationRequest.roleInformation &&
        !!specializationRequest.description &&
        !!specializationRequest.initialChatMessage &&
        !!specializationRequest.openAIDeploymentId &&
        specializationRequest.groupMemberships.length > 0;

    /**
     * Save specialization by creating or updating.
     *
     * @returns {void}
     */
    const onSaveSpecialization = () => {
        specializationRequest.groupMemberships.length > 0;
        if (!isValid) {
            setSaveAttempted(true);
            dispatch(
                addAlert({
                    message: 'Please fill in all required fields.',
                    type: AlertType.Warning,
                }),
            );
            return;
        }
        setSaveAttempted(false);
        if (editMode) {
            void specialization.updateSpecialization(id, specializationRequest);
        } else {
            void specialization.createSpecialization({
                ...specializationRequest,
                order: specializations.length,
            });
        }
    };

    const resetSpecialization = useCallback(() => {
        setSpecializationRequest(defaultSpecializationRequest);

        setId('');
        setImageFile({ file: null, src: null });
        setIconFile({ file: null, src: null });
        setDeploymentOutputTokens(4096);
    }, [defaultSpecializationRequest]);

    useMemo(() => {
        if (selectedId != '') {
            setEditMode(true);
            const specializationObj = specializations.find((specialization) => specialization.id === selectedId);
            if (specializationObj) {
                setSpecializationRequest({
                    label: specializationObj.label,
                    name: specializationObj.name,
                    description: specializationObj.description,
                    roleInformation: specializationObj.roleInformation,
                    indexId: specializationObj.indexId,
                    openAIDeploymentId: specializationObj.openAIDeploymentId,
                    completionDeploymentName: specializationObj.completionDeploymentName,
                    groupMemberships: specializationObj.groupMemberships,
                    initialChatMessage: specializationObj.initialChatMessage,
                    isDefault: specializationObj.isDefault,
                    restrictResultScope: specializationObj.restrictResultScope ?? false,
                    strictness: specializationObj.strictness ?? 3,
                    documentCount: specializationObj.documentCount ?? 5,
                    pastMessagesIncludedCount: specializationObj.pastMessagesIncludedCount ?? 10,
                    maxResponseTokenLimit: specializationObj.maxResponseTokenLimit ?? 1024,
                    order: specializationObj.order,
                    suggestions: specializationObj.suggestions,
                    canGenImages: specializationObj.canGenImages,
                    enableKernelMemoryMultiIndex: specializationObj.enableKernelMemoryMultiIndex,
                    indexIds: specializationObj.indexIds ?? [],
                });
                /**
                 * Set the image and icon file paths
                 * Note: The file is set to null because we only retrieve the file path from the server
                 */
                setImageFile({ file: null, src: specializationObj.imageFilePath });
                setIconFile({ file: null, src: specializationObj.iconFilePath });
                setId(specializationObj.id);
                setDeploymentOutputTokens(
                    chatCompletionDeployments
                        .find((d) => d.id === specializationObj.openAIDeploymentId)
                        ?.chatCompletionDeployments.find((a) => a.name === specializationObj.completionDeploymentName)
                        ?.outputTokens ?? 4096,
                );
            }
        } else {
            setEditMode(false);
            resetSpecialization();
        }
    }, [resetSpecialization, selectedId, specializations, chatCompletionDeployments]);

    const onDeleteSpecialization = () => {
        setIsDeleteDialogOpen(true);
    };

    const confirmDelete = () => {
        if (specializationRequest.isDefault) {
            dispatch(
                addAlert({
                    message: 'Please set another specialization as default before deleting this one.',
                    type: AlertType.Warning,
                }),
            );
            setIsDeleteDialogOpen(false);
            return;
        }
        void specialization.deleteSpecialization(id, specializationRequest.name);
        resetSpecialization();
        setIsDeleteDialogOpen(false);
    };

    const handleChange = (event: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        const value = {
            checkbox: (event as React.ChangeEvent<HTMLInputElement>).target.checked,
            number: parseInt(event.target.value),
            text: event.target.value,
            textarea: event.target.value,
            range: event.target.value,
        }[event.target.type];

        setSpecializationRequest({
            ...specializationRequest,
            [event.target.name]: value,
        });
    };

    const handleTagPickerChange = (_event: Event | React.SyntheticEvent, data: TagPickerOnOptionSelectData) => {
        if (data.value === 'no-options') {
            return;
        }
        setSpecializationRequest({
            ...specializationRequest,
            indexIds: data.selectedOptions,
        });
    };

    const tagPickerOptions = specializationIndexes.filter(
        (indx) => indx.name.includes('kernelmemory') && !specializationRequest.indexIds.includes(indx.id),
    );

    const handleRoleInformationChange = (roleInformation: string) => {
        setSpecializationRequest({
            ...specializationRequest,
            roleInformation: roleInformation,
        });
    };

    const onChangeIndexName = (_event?: SelectionEvents, data?: OptionOnSelectData) => {
        setSpecializationRequest({
            ...specializationRequest,
            indexId: data?.optionValue ?? '',
        });
    };

    const onDeploymentChange = (_event: SelectionEvents, data: OptionOnSelectData) => {
        const obj = JSON.parse(data.optionValue ?? '{}') as unknown as FormattedOpenAIDeployment;

        setSpecializationRequest({
            ...specializationRequest,
            openAIDeploymentId: obj.id,
            completionDeploymentName: obj.completionName,
        });

        const outputTokens = chatCompletionDeployments
            .find((d) => d.id === obj.id)
            ?.chatCompletionDeployments.find((a) => a.name === obj.deploymentName)?.outputTokens;
        setDeploymentOutputTokens(outputTokens ?? 4096);

        if (
            specializationRequest.maxResponseTokenLimit &&
            outputTokens &&
            specializationRequest.maxResponseTokenLimit > outputTokens
        ) {
            setSpecializationRequest({
                ...specializationRequest,
                maxResponseTokenLimit: outputTokens,
            });
        }
    };

    const determineIfNeedsAttention = useCallback(
        (value: string | string[]) => {
            // friendly reminder that !![] === true
            let needsAttention = false;
            if (typeof value === 'string') needsAttention = !value && saveAttempted;
            if (Array.isArray(value))
                needsAttention = (!value.length && saveAttempted) || (value.length === 1 && !value[0] && saveAttempted);

            return needsAttention ? classes.needsAttention : '';
        },
        [classes.needsAttention, saveAttempted],
    );

    const getMarkdown = async (): Promise<IAskResult> => {
        const markDownPrompt = `
            Take the provided text, which serves as instructions about the role behavior of a chat model,
            and convert it into a well-structured Markdown document.
            Use appropriate titles and paragraphs to organize the information in a way that is clear and easy
            for the chatbot to interpret and apply. IMPORTANT: REPLY ONLY WITH THE UPDATED MARKDOWN DO NOT WRITE ANYTHING ELSE IN YOUR RESPONSE.
            HERE IS THE INPUT TEXT for you to convert:

            ${specializationRequest.roleInformation}
        `;
        //Configure ask object for service function request
        const ask: IAsk = {
            input: markDownPrompt,
            variables: [
                {
                    key: 'messageType',
                    value: ChatMessageType.Message.toString(),
                },
            ],
        };
        //passing ask object, auth token, and plugins
        const noChatMessageService = new SingleCompletionService();
        const authToken = await AuthHelper.getSKaaSAccessToken(instance, inProgress);
        return noChatMessageService.getBotResponseNoChat(ask, authToken);
    };

    const getAutoSuggestions = async (): Promise<IAskResult> => {
        const autoSpecPrompt =
            'Provide four topics related to the current dataset, format them as questions, and return the response as a JSON array.';
        const ask: IAsk = {
            input: autoSpecPrompt,
        };
        const noChatMessageService = new SingleCompletionService();
        const authToken = await AuthHelper.getSKaaSAccessToken(instance, inProgress);
        return noChatMessageService.getBotResponseNoChat(ask, authToken, selectedId);
    };

    const handleClickAutoSuggestions = () => {
        setIsLoadingSuggestions(true);
        getAutoSuggestions()
            .then((response) => {
                let arraySuggestions = extractJsonArray(response.value); //First try to convert from the raw string.
                if (!arraySuggestions.length) {
                    //Sometimes the bot will reply with other text and json wrapped in ```json ... ```
                    //so we can try that if the first attempt didn't give us anything.
                    const regex = /```json\s*(\[[\s\S]*?\])\s*```/g;
                    const match = regex.exec(response.value);
                    if (match) {
                        arraySuggestions = extractJsonArray(match[1]);
                    }
                }
                setSpecializationRequest({
                    ...specializationRequest,
                    suggestions: arraySuggestions,
                });
            })
            .catch(() => {
                console.error('Suggestions retrieval failed.');
            })
            .finally(() => {
                setIsLoadingSuggestions(false);
            });
    };

    const handleClickFormatWithAI = () => {
        setIsLoadingContextFormatting(true);
        getMarkdown()
            .then((response) => {
                setSpecializationRequest({
                    ...specializationRequest,
                    roleInformation: response.value,
                });
            })
            .catch((e: Error) => {
                console.error(`Could not retrieve markdown from completions service. ${e.message}`);
            })
            .finally(() => {
                setIsLoadingContextFormatting(false);
            });
    };

    return (
        <div className={classes.scrollableContainer}>
            <div className={classes.root}>
                <div className={classes.horizontal}></div>
                <label htmlFor="name">
                    Name<span className={classes.required}>*</span>
                </label>
                <Input
                    id="name"
                    name="name"
                    required
                    value={specializationRequest.name}
                    className={determineIfNeedsAttention(specializationRequest.name)}
                    onChange={handleChange}
                />
                <label htmlFor="label">
                    Label<span className={classes.required}>*</span>
                </label>
                <Input
                    id="label"
                    name="label"
                    required
                    className={determineIfNeedsAttention(specializationRequest.label)}
                    value={specializationRequest.label}
                    onChange={handleChange}
                />
                <label htmlFor="deployment">
                    Deployment<span className={classes.required}>*</span>
                </label>
                <Dropdown
                    clearable
                    id="deployment"
                    className={determineIfNeedsAttention(specializationRequest.openAIDeploymentId)}
                    onOptionSelect={onDeploymentChange}
                    value={`${specializationRequest.completionDeploymentName} (${chatCompletionDeployments.find((a) => a.id === specializationRequest.openAIDeploymentId)?.name})`}
                >
                    {completionDeploymentsFormatted.map((deployment) => (
                        <Option key={deployment.id} value={JSON.stringify(deployment)}>
                            {`${deployment.completionName} (${deployment.deploymentName})`}
                        </Option>
                    ))}
                </Dropdown>
                <label htmlFor="index-name">Enrichment Index</label>
                <Dropdown
                    clearable
                    id="index-name"
                    onOptionSelect={onChangeIndexName}
                    value={
                        specializationIndexes.find((index) => index.id === specializationRequest.indexId)?.name ??
                        'None'
                    }
                    disabled={specializationRequest.enableKernelMemoryMultiIndex}
                >
                    <Option value="">None</Option>
                    {specializationIndexes.map((specializationIndex) => (
                        <Option
                            value={specializationIndex.id}
                            key={specializationIndex.id}
                            text={specializationIndex.name}
                        >
                            {specializationIndex.name}
                        </Option>
                    ))}
                </Dropdown>
                <Checkbox
                    name="enableKernelMemoryMultiIndex"
                    label="Enable Kernel Memory Multi-index (Experimental)"
                    checked={specializationRequest.enableKernelMemoryMultiIndex}
                    onChange={handleChange}
                />
                {specializationRequest.enableKernelMemoryMultiIndex && (
                    <TagPicker onOptionSelect={handleTagPickerChange} selectedOptions={specializationRequest.indexIds}>
                        <TagPickerControl>
                            <TagPickerGroup aria-label="Selected Indexes">
                                {specializationRequest.indexIds.map((option) => (
                                    <Tag key={option} shape="rounded" value={option}>
                                        {specializationIndexes.find((a) => a.id == option)?.label}
                                    </Tag>
                                ))}
                            </TagPickerGroup>
                            <TagPickerInput aria-label="Select Indexes" />
                        </TagPickerControl>
                        <TagPickerList>
                            {tagPickerOptions.length > 0 ? (
                                tagPickerOptions.map((option) => (
                                    <TagPickerOption value={option.id} key={option.id}>
                                        {option.label}
                                    </TagPickerOption>
                                ))
                            ) : (
                                <TagPickerOption value="no-options">No options available</TagPickerOption>
                            )}
                        </TagPickerList>
                    </TagPicker>
                )}
                <Row>
                    <Checkbox
                        name="isDefault"
                        label="Set as Default Specialization"
                        checked={specializationRequest.isDefault}
                        onChange={handleChange}
                    />
                    <Checkbox
                        name="canGenImages"
                        label="Can Generate Images"
                        checked={specializationRequest.canGenImages}
                        onChange={handleChange}
                    />
                </Row>
                <ConfirmationDialog
                    open={isDeleteDialogOpen}
                    title="Delete Specialization"
                    content={`Are you sure you want to delete the ${specializationRequest.name} specialization?`}
                    confirmLabel="Delete"
                    cancelLabel="Cancel"
                    onConfirm={confirmDelete}
                    onCancel={() => {
                        setIsDeleteDialogOpen(false);
                    }}
                />
                {specializationRequest.indexId && (
                    <>
                        <div>
                            <Checkbox
                                name="restrictResultScope"
                                label="Limit responses to your data content"
                                checked={specializationRequest.restrictResultScope}
                                onChange={handleChange}
                            />
                            <Tooltip
                                content={'Enabling this will limit responses specific to your data content'}
                                relationship="label"
                            >
                                <Button icon={<Info20Regular />} appearance="transparent" />
                            </Tooltip>
                        </div>
                        <div className={classes.slidersContainer}>
                            <label htmlFor="strictness">Strictness (1-5)</label>
                            <div className={classes.slider}>
                                <Slider
                                    id="strictness"
                                    name="strictness"
                                    min={1}
                                    max={5}
                                    value={specializationRequest.strictness}
                                    onChange={handleChange}
                                />
                                <Input
                                    name="strictness"
                                    value={specializationRequest.strictness.toString()}
                                    onChange={handleChange}
                                    type="number"
                                    min={1}
                                    max={5}
                                    className={classes.input}
                                ></Input>
                                <Tooltip
                                    content={
                                        'Strictness sets the threshold to categorize documents as relevant to your queries. Raising strictness means a higher threshold for relevance and filtering out more documents that are less relevant for responses. Very high strictness could cause failure to generate responses due to limited available documents. The default strictness is 3.'
                                    }
                                    relationship="label"
                                >
                                    <Button icon={<Info20Regular />} appearance="transparent" />
                                </Tooltip>
                            </div>
                            <label htmlFor="documentCount">Retrieved Documents (3-20)</label>
                            <div className={classes.slider}>
                                <Slider
                                    id="documentCount"
                                    name="documentCount"
                                    min={3}
                                    max={20}
                                    value={specializationRequest.documentCount}
                                    onChange={handleChange}
                                />
                                <Input
                                    name="documentCount"
                                    value={specializationRequest.documentCount.toString()}
                                    onChange={handleChange}
                                    type="number"
                                    min={3}
                                    max={20}
                                    className={classes.input}
                                ></Input>
                                <Tooltip
                                    content={
                                        'This specifies the number of top-scoring documents from your data index used to generate responses. You want to increase the value when you have short documents or want to provide more context. The default value is 5. Note: if you set the value to 20 but only have 10 documents in your index, only 10 will be used.'
                                    }
                                    relationship="label"
                                >
                                    <Button icon={<Info20Regular />} appearance="transparent" />
                                </Tooltip>
                            </div>
                            <label htmlFor="maxResponse">Past messages included (1-100)</label>
                            <div id="maxResponse" className={classes.slider}>
                                <Slider
                                    name="pastMessagesIncludedCount"
                                    min={1}
                                    max={100}
                                    value={specializationRequest.pastMessagesIncludedCount}
                                    onChange={handleChange}
                                />
                                <Input
                                    name="pastMessagesIncludedCount"
                                    value={specializationRequest.pastMessagesIncludedCount.toString()}
                                    onChange={handleChange}
                                    type="number"
                                    min={1}
                                    max={100}
                                    className={classes.input}
                                ></Input>
                                <Tooltip
                                    content={
                                        'Select the number of past messages to include in each new API request. This helps give the model context for new user queries. Setting this number to 10 will include 5 user queries and 5 system responses.'
                                    }
                                    relationship="label"
                                >
                                    <Button icon={<Info20Regular />} appearance="transparent" />
                                </Tooltip>
                            </div>
                            <label htmlFor="maxResponse">Max Response (1-{deploymentOutputTokens})</label>
                            <div id="maxResponse" className={classes.slider}>
                                <Slider
                                    name="maxResponseTokenLimit"
                                    min={1}
                                    max={deploymentOutputTokens}
                                    value={specializationRequest.maxResponseTokenLimit}
                                    onChange={handleChange}
                                />
                                <Input
                                    name="maxResponseTokenLimit"
                                    value={specializationRequest.maxResponseTokenLimit.toString()}
                                    onChange={handleChange}
                                    type="number"
                                    min={1}
                                    max={deploymentOutputTokens}
                                    className={classes.input}
                                ></Input>
                                <Tooltip
                                    content={
                                        "Set a limit on the number of tokens per model response. The supported number of tokens are shared between the prompt (including system message, examples, message history, and user query) and the model's response. One token is roughly 4 characters for typical English text."
                                    }
                                    relationship="label"
                                >
                                    <Button icon={<Info20Regular />} appearance="transparent" />
                                </Tooltip>
                            </div>
                        </div>
                    </>
                )}
                <label htmlFor="description">
                    Short Description<span className={classes.required}>*</span>
                </label>
                <Textarea
                    id="description"
                    name="description"
                    required
                    resize="vertical"
                    value={specializationRequest.description}
                    className={determineIfNeedsAttention(specializationRequest.description)}
                    rows={2}
                    onChange={handleChange}
                />
                <div style={{ display: 'flex', alignItems: 'center' }}>
                    <label htmlFor="context" style={{ marginRight: '10px' }}>
                        Chat Context<span style={{ color: 'red' }}>*</span>
                    </label>
                    <label
                        onClick={isLoadingContextFormatting ? undefined : handleClickFormatWithAI}
                        style={{
                            cursor: 'pointer',
                            color: 'blue',
                            marginLeft: '10px',
                            textDecoration: 'underline',
                        }}
                    >
                        {isLoadingContextFormatting ? 'Loading...' : `(Format With AI)`}
                    </label>
                </div>

                <div
                    style={{
                        resize: 'vertical',
                        overflow: 'auto',
                        minHeight: '150px',
                        maxHeight: '500px',
                        width: '100%',
                        backgroundColor: 'white',
                        border: '1px',
                        borderBottom: '1px solid black',
                    }}
                >
                    <div
                        className={determineIfNeedsAttention(specializationRequest.roleInformation)}
                        style={{
                            minHeight: '150px',
                            backgroundColor: 'white',
                        }}
                    >
                        <MarkDownEditor
                            markdown={specializationRequest.roleInformation}
                            onChange={handleRoleInformationChange}
                        />
                    </div>
                </div>

                <label htmlFor="initialMessage">
                    Initial Chat Message<span className={classes.required}>*</span>
                </label>
                <Textarea
                    id="initialMessage"
                    name="initialChatMessage"
                    required
                    resize="vertical"
                    value={specializationRequest.initialChatMessage}
                    className={determineIfNeedsAttention(specializationRequest.initialChatMessage)}
                    rows={2}
                    onChange={handleChange}
                />
                <div style={{ display: 'flex', alignItems: 'center' }}>
                    <label htmlFor="initialMessage">
                        Initial Chat Suggestions<span className={classes.required}>*</span>
                    </label>
                    <label
                        onClick={isLoadingSuggestions ? undefined : handleClickAutoSuggestions}
                        style={{
                            cursor: 'pointer',
                            color: 'blue',
                            marginLeft: '10px',
                            textDecoration: 'underline',
                        }}
                    >
                        {isLoadingSuggestions ? 'Loading...' : `(Create With AI)`}
                    </label>
                </div>
                <FieldArray
                    values={specializationRequest.suggestions}
                    maxItems={4}
                    className={determineIfNeedsAttention(specializationRequest.suggestions)}
                    onFieldChanged={(index, newValue) => {
                        const values = specializationRequest.suggestions.slice(0);
                        values[index] = newValue;
                        setSpecializationRequest({
                            ...specializationRequest,
                            suggestions: values,
                        });
                    }}
                    onFieldAdded={() => {
                        const values = specializationRequest.suggestions.slice(0);
                        values.push('');
                        setSpecializationRequest({
                            ...specializationRequest,
                            suggestions: values,
                        });
                    }}
                    onFieldRemoved={(index) => {
                        setSpecializationRequest({
                            ...specializationRequest,
                            suggestions: specializationRequest.suggestions
                                .slice(0, index)
                                .concat(specializationRequest.suggestions.slice(index + 1)),
                        });
                    }}
                />
                <label htmlFor="membership">
                    Entra Membership IDs<span className={classes.required}>*</span>
                </label>
                <Input
                    id="membership"
                    className={determineIfNeedsAttention(specializationRequest.groupMemberships)}
                    required
                    value={specializationRequest.groupMemberships.join(', ')}
                    onChange={(_event, data) => {
                        setSpecializationRequest({
                            ...specializationRequest,
                            groupMemberships: data.value.split(', '),
                        });
                    }}
                />
                {editMode && (
                    <div className={classes.fileUploadContainer}>
                        <div className={classes.imageContainer}>
                            <label>Specialization Image</label>
                            <ImageUploaderPreview
                                buttonLabel="Upload Image"
                                file={imageFile.file ?? imageFile.src}
                                onFileUpdate={(file, src) => {
                                    setImageFile({ file, src });

                                    file
                                        ? void specialization.updateImage(id, file)
                                        : void specialization.deleteImage(id);
                                }}
                            />
                        </div>
                        <div className={classes.imageContainer}>
                            <label>Specialization Icon</label>
                            <ImageUploaderPreview
                                buttonLabel="Upload Icon"
                                file={iconFile.file ?? iconFile.src}
                                onFileUpdate={(file, src) => {
                                    // Set the src to null if the file is falsy ie: '' or null
                                    setIconFile({ file, src: src || null });

                                    file
                                        ? void specialization.updateIcon(id, file)
                                        : void specialization.deleteIcon(id);
                                }}
                            />
                        </div>
                    </div>
                )}
                <div className={classes.controls}>
                    <Button appearance="secondary" disabled={!id} onClick={onDeleteSpecialization}>
                        Delete
                    </Button>

                    <Button appearance="primary" onClick={onSaveSpecialization}>
                        Save
                    </Button>
                </div>
            </div>
        </div>
    );
};
