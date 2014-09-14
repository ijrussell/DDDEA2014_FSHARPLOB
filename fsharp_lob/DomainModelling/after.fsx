// ========================================
// WrappedString 
// ========================================

/// Common code for wrapped strings
module WrappedString = 

    /// An interface that all wrapped strings support
    type IWrappedString = 
        abstract Value : string

    /// Create a wrapped value option
    /// 1) canonicalize the input first
    /// 2) If the validation succeeds, return Some of the given constructor
    /// 3) If the validation fails, return None
    /// Null values are never valid.
    let create canonicalize isValid ctor (s:string) = 
        if s = null 
        then None
        else
            let s' = canonicalize s
            if isValid s'
            then Some (ctor s') 
            else None

    /// Apply the given function to the wrapped value
    let apply f (s:IWrappedString) = 
        s.Value |> f 

    /// Get the wrapped value
    let value s = apply id s

    /// Equality 
    let equals left right = 
        (value left) = (value right)

    /// Comparison
    let compareTo left right = 
        (value left).CompareTo (value right)

    /// Canonicalizes a string before construction
    /// * converts all whitespace to a space char
    /// * trims both ends
    let singleLineTrimmed s =
        System.Text.RegularExpressions.Regex.Replace(s,"\s"," ").Trim()

    /// A validation function based on length
    let lengthValidator len (s:string) =
        s.Length <= len 

    /// A string of length 100
    type String100 = String100 of string with
        interface IWrappedString with
            member this.Value = let (String100 s) = this in s

    /// A constructor for strings of length 100
    let string100 = create singleLineTrimmed (lengthValidator 100) String100 

    /// Converts a wrapped string to a string of length 100
    let convertTo100 s = apply string100 s

    /// A string of length 50
    type String50 = String50 of string with
        interface IWrappedString with
            member this.Value = let (String50 s) = this in s

    /// A constructor for strings of length 50
    let string50 = create singleLineTrimmed (lengthValidator 50)  String50

    /// Converts a wrapped string to a string of length 50
    let convertTo50 s = apply string50 s

    /// map helpers
    let mapAdd k v map = 
        Map.add (value k) v map    

    let mapContainsKey k map =  
        Map.containsKey (value k) map    

    let mapTryFind k map =  
        Map.tryFind (value k) map    

// ========================================
// Email address (not application specific)
// ========================================

module EmailAddress = 

    type T = EmailAddress of string with 
        interface WrappedString.IWrappedString with
            member this.Value = let (EmailAddress s) = this in s

    let create = 
        let canonicalize = WrappedString.singleLineTrimmed 
        let isValid s = 
            (WrappedString.lengthValidator 100 s) &&
            System.Text.RegularExpressions.Regex.IsMatch(s,@"^\S+@\S+\.\S+$") 
        WrappedString.create canonicalize isValid EmailAddress

    /// Converts any wrapped string to an EmailAddress
    let convert s = WrappedString.apply create s

// ========================================
// ZipCode (not application specific)
// ========================================

module ZipCode = 

    type T = ZipCode of string with
        interface WrappedString.IWrappedString with
            member this.Value = let (ZipCode s) = this in s

    let create = 
        let canonicalize = WrappedString.singleLineTrimmed 
        let isValid s = 
            System.Text.RegularExpressions.Regex.IsMatch(s,@"^\d{5}$") 
        WrappedString.create canonicalize isValid ZipCode

    /// Converts any wrapped string to a ZipCode
    let convert s = WrappedString.apply create s

// ========================================
// StateCode (not application specific)
// ========================================

module StateCode = 

    type T = StateCode  of string with
        interface WrappedString.IWrappedString with
            member this.Value = let (StateCode  s) = this in s

    let create = 
        let canonicalize = WrappedString.singleLineTrimmed 
        let stateCodes = ["AZ";"CA";"NY"] //etc
        let isValid s = 
            stateCodes |> List.exists ((=) s)

        WrappedString.create canonicalize isValid StateCode

    /// Converts any wrapped string to a StateCode
    let convert s = WrappedString.apply create s

// ========================================
// PostalAddress (not application specific)
// ========================================

module PostalAddress = 

    type USPostalAddress = 
        {
        Address1: WrappedString.String50;
        Address2: WrappedString.String50;
        City: WrappedString.String50;
        State: StateCode.T;
        Zip: ZipCode.T;
        }

    type UKPostalAddress = 
        {
        Address1: WrappedString.String50;
        Address2: WrappedString.String50;
        Town: WrappedString.String50;
        PostCode: WrappedString.String50;   // todo
        }

    type GenericPostalAddress = 
        {
        Address1: WrappedString.String50;
        Address2: WrappedString.String50;
        Address3: WrappedString.String50;
        Address4: WrappedString.String50;
        Address5: WrappedString.String50;
        }

    type T = 
        | USPostalAddress of USPostalAddress 
        | UKPostalAddress of UKPostalAddress 
        | GenericPostalAddress of GenericPostalAddress 

// ========================================
// PersonalName (not application specific)
// ========================================

module PersonalName = 
    open WrappedString

    type T = 
        {
        FirstName: String50;
        MiddleName: String50 option;
        LastName: String100;
        }

    /// create a new value
    let create first middle last = 
        match (string50 first),(string100 last) with
        | Some f, Some l ->
            Some {
                FirstName = f;
                MiddleName = (string50 middle)
                LastName = l;
                }
        | _ -> 
            None

    /// concat the names together        
    /// and return a raw string
    let fullNameRaw personalName = 
        let f = personalName.FirstName |> value 
        let l = personalName.LastName |> value 
        let names = 
            match personalName.MiddleName with
            | None -> [| f; l |]
            | Some middle -> [| f; (value middle); l |]
        System.String.Join(" ", names)

    /// concat the names together        
    /// and return None if too long
    let fullNameOption personalName = 
        personalName |> fullNameRaw |> string100

    /// concat the names together        
    /// and truncate if too long
    let fullNameTruncated personalName = 
        // helper function
        let left n (s:string) = 
            if (s.Length > n) 
            then s.Substring(0,n)
            else s

        personalName 
        |> fullNameRaw  // concat
        |> left 100     // truncate
        |> string100    // wrap
        |> Option.get   // this will always be ok
//And now the application specific types.

// ========================================
// EmailContactInfo -- state machine
// ========================================

module EmailContactInfo = 
    open System

    // UnverifiedData = just the EmailAddress
    type UnverifiedData = EmailAddress.T

    // VerifiedData = EmailAddress plus the time it was verified
    type VerifiedData = EmailAddress.T * DateTime 

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

// ========================================
// PostalContactInfo -- state machine
// ========================================

module PostalContactInfo = 
    open System

    // InvalidData = just the PostalAddress
    type InvalidData = PostalAddress.T

    // ValidData = PostalAddress plus the time it was verified
    type ValidData = PostalAddress.T * DateTime 

    // set of states
    type T = 
        | InvalidState of InvalidData
        | ValidState of ValidData

    let create address = 
        // invalid on creation
        InvalidState address

    // handle the "validated" event
    let validated postalContactInfo dateValidated = 
        match postalContactInfo with
        | InvalidState address ->
            // construct a new info in the valid state
            ValidState (address, dateValidated) 
        | ValidState _ ->
            // ignore
            postalContactInfo 

    let contactValidationService postalContactInfo = 
        let dateIsTooLongAgo (d:DateTime) =
            d < DateTime.Today.AddYears(-1)

        match postalContactInfo with
        | InvalidState address ->
            printfn "contacting the address validation service"
        | ValidState (address,date) when date |> dateIsTooLongAgo  ->
            printfn "last checked a long time ago."
            printfn "contacting the address validation service again"
        | ValidState  _ ->
            printfn "recently checked. Doing nothing."

// ========================================
// ContactMethod and Contact
// ========================================

type ContactMethod = 
    | Email of EmailContactInfo.T 
    | PostalAddress of PostalContactInfo.T

type Contact = 
    {
    Name: PersonalName.T;
    PrimaryContactMethod: ContactMethod;
    SecondaryContactMethods: ContactMethod list;
    }
