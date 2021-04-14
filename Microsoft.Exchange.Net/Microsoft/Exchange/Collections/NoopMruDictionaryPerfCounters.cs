using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Collections
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NoopMruDictionaryPerfCounters : IMruDictionaryPerfCounters
	{
		private NoopMruDictionaryPerfCounters()
		{
		}

		public void CacheHit()
		{
		}

		public void CacheMiss()
		{
		}

		public void CacheAdd(bool overwrite, bool remove)
		{
		}

		public void CacheRemove()
		{
		}

		public void FileRead(int count)
		{
		}

		public void FileWrite(int count)
		{
		}

		public static NoopMruDictionaryPerfCounters Instance = new NoopMruDictionaryPerfCounters();
	}
}
