using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public static class VisibilityHelper
	{
		public static Visibility Select(IEnumerable<Visibility> visibilities)
		{
			Visibility visibility = Visibility.Public;
			foreach (Visibility v in visibilities)
			{
				visibility = VisibilityHelper.Select(visibility, v);
				if (visibility == Visibility.Private)
				{
					return visibility;
				}
			}
			return visibility;
		}

		public static Visibility Select(Visibility v1, Visibility v2)
		{
			if (v1 == Visibility.Private || v2 == Visibility.Private)
			{
				return Visibility.Private;
			}
			if (v1 == Visibility.Redacted || v2 == Visibility.Redacted)
			{
				return Visibility.Redacted;
			}
			if (v1 == Visibility.Public || v2 == Visibility.Public)
			{
				return Visibility.Public;
			}
			return Visibility.Redacted;
		}

		public static string GetPrefix(Visibility v)
		{
			switch (v)
			{
			case Visibility.Public:
				return "PUBLIC";
			case Visibility.Redacted:
				return "REDACTED";
			case Visibility.Private:
				return "PRIVATE";
			default:
				return "UNKNOWN";
			}
		}
	}
}
