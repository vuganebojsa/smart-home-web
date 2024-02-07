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

    @task
    def get_report_for_sprinkler_from_to(self):
        headers = {"Authorization": f"Bearer {self.token}"}

        response = self.client.get("/smartDevices/" + self.property_id + "/devices?PageNumber=1&PageSize=100",
                                   headers=headers)
        if response.status_code != 200:
            print("Greška prilikom dobavljanja uredjaja")
            return
        response_data = json.loads(response.text)
        sprinkler = None
        for device in response_data:
            if device.get("smartDeviceType") == 5:
                sprinkler = device
                break
        if sprinkler is None:
            print("Korisnik nema Prskalice")
            return
        today = datetime.today()
        tomorrow = today - timedelta(days=1)
        # Get the date 4 days before today
        four_days_ago = today - timedelta(days=12)
        from_date_str = four_days_ago.strftime("%Y-%m-%d")
        to_date_str = tomorrow.strftime("%Y-%m-%d")
        response = self.client.get(f"/reports/getSprinklerEventHistory/{sprinkler.get('id')}",
                                   params={"startDate": from_date_str, "endDate": to_date_str}, headers=headers)
        if response.status_code != 200:
            print('Nevalidan request')

    @task(3)
    def turn_system_on_off(self):
        headers = {"Authorization": f"Bearer {self.token}"}

        response = self.client.get("/smartDevices/" + self.property_id + "/devices?PageNumber=1&PageSize=100",
                                   headers=headers)
        if response.status_code != 200:
            print("Greška prilikom dobavljanja uredjaja")
            return
        response_data = json.loads(response.text)
        sprinkler = None
        for device in response_data:
            if device.get("smartDeviceType") == 5:
                sprinkler = device
                break
        if sprinkler is None:
            print("Korisnik nema Sprinkler")
            return

        url = f"/smartDevices/turn-on-off/{sprinkler.get('id')}"
        is_on = random.choice([True, False])

        response2 = self.client.put(url, json={'isOn': is_on}, headers=headers)

    @task
    def change_sprinkler_start_time(self):
        headers = {"Authorization": f"Bearer {self.token}"}
        devices = self.client.get("/smartDevices/" + self.property_id + "/devices?PageNumber=1&PageSize=100",
                                  headers=headers)
        if devices.status_code != 200:
            print("Greška prilikom dobavljanja uredjaja")
            return
        response_data = json.loads(devices.text)
        sprinkler = None
        for device in response_data:
            if device.get("smartDeviceType") == 5:
                sprinkler = device
                break
        if sprinkler is None:
            print("Korisnik nema Sprinkler")
            return
        hour = random.randint(0, 23)
        minute = random.randint(0, 59)
        start_time = f"{hour:02d}:{minute:02d}"
        start_time_data = {
            "startTime": start_time
        }

        response = self.client.put(f"/smartDevices/change-sprinkler-start-time/{sprinkler.get('id')}",
                                   json=start_time_data, headers=headers)

        if response.status_code != 200:
            print("Greška")