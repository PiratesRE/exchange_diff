using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class EasCommandFailedTransientException : ConnectionsTransientException
	{
		public EasCommandFailedTransientException(string responseStatus, string httpStatus) : base(CXStrings.EasCommandFailed(responseStatus, httpStatus))
		{
			this.responseStatus = responseStatus;
			this.httpStatus = httpStatus;
		}

		public EasCommandFailedTransientException(string responseStatus, string httpStatus, Exception innerException) : base(CXStrings.EasCommandFailed(responseStatus, httpStatus), innerException)
		{
			this.responseStatus = responseStatus;
			this.httpStatus = httpStatus;
		}

		protected EasCommandFailedTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.responseStatus = (string)info.GetValue("responseStatus", typeof(string));
			this.httpStatus = (string)info.GetValue("httpStatus", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("responseStatus", this.responseStatus);
			info.AddValue("httpStatus", this.httpStatus);
		}

		public string ResponseStatus
		{
			get
			{
				return this.responseStatus;
			}
		}

		public string HttpStatus
		{
			get
			{
				return this.httpStatus;
			}
		}

		private readonly string responseStatus;

		private readonly string httpStatus;
	}
}
