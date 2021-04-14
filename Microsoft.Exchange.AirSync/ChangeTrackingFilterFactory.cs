using System;

namespace Microsoft.Exchange.AirSync
{
	internal class ChangeTrackingFilterFactory
	{
		internal static ChangeTrackingFilter CreateFilter(string type, int version)
		{
			if (version >= 120 && type == "Calendar")
			{
				return new ChangeTrackingFilterFactory.V12CalendarFilter();
			}
			if (version >= 140 && type == "Email")
			{
				return new ChangeTrackingFilterFactory.V14EmailFilter();
			}
			if (version >= 120 && type == "Email")
			{
				return new ChangeTrackingFilterFactory.V12EmailFilter();
			}
			if (version <= 25 && type == "Email")
			{
				return new ChangeTrackingFilterFactory.V25EmailFilter();
			}
			if (type == "RecipientInfoCache")
			{
				return new ChangeTrackingFilterFactory.V14RecipientInfoCacheFilter();
			}
			return new ChangeTrackingFilterFactory.AllNodesFilter();
		}

		internal class AllNodesFilter : ChangeTrackingFilter
		{
			internal AllNodesFilter() : base(new ChangeTrackingNode[]
			{
				ChangeTrackingNode.AllNodes
			}, false)
			{
			}
		}

		internal class V12CalendarFilter : ChangeTrackingFilter
		{
			internal V12CalendarFilter() : base(new ChangeTrackingNode[]
			{
				ChangeTrackingNode.AllOtherNodes,
				new ChangeTrackingNode("Calendar:", "DtStamp"),
				new ChangeTrackingNode("Calendar:", "Attendees")
			}, true)
			{
			}
		}

		internal class V14EmailFilter : ChangeTrackingFilter
		{
			internal V14EmailFilter() : base(new ChangeTrackingNode[]
			{
				ChangeTrackingNode.AllOtherNodes,
				new ChangeTrackingNode("Email:", "Read"),
				new ChangeTrackingNode("Email:", "Flag"),
				new ChangeTrackingNode("Email2:", "UmUserNotes"),
				new ChangeTrackingNode("Email2:", "LastVerbExecuted"),
				new ChangeTrackingNode("Email2:", "LastVerbExecutionTime"),
				new ChangeTrackingNode("Email:", "Categories")
			}, true)
			{
			}
		}

		internal class V14RecipientInfoCacheFilter : ChangeTrackingFilter
		{
			internal V14RecipientInfoCacheFilter() : base(new ChangeTrackingNode[]
			{
				ChangeTrackingNode.AllOtherNodes,
				new ChangeTrackingNode("Contacts:", "Email1Address"),
				new ChangeTrackingNode("Contacts:", "FileAs"),
				new ChangeTrackingNode("Contacts:", "Alias"),
				new ChangeTrackingNode("Contacts:", "WeightedRank")
			}, true)
			{
			}
		}

		internal class V12EmailFilter : ChangeTrackingFilter
		{
			internal V12EmailFilter() : base(new ChangeTrackingNode[]
			{
				ChangeTrackingNode.AllOtherNodes,
				new ChangeTrackingNode("Email:", "Read"),
				new ChangeTrackingNode("Email:", "Flag")
			}, true)
			{
			}
		}

		internal class V25EmailFilter : ChangeTrackingFilter
		{
			internal V25EmailFilter() : base(new ChangeTrackingNode[]
			{
				ChangeTrackingNode.AllOtherNodes,
				new ChangeTrackingNode("Email:", "Read")
			}, true)
			{
			}
		}
	}
}
