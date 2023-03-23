namespace Funtom.collections

module Array =
  open System
  open System.Runtime.CompilerServices
  open Funtom.collections.internals.core

  module Internal =
    let rec max128<'T when 'T: unmanaged and 'T: struct and 'T: comparison and 'T: (new: unit -> 'T) and 'T:> System.ValueType>
      (c: byref<'T>, to': byref<'T>, max': vec128<'T>) =
        if Unsafe.IsAddressLessThan(&c, &to')
          then
            max128 (&Unsafe.Add(&c, vec128<'T>.Count), &to', vec128.Max(max', vec128.LoadUnsafe(&c)))
          else
            let max' = vec128.Max(max', vec128.LoadUnsafe(&to'))
            let mutable max'' = max'[0]
            for i = 1 to vec128<'T>.Count - 1 do
              if max'' < max'[i] then max'' <- max'[i]
            max''

    let rec max256<'T when 'T: unmanaged and 'T: struct and 'T: comparison and 'T: (new: unit -> 'T) and 'T:> System.ValueType>
      (c: byref<'T>, to': byref<'T>, max': vec256<'T>) =
        if Unsafe.IsAddressLessThan(&c, &to')
          then
            max256 (&Unsafe.Add(&c, vec256<'T>.Count), &to', vec256.Max(max', vec256.LoadUnsafe(&c)))
          else
            let max' = vec256.Max(max', vec256.LoadUnsafe(&to'))
            let mutable max'' = max'[0]
            for i = 1 to vec256<^T>.Count - 1 do
              if max'' < max'[i] then max'' <- max'[i]
            max''
    
  let inline max<^T when ^T: unmanaged and ^T: struct and ^T: comparison and ^T: (new: unit -> ^T) and ^T:> System.ValueType>
    (src: array<^T>) =
      let inline non_simd() =
        let mutable max = src[0]
        for n in src do if max < n then max <- n
        max
      let inline simd_128() =
        let src = src.AsSpan()
        let r' = &(ref src)
        let to' = &(Unsafe.Add(&r', src.Length - vec128<^T>.Count))
        Internal.max128(&r', &to', vec128.LoadUnsafe(&r'))
      let inline simd_256() = 
        let src = src.AsSpan()
        let r' = &(ref src)
        let to' = &(Unsafe.Add(&r', src.Length - vec256<^T>.Count))
        Internal.max256(&r', &to', vec256.LoadUnsafe(&r'))

      if src = defaultof<_> || src.Length = 0
        then throw_empty()
        else exec (src.Length, non_simd, simd_128, simd_256)

