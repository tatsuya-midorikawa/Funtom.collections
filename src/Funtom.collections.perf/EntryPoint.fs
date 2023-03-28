open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running
open Bogus
open System
open System.Linq
open FSharp.NativeInterop

let fake = Faker()

[<PlainExporter; MemoryDiagnoser>]
type Benchmark () =

  let mutable xs = Array.empty

  [<GlobalSetup>]
  member __.Setup() =
    xs <- [|for _ in 1..1_000 do fake.Random.Int(System.Int32.MinValue, System.Int32.MaxValue)|]

  [<Benchmark>]
  member __.Linq_max() = xs.Max()

  [<Benchmark>]
  member __.Array_max() = Array.max xs
    
  [<Benchmark>]
  member __.Funtom_Array_max_v2() = Funtom.collections.Array.max xs

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