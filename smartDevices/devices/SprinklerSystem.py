import math
import random

from .device import Device
import json
from datetime import datetime, time
import paho.mqtt.publish as publish





class SprinklerSystem(Device):

    def __init__(self, device_info, mqtt_pool):

        super().__init__(device_info, mqtt_pool)

    def on_connect(self, client, userdata, flags, rc):
        self.subscribe_to_day_delete(client)
        self.subscribe_to_day_add(client)
        self.subscribe_to_change_starttime(client)
        self.subscribe_to_change_endtime(client)
        return super().on_connect(client, userdata, flags, rc)

    def device_operation(self, wait_time):
        if self.is_online:
            
            now = datetime.now()
            current_hours_minuts = str(now.hour) + ":" + str(now.minute)
            day_of_week = now.strftime("%A")
            if self.device_info['ssIsSpecialMode']:
                
                if day_of_week in self.device_info['ssActiveDays']:
                    
                    if current_hours_minuts == self.device_info['ssStartSprinkle']:
                        
                        if not self.device_info['isOn']:
                            response_topic = "device-on-off-changed"
                            self.device_info['isOn'] = True
                            response_payload = {
                                "on": True,
                                "deviceId": self.id
                            }
                            publish.single(response_topic, json.dumps(response_payload), hostname="192.168.105.29",
                                           port=1883)

                            response_topic = "sprinkler-on-off"
                            self.device_info['isOn'] = True
                            response_payload = {
                                "on": True,
                                "deviceId": self.id
                            }
                            publish.single(response_topic, json.dumps(response_payload), hostname="192.168.105.29",
                                           port=1883)

                    elif current_hours_minuts == self.device_info['ssEndSprinkle']:
                        if self.device_info['isOn']:
                            response_topic = "device-on-off-changed"
                            self.device_info['isOn'] = False
                            response_payload = {
                                "on": False,
                                "deviceId": self.id
                            }
                            publish.single(response_topic, json.dumps(response_payload), hostname="192.168.105.29",
                                           port=1883)

                            response_topic = "sprinkler-on-off"
                            self.device_info['isOn'] = False
                            response_payload = {
                                "on": False,
                                "deviceId": self.id
                            }
                            publish.single(response_topic, json.dumps(response_payload), hostname="192.168.105.29",
                                           port=1883)
        return super().device_operation(wait_time)

    def process_command(self, payload, id):
        device_id = id
        response_topic = "sprinkler-special-state-changed"
        is_active = False
        if payload.lower() == "turn_on":
            is_active = True
        response_payload = {
            "isPublic": is_active,
            "deviceId": device_id
        }
        self.device_info['ssIsSpecialMode'] = is_active
        publish.single(response_topic, json.dumps(response_payload), hostname="192.168.105.29", port=1883)

        return super().process_command(payload, id)

    def process_day_delete(self, payload):
        day_to_delete = payload
        if day_to_delete:
            if 'ssActiveDays' in self.device_info:
                if day_to_delete in self.device_info['ssActiveDays']:
                    self.device_info['ssActiveDays'].remove(day_to_delete)

        return super().process_command(payload, id)

    def process_day_add(self, payload):
        day_to_add = payload
        if day_to_add:
            if 'ssActiveDays' in self.device_info:
                if day_to_add not in self.device_info['ssActiveDays']:
                    self.device_info['ssActiveDays'].append(day_to_add)
        return super().process_command(payload, id)

    def process_change_startTime(self, payload):
        changed_time = payload
        if changed_time:
            if 'ssStartSprinkle' in self.device_info:
                self.device_info['ssStartSprinkle'] = changed_time
        return super().process_command(payload, id)

    def process_change_endTime(self, payload):
        changed_time = payload
        if changed_time:
            if 'ssEndSprinkle' in self.device_info:
                self.device_info['ssEndSprinkle'] = changed_time
        return super().process_command(payload, id)

    def on_message(self, client, userdata, msg):
        payload = msg.payload.decode('utf-8')
        topic_parts = msg.topic.split('/')
        if len(topic_parts) > 1:
            topic_name = topic_parts[0]
            if topic_name == 'day-delete':
                self.process_day_delete(payload)
            if topic_name == 'day-add':
                self.process_day_add(payload)
            if topic_name == 'change-starttime':
                self.process_change_startTime(payload)
            if topic_name == 'change-endtime':
                self.process_change_endTime(payload)
        return super().on_message(client, userdata, msg)

    def subscribe_to_day_delete(self, client):
        client.subscribe('day-delete/' + self.id)

    def subscribe_to_day_add(self, client):
        client.subscribe('day-add/' + self.id)

    def subscribe_to_change_starttime(self, client):
        client.subscribe('change-starttime/' + self.id)

    def subscribe_to_change_endtime(self, client):
        client.subscribe('change-endtime/' + self.id)



    def run_simulator(self, stop_event, wait_time):
        return super().run_simulator(stop_event, wait_time)

    def run_simulator_callback(self):

        return super().run_simulator_callback()