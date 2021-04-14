using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ErrorFileIsTooLargeForFingerprintException : LocalizedException
	{
		public ErrorFileIsTooLargeForFingerprintException(int fileSize, int max) : base(Strings.ErrorFileIsTooLargeForFingerprint(fileSize, max))
		{
			this.fileSize = fileSize;
			this.max = max;
		}

		public ErrorFileIsTooLargeForFingerprintException(int fileSize, int max, Exception innerException) : base(Strings.ErrorFileIsTooLargeForFingerprint(fileSize, max), innerException)
		{
			this.fileSize = fileSize;
			this.max = max;
		}

		protected ErrorFileIsTooLargeForFingerprintException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.fileSize = (int)info.GetValue("fileSize", typeof(int));
			this.max = (int)info.GetValue("max", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("fileSize", this.fileSize);
			info.AddValue("max", this.max);
		}

		public int FileSize
		{
			get
			{
				return this.fileSize;
			}
		}

		public int Max
		{
			get
			{
				return this.max;
			}
		}

		private readonly int fileSize;

		private readonly int max;
	}
}
