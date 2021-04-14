using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDbActionTransientException : AmCommonTransientException
	{
		public AmDbActionTransientException(string actionError) : base(ReplayStrings.AmDbActionTransientException(actionError))
		{
			this.actionError = actionError;
		}

		public AmDbActionTransientException(string actionError, Exception innerException) : base(ReplayStrings.AmDbActionTransientException(actionError), innerException)
		{
			this.actionError = actionError;
		}

		protected AmDbActionTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
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
