using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.EseRepl
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NetworkCommunicationException : NetworkTransportException
	{
		public NetworkCommunicationException(string remoteNodeName, string errorText) : base(Strings.NetworkCommunicationError(remoteNodeName, errorText))
		{
			this.remoteNodeName = remoteNodeName;
			this.errorText = errorText;
		}

		public NetworkCommunicationException(string remoteNodeName, string errorText, Exception innerException) : base(Strings.NetworkCommunicationError(remoteNodeName, errorText), innerException)
		{
			this.remoteNodeName = remoteNodeName;
			this.errorText = errorText;
		}

		protected NetworkCommunicationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.remoteNodeName = (string)info.GetValue("remoteNodeName", typeof(string));
			this.errorText = (string)info.GetValue("errorText", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("remoteNodeName", this.remoteNodeName);
			info.AddValue("errorText", this.errorText);
		}

		public string RemoteNodeName
		{
			get
			{
				return this.remoteNodeName;
			}
		}

		public string ErrorText
		{
			get
			{
				return this.errorText;
			}
		}

		private readonly string remoteNodeName;

		private readonly string errorText;
	}
}
