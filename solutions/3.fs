module s_3

open FParsec
open System


let example: string array =
    """xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))"""
        .Split("\n")

// mul(2,4) mul(5,5) mul(11,8) mul(8,5)

let example2 =
    """xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))"""
        .Split("\n")
// mul(2,4) don't() do() mul(8,5)
let realInput: string array = System.IO.File.ReadAllLines "inputs/03.txt"


let mulParser = pstring "mul(" >>. pint32 .>> skipChar ',' .>>. pint32 .>> pchar ')'


let parseFirstLine (str: string) =
    let matchPatternWithRecovery =
        many (attempt (mulParser |>> Some) <|> (skipAnyChar >>% None))
        |>> (List.choose id)

    match run matchPatternWithRecovery str with
    | Success(result, _, _) -> result
    | Failure(errorMsg, _, _) -> failwith errorMsg

// example answer is 161
let solveFirst () =
    realInput
    |> Seq.collect parseFirstLine
    |> Seq.map (fun (x, y) -> x * y)
    |> Seq.sum


type Result =
    | Mul of (int * int)
    | Do
    | DoNot

let parseSecondLine (str: string) =
    let doNot = pstring "don't()"
    let doParse = pstring "do()"

    let options =
        choice [ mulParser |>> Mul; doNot |>> (fun _ -> DoNot); doParse |>> (fun _ -> Do) ]

    let matchPatternWithRecovery =
        many (attempt (options |>> Some) <|> (skipAnyChar >>% None))
        |>> (List.choose id)

    match run matchPatternWithRecovery str with
    | Success(result, _, _) -> result
    | Failure(errorMsg, _, _) -> failwith errorMsg

let solveSecond () =
    realInput
    |> Seq.collect parseSecondLine
    |> Seq.fold
        (fun (on, sum) ->
            function
            | Mul(x, y) -> on, if on then sum + x * y else sum
            | Do -> true, sum
            | DoNot -> false, sum)
        (true, 0)
    |> snd
