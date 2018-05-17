module List

open System.Collections.Generic
open Fable.Core

let head = function
    | x::_ -> x
    | _ -> failwith "List was empty"

let tryHead = function
    | x::_ -> Some x
    | _ -> None

let tail = function
    | _::xs -> xs
    | _ -> failwith "List was empty"

let rec last = function
    | [] -> failwith "List was empty"
    | [x] -> x
    | _::xs -> last xs

let rec tryLast = function
    | [] -> None
    | [x] -> Some x
    | _::xs -> tryLast xs

let compareWith (comparer: 'T -> 'T -> int) (xs: 'T list) (ys: 'T list): int =
    if obj.ReferenceEquals(xs, ys)
    then 0
    else
        let rec loop xs ys =
            match xs, ys with
            | [], [] -> 0
            | [], _ -> -1
            | _, [] -> 1
            | x::xs, y::ys ->
                match comparer x y with
                | 0 -> loop xs ys
                | res -> res
        loop xs ys

let equalsWith (comparer: 'T -> 'T -> int) (xs: 'T list) (ys: 'T list): bool =
    compareWith comparer xs ys = 0

let rec foldIndexedAux f i acc = function
   | [] -> acc
   | x::xs -> foldIndexedAux f (i+1) (f i acc x) xs

let foldIndexed<'a,'acc> f (seed:'acc) (xs: 'a list) =
   foldIndexedAux f 0 seed xs

let fold<'a,'acc> f (seed:'acc) (xs: 'a list) =
   foldIndexed (fun _ acc x -> f acc x) seed xs

let reverse xs =
   fold (fun acc x -> x::acc) [] xs

let foldBack<'a,'acc> f (xs: 'a list) (seed:'acc) =
   fold (fun acc x -> f x acc) seed (reverse xs)

let rec foldIndexed2Aux f i acc bs cs =
   match bs, cs with
   | [], [] -> acc
   | x::xs, y::ys -> foldIndexed2Aux f (i+1) (f i acc x y) xs ys
   | _ -> invalidOp "Lists had different lengths"

let foldIndexed2<'a, 'b, 'acc> f (seed:'acc) (xs: 'a list) (ys: 'b list) =
   foldIndexed2Aux f 0 seed xs ys

let fold2<'a, 'b, 'acc> f (seed:'acc) (xs: 'a list) (ys: 'b list) =
   foldIndexed2 (fun _ acc x y -> f acc x y) seed xs ys

let foldBack2<'a, 'b, 'acc> f (xs: 'a list) (ys: 'b list) (seed:'acc) =
   fold2 (fun acc x y -> f x y acc) seed (reverse xs) (reverse ys)

let unfold f state =
    let rec unfoldInner acc state =
        match f state with
        |None -> reverse acc
        |Some (x,state) -> unfoldInner (x::acc) state
    unfoldInner [] state

let rec foldIndexed3Aux f i acc bs cs ds =
   match bs, cs, ds with
   | [], [], [] -> acc
   | x::xs, y::ys, z::zs -> foldIndexed3Aux f (i+1) (f i acc x y z) xs ys zs
   | _ -> invalidOp "Lists had different lengths"

let foldIndexed3<'a, 'b, 'c, 'acc> f (seed:'acc) (xs: 'a list) (ys: 'b list) (zs: 'c list) =
   foldIndexed3Aux f 0 seed xs ys zs

let fold3<'a, 'b, 'c, 'acc> f (seed:'acc) (xs: 'a list) (ys: 'b list) (zs: 'c list) =
   foldIndexed3 (fun _ acc x y z -> f acc x y z) seed xs ys zs

let scan<'a, 'acc> f (seed:'acc) (xs: 'a list) =
   fold (fun acc x ->
      match acc with
      | [] -> failwith "never"
      | y::_ -> f y x::acc) [seed] xs
   |> reverse

let scanBack<'a, 'acc> f (xs: 'a list) (seed:'acc) =
   scan (fun acc x -> f x acc) seed (reverse xs)
   |> reverse

let length xs =
   fold (fun acc _ -> acc + 1) 0 xs

let append xs ys =
   fold (fun acc x -> x::acc) ys (reverse xs)

let collect f xs =
   fold (fun acc x -> append (f x) acc) [] (reverse xs)

let map f xs =
   fold (fun acc x -> (f x::acc)) [] xs
   |> reverse

let mapIndexed f xs =
   foldIndexed (fun i acc x -> f i x::acc) [] xs
   |> reverse

let indexed xs =
    mapIndexed (fun i x -> (i,x)) xs

let map2 f xs ys =
   fold2 (fun acc x y -> f x y::acc) [] xs ys
   |> reverse

let mapIndexed2 f xs ys =
   foldIndexed2 (fun i acc x y  -> f i x y:: acc) [] xs ys
   |> reverse

let map3 f xs ys zs =
   fold3 (fun acc x y z -> f x y z::acc) [] xs ys zs
   |> reverse

let mapIndexed3 f xs ys zs =
   foldIndexed3 (fun i acc x y z -> f i x y z:: acc) [] xs ys zs
   |> reverse

let iterate f xs =
   fold (fun () x -> f x) () xs

let iterate2 f xs ys =
   fold2 (fun () x y -> f x y) () xs ys

let iterateIndexed f xs =
   foldIndexed (fun i () x -> f i x) () xs

let iterateIndexed2 f xs ys =
   foldIndexed2 (fun i () x y -> f i x y) () xs ys

let ofArray xs =
   Array.foldBack (fun x acc -> x::acc) xs []

let empty<'a> : 'a list = []

let isEmpty = function
   | [] -> true
   | _ -> false

let rec tryPickIndexedAux f i = function
   | [] -> None
   | x::xs ->
      let result = f i x
      match result with
      | Some _ -> result
      | None -> tryPickIndexedAux f (i+1) xs

let tryPickIndexed f xs =
   tryPickIndexedAux f 0 xs

let tryPick f xs =
   tryPickIndexed (fun _ x -> f x) xs

let pick f xs =
   match tryPick f xs with
   | None -> invalidOp "List did not contain any matching elements"
   | Some x -> x

let tryFindIndexed f xs =
   tryPickIndexed (fun i x -> if f i x then Some x else None) xs

let tryFind f xs =
   tryPickIndexed (fun _ x -> if f x then Some x else None) xs

let findIndexed f xs =
   match tryFindIndexed f xs with
   | None -> invalidOp "List did not contain any matching elements"
   | Some x -> x

let find f xs =
   findIndexed (fun _ x -> f x) xs

let tryFindIndex f xs =
   tryPickIndexed (fun i x -> if f x then Some i else None) xs

let findIndex f xs =
   match tryFindIndex f xs with
   | None -> invalidOp "List did not contain any matching elements"
   | Some x -> x

let item n xs =
   findIndexed (fun i _ -> n = i) xs

let tryItem n xs =
   tryFindIndexed (fun i _ -> n = i) xs

let filter f xs =
   foldBack (fun x acc ->
      if f x then x::acc
      else acc) xs []

let partition f xs =
   fold (fun (lacc, racc) x ->
      if f x then x::lacc, racc
      else lacc,x::racc) ([],[]) (reverse xs)

let choose f xs =
   fold (fun acc x ->
      match f x with
      | Some y -> y:: acc
      | None -> acc) [] xs |> reverse

let initialize n f =
   let mutable xs = []
   for i = 1 to n do xs <- f (n - i):: xs
   xs

let replicate n x =
   initialize n (fun _ -> x)

let reduce f = function
   | [] -> invalidOp "List was empty"
   | h::t -> fold f h t

let reduceBack f = function
   | [] -> invalidOp "List was empty"
   | h::t -> foldBack f t h

let forAll f xs =
   fold (fun acc x -> acc && f x) true xs

let forAll2 f xs ys =
   fold2 (fun acc x y -> acc && f x y) true xs ys

let rec exists f = function
   | [] -> false
   | x::xs -> f x || exists f xs

let rec exists2 f bs cs =
   match bs, cs with
   | [], [] -> false
   | x::xs, y::ys -> f x y || exists2 f xs ys
   | _ -> invalidOp "Lists had different lengths"

let unzip xs =
   foldBack (fun (x, y) (lacc, racc) -> x::lacc, y::racc) xs ([],[])

let unzip3 xs =
   foldBack (fun (x, y, z) (lacc, macc, racc) -> x::lacc, y::macc, z::racc) xs ([],[],[])

let zip xs ys =
   map2 (fun x y -> x, y) xs ys

let zip3 xs ys zs =
   map3 (fun x y z -> x, y, z) xs ys zs

let sort (xs : 'T list) ([<Inject>] comparer: IComparer<'T>): 'T list =
    Array.sortInPlaceWith (fun x y -> comparer.Compare(x, y)) (Array.ofList xs Array.DynamicArrayCons) |> ofArray

let sortBy (projection:'a->'b) (xs : 'a list) ([<Inject>] comparer: IComparer<'b>): 'a list =
    Array.sortInPlaceWith (fun x y -> comparer.Compare(projection x, projection y)) (Array.ofList xs Array.DynamicArrayCons) |> ofArray

let sortDescending (xs : 'T list) ([<Inject>] comparer: IComparer<'T>): 'T list =
    Array.sortInPlaceWith (fun x y -> comparer.Compare(x, y) * -1) (Array.ofList xs Array.DynamicArrayCons) |> ofArray

let sortByDescending (projection:'a->'b) (xs : 'a list) ([<Inject>] comparer: IComparer<'b>): 'a list =
    Array.sortInPlaceWith (fun x y -> comparer.Compare(projection x, projection y) * -1) (Array.ofList xs Array.DynamicArrayCons) |> ofArray

let sortWith (comparer: 'T -> 'T -> int) (xs : 'T list): 'T list =
    Array.sortInPlaceWith comparer (Array.ofList xs Array.DynamicArrayCons) |> ofArray

// TODO!!!: Pass add function for non-number types
let sum (xs: float list) : float =
   fold (+) 0. xs

let sumBy (f:'a -> float) (xs: 'a list) : float =
   fold (fun acc x -> acc + f x) 0. xs

let maxBy (projection:'a->'b) (xs:'a list) ([<Inject>] comparer: IComparer<'b>): 'a =
    reduce (fun x y -> if comparer.Compare(projection y, projection x) > 0 then y else x) xs

let max (li:'a list) ([<Inject>] comparer: IComparer<'a>): 'a =
    reduce (fun x y -> if comparer.Compare(y, x) > 0 then y else x) li

let minBy (projection:'a->'b) (xs:'a list) ([<Inject>] comparer: IComparer<'b>): 'a =
    reduce (fun x y -> if comparer.Compare(projection y, projection x) > 0 then x else y) xs

let min (xs:'a list) ([<Inject>] comparer: IComparer<'a>): 'a =
    reduce (fun x y -> if comparer.Compare(y, x) > 0 then x else y) xs

let average (zs: float list) : float =
   let total = sum zs
   total / float(List.length zs)

let averageBy (g: 'a -> float ) (zs: 'a list) : float =
   let total = sumBy g zs
   total / float(List.length zs)

let permute f xs =
   xs
   |> List.toArray
   |> Array.permute f
   |> ofArray

// TODO: Is there a more efficient algorithm?
let rec takeSplitAux error i acc xs =
    match i, xs with
    | 0, _ -> reverse acc, xs
    | _, [] ->
      if error then
          failwith "The input sequence has an insufficient number of elements."
      else
          reverse acc, xs
    | _, x::xs -> takeSplitAux error (i - 1) (x::acc) xs
let take i xs =
  match i, xs with
  | i, _ when i < 0 -> failwith "The input must be non-negative."
  | 0, _ -> []
  | 1, x::_ -> [x]
  | i, xs -> takeSplitAux true i [] xs |> fst

let truncate i xs =
  match i, xs with
  | i, _ when i < 0 -> failwith "The input must be non-negative."
  | 0, _ -> []
  | 1, x::_ -> [x]
  | i, xs -> takeSplitAux false i [] xs |> fst

let splitAt i xs =
  match i, xs with
  | i, _ when i < 0 -> failwith "The input must be non-negative."
  | 0, _ -> [],xs
  | 1, x::xs -> [x],xs
  | i, xs -> takeSplitAux true i [] xs

let skip i xs =
  let rec skipInner i xs =
      match i, xs with
      | 0, _ -> xs
      | _, [] -> failwith "The input sequence has an insufficient number of elements."
      | _, _::xs -> skipInner (i - 1) xs
  match i, xs with
  | i, _ when i < 0 -> failwith "The input must be non-negative."
  | 0, _ -> xs
  | 1, _::xs -> xs
  | i, xs -> skipInner i xs

// let toSeq (xs: _ list) =
//     seq {
//         let mutable xs = xs
//         while not xs.IsEmpty do
//             yield xs.Head
//             xs <- xs.Tail
//     }

let toSeq (xs: 'a list): 'a seq =
    Seq.unfold (function [] -> None | x::xs -> Some(x, xs)) xs

let ofSeq (xs: 'a seq): 'a list =
    Seq.foldBack(fun x acc -> x::acc) xs []

let concat (lists : #seq<'a list>) =
    Seq.foldBack (List.foldBack (fun x acc -> x::acc)) lists []

let slice (lower: int option) (upper: int option) (xs: 'T list) =
    let lower = defaultArg lower -1
    let upper = defaultArg upper -1
    ([], xs) ||> foldIndexed (fun i acc x ->
        if (lower = -1 || lower <= i) && (upper = -1 || i <= upper)
        then x::acc
        else acc) |> reverse

let distinctBy projection (xs:'T list) ([<Inject>] eq: IEqualityComparer<'T>) =
    HashSet<'T>(xs |> Seq.map projection, eq) |> ofSeq

let distinct (xs: 'T list) ([<Inject>] eq: IEqualityComparer<'T>) =
    HashSet<'T>(xs, eq) |> ofSeq
