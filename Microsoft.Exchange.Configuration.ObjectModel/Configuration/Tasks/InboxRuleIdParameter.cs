using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class InboxRuleIdParameter : IIdentityParameter
	{
		internal InboxRuleId InternalInboxRuleId
		{
			get
			{
				return this.internalInboxRuleId;
			}
			set
			{
				this.internalInboxRuleId = value;
			}
		}

		internal string RawRuleName
		{
			get
			{
				return this.rawRuleName;
			}
		}

		internal RuleId RawRuleId
		{
			get
			{
				return this.rawRuleId;
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
				return this.rawInput;
			}
		}

		public InboxRuleIdParameter()
		{
		}

		public InboxRuleIdParameter(ConfigurableObject configurableObject)
		{
			if (configurableObject == null)
			{
				throw new ArgumentNullException("configurableObject");
			}
			((IIdentityParameter)this).Initialize(configurableObject.Identity);
			this.rawInput = configurableObject.Identity.ToString();
		}

		public InboxRuleIdParameter(INamedIdentity namedIdentity) : this(namedIdentity.Identity)
		{
			this.rawRuleName = namedIdentity.DisplayName;
		}

		public InboxRuleIdParameter(InboxRuleId inboxRuleId)
		{
			if (null == inboxRuleId)
			{
				throw new ArgumentNullException("inboxRuleId");
			}
			((IIdentityParameter)this).Initialize(inboxRuleId);
			this.rawInput = inboxRuleId.ToString();
		}

		public InboxRuleIdParameter(Mailbox mailbox) : this(mailbox.Id)
		{
		}

		public InboxRuleIdParameter(ADObjectId mailboxOwnerId)
		{
			if (mailboxOwnerId == null)
			{
				throw new ArgumentNullException("mailboxOwnerId");
			}
			this.rawInput = mailboxOwnerId.ToString();
			((IIdentityParameter)this).Initialize(new InboxRuleId(mailboxOwnerId, null, null));
		}

		public InboxRuleIdParameter(string inboxRuleId)
		{
			if (string.IsNullOrEmpty(inboxRuleId))
			{
				throw new ArgumentNullException("inboxRuleId");
			}
			this.rawInput = inboxRuleId;
			int num = inboxRuleId.Length;
			int num2;
			do
			{
				num2 = num;
				num = inboxRuleId.LastIndexOf("\\\\", num2 - 1, num2);
			}
			while (num != -1);
			int num3 = inboxRuleId.LastIndexOf('\\', num2 - 1, num2);
			if (num3 == 0 || num3 == inboxRuleId.Length - 1)
			{
				throw new ArgumentException(Strings.ErrorInvalidParameterFormat("inboxRuleId"), "inboxRuleId");
			}
			string text;
			string text2;
			if (num3 > 0)
			{
				text = inboxRuleId.Substring(0, num3);
				text2 = inboxRuleId.Substring(1 + num3);
			}
			else
			{
				text2 = inboxRuleId;
				text = null;
			}
			if (num2 != inboxRuleId.Length)
			{
				text2 = text2.Replace("\\\\", '\\'.ToString());
			}
			if (!string.IsNullOrEmpty(text))
			{
				this.rawMailbox = new MailboxIdParameter(text);
			}
			ulong value;
			if (ulong.TryParse(text2, out value))
			{
				byte[] array = new byte[8];
				ExBitConverter.Write((long)value, array, 0);
				this.rawRuleId = RuleId.Deserialize(array, 0);
				return;
			}
			this.rawRuleName = text2;
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
			if (null == this.InternalInboxRuleId)
			{
				throw new InvalidOperationException(Strings.ErrorOperationOnInvalidObject);
			}
			IConfigurable[] array = session.Find<T>(null, this.InternalInboxRuleId, false, null);
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
			if (!(objectId is InboxRuleId))
			{
				throw new NotSupportedException("objectId: " + objectId.GetType().FullName);
			}
			this.internalInboxRuleId = (InboxRuleId)objectId;
		}

		public override string ToString()
		{
			if (!string.IsNullOrEmpty(this.rawRuleName))
			{
				return this.rawRuleName;
			}
			if (this.internalInboxRuleId != null)
			{
				return this.internalInboxRuleId.ToString();
			}
			return this.rawInput;
		}

		private MailboxIdParameter rawMailbox;

		private RuleId rawRuleId;

		private string rawRuleName;

		private string rawInput;

		private InboxRuleId internalInboxRuleId;
	}
}
