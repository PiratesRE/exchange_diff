using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Shared
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ClusterApiException : ClusterException
	{
		public ClusterApiException(string msg) : base(Strings.ClusterApiException(msg))
		{
			this.msg = msg;
		}

		public ClusterApiException(string msg, Exception innerException) : base(Strings.ClusterApiException(msg), innerException)
		{
			this.msg = msg;
		}

		protected ClusterApiException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.msg = (string)info.GetValue("msg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("msg", this.msg);
		}

		public string Msg
		{
			get
			{
				return this.msg;
			}
		}

		private readonly string msg;
	}
}
