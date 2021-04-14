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
	internal class SymphonyArgumentFaultException : MigrationTransientException
	{
		public SymphonyArgumentFaultException(string faultMessage) : base(UpgradeHandlerStrings.SymphonyArgumentFault(faultMessage))
		{
			this.faultMessage = faultMessage;
		}

		public SymphonyArgumentFaultException(string faultMessage, Exception innerException) : base(UpgradeHandlerStrings.SymphonyArgumentFault(faultMessage), innerException)
		{
			this.faultMessage = faultMessage;
		}

		protected SymphonyArgumentFaultException(SerializationInfo info, StreamingContext context) : base(info, context)
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
