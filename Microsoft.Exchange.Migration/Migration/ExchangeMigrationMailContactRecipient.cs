using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Migration
{
	internal sealed class ExchangeMigrationMailContactRecipient : ExchangeMigrationRecipient
	{
		public ExchangeMigrationMailContactRecipient() : base(MigrationUserRecipientType.Contact)
		{
		}

		public override HashSet<PropTag> SupportedProperties
		{
			get
			{
				return ExchangeMigrationMailContactRecipient.supportedProperties;
			}
		}

		public override HashSet<PropTag> RequiredProperties
		{
			get
			{
				return ExchangeMigrationMailContactRecipient.requiredProperties;
			}
		}

		private static HashSet<PropTag> supportedProperties = new HashSet<PropTag>(new PropTag[]
		{
			PropTag.DisplayType,
			PropTag.PrimaryFaxNumber,
			PropTag.BusinessTelephoneNumber,
			PropTag.CompanyName,
			PropTag.DepartmentName,
			PropTag.DisplayName,
			PropTag.EmailAddress,
			PropTag.GivenName,
			PropTag.Initials,
			PropTag.MobileTelephoneNumber,
			PropTag.OfficeLocation,
			PropTag.SmtpAddress,
			PropTag.Surname,
			PropTag.Title,
			(PropTag)2148470815U,
			PropTag.HomeTelephoneNumber,
			PropTag.StreetAddress,
			PropTag.Locality,
			PropTag.StateOrProvince,
			PropTag.PostalCode,
			PropTag.Comment,
			PropTag.Account,
			(PropTag)2154364959U,
			(PropTag)2147811359U,
			(PropTag)2148864031U
		});

		private static HashSet<PropTag> requiredProperties = new HashSet<PropTag>(new PropTag[]
		{
			PropTag.DisplayType,
			PropTag.EmailAddress,
			PropTag.SmtpAddress,
			PropTag.DisplayName
		});
	}
}
