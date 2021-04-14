using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidFileNameException : LocalizedException
	{
		public InvalidFileNameException(int fileNameMaximumLength) : base(Strings.InvalidFileNameException(fileNameMaximumLength))
		{
			this.fileNameMaximumLength = fileNameMaximumLength;
		}

		public InvalidFileNameException(int fileNameMaximumLength, Exception innerException) : base(Strings.InvalidFileNameException(fileNameMaximumLength), innerException)
		{
			this.fileNameMaximumLength = fileNameMaximumLength;
		}

		protected InvalidFileNameException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.fileNameMaximumLength = (int)info.GetValue("fileNameMaximumLength", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("fileNameMaximumLength", this.fileNameMaximumLength);
		}

		public int FileNameMaximumLength
		{
			get
			{
				return this.fileNameMaximumLength;
			}
		}

		private readonly int fileNameMaximumLength;
	}
}
