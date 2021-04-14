using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.UM.UMCore;
using Microsoft.Exchange.UM.UMService.Exceptions;

namespace Microsoft.Exchange.UM.UMService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UMServiceControlChannelException : UMServiceBaseException
	{
		public UMServiceControlChannelException(int port, string errorMessage) : base(Strings.UMServiceControlChannelException(port, errorMessage))
		{
			this.port = port;
			this.errorMessage = errorMessage;
		}

		public UMServiceControlChannelException(int port, string errorMessage, Exception innerException) : base(Strings.UMServiceControlChannelException(port, errorMessage), innerException)
		{
			this.port = port;
			this.errorMessage = errorMessage;
		}

		protected UMServiceControlChannelException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.port = (int)info.GetValue("port", typeof(int));
			this.errorMessage = (string)info.GetValue("errorMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("port", this.port);
			info.AddValue("errorMessage", this.errorMessage);
		}

		public int Port
		{
			get
			{
				return this.port;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
		}

		private readonly int port;

		private readonly string errorMessage;
	}
}
