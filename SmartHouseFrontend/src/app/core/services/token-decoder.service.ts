import { Injectable } from '@angular/core';
import jwtDecode from 'jwt-decode';

@Injectable({
  providedIn: 'root'
})
export class TokenDecoderService {

  constructor() { }

  getDecodedAccesToken():any{
    try{
        return jwtDecode(localStorage.getItem('token'));
    }catch(Error){
      return null;
    }
  }
}
