module Enums
    open System

    //////////////////////////

    // Get enumeration state
    let GetEnumState myType (value : string) =
        
        // Filters a string array and finds the correct Enumeration
        try
            Some (Enum.GetNames(myType) |> Seq.filter (fun x -> x.ToLower() = value.ToLower()) |> Seq.head)
        with _ -> eprintf "%s:GetEnumState: Enumeration don't contain value '%s'" (myType.ToString()) value; None
        //match enumName with
        //    | Some string -> enumName//(Enum.Parse(myType, enumName.Value))
        //    | None -> None