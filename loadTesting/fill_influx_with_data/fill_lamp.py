import math
import random
from influxdb_client import InfluxDBClient, Point
from influxdb_client.client.write_api import SYNCHRONOUS
from datetime import datetime, time, timedelta

def get_season(time123):
    now = time123
    month = now.month

    if 3 <= month <= 5:
        return "spring"
    elif 6 <= month <= 8:
        return "summer"
    elif 9 <= month <= 11:
        return "autumn"
    else:
        return "winter"


def get_start_end_time(time123):
    season = get_season(time123)
    if season == "summer":
        start_time = time(6, 0, 0)
        end_time = time(17, 59, 0)
    elif season == "spring":
        start_time = time(7, 0, 0)
        end_time = time(16, 59, 0)
    elif season == "autumn":
        start_time = time(8, 0, 0)
        end_time = time(16, 59, 0)
    else:
        start_time = time(8, 0, 0)
        end_time = time(15, 59, 0)
    return start_time, end_time





def calculate_luminosity( now, start_time, end_time, min_intensity, max_intensity):
    percentage_luminosity = 0
    if (end_time.hour*60+end_time.minute )> (now.hour*60+now.minute) and  (now.hour*60+now.minute) >  (start_time.hour*60+start_time.minute):
        elapsed_minutes1 = abs((now.hour - start_time.hour)) * 60 + abs((now.minute - start_time.minute))
        elapsed_minutes2 = abs((now.hour - end_time.hour)) * 60 + abs((now.minute - end_time.minute))

        difference_start_end_in_minutes = abs((start_time.hour - end_time.hour)) * 60 + abs((start_time.minute - end_time.minute))
        total_difference = elapsed_minutes2 * elapsed_minutes1
        percentageOfDifference = total_difference/math.pow(difference_start_end_in_minutes/2,2)
        normalized_time = percentageOfDifference
        percentage_luminosity = min_intensity + normalized_time * (max_intensity - min_intensity) + random.uniform(-2, 2)
    elif (now.hour*60+now.minute) < (start_time.hour*60+now.minute):
        difference_start_now = abs((now.hour - start_time.hour)) * 60 + abs((now.minute - start_time.minute))
        percentageOfDifference = difference_start_now/(start_time.hour*60+now.minute)
        normalized_time = percentageOfDifference
        percentage_luminosity = min_intensity-min_intensity*normalized_time + random.uniform(-0.2, 0.2)

    else:
        difference_end_now = abs((now.hour - end_time.hour)) * 60 + abs((now.minute - end_time.minute))
        percentageOfDifference = difference_end_now / ((24 * 60) -  (end_time.hour*60+end_time.minute ))
        normalized_time = percentageOfDifference
        percentage_luminosity = min_intensity-(normalized_time*min_intensity) + random.uniform(-0.2, 0.2)

    if percentage_luminosity < 0:
        percentage_luminosity = (0 + random.uniform(0.05, 0.5))
    if percentage_luminosity > 100:
        percentage_luminosity = (100- random.uniform(0,1))
    return  percentage_luminosity





def device_operation():

    bucket = "smarthome"
    org = "FTN"
    
    url = "http://192.168.105.29:8086"
    token = 'PKtMrhnvDHXub6Iel5h9ytnAH3tBBqyvboSPh9hgIEHaGbUS5wAb_3JT0JxJQWHkVfEomVRvMAqiG-HkT1rsDQ=='
    influxdb_client = InfluxDBClient(url=url, token=token, org=org)
    write_api = influxdb_client.write_api(write_options=SYNCHRONOUS)


    now = datetime.now() - timedelta(days=90)
    for i in range(129600):
        now = now + timedelta(seconds=60)
        start_time, end_time = get_start_end_time(now)
        min_intensity = 10
        max_intensity = 100
        total_luminosity = calculate_luminosity(now, start_time, end_time, min_intensity, max_intensity)

        point = (
            Point("luminosity")
            .tag("deviceId", "056f1ab9-b452-4653-a2c2-08dbeb374a21")
            .field("detected", total_luminosity)
            .time(now)
        )
        write_api.write(bucket=bucket, org=org, record=point)


if __name__ == '__main__':
     device_operation()

