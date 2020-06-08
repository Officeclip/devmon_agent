# devmon_agent
Device Monitoring Agent - Simple monitoring of OS, Memory, Network, CPU, Hard Disk Drive

*** Work In Progress : Prototype Stage ***

This description will change as we go along...

To test:

1. Create an IIS application pointing to dev_web_api folder (lets call it http://localhost/monitor))
2. Copy the file appSettings.example.json to appSettings.json in the main folder
3. Set the attribute "Copy if Newer" on appSettings.json
4. Put a unique guid on appSettings.json and set the server_url to the rest server url
5. For localhost (if the IIS is pointing to http://localhost/monitor) this url might look like http://localhost/monitor/api
6. Run devmon_agent/bin/debug/devmon_agent.exe and keep the command prompt open
7. On a browser window run http://localhost/monitor

