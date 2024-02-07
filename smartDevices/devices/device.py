import paho.mqtt.client as mqtt
import paho.mqtt.publish as publish
import json
import time
import random
import threading

class Device:
    def __init__(self, device_info, mqtt_pool):
        '''
            Fields:
            id, name, isOn, typeOfPowerSupply, powerUsage, smartDeviceType
        '''
        self.id = device_info['id']
        self.name = device_info['name']
        self.device_info = device_info
        self.mqtt_pool = mqtt_pool
        self.client = self.mqtt_pool.get_connection()
        self.is_online = False
        self.client.on_connect = self.on_connect
        self.client.on_message = self.on_message
        self.client.connect("192.168.105.29", 1883, 60)
        self.client.loop_start()

    def on_connect(self, client, userdata, flags, rc):
        # subscribe to command on-off
        self.subscribe_to_command(client)
        self.subscribe_to_on_off(client)
        self.subsribe_to_offline(client)

    def subsribe_to_offline(self, client):
        client.subscribe('offline/' + self.id)

    def subscribe_to_command(self, client):
        client.subscribe('command/' + self.id)

    def subscribe_to_on_off(self, client):
        client.subscribe("turn-on-off/" + self.id)

    def on_message(self, client, userdata, msg):

        payload = msg.payload.decode('utf-8')
        
        topic_parts = msg.topic.split('/')
        
        if len(topic_parts) > 1:
            topic_name = topic_parts[0]
            if topic_name == 'turn-on-off':
                self.process_on_off_changed(payload, topic_parts)
            elif topic_name == 'command':
                self.process_command(payload, topic_parts[1])
            elif topic_name == 'offline':
                self.is_online = False

    def process_on_off_changed(self, payload, topic_parts):
        device_id = topic_parts[1]
        response_topic = "device-on-off-changed"
        is_on = False
        if payload.lower() == "turn_on":
            is_on = True

        response_payload = {
                    "on": is_on,
                    "deviceId": device_id
                }
        self.device_info['isOn'] = is_on
        publish.single(response_topic, json.dumps(response_payload), hostname="192.168.105.29", port=1883)

    def process_command(self, payload, id):
        pass

    def run_simulator(self, stop_event, wait_time):
        while not self.client.is_connected():
            time.sleep(1)
        self.ping_online_loop()
        self.device_operation(wait_time)


    def ping_online_loop(self):
        rand_number = random.randint(0, 10)
        if rand_number < 5:
            self.ping_online()
        # Schedule the function to run again after 5 seconds
        threading.Timer(random.uniform(4, 10), self.ping_online_loop, []).start()

    def device_operation(self, wait_time):
        
        threading.Timer(wait_time, self.device_operation, [wait_time]).start()

    def ping_online(self):
        self.is_online = True
        topic = 'online'
        payload = {
            "Id": self.id,
            "Name": self.name
        }
        try:
            self.client.publish(topic, json.dumps(payload))
        except Exception as e:
            print(e)