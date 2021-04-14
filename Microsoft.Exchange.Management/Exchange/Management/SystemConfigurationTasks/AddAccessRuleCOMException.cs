using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AddAccessRuleCOMException : LocalizedException
	{
		public AddAccessRuleCOMException(string thumbprint) : base(Strings.AddAccessRuleCOM(thumbprint))
		{
			this.thumbprint = thumbprint;
		}

		public AddAccessRuleCOMException(string thumbprint, Exception innerException) : base(Strings.AddAccessRuleCOM(thumbprint), innerException)
		{
			this.thumbprint = thumbprint;
		}

		protected AddAccessRuleCOMException(SerializationInfo info, StreamingContext context) : base(info, context)
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
