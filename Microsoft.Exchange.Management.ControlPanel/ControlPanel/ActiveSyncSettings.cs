using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ActiveSyncSettings : BaseRow
	{
		public ActiveSyncSettings(ActiveSyncOrganizationSettings settings) : base(settings)
		{
			this.settings = settings;
			if (settings.AdminMailRecipients == null)
			{
				this.recipients = null;
				return;
			}
			this.recipients = RecipientObjectResolver.Instance.ResolveSmtpAddress(settings.AdminMailRecipients.ToArray());
		}

		[DataMember]
		public string Caption
		{
			get
			{
				return Strings.EditActiveSyncSettingsCaption;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DefaultAccessLevel
		{
			get
			{
				return this.settings.DefaultAccessLevel.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DefaultAccessLevelDescription
		{
			get
			{
				switch (this.settings.DefaultAccessLevel)
				{
				case DeviceAccessLevel.Allow:
					return Strings.AccessLevelDescriptionAllow;
				case DeviceAccessLevel.Block:
					return Strings.AccessLevelDescriptionBlock;
				case DeviceAccessLevel.Quarantine:
					return Strings.AccessLevelDescriptionQuarantine;
				default:
					throw new NotSupportedException();
				}
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public IEnumerable<RecipientObjectResolverRow> AdminMailRecipients
		{
			get
			{
				return this.recipients;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string AdminMailRecipientsDescription
		{
			get
			{
				if (this.recipients != null && this.recipients.Any<RecipientObjectResolverRow>())
				{
					return Strings.QNoteEmailsDescriptionYes;
				}
				return Strings.QNoteEmailsDescriptionNo;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string UserMailInsert
		{
			get
			{
				return this.settings.UserMailInsert;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string UserMailInsertDescription
		{
			get
			{
				if (string.IsNullOrEmpty(this.settings.UserMailInsert))
				{
					return Strings.EmailInsertDescriptionNo;
				}
				return Strings.EmailInsertDescriptionYes;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		private readonly ActiveSyncOrganizationSettings settings;

		private readonly IEnumerable<RecipientObjectResolverRow> recipients;
	}
}
