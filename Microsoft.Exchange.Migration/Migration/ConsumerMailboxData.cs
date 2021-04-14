using System;
using System.Xml.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConsumerMailboxData : IMailboxData, IMigrationSerializable
	{
		public ConsumerMailboxData(Guid mailboxGuid, Guid mailboxDatabaseGuid)
		{
			MigrationUtil.ThrowOnGuidEmptyArgument(mailboxGuid, "mailboxGuid");
			MigrationUtil.ThrowOnGuidEmptyArgument(mailboxDatabaseGuid, "mailboxDatabaseGuid");
			this.UserMailboxId = mailboxGuid;
			this.UserMailboxDatabaseId = mailboxDatabaseGuid;
		}

		internal ConsumerMailboxData()
		{
		}

		public Guid UserMailboxId { get; private set; }

		public Guid UserMailboxDatabaseId { get; private set; }

		public OrganizationId OrganizationId
		{
			get
			{
				return OrganizationId.ForestWideOrgId;
			}
		}

		public string MailboxIdentifier { get; private set; }

		public MigrationUserRecipientType RecipientType
		{
			get
			{
				return MigrationUserRecipientType.Mailbox;
			}
		}

		PropertyDefinition[] IMigrationSerializable.PropertyDefinitions
		{
			get
			{
				return ConsumerMailboxData.ConsumerMailboxDataPropertyDefinition;
			}
		}

		public TIdParameter GetIdParameter<TIdParameter>() where TIdParameter : IIdentityParameter
		{
			IIdentityParameter identityParameter;
			if (typeof(TIdParameter) == typeof(MailboxIdParameter))
			{
				identityParameter = new MailboxIdParameter(this.MailboxIdentifier);
			}
			else
			{
				if (!(typeof(TIdParameter) == typeof(MailboxOrMailUserIdParameter)))
				{
					throw new ArgumentException(string.Format("type not supported {0}", typeof(TIdParameter).Name), "TIdParameter");
				}
				identityParameter = new MailboxOrMailUserIdParameter(this.MailboxIdentifier);
			}
			return (TIdParameter)((object)identityParameter);
		}

		public void Update(string identifier, OrganizationId organizationId)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(identifier, "identifier");
			MigrationUtil.AssertOrThrow(organizationId == null || organizationId == OrganizationId.ForestWideOrgId, "We expect ConsumerMailboxes to always be in FirstOrg", new object[0]);
			this.MailboxIdentifier = identifier;
		}

		public override string ToString()
		{
			return string.Format("{0}:{1}:{2}", this.MailboxIdentifier, this.UserMailboxId, this.UserMailboxDatabaseId);
		}

		public bool ReadFromMessageItem(IMigrationStoreObject message)
		{
			this.UserMailboxId = MigrationHelper.GetGuidProperty(message, MigrationBatchMessageSchema.MigrationJobItemMailboxId, false);
			this.UserMailboxDatabaseId = MigrationHelper.GetGuidProperty(message, MigrationBatchMessageSchema.MigrationJobItemMailboxDatabaseId, false);
			return true;
		}

		public void WriteToMessageItem(IMigrationStoreObject message, bool loaded)
		{
			message[MigrationBatchMessageSchema.MigrationJobItemMailboxDatabaseId] = this.UserMailboxDatabaseId;
			message[MigrationBatchMessageSchema.MigrationJobItemMailboxId] = this.UserMailboxId;
		}

		public XElement GetDiagnosticInfo(IMigrationDataProvider dataProvider, MigrationDiagnosticArgument argument)
		{
			return new XElement("ConsumerMailboxData", new object[]
			{
				new XElement("mailboxId", this.UserMailboxId),
				new XElement("userMailboxDatabaseId", this.UserMailboxDatabaseId)
			});
		}

		public static readonly PropertyDefinition[] ConsumerMailboxDataPropertyDefinition = new StorePropertyDefinition[]
		{
			MigrationBatchMessageSchema.MigrationJobItemMailboxId,
			MigrationBatchMessageSchema.MigrationJobItemMailboxDatabaseId
		};
	}
}
