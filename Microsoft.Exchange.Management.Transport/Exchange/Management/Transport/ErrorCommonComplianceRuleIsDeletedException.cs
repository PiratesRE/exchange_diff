using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Transport
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ErrorCommonComplianceRuleIsDeletedException : LocalizedException
	{
		public ErrorCommonComplianceRuleIsDeletedException(string name) : base(Strings.ErrorCommonComplianceRuleIsDeleted(name))
		{
			this.name = name;
		}

		public ErrorCommonComplianceRuleIsDeletedException(string name, Exception innerException) : base(Strings.ErrorCommonComplianceRuleIsDeleted(name), innerException)
		{
			this.name = name;
		}

		protected ErrorCommonComplianceRuleIsDeletedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.name = (string)info.GetValue("name", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("name", this.name);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		private readonly string name;
	}
}
