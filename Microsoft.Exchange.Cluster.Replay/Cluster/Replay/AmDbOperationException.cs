using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDbOperationException : AmCommonException
	{
		public AmDbOperationException(string opError) : base(ReplayStrings.AmDbOperationException(opError))
		{
			this.opError = opError;
		}

		public AmDbOperationException(string opError, Exception innerException) : base(ReplayStrings.AmDbOperationException(opError), innerException)
		{
			this.opError = opError;
		}

		protected AmDbOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
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
