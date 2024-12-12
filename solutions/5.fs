module s_5

open FParsec
open System

let example =
    """47|53
97|13
97|61
97|47
75|29
61|13
75|53
29|13
97|29
53|29
61|53
97|53
61|29
47|13
75|47
97|75
47|61
75|61
47|29
75|13
53|13

75,47,61,53,29
97,61,53,29,13
75,29,13
75,97,47,61,53
61,13,29
97,13,75,29,47
"""

let realInput = System.IO.File.ReadAllText "inputs/05.txt"

let parse str =
    let orderingRule = pint32 .>> skipChar '|' .>>. pint32 .>> newline
    let update = (sepBy pint32 (pstring ",")) .>> newline
    let parser = (many orderingRule) .>> skipNewline .>>. (many update) .>> eof

    match run parser str with
    | Success(result, _, _) -> result
    | Failure(errorMsg, _, _) -> failwith errorMsg


let p (rules: Map<int, Set<int>>, allUpdates: int Set, seen: int Set) x =
    let mustComeBefore = Map.tryFind x rules |> Option.defaultValue Set.empty
    let didNotComeBefore = Set.difference (Set.intersect mustComeBefore allUpdates) seen
    (Set.isEmpty didNotComeBefore, (rules, allUpdates, Set.add x seen))

let updateIsValid (rules: Map<int, Set<int>>) (update: int list) =
    update
    |> Seq.mapFold p (rules, (update |> Set.ofList), Set.empty)
    |> fst
    |> Seq.forall id


let mkRulesMap rules =
    rules
    |> Seq.groupBy snd
    |> Seq.map (fun (x, y) -> (x, y |> Seq.map fst |> Set.ofSeq))
    |> Map.ofSeq


let solveFirst () =
    let (rules, updates) = realInput |> parse

    let rulesMap = rules |> mkRulesMap

    updates
    |> Seq.filter (updateIsValid rulesMap)
    |> Seq.map (fun l -> List.item (List.length l / 2) l)
    |> Seq.sum



let topologicalSort (graph: Map<'T, 'T Set>) : 'T list =
    let rec dfs (node: 'T) (visited: Set<'T>) (sorted: 'T list) =
        if visited.Contains node then
            visited, sorted
        else
            let visited = visited.Add node
            let neighbors = graph |> Map.tryFind node |> Option.defaultValue Set.empty

            let visited, sorted =
                neighbors |> Set.fold (fun (v, s) n -> dfs n v s) (visited, sorted)

            visited, node :: sorted

    graph.Keys
    |> Seq.fold (fun (visited, sorted) node -> dfs node visited sorted) (Set.empty, [])
    |> snd
    |> List.rev


let processLineSecond (rules: (int * int) list) (line: int Set) =
    let rulesFiltered =
        rules |> List.filter (fun (x, y) -> Seq.contains x line && Set.contains y line)

    let rulesMap = rulesFiltered |> mkRulesMap
    let sorted = rulesMap |> topologicalSort
    sorted |> List.filter (fun y -> Set.contains y line)


let solveSecond () =
    let (rules, updates) = realInput |> parse
    let rulesMap = rules |> mkRulesMap

    updates
    |> List.filter (updateIsValid rulesMap >> not)
    |> List.map Set.ofList
    |> List.map (processLineSecond rules)
    |> Seq.map (fun l -> List.item (List.length l / 2) l)
    |> Seq.sum
