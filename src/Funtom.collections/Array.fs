namespace Funtom.collections

module Array =
  open System.Diagnostics.CodeAnalysis
  open System.Numerics
  open System.Runtime.CompilerServices
  open System.Runtime.InteropServices
  open System.Runtime.Intrinsics
  open System.Runtime.Intrinsics.X86

  let inline max<^T 
    when ^T: unmanaged 
    and ^T: struct
    and ^T: comparison
    and ^T: (new: unit -> ^T)
    and ^T:> System.ValueType> (src: array<^T>) =
    if not Vector128.IsHardwareAccelerated || src.Length < Vector128<^T>.Count
      then
        let mutable max = src[0]
        for i = 1 to src.Length - 1 do
          if max < src[i] then max <- src[i]
        max
    elif Vector256.IsHardwareAccelerated || src.Length < Vector256<^T>.Count
      then
        Unchecked.defaultof<_>
        
    else
      Unchecked.defaultof<_>

