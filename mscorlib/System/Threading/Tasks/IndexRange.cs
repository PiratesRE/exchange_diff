using System;
using System.Runtime.InteropServices;

namespace System.Threading.Tasks
{
	[StructLayout(LayoutKind.Auto)]
	internal struct IndexRange
	{
		internal long m_nFromInclusive;

		internal long m_nToExclusive;

		internal volatile Shared<long> m_nSharedCurrentIndexOffset;

		internal int m_bRangeFinished;
	}
}
