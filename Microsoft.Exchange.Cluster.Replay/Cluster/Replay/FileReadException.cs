using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FileReadException : LocalizedException
	{
		public FileReadException(string fileName, int expectedBytes, int actualBytes) : base(ReplayStrings.FileReadException(fileName, expectedBytes, actualBytes))
		{
			this.fileName = fileName;
			this.expectedBytes = expectedBytes;
			this.actualBytes = actualBytes;
		}

		public FileReadException(string fileName, int expectedBytes, int actualBytes, Exception innerException) : base(ReplayStrings.FileReadException(fileName, expectedBytes, actualBytes), innerException)
		{
			this.fileName = fileName;
			this.expectedBytes = expectedBytes;
			this.actualBytes = actualBytes;
		}

		protected FileReadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.fileName = (string)info.GetValue("fileName", typeof(string));
			this.expectedBytes = (int)info.GetValue("expectedBytes", typeof(int));
			this.actualBytes = (int)info.GetValue("actualBytes", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("fileName", this.fileName);
			info.AddValue("expectedBytes", this.expectedBytes);
			info.AddValue("actualBytes", this.actualBytes);
		}

		public string FileName
		{
			get
			{
				return this.fileName;
			}
		}

		public int ExpectedBytes
		{
			get
			{
				return this.expectedBytes;
			}
		}

		public int ActualBytes
		{
			get
			{
				return this.actualBytes;
			}
		}

		private readonly string fileName;

		private readonly int expectedBytes;

		private readonly int actualBytes;
	}
}
