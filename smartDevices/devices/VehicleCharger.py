import math
import random
import string
from .device import Device
import json
import paho.mqtt.publish as publish
import threading
import time


class Car(object):
    def __init__(self, battery_size, occupancy):
        self.battery_size = battery_size
        self.occupancy = occupancy
        self.plate = self.generate_random_plate()
    
    def generate_random_plate(self):
        # Generate first 2 letters
        first_letters = ''.join(random.choices(string.ascii_letters, k=2))
        
        # Generate 3 numbers
        numbers = ''.join(random.choices(string.digits, k=3))

        # Generate last 2 letters
        last_letters = ''.join(random.choices(string.ascii_letters, k=2))

        # Concatenate the parts to form the final string
        random_string = f"{first_letters}{numbers}{last_letters}"

        return random_string
    def __str__(self):
        return self.plate + ' entered'

def generate_random_car():
    battery_size = random.randint(8, 15)
    occupancy = random.randint(0, battery_size - 5)
    return Car(battery_size, occupancy)

def calculate_charging_time(car, power, target_percentage):
    if target_percentage is None or target_percentage == 0:
        target_percentage = 100
    target_capacity = car.battery_size * target_percentage / 100.0
    if target_capacity <= car.occupancy:
        return 0
    charging_time = (target_capacity - car.occupancy) / power
    if charging_time < 0:
        charging_time = 0
    return charging_time

def calculate_power_needed(car, charging_time, target_percentage):
    if target_percentage is None or target_percentage == 0:
        target_percentage = 100
    target_capacity = car.battery_size * target_percentage / 100.0
    if target_capacity <= car.occupancy:
        return 0
    power_needed = (target_capacity - car.occupancy)
    return power_needed

class VehicleCharger(Device):

    def __init__(self, device_info, mqtt_pool):
        '''
            Fields:
            id, name, isOn, typeOfPowerSupply, powerUsage,
            smartDeviceType, ewNumberOfConnections, ewPower
        '''
        #topic vehicle-charger-power-used
        self.occupied_chargers = 0
        self.lock = threading.Lock()
        self.total_chargers = device_info['ewNumberOfConnections']
        self.power = device_info['ewPower']
        self.percentage_of_charge = device_info['ewPercentageOfCharge']
        super().__init__(device_info, mqtt_pool)

    def on_connect(self, client, userdata, flags, rc):
        self.subscribe_to_vehicle_charger(client)
        return super().on_connect(client, userdata, flags, rc)

    def subscribe_to_vehicle_charger(self, client):
        client.subscribe('vehicle-charger-max-charge/' + self.id)

    def device_operation(self, wait_time):
        if self.is_online:
            random_int = random.randint(0, 1)
            if random_int == 1:
                is_full = False
                with self.lock:
                    if self.total_chargers <= self.occupied_chargers:
                        is_full = True
                if not is_full:
                    car = generate_random_car()
                    total_time = calculate_charging_time(car, self.power, self.percentage_of_charge)
                    total_kwh_needed = calculate_power_needed(car, total_time, self.percentage_of_charge)
                    vehicle = threading.Thread(target=self.charge_car, args=(total_time, total_kwh_needed, car,))
                    vehicle.start()
        return super().device_operation(wait_time)

    def charge_car(self, total_time, total_kwh_needed, car):
        with self.lock:
            self.occupied_chargers = self.occupied_chargers + 1
        # publish start charging
        time_in_minutes = total_time * 60
        payload = {
                    'DeviceId': self.id,
                    'Plate': car.plate,
                    'ToCharge': total_kwh_needed,
                    'MinutesNeeded': time_in_minutes,
                    'CurrentConnections': self.occupied_chargers
                }
        publish.single('vehicle-charger-start-charge', json.dumps(payload), hostname="192.168.105.29", port=1883)
        
        time.sleep(time_in_minutes * 60)
        # charging over
        with self.lock:
            self.occupied_chargers  = self.occupied_chargers - 1
        # publish end charging
        payload = {
                    'DeviceId': self.id,
                    'Plate': car.plate,
                    'TotalKwhConsumed': total_kwh_needed,
                    'MinutesNeeded': time_in_minutes,
                    'CurrentConnections': self.occupied_chargers
                }
        publish.single('vehicle-charger-power-used', json.dumps(payload), hostname="192.168.105.29", port=1883)
           

    def process_command(self, payload, id):
        pass

    def on_message(self, client, userdata, msg):
        payload = msg.payload.decode('utf-8')
        topic_parts = msg.topic.split('/')
        if len(topic_parts) > 1:
            topic_name = topic_parts[0]
            if topic_name == 'vehicle-charger-max-charge':
                self.device_info['ewPercentageOfCharge'] = float(payload)
                self.percentage_of_charge = float(payload)
        return super().on_message(client, userdata, msg)

    def run_simulator(self, stop_event, wait_time):
        return super().run_simulator(stop_event, wait_time)

    def run_simulator_callback(self):

        return super().run_simulator_callback()