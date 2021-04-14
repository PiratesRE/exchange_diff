using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDbActionRejectedLastAdminActionDidNotSucceedException : AmCommonException
	{
		public AmDbActionRejectedLastAdminActionDidNotSucceedException(string actionCode) : base(ReplayStrings.AmDbActionRejectedLastAdminActionDidNotSucceedException(actionCode))
		{
			this.actionCode = actionCode;
		}

		public AmDbActionRejectedLastAdminActionDidNotSucceedException(string actionCode, Exception innerException) : base(ReplayStrings.AmDbActionRejectedLastAdminActionDidNotSucceedException(actionCode), innerException)
		{
			this.actionCode = actionCode;
		}

		protected AmDbActionRejectedLastAdminActionDidNotSucceedException(SerializationInfo info, StreamingContext context) : base(info, context)
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
