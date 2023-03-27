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
  // let mutable xs = List.empty
  // let mutable ys = Array.empty
  // let mutable zs = ResizeArray()
  // let mutable ss = Seq.empty

  // [<GlobalSetup>]
  // member __.Setup() =
  //   xs <- [for _ in 1..10_00_000 do fake.Random.Int(0, 100)]
  //   ys <- [|for _ in 1..10_00_000 do fake.Random.Int(0, 100)|]
  //   zs <- ResizeArray([|for _ in 1..10_00_000 do fake.Random.Int(0, 100)|])
  //   ss <- [|for _ in 1..10_00_000 do fake.Random.Int(0, 100)|] |> Seq.ofArray

  let mutable ys = Array.empty

  [<GlobalSetup>]
  member __.Setup() =
    //ys <- [|for _ in 1..1000 do fake.Random.Int(0, 100)|]
    ys <- [|for _ in 1..10_00_000 do fake.Random.Int(0, 100)|]

  [<Benchmark>]
  member __.Linq_max() = ys.Max()

  [<Benchmark>]
  member __.Array_max() = Array.max ys

  [<Benchmark>]
  member __.Funtom_Array_max() = Funtom.collections.Array.max ys
  
  [<Benchmark>]
  member __.Funtom_Array_max_v2() = Funtom.collections.Array.max_v2 ys

  //[<Benchmark>]
  //member __.SimdLinq_max() = SimdLinq.SimdLinqExtensions.Max(ys)

#if BENCHMARK
[<EntryPoint>]
let main args =
  BenchmarkRunner.Run<Benchmark>() |> ignore
  0
#else
#if RELEASE
[<EntryPoint>]
let main args =
  let xs = [|for _ in 0..10 do fake.Random.Int(0, 100)|]
  let xs = [| 999 |] |> Array.append xs
  xs |> printfn "%A"
  
  xs
  |> Array.max
  |> printfn "FSharp Array.max= %d"
  xs
  |> Funtom.collections.Array.max
  |> printfn "Funtom Array.max= %d"
  xs
  |> Funtom.collections.Array.max_v2
  |> printfn "Funtom Array.max_v2= %d"
  
  0
#else
#nowarn "9"
type X = { mutable v: int }
[<EntryPoint>]
let main args =
  //let mutable x = 0
  //let ptr = NativePtr.toNativeInt<int> &&x
  //let p = NativePtr.ofNativeInt<int> ptr
  //let mutable v = NativePtr.read p
  //v <- 100
  //x |> printfn "%A"
  //System.Console.ReadKey() |> ignore

  let xs = [|for _ in 0..10 do fake.Random.Int(0, 100)|]
  let xs = [| 999 |] |> Array.append xs
  xs |> printfn "%A"
  
  xs
  |> Array.max
  |> printfn "FSharp Array.max= %d"
  xs
  |> Funtom.collections.Array.max
  |> printfn "Funtom Array.max= %d"
  xs
  |> Funtom.collections.Array.max_v2
  |> printfn "Funtom Array.max_v2= %d"
  
  0
  //let mutable p = NativePtr.read (NativePtr.ofNativeInt<int> x)
  //p <- 100
  //x |> printfn "%A"
  //System.Console.ReadKey() |> ignore
  //0
#endif
#endif