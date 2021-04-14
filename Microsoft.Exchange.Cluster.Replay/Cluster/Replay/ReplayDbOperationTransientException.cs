using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplayDbOperationTransientException : TaskServerTransientException
	{
		public ReplayDbOperationTransientException(string opError) : base(ReplayStrings.ReplayDbOperationTransientException(opError))
		{
			this.opError = opError;
		}

		public ReplayDbOperationTransientException(string opError, Exception innerException) : base(ReplayStrings.ReplayDbOperationTransientException(opError), innerException)
		{
			this.opError = opError;
		}

		protected ReplayDbOperationTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
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
