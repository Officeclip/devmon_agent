# devmon_agent
Device Monitoring Agent - Simple monitoring of OS, Memory, Network, CPU, Hard Disk Drive

*** Work In Progress : Prototype Stage ***

This description will change as we go along...

To test:

1. Create an IIS application pointing to dev_web_api folder (lets call it http://localhost/monitor))
2. Copy the file appSettings.example.json to the output folder (bin/debug)
3. Put a unique guid on appSettings.json and set the server_url to the rest server url
4. For localhost (if the IIS is pointing to http://localhost/monitor) this url might look like http://localhost/monitor/api
5. Run devmon_agent/bin/debug/devmon_agent.exe and keep the command prompt open
6. On a browser window run http://localhost/monitor

