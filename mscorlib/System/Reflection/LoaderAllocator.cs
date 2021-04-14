using System;

namespace System.Reflection
{
	internal sealed class LoaderAllocator
	{
		private LoaderAllocator()
		{
			this.m_slots = new object[5];
			this.m_scout = new LoaderAllocatorScout();
		}

		private LoaderAllocatorScout m_scout;

		private object[] m_slots;

		internal CerHashtable<RuntimeMethodInfo, RuntimeMethodInfo> m_methodInstantiations;

		private int m_slotsUsed;
	}
}
