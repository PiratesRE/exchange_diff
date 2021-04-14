using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class TPRInitException : TransientException
	{
		public TPRInitException(string errMsg) : base(ReplayStrings.TPRInitFailure(errMsg))
		{
			this.errMsg = errMsg;
		}

		public TPRInitException(string errMsg, Exception innerException) : base(ReplayStrings.TPRInitFailure(errMsg), innerException)
		{
			this.errMsg = errMsg;
		}

		protected TPRInitException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.errMsg = (string)info.GetValue("errMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("errMsg", this.errMsg);
		}

		public string ErrMsg
		{
			get
			{
				return this.errMsg;
			}
		}

		private readonly string errMsg;
	}
}
