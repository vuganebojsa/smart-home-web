import random
import string
import login
from locust import HttpUser, task, between
import json
from datetime import datetime, timedelta


class RegularUser(HttpUser):
    wait_time = between(1, 3)  # Vreme čekanja između HTTP zahteva

    def on_start(self):
        self.client.verify = False
        # Prijavljivanje običnog korisnika
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

    @task(30)
    def get_report_for_gate_from_to(self):
        headers = {"Authorization": f"Bearer {self.token}"}

        response = self.client.get("/smartDevices/" + self.property_id + "/devices?PageNumber=1&PageSize=100",
                                   headers=headers)
        if response.status_code != 200:
            print("Greška prilikom dobavljanja uredjaja")
            return
        response_data = json.loads(response.text)
        gate = None
        for device in response_data:
            if device.get("smartDeviceType") == 4:
                gate = device
                break
        if gate is None:
            print("Korisnik nema Kapiju")
            return
        today = datetime.today()
        tomorrow = today - timedelta(days=1)
        # Get the date 4 days before today
        four_days_ago = today - timedelta(days=12)
        from_date_str = four_days_ago.strftime("%Y-%m-%d")
        to_date_str = tomorrow.strftime("%Y-%m-%d")
        response = self.client.get(f"/reports/getGateEventHistory/{gate.get('id')}",
                                   params={"startDate": from_date_str, "endDate": to_date_str}, headers=headers)
        if response.status_code != 200:
            print('Nevalidan request')

    @task(40)
    def turn_system_on_off(self):
        headers = {"Authorization": f"Bearer {self.token}"}

        response = self.client.get("/smartDevices/" + self.property_id + "/devices?PageNumber=1&PageSize=100",
                                   headers=headers)
        if response.status_code != 200:
            print("Greška prilikom dobavljanja uredjaja")
            return
        response_data = json.loads(response.text)
        gate = None
        for device in response_data:
            if device.get("smartDeviceType") == 4:
                gate = device
                break
        if gate is None:
            print("Korisnik nema Kapiju")
            return

        url = f"/smartDevices/turn-on-off/{gate.get('id')}"
        is_on = random.choice([True, False])

        response2 = self.client.put(url, json={'isOn': is_on}, headers=headers)

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

        response = self.client.put(f"/smartDevices/register-licence-plate/{gate.get('id')}", json=payload,
                                   headers=headers)
        if response.status_code != 200:
            print("Greška")

    @task(20)
    def change_gate_public(self):
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
        if id == "":
            print("User has no gates")
            return

        payload = {
            "isPublic": True
        }
        response = self.client.put(f"/smartDevices/turn-gate-public-private/{id}", json=payload, headers=headers)
        if response.status_code != 200:
            print("Greška")

    @task(20)
    def change_gate_private(self):
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
        if id == "":
            print("User has no gates")
            return
        payload = {
            "isPublic": False
        }
        response = self.client.put(f"/smartDevices/turn-gate-public-private/{id}", json=payload, headers=headers)

        if response.status_code != 200:
            print("Greška")