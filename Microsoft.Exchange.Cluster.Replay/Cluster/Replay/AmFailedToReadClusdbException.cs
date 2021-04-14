using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmFailedToReadClusdbException : ClusterException
	{
		public AmFailedToReadClusdbException(string error) : base(ReplayStrings.AmFailedToReadClusdbException(error))
		{
			this.error = error;
		}

		public AmFailedToReadClusdbException(string error, Exception innerException) : base(ReplayStrings.AmFailedToReadClusdbException(error), innerException)
		{
			this.error = error;
		}

		protected AmFailedToReadClusdbException(SerializationInfo info, StreamingContext context) : base(info, context)
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
