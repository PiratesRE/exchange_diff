using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class DeviceConditionalAccessRuleAlreadyExistsException : LocalizedException
	{
		public DeviceConditionalAccessRuleAlreadyExistsException(string ruleName) : base(Strings.DeviceConditionalAccessRuleAlreadyExists(ruleName))
		{
			this.ruleName = ruleName;
		}

		public DeviceConditionalAccessRuleAlreadyExistsException(string ruleName, Exception innerException) : base(Strings.DeviceConditionalAccessRuleAlreadyExists(ruleName), innerException)
		{
			this.ruleName = ruleName;
		}

		protected DeviceConditionalAccessRuleAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
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
