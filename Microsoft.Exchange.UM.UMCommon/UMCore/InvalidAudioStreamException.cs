using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidAudioStreamException : LocalizedException
	{
		public InvalidAudioStreamException(string msg) : base(Strings.InvalidAudioStreamException(msg))
		{
			this.msg = msg;
		}

		public InvalidAudioStreamException(string msg, Exception innerException) : base(Strings.InvalidAudioStreamException(msg), innerException)
		{
			this.msg = msg;
		}

		protected InvalidAudioStreamException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.msg = (string)info.GetValue("msg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("msg", this.msg);
		}

		public string Msg
		{
			get
			{
				return this.msg;
			}
		}

		private readonly string msg;
	}
}
