using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RelinquishJobResourceReservationTransientException : RelinquishJobTransientException
	{
		public RelinquishJobResourceReservationTransientException(LocalizedString error) : base(MrsStrings.JobHasBeenRelinquishedDueToResourceReservation(error))
		{
			this.error = error;
		}

		public RelinquishJobResourceReservationTransientException(LocalizedString error, Exception innerException) : base(MrsStrings.JobHasBeenRelinquishedDueToResourceReservation(error), innerException)
		{
			this.error = error;
		}

		protected RelinquishJobResourceReservationTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.error = (LocalizedString)info.GetValue("error", typeof(LocalizedString));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("error", this.error);
		}

		public LocalizedString Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly LocalizedString error;
	}
}
