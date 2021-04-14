using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServerProtocolViolationException : LocalizedException
	{
		public ServerProtocolViolationException(string size) : base(HttpStrings.ServerProtocolViolationException(size))
		{
			this.size = size;
		}

		public ServerProtocolViolationException(string size, Exception innerException) : base(HttpStrings.ServerProtocolViolationException(size), innerException)
		{
			this.size = size;
		}

		protected ServerProtocolViolationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.size = (string)info.GetValue("size", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("size", this.size);
		}

		public string Size
		{
			get
			{
				return this.size;
			}
		}

		private readonly string size;
	}
}
