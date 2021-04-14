using System;

namespace System.Runtime.Serialization
{
	[Serializable]
	internal class FixupHolder
	{
		internal FixupHolder(long id, object fixupInfo, int fixupType)
		{
			this.m_id = id;
			this.m_fixupInfo = fixupInfo;
			this.m_fixupType = fixupType;
		}

		internal const int ArrayFixup = 1;

		internal const int MemberFixup = 2;

		internal const int DelayedFixup = 4;

		internal long m_id;

		internal object m_fixupInfo;

		internal int m_fixupType;
	}
}
