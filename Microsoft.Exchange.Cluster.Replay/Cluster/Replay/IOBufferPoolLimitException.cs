using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IOBufferPoolLimitException : LocalizedException
	{
		public IOBufferPoolLimitException(int limit, int bufSize) : base(ReplayStrings.IOBufferPoolLimitError(limit, bufSize))
		{
			this.limit = limit;
			this.bufSize = bufSize;
		}

		public IOBufferPoolLimitException(int limit, int bufSize, Exception innerException) : base(ReplayStrings.IOBufferPoolLimitError(limit, bufSize), innerException)
		{
			this.limit = limit;
			this.bufSize = bufSize;
		}

		protected IOBufferPoolLimitException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.limit = (int)info.GetValue("limit", typeof(int));
			this.bufSize = (int)info.GetValue("bufSize", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("limit", this.limit);
			info.AddValue("bufSize", this.bufSize);
		}

		public int Limit
		{
			get
			{
				return this.limit;
			}
		}

		public int BufSize
		{
			get
			{
				return this.bufSize;
			}
		}

		private readonly int limit;

		private readonly int bufSize;
	}
}
