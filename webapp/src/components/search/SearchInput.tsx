// Copyright (c) Microsoft. All rights reserved.
import { Button, Dropdown, makeStyles, Option, SearchBox, tokens } from '@fluentui/react-components';
import { Dismiss20Regular, SendRegular } from '@fluentui/react-icons';
import React, { useId, useState } from 'react';
import { AlertType } from '../../libs/models/AlertType';
import { useAppDispatch, useAppSelector } from '../../redux/app/hooks';
import { RootState } from '../../redux/app/store';
import { addAlert } from '../../redux/features/app/appSlice';
import { setSearch, setSelectedSearchItem } from '../../redux/features/search/searchSlice';

const useClasses = makeStyles({
    root: {
        paddingTop: tokens.spacingVerticalL,
    },
    keyWidth: {
        width: '20%',
        '& .ui-box::after': {
            transformOrigin: 'left top',
        },
    },
    inputWidth: {
        maxWidth: '70%',
        width: '70%',
        '& .ui-box::after': {
            transformOrigin: 'left top',
        },
        marginLeft: tokens.spacingHorizontalL,
        marginRight: tokens.spacingHorizontalS,
    },
    flex: {
        display: 'flex',
    },
});

interface SearchInputProps {
    onSubmit: (indexId: string, value: string) => Promise<void>;
}

interface SpecializationIndex {
    id: string;
    name: string;
}

export const SearchInput: React.FC<SearchInputProps> = ({ onSubmit }) => {
    const classes = useClasses();
    const dispatch = useAppDispatch();
    const { specializationIndexes, specializations } = useAppSelector((state: RootState) => state.admin);
    const { app } = useAppSelector((state: RootState) => state);

    const filteredSpecializations = specializations.filter((_specialization) => {
        const hasMembership =
            app.activeUserInfo?.groups.some((val) => _specialization.groupMemberships.includes(val)) ?? false;
        if (hasMembership || _specialization.groupMemberships.length === 0) {
            return _specialization;
        }
        return;
    });
    const filteredIndexes = specializationIndexes.filter((indx) =>
        filteredSpecializations.some((spc) => spc.indexId == indx.id),
    );

    const [index, setIndex] = useState<SpecializationIndex>(
        filteredIndexes.length ? { id: filteredIndexes[0].id, name: filteredIndexes[0].label } : { id: '', name: '' },
    );
    const [value, setValue] = useState('');

    const dropdownId = useId();

    const clearSearchInputState = () => {
        // setSpecialization({ key: '', name: '' });
        setValue('');
        dispatch(setSearch({ count: 0, value: [] }));
    };

    const handleSubmit = () => {
        if (value.trim() === '' || index.id.trim() === '') {
            return; // only submit if value is not empty
        }
        onSubmit(index.id, value)
            .then(() => {
                dispatch(setSelectedSearchItem({ filename: '', id: 0 }));
            })
            .catch((error) => {
                const message = `Error submitting search input: ${(error as Error).message}`;
                dispatch(
                    addAlert({
                        type: AlertType.Error,
                        message,
                    }),
                );
            });
        //clearSearchInputState();
    };

    return (
        <>
            <div className={classes.root}>
                <div className={classes.flex}>
                    <Dropdown
                        className={classes.keyWidth}
                        aria-labelledby={dropdownId}
                        placeholder="Select specialization"
                        value={index.name}
                        selectedOptions={[index.name]}
                    >
                        {filteredIndexes.map(
                            (idx) =>
                                idx.id != 'general' && (
                                    <Option
                                        key={idx.id}
                                        onClick={() => {
                                            setIndex({ id: idx.id, name: idx.label });
                                        }}
                                    >
                                        {idx.label}
                                    </Option>
                                ),
                        )}
                    </Dropdown>
                    <SearchBox
                        placeholder="Search Query"
                        className={classes.inputWidth}
                        value={value}
                        appearance="outline"
                        onChange={(_event, data) => {
                            setValue(data.value);
                        }}
                        onKeyDown={(event) => {
                            if (event.key === 'Enter' && !event.shiftKey) {
                                event.preventDefault();
                                handleSubmit();
                            }
                        }}
                        dismiss={
                            <Button
                                title="Reset"
                                aria-label="Reset Search"
                                appearance="transparent"
                                icon={<Dismiss20Regular />}
                                onClick={() => {
                                    clearSearchInputState();
                                }}
                            />
                        }
                    />
                    {/* <Input
                        
                        className={classes.inputWidth}
                        value={value}
                        onChange={(_event, data) => {
                            setValue(data.value);
                        }}
                        onKeyDown={(event) => {
                            if (event.key === 'Enter' && !event.shiftKey) {
                                event.preventDefault();
                                handleSubmit();
                            }
                        }}
                    /> */}
                    <Button
                        size="large"
                        title="Submit"
                        aria-label="Search"
                        appearance="transparent"
                        icon={<SendRegular />}
                        onClick={() => {
                            handleSubmit();
                        }}
                    />
                </div>
            </div>
        </>
    );
};
