using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDbActionException : AmCommonException
	{
		public AmDbActionException(string actionError) : base(ReplayStrings.AmDbActionException(actionError))
		{
			this.actionError = actionError;
		}

		public AmDbActionException(string actionError, Exception innerException) : base(ReplayStrings.AmDbActionException(actionError), innerException)
		{
			this.actionError = actionError;
		}

		protected AmDbActionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.actionError = (string)info.GetValue("actionError", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("actionError", this.actionError);
		}

		public string ActionError
		{
			get
			{
				return this.actionError;
			}
		}

		private readonly string actionError;
	}
}
