using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToReadDatabasePage : TransientException
	{
		public FailedToReadDatabasePage(int error) : base(ReplayStrings.FailedToReadDatabasePage(error))
		{
			this.error = error;
		}

		public FailedToReadDatabasePage(int error, Exception innerException) : base(ReplayStrings.FailedToReadDatabasePage(error), innerException)
		{
			this.error = error;
		}

		protected FailedToReadDatabasePage(SerializationInfo info, StreamingContext context) : base(info, context)
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
