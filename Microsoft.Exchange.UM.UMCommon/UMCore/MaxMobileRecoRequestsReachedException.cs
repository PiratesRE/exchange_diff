using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MaxMobileRecoRequestsReachedException : MobileRecoRequestCannotBeHandledException
	{
		public MaxMobileRecoRequestsReachedException(int current, int max) : base(Strings.MaxMobileRecoRequestsReached(current, max))
		{
			this.current = current;
			this.max = max;
		}

		public MaxMobileRecoRequestsReachedException(int current, int max, Exception innerException) : base(Strings.MaxMobileRecoRequestsReached(current, max), innerException)
		{
			this.current = current;
			this.max = max;
		}

		protected MaxMobileRecoRequestsReachedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.current = (int)info.GetValue("current", typeof(int));
			this.max = (int)info.GetValue("max", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("current", this.current);
			info.AddValue("max", this.max);
		}

		public int Current
		{
			get
			{
				return this.current;
			}
		}

		public int Max
		{
			get
			{
				return this.max;
			}
		}

		private readonly int current;

		private readonly int max;
	}
}
