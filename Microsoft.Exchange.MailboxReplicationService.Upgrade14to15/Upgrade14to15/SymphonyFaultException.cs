using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class SymphonyFaultException : MigrationTransientException
	{
		public SymphonyFaultException(string faultMessage) : base(UpgradeHandlerStrings.SymphonyFault(faultMessage))
		{
			this.faultMessage = faultMessage;
		}

		public SymphonyFaultException(string faultMessage, Exception innerException) : base(UpgradeHandlerStrings.SymphonyFault(faultMessage), innerException)
		{
			this.faultMessage = faultMessage;
		}

		protected SymphonyFaultException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.faultMessage = (string)info.GetValue("faultMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("faultMessage", this.faultMessage);
		}

		public string FaultMessage
		{
			get
			{
				return this.faultMessage;
			}
		}

		private readonly string faultMessage;
	}
}
