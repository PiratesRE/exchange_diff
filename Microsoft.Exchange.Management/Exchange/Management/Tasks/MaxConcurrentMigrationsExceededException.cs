using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MaxConcurrentMigrationsExceededException : MailboxReplicationTransientException
	{
		public MaxConcurrentMigrationsExceededException(int currentMax) : base(Strings.ErrorMaxConcurrentMigrationsExceeded(currentMax))
		{
			this.currentMax = currentMax;
		}

		public MaxConcurrentMigrationsExceededException(int currentMax, Exception innerException) : base(Strings.ErrorMaxConcurrentMigrationsExceeded(currentMax), innerException)
		{
			this.currentMax = currentMax;
		}

		protected MaxConcurrentMigrationsExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.currentMax = (int)info.GetValue("currentMax", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("currentMax", this.currentMax);
		}

		public int CurrentMax
		{
			get
			{
				return this.currentMax;
			}
		}

		private readonly int currentMax;
	}
}
