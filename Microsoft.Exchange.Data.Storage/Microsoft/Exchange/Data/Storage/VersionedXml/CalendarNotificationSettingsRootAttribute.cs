using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Storage.VersionedXml
{
	public class CalendarNotificationSettingsRootAttribute : XmlRootAttribute
	{
		public CalendarNotificationSettingsRootAttribute() : base("CalendarNotificationSettings")
		{
		}
	}
}
