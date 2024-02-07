from influxdb_client import InfluxDBClient, DeleteApi
from datetime import datetime, timedelta

# InfluxDB setup
bucket = "smarthome"
org = "FTN"
url = "http://localhost:8086"
token = 'enjCkAh2pW6OM19q2oavjrdTS4z4Ft8syGt2abMHWrXH5OM_wO9xR-lvHZ2i08-3mV1BIcVIwWYncefmM7leYA=='
influxdb_client = InfluxDBClient(url=url, token=token, org=org)

# Construct the delete request
delete_api = DeleteApi(influxdb_client)
start_time = (datetime.now() - timedelta(days=3*28)).isoformat()  # 3 months ago
end_time =  (datetime.now() - timedelta(days=4)).isoformat()  # Current time
start_time = start_time[:23] + 'Z'  # Convert to RFC3339Nano format
end_time = end_time[:23] + 'Z'  # Convert to RFC3339Nano format

predicate = '_measurement="electricenergy" and _field="totalProduced" and city="Novi Sad"'
delete_api.delete(org=org, bucket=bucket, start=start_time, stop=end_time, predicate=predicate)
