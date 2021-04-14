using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RepairStateException : LocalizedException
	{
		public RepairStateException(string error) : base(ReplayStrings.RepairStateError(error))
		{
			this.error = error;
		}

		public RepairStateException(string error, Exception innerException) : base(ReplayStrings.RepairStateError(error), innerException)
		{
			this.error = error;
		}

		protected RepairStateException(SerializationInfo info, StreamingContext context) : base(info, context)
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
