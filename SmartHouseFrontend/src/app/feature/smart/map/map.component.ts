import { AfterViewInit, Component, ElementRef, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import * as L from 'leaflet';
import { MapService } from 'src/app/core/services/map.service';
@Component({
  selector: 'app-map',
  templateUrl: './map.component.html',
  styleUrls: ['./map.component.css']
})
export class MapComponent implements AfterViewInit{
  @Output() coordinatesClicked = new EventEmitter<{ lat: number, lon: number, city:string, country:string, address:string }>();
  @ViewChild('map') mapElement: ElementRef;
  private map: any;
  city = ''
  private pin: L.Marker;
  cityLat: number = 0;
  cityLon: number = 0;
  constructor(private mapService: MapService) { }
  private initMap(): void {
    this.map = L.map('map', {
      center: [45.2396, 19.8227],
      zoom: 13,
    });
    this.map.on('click', this.handleMapClick.bind(this));

    const tiles = L.tileLayer(
      'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png',
      {
        maxZoom: 18,
        minZoom: 3,
        attribution:
          '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>',
      }
    );
    tiles.addTo(this.map);
    
   // Postavi centar mape na koordinate grada


    // this.search();
    // this.addMarker();
    // this.registerOnClick();
    // this.route();
  }
  public updateMapView(lat: number, lng: number) {
    this.map.setView([lat, lng], 13); // Postavite prikaz na odabrane koordinate
    
    // Dodajte pin na kartu
    if (this.pin) {
      this.map.removeLayer(this.pin);
    }

    this.pin = L.marker([lat, lng]).addTo(this.map);
  }
  private handleMapClick(event: L.LeafletMouseEvent) {
    const lat = event.latlng.lat;
    const lon = event.latlng.lng;

    this.mapService.reverseSearch(lat, lon).subscribe(data => {
      const country = data.address.country;
      let city = data.address.city;
      
      if(city== undefined){
        city = data.address.city_district;
       
      }
      if (city == undefined){
        city = data.address.village;
      }
      if (city == undefined){
        city = data.address.town;
      }
      if (city == undefined){
        city = data.address.municipality;
      }
      if (city == undefined){
        city = data.address.county;
      }
      
    
      let address = data.address.road
      
      const coordinates = { lat, lon, country, city, address };
      this.coordinatesClicked.emit(coordinates);

      if (this.pin) {
        this.map.removeLayer(this.pin);
      }

      this.pin = L.marker([lat, lon]).addTo(this.map).bindPopup('latitude: ' + lat + ", longitude: " + lon + " have been chosen.")
      .openPopup();;
    });
  }


  



  ngAfterViewInit(): void {
    let DefaultIcon = L.icon({
      iconUrl: 'https://unpkg.com/leaflet@1.6.0/dist/images/marker-icon.png',
    });

    L.Marker.prototype.options.icon = DefaultIcon;
    this.initMap();
  }

}
