using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class HeatMapNotBuiltException : MailboxLoadBalanceTransientException
	{
		public HeatMapNotBuiltException() : base(MigrationWorkflowServiceStrings.ErrorHeatMapNotBuilt)
		{
		}

		public HeatMapNotBuiltException(Exception innerException) : base(MigrationWorkflowServiceStrings.ErrorHeatMapNotBuilt, innerException)
		{
		}

		protected HeatMapNotBuiltException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
