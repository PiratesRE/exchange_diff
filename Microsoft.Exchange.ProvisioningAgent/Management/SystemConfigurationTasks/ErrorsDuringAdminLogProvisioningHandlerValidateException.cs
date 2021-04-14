using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.ProvisioningAgent;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ErrorsDuringAdminLogProvisioningHandlerValidateException : AdminAuditLogException
	{
		public ErrorsDuringAdminLogProvisioningHandlerValidateException(string error) : base(Strings.ErrorsDuringAdminLogProvisioningHandlerValidate(error))
		{
			this.error = error;
		}

		public ErrorsDuringAdminLogProvisioningHandlerValidateException(string error, Exception innerException) : base(Strings.ErrorsDuringAdminLogProvisioningHandlerValidate(error), innerException)
		{
			this.error = error;
		}

		protected ErrorsDuringAdminLogProvisioningHandlerValidateException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("error", this.error);
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string error;
	}
}
