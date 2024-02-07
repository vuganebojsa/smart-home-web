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
    def delete_licence_plate(self):
        headers = {"Authorization": f"Bearer {self.token}"}
        devices = self.client.get("/smartDevices/" + self.property_id + "/devices?PageNumber=1&PageSize=100",
                                  headers=headers)
        devices_data = json.loads(devices.text)
        if len(devices_data) == 0:
            return
        id = ""
        for device in devices_data:
            if device.get("smartDeviceType") == 4:
                id = device.get("id")
                break
        if id == "":
            print("User has no gates")
            return
        valid_plates = self.client.get(f"/smartDevices/licence-plates/{id}", headers=headers).json()
        if len(valid_plates)==0:
            return
        plate = random.choice(valid_plates)
        payload = {
            "plate": plate,
        }

        response = self.client.put(f"/smartDevices/remove-licence-plate/{id}", json=payload, headers=headers)
        if response.status_code != 200:
            print("greska")