export interface Inmate {
    FirstName: string;
    LastName: string;
    IncarcerationId: number;
    Gender: string;
    DOB: Date;
    AdmitDate: Date;
    InTakeId: string;
    Status: string;
    ReleaseDate: Date | null;
}

export interface InmateImage {
    InmateDetails: Inmate;
    Base64Image: string;
    ImageExtension: string;
}

export interface ReleaseInmate {
    intakeid: string;
    releaseDate: Date;
}