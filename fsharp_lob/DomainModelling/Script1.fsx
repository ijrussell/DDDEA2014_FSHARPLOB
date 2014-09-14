module EmailAddress = 

    type T = EmailAddress of string

    // wrap
    let create (s:string) = 
        if System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$") 
            then Some (EmailAddress s)
            else None
    
    // unwrap
    let value (EmailAddress e) = e

module ContactInfo = 
    open System

    // placeholder
    type EmailAddress = string

    // UnverifiedData = just the email
    type UnverifiedData = EmailAddress

    // VerifiedData = email plus the time it was verified
    type VerifiedData = EmailAddress * DateTime 

    // set of states
    type T = 
        | UnverifiedState of UnverifiedData
        | VerifiedState of VerifiedData

    let create email = 
        // unverified on creation
        UnverifiedState email

    // handle the "verified" event
    let verified emailContactInfo dateVerified = 
        match emailContactInfo with
        | UnverifiedState email ->
            // construct a new info in the verified state
            VerifiedState (email, dateVerified) 
        | VerifiedState _ ->
            // ignore
            emailContactInfo

    let sendVerificationEmail emailContactInfo = 
        match emailContactInfo with
        | UnverifiedState email ->
            // send email
            printfn "sending email"
        | VerifiedState _ ->
            // do nothing
            ()

    let sendPasswordReset emailContactInfo = 
        match emailContactInfo with
        | UnverifiedState email ->
            // ignore
            ()
        | VerifiedState _ ->
            // ignore
            printfn "sending password reset"


type PersonalInfo = {
    FirstName: string
    MiddleInitial: string option
    LastName: string
}

type Address = {
    Address1: string
    Address2: string option
    City: string
    County: string
    PostCode: string
}

type AddressInfo = {
    Address: Address
    //true if validated against address service
    IsAddressValid: bool 
}

type Contact = {
    PersonalInfo: PersonalInfo
    ContactInfo: ContactInfo.T
    AddressInfo: AddressInfo
}

let personalInfo = { 
    FirstName = "Ian"
    MiddleInitial = Some("John")
    LastName = "Russell"
}

let email = EmailAddress.create "ian@craftycoders.net"

let contactInfo = ContactInfo.create (EmailAddress.value email.Value) //"ian@craftycoders.net"

let address = { Address1 = "1 High Street"; Address2 = None; City = "Coventry"; County = "Warwickshire"; PostCode = "CV1 2TT"; }

let addressInfo = { Address=address; IsAddressValid = false; }

let contact1 = { 
    PersonalInfo=personalInfo
    ContactInfo=contactInfo
    AddressInfo=addressInfo
}

printfn "%s" contact1.PersonalInfo.MiddleInitial.Value

ContactInfo.sendVerificationEmail contact1.ContactInfo

let verifiedDate = System.DateTime(2014, 05, 28)

let newContactInfo = ContactInfo.verified contact1.ContactInfo verifiedDate

let contact2 = {
    PersonalInfo=personalInfo
    ContactInfo=newContactInfo
    AddressInfo=addressInfo 
}

//contact1.ContactInfo <- newContactInfo

ContactInfo.sendPasswordReset contact1.ContactInfo