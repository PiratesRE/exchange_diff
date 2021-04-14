using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SeedingChannelIsClosedException : LocalizedException
	{
		public SeedingChannelIsClosedException(Guid g) : base(ReplayStrings.SeedingChannelIsClosedException(g))
		{
			this.g = g;
		}

		public SeedingChannelIsClosedException(Guid g, Exception innerException) : base(ReplayStrings.SeedingChannelIsClosedException(g), innerException)
		{
			this.g = g;
		}

		protected SeedingChannelIsClosedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.g = (Guid)info.GetValue("g", typeof(Guid));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("g", this.g);
		}

		public Guid G
		{
			get
			{
				return this.g;
			}
		}

		private readonly Guid g;
	}
}
