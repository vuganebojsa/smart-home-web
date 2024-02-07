from influxdb_client import InfluxDBClient, Point
from influxdb_client.client.write_api import SYNCHRONOUS
from datetime import datetime, timedelta
import random
# influx setup
bucket = "smarthome"
org = "FTN"
url = "http://192.168.105.29:8086"
token = 'PKtMrhnvDHXub6Iel5h9ytnAH3tBBqyvboSPh9hgIEHaGbUS5wAb_3JT0JxJQWHkVfEomVRvMAqiG-HkT1rsDQ=='
influxdb_client = InfluxDBClient(url=url, token=token, org=org)
write_api = influxdb_client.write_api(write_options=SYNCHRONOUS)

current_datetime = datetime.now()
adjusted_datetime = current_datetime - timedelta(hours=1)
formatted_time = adjusted_datetime.isoformat()
device_ids = ['90d3e901-bb0f-4d42-520b-08dbf0cfbbd7',
              '8fe8e973-f6a2-4985-5e59-08dbf0f403ff',
              '1cf168ef-4bd6-4022-3777-08dbf1ab3238',
              'a7cbd5f6-c2de-49e4-55f5-08dbf310e12b',
              '7744d6c6-33a5-44a1-be05-08dc085ce13e',
              'd71216a9-c344-4c51-547d-08dc086e71c3',
              '056f1ab9-b452-4653-a2c2-08dbeb374a21',
              'b1b13eab-0d6e-4750-a2c5-08dbeb374a21',
              '21bdf5a5-d69d-4085-b09a-08dbf963f141',
              'a871913d-a8f1-4d73-547e-08dc086e71c3',
              '468737c6-def5-4152-a7eb-08dbff2a7579',
              '788b07c4-24e3-43ae-3120-08dc24c87652']
start_of_month = datetime.now().replace(day=1, hour=0, minute=0, second=0, microsecond=0) - timedelta(days=3*30)

for month_offset in range(2, 3):
    # Adjust time to the beginning of the month
    start_of_month += timedelta(days=30*month_offset)


    for minute_offset in range(0, 44640, 60):  # 2 minutes apart for 3 months
        adjusted_datetime = start_of_month + timedelta(minutes=minute_offset)
        #formatted_time = adjusted_datetime.isoformat()
        for dev_id in device_ids:
            i = random.randint(0, 5)
            is_online = 0
            if i < 4:
                is_online = 1
            point = (
                Point("onlineStatus")
                .tag("deviceId", dev_id) 
                .field("isOnline", is_online)
                .time(adjusted_datetime)
            )
            write_api.write(bucket=bucket, org=org, record=point)
