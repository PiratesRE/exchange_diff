using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayServiceTooManyHandlesException : TransientException
	{
		public ReplayServiceTooManyHandlesException(long numberOfHandles, long maxNumberOfHandles) : base(ReplayStrings.ReplayServiceTooManyHandlesException(numberOfHandles, maxNumberOfHandles))
		{
			this.numberOfHandles = numberOfHandles;
			this.maxNumberOfHandles = maxNumberOfHandles;
		}

		public ReplayServiceTooManyHandlesException(long numberOfHandles, long maxNumberOfHandles, Exception innerException) : base(ReplayStrings.ReplayServiceTooManyHandlesException(numberOfHandles, maxNumberOfHandles), innerException)
		{
			this.numberOfHandles = numberOfHandles;
			this.maxNumberOfHandles = maxNumberOfHandles;
		}

		protected ReplayServiceTooManyHandlesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.numberOfHandles = (long)info.GetValue("numberOfHandles", typeof(long));
			this.maxNumberOfHandles = (long)info.GetValue("maxNumberOfHandles", typeof(long));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("numberOfHandles", this.numberOfHandles);
			info.AddValue("maxNumberOfHandles", this.maxNumberOfHandles);
		}

		public long NumberOfHandles
		{
			get
			{
				return this.numberOfHandles;
			}
		}

		public long MaxNumberOfHandles
		{
			get
			{
				return this.maxNumberOfHandles;
			}
		}

		private readonly long numberOfHandles;

		private readonly long maxNumberOfHandles;
	}
}
