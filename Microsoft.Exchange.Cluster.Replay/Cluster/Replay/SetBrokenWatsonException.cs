using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SetBrokenWatsonException : TransientException
	{
		public SetBrokenWatsonException(string errMsg) : base(ReplayStrings.SetBrokenWatsonException(errMsg))
		{
			this.errMsg = errMsg;
		}

		public SetBrokenWatsonException(string errMsg, Exception innerException) : base(ReplayStrings.SetBrokenWatsonException(errMsg), innerException)
		{
			this.errMsg = errMsg;
		}

		protected SetBrokenWatsonException(SerializationInfo info, StreamingContext context) : base(info, context)
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
