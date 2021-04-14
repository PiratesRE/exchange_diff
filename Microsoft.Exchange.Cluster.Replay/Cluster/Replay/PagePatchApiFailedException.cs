using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PagePatchApiFailedException : LocalizedException
	{
		public PagePatchApiFailedException(string msg) : base(ReplayStrings.PagePatchApiFailedException(msg))
		{
			this.msg = msg;
		}

		public PagePatchApiFailedException(string msg, Exception innerException) : base(ReplayStrings.PagePatchApiFailedException(msg), innerException)
		{
			this.msg = msg;
		}

		protected PagePatchApiFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
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
