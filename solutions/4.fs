module s_4

open System

let example =
    """MMMSXXMASM
MSAMXMSMSA
AMXSXMAAMM
MSAMASMSMX
XMASAMXAMM
XXAMMXXAMA
SMSMSASXSS
SAXAMASAAA
MAMMMXMMMM
MXMXAXMASX"""
        .Split("\n")

let realInput: string array = System.IO.File.ReadAllLines "inputs/04.txt"

let diagonals (grid: 'a list list) =
    let n = List.length grid
    let m = if n > 0 then List.length (List.head grid) else 0

    // Helper function to generate a diagonal using functional iteration
    let extractDiagonal (row, col) (dr, dc) =
        Seq.initInfinite id
        |> Seq.map (fun i -> (row + i * dr, col + i * dc))
        |> Seq.takeWhile (fun (r, c) -> r >= 0 && r < n && c >= 0 && c < m)
        |> Seq.map (fun (r, c) -> List.item c (List.item r grid))
        |> Seq.toList

    // Collect main diagonals
    let mainDiagonals =
        [ for i in 0 .. n - 1 -> extractDiagonal (i, 0) (1, 1) ]
        @ [ for j in 1 .. m - 1 -> extractDiagonal (0, j) (1, 1) ]

    // Collect anti-diagonals
    let antiDiagonals =
        [ for i in 0 .. n - 1 -> extractDiagonal (i, m - 1) (1, -1) ]
        @ [ for j in m - 2 .. -1 .. 0 -> extractDiagonal (0, j) (1, -1) ]

    mainDiagonals @ antiDiagonals


let countOccurrences (word: string) (charList: char list) =
    let text = System.String.Concat(charList)

    let rec countFromIndex (index: int) acc =
        match text.IndexOf(word, index) with
        | -1 -> acc
        | foundIndex -> countFromIndex (foundIndex + word.Length) (acc + 1)

    countFromIndex 0 0

let wordinCharsOrReverse (chars: char list) (word: string) =
    (countOccurrences word chars) + (countOccurrences word (chars |> List.rev))

let solveFirst () =
    let characters =
        realInput |> Seq.map (fun x -> x.ToCharArray() |> List.ofArray) |> List.ofSeq

    (characters @ (characters |> List.transpose) @ (diagonals characters))
    |> List.sumBy (fun x -> (wordinCharsOrReverse x "XMAS"))

let isCross (a: int, b: int) (grid: char array array) =
    let isValidPair (dx1, dy1) (dx2, dy2) =
        grid.[a + dx1].[b + dy1] = 'M' && grid.[a + dx2].[b + dy2] = 'S'

    (a > 0 && b > 0 && a < grid.Length - 1 && b < grid.[0].Length - 1)
    && grid.[a].[b] = 'A'
    && [ ((-1, -1), (1, 1)); ((1, -1), (-1, 1)) ]
       |> List.filter (fun (p1, p2) -> isValidPair p1 p2 || isValidPair p2 p1)
       |> List.length > 1

let solveSecond () =
    let grid = realInput |> Array.map (fun x -> x.ToCharArray() |> Array.ofSeq)

    realInput
    |> Array.map (fun x -> x.ToCharArray() |> Array.ofSeq)
    |> Array.mapi (fun i row ->
        row
        |> Array.mapi (fun j _ -> if isCross (i, j) grid then Some(i, j) else None)
        |> Array.choose id // Remove `None` values
    )
    |> Array.collect id
    |> Array.length
