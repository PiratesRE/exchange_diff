using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxNotJunkRuleCapableException : LocalizedException
	{
		public MailboxNotJunkRuleCapableException(string identity) : base(Strings.MailboxNotJunkRuleCapable(identity))
		{
			this.identity = identity;
		}

		public MailboxNotJunkRuleCapableException(string identity, Exception innerException) : base(Strings.MailboxNotJunkRuleCapable(identity), innerException)
		{
			this.identity = identity;
		}

		protected MailboxNotJunkRuleCapableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.identity = (string)info.GetValue("identity", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("identity", this.identity);
		}

		public string Identity
		{
			get
			{
				return this.identity;
			}
		}

		private readonly string identity;
	}
}
