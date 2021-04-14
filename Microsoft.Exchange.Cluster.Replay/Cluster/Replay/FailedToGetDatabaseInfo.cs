using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToGetDatabaseInfo : TransientException
	{
		public FailedToGetDatabaseInfo(int error) : base(ReplayStrings.FailedToGetDatabaseInfo(error))
		{
			this.error = error;
		}

		public FailedToGetDatabaseInfo(int error, Exception innerException) : base(ReplayStrings.FailedToGetDatabaseInfo(error), innerException)
		{
			this.error = error;
		}

		protected FailedToGetDatabaseInfo(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.error = (int)info.GetValue("error", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("error", this.error);
		}

		public int Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly int error;
	}
}
