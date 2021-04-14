using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MaxPAACountReachedException : StoragePermanentException
	{
		public MaxPAACountReachedException(int count) : base(Strings.ErrorMaxPAACountReached(count))
		{
			this.count = count;
		}

		public MaxPAACountReachedException(int count, Exception innerException) : base(Strings.ErrorMaxPAACountReached(count), innerException)
		{
			this.count = count;
		}

		protected MaxPAACountReachedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.count = (int)info.GetValue("count", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("count", this.count);
		}

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		private readonly int count;
	}
}
