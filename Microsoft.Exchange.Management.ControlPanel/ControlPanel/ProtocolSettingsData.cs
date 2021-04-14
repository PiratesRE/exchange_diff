using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ProtocolSettingsData : BaseRow
	{
		public ProtocolSettingsData(CASMailbox casMailbox) : base(casMailbox.ToIdentity(), casMailbox)
		{
			this.casMailbox = casMailbox;
		}

		[DataMember]
		public string ExternalPopSetting
		{
			get
			{
				string text = this.casMailbox.ExternalPopSettings;
				if (string.IsNullOrEmpty(text))
				{
					text = OwaOptionStrings.SettingNotAvailable;
				}
				if (!this.casMailbox.PopEnabled)
				{
					text = OwaOptionStrings.SettingAccessDisabled;
				}
				return text;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string ExternalImapSetting
		{
			get
			{
				string text = this.casMailbox.ExternalImapSettings;
				if (string.IsNullOrEmpty(text))
				{
					text = OwaOptionStrings.SettingNotAvailable;
				}
				if (!this.casMailbox.ImapEnabled)
				{
					text = OwaOptionStrings.SettingAccessDisabled;
				}
				return text;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string ExternalSmtpSetting
		{
			get
			{
				string text = this.casMailbox.ExternalSmtpSettings;
				if (string.IsNullOrEmpty(text))
				{
					text = OwaOptionStrings.SettingNotAvailable;
				}
				return text;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		private CASMailbox casMailbox;
	}
}
