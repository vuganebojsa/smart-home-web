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
    def get_report_from_to_with_executer(self):
        headers = {"Authorization": f"Bearer {self.token}"}
        response = self.client.get("/smartDevices/" + self.property_id  + "/devices?PageNumber=1&PageSize=100", headers=headers)
        if response.status_code != 200:
            print("Greška prilikom dobavljanja uredjaja")
            return
        response_data = json.loads(response.text)
        charger = None
        for device in response_data:
            if device.get("smartDeviceType") == 8:
                charger = device
                break
        if charger is None:
            print("Korisnik nema SPS")
            return

        url = f"/smartDevices/vehicle-charger/{charger.get('id')}"
        response = self.client.get(url, headers=headers)
        url = f"/reports/vehicle-charger-history/{charger.get('id')}"
        response = self.client.get(url, headers=headers)
        if response.status_code != 200:
            print('Nevalidan request')
            return
        
        today = datetime.today()
        tomorrow = today - timedelta(days=1)
        four_days_ago = today - timedelta(days=12)
        from_date_str = four_days_ago.strftime("%Y-%m-%d")
        to_date_str = tomorrow.strftime("%Y-%m-%d")
        url = f"/reports/vehicle-charger-history-in-range-with-executer/{charger.get('id')}?from={from_date_str}&to={to_date_str}&executer=nebojsavuga"
        response = self.client.get(url, headers=headers)
        if response.status_code != 200:
            print('Nevalidan request')
    
    @task
    def get_report_from_to(self):
        headers = {"Authorization": f"Bearer {self.token}"}
        response = self.client.get("/smartDevices/" + self.property_id  + "/devices?PageNumber=1&PageSize=100", headers=headers)
        if response.status_code != 200:
            print("Greška prilikom dobavljanja uredjaja")
            return
        response_data = json.loads(response.text)
        charger = None
        for device in response_data:
            if device.get("smartDeviceType") == 8:
                charger = device
                break
        if charger is None:
            print("Korisnik nema SPS")
            return

        url = f"/smartDevices/vehicle-charger/{charger.get('id')}"
        response = self.client.get(url, headers=headers)
        url = f"/reports/vehicle-charger-history/{charger.get('id')}"
        response = self.client.get(url, headers=headers)
        if response.status_code != 200:
            print('Nevalidan request')
            return
        
        today = datetime.today()
        tomorrow = today - timedelta(days=1)
        four_days_ago = today - timedelta(days=12)
        from_date_str = four_days_ago.strftime("%Y-%m-%d")
        to_date_str = tomorrow.strftime("%Y-%m-%d")
        url = f"/reports/vehicle-charger-history-in-range/{charger.get('id')}?from={from_date_str}&to={to_date_str}"
        response = self.client.get(url, headers=headers)
        if response.status_code != 200:
            print('Nevalidan request')

    @task(4)
    def change_charge_percentage(self):
        headers = {"Authorization": f"Bearer {self.token}"}
        response = self.client.get("/smartDevices/" + self.property_id  + "/devices?PageNumber=1&PageSize=100", headers=headers)
        if response.status_code != 200:
            print("Greška prilikom dobavljanja uredjaja")
            return
        response_data = json.loads(response.text)
        charger = None
        for device in response_data:
            if device.get("smartDeviceType") == 8:
                charger = device
                break
        if charger is None:
            print("Korisnik nema SPS")
            return

        url = f"/smartDevices/vehicle-charger/{charger.get('id')}"
        percentage = random.randint(10, 100)
        response = self.client.get(url, headers=headers)
        url = f"/smartDevices/vehicle-charge/{charger.get('id')}"

        response = self.client.put(url, json = {'percentageOfCharge': percentage}, headers=headers)