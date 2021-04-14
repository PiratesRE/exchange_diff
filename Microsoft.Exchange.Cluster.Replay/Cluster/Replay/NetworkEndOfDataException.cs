using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NetworkEndOfDataException : NetworkTransportException
	{
		public NetworkEndOfDataException(string nodeName, string messageText) : base(ReplayStrings.NetworkEndOfData(nodeName, messageText))
		{
			this.nodeName = nodeName;
			this.messageText = messageText;
		}

		public NetworkEndOfDataException(string nodeName, string messageText, Exception innerException) : base(ReplayStrings.NetworkEndOfData(nodeName, messageText), innerException)
		{
			this.nodeName = nodeName;
			this.messageText = messageText;
		}

		protected NetworkEndOfDataException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.nodeName = (string)info.GetValue("nodeName", typeof(string));
			this.messageText = (string)info.GetValue("messageText", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("nodeName", this.nodeName);
			info.AddValue("messageText", this.messageText);
		}

		public string NodeName
		{
			get
			{
				return this.nodeName;
			}
		}

		public string MessageText
		{
			get
			{
				return this.messageText;
			}
		}

		private readonly string nodeName;

		private readonly string messageText;
	}
}
