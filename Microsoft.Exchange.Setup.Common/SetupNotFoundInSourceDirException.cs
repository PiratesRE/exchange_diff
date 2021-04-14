using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Setup.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SetupNotFoundInSourceDirException : LocalizedException
	{
		public SetupNotFoundInSourceDirException(string fileName) : base(Strings.SetupNotFoundInSourceDirError(fileName))
		{
			this.fileName = fileName;
		}

		public SetupNotFoundInSourceDirException(string fileName, Exception innerException) : base(Strings.SetupNotFoundInSourceDirError(fileName), innerException)
		{
			this.fileName = fileName;
		}

		protected SetupNotFoundInSourceDirException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.fileName = (string)info.GetValue("fileName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("fileName", this.fileName);
		}

		public string FileName
		{
			get
			{
				return this.fileName;
			}
		}

		private readonly string fileName;
	}
}
