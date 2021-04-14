using System;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class GroupProvisioningSnapshot : ProvisioningSnapshot, IMigrationSerializable
	{
		public GroupProvisioningSnapshot(ProvisionedObject provisionedObject) : base(provisionedObject)
		{
			this.CountOfProvisionedMembers = provisionedObject.GroupMemberProvisioned;
			this.CountOfSkippedMembers = provisionedObject.GroupMemberSkipped;
		}

		public GroupProvisioningSnapshot()
		{
		}

		public GroupMembershipProvisioningState ProvisioningState { get; set; }

		public int CountOfProvisionedMembers { get; set; }

		public int CountOfSkippedMembers { get; set; }

		public PropertyDefinition[] PropertyDefinitions
		{
			get
			{
				return GroupProvisioningSnapshot.GroupPropertyDefinitions;
			}
		}

		public void WriteToMessageItem(IMigrationStoreObject message, bool loaded)
		{
			message[MigrationBatchMessageSchema.MigrationJobItemGroupMemberProvisioningState] = (int)this.ProvisioningState;
			message[MigrationBatchMessageSchema.MigrationJobItemGroupMemberSkipped] = this.CountOfSkippedMembers;
			message[MigrationBatchMessageSchema.MigrationJobItemGroupMemberProvisioned] = this.CountOfProvisionedMembers;
		}

		public bool ReadFromMessageItem(IMigrationStoreObject message)
		{
			int valueOrDefault = message.GetValueOrDefault<int>(MigrationBatchMessageSchema.MigrationJobItemGroupMemberProvisioningState, 0);
			if (!Enum.IsDefined(typeof(GroupMembershipProvisioningState), valueOrDefault))
			{
				throw new MigrationDataCorruptionException("Invalid MigrationJobItemGroupMemberProvisioningState. Message ID: " + message.Id);
			}
			this.ProvisioningState = (GroupMembershipProvisioningState)valueOrDefault;
			this.CountOfSkippedMembers = message.GetValueOrDefault<int>(MigrationBatchMessageSchema.MigrationJobItemGroupMemberSkipped, 0);
			this.CountOfProvisionedMembers = message.GetValueOrDefault<int>(MigrationBatchMessageSchema.MigrationJobItemGroupMemberProvisioned, 0);
			return true;
		}

		public XElement GetDiagnosticInfo(IMigrationDataProvider dataProvider, MigrationDiagnosticArgument argument)
		{
			XElement xelement = new XElement("GroupProvisioningSnapshot");
			xelement.Add(new object[]
			{
				new XElement("ProvisioningState", this.ProvisioningState),
				new XElement("MembersProvisioned", this.CountOfProvisionedMembers),
				new XElement("MembersSkipped", this.CountOfSkippedMembers)
			});
			return xelement;
		}

		public static readonly PropertyDefinition[] GroupPropertyDefinitions = new PropertyDefinition[]
		{
			MigrationBatchMessageSchema.MigrationJobItemGroupMemberProvisioned,
			MigrationBatchMessageSchema.MigrationJobItemGroupMemberSkipped,
			MigrationBatchMessageSchema.MigrationJobItemGroupMemberProvisioningState
		};
	}
}
