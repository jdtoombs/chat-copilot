# Q-Pilot Application

The project is built on Microsoft [Semantic Kernel](https://github.com/microsoft/semantic-kernel) and has three components:

1. A frontend application [React web app](./webapp/)
2. A backend REST API [.NET web API service](./webapi/)
3. A [.NET worker service](./memorypipeline/) for processing semantic memory.

TODO: Update

# Requirements

You will need the following items to run the application:

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js](https://nodejs.org/en/download)
- [Yarn](https://classic.yarnpkg.com/docs/install)
- [Git](https://www.git-scm.com/downloads)

# Instructions

TODO: Update

## Windows

1. Open PowerShell

TODO: Update

1. Configure Q-Pilot.

TODO: Update

1. Run Q-Pilot locally.

TODO: Update

## Linux/macOS

TODO: Update
     
## Configuring SharePoint Indexer

To configure the SharePoint Indexer, I followed the documentation [here](https://learn.microsoft.com/en-us/azure/search/search-howto-index-sharepoint-online). You will have to read this document to get famiiar with everything thats happening with the sharepoint indexer. As per the tweaks I did to make ours work and our current configurations for the current indexer, I talk about it in detail in the section referenced [here](./sharepoint-indexer/README.md).

## Enable backend authentication via Azure AD

### Requirements

- [Azure account]
- [Azure AD Tenant]

### Instructions

1. Create an [application registration](https://learn.microsoft.com/azure/active-directory/develop/quickstart-register-app) for the frontend web app, using the values below

   - `Supported account types`: "_Accounts in this organizational directory only ({YOUR TENANT} only - Single tenant)_"
   - `Redirect URI (optional)`: _Single-page application (SPA)_ and use _http://localhost:3000_.

2. Create a second [application registration](https://learn.microsoft.com/azure/active-directory/develop/quickstart-register-app) for the backend web api, using the values below:
   - `Supported account types`: "_Accounts in this organizational directory only ({YOUR TENANT} only - Single tenant)_"
   - Do **not** configure a `Redirect URI (optional)`

> Take note of the `Application (client) ID` for both app registrations as you will need them in future steps.

3. Expose an API within the second app registration

   1. Select _Expose an API_ from the menu

   2. Add an _Application ID URI_

      1. This will generate an `api://` URI

      2. Click _Save_ to store the generated URI

   3. Add a scope for `access_as_user`

      1. Click _Add scope_

      2. Set _Scope name_ to `access_as_user`

      3. Set _Who can consent_ to _Admins and users_

      4. Set _Admin consent display name_ and _User consent display name_ to `Access copilot chat as a user`

      5. Set _Admin consent description_ and _User consent description_ to `Allows the accesses to the Copilot chat web API as a user`

   4. Add the web app frontend as an authorized client application

      1. Click _Add a client application_

      2. For _Client ID_, enter the frontend's application (client) ID

      3. Check the checkbox under _Authorized scopes_

      4. Click _Add application_

4. Add permissions to web app frontend to access web api as user

   1. Open app registration for web app frontend

   2. Go to _API Permissions_

   3. Click _Add a permission_

   4. Select the tab _APIs my organization uses_

   5. Choose the app registration representing the web api backend

   6. Select permissions `access_as_user`

   7. Click _Add permissions_

5. Config Appsettings

   - `AI_SERVICE`: `AzureOpenAI` or `OpenAI`.
   - `API_KEY`: The `API key` for Azure OpenAI or for OpenAI.
   - `AZURE_OPENAI_ENDPOINT`: The Azure OpenAI resource `Endpoint` address. This is only required when using Azure OpenAI, omit `-Endpoint` if using OpenAI.
   - `FRONTEND_APPLICATION_ID`: The `Application (client) ID` associated with the application registration for the frontend.
   - `BACKEND_APPLICATION_ID`: The `Application (client) ID` associated with the application registration for the backend.
   - `TENANT_ID` : Your Azure AD tenant ID
   - `AZURE_AD_INSTANCE` _(optional)_: The Azure AD cloud instance for the authenticating users. Defaults to `https://login.microsoftonline.com`.

# Quartech Deployment to Azure K8s

## Manual by Developer

1. az cloud set --name AzureCloud
1. az acr login --name crpegasusshared
1. docker build -f docker/webapi/Dockerfile -t chat-copilot-webapi .
1. docker tag chat-copilot-webapi:latest crpegasusshared.azurecr.io/chat-copilot-webapi:latest
1. docker push crpegasusshared.azurecr.io/chat-copilot-webapi
1. docker build -f docker/webapp/Dockerfile.nginx -t chat-copilot-webapp .
1. docker tag chat-copilot-webapp:latest crpegasusshared.azurecr.io/chat-copilot-webapp:latest
1. docker push crpegasusshared.azurecr.io/chat-copilot-webapp
1. cd helm/
1. helm upgrade -n copilot-dev --install dev .

# Troubleshooting

TODO: Update
