import json
from datetime import datetime, timedelta
import random
import string

from dateutil.relativedelta import relativedelta

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
    def get_last_6_hours_history(self):
        headers = {"Authorization": f"Bearer {self.token}"}
        devices = self.client.get("/smartDevices/" + self.property_id + "/devices?PageNumber=1&PageSize=100",
                                  headers=headers)
        devices_data = json.loads(devices.text)
        if len(devices_data) == 0:
            return
        id = None
        ids = []
        for device in devices_data:
            if device.get("smartDeviceType") == 5:
                ids.append(device.get("id"))
        id = random.choice(ids)
        if id == None:
            print("User has no sprinklers")
            return
        current_time = datetime.now()
        time_before_6_hours = current_time - timedelta(hours=6)
        response = self.client.get(f"/reports/getSprinklerEventHistory/{id}",
                                   params={"startDate": time_before_6_hours, "endDate": current_time}, headers=headers)
        if response.status_code != 200:
            print("Greška")

    @task
    def get_last_12_hours_history(self):
        headers = {"Authorization": f"Bearer {self.token}"}
        devices = self.client.get("/smartDevices/" + self.property_id + "/devices?PageNumber=1&PageSize=100",
                                  headers=headers)
        devices_data = json.loads(devices.text)
        if len(devices_data) == 0:
            return
        id = None
        ids = []
        for device in devices_data:
            if device.get("smartDeviceType") == 5:
                ids.append(device.get("id"))
        id = random.choice(ids)
        if id == None:
            print("User has no sprinklers")
            return
        current_time = datetime.now()
        time_before_12_hours = current_time - timedelta(hours=12)
        response = self.client.get(f"/reports/getSprinklerEventHistory/{id}",
                                   params={"startDate": time_before_12_hours, "endDate": current_time}, headers=headers)
        if response.status_code != 200:
            print("Greška")

    @task
    def get_last_24_hours_history(self):
        headers = {"Authorization": f"Bearer {self.token}"}
        devices = self.client.get("/smartDevices/" + self.property_id + "/devices?PageNumber=1&PageSize=100",
                                  headers=headers)
        devices_data = json.loads(devices.text)
        if len(devices_data) == 0:
            return
        id = None
        ids = []
        for device in devices_data:
            if device.get("smartDeviceType") == 5:
                ids.append(device.get("id"))
        id = random.choice(ids)
        if id == None:
            print("User has no sprinklers")
            return
        current_time = datetime.now()
        time_before_24_hours = current_time - timedelta(hours=24)

        response = self.client.get(f"/reports/getSprinklerEventHistory/{id}",
                                   params={"startDate": time_before_24_hours, "endDate": current_time}, headers=headers)
        if response.status_code != 200:
            print("Greška")

    @task
    def get_last_7_days_history(self):
        headers = {"Authorization": f"Bearer {self.token}"}
        devices = self.client.get("/smartDevices/" + self.property_id + "/devices?PageNumber=1&PageSize=100",
                                  headers=headers)
        devices_data = json.loads(devices.text)
        if len(devices_data) == 0:
            return
        id = None
        ids = []
        for device in devices_data:
            if device.get("smartDeviceType") == 5:
                ids.append(device.get("id"))
        id = random.choice(ids)
        if id == None:
            print("User has no sprinklers")
            return
        current_time = datetime.now()
        time_before_7_days = current_time - timedelta(days=7)
        response = self.client.get(f"/reports/getSprinklerEventHistory/{id}",
                                   params={"startDate": time_before_7_days, "endDate": current_time}, headers=headers)
        if response.status_code != 200:
            print("Greška")

    @task
    def get_last_month_ago_history(self):
        headers = {"Authorization": f"Bearer {self.token}"}
        devices = self.client.get("/smartDevices/" + self.property_id + "/devices?PageNumber=1&PageSize=100",
                                  headers=headers)
        devices_data = json.loads(devices.text)
        if len(devices_data) == 0:
            return
        id = None
        ids = []
        for device in devices_data:
            if device.get("smartDeviceType") == 5:
                ids.append(device.get("id"))
        id = random.choice(ids)
        if id == None:
            print("User has no sprinklers")
            return
        current_time = datetime.now()
        one_month_ago = current_time - relativedelta(months=1)
        response = self.client.get(f"/reports/getSprinklerEventHistory/{id}",
                                   params={"startDate": one_month_ago, "endDate": current_time}, headers=headers)
        if response.status_code != 200:
            print("Greška")

    @task
    def get_custom_valid_history(self):
        headers = {"Authorization": f"Bearer {self.token}"}
        devices = self.client.get("/smartDevices/" + self.property_id + "/devices?PageNumber=1&PageSize=100",
                                  headers=headers)
        devices_data = json.loads(devices.text)
        if len(devices_data) == 0:
            return
        id = None
        ids = []
        for device in devices_data:
            if device.get("smartDeviceType") == 5:
                ids.append(device.get("id"))
        id = random.choice(ids)
        if id == None:
            print("User has no sprinklers")
            return
        current_time = datetime.now()
        randomStart = current_time - relativedelta(days=random.randint(1,27))
        response = self.client.get(f"/reports/getSprinklerEventHistory/{id}",
                                   params={"startDate": randomStart, "endDate": current_time}, headers=headers)
        if response.status_code != 200:
            print("Greška")

    @task(3)
    def get_custom_valid_history_with_username(self):
        headers = {"Authorization": f"Bearer {self.token}"}
        devices = self.client.get("/smartDevices/" + self.property_id + "/devices?PageNumber=1&PageSize=100",
                                  headers=headers)
        devices_data = json.loads(devices.text)
        if len(devices_data) == 0:
            return
        id = None
        ids = []
        for device in devices_data:
            if device.get("smartDeviceType") == 5:
                ids.append(device.get("id"))
        id = random.choice(ids)
        if id == None:
            print("User has no sprinklers")
            return
        current_time = datetime.now()
        randomStart = current_time - relativedelta(days=random.randint(1, 27))
        response = self.client.get(f"/reports/getSprinklerEventHistory/{id}",
                                   params={"startDate": randomStart, "endDate": current_time, "username":"bogdanjanosevic"}, headers=headers)
        if response.status_code != 200:
            print("Greška")