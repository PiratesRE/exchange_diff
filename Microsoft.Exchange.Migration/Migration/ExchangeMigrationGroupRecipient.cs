using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Migration
{
	internal sealed class ExchangeMigrationGroupRecipient : ExchangeMigrationRecipient, IMigrationSerializable
	{
		public ExchangeMigrationGroupRecipient() : base(MigrationUserRecipientType.Group)
		{
		}

		public override HashSet<PropTag> SupportedProperties
		{
			get
			{
				return ExchangeMigrationGroupRecipient.supportedProperties;
			}
		}

		public override HashSet<PropTag> RequiredProperties
		{
			get
			{
				return ExchangeMigrationGroupRecipient.requiredProperties;
			}
		}

		public string[] Members { get; set; }

		public bool MembersChanged { get; set; }

		public override void WriteToMessageItem(IMigrationStoreObject message, bool loaded)
		{
			base.WriteToMessageItem(message, loaded);
			if (this.MembersChanged)
			{
				((IMigrationMessageItem)message).DeleteAttachment("GroupMembers.csv");
				using (IMigrationAttachment migrationAttachment = ((IMigrationMessageItem)message).CreateAttachment("GroupMembers.csv"))
				{
					using (StreamWriter streamWriter = new StreamWriter(migrationAttachment.Stream))
					{
						ExchangeMigrationGroupMembersCsvSchema.Write(streamWriter, this.Members);
					}
					migrationAttachment.Save(null);
				}
			}
		}

		public override bool ReadFromMessageItem(IMigrationStoreObject message)
		{
			if (!base.ReadFromMessageItem(message))
			{
				return false;
			}
			List<string> list = null;
			IMigrationAttachment migrationAttachment;
			if (((IMigrationMessageItem)message).TryGetAttachment("GroupMembers.csv", PropertyOpenMode.ReadOnly, out migrationAttachment))
			{
				using (migrationAttachment)
				{
					try
					{
						list = new List<string>(ExchangeMigrationGroupMembersCsvSchema.Read(migrationAttachment.Stream));
					}
					catch (CsvValidationException)
					{
						string propertyValue = base.GetPropertyValue<string>(PropTag.SmtpAddress);
						throw new MigrationPermanentException(ServerStrings.MigrationGroupMembersAttachmentCorrupted(propertyValue));
					}
				}
			}
			if (list != null)
			{
				this.Members = list.ToArray();
			}
			this.MembersChanged = false;
			return true;
		}

		private static HashSet<PropTag> supportedProperties = new HashSet<PropTag>(new PropTag[]
		{
			PropTag.DisplayType,
			PropTag.DisplayName,
			PropTag.EmailAddress,
			PropTag.SmtpAddress,
			(PropTag)2148470815U,
			(PropTag)2148073485U,
			PropTag.Account,
			(PropTag)2148864031U,
			(PropTag)2148270111U
		});

		private static HashSet<PropTag> requiredProperties = new HashSet<PropTag>(new PropTag[]
		{
			PropTag.DisplayType,
			PropTag.EmailAddress,
			PropTag.SmtpAddress,
			PropTag.DisplayName
		});
	}
}
