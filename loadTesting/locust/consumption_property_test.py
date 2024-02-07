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
    def get_report_time_period(self):
        headers = {"Authorization": f"Bearer {self.token}"}
        time_period = random.randint(0, 3)
        response = self.client.get("/reports/property-energy-consumption-time-period/" + self.property_id  + "/" + str(time_period), headers=headers)
        if response.status_code != 200:
            print("Greška prilikom dobavljanja izvestaja")
            return
    
    @task
    def get_report_from_to(self):
        headers = {"Authorization": f"Bearer {self.token}"}
        today = datetime.today()
        tomorrow = today - timedelta(days=1)
        four_days_ago = today - timedelta(days=12)
        from_date_str = four_days_ago.strftime("%Y-%m-%d")
        to_date_str = tomorrow.strftime("%Y-%m-%d")
        url = f"/reports/property-energy-consumption-from-to/{self.property_id}?from={from_date_str}&to={to_date_str}"
        response = self.client.get(url, headers=headers)
        if response.status_code != 200:
            print("Greška prilikom dobavljanja izvestaja")
        