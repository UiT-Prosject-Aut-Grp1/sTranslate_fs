namespace sTranslate_fs

module Enums =
    open System
    open Model

    //////////////////////////

    // Get enumeration state
    let GetEnumState myType (value : string) =
        
        // Filters a string array and finds the correct Enumeration
        Enum.GetNames(myType)
        |> Seq.filter (fun x -> x.ToLower() = value.ToLower())
        |> checkHead

        //match enumName with
        //    | Some string -> enumName//(Enum.Parse(myType, enumName.Value))
        //    | None -> None