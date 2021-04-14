using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class StartAfterOrCompleteAfterCannotBeSetWhenJobCompletingException : CannotSetCompletingPermanentException
	{
		public StartAfterOrCompleteAfterCannotBeSetWhenJobCompletingException() : base(Strings.StartAfterOrCompleteAfterCannotBeSetWhenJobCompleting)
		{
		}

		public StartAfterOrCompleteAfterCannotBeSetWhenJobCompletingException(Exception innerException) : base(Strings.StartAfterOrCompleteAfterCannotBeSetWhenJobCompleting, innerException)
		{
		}

		protected StartAfterOrCompleteAfterCannotBeSetWhenJobCompletingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
