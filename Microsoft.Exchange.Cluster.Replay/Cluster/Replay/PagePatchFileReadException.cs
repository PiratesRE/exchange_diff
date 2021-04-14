using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PagePatchFileReadException : PagePatchApiFailedException
	{
		public PagePatchFileReadException(string fileName, long actualBytesRead, long expectedBytesRead) : base(ReplayStrings.PagePatchFileReadException(fileName, actualBytesRead, expectedBytesRead))
		{
			this.fileName = fileName;
			this.actualBytesRead = actualBytesRead;
			this.expectedBytesRead = expectedBytesRead;
		}

		public PagePatchFileReadException(string fileName, long actualBytesRead, long expectedBytesRead, Exception innerException) : base(ReplayStrings.PagePatchFileReadException(fileName, actualBytesRead, expectedBytesRead), innerException)
		{
			this.fileName = fileName;
			this.actualBytesRead = actualBytesRead;
			this.expectedBytesRead = expectedBytesRead;
		}

		protected PagePatchFileReadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.fileName = (string)info.GetValue("fileName", typeof(string));
			this.actualBytesRead = (long)info.GetValue("actualBytesRead", typeof(long));
			this.expectedBytesRead = (long)info.GetValue("expectedBytesRead", typeof(long));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("fileName", this.fileName);
			info.AddValue("actualBytesRead", this.actualBytesRead);
			info.AddValue("expectedBytesRead", this.expectedBytesRead);
		}

		public string FileName
		{
			get
			{
				return this.fileName;
			}
		}

		public long ActualBytesRead
		{
			get
			{
				return this.actualBytesRead;
			}
		}

		public long ExpectedBytesRead
		{
			get
			{
				return this.expectedBytesRead;
			}
		}

		private readonly string fileName;

		private readonly long actualBytesRead;

		private readonly long expectedBytesRead;
	}
}
