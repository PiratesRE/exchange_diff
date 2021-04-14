using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class JobIsPoisonedPermananentException : MailboxReplicationPermanentException
	{
		public JobIsPoisonedPermananentException(int poisonCount) : base(MrsStrings.JobIsPoisoned(poisonCount))
		{
			this.poisonCount = poisonCount;
		}

		public JobIsPoisonedPermananentException(int poisonCount, Exception innerException) : base(MrsStrings.JobIsPoisoned(poisonCount), innerException)
		{
			this.poisonCount = poisonCount;
		}

		protected JobIsPoisonedPermananentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.poisonCount = (int)info.GetValue("poisonCount", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("poisonCount", this.poisonCount);
		}

		public int PoisonCount
		{
			get
			{
				return this.poisonCount;
			}
		}

		private readonly int poisonCount;
	}
}
