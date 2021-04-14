using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.ClientAccessRules
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ClientAccessRulesBlockedConnectionException : LocalizedException
	{
		public ClientAccessRulesBlockedConnectionException(string ruleName) : base(DataStrings.ClientAccessRulesBlockedConnection(ruleName))
		{
			this.ruleName = ruleName;
		}

		public ClientAccessRulesBlockedConnectionException(string ruleName, Exception innerException) : base(DataStrings.ClientAccessRulesBlockedConnection(ruleName), innerException)
		{
			this.ruleName = ruleName;
		}

		protected ClientAccessRulesBlockedConnectionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.ruleName = (string)info.GetValue("ruleName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("ruleName", this.ruleName);
		}

		public string RuleName
		{
			get
			{
				return this.ruleName;
			}
		}

		private readonly string ruleName;
	}
}
