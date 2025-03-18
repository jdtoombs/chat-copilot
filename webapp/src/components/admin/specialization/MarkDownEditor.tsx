import React, { useRef } from 'react';
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
    type MDXEditorMethods,
    type MDXEditorProps,
    markdownShortcutPlugin,
} from '@mdxeditor/editor';

const MarkDownEditor: React.FC<MDXEditorProps> = ({ markdown, onChange }) => {
    const ref = useRef<MDXEditorMethods>(null);
    ref.current?.setMarkdown(markdown);

    return (
        <div>
            <MDXEditor
                ref={ref}
                markdown=""
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
                onChange={onChange}
            />
        </div>
    );
};

export default MarkDownEditor;
