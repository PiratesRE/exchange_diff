using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmBcsException : AmServerException
	{
		public AmBcsException(string bcsError) : base(ReplayStrings.AmBcsException(bcsError))
		{
			this.bcsError = bcsError;
		}

		public AmBcsException(string bcsError, Exception innerException) : base(ReplayStrings.AmBcsException(bcsError), innerException)
		{
			this.bcsError = bcsError;
		}

		protected AmBcsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.bcsError = (string)info.GetValue("bcsError", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("bcsError", this.bcsError);
		}

		public string BcsError
		{
			get
			{
				return this.bcsError;
			}
		}

		private readonly string bcsError;
	}
}
