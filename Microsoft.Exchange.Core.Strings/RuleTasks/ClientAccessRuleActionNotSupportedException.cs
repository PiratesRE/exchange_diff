using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Core.RuleTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ClientAccessRuleActionNotSupportedException : LocalizedException
	{
		public ClientAccessRuleActionNotSupportedException(string action) : base(RulesTasksStrings.ClientAccessRuleActionNotSupported(action))
		{
			this.action = action;
		}

		public ClientAccessRuleActionNotSupportedException(string action, Exception innerException) : base(RulesTasksStrings.ClientAccessRuleActionNotSupported(action), innerException)
		{
			this.action = action;
		}

		protected ClientAccessRuleActionNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.action = (string)info.GetValue("action", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("action", this.action);
		}

		public string Action
		{
			get
			{
				return this.action;
			}
		}

		private readonly string action;
	}
}
