using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CommunicationErrorTransientException : TransientException
	{
		public CommunicationErrorTransientException(string serviceURI, LocalizedString exceptionMessage) : base(NetServerException.CommunicationError(serviceURI, exceptionMessage))
		{
			this.serviceURI = serviceURI;
			this.exceptionMessage = exceptionMessage;
		}

		public CommunicationErrorTransientException(string serviceURI, LocalizedString exceptionMessage, Exception innerException) : base(NetServerException.CommunicationError(serviceURI, exceptionMessage), innerException)
		{
			this.serviceURI = serviceURI;
			this.exceptionMessage = exceptionMessage;
		}

		protected CommunicationErrorTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serviceURI = (string)info.GetValue("serviceURI", typeof(string));
			this.exceptionMessage = (LocalizedString)info.GetValue("exceptionMessage", typeof(LocalizedString));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serviceURI", this.serviceURI);
			info.AddValue("exceptionMessage", this.exceptionMessage);
		}

		public string ServiceURI
		{
			get
			{
				return this.serviceURI;
			}
		}

		public LocalizedString ExceptionMessage
		{
			get
			{
				return this.exceptionMessage;
			}
		}

		private readonly string serviceURI;

		private readonly LocalizedString exceptionMessage;
	}
}
