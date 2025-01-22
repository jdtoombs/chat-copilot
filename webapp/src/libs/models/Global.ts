export {};

declare global {
    interface Window {
        _env_: ENVType;
    }
}

interface ENVType {
    VITE_APP_SK_BUILD_INFO: string;
    VITE_APP_SK_VERSION: string;
    // Add New Runtime Variables here
    VITE_APP_BACKEND_URI: string;
    VITE_APP_ENVIRONMENT: string;
    // The security group the user must have in order to access the site
    SECURITY_GROUP_ID: string;
}
