using System;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	[Serializable]
	internal class SerializationException : OnlineMeetingSchedulerException
	{
		public SerializationException(string message) : this(message, null)
		{
		}

		public SerializationException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected SerializationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public HttpWebResponse HttpResponse { get; set; }

		[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
