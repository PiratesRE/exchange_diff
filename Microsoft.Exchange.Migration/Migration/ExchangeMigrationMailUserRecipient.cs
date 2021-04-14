using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Migration
{
	internal sealed class ExchangeMigrationMailUserRecipient : ExchangeMigrationRecipientWithHomeServer
	{
		public ExchangeMigrationMailUserRecipient() : base(MigrationUserRecipientType.Mailbox)
		{
		}

		public override HashSet<PropTag> SupportedProperties
		{
			get
			{
				return ExchangeMigrationMailUserRecipient.supportedProperties;
			}
		}

		public override HashSet<PropTag> RequiredProperties
		{
			get
			{
				return ExchangeMigrationMailUserRecipient.requiredProperties;
			}
		}

		public override bool TryValidateRequiredProperties(out LocalizedString errorMessage)
		{
			if (!base.TryValidateRequiredProperties(out errorMessage))
			{
				return false;
			}
			if (base.IsPropertyRequired((PropTag)2147876895U) && string.IsNullOrEmpty(base.MsExchHomeServerName))
			{
				errorMessage = ServerStrings.MigrationNSPIMissingRequiredField((PropTag)2147876895U);
				return false;
			}
			return true;
		}

		protected override void UpdateDependentProperties(PropTag proptag)
		{
			base.UpdateDependentProperties(proptag);
			if (proptag == (PropTag)2147876895U)
			{
				string propertyValue = base.GetPropertyValue<string>((PropTag)2147876895U);
				if (!string.IsNullOrEmpty(propertyValue))
				{
					int num = propertyValue.IndexOf("/cn=Microsoft Private MDB", StringComparison.OrdinalIgnoreCase);
					base.MsExchHomeServerName = ((num >= 0) ? propertyValue.Substring(0, num) : null);
				}
			}
		}

		public const string HomeMDBSuffix = "/cn=Microsoft Private MDB";

		private static HashSet<PropTag> supportedProperties = new HashSet<PropTag>(new PropTag[]
		{
			PropTag.DisplayType,
			PropTag.DisplayTypeEx,
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
			(PropTag)2147876895U,
			(PropTag)2147811359U,
			(PropTag)2148864031U,
			(PropTag)134676483U,
			(PropTag)2361524482U,
			(PropTag)2154364959U,
			(PropTag)2359230495U
		});

		private static HashSet<PropTag> requiredProperties = new HashSet<PropTag>(new PropTag[]
		{
			PropTag.DisplayType,
			PropTag.EmailAddress,
			PropTag.SmtpAddress,
			PropTag.DisplayName,
			(PropTag)2147876895U
		});
	}
}
