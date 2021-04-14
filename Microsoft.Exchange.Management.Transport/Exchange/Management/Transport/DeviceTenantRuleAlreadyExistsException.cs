using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class DeviceTenantRuleAlreadyExistsException : LocalizedException
	{
		public DeviceTenantRuleAlreadyExistsException(string ruleName) : base(Strings.DeviceTenantRuleAlreadyExists(ruleName))
		{
			this.ruleName = ruleName;
		}

		public DeviceTenantRuleAlreadyExistsException(string ruleName, Exception innerException) : base(Strings.DeviceTenantRuleAlreadyExists(ruleName), innerException)
		{
			this.ruleName = ruleName;
		}

		protected DeviceTenantRuleAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
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
