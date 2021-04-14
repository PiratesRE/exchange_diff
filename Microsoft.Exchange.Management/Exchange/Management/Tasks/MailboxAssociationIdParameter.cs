using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Tasks
{
	[Serializable]
	public class MailboxAssociationIdParameter : IIdentityParameter
	{
		public MailboxIdParameter MailboxId { get; private set; }

		public string AssociationIdType { get; private set; }

		public string AssociationIdValue { get; private set; }

		string IIdentityParameter.RawIdentity
		{
			get
			{
				return this.rawIdentity;
			}
		}

		public MailboxAssociationIdParameter()
		{
		}

		public MailboxAssociationIdParameter(MailboxAssociationIdParameter mailboxAssociationId) : this(mailboxAssociationId.ToString())
		{
			this.rawIdentity = mailboxAssociationId.rawIdentity;
			this.MailboxId = mailboxAssociationId.MailboxId;
			this.AssociationIdType = mailboxAssociationId.AssociationIdType;
			this.AssociationIdValue = mailboxAssociationId.AssociationIdValue;
		}

		public MailboxAssociationIdParameter(INamedIdentity namedIdentity) : this(namedIdentity.Identity)
		{
			this.rawIdentity = namedIdentity.DisplayName;
		}

		public MailboxAssociationIdParameter(Mailbox mailbox) : this(mailbox.Id)
		{
		}

		public MailboxAssociationIdParameter(ADObjectId mailboxId)
		{
			ArgumentValidator.ThrowIfNull("mailboxId", mailboxId);
			this.rawIdentity = mailboxId.ToString();
			((IIdentityParameter)this).Initialize(new MailboxStoreObjectId(mailboxId, null));
		}

		public MailboxAssociationIdParameter(ConfigurableObject configurableObject)
		{
			ArgumentValidator.ThrowIfNull("configurableObject", configurableObject);
			this.rawIdentity = configurableObject.Identity.ToString();
			((IIdentityParameter)this).Initialize(configurableObject.Identity);
		}

		public MailboxAssociationIdParameter(MailboxStoreObjectId mailboxStoreObjectId) : this(mailboxStoreObjectId.ToString())
		{
		}

		public MailboxAssociationIdParameter(string mailboxAssociationId)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("mailboxAssociationId", mailboxAssociationId);
			this.rawIdentity = mailboxAssociationId;
			string[] array = mailboxAssociationId.Split(MailboxAssociationIdParameter.IdTokenizer, 3);
			if (array.Length == 2 || array.Length > 3)
			{
				throw new FormatException(Strings.ErrorInvalidMailboxAssociationIdentity(this.rawIdentity));
			}
			try
			{
				MailboxStoreObjectIdParameter mailboxStoreObjectIdParameter = new MailboxStoreObjectIdParameter(array[0]);
				if (mailboxStoreObjectIdParameter.RawOwner != null)
				{
					this.MailboxId = mailboxStoreObjectIdParameter.RawOwner;
					this.AssociationIdType = MailboxAssociationIdParameter.IdTypeItemId;
					this.AssociationIdValue = mailboxStoreObjectIdParameter.RawStoreObjectId.ToBase64String();
				}
				return;
			}
			catch (FormatException)
			{
			}
			this.MailboxId = new MailboxIdParameter(array[0]);
			this.AssociationIdType = null;
			this.AssociationIdValue = null;
			if (array.Length == 3)
			{
				if (!MailboxAssociationIdParameter.IsValidIdType(array[1]) || string.IsNullOrWhiteSpace(array[2]))
				{
					throw new FormatException(Strings.ErrorInvalidMailboxAssociationIdentity(this.rawIdentity));
				}
				this.AssociationIdType = array[1];
				this.AssociationIdValue = array[2];
			}
		}

		public override string ToString()
		{
			return this.rawIdentity;
		}

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session)
		{
			LocalizedString? localizedString;
			return ((IIdentityParameter)this).GetObjects<T>(rootId, session, null, out localizedString);
		}

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			return this.MailboxId.GetObjects<T>(rootId, session, optionalData, out notFoundReason);
		}

		void IIdentityParameter.Initialize(ObjectId objectId)
		{
			ArgumentValidator.ThrowIfNull("objectId", objectId);
			MailboxStoreObjectId mailboxStoreObjectId = objectId as MailboxStoreObjectId;
			if (mailboxStoreObjectId == null)
			{
				throw new NotSupportedException("objectId: " + objectId.GetType().FullName);
			}
			this.MailboxId = new MailboxIdParameter(mailboxStoreObjectId.MailboxOwnerId);
			if (mailboxStoreObjectId.StoreObjectId != null)
			{
				this.AssociationIdType = MailboxAssociationIdParameter.IdTypeItemId;
				this.AssociationIdValue = mailboxStoreObjectId.StoreObjectId.ToBase64String();
			}
		}

		private static bool IsValidIdType(string idType)
		{
			return MailboxAssociationIdParameter.IdTypeExternalId.Equals(idType, StringComparison.OrdinalIgnoreCase) || MailboxAssociationIdParameter.IdTypeLegacyDn.Equals(idType, StringComparison.OrdinalIgnoreCase) || MailboxAssociationIdParameter.IdTypeItemId.Equals(idType, StringComparison.OrdinalIgnoreCase);
		}

		public static readonly string IdTypeExternalId = "EOI";

		public static readonly string IdTypeLegacyDn = "LDN";

		public static readonly string IdTypeItemId = "IID";

		private static readonly char[] IdTokenizer = new char[]
		{
			':'
		};

		private readonly string rawIdentity;
	}
}
