using System;
using System.Globalization;
using System.Xml.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxData : IMailboxData, IMigrationSerializable
	{
		public MailboxData(Guid mailboxGuid, Fqdn mailboxServer, string mailboxLegacyDN, ADObjectId mailboxADObjectId, Guid exchangeObjectId) : this(mailboxGuid, Guid.Empty, mailboxServer, mailboxLegacyDN, mailboxADObjectId, exchangeObjectId)
		{
		}

		public MailboxData(Guid mailboxGuid, Guid mailboxDatabaseGuid, Fqdn mailboxServer, string mailboxLegacyDN, ADObjectId mailboxADObjectId, Guid exchangeObjectId)
		{
			MigrationUtil.ThrowOnNullArgument(mailboxGuid, "mailboxGuid");
			MigrationUtil.ThrowOnNullArgument(mailboxServer, "mailboxServer");
			MigrationUtil.ThrowOnNullOrEmptyArgument(mailboxLegacyDN, "mailboxLegacyDN");
			MigrationUtil.ThrowOnGuidEmptyArgument(mailboxGuid, "mailboxGuid");
			MigrationUtil.ThrowOnNullArgument(mailboxADObjectId, "mailboxADObjectId");
			MigrationUtil.ThrowOnGuidEmptyArgument(exchangeObjectId, "exchangeObjectId");
			this.UserMailboxId = mailboxGuid;
			this.UserMailboxDatabaseId = mailboxDatabaseGuid;
			this.MailboxServer = mailboxServer;
			this.MailboxLegacyDN = mailboxLegacyDN;
			this.UserMailboxADObjectId = mailboxADObjectId;
			this.ExchangeObjectId = exchangeObjectId;
			this.recipientType = null;
		}

		public MailboxData(Guid mailboxGuid, string mailboxLegacyDn, ADObjectId mailboxADObjectId, Guid exchangeObjectId)
		{
			MigrationUtil.ThrowOnGuidEmptyArgument(mailboxGuid, "mailboxGuid");
			MigrationUtil.ThrowOnNullOrEmptyArgument(mailboxLegacyDn, "mailboxLegacyDn");
			MigrationUtil.ThrowOnNullArgument(mailboxADObjectId, "mailboxADObjectId");
			MigrationUtil.ThrowOnGuidEmptyArgument(exchangeObjectId, "exchangeObjectId");
			this.UserMailboxId = mailboxGuid;
			this.MailboxLegacyDN = mailboxLegacyDn;
			this.UserMailboxADObjectId = mailboxADObjectId;
			this.ExchangeObjectId = exchangeObjectId;
			this.recipientType = null;
		}

		internal MailboxData()
		{
		}

		public Guid UserMailboxId { get; private set; }

		public Guid UserMailboxDatabaseId { get; private set; }

		public Fqdn MailboxServer { get; private set; }

		public string MailboxLegacyDN { get; private set; }

		public ADObjectId UserMailboxADObjectId { get; private set; }

		public Guid ExchangeObjectId { get; private set; }

		public string Name { get; private set; }

		public OrganizationId OrganizationId { get; private set; }

		public string MailboxIdentifier
		{
			get
			{
				if (this.ExchangeObjectId != Guid.Empty)
				{
					return MailboxData.CreateMailboxIdentifierString(this.OrganizationId, this.ExchangeObjectId);
				}
				return this.UserMailboxADObjectId.DistinguishedName;
			}
		}

		public MigrationUserRecipientType RecipientType
		{
			get
			{
				if (this.recipientType != null)
				{
					return this.recipientType.Value;
				}
				if (string.IsNullOrEmpty(this.MailboxServer))
				{
					return MigrationUserRecipientType.Mailuser;
				}
				return MigrationUserRecipientType.Mailbox;
			}
		}

		PropertyDefinition[] IMigrationSerializable.PropertyDefinitions
		{
			get
			{
				return MailboxData.MailboxDataPropertyDefinition;
			}
		}

		public static MailboxData CreateFromADUser(ADUser user)
		{
			return new MailboxData
			{
				ExchangeObjectId = user.ExchangeObjectId,
				MailboxLegacyDN = user.LegacyExchangeDN,
				MailboxServer = Fqdn.Parse(user.ServerName),
				Name = user.Name,
				OrganizationId = user.OrganizationId,
				UserMailboxADObjectId = user.Id,
				UserMailboxDatabaseId = user.Database.ObjectGuid,
				UserMailboxId = user.ExchangeGuid
			};
		}

		public TIdParameter GetIdParameter<TIdParameter>() where TIdParameter : IIdentityParameter
		{
			string text = null;
			if (this.ExchangeObjectId != Guid.Empty)
			{
				text = MailboxData.CreateMailboxIdentifierString(this.OrganizationId, this.ExchangeObjectId);
			}
			IIdentityParameter identityParameter;
			if (typeof(TIdParameter) == typeof(MoveRequestIdParameter))
			{
				if (!string.IsNullOrEmpty(text))
				{
					identityParameter = new MoveRequestIdParameter(text);
				}
				else
				{
					identityParameter = new MoveRequestIdParameter(this.UserMailboxADObjectId);
				}
			}
			else if (typeof(TIdParameter) == typeof(MailboxIdParameter))
			{
				if (!string.IsNullOrEmpty(text))
				{
					identityParameter = new MailboxIdParameter(text);
				}
				else
				{
					identityParameter = new MailboxIdParameter(this.UserMailboxADObjectId);
				}
			}
			else
			{
				if (!(typeof(TIdParameter) == typeof(MailboxOrMailUserIdParameter)))
				{
					throw new ArgumentException(string.Format("type not supported {0}", typeof(TIdParameter).Name), "TIdParameter");
				}
				if (!string.IsNullOrEmpty(text))
				{
					identityParameter = new MailboxOrMailUserIdParameter(text);
				}
				else
				{
					identityParameter = new MailboxOrMailUserIdParameter(this.UserMailboxADObjectId);
				}
			}
			return (TIdParameter)((object)identityParameter);
		}

		public void Update(string identifier, OrganizationId organizationId)
		{
			this.OrganizationId = organizationId;
		}

		public override string ToString()
		{
			return string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				this.UserMailboxId,
				this.UserMailboxDatabaseId,
				this.MailboxServer,
				this.Name,
				this.RecipientType
			});
		}

		public bool ReadFromMessageItem(IMigrationStoreObject message)
		{
			this.MailboxLegacyDN = message.GetValueOrDefault<string>(MigrationBatchMessageSchema.MigrationJobItemMailboxLegacyDN, null);
			if (string.IsNullOrEmpty(this.MailboxLegacyDN))
			{
				return false;
			}
			this.MailboxServer = MigrationHelper.GetFqdnProperty(message, MigrationBatchMessageSchema.MigrationJobItemMailboxServer, false);
			this.UserMailboxId = MigrationHelper.GetGuidProperty(message, MigrationBatchMessageSchema.MigrationJobItemMailboxId, false);
			this.UserMailboxDatabaseId = MigrationHelper.GetGuidProperty(message, MigrationBatchMessageSchema.MigrationJobItemMailboxDatabaseId, false);
			byte[] valueOrDefault = message.GetValueOrDefault<byte[]>(MigrationBatchMessageSchema.MigrationJobItemOwnerId, null);
			if (valueOrDefault != null)
			{
				this.UserMailboxADObjectId = new ADObjectId(valueOrDefault);
			}
			this.ExchangeObjectId = MigrationHelper.GetGuidProperty(message, MigrationBatchMessageSchema.MigrationExchangeObjectId, false);
			return true;
		}

		public void WriteToMessageItem(IMigrationStoreObject message, bool loaded)
		{
			if (this.MailboxServer != null)
			{
				message[MigrationBatchMessageSchema.MigrationJobItemMailboxServer] = this.MailboxServer.Domain;
				message[MigrationBatchMessageSchema.MigrationJobItemMailboxDatabaseId] = this.UserMailboxDatabaseId;
			}
			message[MigrationBatchMessageSchema.MigrationJobItemMailboxId] = this.UserMailboxId;
			message[MigrationBatchMessageSchema.MigrationJobItemOwnerId] = this.UserMailboxADObjectId.GetBytes();
			message[MigrationBatchMessageSchema.MigrationJobItemMailboxLegacyDN] = this.MailboxLegacyDN;
			message[MigrationBatchMessageSchema.MigrationExchangeObjectId] = this.ExchangeObjectId;
		}

		public XElement GetDiagnosticInfo(IMigrationDataProvider dataProvider, MigrationDiagnosticArgument argument)
		{
			return new XElement("MailboxData", new object[]
			{
				new XElement("mailboxServer", this.MailboxServer),
				new XElement("mailboxId", this.UserMailboxId),
				new XElement("mailboxOwner", this.UserMailboxADObjectId),
				new XElement("mailboxExchangeObjectId", this.ExchangeObjectId),
				new XElement("userMailboxDatabaseId", this.UserMailboxDatabaseId),
				new XElement("mailboxName", this.MailboxLegacyDN)
			});
		}

		internal static string CreateMailboxIdentifierString(OrganizationId organizationId, Guid exchangeObjectId)
		{
			if (organizationId != null && organizationId != OrganizationId.ForestWideOrgId)
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}\\{1}", new object[]
				{
					organizationId.OrganizationalUnit.Name,
					exchangeObjectId
				});
			}
			return exchangeObjectId.ToString();
		}

		public static readonly PropertyDefinition[] MailboxDataPropertyDefinition = new StorePropertyDefinition[]
		{
			MigrationBatchMessageSchema.MigrationJobItemMailboxServer,
			MigrationBatchMessageSchema.MigrationJobItemMailboxId,
			MigrationBatchMessageSchema.MigrationJobItemMailboxDatabaseId,
			MigrationBatchMessageSchema.MigrationJobItemMailboxLegacyDN,
			MigrationBatchMessageSchema.MigrationJobItemOwnerId,
			MigrationBatchMessageSchema.MigrationExchangeObjectId
		};

		private MigrationUserRecipientType? recipientType;
	}
}
