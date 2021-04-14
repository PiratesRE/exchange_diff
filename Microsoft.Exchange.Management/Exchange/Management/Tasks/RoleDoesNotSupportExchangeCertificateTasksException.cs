using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RoleDoesNotSupportExchangeCertificateTasksException : LocalizedException
	{
		public RoleDoesNotSupportExchangeCertificateTasksException() : base(Strings.RoleDoesNotSupportExchangeCertificateTasksException)
		{
		}

		public RoleDoesNotSupportExchangeCertificateTasksException(Exception innerException) : base(Strings.RoleDoesNotSupportExchangeCertificateTasksException, innerException)
		{
		}

		protected RoleDoesNotSupportExchangeCertificateTasksException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
