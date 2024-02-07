import math
import random
import string
import time
from influxdb_client import InfluxDBClient, Point
from influxdb_client.client.write_api import SYNCHRONOUS
from datetime import datetime, timedelta
import json

bucket = "smarthome"
org = "FTN"

url = "http://192.168.105.29:8086"
token = 'PKtMrhnvDHXub6Iel5h9ytnAH3tBBqyvboSPh9hgIEHaGbUS5wAb_3JT0JxJQWHkVfEomVRvMAqiG-HkT1rsDQ=='
influxdb_client = InfluxDBClient(url=url, token=token, org=org)
write_api = influxdb_client.write_api(write_options=SYNCHRONOUS)


def device_operation():
    id = "468737c6-def5-4152-a7eb-08dbff2a7579"
    now = datetime.now() - timedelta(days=90)
    isOn = 1
    isPublic = 1
    for i in range(129600):
        now = now + timedelta(seconds=900)
        validPlates = ["BG-222", "SI-066", "MM-421"]

        prevOn = isOn
        prevPub = isPublic
        if (random.random() < 0.5):
            isOn = 0
        else:
            isOn = 1

        if (random.random() < 0.5):
            isPublic = 0
        else: isPublic = 1

        if isPublic != prevPub:
            point = (
                Point("gateStatus")
                .tag("deviceId", id)
                .field("isPublic", isPublic)
                .time(now)
            )
            write_api.write(bucket=bucket, org=org, record=point)
        if isOn != prevOn:
            point = (
                Point("status")
                .tag("deviceId", id)
                .field("isOn", isOn)
                .time(now)
            )
            write_api.write(bucket=bucket, org=org, record=point)



        if len(validPlates)==0:
            licencePlate = ''.join(random.choices(string.ascii_uppercase + string.digits, k=8))
        else:
            licencePlate = random.choice(validPlates)
        is_entering = True
        if(random.random() < 0.5):
            licencePlate = ''.join(random.choices(string.ascii_uppercase + string.digits, k=8))
        if (random.random() < 0.5):
            is_entering = False


        if isOn:
            if is_entering:
                point = (
                    Point("event")
                    .tag("deviceId", id)
                    .tag("licencePlate", licencePlate)
                    .field("action", 3)
                    .time(now)
                )
                write_api.write(bucket=bucket, org=org, record=point)

            else:
                point = (
                    Point("event")
                    .tag("deviceId", id)
                    .tag("licencePlate", licencePlate)
                    .field("action", 4)
                    .time(now)
                )
                write_api.write(bucket=bucket, org=org, record=point)
        else:
            if not is_entering:
                validPlateVehicleEnteringLeaving(now,licencePlate, 4)
            else:
                if isPublic:
                    validPlateVehicleEnteringLeaving(now, licencePlate, 3)
                else:
                    if licencePlate in validPlates:
                        validPlateVehicleEnteringLeaving(now, licencePlate, 3)
                    else:
                        point = (
                            Point("event")
                            .tag("deviceId", id)
                            .tag("licencePlate", licencePlate)
                            .field("action", 5)
                            .time(now)
                        )
                        write_api.write(bucket=bucket, org=org, record=point)
                        now = now + timedelta(seconds=2)


def validPlateVehicleEnteringLeaving(now, licencePlate, actionNumber):
    point = (
        Point("event")
        .tag("deviceId", id)
        .tag("licencePlate", licencePlate)
        .field("action", 1)
        .time(now)
    )
    write_api.write(bucket=bucket, org=org, record=point)
    now = now + timedelta(seconds=2)

    point = (
        Point("event")
        .tag("deviceId", id)
        .tag("licencePlate", licencePlate)
        .field("action", actionNumber)
        .time(now)
    )
    write_api.write(bucket=bucket, org=org, record=point)
    now = now + timedelta(seconds=2)
    point = (
        Point("event")
        .tag("deviceId", id)
        .tag("licencePlate", licencePlate)
        .field("action", 2)
        .time(now)
    )
    write_api.write(bucket=bucket, org=org, record=point)

if __name__ == '__main__':
    device_operation()
