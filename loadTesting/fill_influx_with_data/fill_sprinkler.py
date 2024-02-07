import math
import random

import json
from datetime import datetime, time
from influxdb_client import InfluxDBClient, Point
from influxdb_client.client.write_api import SYNCHRONOUS
from datetime import datetime, time, timedelta


bucket = "smarthome"
org = "FTN"
url = "http://192.168.105.29:8086"
token = 'PKtMrhnvDHXub6Iel5h9ytnAH3tBBqyvboSPh9hgIEHaGbUS5wAb_3JT0JxJQWHkVfEomVRvMAqiG-HkT1rsDQ=='
influxdb_client = InfluxDBClient(url=url, token=token, org=org)
write_api = influxdb_client.write_api(write_options=SYNCHRONOUS)


def device_operation():
    id = "bed727c9-367e-4088-5513-08dc24d9342d"
    isSpecialMode = True
    isOn = True
    allDays = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"]
    activeDays = ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"]
    username = "nebojsavuga01@gmail.com"
    start_sprinkler = "10:00"
    end_sprinkler = "12:00"
    now = datetime.now() - timedelta(days=90)
    for i in range(129600):
        if random.randint(1, 1200) == 1:
            day = random.choice(allDays)
            if day not in activeDays:
                activeDays.append(day)
                point = (
                    Point("sprinkleEvent")
                    .tag("deviceId", id)
                    .tag("username", username)
                    .tag("actionNumber", "3")
                    .field("value", day)
                    .time(now)
                )
                write_api.write(bucket=bucket, org=org, record=point)

        if random.randint(1, 1200) == 1:
            day = random.choice(allDays)
            if day in activeDays:
                activeDays.remove(day)
                point = (
                    Point("sprinkleEvent")
                    .tag("deviceId", id)
                    .tag("username", username)
                    .tag("actionNumber", "4")
                    .field("value", day)
                    .time(now)
                )
                write_api.write(bucket=bucket, org=org, record=point)
        if random.randint(1, 1200) == 1:
            isOn = not isOn
            value = "False"
            if isOn:
                value = "True"

            point = (
                Point("sprinkleEvent")
                .tag("deviceId", id)
                .tag("username", username)
                .tag("actionNumber", "1")
                .field("value", value)
                .time(now)
            )

            write_api.write(bucket=bucket, org=org, record=point)

        if random.randint(1, 1200) == 1:
            hours = random.randint(0, 23)
            minutes = random.randint(0, 59)
            start_sprinkler = f"{hours:02d}:{minutes:02d}"
            point = (
                Point("sprinkleEvent")
                .tag("deviceId", id)
                .tag("username", username)
                .tag("actionNumber", "5")
                .field("value", start_sprinkler)
                .time(now)
            )
            write_api.write(bucket=bucket, org=org, record=point)

        if random.randint(1, 1200) == 1:
            hours = random.randint(0, 23)
            minutes = random.randint(0, 59)
            end_sprinkler = f"{hours:02d}:{minutes:02d}"
            point = (
                Point("sprinkleEvent")
                .tag("deviceId", id)
                .tag("username", username)
                .tag("actionNumber", "6")
                .field("value", end_sprinkler)
                .time(now)
            )
            write_api.write(bucket=bucket, org=org, record=point)

        if isSpecialMode == True:
            if random.randint(1, 3200) == 1:
                isSpecialMode = not isSpecialMode
                point = (
                    Point("sprinkleEvent")
                    .tag("deviceId", id)
                    .tag("username", username)
                    .tag("actionNumber", "2")
                    .field("value", "False")
                    .time(now)
                )
                write_api.write(bucket=bucket, org=org, record=point)
        if isSpecialMode == False:
            if random.randint(1, 300) == 1:
                isSpecialMode = not isSpecialMode
                point = (
                    Point("sprinkleEvent")
                    .tag("deviceId", id)
                    .tag("username", username)
                    .tag("actionNumber", "2")
                    .field("value", "True")
                    .time(now)
                )
                write_api.write(bucket=bucket, org=org, record=point)
        now = now + timedelta(seconds=60)
        current_hours_minuts = str(now.hour) + ":" + str(now.minute)

        day_of_week = now.strftime("%A")
        if isSpecialMode:
            if day_of_week in activeDays:
                if current_hours_minuts == start_sprinkler:
                    if not isOn:
                        isOn = True
                        point = (
                            Point("status")
                            .tag("deviceId", id)
                            .field("isOn", 1)
                            .time(now)
                        )
                        write_api.write(bucket=bucket, org=org, record=point)
                        point = (
                            Point("sprinkleEvent")
                            .tag("deviceId", id)
                            .tag("username", "System")
                            .tag("actionNumber", "1")
                            .field("value", "True")
                            .time(now)
                        )
                        write_api.write(bucket=bucket, org=org, record=point)
                elif current_hours_minuts == end_sprinkler:
                    if isOn:
                        isOn = False
                        point = (
                            Point("status")
                            .tag("deviceId", id)
                            .field("isOn", 0)
                            .time(now)
                        )
                        write_api.write(bucket=bucket, org=org, record=point)

                        point = (
                            Point("sprinkleEvent")
                            .tag("deviceId", id)
                            .tag("username", "System")
                            .tag("actionNumber", "1")
                            .field("value", "False")
                            .time(now)
                        )

                        write_api.write(bucket=bucket, org=org, record=point)



if __name__ == '__main__':
    device_operation()















