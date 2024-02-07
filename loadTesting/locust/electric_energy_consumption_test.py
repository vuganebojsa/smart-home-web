import random
import string
import login
from locust import HttpUser, task, between
import json
class RegularUser(HttpUser):
    wait_time = between(1, 3)  # Vreme čekanja između HTTP zahteva

    def on_start(self):
        self.client.verify = False
        # Prijavljivanje običnog korisnika
        self.token = login.login_admin(self.client)
    def generate_random_string(self, length):
        return ''.join(random.choices(string.ascii_letters + string.digits, k=length))
    @task
    def get_report_for_property(self):
        headers = {"Authorization": f"Bearer {self.token}"}
        response = self.client.get("/smartProperties/get-properties?PageSize=10&PageNumber=1", headers=headers)
        response_data = json.loads(response.text)
        if response.status_code != 200:
            print("Greška prilikom dobavljanja propertija")
            return
        if len(response_data) == 0:
            print("Greška prilikom dobavljanja propertija")
            return
        property = response_data[0]
        time_period = random.randint(0, 4)
        response = self.client.get("/reports/energy-consumed-property/" + property.get("id")  + "/" + str(False) + "/" + str(time_period), headers=headers)
        if response.status_code != 200:
            print("Greška prilikom dobavljanja propertija")
            return
        response = self.client.get("/reports/energy-consumed-property/" + property.get("id")  + "/" + str(True) + "/" + str(time_period), headers=headers)
        if response.status_code != 200:
            print("Greška prilikom dobavljanja propertija")
            return
    
    @task
    def get_report_for_city(self):
        headers = {"Authorization": f"Bearer {self.token}"}
        response = self.client.get("/smartProperties/GetCities", headers=headers)
        response_data = json.loads(response.text)
        if response.status_code != 200:
            print("Greška prilikom dobavljanja propertija")
            return
        if len(response_data) < 3:
            print("Greška prilikom dobavljanja propertija")
            return
        city_choice = random.randint(0, 2)
        city = response_data[city_choice]
        time_period = random.randint(0, 4)
        response = self.client.get("/reports/energy-consumed-city/" + city.get("name")  + "/" + str(False) + "/" + str(time_period), headers=headers)
        if response.status_code != 200:
            print("Greška prilikom dobavljanja propertija")
            return
        response = self.client.get("/reports/energy-consumed-city/" + city.get("name")  + "/" + str(True) + "/" + str(time_period), headers=headers)
        if response.status_code != 200:
            print("Greška prilikom dobavljanja propertija")
            return