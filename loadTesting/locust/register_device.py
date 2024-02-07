import random
import string
import login
from locust import HttpUser, task, between
import json
class RegularUser(HttpUser):
    wait_time = between(1, 3)
    def on_start(self):
        self.client.verify = False
        # Prijavljivanje običnog korisnika
        self.token = login.login(self.client)
        self.property_id = self.get_property_id()

    def generate_random_string(self, length):
        return ''.join(random.choices(string.ascii_letters + string.digits, k=length))
    
    def get_property_id(self):
        headers = {"Authorization": f"Bearer {self.token}"}
        response = self.client.get("/users/GetUserProperties?PageSize=1&PageNumber=1", headers=headers)

        if response.status_code == 200:
            response_data = json.loads(response.text)
            for property in response_data:
                if property.get("isAccepted") == 0:
                    return property.get("id")
            return None
        else:
            raise Exception(f"Login failed with status code {response.status_code}: {response.text}")
    @task
    def create_charger(self):
        power_supply = random.randint(0, 1)
        power_usage = 0
        if power_supply == 1:
            power_usage = random.random()
        payload = {
        "name": "string",
        "numberOfConnections": random.randint(2, 5),
        "power": random.randint(2, 10),
        "luminosity": random.randint(10, 50),
        "powerSupply": power_supply,
        "powerUsage": power_usage,
        "smartPropertyId": self.property_id,
        "image": "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAA1JREFUGFdjaJ+2/T8ABlAC1OB5hf4AAAAASUVORK5CYII=",
        "imageType": "jpg",
        }
        headers = {"Authorization": f"Bearer {self.token}"}
        response = self.client.post("/smartDevices/register-electric-vehicle-charger", json=payload, headers=headers)
        if response.status_code != 200:
            print("Greška prilikom kreiranja punjaca")
    
    @task
    def create_battery(self):

        payload = {
        "name": "string",
        "batterySize": random.randint(20, 500),
        "powerSupply": 1,
        "powerUsage": 0,
        "smartPropertyId": self.property_id,
        "image": "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAA1JREFUGFdjaJ+2/T8ABlAC1OB5hf4AAAAASUVORK5CYII=",
        "imageType": "jpg",
        }
        headers = {"Authorization": f"Bearer {self.token}"}
        response = self.client.post("/smartDevices/register-house-battery", json=payload, headers=headers)
        if response.status_code != 200:
            print("Greška prilikom kreiranja baterije")