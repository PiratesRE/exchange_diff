using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UriComparer : IEqualityComparer<Uri>
	{
		private UriComparer()
		{
		}

		public static bool IsEqual(Uri x, Uri y)
		{
			return UriComparer.Default.Equals(x, y);
		}

		public bool Equals(Uri x, Uri y)
		{
			return Uri.Compare(x, y, UriComponents.AbsoluteUri, UriFormat.UriEscaped, StringComparison.OrdinalIgnoreCase) == 0;
		}

		public int GetHashCode(Uri x)
		{
			return x.GetHashCode();
		}

		public static UriComparer Default = new UriComparer();
	}
}
