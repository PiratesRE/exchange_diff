using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayServiceTooMuchMemoryException : TransientException
	{
		public ReplayServiceTooMuchMemoryException(double memoryUsageInMib, long maximumMemoryUsageInMib) : base(ReplayStrings.ReplayServiceTooMuchMemoryException(memoryUsageInMib, maximumMemoryUsageInMib))
		{
			this.memoryUsageInMib = memoryUsageInMib;
			this.maximumMemoryUsageInMib = maximumMemoryUsageInMib;
		}

		public ReplayServiceTooMuchMemoryException(double memoryUsageInMib, long maximumMemoryUsageInMib, Exception innerException) : base(ReplayStrings.ReplayServiceTooMuchMemoryException(memoryUsageInMib, maximumMemoryUsageInMib), innerException)
		{
			this.memoryUsageInMib = memoryUsageInMib;
			this.maximumMemoryUsageInMib = maximumMemoryUsageInMib;
		}

		protected ReplayServiceTooMuchMemoryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.memoryUsageInMib = (double)info.GetValue("memoryUsageInMib", typeof(double));
			this.maximumMemoryUsageInMib = (long)info.GetValue("maximumMemoryUsageInMib", typeof(long));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("memoryUsageInMib", this.memoryUsageInMib);
			info.AddValue("maximumMemoryUsageInMib", this.maximumMemoryUsageInMib);
		}

		public double MemoryUsageInMib
		{
			get
			{
				return this.memoryUsageInMib;
			}
		}

		public long MaximumMemoryUsageInMib
		{
			get
			{
				return this.maximumMemoryUsageInMib;
			}
		}

		private readonly double memoryUsageInMib;

		private readonly long maximumMemoryUsageInMib;
	}
}
