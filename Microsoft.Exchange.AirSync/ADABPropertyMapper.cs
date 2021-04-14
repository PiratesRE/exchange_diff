using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ABProviderFramework;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.AirSync
{
	internal static class ADABPropertyMapper
	{
		public static ADPropertyDefinition GetADPropertyDefinition(ABPropertyDefinition addressBookProperty)
		{
			ADPropertyDefinition result;
			if (ADABPropertyMapper.propertyMap.TryGetValue(addressBookProperty, out result))
			{
				return result;
			}
			return null;
		}

		internal static ADPropertyDefinition[] ConvertToADProperties(ABPropertyDefinitionCollection properties)
		{
			object nativePropertyCollection = properties.GetNativePropertyCollection("AD");
			if (nativePropertyCollection != null)
			{
				return (ADPropertyDefinition[])nativePropertyCollection;
			}
			List<ADPropertyDefinition> list = new List<ADPropertyDefinition>(properties.Count);
			foreach (ABPropertyDefinition addressBookProperty in properties)
			{
				ADABPropertyMapper.AddCorrespondingADPropertyToList(list, addressBookProperty);
			}
			ADPropertyDefinition[] array = list.ToArray();
			properties.SetNativePropertyCollection("AD", array);
			return array;
		}

		private static void AddCorrespondingADPropertyToList(List<ADPropertyDefinition> activeDirectoryProperties, ABPropertyDefinition addressBookProperty)
		{
			ADPropertyDefinition adpropertyDefinition = ADABPropertyMapper.GetADPropertyDefinition(addressBookProperty);
			if (adpropertyDefinition != null)
			{
				activeDirectoryProperties.Add(adpropertyDefinition);
			}
		}

		private static Dictionary<ABPropertyDefinition, ADPropertyDefinition> CreateMappingDictionary()
		{
			Dictionary<ABPropertyDefinition, ADPropertyDefinition> dictionary = new Dictionary<ABPropertyDefinition, ADPropertyDefinition>();
			foreach (ADABPropertyMapper.ABPropertyMapEntry abpropertyMapEntry in ADABPropertyMapper.propertyMapArray)
			{
				dictionary.Add(abpropertyMapEntry.AddressBookProperty, abpropertyMapEntry.ActiveDirectoryProperty);
			}
			return dictionary;
		}

		private static ADABPropertyMapper.ABPropertyMapEntry[] propertyMapArray = new ADABPropertyMapper.ABPropertyMapEntry[]
		{
			new ADABPropertyMapper.ABPropertyMapEntry(ABObjectSchema.Alias, ADRecipientSchema.Alias),
			new ADABPropertyMapper.ABPropertyMapEntry(ABObjectSchema.DisplayName, ADRecipientSchema.DisplayName),
			new ADABPropertyMapper.ABPropertyMapEntry(ABObjectSchema.LegacyExchangeDN, ADRecipientSchema.LegacyExchangeDN),
			new ADABPropertyMapper.ABPropertyMapEntry(ABObjectSchema.CanEmail, ADRecipientSchema.RecipientType),
			new ADABPropertyMapper.ABPropertyMapEntry(ABObjectSchema.Id, ADObjectSchema.Id),
			new ADABPropertyMapper.ABPropertyMapEntry(ABObjectSchema.EmailAddress, ADRecipientSchema.PrimarySmtpAddress),
			new ADABPropertyMapper.ABPropertyMapEntry(ABContactSchema.GivenName, ADOrgPersonSchema.FirstName),
			new ADABPropertyMapper.ABPropertyMapEntry(ABContactSchema.Surname, ADOrgPersonSchema.LastName),
			new ADABPropertyMapper.ABPropertyMapEntry(ABContactSchema.Initials, ADOrgPersonSchema.Initials),
			new ADABPropertyMapper.ABPropertyMapEntry(ABContactSchema.BusinessPhoneNumber, ADOrgPersonSchema.Phone),
			new ADABPropertyMapper.ABPropertyMapEntry(ABContactSchema.BusinessFaxNumber, ADOrgPersonSchema.Fax),
			new ADABPropertyMapper.ABPropertyMapEntry(ABContactSchema.OfficeLocation, ADOrgPersonSchema.Office),
			new ADABPropertyMapper.ABPropertyMapEntry(ABContactSchema.CompanyName, ADOrgPersonSchema.Company),
			new ADABPropertyMapper.ABPropertyMapEntry(ABContactSchema.DepartmentName, ADOrgPersonSchema.Department),
			new ADABPropertyMapper.ABPropertyMapEntry(ABContactSchema.HomePhoneNumber, ADOrgPersonSchema.HomePhone),
			new ADABPropertyMapper.ABPropertyMapEntry(ABContactSchema.MobilePhoneNumber, ADOrgPersonSchema.MobilePhone),
			new ADABPropertyMapper.ABPropertyMapEntry(ABContactSchema.Title, ADOrgPersonSchema.Title),
			new ADABPropertyMapper.ABPropertyMapEntry(ABContactSchema.WorkAddressCity, ADOrgPersonSchema.City),
			new ADABPropertyMapper.ABPropertyMapEntry(ABContactSchema.WorkAddressCountry, ADOrgPersonSchema.Co),
			new ADABPropertyMapper.ABPropertyMapEntry(ABContactSchema.WorkAddressPostalCode, ADOrgPersonSchema.PostalCode),
			new ADABPropertyMapper.ABPropertyMapEntry(ABContactSchema.WorkAddressPostOfficeBox, ADOrgPersonSchema.PostOfficeBox),
			new ADABPropertyMapper.ABPropertyMapEntry(ABContactSchema.WorkAddressState, ADOrgPersonSchema.StateOrProvince),
			new ADABPropertyMapper.ABPropertyMapEntry(ABContactSchema.WorkAddressStreet, ADOrgPersonSchema.StreetAddress),
			new ADABPropertyMapper.ABPropertyMapEntry(ABContactSchema.WebPage, ADRecipientSchema.WebPage),
			new ADABPropertyMapper.ABPropertyMapEntry(ABGroupSchema.HiddenMembership, ADGroupSchema.HiddenGroupMembershipEnabled),
			new ADABPropertyMapper.ABPropertyMapEntry(ABGroupSchema.OwnerId, ADGroupSchema.ManagedBy)
		};

		private static Dictionary<ABPropertyDefinition, ADPropertyDefinition> propertyMap = ADABPropertyMapper.CreateMappingDictionary();

		private struct ABPropertyMapEntry
		{
			public ABPropertyMapEntry(ABPropertyDefinition addressBookProperty, ADPropertyDefinition activeDirectoryProperty)
			{
				if (addressBookProperty == null)
				{
					throw new ArgumentNullException("addressBookProperty");
				}
				if (activeDirectoryProperty == null)
				{
					throw new ArgumentNullException("activeDirectoryProperty");
				}
				this.addressBookProperty = addressBookProperty;
				this.activeDirectoryProperty = activeDirectoryProperty;
			}

			public ABPropertyDefinition AddressBookProperty
			{
				get
				{
					return this.addressBookProperty;
				}
			}

			public ADPropertyDefinition ActiveDirectoryProperty
			{
				get
				{
					return this.activeDirectoryProperty;
				}
			}

			private ABPropertyDefinition addressBookProperty;

			private ADPropertyDefinition activeDirectoryProperty;
		}
	}
}
