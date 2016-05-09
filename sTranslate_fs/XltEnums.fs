namespace sTranslate_fs

module XltEnums =
    open System

    type PropertyTypes = 
        | Id      = 1 
        | Text    = 2
        | ToolTip = 3
        | Page    = 4
    type CriteriaTypes =
        | None      = 0
        | Exact     = 1
        | StartWith = 2
        | EndWith   = 3
        | Contains  = 4

    // Helper function to give the first element of a sequence, if it contains something
    let checkHead (s : seq<'a>) =
        match Seq.isEmpty s with
        | true -> None
        | false -> Some <| Seq.head s

    // Get enumeration state
    let getEnumState myType (value : string) =
        
        // Filters a string array and finds the correct Enumeration
        Enum.GetNames(myType)
        |> Seq.filter (fun x -> x.ToLower() = value.ToLower())
        |> checkHead

    // Creates an object of type PropertyType from the input string
    let toProperty value = 
        getEnumState typeof<PropertyTypes> value

    // Creates an object of type Criterias from the input string
    let toCriteria value =
        getEnumState typeof<CriteriaTypes> value