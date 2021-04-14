using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SpareConflictInLayoutException : DatabaseCopyLayoutException
	{
		public SpareConflictInLayoutException(int spares) : base(ReplayStrings.SpareConflictInLayoutException(spares))
		{
			this.spares = spares;
		}

		public SpareConflictInLayoutException(int spares, Exception innerException) : base(ReplayStrings.SpareConflictInLayoutException(spares), innerException)
		{
			this.spares = spares;
		}

		protected SpareConflictInLayoutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.spares = (int)info.GetValue("spares", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("spares", this.spares);
		}

		public int Spares
		{
			get
			{
				return this.spares;
			}
		}

		private readonly int spares;
	}
}
