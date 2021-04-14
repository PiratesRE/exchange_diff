using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class MessageCategoryIdParameter : IIdentityParameter
	{
		internal MessageCategoryId InternalMessageCategoryId
		{
			get
			{
				return this.internalMessageCategoryId;
			}
			set
			{
				this.internalMessageCategoryId = value;
			}
		}

		internal string RawCategoryName
		{
			get
			{
				return this.rawCategoryName;
			}
		}

		internal Guid? RawCategoryId
		{
			get
			{
				return this.rawCategoryId;
			}
		}

		internal MailboxIdParameter RawMailbox
		{
			get
			{
				return this.rawMailbox;
			}
		}

		string IIdentityParameter.RawIdentity
		{
			get
			{
				if (this.internalMessageCategoryId != null)
				{
					return this.internalMessageCategoryId.ToString();
				}
				return this.rawInput;
			}
		}

		public MessageCategoryIdParameter()
		{
		}

		public MessageCategoryIdParameter(MessageCategory category)
		{
			if (category == null)
			{
				throw new ArgumentNullException("MessageCategory");
			}
			((IIdentityParameter)this).Initialize(category.Identity);
			this.rawInput = category.Identity.ToString();
			this.rawCategoryName = category.Name;
		}

		public MessageCategoryIdParameter(INamedIdentity namedIdentity) : this(namedIdentity.Identity)
		{
		}

		public MessageCategoryIdParameter(MessageCategoryId messageCategoryId)
		{
			if (null == messageCategoryId)
			{
				throw new ArgumentNullException("messageCategoryId");
			}
			((IIdentityParameter)this).Initialize(messageCategoryId);
			this.rawInput = messageCategoryId.ToString();
		}

		public MessageCategoryIdParameter(Mailbox mailbox) : this(mailbox.Id)
		{
		}

		public MessageCategoryIdParameter(ADObjectId mailboxOwnerId)
		{
			if (mailboxOwnerId == null)
			{
				throw new ArgumentNullException("mailboxOwnerId");
			}
			((IIdentityParameter)this).Initialize(new MessageCategoryId(mailboxOwnerId, null, null));
			this.rawInput = mailboxOwnerId.ToString();
		}

		public MessageCategoryIdParameter(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				throw new ArgumentNullException("input");
			}
			this.rawInput = input;
			int num = input.IndexOf('\\');
			string text;
			string text2;
			if (num > 1)
			{
				text = input.Substring(0, num);
				text2 = input.Substring(1 + num);
			}
			else
			{
				text2 = input;
				text = null;
			}
			if (!string.IsNullOrEmpty(text))
			{
				this.rawMailbox = new MailboxIdParameter(text);
			}
			this.rawCategoryName = null;
			this.rawCategoryId = null;
			if (!string.IsNullOrEmpty(text2))
			{
				Guid value;
				if (GuidHelper.TryParseGuid(text2, out value))
				{
					this.rawCategoryId = new Guid?(value);
					return;
				}
				this.rawCategoryName = text2;
			}
		}

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
			if (null == this.InternalMessageCategoryId)
			{
				throw new InvalidOperationException(Strings.ErrorOperationOnInvalidObject);
			}
			MessageCategoryDataProvider messageCategoryDataProvider = session as MessageCategoryDataProvider;
			if (messageCategoryDataProvider == null)
			{
				throw new NotSupportedException("DataProvider: " + session.GetType().FullName);
			}
			IConfigurable[] array = messageCategoryDataProvider.Find<T>(null, this.InternalMessageCategoryId, false, null);
			if (array == null || array.Length == 0)
			{
				notFoundReason = new LocalizedString?(Strings.ErrorManagementObjectNotFound(this.ToString()));
				return new T[0];
			}
			notFoundReason = null;
			T[] array2 = new T[array.Length];
			int num = 0;
			foreach (IConfigurable configurable in array)
			{
				array2[num++] = (T)((object)configurable);
			}
			return array2;
		}

		void IIdentityParameter.Initialize(ObjectId objectId)
		{
			if (objectId == null)
			{
				throw new ArgumentNullException("objectId");
			}
			if (!(objectId is MessageCategoryId))
			{
				throw new NotSupportedException("objectId: " + objectId.GetType().FullName);
			}
			this.internalMessageCategoryId = (MessageCategoryId)objectId;
		}

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(this.rawCategoryName))
			{
				return this.rawCategoryName;
			}
			if (this.internalMessageCategoryId != null)
			{
				return this.internalMessageCategoryId.ToString();
			}
			return this.rawInput;
		}

		private MailboxIdParameter rawMailbox;

		private Guid? rawCategoryId;

		private string rawCategoryName;

		private string rawInput;

		private MessageCategoryId internalMessageCategoryId;
	}
}
