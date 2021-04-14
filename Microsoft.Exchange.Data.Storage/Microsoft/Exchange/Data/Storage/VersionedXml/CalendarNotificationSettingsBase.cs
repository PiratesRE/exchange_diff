using System;

namespace Microsoft.Exchange.Data.Storage.VersionedXml
{
	[CalendarNotificationSettingsRoot]
	[Serializable]
	public class CalendarNotificationSettingsBase : VersionedXmlBase
	{
		public CalendarNotificationSettingsBase()
		{
		}

		public CalendarNotificationSettingsBase(Version version) : base(version)
		{
		}
	}
}
