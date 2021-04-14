using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	internal sealed class OriginPreferenceComparer
	{
		private OriginPreferenceComparer()
		{
		}

		internal int Compare(Context context, Mailbox mailbox, StorePropTag proptag, object value1, string origin1, DateTime creationTime1, object value2, string origin2, DateTime creationTime2)
		{
			if (object.ReferenceEquals(value1, value2))
			{
				return 0;
			}
			int num = this.CompareStringValue(context, mailbox, value1, origin1, value2, origin2);
			if (num == 0)
			{
				return Comparer<DateTime>.Default.Compare(creationTime1, creationTime2);
			}
			return num;
		}

		private int CompareStringValue(Context context, Mailbox mailbox, object value1, string origin1, object value2, string origin2)
		{
			int y = this.MapValueAndOriginToPriority(new Func<Context, Mailbox, object, bool>(this.NotBlank), context, mailbox, value1, origin1);
			int x = this.MapValueAndOriginToPriority(new Func<Context, Mailbox, object, bool>(this.NotBlank), context, mailbox, value2, origin2);
			return Comparer<int>.Default.Compare(x, y);
		}

		private int MapValueAndOriginToPriority(Func<Context, Mailbox, object, bool> valuePreconditionEvaluator, Context context, Mailbox mailbox, object value, string origin)
		{
			if (!valuePreconditionEvaluator(context, mailbox, value))
			{
				return int.MaxValue;
			}
			return this.MapOriginToPriority(origin);
		}

		private int MapOriginToPriority(string origin)
		{
			if (string.IsNullOrWhiteSpace(origin))
			{
				return 0;
			}
			if (string.Equals(origin, OriginPreferenceComparer.WellKnownNetworkNames.GAL))
			{
				return 1;
			}
			if (string.Equals(origin, OriginPreferenceComparer.WellKnownNetworkNames.LinkedIn))
			{
				return 4;
			}
			if (string.Equals(origin, OriginPreferenceComparer.WellKnownNetworkNames.Facebook))
			{
				return 5;
			}
			return 9;
		}

		private bool NotBlank(Context context, Mailbox mailbox, object value)
		{
			return value != null && !string.IsNullOrWhiteSpace(value.ToString());
		}

		internal static readonly OriginPreferenceComparer Instance = new OriginPreferenceComparer();

		private static class WellKnownNetworkNames
		{
			public static readonly string Facebook = "Facebook";

			public static readonly string LinkedIn = "LinkedIn";

			public static readonly string Sharepoint = "Sharepoint";

			public static readonly string GAL = "GAL";

			public static readonly string QuickContacts = "QuickContacts";

			public static readonly string LyncContacts = "LyncContacts";

			public static readonly string RecipientCache = "RecipientCache";
		}
	}
}
