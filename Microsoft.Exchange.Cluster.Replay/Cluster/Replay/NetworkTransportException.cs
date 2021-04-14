using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NetworkTransportException : TransientException
	{
		public NetworkTransportException(string err) : base(ReplayStrings.NetworkTransportError(err))
		{
			this.err = err;
		}

		public NetworkTransportException(string err, Exception innerException) : base(ReplayStrings.NetworkTransportError(err), innerException)
		{
			this.err = err;
		}

		protected NetworkTransportException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.err = (string)info.GetValue("err", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("err", this.err);
		}

		public string Err
		{
			get
			{
				return this.err;
			}
		}

		private readonly string err;
	}
}
