import { Subtitle1, makeStyles, tokens } from '@fluentui/react-components';
import { FC } from 'react';
import { PluginGallery } from '../open-api-plugins/PluginGallery';
import { UserSettingsMenu } from '../header/UserSettingsMenu';
import { AppState } from '../../App';
import logo from '../../assets/quartech-icons/logo.png';

const useStyles = makeStyles({
    header: {
        alignItems: 'center',
        backgroundColor: 'white',
        color: tokens.colorNeutralForegroundOnBrand,
        display: 'flex',
        height: '48px',
        justifyContent: 'space-between',
        width: '100%',
        padding: '0 16px',
        position: 'relative',
    },
    title: {
        flex: 1,
        textAlign: 'left',
        color: 'black',
    },
    logo: {
        maxWidth: '70%',
        maxHeight: '70%',
        position: 'absolute',
        left: '50%',
        transform: 'translateX(-50%)',
    },
    cornerItems: {
        display: 'flex',
        gap: tokens.spacingHorizontalS,
        color: 'black',
        textAlign: 'right',
    },
});

interface HeaderProps {
    appState: AppState;
    setAppState: (state: AppState) => void;
    showPluginsAndSettings: boolean;
}

const Header: FC<HeaderProps> = ({ appState, setAppState, showPluginsAndSettings }) => {
    const classes = useStyles();

    return (
        <div className={classes.header}>
            <Subtitle1 as="h1" className={classes.title}>
                Chat Copilot - Beta
            </Subtitle1>
            <img src={logo} alt="Logo" className={classes.logo} />
            {showPluginsAndSettings && appState > AppState.SettingUserInfo && (
                <div className={classes.cornerItems}>
                    <PluginGallery />
                    <UserSettingsMenu
                        setLoadingState={() => {
                            setAppState(AppState.SigningOut);
                        }}
                    />
                </div>
            )}
        </div>
    );
};

export default Header;