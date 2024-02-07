import { HttpClient, HttpEvent, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environment/environment';
import { PendingSmartPropertyListDTO, ProccesRequest, SinglePropertyDTO, SmartPropertyListDTO, SmartPropertyRegisterDTO, SmartPropertyWithUserDTO } from 'src/app/shared/models/SmartProperty';
import { Observable } from 'rxjs';
import { CityDTO } from 'src/app/shared/models/City';

@Injectable({
  providedIn: 'root'
})
export class SmartPropertyService {

  smartPropertyUrl: string = environment.apiHost + 'smartProperties';
  constructor(private http:HttpClient) { }

  getUserProperties(page:number, count:number) : Observable<HttpResponse<SmartPropertyListDTO[]>> {
    return this.http.get<SmartPropertyListDTO[]>(environment.apiHost + "users/GetUserProperties?PageNumber=" + page + '&PageSize=' + count, { observe: 'response' });
  }

  getPendingProperties(page:number, count:number) : Observable<HttpResponse<PendingSmartPropertyListDTO[]>> {
    return this.http.get<PendingSmartPropertyListDTO[]>(environment.apiHost + "users/GetPendingProperties?PageNumber=" + page + '&PageSize=' + count, { observe: 'response' });
  }

  proccesPendingPropertyRequest(id:string, proccessRequest:ProccesRequest):Observable<string>{
    return this.http.post<string>(environment.apiHost +"users/" + id + "/ProcessRequest", proccessRequest )
  }

  getCities() : Observable<CityDTO[]> {
    return this.http.get<CityDTO[]>( this.smartPropertyUrl+ "/GetCities")
  }

  registerSmartProperty(smartProperty:SmartPropertyRegisterDTO):Observable<PendingSmartPropertyListDTO>{
    return this.http.post<PendingSmartPropertyListDTO>(this.smartPropertyUrl + "/register", smartProperty )
  }
  getProperty(id) : Observable<SinglePropertyDTO> {
    return this.http.get<SinglePropertyDTO>(this.smartPropertyUrl + id)
  }

  getAdminProperties(page:number, count:number) : Observable<HttpResponse<SmartPropertyWithUserDTO[]>> {
    return this.http.get<SmartPropertyWithUserDTO[]>(environment.apiHost + "smartProperties/get-properties?PageNumber=" + page + '&PageSize=' + count, { observe: 'response' });
  }
}
