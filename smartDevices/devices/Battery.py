from .device import Device
import paho.mqtt.publish as publish
import json
class Battery(Device):
    
    def __init__(self, device_info, mqtt_pool):
        '''
            Fields:
            id, name, isOn, typeOfPowerSupply, powerUsage,
            smartDeviceType, hbBatterySize, hbOccupationLevel
            
        '''
        super().__init__(device_info, mqtt_pool)
    
    def device_operation(self, wait_time):
        if self.is_online:
            response_payload = {
                'DeviceId': self.id,
                'BatteryOccupation': self.device_info['hbOccupationLevel']
            }
            publish.single('batteryOccupation', json.dumps(response_payload), hostname='192.168.105.29', port=1883)
        return super().device_operation(wait_time)
    
    def on_connect(self, client, userdata, flags, rc):
        self.subscribe_to_occupation(client)
        return super().on_connect(client, userdata, flags, rc)
    
    def subscribe_to_occupation(self, client):
        client.subscribe('occupation/' + self.id)

    def process_command(self, payload, id):
        ## DODATI KOD VAMO TO JE STA KORISNIK SALJE UREDJAJU

        return super().process_command(payload, id)
    
    def on_message(self, client, userdata, msg):
        
        payload = msg.payload.decode('utf-8')
        topic_parts = msg.topic.split('/')
        if len(topic_parts) > 1:
            topic_name = topic_parts[0]
            if topic_name == 'occupation':
                self.device_info['hbOccupationLevel'] = float(payload)
        return super().on_message(client, userdata, msg)
    
    def run_simulator(self, stop_event, wait_time):
        return super().run_simulator(stop_event, wait_time)
    
    def run_simulator_callback(self):
        
        return super().run_simulator_callback()