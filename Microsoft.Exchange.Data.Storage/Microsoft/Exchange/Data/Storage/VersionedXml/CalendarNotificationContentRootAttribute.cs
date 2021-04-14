using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Storage.VersionedXml
{
	public class CalendarNotificationContentRootAttribute : XmlRootAttribute
	{
		public CalendarNotificationContentRootAttribute() : base("CalendarNotificationContent")
		{
		}
	}
}
