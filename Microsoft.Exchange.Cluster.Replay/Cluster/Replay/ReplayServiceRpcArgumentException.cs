using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayServiceRpcArgumentException : TaskServerException
	{
		public ReplayServiceRpcArgumentException(string argument) : base(ReplayStrings.ReplayServiceRpcArgumentException(argument))
		{
			this.argument = argument;
		}

		public ReplayServiceRpcArgumentException(string argument, Exception innerException) : base(ReplayStrings.ReplayServiceRpcArgumentException(argument), innerException)
		{
			this.argument = argument;
		}

		protected ReplayServiceRpcArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.argument = (string)info.GetValue("argument", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("argument", this.argument);
		}

		public string Argument
		{
			get
			{
				return this.argument;
			}
		}

		private readonly string argument;
	}
}
