using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LastLogReplacementException : LocalizedException
	{
		public LastLogReplacementException(string msg) : base(ReplayStrings.LastLogReplacementException(msg))
		{
			this.msg = msg;
		}

		public LastLogReplacementException(string msg, Exception innerException) : base(ReplayStrings.LastLogReplacementException(msg), innerException)
		{
			this.msg = msg;
		}

		protected LastLogReplacementException(SerializationInfo info, StreamingContext context) : base(info, context)
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
