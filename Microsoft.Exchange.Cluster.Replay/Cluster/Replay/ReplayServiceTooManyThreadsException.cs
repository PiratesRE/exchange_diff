using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayServiceTooManyThreadsException : TransientException
	{
		public ReplayServiceTooManyThreadsException(long numberOfThreads, long maxNumberOfThreads) : base(ReplayStrings.ReplayServiceTooManyThreadsException(numberOfThreads, maxNumberOfThreads))
		{
			this.numberOfThreads = numberOfThreads;
			this.maxNumberOfThreads = maxNumberOfThreads;
		}

		public ReplayServiceTooManyThreadsException(long numberOfThreads, long maxNumberOfThreads, Exception innerException) : base(ReplayStrings.ReplayServiceTooManyThreadsException(numberOfThreads, maxNumberOfThreads), innerException)
		{
			this.numberOfThreads = numberOfThreads;
			this.maxNumberOfThreads = maxNumberOfThreads;
		}

		protected ReplayServiceTooManyThreadsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.numberOfThreads = (long)info.GetValue("numberOfThreads", typeof(long));
			this.maxNumberOfThreads = (long)info.GetValue("maxNumberOfThreads", typeof(long));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("numberOfThreads", this.numberOfThreads);
			info.AddValue("maxNumberOfThreads", this.maxNumberOfThreads);
		}

		public long NumberOfThreads
		{
			get
			{
				return this.numberOfThreads;
			}
		}

		public long MaxNumberOfThreads
		{
			get
			{
				return this.maxNumberOfThreads;
			}
		}

		private readonly long numberOfThreads;

		private readonly long maxNumberOfThreads;
	}
}
