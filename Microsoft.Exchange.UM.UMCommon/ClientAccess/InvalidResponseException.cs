using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.ClientAccess
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidResponseException : TransportException
	{
		public InvalidResponseException(string channel, string error) : base(Strings.InvalidResponseException(channel, error))
		{
			this.channel = channel;
			this.error = error;
		}

		public InvalidResponseException(string channel, string error, Exception innerException) : base(Strings.InvalidResponseException(channel, error), innerException)
		{
			this.channel = channel;
			this.error = error;
		}

		protected InvalidResponseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.channel = (string)info.GetValue("channel", typeof(string));
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("channel", this.channel);
			info.AddValue("error", this.error);
		}

		public string Channel
		{
			get
			{
				return this.channel;
			}
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string channel;

		private readonly string error;
	}
}
