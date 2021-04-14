using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Entities.People.Utilities
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class GALContactsFolderSchema
	{
		internal static readonly PropertyDefinition[] ContactPropertyDefinitions = new PropertyDefinition[]
		{
			ContactSchema.PersonType,
			StoreObjectSchema.LastModifiedTime,
			ContactSchema.BusinessPhoneNumber,
			ContactSchema.MobilePhone,
			ContactSchema.HomePhone,
			ContactSchema.GALLinkID,
			ContactSchema.PersonId,
			ContactSchema.PartnerNetworkId,
			StoreObjectSchema.DisplayName,
			ContactBaseSchema.DisplayNameFirstLast,
			ContactBaseSchema.DisplayNameLastFirst,
			ContactSchema.Email1EmailAddress,
			ContactSchema.Email1AddrType,
			ContactBaseSchema.FileAs,
			ContactSchema.GivenName,
			ContactSchema.MiddleName,
			ContactSchema.Surname,
			ContactSchema.Nickname,
			ContactSchema.YomiCompany,
			ContactSchema.YomiFirstName,
			ContactSchema.YomiLastName,
			ContactSchema.Title,
			ContactSchema.Department,
			ContactSchema.CompanyName,
			ContactSchema.Location,
			ContactSchema.HomeCity,
			ContactSchema.IMAddress,
			ContactSchema.RelevanceScore,
			ContactSchema.OfficeLocation,
			ContactSchema.WorkFax,
			ContactSchema.Manager,
			ContactSchema.WorkAddressStreet,
			ContactSchema.WorkAddressCity,
			ContactSchema.WorkAddressState,
			ContactSchema.WorkAddressCountry,
			ContactSchema.WorkAddressPostalCode,
			ContactSchema.OtherStreet,
			ContactSchema.OtherState,
			ContactSchema.OtherPostalCode,
			ContactSchema.OtherPostOfficeBox,
			ContactSchema.BusinessHomePage
		};
	}
}
