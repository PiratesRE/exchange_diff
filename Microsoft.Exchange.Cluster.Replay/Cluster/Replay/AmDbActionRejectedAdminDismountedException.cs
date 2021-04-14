using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDbActionRejectedAdminDismountedException : AmCommonException
	{
		public AmDbActionRejectedAdminDismountedException(string actionCode) : base(ReplayStrings.AmDbActionRejectedAdminDismountedException(actionCode))
		{
			this.actionCode = actionCode;
		}

		public AmDbActionRejectedAdminDismountedException(string actionCode, Exception innerException) : base(ReplayStrings.AmDbActionRejectedAdminDismountedException(actionCode), innerException)
		{
			this.actionCode = actionCode;
		}

		protected AmDbActionRejectedAdminDismountedException(SerializationInfo info, StreamingContext context) : base(info, context)
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
