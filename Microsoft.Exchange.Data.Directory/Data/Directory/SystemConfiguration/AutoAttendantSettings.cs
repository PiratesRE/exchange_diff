using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[XmlInclude(typeof(CustomMenuKeyMapping))]
	[Serializable]
	public class AutoAttendantSettings
	{
		public AutoAttendantSettings()
		{
			this.Parent = null;
		}

		public AutoAttendantSettings(UMAutoAttendant p, bool businessHourSetting)
		{
			this.Parent = p;
			this.IsBusinessHourSetting = businessHourSetting;
		}

		public AutoAttendantSettings(UMAutoAttendant p) : this(p, false)
		{
		}

		public static AutoAttendantSettings FromXml(string xml)
		{
			object obj = null;
			try
			{
				obj = SerializationHelper.Deserialize(xml, typeof(AutoAttendantSettings));
			}
			catch
			{
			}
			return (AutoAttendantSettings)obj;
		}

		public static string ToXml(AutoAttendantSettings aa)
		{
			return SerializationHelper.Serialize(aa);
		}

		public string TimeZoneKeyName;

		public bool IsBusinessHourSetting;

		public string WelcomeGreetingFilename;

		public bool WelcomeGreetingEnabled;

		public string GlobalInfoAnnouncementFilename;

		public bool MainMenuCustomPromptEnabled;

		public string MainMenuCustomPromptFilename;

		public bool TransferToOperatorEnabled;

		public string GlobalOperatorExtension;

		public bool KeyMappingEnabled;

		public CustomMenuKeyMapping[] KeyMapping;

		[XmlIgnore]
		public UMAutoAttendant Parent;
	}
}
