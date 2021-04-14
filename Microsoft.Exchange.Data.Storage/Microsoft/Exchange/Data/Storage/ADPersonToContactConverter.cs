using System;
using System.Collections;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ADPersonToContactConverter
	{
		protected ADPersonToContactConverter(PropertyDefinition contactProperty, ADPropertyDefinition[] adProperties)
		{
			this.contactProperty = contactProperty;
			this.adProperties = adProperties;
		}

		public PropertyDefinition ContactProperty
		{
			get
			{
				return this.contactProperty;
			}
		}

		public ADPropertyDefinition[] ADProperties
		{
			get
			{
				return this.adProperties;
			}
		}

		public abstract void Convert(ADRawEntry adObject, IStorePropertyBag contact);

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder((this.adProperties.Length + 1) * 30);
			stringBuilder.Append("Type=");
			stringBuilder.Append(base.GetType().Name);
			stringBuilder.Append(";ContactProperty=");
			stringBuilder.Append(this.ContactProperty.Name);
			if (this.adProperties.Length > 0)
			{
				stringBuilder.Append(";ADProperties=");
				stringBuilder.Append(this.adProperties[0].Name);
				for (int i = 1; i < this.adProperties.Length; i++)
				{
					stringBuilder.Append(",");
					stringBuilder.Append(this.adProperties[i].Name);
				}
			}
			return stringBuilder.ToString();
		}

		public static string GetSipUri(ADRawEntry adEntry)
		{
			string text = null;
			object obj;
			if (adEntry.TryGetValueWithoutDefault(ADUserSchema.RTCSIPPrimaryUserAddress, out obj))
			{
				text = (obj as string);
			}
			object obj2;
			if (string.IsNullOrWhiteSpace(text) && adEntry.TryGetValueWithoutDefault(ADRecipientSchema.EmailAddresses, out obj2))
			{
				ProxyAddressCollection proxyAddressCollection = obj2 as ProxyAddressCollection;
				if (proxyAddressCollection != null)
				{
					text = proxyAddressCollection.GetSipUri();
				}
			}
			return text;
		}

		public static readonly ADPersonToContactConverter PersonType = new ADPersonToContactConverter.DirectPropertyConverter(ContactSchema.PersonType, ADRecipientSchema.RecipientPersonType);

		public static readonly ADPersonToContactConverter DisplayName = new ADPersonToContactConverter.DisplayNameConverter();

		public static readonly ADPersonToContactConverter Surname = new ADPersonToContactConverter.DirectPropertyConverter(ContactSchema.Surname, ADOrgPersonSchema.LastName);

		public static readonly ADPersonToContactConverter GivenName = new ADPersonToContactConverter.DirectPropertyConverter(ContactSchema.GivenName, ADOrgPersonSchema.FirstName);

		public static readonly ADPersonToContactConverter FileAs = new ADPersonToContactConverter.DirectPropertyConverter(ContactBaseSchema.FileAs, ADRecipientSchema.SimpleDisplayName);

		public static readonly ADPersonToContactConverter EmailAddress = new ADPersonToContactConverter.SmtpAddressConverter(ContactSchema.Email1EmailAddress);

		public static readonly ADPersonToContactConverter BusinessHomePage = new ADPersonToContactConverter.DirectPropertyConverter(ContactSchema.BusinessHomePage, ADRecipientSchema.WebPage);

		public static readonly ADPersonToContactConverter BusinessPhoneNumber = new ADPersonToContactConverter.DirectPropertyConverter(ContactSchema.BusinessPhoneNumber, ADOrgPersonSchema.Phone);

		public static readonly ADPersonToContactConverter BusinessPhoneNumber2 = new ADPersonToContactConverter.MultiValueToSigleValuePropertyConverter(ContactSchema.BusinessPhoneNumber2, ADOrgPersonSchema.OtherTelephone);

		public static readonly ADPersonToContactConverter MobilePhone = new ADPersonToContactConverter.DirectPropertyConverter(ContactSchema.MobilePhone, ADOrgPersonSchema.MobilePhone);

		public static readonly ADPersonToContactConverter HomePhone = new ADPersonToContactConverter.DirectPropertyConverter(ContactSchema.HomePhone, ADOrgPersonSchema.HomePhone);

		public static readonly ADPersonToContactConverter HomePhone2 = new ADPersonToContactConverter.MultiValueToSigleValuePropertyConverter(ContactSchema.HomePhone2, ADOrgPersonSchema.OtherHomePhone);

		public static readonly ADPersonToContactConverter CompanyName = new ADPersonToContactConverter.DirectPropertyConverter(ContactSchema.CompanyName, ADOrgPersonSchema.Company);

		public static readonly ADPersonToContactConverter Title = new ADPersonToContactConverter.DirectPropertyConverter(ContactSchema.Title, ADOrgPersonSchema.Title);

		public static readonly ADPersonToContactConverter Department = new ADPersonToContactConverter.DirectPropertyConverter(ContactSchema.Department, ADOrgPersonSchema.Department);

		public static readonly ADPersonToContactConverter OfficeLocation = new ADPersonToContactConverter.DirectPropertyConverter(ContactSchema.OfficeLocation, ADOrgPersonSchema.Office);

		public static readonly ADPersonToContactConverter PostOfficeBox = new ADPersonToContactConverter.MultiValueToSigleValuePropertyConverter(ContactSchema.WorkPostOfficeBox, ADOrgPersonSchema.PostOfficeBox);

		public static readonly ADPersonToContactConverter Street = new ADPersonToContactConverter.DirectPropertyConverter(ContactSchema.WorkAddressStreet, ADOrgPersonSchema.StreetAddress);

		public static readonly ADPersonToContactConverter Country = new ADPersonToContactConverter.CountryValueConverter();

		public static readonly ADPersonToContactConverter PostalCode = new ADPersonToContactConverter.DirectPropertyConverter(ContactSchema.WorkAddressPostalCode, ADOrgPersonSchema.PostalCode);

		public static readonly ADPersonToContactConverter State = new ADPersonToContactConverter.DirectPropertyConverter(ContactSchema.WorkAddressState, ADOrgPersonSchema.StateOrProvince);

		public static readonly ADPersonToContactConverter City = new ADPersonToContactConverter.DirectPropertyConverter(ContactSchema.WorkAddressCity, ADOrgPersonSchema.City);

		public static readonly ADPersonToContactConverter Fax = new ADPersonToContactConverter.DirectPropertyConverter(ContactSchema.WorkFax, ADOrgPersonSchema.Fax);

		public static readonly ADPersonToContactConverter AssistantName = new ADPersonToContactConverter.DirectPropertyConverter(ContactSchema.AssistantName, ADRecipientSchema.AssistantName);

		public static readonly ADPersonToContactConverter YomiCompany = new ADPersonToContactConverter.DirectPropertyConverter(ContactSchema.YomiCompany, ADRecipientSchema.PhoneticCompany);

		public static readonly ADPersonToContactConverter YomiFirstName = new ADPersonToContactConverter.DirectPropertyConverter(ContactSchema.YomiFirstName, ADRecipientSchema.PhoneticFirstName);

		public static readonly ADPersonToContactConverter YomiLastName = new ADPersonToContactConverter.DirectPropertyConverter(ContactSchema.YomiLastName, ADRecipientSchema.PhoneticLastName);

		public static readonly ADPersonToContactConverter IMAddress = new ADPersonToContactConverter.IMAddressConverter();

		public static readonly ADPersonToContactConverter HomeLocationSource = new ADPersonToContactConverter.GeoCoordinatesConverter();

		public static readonly ADPersonToContactConverter RecipientType = new ADPersonToContactConverter.RecipientTypeConverter();

		public static readonly ADPersonToContactConverter[] AllConverters = new ADPersonToContactConverter[]
		{
			ADPersonToContactConverter.PersonType,
			ADPersonToContactConverter.DisplayName,
			ADPersonToContactConverter.Surname,
			ADPersonToContactConverter.GivenName,
			ADPersonToContactConverter.FileAs,
			ADPersonToContactConverter.EmailAddress,
			ADPersonToContactConverter.IMAddress,
			ADPersonToContactConverter.BusinessHomePage,
			ADPersonToContactConverter.BusinessPhoneNumber,
			ADPersonToContactConverter.BusinessPhoneNumber2,
			ADPersonToContactConverter.MobilePhone,
			ADPersonToContactConverter.HomePhone,
			ADPersonToContactConverter.HomePhone2,
			ADPersonToContactConverter.CompanyName,
			ADPersonToContactConverter.Title,
			ADPersonToContactConverter.Department,
			ADPersonToContactConverter.OfficeLocation,
			ADPersonToContactConverter.PostOfficeBox,
			ADPersonToContactConverter.Street,
			ADPersonToContactConverter.Country,
			ADPersonToContactConverter.PostalCode,
			ADPersonToContactConverter.State,
			ADPersonToContactConverter.City,
			ADPersonToContactConverter.Fax,
			ADPersonToContactConverter.AssistantName,
			ADPersonToContactConverter.YomiCompany,
			ADPersonToContactConverter.YomiFirstName,
			ADPersonToContactConverter.YomiLastName,
			ADPersonToContactConverter.HomeLocationSource,
			ADPersonToContactConverter.RecipientType
		};

		private static readonly Trace Tracer = ExTraceGlobals.ContactLinkingTracer;

		private readonly PropertyDefinition contactProperty;

		private readonly ADPropertyDefinition[] adProperties;

		private abstract class SinglePropertyConverter : ADPersonToContactConverter
		{
			public SinglePropertyConverter(PropertyDefinition contactProperty, ADPropertyDefinition adProperty) : base(contactProperty, new ADPropertyDefinition[]
			{
				adProperty
			})
			{
			}

			public ADPropertyDefinition ADProperty
			{
				get
				{
					return this.adProperties[0];
				}
			}
		}

		private sealed class DirectPropertyConverter : ADPersonToContactConverter.SinglePropertyConverter
		{
			public DirectPropertyConverter(PropertyDefinition contactProperty, ADPropertyDefinition adProperty) : base(contactProperty, adProperty)
			{
			}

			public override void Convert(ADRawEntry adObject, IStorePropertyBag contact)
			{
				Util.ThrowOnNullArgument(adObject, "adObject");
				Util.ThrowOnNullArgument(contact, "contact");
				object obj;
				if (adObject.TryGetValueWithoutDefault(base.ADProperty, out obj))
				{
					contact[base.ContactProperty] = obj;
					ADPersonToContactConverter.Tracer.TraceDebug<string, string, object>(0L, "Setting contact property {0} with value from AD property {1}. Value: {2}", base.ADProperty.Name, base.ContactProperty.Name, obj);
					return;
				}
				ADPersonToContactConverter.Tracer.TraceDebug<string, string>(0L, "Deleting contact property {0} since AD property {1} not found.", base.ContactProperty.Name, base.ADProperty.Name);
				contact.Delete(base.ContactProperty);
			}
		}

		private sealed class DisplayNameConverter : ADPersonToContactConverter.SinglePropertyConverter
		{
			public DisplayNameConverter() : base(StoreObjectSchema.DisplayName, ADRecipientSchema.DisplayName)
			{
			}

			public override void Convert(ADRawEntry adObject, IStorePropertyBag contact)
			{
				Util.ThrowOnNullArgument(adObject, "adObject");
				Util.ThrowOnNullArgument(contact, "contact");
				object value;
				if (adObject.TryGetValueWithoutDefault(base.ADProperty, out value))
				{
					contact[StoreObjectSchema.DisplayName] = (contact[ContactBaseSchema.DisplayNameFirstLast] = (contact[ContactBaseSchema.DisplayNameLastFirst] = value));
					return;
				}
				ADPersonToContactConverter.Tracer.TraceDebug<string>(0L, "Deleting contact DisplayName properties since AD property {0} not found.", base.ADProperty.Name);
				contact.Delete(ContactBaseSchema.DisplayNameFirstLast);
				contact.Delete(ContactBaseSchema.DisplayNameLastFirst);
				contact.Delete(StoreObjectSchema.DisplayName);
			}
		}

		private sealed class SmtpAddressConverter : ADPersonToContactConverter.SinglePropertyConverter
		{
			public SmtpAddressConverter(PropertyDefinition contactProperty) : base(contactProperty, ADRecipientSchema.EmailAddresses)
			{
			}

			public override void Convert(ADRawEntry adObject, IStorePropertyBag contact)
			{
				Util.ThrowOnNullArgument(adObject, "adObject");
				Util.ThrowOnNullArgument(contact, "contact");
				string text = null;
				object obj;
				if (adObject.TryGetValueWithoutDefault(ADRecipientSchema.EmailAddresses, out obj))
				{
					ProxyAddressCollection proxyAddressCollection = obj as ProxyAddressCollection;
					if (proxyAddressCollection != null)
					{
						foreach (ProxyAddress proxyAddress in proxyAddressCollection)
						{
							SmtpProxyAddress smtpProxyAddress = proxyAddress as SmtpProxyAddress;
							if (smtpProxyAddress != null)
							{
								if (smtpProxyAddress.IsPrimaryAddress)
								{
									text = smtpProxyAddress.SmtpAddress;
									break;
								}
								if (text == null)
								{
									text = smtpProxyAddress.SmtpAddress;
								}
							}
						}
					}
				}
				if (text != null)
				{
					contact[base.ContactProperty] = text;
					return;
				}
				ADPersonToContactConverter.Tracer.TraceDebug<string>(0L, "Deleting contact property {0} since AD Object has no email addresses.", base.ContactProperty.Name);
				contact.Delete(base.ContactProperty);
			}
		}

		private sealed class IMAddressConverter : ADPersonToContactConverter
		{
			public IMAddressConverter() : base(ContactSchema.IMAddress, ADPersonToContactConverter.IMAddressConverter.requiredAdProperties)
			{
			}

			public override void Convert(ADRawEntry adObject, IStorePropertyBag contact)
			{
				Util.ThrowOnNullArgument(adObject, "adObject");
				Util.ThrowOnNullArgument(contact, "contact");
				string sipUri = ADPersonToContactConverter.GetSipUri(adObject);
				if (sipUri != null)
				{
					contact[base.ContactProperty] = sipUri;
					return;
				}
				ADPersonToContactConverter.Tracer.TraceDebug<string>(0L, "Deleting contact property {0} since AD Object has no SIP addresses.", base.ContactProperty.Name);
				contact.Delete(base.ContactProperty);
			}

			private static readonly ADPropertyDefinition[] requiredAdProperties = new ADPropertyDefinition[]
			{
				ADUserSchema.RTCSIPPrimaryUserAddress,
				ADRecipientSchema.EmailAddresses
			};
		}

		private sealed class RecipientTypeConverter : ADPersonToContactConverter.SinglePropertyConverter
		{
			public RecipientTypeConverter() : base(InternalSchema.InternalPersonType, ADRecipientSchema.RecipientPersonType)
			{
			}

			public override void Convert(ADRawEntry adObject, IStorePropertyBag contact)
			{
				Util.ThrowOnNullArgument(adObject, "adObject");
				Util.ThrowOnNullArgument(contact, "contact");
				PersonType personType = (PersonType)adObject[ADRecipientSchema.RecipientPersonType];
				contact[InternalSchema.InternalPersonType] = (int)personType;
			}
		}

		private sealed class GeoCoordinatesConverter : ADPersonToContactConverter
		{
			public GeoCoordinatesConverter() : base(ContactSchema.HomeLocationSource, ADPersonToContactConverter.GeoCoordinatesConverter.RequiredAdProperties)
			{
			}

			public override void Convert(ADRawEntry adObject, IStorePropertyBag contact)
			{
				Util.ThrowOnNullArgument(adObject, "adObject");
				Util.ThrowOnNullArgument(contact, "contact");
				object obj;
				if (adObject.TryGetValueWithoutDefault(ADRecipientSchema.GeoCoordinates, out obj))
				{
					GeoCoordinates geoCoordinates = (GeoCoordinates)obj;
					contact[ContactSchema.HomeLatitude] = geoCoordinates.Latitude;
					contact[ContactSchema.HomeLongitude] = geoCoordinates.Longitude;
					contact[ContactSchema.HomeLocationSource] = LocationSource.Contact;
					if (geoCoordinates.Altitude != null)
					{
						contact[ContactSchema.HomeAltitude] = geoCoordinates.Altitude;
					}
					else
					{
						ADPersonToContactConverter.Tracer.TraceDebug(0L, "Deleting contact HomeAltitude property it is not found on AD GeoCoordinates property.");
						contact.Delete(ContactSchema.HomeAltitude);
					}
					ADPersonToContactConverter.GeoCoordinatesConverter.locationUriConverter.Convert(adObject, contact);
					return;
				}
				ADPersonToContactConverter.Tracer.TraceDebug(0L, "Deleting contact location properties since AD GeoCoordinates property not found.");
				contact.Delete(ContactSchema.HomeLatitude);
				contact.Delete(ContactSchema.HomeLongitude);
				contact.Delete(ContactSchema.HomeLocationSource);
				contact.Delete(ContactSchema.HomeAltitude);
				ADPersonToContactConverter.GeoCoordinatesConverter.locationUriConverter.Convert(adObject, contact);
			}

			private static readonly ADPropertyDefinition[] RequiredAdProperties = new ADPropertyDefinition[]
			{
				ADRecipientSchema.Latitude,
				ADRecipientSchema.Longitude,
				ADRecipientSchema.Altitude,
				ADRecipientSchema.PrimarySmtpAddress
			};

			private static readonly ADPersonToContactConverter.SmtpAddressConverter locationUriConverter = new ADPersonToContactConverter.SmtpAddressConverter(ContactSchema.HomeLocationUri);
		}

		private sealed class MultiValueToSigleValuePropertyConverter : ADPersonToContactConverter.SinglePropertyConverter
		{
			public MultiValueToSigleValuePropertyConverter(PropertyDefinition contactProperty, ADPropertyDefinition adProperty) : base(contactProperty, adProperty)
			{
			}

			public override void Convert(ADRawEntry adObject, IStorePropertyBag contact)
			{
				Util.ThrowOnNullArgument(adObject, "adObject");
				Util.ThrowOnNullArgument(contact, "contact");
				object obj = null;
				object obj2;
				if (adObject.TryGetValueWithoutDefault(base.ADProperty, out obj2))
				{
					IList list = obj2 as IList;
					if (list != null && list.Count > 0)
					{
						obj = list[0];
					}
				}
				if (obj != null)
				{
					contact[base.ContactProperty] = obj;
					ADPersonToContactConverter.Tracer.TraceDebug<string, string, object>(0L, "Setting contact property {0} with value from AD property {1}. Value: {2}", base.ADProperty.Name, base.ContactProperty.Name, obj);
					return;
				}
				ADPersonToContactConverter.Tracer.TraceDebug<string, string>(0L, "Deleting contact property {0} since AD property {1} not found.", base.ContactProperty.Name, base.ADProperty.Name);
				contact.Delete(base.ContactProperty);
			}
		}

		private sealed class CountryValueConverter : ADPersonToContactConverter.SinglePropertyConverter
		{
			public CountryValueConverter() : base(ContactSchema.WorkAddressCountry, ADOrgPersonSchema.CountryOrRegion)
			{
			}

			public override void Convert(ADRawEntry adObject, IStorePropertyBag contact)
			{
				object obj;
				if (adObject.TryGetValueWithoutDefault(base.ADProperty, out obj))
				{
					CountryInfo countryInfo = (CountryInfo)obj;
					contact[base.ContactProperty] = countryInfo.LocalizedDisplayName.ToString();
					return;
				}
				contact.Delete(base.ContactProperty);
			}
		}
	}
}
