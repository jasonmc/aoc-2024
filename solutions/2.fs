module s_2

open System

let ex1 =
    ("""7 6 4 2 1
1 2 7 8 9
9 7 6 2 1
1 3 2 4 5
8 6 4 4 1
1 3 6 7 9""")
        .Split("\n")

let realInput = System.IO.File.ReadAllLines "inputs/02.txt"



let parseLine (line: string) = line.Split " " |> Array.map int

let isSafe (report: int seq) =
    let deltas = report |> Seq.pairwise |> Seq.map (fun (x, y) -> x - y)
    let absDeltas = deltas |> Seq.map Math.Abs
    let maxDelta = absDeltas |> Seq.max
    let minDelta = absDeltas |> Seq.min
    let sameDirection = deltas |> Seq.forall ((>) 0) || deltas |> Seq.forall ((<) 0)
    (maxDelta < 4) && (minDelta > 0) && sameDirection


let solveFirst () =
    realInput |> Array.map parseLine |> Array.filter isSafe |> Array.length

let removeEachPosition seq =
    let seqList = Seq.toList seq

    seqList
    |> List.mapi (fun i _ -> (List.take i seqList) @ (List.skip (i + 1) seqList))
    |> List.toSeq


let isSafeWithTolerance (report: int seq) =
    report |> removeEachPosition |> Seq.exists isSafe

let solveSecond () =
    realInput
    |> Array.map parseLine
    |> Array.filter isSafeWithTolerance
    |> Array.length
