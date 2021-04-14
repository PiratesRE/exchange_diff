using System;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services
{
	public static class Extensions
	{
		internal static bool Contains(this PlacesSource sourcesMask, PlacesSource sourceFlag)
		{
			return (sourcesMask & sourceFlag) != PlacesSource.None;
		}
	}
}
