using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Security.Authentication
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class LogonAsNetworkServiceException : LocalizedException
	{
		public LogonAsNetworkServiceException(string error) : base(NetException.LogonAsNetworkServiceFailed(error))
		{
			this.error = error;
		}

		public LogonAsNetworkServiceException(string error, Exception innerException) : base(NetException.LogonAsNetworkServiceFailed(error), innerException)
		{
			this.error = error;
		}

		protected LogonAsNetworkServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("error", this.error);
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string error;
	}
}
