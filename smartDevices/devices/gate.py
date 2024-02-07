import math
import random
import string
import time
from .device import Device
import json
import paho.mqtt.publish as publish

class Gate(Device):

    def __init__(self, device_info, mqtt_pool):
        '''
            Fields:
            id, name, isOn, typeOfPowerSupply, powerUsage,
            smartDeviceType, spsPanels(size, efficency)

            FORMULA: Size * Efficency
        '''
        super().__init__(device_info, mqtt_pool)

    def on_connect(self, client, userdata, flags, rc):
        self.subscribe_to_licence_delete(client)
        self.subscribe_to_licence_register(client)
        return super().on_connect(client, userdata, flags, rc)


    def subscribe_to_licence_delete(self, client):
        client.subscribe('licence-delete/' + self.id)

    def subscribe_to_licence_register(self, client):
        client.subscribe('licence-register/' + self.id)
    def device_operation(self, wait_time):
        if len(self.device_info['vgValidLicensePlates'])==0:
            licencePlate = ''.join(random.choices(string.ascii_uppercase + string.digits, k=8))
        else:
            licencePlate = random.choice(self.device_info['vgValidLicensePlates'])
        is_entering = True
        if(random.random() < 0.5):
            licencePlate = ''.join(random.choices(string.ascii_uppercase + string.digits, k=8))
        if (random.random() < 0.5):
            is_entering = False

        if self.is_online:
            if self.device_info['isOn']:
                if is_entering:
                    payload = {
                        'DeviceId': self.id,
                        'licencePlate': licencePlate,
                        'action': 3
                    }
                    publish.single('gateEvent', json.dumps(payload), hostname="192.168.105.29", port=1883)
                else:
                    payload = {
                        'DeviceId': self.id,
                        'licencePlate': licencePlate,
                        'action': 4
                    }
                    publish.single('gateEvent', json.dumps(payload), hostname="192.168.105.29", port=1883)
            else:
                if not is_entering:
                    self.validPlateVehicleEnteringLeaving(licencePlate, 4)
                else:
                    if self.device_info['vgIsPublic']:
                        self.validPlateVehicleEnteringLeaving(licencePlate, 3)
                    else:
                        if licencePlate in self.device_info['vgValidLicensePlates']:
                            self.validPlateVehicleEnteringLeaving(licencePlate, 3)
                        else:
                            payload = {
                                'DeviceId': self.id,
                                'licencePlate': licencePlate,
                                'action': 5
                            }
                            publish.single('gateEvent', json.dumps(payload), hostname="192.168.105.29", port=1883)
                            time.sleep(2)

        return super().device_operation(wait_time)

    def validPlateVehicleEnteringLeaving(self, licencePlate, actionNumber):
        payload = {
            'DeviceId': self.id,
            'licencePlate': licencePlate,
            'action': 1
        }

        publish.single('gateEvent', json.dumps(payload), hostname="192.168.105.29", port=1883)
        time.sleep(2)
        payload = {
            'DeviceId': self.id,
            'licencePlate': licencePlate,
            'action': actionNumber
        }

        publish.single('gateEvent', json.dumps(payload), hostname="192.168.105.29", port=1883)
        time.sleep(2)
        payload = {
            'DeviceId': self.id,
            'licencePlate': licencePlate,
            'action': 2
        }
        publish.single('gateEvent', json.dumps(payload), hostname="192.168.105.29", port=1883)



    def process_command(self, payload, id):
        device_id = id
        response_topic = "gate-public-private-changed"
        is_public = False
        if payload.lower() == "turn_public":
            is_public = True
        response_payload = {
            "isPublic": is_public,
            "deviceId": device_id
        }
        self.device_info['vgIsPublic'] = is_public
        publish.single(response_topic, json.dumps(response_payload), hostname="192.168.105.29", port=1883)
        return super().process_command(payload, id)

    def process_licence_delete(self, payload):
        licence_to_delete = payload
        if licence_to_delete:
            if 'vgValidLicensePlates' in self.device_info:
                if licence_to_delete in self.device_info['vgValidLicensePlates']:
                    self.device_info['vgValidLicensePlates'].remove(licence_to_delete)

        return super().process_command(payload, id)

    def process_licence_register(self, payload):
        licence_to_register = payload
        if licence_to_register:
            if 'vgValidLicensePlates' in self.device_info:
                if licence_to_register not in self.device_info['vgValidLicensePlates']:
                    self.device_info['vgValidLicensePlates'].append(licence_to_register)
        return super().process_command(payload, id)

    def on_message(self, client, userdata, msg):
        payload = msg.payload.decode('utf-8')
        topic_parts = msg.topic.split('/')
        if len(topic_parts) > 1:
            topic_name = topic_parts[0]
            if topic_name == 'licence-delete':

                self.process_licence_delete(payload)
            if topic_name == 'licence-register':
                self.process_licence_register(payload)

        return super().on_message(client, userdata, msg)

    def run_simulator(self, stop_event, wait_time):
        return super().run_simulator(stop_event, wait_time)

    def run_simulator_callback(self):

        return super().run_simulator_callback()