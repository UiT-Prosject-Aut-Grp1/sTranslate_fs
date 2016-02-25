module Model

    // Helper function to give the first element of a sequence, if it contains something
    let checkHead (s : seq<'a>) =
            if Seq.isEmpty s then
                None
            else 
                Some <| Seq.head s

    type Translation =
        {
            Id : int
            FromText : string
            ToText : string
            Context : string
            Property : string
            Criteria : string
            FromLang : string
            ToLang : string
        }