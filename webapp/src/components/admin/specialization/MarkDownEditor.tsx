import React, { useEffect, useRef } from 'react';
import {
    MDXEditor,
    UndoRedo,
    BoldItalicUnderlineToggles,
    toolbarPlugin,
    ListsToggle,
    DiffSourceToggleWrapper,
    diffSourcePlugin,
    listsPlugin,
    codeBlockPlugin,
    tablePlugin,
    linkPlugin,
    imagePlugin,
    headingsPlugin,
    quotePlugin,
    jsxPlugin,
    codeMirrorPlugin,
    thematicBreakPlugin,
    InsertTable,
    BlockTypeSelect,
} from '@mdxeditor/editor';

interface MDXEditorMethods {
    setMarkdown: (value: string) => void;
    getMarkdown: () => string;
    insertMarkdown: (value: string) => void;
    focus: (
        callbackFn?: () => void,
        opts?: { defaultSelection?: 'rootStart' | 'rootEnd'; preventScroll?: boolean },
    ) => void;
}

interface MarkDownEditorProps {
    roleInformation: string;
    setRoleInformation: (newMarkdown: string) => void;
    id: string;
}

const MarkDownEditor: React.FC<MarkDownEditorProps> = ({ roleInformation, setRoleInformation, id }) => {
    const editorRef = useRef<MDXEditorMethods | null>(null);
    useEffect(() => {
        if (editorRef.current) {
            editorRef.current.setMarkdown(roleInformation);
        }
    }, [roleInformation]);

    return (
        <div className="specialization-manager">
            <MDXEditor
                key={id}
                ref={editorRef} // Attach the ref here
                markdown={roleInformation} // Pass roleInformation as the markdown prop
                plugins={[
                    diffSourcePlugin({
                        diffMarkdown: 'An older version',
                        viewMode: 'rich-text',
                        readOnlyDiff: true,
                    }),
                    toolbarPlugin({
                        toolbarClassName: 'my-classname',
                        toolbarContents: () => (
                            <DiffSourceToggleWrapper>
                                <UndoRedo />
                                <BoldItalicUnderlineToggles />
                                <ListsToggle />
                                <InsertTable />
                                <BlockTypeSelect />
                            </DiffSourceToggleWrapper>
                        ),
                    }),
                    listsPlugin(),
                    codeBlockPlugin(),
                    tablePlugin(),
                    linkPlugin(),
                    imagePlugin(),
                    headingsPlugin(),
                    quotePlugin(),
                    jsxPlugin(),
                    codeMirrorPlugin(),
                    thematicBreakPlugin(),
                ]}
                onChange={(newMarkdown: string) => {
                    setRoleInformation(newMarkdown); // Update the parent component state with new markdown
                }}
            />
        </div>
    );
};

export default MarkDownEditor;
