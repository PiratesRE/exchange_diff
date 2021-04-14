using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SuspendCommentTooLongException : LocalizedException
	{
		public SuspendCommentTooLongException(int length, int limit) : base(ReplayStrings.SuspendCommentTooLong(length, limit))
		{
			this.length = length;
			this.limit = limit;
		}

		public SuspendCommentTooLongException(int length, int limit, Exception innerException) : base(ReplayStrings.SuspendCommentTooLong(length, limit), innerException)
		{
			this.length = length;
			this.limit = limit;
		}

		protected SuspendCommentTooLongException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.length = (int)info.GetValue("length", typeof(int));
			this.limit = (int)info.GetValue("limit", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("length", this.length);
			info.AddValue("limit", this.limit);
		}

		public int Length
		{
			get
			{
				return this.length;
			}
		}

		public int Limit
		{
			get
			{
				return this.limit;
			}
		}

		private readonly int length;

		private readonly int limit;
	}
}
