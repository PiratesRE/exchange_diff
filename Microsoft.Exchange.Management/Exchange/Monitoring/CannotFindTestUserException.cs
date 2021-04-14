using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotFindTestUserException : LocalizedException
	{
		public CannotFindTestUserException() : base(Strings.CannotFindTestUser)
		{
		}

		public CannotFindTestUserException(Exception innerException) : base(Strings.CannotFindTestUser, innerException)
		{
		}

		protected CannotFindTestUserException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
