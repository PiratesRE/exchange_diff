using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class HeaderFileArgumentInvalidException : LocalizedException
	{
		public HeaderFileArgumentInvalidException(string argName) : base(Strings.HeaderFileArgumentInvalid(argName))
		{
			this.argName = argName;
		}

		public HeaderFileArgumentInvalidException(string argName, Exception innerException) : base(Strings.HeaderFileArgumentInvalid(argName), innerException)
		{
			this.argName = argName;
		}

		protected HeaderFileArgumentInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.argName = (string)info.GetValue("argName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("argName", this.argName);
		}

		public string ArgName
		{
			get
			{
				return this.argName;
			}
		}

		private readonly string argName;
	}
}
