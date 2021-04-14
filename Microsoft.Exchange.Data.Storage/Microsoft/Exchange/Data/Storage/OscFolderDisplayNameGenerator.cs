using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class OscFolderDisplayNameGenerator : IEnumerable<string>, IEnumerable
	{
		public OscFolderDisplayNameGenerator(Guid provider, int count)
		{
			if (count < 1)
			{
				throw new ArgumentOutOfRangeException("count", count, "At least 1 name must be generated.");
			}
			this.defaultFolderDisplayName = OscProviderRegistry.GetDefaultFolderDisplayName(provider);
			this.namesToGenerate = count;
		}

		public IEnumerator<string> GetEnumerator()
		{
			yield return this.defaultFolderDisplayName;
			for (int suffix = 1; suffix < this.namesToGenerate; suffix++)
			{
				yield return string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[]
				{
					this.defaultFolderDisplayName,
					suffix
				});
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotSupportedException("Must use the generic version of GetEnumerator.");
		}

		private readonly int namesToGenerate;

		private readonly string defaultFolderDisplayName;
	}
}
