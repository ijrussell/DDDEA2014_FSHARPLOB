type Contact = {
    FirstName: string
    MiddleInitial: string
    LastName: string

    EmailAddress: string
    //true if ownership of email address is confirmed
    IsEmailVerified: bool

    Address1: string
    Address2: string
    City: string
    County: string
    PostCode: string
    //true if validated against address service
    IsAddressValid: bool 
}
