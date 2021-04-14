using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Notifications.Broker
{
	[KnownType(typeof(NotificationParticipantLocationKind))]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[Serializable]
	public class NotificationParticipant
	{
		[DataMember(EmitDefaultValue = false)]
		public NotificationParticipantLocationKind LocationKind { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public OrganizationId OrganizationId { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public Guid ExternalDirectoryOrganizationId { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public Guid DatabaseGuid { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public Guid MailboxGuid { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string MailboxSmtp { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string FrontEndUrl { get; set; }

		internal NotificationParticipant AsNotificationSender()
		{
			NotificationParticipant notificationParticipant = new NotificationParticipant
			{
				DatabaseGuid = this.DatabaseGuid,
				MailboxGuid = this.MailboxGuid,
				MailboxSmtp = this.MailboxSmtp
			};
			if (this.OrganizationId != OrganizationId.ForestWideOrgId)
			{
				notificationParticipant.ExternalDirectoryOrganizationId = new Guid(this.OrganizationId.ToExternalDirectoryOrganizationId());
			}
			return notificationParticipant;
		}

		internal NotificationParticipant AsNotificationReceiver()
		{
			return new NotificationParticipant
			{
				MailboxGuid = this.MailboxGuid,
				MailboxSmtp = this.MailboxSmtp,
				FrontEndUrl = this.FrontEndUrl
			};
		}
	}
}
