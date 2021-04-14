using System;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class XmlSerializationWriterAutoAttendantSettings : XmlSerializationWriter
	{
		public void Write5_AutoAttendantSettings(object o)
		{
			base.WriteStartDocument();
			if (o == null)
			{
				base.WriteNullTagLiteral("AutoAttendantSettings", string.Empty);
				return;
			}
			base.TopLevelElement();
			this.Write4_AutoAttendantSettings("AutoAttendantSettings", string.Empty, (AutoAttendantSettings)o, true, false);
		}

		private void Write4_AutoAttendantSettings(string n, string ns, AutoAttendantSettings o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(AutoAttendantSettings)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("AutoAttendantSettings", string.Empty);
			}
			base.WriteElementString("TimeZoneKeyName", string.Empty, o.TimeZoneKeyName);
			base.WriteElementStringRaw("IsBusinessHourSetting", string.Empty, XmlConvert.ToString(o.IsBusinessHourSetting));
			base.WriteElementString("WelcomeGreetingFilename", string.Empty, o.WelcomeGreetingFilename);
			base.WriteElementStringRaw("WelcomeGreetingEnabled", string.Empty, XmlConvert.ToString(o.WelcomeGreetingEnabled));
			base.WriteElementString("GlobalInfoAnnouncementFilename", string.Empty, o.GlobalInfoAnnouncementFilename);
			base.WriteElementStringRaw("MainMenuCustomPromptEnabled", string.Empty, XmlConvert.ToString(o.MainMenuCustomPromptEnabled));
			base.WriteElementString("MainMenuCustomPromptFilename", string.Empty, o.MainMenuCustomPromptFilename);
			base.WriteElementStringRaw("TransferToOperatorEnabled", string.Empty, XmlConvert.ToString(o.TransferToOperatorEnabled));
			base.WriteElementString("GlobalOperatorExtension", string.Empty, o.GlobalOperatorExtension);
			base.WriteElementStringRaw("KeyMappingEnabled", string.Empty, XmlConvert.ToString(o.KeyMappingEnabled));
			CustomMenuKeyMapping[] keyMapping = o.KeyMapping;
			if (keyMapping != null)
			{
				base.WriteStartElement("KeyMapping", string.Empty, null, false);
				for (int i = 0; i < keyMapping.Length; i++)
				{
					this.Write3_CustomMenuKeyMapping("CustomMenuKeyMapping", string.Empty, keyMapping[i], true, false);
				}
				base.WriteEndElement();
			}
			base.WriteEndElement(o);
		}

		private void Write3_CustomMenuKeyMapping(string n, string ns, CustomMenuKeyMapping o, bool isNullable, bool needType)
		{
			if (o == null)
			{
				if (isNullable)
				{
					base.WriteNullTagLiteral(n, ns);
				}
				return;
			}
			if (!needType)
			{
				Type type = o.GetType();
				if (!(type == typeof(CustomMenuKeyMapping)))
				{
					throw base.CreateUnknownTypeException(o);
				}
			}
			base.WriteStartElement(n, ns, o, false, null);
			if (needType)
			{
				base.WriteXsiType("CustomMenuKeyMapping", string.Empty);
			}
			base.WriteElementString("MappedKey", string.Empty, this.Write1_CustomMenuKey(o.MappedKey));
			base.WriteElementString("Description", string.Empty, o.Description);
			base.WriteElementString("Extension", string.Empty, o.Extension);
			base.WriteElementString("AutoAttendantName", string.Empty, o.AutoAttendantName);
			base.WriteElementString("LeaveVoicemailFor", string.Empty, o.LeaveVoicemailFor);
			base.WriteElementString("LegacyDNToUseForLeaveVoicemailFor", string.Empty, o.LegacyDNToUseForLeaveVoicemailFor);
			base.WriteElementString("TransferToMailbox", string.Empty, o.TransferToMailbox);
			base.WriteElementString("LegacyDNToUseForTransferToMailbox", string.Empty, o.LegacyDNToUseForTransferToMailbox);
			base.WriteElementString("PromptFileName", string.Empty, o.PromptFileName);
			base.WriteElementString("AsrPhrases", string.Empty, o.AsrPhrases);
			base.WriteElementString("AnnounceBusinessLocation", string.Empty, o.AnnounceBusinessLocation);
			base.WriteElementString("AnnounceBusinessHours", string.Empty, o.AnnounceBusinessHours);
			base.WriteEndElement(o);
		}

		private string Write1_CustomMenuKey(CustomMenuKey v)
		{
			string result;
			switch (v)
			{
			case CustomMenuKey.InvalidKey:
				result = "InvalidKey";
				break;
			case CustomMenuKey.One:
				result = "One";
				break;
			case CustomMenuKey.Two:
				result = "Two";
				break;
			case CustomMenuKey.Three:
				result = "Three";
				break;
			case CustomMenuKey.Four:
				result = "Four";
				break;
			case CustomMenuKey.Five:
				result = "Five";
				break;
			case CustomMenuKey.Six:
				result = "Six";
				break;
			case CustomMenuKey.Seven:
				result = "Seven";
				break;
			case CustomMenuKey.Eight:
				result = "Eight";
				break;
			case CustomMenuKey.Nine:
				result = "Nine";
				break;
			case CustomMenuKey.NotSpecified:
				result = "NotSpecified";
				break;
			case CustomMenuKey.Timeout:
				result = "Timeout";
				break;
			default:
				throw base.CreateInvalidEnumValueException(((long)v).ToString(CultureInfo.InvariantCulture), "Microsoft.Exchange.Data.CustomMenuKey");
			}
			return result;
		}

		protected override void InitCallbacks()
		{
		}
	}
}
