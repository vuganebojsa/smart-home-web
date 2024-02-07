from .device import Device
import json
from datetime import datetime, time
import paho.mqtt.publish as publish
import random

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
    start_time = time(8, 0, 0)  
    end_time = time(17, 0, 0) 
    if season == "summer":
        start_time = time(6, 0, 0)
        end_time = time(18, 0, 0)
    elif season == "spring":
        start_time = time(7, 0, 0)
        end_time = time(17, 0, 0)
    elif season == "autumn":
        start_time = time(8, 0, 0)
        end_time = time(17, 0, 0)
    else:
        start_time = time(8, 0, 0)
        end_time = time(16, 0, 0)
    return start_time, end_time

def get_weather_coeficient(total_kwh_per_minute):
    weather_type = random.randint(1, 4)
    # 1 is sunny
    if weather_type == 1:
        return 0
    # 2 is rainy/foggy
    elif weather_type == 2:
        return - 0.2 * total_kwh_per_minute
    # 3 is half sunny
    elif weather_type == 3:
        return  - 0.1 * total_kwh_per_minute
    return - 0.05 * total_kwh_per_minute

class SolarPanelSystem(Device):
    
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
    
    def device_operation(self, wait_time):
        if self.is_online and self.device_info['isOn']:
            panels = self.device_info['spsPanels']
            # size, efficency
            total_kwh_per_minute = 0
            current_time = datetime.now().time()
            start_time, end_time = get_start_end_time()
            if start_time <= current_time <= end_time: 
                for panel in panels:
                    size = panel['size']
                    efficency = panel['efficency']
                    total = size * (efficency/100) * 0.0167
                    total_kwh_per_minute += total
                if total_kwh_per_minute > 0:
                    weather_coeficient = get_weather_coeficient(total_kwh_per_minute)
                    total_kwh_per_minute = total_kwh_per_minute + weather_coeficient
            if total_kwh_per_minute > 0:
                # only send if actualy produced some energy
                payload = {
                    'DeviceId': self.id,
                    'TotalKwhPerMinute': total_kwh_per_minute
                }
                publish.single('solarpanelsystem', json.dumps(payload), hostname="192.168.105.29", port=1883)

        return super().device_operation(wait_time)

    def process_command(self, payload, id):

        return super().process_command(payload, id)
    def on_message(self, client, userdata, msg):

        return super().on_message(client, userdata, msg)
    
    def run_simulator(self, stop_event, wait_time):
        return super().run_simulator(stop_event, wait_time)
    
    def run_simulator_callback(self):
        
        return super().run_simulator_callback()