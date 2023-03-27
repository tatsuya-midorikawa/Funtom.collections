namespace Funtom.collections

module Array =
  open System
  open System.Runtime.CompilerServices
  open Funtom.collections.internals.core

  module Internal =
    let rec max128<'T when 'T: unmanaged and 'T: struct and 'T: comparison and 'T: (new: unit -> 'T) and 'T:> System.ValueType>
      (current: byref<'T>, last': byref<'T>, best': vec128<'T>) =
        if Unsafe.IsAddressLessThan(&current, &last')
          then
            max128 (&Unsafe.Add(&current, vec128<'T>.Count), &last', vec128.Max(best', vec128.LoadUnsafe(&current)))
          else
            let best = vec128.Max(best', vec128.LoadUnsafe(&last'))
            let mutable max = best[0]
            for i = 1 to vec128<'T>.Count - 1 do
              if max < best[i] then max <- best[i]
            max

    //let rec max256<'T when 'T: unmanaged and 'T: struct and 'T: comparison and 'T: (new: unit -> 'T) and 'T:> System.ValueType>
    //  (current': byref<'T>, last': byref<'T>, best': vec256<'T>) =
    //    if Unsafe.IsAddressLessThan(&current', &last')
    //      then
    //        max256 (&Unsafe.Add(&current', vec256<'T>.Count), &last', vec256.Max(best', vec256.LoadUnsafe(&current')))
    //      else
    //        let best = vec256.Max(best', vec256.LoadUnsafe(&last'))
    //        let mutable max = best[0]
    //        let mutable i = 1
    //        while i < vec256<'T>.Count do
    //          if max < best[i] then max <- best[i]
    //          i <- i + 1
    //        max

    let rec max256<'T when 'T: unmanaged and 'T: struct and 'T: comparison and 'T: (new: unit -> 'T) and 'T:> System.ValueType>
      (current': byref<'T>, last': byref<'T>, best': vec256<'T>) =
        if Unsafe.IsAddressLessThan(&current', &last')
          then
            max256 (&Unsafe.Add(&current', vec256<'T>.Count), &last', vec256.Max(best', vec256.LoadUnsafe(&current')))
          else
            let best = vec256.Max(best', vec256.LoadUnsafe(&last'))
            let mutable max = best[0]
            let mutable i = 1
            while i < vec256<'T>.Count do
              if max < best[i] then max <- best[i]
              i <- i + 1
            max

    let inline max256v2<^T when ^T: unmanaged and ^T: struct and ^T: comparison and ^T: (new: unit -> ^T) and ^T:> System.ValueType>
      (current': byref<^T>, last': byref<^T>) =
        let mutable best = vec256.LoadUnsafe(&current')
        current' <- Unsafe.Add(&current', vec256<^T>.Count)
        while Unsafe.IsAddressLessThan(&last', &current') do
          best <- vec256.Max(best, vec256.LoadUnsafe &current')
          current' <- Unsafe.Add(&current', vec256<^T>.Count)
        best <- vec256.Max(best, vec256.LoadUnsafe &last')
        let mutable max = best[0]
        let mutable i = 1
        while i < vec256<^T>.Count do
          if max < best[i] then max <- best[i]
          i <- i + 1
        max

           
  let inline max<^T when ^T: unmanaged and ^T: struct and ^T: comparison and ^T: (new: unit -> ^T) and ^T:> System.ValueType>
    (src: array<^T>) =

      let inline non_simd() =
        let mutable max = src[0]
        for n in src do if max < n then max <- n
        max
      let inline simd_128() =
        let src = src.AsSpan()
        let current' = &(ref src)
        let last' = &(Unsafe.Add(&current', src.Length - vec128<^T>.Count))
        Internal.max128(&current', &last', vec128.LoadUnsafe(&current'))
      let inline simd_256() = 
        let mutable src = src.AsSpan()
        let mutable current' = &(ref src)
        let mutable last' = &(Unsafe.Add(&current', src.Length - vec256<^T>.Count))
        Internal.max256(&current', &last', vec256.LoadUnsafe(&current'))

      let inline simd_256_v2() = 
        let mutable src = src.AsSpan()
        let mutable current' = &(ref src)
        let mutable last' = &(Unsafe.Add(&current', src.Length - vec256<^T>.Count))
        Internal.max256v2(&current', &last')

      if src = defaultof<_> || src.Length = 0
        then throw_empty()
        else exec (src.Length, non_simd, simd_128, simd_256_v2)
        //else exec (src.Length, non_simd, simd_128, simd_256)