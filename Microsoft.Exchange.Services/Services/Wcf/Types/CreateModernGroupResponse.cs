using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Office.Server.Directory;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Name = "CreateModernGroupResponse", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CreateModernGroupResponse : BaseJsonResponse
	{
		internal CreateModernGroupResponse(Group group, string warningMessage = null)
		{
			Guid guid = (Guid)group.Properties["ExchangeDirectoryObjectId"];
			string text = (string)group.Properties["Mail"];
			this.Persona = new Persona
			{
				PersonaId = IdConverter.PersonaIdFromADObjectId(guid),
				ADObjectId = guid,
				DisplayName = group.DisplayName,
				Alias = group.Alias,
				EmailAddress = new EmailAddressWrapper
				{
					EmailAddress = text,
					MailboxType = MailboxHelper.MailboxTypeType.GroupMailbox.ToString(),
					Name = text,
					RoutingType = "SMTP"
				},
				PersonaType = PersonaTypeConverter.ToString(PersonType.ModernGroup)
			};
			this.Warning = warningMessage;
		}

		internal CreateModernGroupResponse(GroupMailbox groupMailbox)
		{
			this.Persona = new Persona
			{
				PersonaId = IdConverter.PersonaIdFromADObjectId(groupMailbox.Guid),
				ADObjectId = groupMailbox.Guid,
				DisplayName = groupMailbox.DisplayName,
				Alias = groupMailbox.Alias,
				EmailAddress = new EmailAddressWrapper
				{
					EmailAddress = groupMailbox.PrimarySmtpAddress.ToString(),
					MailboxType = MailboxHelper.MailboxTypeType.GroupMailbox.ToString(),
					Name = groupMailbox.DisplayName,
					RoutingType = "SMTP"
				},
				PersonaType = PersonaTypeConverter.ToString(PersonType.ModernGroup)
			};
		}

		internal CreateModernGroupResponse(string errorMessage)
		{
			this.Error = errorMessage;
		}

		[DataMember(Name = "Persona", IsRequired = false)]
		public Persona Persona { get; set; }

		[DataMember(Name = "Error", IsRequired = false)]
		public string Error { get; set; }

		[DataMember(Name = "Warning", IsRequired = false)]
		public string Warning { get; set; }

		[DataMember(Name = "Logs", IsRequired = false)]
		public ModernGroupCreateLogEntry[] Logs { get; set; }
	}
}
