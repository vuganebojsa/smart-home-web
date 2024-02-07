import random
import string
import login
from locust import HttpUser, task, between
class RegularUser(HttpUser):
    wait_time = between(1, 3)  # Vreme čekanja između HTTP zahteva

    def on_start(self):
        self.client.verify = False
        self.token = login.login_admin(self.client)


    @task
    def accept_property(self):
        headers = {"Authorization": f"Bearer {self.token}"}
        pending_properties = self.client.get("/users/GetPendingProperties?PageSize=5&PageNumber=1", headers=headers).json()
        property = random.choice(pending_properties)
        if len(property)==0:
            return
        id = property["id"]
        payload = {
            "accept": True,
            "reason": ""
        }
        response = self.client.post(f"/users/{id}/ProcessRequest", json=payload, headers=headers)
        if response.status_code != 200:
            print("Greška")

    @task
    def decline_property(self):
        headers = {"Authorization": f"Bearer {self.token}"}
        pending_properties = self.client.get("/users/GetPendingProperties?PageSize=5&PageNumber=1", headers=headers).json()
        property = random.choice(pending_properties)
        if len(property)==0:
            return
        id = property["id"]
        payload = {
            "accept": False,
            "reason": "test decline"
        }
        response = self.client.post(f"/users/{id}/ProcessRequest", json=payload, headers=headers)
        if response.status_code != 200:
            print("Greška")