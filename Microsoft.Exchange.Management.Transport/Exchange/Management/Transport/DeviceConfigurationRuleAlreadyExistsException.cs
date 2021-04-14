using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class DeviceConfigurationRuleAlreadyExistsException : LocalizedException
	{
		public DeviceConfigurationRuleAlreadyExistsException(string ruleName) : base(Strings.DeviceConfigurationRuleAlreadyExists(ruleName))
		{
			this.ruleName = ruleName;
		}

		public DeviceConfigurationRuleAlreadyExistsException(string ruleName, Exception innerException) : base(Strings.DeviceConfigurationRuleAlreadyExists(ruleName), innerException)
		{
			this.ruleName = ruleName;
		}

		protected DeviceConfigurationRuleAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
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
