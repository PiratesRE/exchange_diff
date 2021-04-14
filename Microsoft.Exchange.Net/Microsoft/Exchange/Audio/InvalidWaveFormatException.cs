using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Audio
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidWaveFormatException : LocalizedException
	{
		public InvalidWaveFormatException(string s) : base(NetException.InvalidWaveFormat(s))
		{
			this.s = s;
		}

		public InvalidWaveFormatException(string s, Exception innerException) : base(NetException.InvalidWaveFormat(s), innerException)
		{
			this.s = s;
		}

		protected InvalidWaveFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.s = (string)info.GetValue("s", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("s", this.s);
		}

		public string S
		{
			get
			{
				return this.s;
			}
		}

		private readonly string s;
	}
}
