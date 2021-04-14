using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ContainerCannotReceiveLoadException : MailboxLoadBalancePermanentException
	{
		public ContainerCannotReceiveLoadException(string containerGuid) : base(MigrationWorkflowServiceStrings.ErrorContainerCannotTakeLoad(containerGuid))
		{
			this.containerGuid = containerGuid;
		}

		public ContainerCannotReceiveLoadException(string containerGuid, Exception innerException) : base(MigrationWorkflowServiceStrings.ErrorContainerCannotTakeLoad(containerGuid), innerException)
		{
			this.containerGuid = containerGuid;
		}

		protected ContainerCannotReceiveLoadException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.containerGuid = (string)info.GetValue("containerGuid", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("containerGuid", this.containerGuid);
		}

		public string ContainerGuid
		{
			get
			{
				return this.containerGuid;
			}
		}

		private readonly string containerGuid;
	}
}
