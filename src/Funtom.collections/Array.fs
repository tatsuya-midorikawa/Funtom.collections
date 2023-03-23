namespace Funtom.collections

module Array =
  open System
  open System.Diagnostics.CodeAnalysis
  open System.Numerics
  open System.Runtime.CompilerServices
  open System.Runtime.InteropServices
  open System.Runtime.Intrinsics
  open System.Runtime.Intrinsics.X86

  module Internal =
    let rec max256<'T when 'T: unmanaged and 'T: struct and 'T: comparison and 'T: (new: unit -> 'T) and 'T:> System.ValueType>
      (c: byref<'T>, to': byref<'T>, max': Vector256<'T>) =
        if Unsafe.IsAddressLessThan(&c, &to')
          then
            max256 (&Unsafe.Add(&c, Vector256<'T>.Count), &to', Vector256.Max(max', Vector256.LoadUnsafe(&c)))
          else
            let max' = Vector256.Max(max', Vector256.LoadUnsafe(&to'))
            let mutable max'' = max'[0]
            for i = 1 to Vector256<^T>.Count - 1 do
              if max'' < max'[i] then max'' <- max'[i]
            max''
    
    let rec max128<'T when 'T: unmanaged and 'T: struct and 'T: comparison and 'T: (new: unit -> 'T) and 'T:> System.ValueType>
      (c: byref<'T>, to': byref<'T>, max': Vector128<'T>) =
        if Unsafe.IsAddressLessThan(&c, &to')
          then
            max128 (&Unsafe.Add(&c, Vector128<'T>.Count), &to', Vector128.Max(max', Vector128.LoadUnsafe(&c)))
          else
            let max' = Vector128.Max(max', Vector128.LoadUnsafe(&to'))
            let mutable max'' = max'[0]
            for i = 1 to Vector128<'T>.Count - 1 do
              if max'' < max'[i] then max'' <- max'[i]
            max''

  let inline max<'T when 'T: unmanaged and 'T: struct and 'T: comparison and 'T: (new: unit -> 'T) and 'T:> System.ValueType>
    (src: array<^T>) =
      if not Vector128.IsHardwareAccelerated || src.Length < Vector128<^T>.Count
        // Not SIMD
        then
          let mutable max = src[0]
          for i = 1 to src.Length - 1 do
            if max < src[i] then max <- src[i]
          max
      elif not Vector256.IsHardwareAccelerated || src.Length < Vector256<^T>.Count
        // SIMD : 128bit
        then
          let src = src.AsSpan()
          let current = &(MemoryMarshal.GetReference src)
          let to' = &(Unsafe.Add(&current, src.Length - Vector128<^T>.Count))
          Internal.max128(&current, &to', Vector128.LoadUnsafe(&current))
        // SIMD : 256bit
        else
          let src = src.AsSpan()
          let current = &(MemoryMarshal.GetReference src)
          let to' = &(Unsafe.Add(&current, src.Length - Vector256<^T>.Count))
          Internal.max256(&current, &to', Vector256.LoadUnsafe(&current))

