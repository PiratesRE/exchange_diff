using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDbActionRejectedMountAtStartupNotEnabledException : AmCommonException
	{
		public AmDbActionRejectedMountAtStartupNotEnabledException(string actionCode) : base(ReplayStrings.AmDbActionRejectedMountAtStartupNotEnabledException(actionCode))
		{
			this.actionCode = actionCode;
		}

		public AmDbActionRejectedMountAtStartupNotEnabledException(string actionCode, Exception innerException) : base(ReplayStrings.AmDbActionRejectedMountAtStartupNotEnabledException(actionCode), innerException)
		{
			this.actionCode = actionCode;
		}

		protected AmDbActionRejectedMountAtStartupNotEnabledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.actionCode = (string)info.GetValue("actionCode", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("actionCode", this.actionCode);
		}

		public string ActionCode
		{
			get
			{
				return this.actionCode;
			}
		}

		private readonly string actionCode;
	}
}
