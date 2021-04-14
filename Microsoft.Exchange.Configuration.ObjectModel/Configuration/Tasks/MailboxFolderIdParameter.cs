using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class MailboxFolderIdParameter : IIdentityParameter
	{
		internal MailboxIdParameter RawOwner { get; private set; }

		internal MapiFolderPath RawFolderPath { get; private set; }

		internal StoreObjectId RawFolderStoreId { get; private set; }

		internal MailboxFolderId InternalMailboxFolderId { get; set; }

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
			if (null == this.InternalMailboxFolderId)
			{
				throw new InvalidOperationException(Strings.ErrorOperationOnInvalidObject);
			}
			if (!(session is MailboxFolderDataProviderBase))
			{
				throw new NotSupportedException("session: " + session.GetType().FullName);
			}
			if (optionalData != null && optionalData.AdditionalFilter != null)
			{
				throw new NotSupportedException("Supplying Additional Filters is not currently supported by this IdParameter.");
			}
			T t = (T)((object)session.Read<T>(this.InternalMailboxFolderId));
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
			if (!(objectId is MailboxFolderId))
			{
				throw new NotSupportedException("objectId: " + objectId.GetType().FullName);
			}
			this.InternalMailboxFolderId = (MailboxFolderId)objectId;
			this.RawOwner = new MailboxIdParameter(this.InternalMailboxFolderId.MailboxOwnerId);
			this.RawFolderPath = this.InternalMailboxFolderId.MailboxFolderPath;
			this.RawFolderStoreId = this.InternalMailboxFolderId.StoreObjectIdValue;
		}

		string IIdentityParameter.RawIdentity
		{
			get
			{
				return this.rawIdentity;
			}
		}

		public MailboxFolderIdParameter()
		{
		}

		public MailboxFolderIdParameter(MailboxFolder mailboxFolder)
		{
			if (mailboxFolder == null)
			{
				throw new ArgumentNullException("mailboxFolder");
			}
			ObjectId identity = mailboxFolder.Identity;
			if (identity == null)
			{
				throw new ArgumentNullException("mailboxFolder.Identity");
			}
			this.rawIdentity = identity.ToString();
			((IIdentityParameter)this).Initialize(identity);
		}

		public MailboxFolderIdParameter(PublicFolderId publicFolderId) : this(publicFolderId.ToString())
		{
		}

		public MailboxFolderIdParameter(MailboxFolderId mailboxFolderId)
		{
			if (null == mailboxFolderId)
			{
				throw new ArgumentNullException("mailboxFolderId");
			}
			this.rawIdentity = mailboxFolderId.ToString();
			((IIdentityParameter)this).Initialize(mailboxFolderId);
		}

		public MailboxFolderIdParameter(string mailboxFolderId)
		{
			if (string.IsNullOrEmpty(mailboxFolderId))
			{
				throw new ArgumentNullException("mailboxFolderId");
			}
			this.rawIdentity = mailboxFolderId;
			int num = mailboxFolderId.IndexOf(':');
			if (-1 == num)
			{
				this.RawOwner = new MailboxIdParameter(mailboxFolderId);
				this.RawFolderPath = MapiFolderPath.IpmSubtreeRoot;
				return;
			}
			string text = mailboxFolderId.Substring(0, num);
			string text2 = mailboxFolderId.Substring(1 + num);
			if (!string.IsNullOrEmpty(text))
			{
				this.RawOwner = new MailboxIdParameter(text);
			}
			if (string.IsNullOrEmpty(text2))
			{
				throw new FormatException(Strings.ErrorInvalidMailboxFolderIdentity(this.rawIdentity));
			}
			try
			{
				if (text2[0] == '\\' || text2[0] == '￾')
				{
					this.RawFolderPath = MapiFolderPath.Parse(text2);
				}
				else
				{
					this.RawFolderStoreId = StoreObjectId.Deserialize(text2);
				}
			}
			catch (FormatException innerException)
			{
				throw new FormatException(Strings.ErrorInvalidMailboxFolderIdentity(this.rawIdentity), innerException);
			}
			catch (CorruptDataException innerException2)
			{
				throw new FormatException(Strings.ErrorInvalidMailboxFolderIdentity(this.rawIdentity), innerException2);
			}
		}

		public MailboxFolderIdParameter(INamedIdentity namedIdentity) : this(namedIdentity.Identity)
		{
			this.rawIdentity = namedIdentity.DisplayName;
		}

		public MailboxFolderIdParameter(Mailbox mailbox) : this(mailbox.Id)
		{
		}

		public MailboxFolderIdParameter(ADObjectId mailboxOwnerId)
		{
			if (mailboxOwnerId == null)
			{
				throw new ArgumentNullException("mailboxOwnerId");
			}
			this.rawIdentity = mailboxOwnerId.ToString();
			((IIdentityParameter)this).Initialize(new MailboxFolderId(mailboxOwnerId, null, MapiFolderPath.IpmSubtreeRoot));
		}

		internal MailboxFolderIdParameter(PublicFolderIdParameter publicFolderIdParameter, ADUser publicFolderHierarchyMailbox)
		{
			if (publicFolderIdParameter == null)
			{
				throw new ArgumentNullException("mailboxFolderId");
			}
			this.rawIdentity = publicFolderIdParameter.ToString();
			this.RawFolderPath = publicFolderIdParameter.PublicFolderId.MapiFolderPath;
			this.RawFolderStoreId = publicFolderIdParameter.PublicFolderId.StoreObjectId;
			this.RawOwner = new MailboxIdParameter(publicFolderHierarchyMailbox.ObjectId);
		}

		public override string ToString()
		{
			return this.rawIdentity;
		}

		private string rawIdentity;
	}
}
