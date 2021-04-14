using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Collections
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMruDictionaryPerfCounters
	{
		void CacheHit();

		void CacheMiss();

		void CacheAdd(bool overwrite, bool remove);

		void CacheRemove();

		void FileRead(int count);

		void FileWrite(int count);
	}
}
