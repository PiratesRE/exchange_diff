using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MoveInProgressReservationException : ResourceReservationException
	{
		public MoveInProgressReservationException(string resourceName, string clientName) : base(MrsStrings.ErrorMoveInProgress(resourceName, clientName))
		{
			this.resourceName = resourceName;
			this.clientName = clientName;
		}

		public MoveInProgressReservationException(string resourceName, string clientName, Exception innerException) : base(MrsStrings.ErrorMoveInProgress(resourceName, clientName), innerException)
		{
			this.resourceName = resourceName;
			this.clientName = clientName;
		}

		protected MoveInProgressReservationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.resourceName = (string)info.GetValue("resourceName", typeof(string));
			this.clientName = (string)info.GetValue("clientName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("resourceName", this.resourceName);
			info.AddValue("clientName", this.clientName);
		}

		public string ResourceName
		{
			get
			{
				return this.resourceName;
			}
		}

		public string ClientName
		{
			get
			{
				return this.clientName;
			}
		}

		private readonly string resourceName;

		private readonly string clientName;
	}
}
