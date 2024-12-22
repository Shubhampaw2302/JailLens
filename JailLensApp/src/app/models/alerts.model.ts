export interface Alerts {
    jaillensalertid: number,
    intakeid: string,
    programname: string,
    inmatename: string,
    alertdescription: string,
    alertcategory: string,
    createddate: Date,
    comments: string | null,
    isprocessed: number,
    actualprogramname: string
}