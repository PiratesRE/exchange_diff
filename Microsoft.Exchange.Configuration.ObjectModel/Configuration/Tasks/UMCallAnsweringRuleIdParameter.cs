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
	public class UMCallAnsweringRuleIdParameter : IIdentityParameter
	{
		internal Guid? RawRuleGuid { get; private set; }

		internal MailboxIdParameter RawMailbox { get; private set; }

		internal UMCallAnsweringRuleId CallAnsweringRuleId { get; set; }

		string IIdentityParameter.RawIdentity
		{
			get
			{
				return this.rawInput;
			}
		}

		public UMCallAnsweringRuleIdParameter()
		{
		}

		public UMCallAnsweringRuleIdParameter(INamedIdentity namedIdentity) : this(namedIdentity.Identity)
		{
		}

		public UMCallAnsweringRuleIdParameter(ConfigurableObject configurableObject)
		{
			if (configurableObject == null)
			{
				throw new ArgumentNullException("configurableObject");
			}
			((IIdentityParameter)this).Initialize(configurableObject.Identity);
			this.rawInput = configurableObject.Identity.ToString();
		}

		public UMCallAnsweringRuleIdParameter(UMCallAnsweringRuleId ruleId)
		{
			if (ruleId == null)
			{
				throw new ArgumentNullException("ruleId");
			}
			((IIdentityParameter)this).Initialize(ruleId);
			this.rawInput = ruleId.ToString();
		}

		public UMCallAnsweringRuleIdParameter(Mailbox mailbox) : this(mailbox.Id)
		{
		}

		public UMCallAnsweringRuleIdParameter(ADObjectId mailboxOwnerId)
		{
			if (mailboxOwnerId == null)
			{
				throw new ArgumentNullException("mailboxOwnerId");
			}
			this.rawInput = mailboxOwnerId.ToString();
			((IIdentityParameter)this).Initialize(new UMCallAnsweringRuleId(mailboxOwnerId, Guid.Empty));
		}

		public UMCallAnsweringRuleIdParameter(string callAnsweringRuleId)
		{
			if (string.IsNullOrEmpty(callAnsweringRuleId))
			{
				throw new ArgumentNullException("CallAnsweringRuleId");
			}
			this.rawInput = callAnsweringRuleId;
			string input = string.Empty;
			string text = string.Empty;
			int num = callAnsweringRuleId.LastIndexOf("\\");
			if (num == 0 || num == callAnsweringRuleId.Length - 1)
			{
				throw new ArgumentException(Strings.ErrorInvalidParameterFormat("CallAnsweringRuleId"), "CallAnsweringRuleId");
			}
			if (num > 0)
			{
				text = callAnsweringRuleId.Substring(0, num);
				input = callAnsweringRuleId.Substring(1 + num);
			}
			else
			{
				input = callAnsweringRuleId;
				text = null;
			}
			if (!string.IsNullOrEmpty(text))
			{
				this.RawMailbox = new MailboxIdParameter(text);
			}
			this.RawRuleGuid = new Guid?(Guid.Parse(input));
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
			if (null == this.CallAnsweringRuleId)
			{
				throw new InvalidOperationException(Strings.ErrorOperationOnInvalidObject);
			}
			IConfigurable[] array = session.Find<T>(null, this.CallAnsweringRuleId, false, null);
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
			this.CallAnsweringRuleId = (objectId as UMCallAnsweringRuleId);
			if (this.CallAnsweringRuleId == null)
			{
				throw new NotSupportedException("objectId: " + objectId.GetType().FullName);
			}
		}

		public override string ToString()
		{
			if (this.RawRuleGuid != null)
			{
				return this.RawRuleGuid.ToString();
			}
			if (this.CallAnsweringRuleId != null)
			{
				return this.CallAnsweringRuleId.ToString();
			}
			return this.rawInput;
		}

		private readonly string rawInput;
	}
}
