using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Common.DiskManagement
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidFilePathException : BitlockerUtilException
	{
		public InvalidFilePathException(string filePath) : base(DiskManagementStrings.InvalidFilePathError(filePath))
		{
			this.filePath = filePath;
		}

		public InvalidFilePathException(string filePath, Exception innerException) : base(DiskManagementStrings.InvalidFilePathError(filePath), innerException)
		{
			this.filePath = filePath;
		}

		protected InvalidFilePathException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.filePath = (string)info.GetValue("filePath", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("filePath", this.filePath);
		}

		public string FilePath
		{
			get
			{
				return this.filePath;
			}
		}

		private readonly string filePath;
	}
}
