import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { WebcamImage } from 'ngx-webcam';
import { Observable, Subject, takeUntil } from 'rxjs';
import { Inmate, InmateImage, ReleaseInmate } from 'src/app/models/InmateDetails.model';
import { RegisterInmateService } from 'src/app/services/register-inmate.service';

@Component({
  selector: 'app-register-inmate',
  templateUrl: './register-inmate.component.html',
  styleUrls: ['./register-inmate.component.css']
})
export class RegisterInmateComponent implements OnInit {

  constructor(private registerInmateService: RegisterInmateService, private route: ActivatedRoute) { }

  destroy$: Subject<boolean> = new Subject<boolean>();
  lastName: string = '';
  firstName: string = '';
  gender: string = '';
  dob = '';
  admitDate = new Date();
  imageSource = '';
  private trigger: Subject<any> = new Subject();
  public webcamImage!: WebcamImage;
  private nextWebcam: Subject<any> = new Subject();
  openCamera = false;
  inTakeId = '';
  selectedImage: File | null = null;
  base64Image = '';
  isRelease = false;
  buttonName = 'Submit';
  releaseDate = '';
  imageName = 'HandCuffed';
  
  ngOnInit(): void {
    document.body.className = "selector";
    this.route.data.subscribe((data: any) => {
      if (data.IsRelease) {
        this.isRelease = true;
        this.buttonName = 'Release';
        this.imageName = 'HandCuffFree'
      }
    });
  }

  submitInmateDetails() {
    if (this.selectedImage == null) {
      alert("Please upload or Capture an Image");
      return;
    }

    if (this.inTakeId == null || this.inTakeId == '') {
      alert("Intake ID is required");
      return;
    }

    if (this.buttonName == 'Submit') {
      this.InmateIntake();
    } else {
      this.ReleaseInmate();
    }
    
    this.InitializeInputs();
  }


  displayImage(file: File) {
    const reader = new FileReader();
    reader.onload = (event: any) => {
      this.imageSource = event.target.result;
      this.base64Image = reader.result as string;
      this.findMatch();
    };
    reader.readAsDataURL(file);
  }

  onFileSelected(event: any) {
    this.openCamera = false;
    const file: File = event.target.files[0];                   // event.target.files is an array of FilesList (List of File datatype)
    this.selectedImage = file;
    this.displayImage(file);
  }

  findMatch() {
    this.registerInmateService.verifyIfRegistered(this.base64Image.split(',')[1]).pipe(takeUntil(this.destroy$))
    .subscribe((response: any) => {
      if (response) {
        alert("Match Found")
        this.firstName = response.firstname,
        this.lastName = response.lastname,
        this.gender = response.gender,
        this.dob = response.dob.slice(0, 10)

        if (this.isRelease) {
          this.inTakeId = response.intakeid
          this.admitDate = response.admitdate.slice(0, 10)

          let date = new Date().getDate()
          let month = new Date().getMonth() + 1
          let updatedDate = null;
          let updatedMonth = null;

          if (date < 10) {
            updatedDate = '0' + date
          }

          if (month < 10) {
            updatedMonth = '0' + month
          }

          updatedMonth ??= month;
          updatedDate ??= date
          this.releaseDate = new Date().getFullYear() + '-' + updatedMonth + '-' + updatedDate
        }
      } else {
        alert("No match found");
        this.InitializeInputs()
      }
    })
  }

  getExtension(fileName: string) {
    const parts = fileName.split('.');
    const extension = parts[parts.length - 1];
    return extension.toLowerCase();
  }

  InmateIntake() {
    let inmate: Inmate = {
      FirstName : this.firstName,
      LastName : this.lastName,
      IncarcerationId : 0,
      Gender : this.gender,
      DOB : new Date(this.dob),
      AdmitDate : this.admitDate,
      InTakeId: this.inTakeId,
      Status: 'Active',
      ReleaseDate: null
    }

    let inmateDetails: InmateImage = {
      InmateDetails: inmate,
      Base64Image: this.base64Image.split(',')[1],
      ImageExtension: this.getExtension(this.selectedImage!.name)
    }

    this.registerInmateService.registerInmate(inmateDetails).pipe(takeUntil(this.destroy$))
    .subscribe((response) => {
      if (response == 200) {
        alert("Inmate Registered Successfully");
      }
    })
  }

  ReleaseInmate() {
    let inmateDetails: ReleaseInmate = {
      intakeid: this.inTakeId,
      releaseDate: new Date(this.releaseDate)
    }

    this.registerInmateService.releaseInmate(inmateDetails).pipe(takeUntil(this.destroy$))
    .subscribe((response) => {
      if (response == 200) {
        alert("Inmate Released Successfully");
      }
    })
  }

  InitializeInputs() {
    this.firstName = '';
    this.lastName = '';
    this.gender = '';
    this.dob = '';
    this.admitDate = new Date();
    this.inTakeId = '';
    this.releaseDate = '';
    this.imageSource = '';
    this.selectedImage = null;
  }

  // Camera Capture

  public getSnapshot(): void {
    this.trigger.next(void 0);
    this.openCamera = false;
    this.imageSource = this.webcamImage?.imageAsDataUrl;
  }
  public captureImg(webcamImage: WebcamImage): void {
    this.webcamImage = webcamImage;
    this.base64Image = webcamImage!.imageAsDataUrl;
    this.selectedImage = this.imageToFile(webcamImage);
    this.findMatch();
  }
  public get invokeObservable(): Observable<any> {
    return this.trigger.asObservable();
  }
  public get nextWebcamObservable(): Observable<any> {
    return this.nextWebcam.asObservable();
  }

  onCapture() {
    this.openCamera = true;
    this.imageSource = '';
  }

  imageToFile(webcamImage: WebcamImage): File {
    const imageDataBlob = this.dataURItoBlob(webcamImage.imageAsDataUrl);
    return new File([imageDataBlob], 'webcam_image.jpg');
  }
  
  
  dataURItoBlob(dataURI: string): Blob {
    const byteString = atob(dataURI.split(',')[1]);
    const mimeString = dataURI.split(',')[0].split(':')[1].split(';')[0];
    const arrayBuffer = new ArrayBuffer(byteString.length);
    const uint8Array = new Uint8Array(arrayBuffer);
    for (let i = 0; i < byteString.length; i++) {
      uint8Array[i] = byteString.charCodeAt(i);
    }
    return new Blob([arrayBuffer], { type: mimeString });
  }  
  
  ngOnDestroy() {
    document.body.className="";
    this.destroy$.next(true);
    this.destroy$.unsubscribe();
  }

}
