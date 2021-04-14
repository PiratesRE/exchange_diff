using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ClusCommonFailException : ClusterException
	{
		public ClusCommonFailException(string error) : base(Strings.ClusCommonFailException(error))
		{
			this.error = error;
		}

		public ClusCommonFailException(string error, Exception innerException) : base(Strings.ClusCommonFailException(error), innerException)
		{
			this.error = error;
		}

		protected ClusCommonFailException(SerializationInfo info, StreamingContext context) : base(info, context)
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
