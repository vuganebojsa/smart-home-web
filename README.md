# ADVANCED WEB TECHNOLOGIES

## TEAM 12

### Members
     1.  Dusan Bibin SV52/2020
     2.  Bogdan Janosevic SV65/2020
     3.  Nebojsa Vuga SV53/2020

### Technologies Used

 - Backend: .Net Web Api
 - Frontend: Angular
 - Databases:
    - For images: Local storage
    - For Users, Properties, Devices...: Sql Server(For Development)
    - For TimeSeries: Still not decided
- Sending Email: SendGrid


### Whereabouts
    - When you first run the app the superadmin profile is created.
    Its info is stored in the file: SmartHouseBackend/SmartHouseBackend/confidental/superadmin.txt
    Also there are some users in the same path with the file being named users.txt

### How to Run?
    - Docker: Position yourself in dockerSettings folder. 
        Type command:
            docker-compose -p influx up
    - Nginx: Position yourself in nginx folder.
        Type command:
            start nginx
    - Frontend: Position yourself in the frontend app folder(SmartHouseFrontend) and type 2 commands:
        1. npm install --force
        2. ng serve
    - Backend: Position yourself in the backend app folder(SmartHouseBackend). Start the App with Ctrl + F5 (default start command) or in terminal with command:
            ```
                1. Add-Migration Migg -Project SmartHouse.Infrastructure
                2. Update-Database
                3. dotnet run
                
            ```
    - Smart Device: Position yourself in the smart device folder(smartDevices).
        Create virtualenv with command: 
            ```

                py -m venv venv.

            ```
        Run the environment with the command(windows example):
            ```

                venv\Scripts\activate

            ```
        Install requirements with command:
            ```

            pip install -r requirements.txt

            ```
        
        Run the script with command:
            ```

                py simulation.py 50 8 (first arg is num of devices, second arg is the type of device)
                0 - Ambient Sensor
                1 - AC
                2 - Washing Machine
                3 - Lamp
                4 - Vehicle Gate
                5 - Sprinkler System
                6 - Solar Panels
                7 - House Battery 
                8 - vehicle charger
                
            ```
