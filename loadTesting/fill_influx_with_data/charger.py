from influxdb_client import InfluxDBClient, Point
from influxdb_client.client.write_api import SYNCHRONOUS
from datetime import datetime, timedelta
import random
import string

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
              '7744d6c6-33a5-44a1-be05-08dc085ce13e',
              'd71216a9-c344-4c51-547d-08dc086e71c3',
              ]
start_of_month = datetime.now().replace(day=1, hour=0, minute=0, second=0, microsecond=0) - timedelta(days=3*30)
def generate_random_plate():
        # Generate first 2 letters
        first_letters = ''.join(random.choices(string.ascii_letters, k=2))
        
        # Generate 3 numbers
        numbers = ''.join(random.choices(string.digits, k=3))

        # Generate last 2 letters
        last_letters = ''.join(random.choices(string.ascii_letters, k=2))

        # Concatenate the parts to form the final string
        random_string = f"{first_letters}{numbers}{last_letters}"

        return random_string
for month_offset in range(2, 3):
    # Adjust time to the beginning of the month
    start_of_month += timedelta(days=30 * month_offset + 15)

    for minute_offset in range(0, 44640, 120):
        adjusted_datetime = start_of_month + timedelta(minutes=minute_offset)
        #formatted_time = adjusted_datetime.isoformat()
        first_plate = generate_random_plate()
        second_plate = generate_random_plate()
        first_kwh = random.uniform(0, 4)
        second_kwh = random.uniform(0, 4)
        minutes_first = random.uniform(1, 5) * first_kwh
        minutes_second = random.uniform(1, 5) * second_kwh
        point = (
            Point("vehiclecharging")
            .tag("deviceId", device_ids[0]) 
            .tag('plate', first_plate)
            .tag('minutesNeeded', minutes_first)
            .field("startChargeNeededKW", first_kwh)
            .time(adjusted_datetime)
        )
        write_api.write(bucket=bucket, org=org, record=point)

        point = (
            Point("vehiclecharging")
            .tag("deviceId", device_ids[1]) 
            .tag('plate', second_plate)
            .tag('minutesNeeded', minutes_second)
            .field("startChargeNeededKW", second_kwh)
            .time(adjusted_datetime)
        )
        write_api.write(bucket=bucket, org=org, record=point)
        after_first = adjusted_datetime + timedelta(minutes=minute_offset + int(minutes_first))
        after_second = adjusted_datetime + timedelta(minutes=minute_offset + int(minutes_second))

        point = (
            Point("vehiclecharging")
            .tag("deviceId", device_ids[0]) 
            .tag('plate', first_plate)
            .tag('minutesNeeded', minutes_first)
            .field("endChargeConsumedKWH", first_kwh)
            .time(after_first)
        )
        write_api.write(bucket=bucket, org=org, record=point)

        point = (
            Point("vehiclecharging")
            .tag("deviceId", device_ids[1]) 
            .tag('plate', second_plate)
            .tag('minutesNeeded', minutes_second)
            .field("endChargeConsumedKWH", second_kwh)
            .time(after_second)
        )
        write_api.write(bucket=bucket, org=org, record=point)
