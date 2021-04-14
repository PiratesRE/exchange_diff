using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class IncrementalReseedFailedException : TransientException
	{
		public IncrementalReseedFailedException(string msg, uint error) : base(ReplayStrings.IncrementalReseedFailedException(msg, error))
		{
			this.msg = msg;
			this.error = error;
		}

		public IncrementalReseedFailedException(string msg, uint error, Exception innerException) : base(ReplayStrings.IncrementalReseedFailedException(msg, error), innerException)
		{
			this.msg = msg;
			this.error = error;
		}

		protected IncrementalReseedFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.msg = (string)info.GetValue("msg", typeof(string));
			this.error = (uint)info.GetValue("error", typeof(uint));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("msg", this.msg);
			info.AddValue("error", this.error);
		}

		public string Msg
		{
			get
			{
				return this.msg;
			}
		}

		public uint Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string msg;

		private readonly uint error;
	}
}
