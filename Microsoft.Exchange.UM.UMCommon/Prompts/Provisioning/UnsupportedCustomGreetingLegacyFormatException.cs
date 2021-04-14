using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.Prompts.Provisioning
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnsupportedCustomGreetingLegacyFormatException : PublishingException
	{
		public UnsupportedCustomGreetingLegacyFormatException(string fileName) : base(Strings.UnsupportedCustomGreetingLegacyFormat(fileName))
		{
			this.fileName = fileName;
		}

		public UnsupportedCustomGreetingLegacyFormatException(string fileName, Exception innerException) : base(Strings.UnsupportedCustomGreetingLegacyFormat(fileName), innerException)
		{
			this.fileName = fileName;
		}

		protected UnsupportedCustomGreetingLegacyFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
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
