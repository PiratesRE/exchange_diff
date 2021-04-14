using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class HttpResponseStreamNullException : LocalizedException
	{
		public HttpResponseStreamNullException() : base(Strings.HttpResponseStreamNullException)
		{
		}

		public HttpResponseStreamNullException(Exception innerException) : base(Strings.HttpResponseStreamNullException, innerException)
		{
		}

		protected HttpResponseStreamNullException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
