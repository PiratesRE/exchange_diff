using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotModifyPhotoBecauseMailboxIsInTransitException : LocalizedException
	{
		public CannotModifyPhotoBecauseMailboxIsInTransitException() : base(Strings.CannotModifyPhotoBecauseMailboxIsInTransit)
		{
		}

		public CannotModifyPhotoBecauseMailboxIsInTransitException(Exception innerException) : base(Strings.CannotModifyPhotoBecauseMailboxIsInTransit, innerException)
		{
		}

		protected CannotModifyPhotoBecauseMailboxIsInTransitException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
