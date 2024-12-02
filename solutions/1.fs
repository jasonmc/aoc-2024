module s_1

open System


let ex1 =
    ("""3   4
4   3
2   5
1   3
3   9
3   3""")
        .Split("\n")



let realInput = System.IO.File.ReadAllLines "inputs/01.txt"

let processLine (line: string) =
    let [| a; b |] = line.Split "   "
    (int a, int b)



let solveFirst () =
    let x = realInput |> Seq.map processLine
    let lefts = x |> Seq.map fst |> Seq.sort
    let rights = x |> Seq.map snd |> Seq.sort
    let repaired = Seq.zip lefts rights
    repaired |> Seq.map (fun (a, b) -> Math.Abs(a - b)) |> Seq.sum


let solveSecond () =
    let x = realInput |> Seq.map processLine
    let rightCounts = x |> Seq.map snd |> Seq.countBy (fun x -> x) |> Map.ofSeq

    x
    |> Seq.map fst
    |> Seq.map (fun (i) ->
        (Map.tryFind i rightCounts)
        |> Option.map (fun j -> i * j)
        |> Option.defaultValue 0)
    |> Seq.sum
