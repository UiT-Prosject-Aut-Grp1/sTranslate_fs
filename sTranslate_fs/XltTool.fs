namespace sTranslate_fs

module XltTool =
    open System
    open System.Data
    open System.Data.Linq
    open FSharp.Data.TypeProviders
    open Microsoft.FSharp.Linq
    open FSharp.Configuration
    open XltEnums

    // Get typed access to App.config to fetch the connection string later
    type Settings = AppSettings<"App.config">

    // SQL Type Provider. Give it a dummy connection string to satisfy the compiler.
    type dbSchema = SqlDataConnection<ConnectionStringName = "dbConnectionString">

    // Record type so that we can cache the database to memory and not have the data context go out of scope
    type Translation = {
            Id : int
            FromText : string
            ToText : string
            Context : string
            Property : string
            Criteria : string
            FromLang : string
            ToLang : string
        }

    // Copy the contents of a database row into a record type
    let toTranslation (xlt : dbSchema.ServiceTypes.Translation) = {
            Id = xlt.Id
            FromText = xlt.FromText
            ToText = xlt.ToText
            Context = xlt.Context
            Property = xlt.Property
            Criteria = xlt.Criteria
            FromLang = xlt.FromLang
            ToLang = xlt.ToLang
        }

    // Database only supports translating from english for now
    let FromLanguageCode = "en"

    // Copies the database to memory
    let GetTranslations =
        use db = dbSchema.GetDataContext(Settings.ConnectionStrings.DbConnectionString)
        query {
            for row in db.Translation do 
                select row
        } |> Seq.toList |> List.map toTranslation

    let FindTranslation collectionFunction (criteria : Option<string>) (fromText : string) (property : Option<string>) (context : string) toLanguageCode =
        // If fromtext does not contain a word, return an empty string
        match fromText.Trim() with
        | "" -> ""
        | _ ->

            // If criteria or property was not found in enums, return the original string
            match criteria, property with
            | None,_ | _,None -> fromText
            | Some criteria, Some property ->
            
                // If toLanguageCode is not valid, sets it to default "no"
                let toLang =
                    match toLanguageCode with
                    | null | "" -> "no"
                    | _ -> toLanguageCode

                // Search for a valid translation
                let result =
                    collectionFunction
                    |> List.tryFind ( fun row -> 
                        row.Criteria.ToLower() = criteria.ToLower() &&
                        row.FromLang = FromLanguageCode &&
                        row.FromText = fromText &&
                        row.Property.ToLower() = property.ToLower() &&
                        row.Context.ToLower() = context.ToLower() &&
                        row.ToLang = toLang )

                match result with
                    | Some x -> x.ToText
                    | None -> fromText

    //////////////////////////////////////////////////////////////////////////////////////////////
    //     GetToText function returns the translated string, if defined in the Translate table. 
    //     If the fromText is not found, the value of fromText is returned unchanged.
    let GetToText (criteria : Option<string>) (fromText : string) (property : Option<string>) (context : string) toLanguageCode =
        FindTranslation GetTranslations criteria fromText property context toLanguageCode