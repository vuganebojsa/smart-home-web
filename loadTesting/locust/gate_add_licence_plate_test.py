import json
import random
import string
import login
from locust import HttpUser, task, between
class RegularUser(HttpUser):
    wait_time = between(1, 3)  # Vreme čekanja između HTTP zahteva

    def on_start(self):
        self.client.verify = False
        self.token = login.login(self.client)
        self.property_id = self.get_property_id()

    def generate_random_string(self, length):
        return ''.join(random.choices(string.ascii_letters + string.digits, k=length))

    def get_property_id(self):
        headers = {"Authorization": f"Bearer {self.token}"}
        response = self.client.get("/users/GetUserProperties?PageSize=5&PageNumber=1", headers=headers)

        if response.status_code == 200:
            response_data = json.loads(response.text)
            for property in response_data:
                if property.get("isAccepted") == 0:
                    return property.get("id")
            return None
        else:
            raise Exception(f"Login failed with status code {response.status_code}: {response.text}")

    @task
    def add_licence_plate(self):
        headers = {"Authorization": f"Bearer {self.token}"}
        devices = self.client.get("/smartDevices/" + self.property_id + "/devices?PageNumber=1&PageSize=100",
                                  headers=headers)
        response_data = json.loads(devices.text)
        gate = None
        for device in response_data:
            if device.get("smartDeviceType") == 4:
                gate = device
                break
        if gate is None:
            print("Korisnik nema Gate")
            return
        payload = {
            "plate": self.generate_random_string(8),
        }

        response = self.client.put(f"/smartDevices/register-licence-plate/{gate.get('id')}", json=payload, headers=headers)
        if response.status_code != 200:
            print("Greška")