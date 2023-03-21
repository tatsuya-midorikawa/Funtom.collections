open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open Bogus
open System
open System.Linq
open FSharp.NativeInterop

let fake = Faker()

type Sample = { Num1: int; Num2: int }

[<PlainExporter; MemoryDiagnoser>]
type Benchmark () =  
  let mutable xs = List.empty
  let mutable ys = Array.empty
  let mutable zs = ResizeArray()
  let mutable ss = Seq.empty
  
  let mutable xs' = List.empty
  let mutable ys' = Array.empty
  let mutable zs' = ResizeArray()
  let mutable ss' = Seq.empty
  
  let mutable xs'' : list<Sample> = List.empty
  let mutable ys'' : array<Sample> = Array.empty
  let mutable zs'' : ResizeArray<Sample> = ResizeArray()
  let mutable ss'' : seq<Sample> = Seq.empty
  
  [<GlobalSetup>]
  member this.Setup() =
    xs <- [for _ in 1..10000 do fake.Random.Int()]
    ys <- [|for _ in 1..10000 do fake.Random.Int()|]
    zs <- ResizeArray([|for _ in 1..10000 do fake.Random.Int()|])
    ss <- [|for _ in 1..10000 do fake.Random.Int()|] |> Seq.ofArray

    //xs' <- [1..10000]
    //ys' <- [|1..10000|]
    //zs' <- ResizeArray([|1..10000|])
    //ss' <- [|1..10000|] |> Seq.ofArray
    
    //xs'' <- [for _ in 1..10000 do { Num1 = fake.Random.Int(); Num2 = fake.Random.Int() } ]
    //ys'' <- [|for _ in 1..10000 do { Num1 = fake.Random.Int(); Num2 = fake.Random.Int() } |]
    //zs'' <- ResizeArray([|for _ in 1..10000 do { Num1 = fake.Random.Int(); Num2 = fake.Random.Int() } |])
    //ss'' <- [|for _ in 1..10000 do { Num1 = fake.Random.Int(); Num2 = fake.Random.Int() } |] |> Seq.ofArray

  [<Benchmark>]
  member __.normal_for() =
    let mutable acc = 0
    for y in ys do
      acc <- acc + y
    acc

  [<Benchmark>]
  member __.span_for() =
    let mutable acc = 0
    for y in ys.AsSpan() do
      acc <- acc + y
    acc

#if BENCHMARK
[<EntryPoint>]
let main args =
  BenchmarkRunner.Run<Benchmark>() |> ignore
  System.Console.ReadKey() |> ignore
  0
#else
#nowarn "9"
type X = { mutable v: int }
[<EntryPoint>]
let main args =
  let mutable x = 0
  let ptr = NativePtr.toNativeInt<int> &&x
  let p = NativePtr.ofNativeInt<int> ptr
  let mutable v = NativePtr.read p
  v <- 100
  x |> printfn "%A"
  System.Console.ReadKey() |> ignore

  0
  //let mutable p = NativePtr.read (NativePtr.ofNativeInt<int> x)
  //p <- 100
  //x |> printfn "%A"
  //System.Console.ReadKey() |> ignore
  //0
#endif