using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotRemoveDefaultRemoteDomainException : LocalizedException
	{
		public CannotRemoveDefaultRemoteDomainException() : base(Strings.CannotRemoveDefaultRemoteDomain)
		{
		}

		public CannotRemoveDefaultRemoteDomainException(Exception innerException) : base(Strings.CannotRemoveDefaultRemoteDomain, innerException)
		{
		}

		protected CannotRemoveDefaultRemoteDomainException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
