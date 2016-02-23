module XltEnums
    open Enums

    // Defines the types
    type PropertyTypes = 
        | Id      = 1 
        | Text    = 2
        | ToolTip = 3
        | Page    = 4
    type Criterias =
        | None      = 0
        | Exact     = 1
        | StartWith = 2
        | EndWith   = 3
        | Contains  = 4

    // Creates an object of type PropertyType from the input string
    let ToPropertyType value = 
        GetEnumState typeof<PropertyTypes> value

    // Creates an object of type Criterias from the input string
    let ToCriteria value =
        GetEnumState typeof<Criterias> value
    