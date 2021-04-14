using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidMailboxContainerGuidException : MailboxReplicationPermanentException
	{
		public InvalidMailboxContainerGuidException(string specifiedMailboxContainerGuid, string linkedMailboxContainerGuid) : base(Strings.InvalidMailboxContainerGuid(specifiedMailboxContainerGuid, linkedMailboxContainerGuid))
		{
			this.specifiedMailboxContainerGuid = specifiedMailboxContainerGuid;
			this.linkedMailboxContainerGuid = linkedMailboxContainerGuid;
		}

		public InvalidMailboxContainerGuidException(string specifiedMailboxContainerGuid, string linkedMailboxContainerGuid, Exception innerException) : base(Strings.InvalidMailboxContainerGuid(specifiedMailboxContainerGuid, linkedMailboxContainerGuid), innerException)
		{
			this.specifiedMailboxContainerGuid = specifiedMailboxContainerGuid;
			this.linkedMailboxContainerGuid = linkedMailboxContainerGuid;
		}

		protected InvalidMailboxContainerGuidException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.specifiedMailboxContainerGuid = (string)info.GetValue("specifiedMailboxContainerGuid", typeof(string));
			this.linkedMailboxContainerGuid = (string)info.GetValue("linkedMailboxContainerGuid", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("specifiedMailboxContainerGuid", this.specifiedMailboxContainerGuid);
			info.AddValue("linkedMailboxContainerGuid", this.linkedMailboxContainerGuid);
		}

		public string SpecifiedMailboxContainerGuid
		{
			get
			{
				return this.specifiedMailboxContainerGuid;
			}
		}

		public string LinkedMailboxContainerGuid
		{
			get
			{
				return this.linkedMailboxContainerGuid;
			}
		}

		private readonly string specifiedMailboxContainerGuid;

		private readonly string linkedMailboxContainerGuid;
	}
}
