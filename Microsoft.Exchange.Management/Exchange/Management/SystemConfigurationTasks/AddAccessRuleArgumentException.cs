using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AddAccessRuleArgumentException : LocalizedException
	{
		public AddAccessRuleArgumentException(string thumbprint) : base(Strings.AddAccessRuleArgument(thumbprint))
		{
			this.thumbprint = thumbprint;
		}

		public AddAccessRuleArgumentException(string thumbprint, Exception innerException) : base(Strings.AddAccessRuleArgument(thumbprint), innerException)
		{
			this.thumbprint = thumbprint;
		}

		protected AddAccessRuleArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.thumbprint = (string)info.GetValue("thumbprint", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("thumbprint", this.thumbprint);
		}

		public string Thumbprint
		{
			get
			{
				return this.thumbprint;
			}
		}

		private readonly string thumbprint;
	}
}
