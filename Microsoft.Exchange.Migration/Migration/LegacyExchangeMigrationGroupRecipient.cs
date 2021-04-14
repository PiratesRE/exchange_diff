using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Migration
{
	internal sealed class LegacyExchangeMigrationGroupRecipient : ExchangeMigrationRecipient, IMigrationSerializable
	{
		public LegacyExchangeMigrationGroupRecipient() : base(MigrationUserRecipientType.Group)
		{
			this.memberBatchSize = ConfigBase<MigrationServiceConfigSchema>.GetConfig<int>("MigrationGroupMembersBatchSize");
		}

		public int CountOfProvisionedMembers { get; private set; }

		public int CountOfSkippedMembers { get; private set; }

		public override HashSet<PropTag> SupportedProperties
		{
			get
			{
				return LegacyExchangeMigrationGroupRecipient.supportedProperties;
			}
		}

		public override HashSet<PropTag> RequiredProperties
		{
			get
			{
				return LegacyExchangeMigrationGroupRecipient.requiredProperties;
			}
		}

		public bool IsMembersRetrieved()
		{
			return this.provisioningState != GroupMembershipProvisioningState.MemberNotRetrieved;
		}

		public bool IsMembersProvisioned()
		{
			return this.provisioningState == GroupMembershipProvisioningState.MemberRetrievedAndProvisioned;
		}

		public List<string> GetNextBatchOfMembers(IMigrationDataProvider provider, IMigrationMessageItem message)
		{
			MigrationUtil.ThrowOnNullArgument(provider, "provider");
			MigrationUtil.ThrowOnNullArgument(message, "message");
			if (!this.IsMembersRetrieved())
			{
				string propertyValue = base.GetPropertyValue<string>(PropTag.SmtpAddress);
				throw new MigrationPermanentException(ServerStrings.MigrationGroupMembersNotAvailable(propertyValue));
			}
			if (this.IsMembersProvisioned())
			{
				return null;
			}
			List<string> list = new List<string>(this.memberBatchSize);
			using (IMigrationAttachment attachment = message.GetAttachment("GroupMembers.csv", PropertyOpenMode.ReadOnly))
			{
				try
				{
					int num = 1;
					int num2 = this.CountOfSkippedMembers + this.CountOfProvisionedMembers;
					foreach (string item in ExchangeMigrationGroupMembersCsvSchema.Read(attachment.Stream))
					{
						if (num > num2)
						{
							list.Add(item);
							if (list.Count == this.memberBatchSize)
							{
								break;
							}
						}
						num++;
					}
				}
				catch (CsvValidationException)
				{
					string propertyValue2 = base.GetPropertyValue<string>(PropTag.SmtpAddress);
					throw new MigrationPermanentException(ServerStrings.MigrationGroupMembersAttachmentCorrupted(propertyValue2));
				}
			}
			if (list.Count == 0)
			{
				this.provisioningState = GroupMembershipProvisioningState.MemberRetrievedAndProvisioned;
				this.WriteToMessageItem(message);
				return null;
			}
			return list;
		}

		public void SetGroupMembersInfo(IMigrationMessageItem message, string[] members)
		{
			MigrationUtil.ThrowOnNullArgument(message, "message");
			MigrationUtil.ThrowOnNullArgument(members, "members");
			if (this.IsMembersRetrieved())
			{
				string propertyValue = base.GetPropertyValue<string>(PropTag.SmtpAddress);
				throw new MigrationPermanentException(ServerStrings.MigrationGroupMembersAlreadyAvailable(propertyValue));
			}
			message.DeleteAttachment("GroupMembers.csv");
			using (IMigrationAttachment migrationAttachment = message.CreateAttachment("GroupMembers.csv"))
			{
				using (StreamWriter streamWriter = new StreamWriter(migrationAttachment.Stream))
				{
					ExchangeMigrationGroupMembersCsvSchema.Write(streamWriter, members);
				}
				migrationAttachment.Save(null);
				this.CountOfProvisionedMembers = 0;
				this.CountOfSkippedMembers = 0;
				if (members.Length > 0)
				{
					this.provisioningState = GroupMembershipProvisioningState.MemberRetrievedButNotProvisioned;
				}
				else
				{
					this.provisioningState = GroupMembershipProvisioningState.MemberRetrievedAndProvisioned;
				}
			}
			this.WriteToMessageItem(message);
		}

		public void UpdateGroupMembersInfo(IMigrationMessageItem message, int membersProvisioned, int membersSkipped)
		{
			MigrationUtil.ThrowOnNullArgument(message, "message");
			this.CountOfSkippedMembers += membersSkipped;
			this.CountOfProvisionedMembers += membersProvisioned;
			this.WriteToMessageItem(message);
		}

		public override void WriteToMessageItem(IMigrationStoreObject message, bool loaded)
		{
			base.WriteToMessageItem(message, loaded);
			this.WriteToMessageItem(message);
		}

		public override bool ReadFromMessageItem(IMigrationStoreObject message)
		{
			if (!base.ReadFromMessageItem(message))
			{
				return false;
			}
			int valueOrDefault = message.GetValueOrDefault<int>(MigrationBatchMessageSchema.MigrationJobItemGroupMemberProvisioningState, 0);
			if (!Enum.IsDefined(typeof(GroupMembershipProvisioningState), valueOrDefault))
			{
				throw new MigrationDataCorruptionException("Invalid MigrationJobItemGroupMemberProvisioningState. Message ID: " + message.Id);
			}
			this.provisioningState = (GroupMembershipProvisioningState)valueOrDefault;
			this.CountOfSkippedMembers = message.GetValueOrDefault<int>(MigrationBatchMessageSchema.MigrationJobItemGroupMemberSkipped, 0);
			this.CountOfProvisionedMembers = message.GetValueOrDefault<int>(MigrationBatchMessageSchema.MigrationJobItemGroupMemberProvisioned, 0);
			return true;
		}

		private void WriteToMessageItem(IMigrationStoreObject message)
		{
			message[MigrationBatchMessageSchema.MigrationJobItemGroupMemberProvisioningState] = (int)this.provisioningState;
			message[MigrationBatchMessageSchema.MigrationJobItemGroupMemberSkipped] = this.CountOfSkippedMembers;
			message[MigrationBatchMessageSchema.MigrationJobItemGroupMemberProvisioned] = this.CountOfProvisionedMembers;
		}

		public static readonly PropertyDefinition[] GroupPropertyDefinitions = new PropertyDefinition[]
		{
			MigrationBatchMessageSchema.MigrationJobItemGroupMemberProvisioned,
			MigrationBatchMessageSchema.MigrationJobItemGroupMemberSkipped,
			MigrationBatchMessageSchema.MigrationJobItemGroupMemberProvisioningState
		};

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

		private GroupMembershipProvisioningState provisioningState;

		private readonly int memberBatchSize;
	}
}
