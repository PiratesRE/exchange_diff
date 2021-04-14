using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PagePatchInvalidPageSizeException : PagePatchApiFailedException
	{
		public PagePatchInvalidPageSizeException(long dataSize, long expectedPageSize) : base(ReplayStrings.PagePatchInvalidPageSizeException(dataSize, expectedPageSize))
		{
			this.dataSize = dataSize;
			this.expectedPageSize = expectedPageSize;
		}

		public PagePatchInvalidPageSizeException(long dataSize, long expectedPageSize, Exception innerException) : base(ReplayStrings.PagePatchInvalidPageSizeException(dataSize, expectedPageSize), innerException)
		{
			this.dataSize = dataSize;
			this.expectedPageSize = expectedPageSize;
		}

		protected PagePatchInvalidPageSizeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dataSize = (long)info.GetValue("dataSize", typeof(long));
			this.expectedPageSize = (long)info.GetValue("expectedPageSize", typeof(long));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dataSize", this.dataSize);
			info.AddValue("expectedPageSize", this.expectedPageSize);
		}

		public long DataSize
		{
			get
			{
				return this.dataSize;
			}
		}

		public long ExpectedPageSize
		{
			get
			{
				return this.expectedPageSize;
			}
		}

		private readonly long dataSize;

		private readonly long expectedPageSize;
	}
}
