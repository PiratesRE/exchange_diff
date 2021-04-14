using System;

namespace Microsoft.Exchange.Data.Storage.VersionedXml
{
	[CalendarNotificationContentRoot]
	[Serializable]
	public class CalendarNotificationContentBase : VersionedXmlBase
	{
		public CalendarNotificationContentBase()
		{
		}

		public CalendarNotificationContentBase(Version version) : base(version)
		{
		}
	}
}
