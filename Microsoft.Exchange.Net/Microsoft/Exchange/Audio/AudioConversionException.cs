using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Audio
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AudioConversionException : LocalizedException
	{
		public AudioConversionException(string reason) : base(NetException.AudioConversionFailed(reason))
		{
			this.reason = reason;
		}

		public AudioConversionException(string reason, Exception innerException) : base(NetException.AudioConversionFailed(reason), innerException)
		{
			this.reason = reason;
		}

		protected AudioConversionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.reason = (string)info.GetValue("reason", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("reason", this.reason);
		}

		public string Reason
		{
			get
			{
				return this.reason;
			}
		}

		private readonly string reason;
	}
}
