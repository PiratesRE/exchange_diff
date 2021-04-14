using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayDbOperationException : TaskServerException
	{
		public ReplayDbOperationException(string opError) : base(ReplayStrings.ReplayDbOperationException(opError))
		{
			this.opError = opError;
		}

		public ReplayDbOperationException(string opError, Exception innerException) : base(ReplayStrings.ReplayDbOperationException(opError), innerException)
		{
			this.opError = opError;
		}

		protected ReplayDbOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.opError = (string)info.GetValue("opError", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("opError", this.opError);
		}

		public string OpError
		{
			get
			{
				return this.opError;
			}
		}

		private readonly string opError;
	}
}
