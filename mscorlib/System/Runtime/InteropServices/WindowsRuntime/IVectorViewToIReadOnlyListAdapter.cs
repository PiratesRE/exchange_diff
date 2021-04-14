using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;
using System.StubHelpers;

namespace System.Runtime.InteropServices.WindowsRuntime
{
	[DebuggerDisplay("Count = {Count}")]
	internal sealed class IVectorViewToIReadOnlyListAdapter
	{
		private IVectorViewToIReadOnlyListAdapter()
		{
		}

		[SecurityCritical]
		internal T Indexer_Get<T>(int index)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException("index");
			}
			IVectorView<T> vectorView = JitHelpers.UnsafeCast<IVectorView<T>>(this);
			T at;
			try
			{
				at = vectorView.GetAt((uint)index);
			}
			catch (Exception ex)
			{
				if (-2147483637 == ex._HResult)
				{
					throw new ArgumentOutOfRangeException("index");
				}
				throw;
			}
			return at;
		}

		[SecurityCritical]
		internal T Indexer_Get_Variance<T>(int index) where T : class
		{
			bool flag;
			Delegate targetForAmbiguousVariantCall = StubHelpers.GetTargetForAmbiguousVariantCall(this, typeof(IReadOnlyList<T>).TypeHandle.Value, out flag);
			if (targetForAmbiguousVariantCall != null)
			{
				return JitHelpers.UnsafeCast<Indexer_Get_Delegate<T>>(targetForAmbiguousVariantCall)(index);
			}
			if (flag)
			{
				return JitHelpers.UnsafeCast<T>(this.Indexer_Get<string>(index));
			}
			return this.Indexer_Get<T>(index);
		}
	}
}
