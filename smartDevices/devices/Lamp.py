import math
import random

from .device import Device
import json
from datetime import datetime, time
import paho.mqtt.publish as publish

def get_season():
    now = datetime.now()
    month = now.month

    if 3 <= month <= 5:
        return "spring"
    elif 6 <= month <= 8:
        return "summer"
    elif 9 <= month <= 11:
        return "autumn"
    else:
        return "winter"


def get_start_end_time():
    season = get_season()
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

class Lamp(Device):

    def __init__(self, device_info, mqtt_pool):
        '''
            Fields:
            id, name, isOn, typeOfPowerSupply, powerUsage,
            smartDeviceType, spsPanels(size, efficency)

            FORMULA: Size * Efficency
        '''
        super().__init__(device_info, mqtt_pool)

    def on_connect(self, client, userdata, flags, rc):
        return super().on_connect(client, userdata, flags, rc)

    def turn_on_off(self, luminosity):
        response_topic = "device-on-off-changed"
        is_on = False
        if luminosity < self.device_info['lLuminosity']:
            is_on = True
        response_payload = {
            "on": is_on,
            "deviceId": self.id
        }

        self.device_info['isOn'] = is_on
        publish.single(response_topic, json.dumps(response_payload), hostname="192.168.105.29", port=1883)

    def calculate_luminosity(self, now, start_time, end_time, min_intensity, max_intensity):
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





    def device_operation(self, wait_time):
        if self.is_online:
            now = datetime.now().time()
            start_time, end_time = get_start_end_time()
            min_intensity = 10
            max_intensity = 100
            total_luminosity = self.calculate_luminosity(now, start_time, end_time, min_intensity, max_intensity)
            payload = {
                'DeviceId': self.id,
                'TotalLuminosity': total_luminosity
            }
            publish.single('lampLuminosity', json.dumps(payload), hostname="192.168.105.29", port=1883)
            self.turn_on_off(total_luminosity)


        return super().device_operation(wait_time)

    def process_command(self, payload, id):

        return super().process_command(payload, id)

    def on_message(self, client, userdata, msg):

        return super().on_message(client, userdata, msg)

    def run_simulator(self, stop_event, wait_time):
        return super().run_simulator(stop_event, wait_time)

    def run_simulator_callback(self):

        return super().run_simulator_callback()