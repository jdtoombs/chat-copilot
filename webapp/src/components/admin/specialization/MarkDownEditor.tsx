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
    markdownShortcutPlugin,
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
                ref={editorRef}
                markdown={roleInformation}
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
                    codeMirrorPlugin({
                        codeBlockLanguages: {
                            js: 'JavaScript',
                            css: 'CSS',
                            txt: 'text',
                            tsx: 'TypeScript',
                            ts: 'TypeScript',
                            md: 'MarkDown',
                        },
                    }),
                    thematicBreakPlugin(),
                    markdownShortcutPlugin(),
                ]}
                onChange={(newMarkdown: string) => {
                    setRoleInformation(newMarkdown);
                }}
            />
        </div>
    );
};

export default MarkDownEditor;
