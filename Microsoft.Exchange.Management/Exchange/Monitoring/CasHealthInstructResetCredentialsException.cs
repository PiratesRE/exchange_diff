using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CasHealthInstructResetCredentialsException : LocalizedException
	{
		public CasHealthInstructResetCredentialsException() : base(Strings.CasHealthInstructResetCredentials)
		{
		}

		public CasHealthInstructResetCredentialsException(Exception innerException) : base(Strings.CasHealthInstructResetCredentials, innerException)
		{
		}

		protected CasHealthInstructResetCredentialsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
