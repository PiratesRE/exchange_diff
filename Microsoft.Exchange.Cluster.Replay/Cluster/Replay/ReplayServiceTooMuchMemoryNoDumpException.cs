using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayServiceTooMuchMemoryNoDumpException : TransientException
	{
		public ReplayServiceTooMuchMemoryNoDumpException(double memoryUsageInMib, long maximumMemoryUsageInMib, string enableWatsonRegKey) : base(ReplayStrings.ReplayServiceTooMuchMemoryNoDumpException(memoryUsageInMib, maximumMemoryUsageInMib, enableWatsonRegKey))
		{
			this.memoryUsageInMib = memoryUsageInMib;
			this.maximumMemoryUsageInMib = maximumMemoryUsageInMib;
			this.enableWatsonRegKey = enableWatsonRegKey;
		}

		public ReplayServiceTooMuchMemoryNoDumpException(double memoryUsageInMib, long maximumMemoryUsageInMib, string enableWatsonRegKey, Exception innerException) : base(ReplayStrings.ReplayServiceTooMuchMemoryNoDumpException(memoryUsageInMib, maximumMemoryUsageInMib, enableWatsonRegKey), innerException)
		{
			this.memoryUsageInMib = memoryUsageInMib;
			this.maximumMemoryUsageInMib = maximumMemoryUsageInMib;
			this.enableWatsonRegKey = enableWatsonRegKey;
		}

		protected ReplayServiceTooMuchMemoryNoDumpException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.memoryUsageInMib = (double)info.GetValue("memoryUsageInMib", typeof(double));
			this.maximumMemoryUsageInMib = (long)info.GetValue("maximumMemoryUsageInMib", typeof(long));
			this.enableWatsonRegKey = (string)info.GetValue("enableWatsonRegKey", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("memoryUsageInMib", this.memoryUsageInMib);
			info.AddValue("maximumMemoryUsageInMib", this.maximumMemoryUsageInMib);
			info.AddValue("enableWatsonRegKey", this.enableWatsonRegKey);
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

		public string EnableWatsonRegKey
		{
			get
			{
				return this.enableWatsonRegKey;
			}
		}

		private readonly double memoryUsageInMib;

		private readonly long maximumMemoryUsageInMib;

		private readonly string enableWatsonRegKey;
	}
}
