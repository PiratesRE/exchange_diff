using System;

namespace System.Threading
{
	internal struct CancellationCallbackCoreWorkArguments
	{
		public CancellationCallbackCoreWorkArguments(SparselyPopulatedArrayFragment<CancellationCallbackInfo> currArrayFragment, int currArrayIndex)
		{
			this.m_currArrayFragment = currArrayFragment;
			this.m_currArrayIndex = currArrayIndex;
		}

		internal SparselyPopulatedArrayFragment<CancellationCallbackInfo> m_currArrayFragment;

		internal int m_currArrayIndex;
	}
}
