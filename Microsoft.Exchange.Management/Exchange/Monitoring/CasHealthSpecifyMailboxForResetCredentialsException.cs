using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CasHealthSpecifyMailboxForResetCredentialsException : LocalizedException
	{
		public CasHealthSpecifyMailboxForResetCredentialsException() : base(Strings.CasHealthSpecifyMailboxForResetCredentials)
		{
		}

		public CasHealthSpecifyMailboxForResetCredentialsException(Exception innerException) : base(Strings.CasHealthSpecifyMailboxForResetCredentials, innerException)
		{
		}

		protected CasHealthSpecifyMailboxForResetCredentialsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
