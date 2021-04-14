using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToGetProcessForServiceException : TransientException
	{
		public FailedToGetProcessForServiceException(string serviceName, string msg) : base(ReplayStrings.FailedToGetProcessForServiceException(serviceName, msg))
		{
			this.serviceName = serviceName;
			this.msg = msg;
		}

		public FailedToGetProcessForServiceException(string serviceName, string msg, Exception innerException) : base(ReplayStrings.FailedToGetProcessForServiceException(serviceName, msg), innerException)
		{
			this.serviceName = serviceName;
			this.msg = msg;
		}

		protected FailedToGetProcessForServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.serviceName = (string)info.GetValue("serviceName", typeof(string));
			this.msg = (string)info.GetValue("msg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("serviceName", this.serviceName);
			info.AddValue("msg", this.msg);
		}

		public string ServiceName
		{
			get
			{
				return this.serviceName;
			}
		}

		public string Msg
		{
			get
			{
				return this.msg;
			}
		}

		private readonly string serviceName;

		private readonly string msg;
	}
}
