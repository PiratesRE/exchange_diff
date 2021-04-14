using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class MailboxStoreObjectIdParameter : IIdentityParameter
	{
		internal MailboxIdParameter RawOwner { get; private set; }

		internal StoreObjectId RawStoreObjectId { get; private set; }

		internal MailboxStoreObjectId InternalStoreObjectId { get; set; }

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session)
		{
			LocalizedString? localizedString;
			return ((IIdentityParameter)this).GetObjects<T>(rootId, session, null, out localizedString);
		}

		IEnumerable<T> IIdentityParameter.GetObjects<T>(ObjectId rootId, IConfigDataProvider session, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (null == this.InternalStoreObjectId)
			{
				throw new InvalidOperationException(Strings.ErrorOperationOnInvalidObject);
			}
			if (!(session is MailMessageDataProvider))
			{
				throw new NotSupportedException("session: " + session.GetType().FullName);
			}
			if (optionalData != null && optionalData.AdditionalFilter != null)
			{
				throw new NotSupportedException("Supplying Additional Filters is not currently supported by this IdParameter.");
			}
			T t = (T)((object)session.Read<T>(this.InternalStoreObjectId));
			if (t == null)
			{
				notFoundReason = new LocalizedString?(Strings.ErrorManagementObjectNotFound(this.ToString()));
				return new T[0];
			}
			notFoundReason = null;
			return new T[]
			{
				t
			};
		}

		void IIdentityParameter.Initialize(ObjectId objectId)
		{
			if (objectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			MailboxStoreObjectId mailboxStoreObjectId = objectId as MailboxStoreObjectId;
			if (null == mailboxStoreObjectId)
			{
				throw new NotSupportedException("objectId: " + objectId.GetType().FullName);
			}
			this.RawOwner = new MailboxIdParameter(mailboxStoreObjectId.MailboxOwnerId);
			this.RawStoreObjectId = mailboxStoreObjectId.StoreObjectId;
		}

		string IIdentityParameter.RawIdentity
		{
			get
			{
				return this.rawIdentity;
			}
		}

		public MailboxStoreObjectIdParameter()
		{
		}

		public MailboxStoreObjectIdParameter(XsoMailboxConfigurationObject storeObject)
		{
			if (storeObject == null)
			{
				throw new ArgumentNullException("storeObject");
			}
			ObjectId identity = storeObject.Identity;
			if (identity == null)
			{
				throw new ArgumentNullException("storeObject.Identity");
			}
			this.rawIdentity = identity.ToString();
			((IIdentityParameter)this).Initialize(identity);
		}

		public MailboxStoreObjectIdParameter(MailboxStoreObjectId storeObjectId)
		{
			if (null == storeObjectId)
			{
				throw new ArgumentNullException("storeObjectId");
			}
			this.rawIdentity = storeObjectId.ToString();
			((IIdentityParameter)this).Initialize(storeObjectId);
		}

		public MailboxStoreObjectIdParameter(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentNullException("mailMessageId");
			}
			this.rawIdentity = id;
			int num = id.LastIndexOf('\\');
			string text;
			if (-1 == num)
			{
				text = id;
			}
			else
			{
				string text2 = id.Substring(0, num);
				text = id.Substring(1 + num);
				if (!string.IsNullOrEmpty(text2))
				{
					this.RawOwner = new MailboxIdParameter(text2);
				}
				if (string.IsNullOrEmpty(text))
				{
					throw new FormatException(Strings.ErrorInvalidMailboxStoreObjectIdentity(this.rawIdentity));
				}
			}
			try
			{
				this.RawStoreObjectId = StoreObjectId.Deserialize(text);
			}
			catch (FormatException innerException)
			{
				throw new FormatException(Strings.ErrorInvalidMailboxStoreObjectIdentity(this.rawIdentity), innerException);
			}
			catch (CorruptDataException innerException2)
			{
				throw new FormatException(Strings.ErrorInvalidMailboxStoreObjectIdentity(this.rawIdentity), innerException2);
			}
		}

		public MailboxStoreObjectIdParameter(INamedIdentity namedIdentity) : this(namedIdentity.Identity)
		{
			this.rawIdentity = namedIdentity.DisplayName;
		}

		public MailboxStoreObjectIdParameter(Mailbox mailbox) : this(mailbox.Id)
		{
		}

		public MailboxStoreObjectIdParameter(ADObjectId mailboxOwnerId)
		{
			if (mailboxOwnerId == null)
			{
				throw new ArgumentNullException("mailboxOwnerId");
			}
			this.rawIdentity = mailboxOwnerId.ToString();
			((IIdentityParameter)this).Initialize(new MailboxStoreObjectId(mailboxOwnerId, null));
		}

		public override string ToString()
		{
			return this.rawIdentity;
		}

		private string rawIdentity;
	}
}
