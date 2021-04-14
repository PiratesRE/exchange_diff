using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Setup.Bootstrapper.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class StartSetupFileNotFoundException : LocalizedException
	{
		public StartSetupFileNotFoundException(string fileName) : base(Strings.StartSetupFileNotFound(fileName))
		{
			this.fileName = fileName;
		}

		public StartSetupFileNotFoundException(string fileName, Exception innerException) : base(Strings.StartSetupFileNotFound(fileName), innerException)
		{
			this.fileName = fileName;
		}

		protected StartSetupFileNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
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
