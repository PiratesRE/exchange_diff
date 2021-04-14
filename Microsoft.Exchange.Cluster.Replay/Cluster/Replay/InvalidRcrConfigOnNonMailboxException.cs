using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidRcrConfigOnNonMailboxException : TransientException
	{
		public InvalidRcrConfigOnNonMailboxException(string nodeName) : base(ReplayStrings.InvalidRcrConfigOnNonMailboxException(nodeName))
		{
			this.nodeName = nodeName;
		}

		public InvalidRcrConfigOnNonMailboxException(string nodeName, Exception innerException) : base(ReplayStrings.InvalidRcrConfigOnNonMailboxException(nodeName), innerException)
		{
			this.nodeName = nodeName;
		}

		protected InvalidRcrConfigOnNonMailboxException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.nodeName = (string)info.GetValue("nodeName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("nodeName", this.nodeName);
		}

		public string NodeName
		{
			get
			{
				return this.nodeName;
			}
		}

		private readonly string nodeName;
	}
}
