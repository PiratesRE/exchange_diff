using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TargetDeliveryDomainMismatchPermanentException : MailboxReplicationPermanentException
	{
		public TargetDeliveryDomainMismatchPermanentException(string targetDeliveryDomain) : base(MrsStrings.ErrorTargetDeliveryDomainMismatch(targetDeliveryDomain))
		{
			this.targetDeliveryDomain = targetDeliveryDomain;
		}

		public TargetDeliveryDomainMismatchPermanentException(string targetDeliveryDomain, Exception innerException) : base(MrsStrings.ErrorTargetDeliveryDomainMismatch(targetDeliveryDomain), innerException)
		{
			this.targetDeliveryDomain = targetDeliveryDomain;
		}

		protected TargetDeliveryDomainMismatchPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.targetDeliveryDomain = (string)info.GetValue("targetDeliveryDomain", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("targetDeliveryDomain", this.targetDeliveryDomain);
		}

		public string TargetDeliveryDomain
		{
			get
			{
				return this.targetDeliveryDomain;
			}
		}

		private readonly string targetDeliveryDomain;
	}
}
