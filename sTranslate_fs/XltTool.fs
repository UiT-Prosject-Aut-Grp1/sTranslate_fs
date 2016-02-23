module XltTool
    open System
    open System.Data
    open System.Data.Linq
    open FSharp.Data.TypeProviders
    open Microsoft.FSharp.Linq
    open FSharp.Configuration
    open XltEnums
    open Model

    // Get typed access to App.config to fetch the connection string later
    type Settings = AppSettings<"App.config">

    // SQL Type Provider. Give it a dummy connection string to satisfy the compiler.
    type dbSchema = SqlDataConnection<ConnectionStringName = "dbConnectionString">

    // Copy the contents of a database row into a record type
    let toTranslation (xlt : dbSchema.ServiceTypes.Translation) =
        {
            Id = xlt.Id
            FromText = xlt.FromText
            ToText = xlt.ToText
            Context = xlt.Context
            Property = xlt.Property
            Criteria = xlt.Criteria
            FromLang = xlt.FromLang
            ToLang = xlt.ToLang
        }

    // Helper function to give the first element of a sequence, if it contains something
    let checkHead (s : seq<'a>) =
            if Seq.isEmpty s then
                None
            else 
                Some <| Seq.head s

    // Database only supports translating from english for now
    let FromLanguageCode = "en"

    // The cached version of the database
    let mutable _translateColl : List<Translation> = []

    // Reads the entire database and copies it to memory
    let GetTranslations =
        use db = dbSchema.GetDataContext(Settings.ConnectionStrings.DbConnectionString)
        query {
            for row in db.Translation do 
                select row
        } |> Seq.toList |> List.map toTranslation 
    
    /////////////////////////////////////////////////////////////////////////////////////////////
    //     ToText function returns the translated string, if defined in the Translate table. 
    //     If the fromText is not found, the value of fromText is returned unchanged.
    //     If _translateColl is empty, GetTranslations is called to cache the database in memory
    let ToText (criteria : Option<string>) (fromText : string) (property : Option<string>) (context : string) toLanguageCode =
        if fromText.Trim() = "" then "" |> ignore
            
        // If toLanguageCode is not valid, sets it to default "no"
        let toLang =
            match toLanguageCode with
            | null | "" -> "no"
            | _ -> toLanguageCode

        

        // Get the database 
        if _translateColl = [] then
            _translateColl <- GetTranslations
            
        // Search for a valid translation
        let result = 
            _translateColl
            |> Seq.filter (fun row -> row.Criteria.ToLower() = criteria.Value.ToLower())
            |> Seq.filter (fun row -> row.FromLang = FromLanguageCode)
            |> Seq.filter (fun row -> row.FromText = fromText)
            |> Seq.filter (fun row -> row.Property.ToLower() = property.Value.ToLower())
            |> Seq.filter (fun row -> row.Context.ToLower() = context.ToLower())
            |> Seq.filter (fun row -> row.ToLang = toLang)
            |> checkHead

        match result with
            | Some x -> x.ToText
            | None -> fromText
            

    //////////////////////////////////////////////////////////////////////////////////////////////
    //     GetToText function returns the translated string, if defined in the Translate table. 
    //     If the fromText is not found, the value of fromText is returned unchanged.
    //     For every call to this function, a database connection is opened and closed.
    let GetToText (criteria : Option<string>) (fromText : string) (property : Option<string>) (context : string) toLanguageCode =
        if fromText.Trim() = "" then "" |> ignore
            
        // If toLanguageCode is not valid, sets it to default "no"
        let toLang =
            match toLanguageCode with
            | null | "" -> "no"
            | _ -> toLanguageCode
            
        let checkHead (s : seq<'a>) =
            if Seq.length s > 0 then
                Some <| Seq.head s
            else 
                None

        // Open a connection to the database
        use db = dbSchema.GetDataContext(Settings.ConnectionStrings.DbConnectionString)
            
        // Search for a valid translation
        let collection = query {for row in db.Translation do select row}
        let result =
            collection
            |> Seq.filter (fun row -> row.Criteria.ToLower() = criteria.Value.ToLower())
            |> Seq.filter (fun row -> row.FromLang = FromLanguageCode)
            |> Seq.filter (fun row -> row.FromText = fromText)
            |> Seq.filter (fun row -> row.Property.ToLower() = property.Value.ToLower())
            |> Seq.filter (fun row -> row.Context.ToLower() = context.ToLower())
            |> Seq.filter (fun row -> row.ToLang = toLang)
            |> checkHead

        match result with
            | Some x -> x.ToText
            | None -> fromText

    ////////////////////////////////////////////////////////////////////////////////////////////
