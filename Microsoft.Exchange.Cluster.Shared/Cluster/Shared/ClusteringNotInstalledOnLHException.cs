using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ClusteringNotInstalledOnLHException : ClusCommonFailException
	{
		public ClusteringNotInstalledOnLHException(string errorMessage) : base(Strings.ClusteringNotInstalledOnLHException(errorMessage))
		{
			this.errorMessage = errorMessage;
		}

		public ClusteringNotInstalledOnLHException(string errorMessage, Exception innerException) : base(Strings.ClusteringNotInstalledOnLHException(errorMessage), innerException)
		{
			this.errorMessage = errorMessage;
		}

		protected ClusteringNotInstalledOnLHException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errorMessage = (string)info.GetValue("errorMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errorMessage", this.errorMessage);
		}

		public string ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
		}

		private readonly string errorMessage;
	}
}
