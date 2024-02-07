import paho.mqtt.client as mqtt
import paho.mqtt.publish as publish
from functools import partial
import json
import requests
import threading
import time
import random
import sys
from devices.Battery import Battery
from devices.Lamp import Lamp
from devices.SolarPanelSystem import SolarPanelSystem
from devices.SprinklerSystem import SprinklerSystem
from devices.device import Device
from devices.gate import Gate
from devices.AmbientSensor import AmbientSensor
from devices.VehicleCharger import VehicleCharger

import queue

class MQTTPool:
    def __init__(self, max_connections=50):
        self.max_connections = max_connections
        self.connections = queue.Queue(maxsize=max_connections)
        self.lock = threading.Lock()

    def get_connection(self):
        if not self.connections.empty():
            return self.connections.get()
        else:
            # Create a new MQTT connection if the pool is empty
            new_connection = mqtt.Client()
            return new_connection

    def release_connection(self, connection):
        self.connections.put(connection)
def run_thread(device, threads, stop_event):
    mqtt_pool = MQTTPool(max_connections=10)

    wait_time = 30.0
    if device['hbBatterySize'] is not None:
        device_instance = Battery(device, mqtt_pool)
        wait_time = random.uniform(8, 15)
    elif device['spsPanels'] is not None:
        device_instance = SolarPanelSystem(device, mqtt_pool)
        wait_time = random.uniform(55, 65)
    elif device['lLuminosity'] is not None:
        device_instance = Lamp(device, mqtt_pool)
        wait_time = random.uniform(55, 65)
    elif device['vgValidLicensePlates'] is not None:
        device_instance = Gate(device, mqtt_pool)
        wait_time = random.uniform(16, 22)
    elif device['asRoomTemperature'] is not None:
        device_instance = AmbientSensor(device, mqtt_pool)
        wait_time = random.uniform(55, 65)
    elif device['ssEndSprinkle'] is not None:
        device_instance = SprinklerSystem(device, mqtt_pool)
        wait_time = random.uniform(50, 60)
    elif device['ewPower'] is not None:
        device_instance = VehicleCharger(device, mqtt_pool)
        wait_time = random.uniform(80, 95)
    else:
        device_instance = Device(device, mqtt_pool)
    thread = threading.Thread(target=device_instance.run_simulator, args=(stop_event, wait_time))
    thread.start()
    threads.append(thread)

if __name__ == "__main__":
    num_of_devices = 20
    type_of_device = 0
    if len(sys.argv) > 2:
        num_of_devices = int(sys.argv[1])
        type_of_device = int(sys.argv[2])
        get_n = False
    elif len(sys.argv) > 1:
        num_of_devices = int(sys.argv[1])
        get_n = True
    if get_n:        
        response = requests.get('https://localhost:7217/api/v1/smartDevices/n-devices?totalDevices=' + str(num_of_devices), verify=False)
    else:
        response = requests.get('https://localhost:7217/api/v1/smartDevices/n-devices-type?totalDevices=' + str(num_of_devices) + '&type=' + str(type_of_device), verify=False)

    devices = response.json()
    # for device in devices:
    #     print(device)
        
    threads = []
    stop_event = threading.Event()

    try:
        # start thread for each device
        for device in devices:
            run_thread(device, threads, stop_event)
    except KeyboardInterrupt:
        print('Stopping simulation.')
        stop_event.set()
        for t in threads:
            t.join()  # wait for threads to finish