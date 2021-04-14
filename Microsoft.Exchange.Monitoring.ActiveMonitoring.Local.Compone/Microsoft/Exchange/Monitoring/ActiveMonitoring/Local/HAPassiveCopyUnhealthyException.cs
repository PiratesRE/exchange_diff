using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class HAPassiveCopyUnhealthyException : LocalizedException
	{
		public HAPassiveCopyUnhealthyException(string copyState) : base(Strings.HAPassiveCopyUnhealthy(copyState))
		{
			this.copyState = copyState;
		}

		public HAPassiveCopyUnhealthyException(string copyState, Exception innerException) : base(Strings.HAPassiveCopyUnhealthy(copyState), innerException)
		{
			this.copyState = copyState;
		}

		protected HAPassiveCopyUnhealthyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.copyState = (string)info.GetValue("copyState", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("copyState", this.copyState);
		}

		public string CopyState
		{
			get
			{
				return this.copyState;
			}
		}

		private readonly string copyState;
	}
}
