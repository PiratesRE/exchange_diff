using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotBypassDomainNameValidationException : LocalizedException
	{
		public CannotBypassDomainNameValidationException() : base(Strings.CannotBypassDomainNameValidation)
		{
		}

		public CannotBypassDomainNameValidationException(Exception innerException) : base(Strings.CannotBypassDomainNameValidation, innerException)
		{
		}

		protected CannotBypassDomainNameValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
