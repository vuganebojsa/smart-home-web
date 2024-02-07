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
device_ids = [
              '90d3e901-bb0f-4d42-520b-08dbf0cfbbd7',
              '8fe8e973-f6a2-4985-5e59-08dbf0f403ff',
              ]
start_of_month = datetime.now().replace(day=1, hour=0, minute=0, second=0, microsecond=0) - timedelta(days=3*30)

for month_offset in range(2, 3):
    # Adjust time to the beginning of the month
    start_of_month += timedelta(days=30 * month_offset)
    is_on = 1
    for minute_offset in range(0, 44640, 450):
        adjusted_datetime = start_of_month + timedelta(minutes=minute_offset)
        #formatted_time = adjusted_datetime.isoformat()
        
        point = (
            Point("deviceonoff")
            .tag("deviceId", device_ids[0]) 
            .tag('username', 'nebojsavuga')
            .tag('typeOfDevice', 'SolarPanelSystem')
            .field("isOn", is_on)
            .time(adjusted_datetime)
        )
        write_api.write(bucket=bucket, org=org, record=point)
        if is_on:
            is_on = 0
        else:
            is_on = 1

