import { makeStyles } from '@fluentui/react-components';
import React from 'react';
import 'react-multi-carousel/lib/styles.css';
import { ISpecialization } from '../../libs/models/Specialization';
import { SpecializationCard } from './SpecializationCard';

const useClasses = makeStyles({
    carouselroot: {
        width: '700px',
    },
    // Allows for the carousel to be centered when there is only one item
    carouselrootSingleItem: {
        width: '700px',
        display: 'flex',
        justifyContent: 'center',
    },
    innertitle: {
        textAlign: 'center',
    },
    cardPane: {
        display: 'grid',
        gridTemplateColumns: '1fr',
        gap: '1.5rem',
        '@media screen and (min-width: 1024px)': {
            gridTemplateColumns: '1fr 1fr 1fr',
        },
        '@media screen and (min-width: 768px) and (max-width: 1023px)': {
            gridTemplateColumns: '1fr 1fr',
        },
    },
});

interface SpecializationProps {
    specializations: ISpecialization[];
}

/**
 * Renders the SpecializationCardList Carousel component.
 *
 * Note: Dynamic styling to handle the case when there is only one specialization.
 *
 * @param {{specializations: ISpecialization[]}} - List of specializations
 * @returns {*} Specialization Carousel select component
 */
export const SpecializationCardPane: React.FC<SpecializationProps> = ({ specializations }) => {
    const classes = useClasses();

    return (
        <div
            style={{
                maxWidth: '1280px',
                margin: '0 auto',
                padding: '1.5rem',
                height: '100%',
            }}
        >
            <div style={{ marginLeft: '8px', marginTop: '48px', maxWidth: '700px' }}>
                <h1 style={{ marginBottom: '16px', marginTop: '0px' }}>Choose a specialization</h1>
                <p style={{ marginBottom: '40px', marginTop: '0px', fontSize: '16px' }}>
                    Your specialization will provide Q-Pilot with domain specific knowledge to further enhance responses
                    to your questions.
                </p>
            </div>

            {specializations.length > 0 ? (
                <div className={classes.cardPane}>
                    {specializations.map((specialization) => (
                        <SpecializationCard key={specialization.id} specialization={specialization} />
                    ))}
                </div>
            ) : (
                <div>No specializations found. Please create one or contact your administrator.</div>
            )}
        </div>
    );
};
