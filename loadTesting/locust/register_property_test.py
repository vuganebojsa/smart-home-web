import random
import string
import login
from locust import HttpUser, task, between
class RegularUser(HttpUser):
    wait_time = between(1, 3)  # Vreme čekanja između HTTP zahteva

    def on_start(self):
        self.client.verify = False
        # Prijavljivanje običnog korisnika
        self.token = login.login(self.client)
    def generate_random_string(self, length):
        return ''.join(random.choices(string.ascii_letters + string.digits, k=length))
    @task
    def create_property(self):
        city = random.choice(["Belgrade", "New York", "London", "Tokyo"])
        country = random.choice(["Serbia", "USA", "UK", "Japan"])
        quadrature = random.randint(1, 300)
        numberOfFloors = random.randint(1, 10)
        latitude = random.uniform(-90, 90)
        longitude = random.uniform(-180, 180)
        name = self.generate_random_string(10)
        address = self.generate_random_string(20)

        payload = {
            "typeOfProperty": random.randint(0,1),
            "name": name,
            "address": address,
            "city": city,
            "country": country,
            "quadrature": quadrature,
            "numberOfFloors": numberOfFloors,
            "image": "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAA1JREFUGFdjaJ+2/T8ABlAC1OB5hf4AAAAASUVORK5CYII=",
            "imageType": "jpg",
            "latitude": latitude,
            "longitude": longitude
        }
        headers = {"Authorization": f"Bearer {self.token}"}
        response = self.client.post("/smartProperties/register", json=payload, headers=headers)
        if response.status_code != 200:
            print("Greška prilikom kreiranja nekretnine")