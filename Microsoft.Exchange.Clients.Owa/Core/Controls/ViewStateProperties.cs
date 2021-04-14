using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core.Controls
{
	internal sealed class ViewStateProperties
	{
		private ViewStateProperties()
		{
		}

		private const string ViewStatePropertyNamePrefix = "http://schemas.microsoft.com/exchange/";

		private static readonly Guid publicStringsGuid = new Guid("00020329-0000-0000-C000-000000000046");

		public static readonly PropertyDefinition ReadingPanePosition = GuidNamePropertyDefinition.CreateCustom("ReadingPanePosition", typeof(int), ViewStateProperties.publicStringsGuid, "http://schemas.microsoft.com/exchange/preview", PropertyFlags.None);

		public static readonly PropertyDefinition ReadingPanePositionMultiDay = GuidNamePropertyDefinition.CreateCustom("ReadingPanePositionMultiDay", typeof(int), ViewStateProperties.publicStringsGuid, "http://schemas.microsoft.com/exchange/previewMultiDay", PropertyFlags.None);

		public static readonly PropertyDefinition ViewWidth = GuidNamePropertyDefinition.CreateCustom("ViewWidth", typeof(int), ViewStateProperties.publicStringsGuid, "http://schemas.microsoft.com/exchange/wcviewwidth", PropertyFlags.None);

		public static readonly PropertyDefinition ViewHeight = GuidNamePropertyDefinition.CreateCustom("ViewHeight", typeof(int), ViewStateProperties.publicStringsGuid, "http://schemas.microsoft.com/exchange/wcviewheight", PropertyFlags.None);

		public static readonly PropertyDefinition MultiLine = GuidNamePropertyDefinition.CreateCustom("MultiLine", typeof(bool), ViewStateProperties.publicStringsGuid, "http://schemas.microsoft.com/exchange/wcmultiline", PropertyFlags.None);

		public static readonly PropertyDefinition SortColumn = GuidNamePropertyDefinition.CreateCustom("SortColumn", typeof(string), ViewStateProperties.publicStringsGuid, "http://schemas.microsoft.com/exchange/wcsortcolumn", PropertyFlags.None);

		public static readonly PropertyDefinition SortOrder = GuidNamePropertyDefinition.CreateCustom("SortOrder", typeof(int), ViewStateProperties.publicStringsGuid, "http://schemas.microsoft.com/exchange/wcsortorder", PropertyFlags.None);

		public static readonly PropertyDefinition CalendarViewType = GuidNamePropertyDefinition.CreateCustom("CalendarViewType", typeof(int), ViewStateProperties.publicStringsGuid, "http://schemas.microsoft.com/exchange/calviewtype", PropertyFlags.None);

		public static readonly PropertyDefinition DailyViewDays = GuidNamePropertyDefinition.CreateCustom("DailyViewDays", typeof(int), ViewStateProperties.publicStringsGuid, "http://schemas.microsoft.com/exchange/dailyviewdays", PropertyFlags.None);

		public static readonly PropertyDefinition TreeNodeCollapseStatus = GuidNamePropertyDefinition.CreateCustom("TreeNodeCollapseStatus", typeof(int), ViewStateProperties.publicStringsGuid, "http://schemas.microsoft.com/exchange/treenodecollapsestatus", PropertyFlags.None);

		public static readonly PropertyDefinition AddressBookLookupReadingPanePosition = GuidNamePropertyDefinition.CreateCustom("AddressBookLookupReadingPanePosition", typeof(int), ViewStateProperties.publicStringsGuid, "http://schemas.microsoft.com/exchange/ablupreview", PropertyFlags.None);

		public static readonly PropertyDefinition AddressBookLookupMultiLine = GuidNamePropertyDefinition.CreateCustom("AddressBookLookupMultiLine", typeof(bool), ViewStateProperties.publicStringsGuid, "http://schemas.microsoft.com/exchange/ablumultiline", PropertyFlags.None);

		public static readonly PropertyDefinition AddressBookPickerMultiLine = GuidNamePropertyDefinition.CreateCustom("AddressBookPickerMultiLine", typeof(bool), ViewStateProperties.publicStringsGuid, "http://schemas.microsoft.com/exchange/abpkmultiline", PropertyFlags.None);

		public static readonly PropertyDefinition ViewFilter = GuidNamePropertyDefinition.CreateCustom("ViewFilter", typeof(int), ViewStateProperties.publicStringsGuid, "http://schemas.microsoft.com/exchange/vwflt", PropertyFlags.None);

		public static readonly PropertyDefinition FilteredViewLabel = GuidNamePropertyDefinition.CreateCustom("FilteredViewLabel", typeof(string[]), ViewStateProperties.publicStringsGuid, "http://schemas.microsoft.com/exchange/fldfltr", PropertyFlags.None);

		public static readonly PropertyDefinition FilteredViewAccessTime = GuidNamePropertyDefinition.CreateCustom("FilteredViewAccessTime", typeof(ExDateTime), ViewStateProperties.publicStringsGuid, "http://schemas.microsoft.com/exchange/fltract", PropertyFlags.None);

		public static readonly PropertyDefinition FilteredViewFlags = GuidNamePropertyDefinition.CreateCustom("FilteredViewFlags", typeof(int), ViewStateProperties.publicStringsGuid, "http://schemas.microsoft.com/exchange/fltrflg", PropertyFlags.None);

		public static readonly PropertyDefinition FilteredViewFrom = GuidNamePropertyDefinition.CreateCustom("FilteredViewFrom", typeof(string), ViewStateProperties.publicStringsGuid, "http://schemas.microsoft.com/exchange/fltrfrm", PropertyFlags.None);

		public static readonly PropertyDefinition FilteredViewTo = GuidNamePropertyDefinition.CreateCustom("FilteredViewTo", typeof(string), ViewStateProperties.publicStringsGuid, "http://schemas.microsoft.com/exchange/fltrto", PropertyFlags.None);

		public static readonly PropertyDefinition FilterSourceFolder = GuidNamePropertyDefinition.CreateCustom("FilterSourceFolder", typeof(string), ViewStateProperties.publicStringsGuid, "http://schemas.microsoft.com/exchange/fltrsrcfldr", PropertyFlags.None);

		public static readonly PropertyDefinition ExpandedGroups = GuidNamePropertyDefinition.CreateCustom("ExpandedGroups", typeof(string[]), ViewStateProperties.publicStringsGuid, "http://schemas.microsoft.com/exchange/expandedgroups", PropertyFlags.None);

		public static readonly PropertyDefinition SignedOutOfIM = GuidNamePropertyDefinition.CreateCustom("SignedOutOfIM", typeof(bool), ViewStateProperties.publicStringsGuid, "http://schemas.microsoft.com/exchange/signedoutofim", PropertyFlags.None);
	}
}
