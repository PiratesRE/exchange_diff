using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Setup.Bootstrapper.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class FileNotExistsException : LocalizedException
	{
		public FileNotExistsException(string fullFilePath) : base(Strings.FileNotExistsError(fullFilePath))
		{
			this.fullFilePath = fullFilePath;
		}

		public FileNotExistsException(string fullFilePath, Exception innerException) : base(Strings.FileNotExistsError(fullFilePath), innerException)
		{
			this.fullFilePath = fullFilePath;
		}

		protected FileNotExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.fullFilePath = (string)info.GetValue("fullFilePath", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("fullFilePath", this.fullFilePath);
		}

		public string FullFilePath
		{
			get
			{
				return this.fullFilePath;
			}
		}

		private readonly string fullFilePath;
	}
}
