using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmBcsSelectionException : AmBcsException
	{
		public AmBcsSelectionException(string bcsMessage) : base(ReplayStrings.AmBcsSelectionException(bcsMessage))
		{
			this.bcsMessage = bcsMessage;
		}

		public AmBcsSelectionException(string bcsMessage, Exception innerException) : base(ReplayStrings.AmBcsSelectionException(bcsMessage), innerException)
		{
			this.bcsMessage = bcsMessage;
		}

		protected AmBcsSelectionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.bcsMessage = (string)info.GetValue("bcsMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("bcsMessage", this.bcsMessage);
		}

		public string BcsMessage
		{
			get
			{
				return this.bcsMessage;
			}
		}

		private readonly string bcsMessage;
	}
}
