using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Authorization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class LowVersionUserDeniedException : AuthorizationException
	{
		public LowVersionUserDeniedException() : base(Strings.LowVersionUser)
		{
		}

		public LowVersionUserDeniedException(Exception innerException) : base(Strings.LowVersionUser, innerException)
		{
		}

		protected LowVersionUserDeniedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
