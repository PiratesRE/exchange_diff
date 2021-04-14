using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class RandomPhotoRequestIdGenerator
	{
		internal static string Generate()
		{
			return Guid.NewGuid().ToString();
		}
	}
}
