from .device import Device
import json
from datetime import datetime, time
import paho.mqtt.publish as publish
import random

class AmbientSensor(Device):
    
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
        if self.is_online:
            temperature = random.uniform(20,22)
            humidity = random.uniform(37, 44)

            payload = {
                'DeviceId': self.id,
                'RoomTemperature': temperature,
                'RoomHumidity': humidity
            }
            
            publish.single('roomData', json.dumps(payload), hostname="localhost", port=1883)


        return super().device_operation(wait_time)

    def process_command(self, payload, id):

        return super().process_command(payload, id)
    def on_message(self, client, userdata, msg):

        return super().on_message(client, userdata, msg)
    
    def run_simulator(self, stop_event, wait_time):
        return super().run_simulator(stop_event, wait_time)
    
    def run_simulator_callback(self):
        
        return super().run_simulator_callback()