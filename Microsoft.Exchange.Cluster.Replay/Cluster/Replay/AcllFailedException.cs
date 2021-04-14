using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AcllFailedException : TransientException
	{
		public AcllFailedException(string error) : base(ReplayStrings.AcllFailedException(error))
		{
			this.error = error;
		}

		public AcllFailedException(string error, Exception innerException) : base(ReplayStrings.AcllFailedException(error), innerException)
		{
			this.error = error;
		}

		protected AcllFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("error", this.error);
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string error;
	}
}
