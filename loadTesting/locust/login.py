
import json
import random


def login(client):
    i = random.randint(0, 2)
    infos = [{"email": "bogdanjanosevic@gmail.com", "password": "Bogdan123"},
             {"email": "nebojsavuga01@gmail.com", "password": "Nebojsa123"},
             {"email": "dusanbibin2@gmail.com", "password": "Dusan123"}]
    login_info = infos[i]
    response = client.post("/users", json=login_info)

    if response.status_code == 200:
        response_data = json.loads(response.text)
        token = response_data.get("token")
        return token
    else:
        raise Exception(f"Login failed with status code {response.status_code}: {response.text}")

def login_admin(client):
    login_info = {"email": "admin", "password": "Admin123"}
    response = client.post("/users", json=login_info)

    if response.status_code == 200:
        response_data = json.loads(response.text)
        token = response_data.get("token")
        return token
    else:
        raise Exception(f"Login failed with status code {response.status_code}: {response.text}")
    
def login_nebojsa(client):
    login_info = {"email": "nebojsavuga01@gmail.com", "password": "Nebojsa123"}
    response = client.post("/users", json=login_info)

    if response.status_code == 200:
        response_data = json.loads(response.text)
        token = response_data.get("token")
        return token
    else:
        raise Exception(f"Login failed with status code {response.status_code}: {response.text}")

def login_ret_user(client):
    i = random.randint(0, 2)
    infos = [{"email": "bogdanjanosevic@gmail.com", "password": "Bogdan123"},
             {"email": "nebojsavuga01@gmail.com", "password": "Nebojsa123"},
             {"email": "dusanbibin2@gmail.com", "password": "Dusan123"}]
    login_info = infos[i]
    response = client.post("/users", json=login_info)

    if response.status_code == 200:
        response_data = json.loads(response.text)
        token = response_data.get("token")
        return token, infos[i]
    else:
        raise Exception(f"Login failed with status code {response.status_code}: {response.text}")