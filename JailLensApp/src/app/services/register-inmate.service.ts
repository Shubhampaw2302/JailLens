import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { InmateImage, ReleaseInmate } from '../models/InmateDetails.model';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class RegisterInmateService {

  constructor(private httpservice: HttpClient) { }

  registerInmate(inputData: InmateImage) {
    return this.httpservice.post(`${environment.apiUrl}/Inmate/Register`, inputData)
  }

  verifyIfRegistered(base64: string) {
    return this.httpservice.post(`${environment.apiUrl}/Inmate/FindMatch`, {image: base64})
  }

  releaseInmate(inputData: ReleaseInmate) {
    return this.httpservice.post(`${environment.apiUrl}/Inmate/Release`, inputData)
  }
}
