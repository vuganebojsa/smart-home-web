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
        self.token, self.user = login.login_ret_user(self.client)
             
    def generate_random_string(self, length):
        return ''.join(random.choices(string.ascii_letters + string.digits, k=length))

    @task
    def turn_system_on_off(self):
        headers = {"Authorization": f"Bearer {self.token}"}
        device_ids = []
        choice = random.randint(0, len(device_ids) - 1)

        url = f"/smartDevices/turn-on-off/{device_ids[choice]}"
        is_on = random.choice([True, False])

        response = self.client.put(url, json = {'isOn': is_on}, headers=headers)