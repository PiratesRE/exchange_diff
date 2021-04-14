using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Connections.Eas.Model.Common.AirSyncBase
{
	public enum SyncFilterType
	{
		[XmlEnum("0")]
		NoFilter,
		[XmlEnum("1")]
		OneDayBack,
		[XmlEnum("2")]
		ThreeDaysBack,
		[XmlEnum("3")]
		OneWeekBack,
		[XmlEnum("4")]
		TwoWeeksBack,
		[XmlEnum("5")]
		OneMonthBack,
		[XmlEnum("6")]
		ThreeMonthsBack,
		[XmlEnum("7")]
		SixMonthsBack,
		[XmlEnum("8")]
		IncompleteTasks
	}
}
