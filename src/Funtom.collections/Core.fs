namespace Funtom.collections.internals

module core =
  let inline throw_empty() = raise (System.InvalidOperationException "The source is empty.")
  let inline debug_writel(str) = System.Diagnostics.Debug.WriteLine(str)

  type vec128 = System.Runtime.Intrinsics.Vector128
  type vec128<'T when 'T: unmanaged and 'T: struct and 'T: comparison and 'T: (new: unit -> 'T) and 'T:> System.ValueType> = System.Runtime.Intrinsics.Vector128<'T>
  type vec256 = System.Runtime.Intrinsics.Vector256
  type vec256<'T when 'T: unmanaged and 'T: struct and 'T: comparison and 'T: (new: unit -> 'T) and 'T:> System.ValueType> = System.Runtime.Intrinsics.Vector256<'T>
  
  let inline defaultof<'T> = Unchecked.defaultof<'T>
  let inline ref (span: System.Span<'a>) = &(System.Runtime.InteropServices.MemoryMarshal.GetReference span)
  let inline exec<^T when ^T: unmanaged and ^T: struct and ^T: comparison and ^T: (new: unit -> ^T) and ^T:> System.ValueType> (
    src_length: int,
    [<InlineIfLambda>] non_simd: unit -> ^T,
    [<InlineIfLambda>] simd_128: unit -> ^T,
    [<InlineIfLambda>] simd_256: unit -> ^T ) : ^T =
      if not vec128.IsHardwareAccelerated || src_length < vec128<^T>.Count
        // Not SIMD
        then non_simd()
        elif not vec256.IsHardwareAccelerated || src_length < vec256<^T>.Count
          // SIMD : 128bit
          then simd_128()
          // SIMD : 256bit
          else simd_256()
