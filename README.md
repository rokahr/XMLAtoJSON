# XMLAtoRest
This projects connects to the XMLA endpoint of PowerBI, reads data and transforms it to JSON. 

## General setup
1. Download Visual Studio Code and open
2. Click on file, open folder and open either the ConsoleApp or WebApp
3. Follow the steps below for the specific config. Return to 4 when done
4. Click F5 to debug
5. Use terminal in VS Code and nuget .net cli to install missing packages

If you want to deploy the webapp to azure do the following (only for WebApp, taken from https://docs.microsoft.com/en-us/aspnet/core/tutorials/publish-to-azure-webapp-using-vscode?view=aspnetcore-5.0)
1. in VS code console, paste dotnet publish -c Release -o ./publish
2. make sure you have installed the Azure web app extension for VS code (https://marketplace.visualstudio.com/items?itemName=ms-azuretools.vscode-azureappservice)
3. right click on the publish folder and say Deploy to Azure Web App...

## Console App
- Open VSCode/Program.cs
- Change all parameters in line 21 and 30

## Web App
- Open Models/XmlaToJson.cs and paste your tenant ID on line 19

## To do
- Add real Authentification, currently passed as parameters
- Proper error handeling
- Performance testing / auto-scaling
- RLS for service principals (currently 11.2020 not supported by Microsoft for SP's in PBI)
- Add possibility for user authentication instead of SP
- Check for RLS role and apply it when user logs in