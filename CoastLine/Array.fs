namespace CoastLine

module Array =
    let breakOn (f : 'a -> bool) (a : array<'a>) =
        seq {
            let result = ResizeArray()
            for x in a do
                if f x then 
                    yield result |> Array.ofSeq
                    result.Clear()
                else     
                    result.Add(x)
            yield result |> Array.ofSeq
        } |> Array.ofSeq