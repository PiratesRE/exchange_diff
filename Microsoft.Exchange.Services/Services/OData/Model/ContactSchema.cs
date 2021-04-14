using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.OData.Edm.Library;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal class ContactSchema : ItemSchema
	{
		public new static ContactSchema SchemaInstance
		{
			get
			{
				return ContactSchema.ContactSchemaInstance.Member;
			}
		}

		public override EdmEntityType EdmEntityType
		{
			get
			{
				return Contact.EdmEntityType;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> DeclaredProperties
		{
			get
			{
				return ContactSchema.DeclaredContactProperties;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> AllProperties
		{
			get
			{
				return ContactSchema.AllContactProperties;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> DefaultProperties
		{
			get
			{
				return ContactSchema.DefaultContactProperties;
			}
		}

		public override ReadOnlyCollection<PropertyDefinition> MandatoryCreationProperties
		{
			get
			{
				return ContactSchema.MandatoryContactCreationProperties;
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static ContactSchema()
		{
			PropertyDefinition propertyDefinition = new PropertyDefinition("ParentFolderId", typeof(string));
			propertyDefinition.EdmType = EdmCoreModel.Instance.GetString(true);
			PropertyDefinition propertyDefinition2 = propertyDefinition;
			SimpleEwsPropertyProvider simpleEwsPropertyProvider = new SimpleEwsPropertyProvider(ItemSchema.ParentFolderId);
			simpleEwsPropertyProvider.Getter = delegate(Entity e, PropertyDefinition ep, ServiceObject s, PropertyInformation sp)
			{
				FolderId folderId = s[sp] as FolderId;
				if (folderId != null && !string.IsNullOrEmpty(folderId.Id))
				{
					e[ep] = EwsIdConverter.EwsIdToODataId(folderId.Id);
				}
			};
			propertyDefinition2.EwsPropertyProvider = simpleEwsPropertyProvider;
			ContactSchema.ParentFolderId = propertyDefinition;
			ContactSchema.Birthday = new PropertyDefinition("Birthday", typeof(DateTime))
			{
				EdmType = EdmCoreModel.Instance.GetDateTimeOffset(true),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new DateTimePropertyProvider(ContactSchema.Birthday)
			};
			ContactSchema.FileAs = new PropertyDefinition("FileAs", typeof(string))
			{
				EdmType = EdmCoreModel.Instance.GetString(true),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new SimpleEwsPropertyProvider(ContactSchema.FileAs)
			};
			ContactSchema.DisplayName = new PropertyDefinition("DisplayName", typeof(string))
			{
				EdmType = EdmCoreModel.Instance.GetString(true),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new SimpleEwsPropertyProvider(ContactSchema.DisplayName)
			};
			ContactSchema.GivenName = new PropertyDefinition("GivenName", typeof(string))
			{
				EdmType = EdmCoreModel.Instance.GetString(true),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new SimpleEwsPropertyProvider(ContactSchema.GivenName)
			};
			ContactSchema.Initials = new PropertyDefinition("Initials", typeof(string))
			{
				EdmType = EdmCoreModel.Instance.GetString(true),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new SimpleEwsPropertyProvider(ContactSchema.Initials)
			};
			ContactSchema.MiddleName = new PropertyDefinition("MiddleName", typeof(string))
			{
				EdmType = EdmCoreModel.Instance.GetString(true),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new SimpleEwsPropertyProvider(ContactSchema.MiddleName)
			};
			ContactSchema.NickName = new PropertyDefinition("NickName", typeof(string))
			{
				EdmType = EdmCoreModel.Instance.GetString(true),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new SimpleEwsPropertyProvider(ContactSchema.Nickname)
			};
			ContactSchema.Surname = new PropertyDefinition("Surname", typeof(string))
			{
				EdmType = EdmCoreModel.Instance.GetString(true),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new SimpleEwsPropertyProvider(ContactSchema.Surname)
			};
			PropertyDefinition propertyDefinition3 = new PropertyDefinition("Title", typeof(string));
			propertyDefinition3.EdmType = EdmCoreModel.Instance.GetString(true);
			propertyDefinition3.Flags = PropertyDefinitionFlags.CanFilter;
			PropertyDefinition propertyDefinition4 = propertyDefinition3;
			SimpleEwsPropertyProvider simpleEwsPropertyProvider2 = new SimpleEwsPropertyProvider(ContactSchema.CompleteName);
			simpleEwsPropertyProvider2.Getter = delegate(Entity e, PropertyDefinition ep, ServiceObject s, PropertyInformation sp)
			{
				CompleteNameType valueOrDefault = s.GetValueOrDefault<CompleteNameType>(sp);
				if (valueOrDefault != null)
				{
					e[ep] = valueOrDefault.Title;
				}
			};
			propertyDefinition4.EwsPropertyProvider = simpleEwsPropertyProvider2;
			ContactSchema.Title = propertyDefinition3;
			PropertyDefinition propertyDefinition5 = new PropertyDefinition("YomiFirstName", typeof(string));
			propertyDefinition5.EdmType = EdmCoreModel.Instance.GetString(true);
			propertyDefinition5.Flags = PropertyDefinitionFlags.CanFilter;
			PropertyDefinition propertyDefinition6 = propertyDefinition5;
			SimpleEwsPropertyProvider simpleEwsPropertyProvider3 = new SimpleEwsPropertyProvider(ContactSchema.CompleteName);
			simpleEwsPropertyProvider3.Getter = delegate(Entity e, PropertyDefinition ep, ServiceObject s, PropertyInformation sp)
			{
				CompleteNameType valueOrDefault = s.GetValueOrDefault<CompleteNameType>(sp);
				if (valueOrDefault != null)
				{
					e[ep] = valueOrDefault.YomiFirstName;
				}
			};
			propertyDefinition6.EwsPropertyProvider = simpleEwsPropertyProvider3;
			ContactSchema.YomiFirstName = propertyDefinition5;
			PropertyDefinition propertyDefinition7 = new PropertyDefinition("YomiLastName", typeof(string));
			propertyDefinition7.EdmType = EdmCoreModel.Instance.GetString(true);
			propertyDefinition7.Flags = PropertyDefinitionFlags.CanFilter;
			PropertyDefinition propertyDefinition8 = propertyDefinition7;
			SimpleEwsPropertyProvider simpleEwsPropertyProvider4 = new SimpleEwsPropertyProvider(ContactSchema.CompleteName);
			simpleEwsPropertyProvider4.Getter = delegate(Entity e, PropertyDefinition ep, ServiceObject s, PropertyInformation sp)
			{
				CompleteNameType valueOrDefault = s.GetValueOrDefault<CompleteNameType>(sp);
				if (valueOrDefault != null)
				{
					e[ep] = s.GetValueOrDefault<CompleteNameType>(sp).YomiLastName;
				}
			};
			propertyDefinition8.EwsPropertyProvider = simpleEwsPropertyProvider4;
			ContactSchema.YomiLastName = propertyDefinition7;
			ContactSchema.Generation = new PropertyDefinition("Generation", typeof(string))
			{
				EdmType = EdmCoreModel.Instance.GetString(true),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new SimpleEwsPropertyProvider(ContactSchema.Generation)
			};
			PropertyDefinition propertyDefinition9 = new PropertyDefinition("EmailAddress1", typeof(string));
			propertyDefinition9.EdmType = EdmCoreModel.Instance.GetString(true);
			propertyDefinition9.Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate);
			PropertyDefinition propertyDefinition10 = propertyDefinition9;
			SimpleEwsPropertyProvider simpleEwsPropertyProvider5 = new SimpleEwsPropertyProvider(ContactSchema.EmailAddressEmailAddress1);
			simpleEwsPropertyProvider5.Getter = delegate(Entity e, PropertyDefinition ep, ServiceObject s, PropertyInformation sp)
			{
				EmailAddressDictionaryEntryType valueOrDefault = s.GetValueOrDefault<EmailAddressDictionaryEntryType>(sp);
				e[ep] = ((valueOrDefault != null) ? valueOrDefault.Value : null);
			};
			simpleEwsPropertyProvider5.Setter = delegate(Entity e, PropertyDefinition ep, ServiceObject s, PropertyInformation sp)
			{
				s[sp] = new EmailAddressDictionaryEntryType(EmailAddressKeyType.EmailAddress1, e[ep] as string);
			};
			propertyDefinition10.EwsPropertyProvider = simpleEwsPropertyProvider5;
			ContactSchema.EmailAddress1 = propertyDefinition9;
			PropertyDefinition propertyDefinition11 = new PropertyDefinition("EmailAddress2", typeof(string));
			propertyDefinition11.EdmType = EdmCoreModel.Instance.GetString(true);
			propertyDefinition11.Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate);
			PropertyDefinition propertyDefinition12 = propertyDefinition11;
			SimpleEwsPropertyProvider simpleEwsPropertyProvider6 = new SimpleEwsPropertyProvider(ContactSchema.EmailAddressEmailAddress2);
			simpleEwsPropertyProvider6.Getter = delegate(Entity e, PropertyDefinition ep, ServiceObject s, PropertyInformation sp)
			{
				EmailAddressDictionaryEntryType valueOrDefault = s.GetValueOrDefault<EmailAddressDictionaryEntryType>(sp);
				e[ep] = ((valueOrDefault != null) ? valueOrDefault.Value : null);
			};
			simpleEwsPropertyProvider6.Setter = delegate(Entity e, PropertyDefinition ep, ServiceObject s, PropertyInformation sp)
			{
				s[sp] = new EmailAddressDictionaryEntryType(EmailAddressKeyType.EmailAddress2, e[ep] as string);
			};
			propertyDefinition12.EwsPropertyProvider = simpleEwsPropertyProvider6;
			ContactSchema.EmailAddress2 = propertyDefinition11;
			PropertyDefinition propertyDefinition13 = new PropertyDefinition("EmailAddress3", typeof(string));
			propertyDefinition13.EdmType = EdmCoreModel.Instance.GetString(true);
			propertyDefinition13.Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate);
			PropertyDefinition propertyDefinition14 = propertyDefinition13;
			SimpleEwsPropertyProvider simpleEwsPropertyProvider7 = new SimpleEwsPropertyProvider(ContactSchema.EmailAddressEmailAddress3);
			simpleEwsPropertyProvider7.Getter = delegate(Entity e, PropertyDefinition ep, ServiceObject s, PropertyInformation sp)
			{
				EmailAddressDictionaryEntryType valueOrDefault = s.GetValueOrDefault<EmailAddressDictionaryEntryType>(sp);
				e[ep] = ((valueOrDefault != null) ? valueOrDefault.Value : null);
			};
			simpleEwsPropertyProvider7.Setter = delegate(Entity e, PropertyDefinition ep, ServiceObject s, PropertyInformation sp)
			{
				s[sp] = new EmailAddressDictionaryEntryType(EmailAddressKeyType.EmailAddress3, e[ep] as string);
			};
			propertyDefinition14.EwsPropertyProvider = simpleEwsPropertyProvider7;
			ContactSchema.EmailAddress3 = propertyDefinition13;
			ContactSchema.ImAddress1 = new PropertyDefinition("ImAddress1", typeof(string))
			{
				EdmType = EdmCoreModel.Instance.GetString(true),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new SimpleEwsPropertyProvider(ContactSchema.ImAddressImAddress1)
			};
			ContactSchema.ImAddress2 = new PropertyDefinition("ImAddress2", typeof(string))
			{
				EdmType = EdmCoreModel.Instance.GetString(true),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new SimpleEwsPropertyProvider(ContactSchema.ImAddressImAddress2)
			};
			ContactSchema.ImAddress3 = new PropertyDefinition("ImAddress3", typeof(string))
			{
				EdmType = EdmCoreModel.Instance.GetString(true),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new SimpleEwsPropertyProvider(ContactSchema.ImAddressImAddress3)
			};
			ContactSchema.JobTitle = new PropertyDefinition("JobTitle", typeof(string))
			{
				EdmType = EdmCoreModel.Instance.GetString(true),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new SimpleEwsPropertyProvider(ContactSchema.JobTitle)
			};
			ContactSchema.CompanyName = new PropertyDefinition("CompanyName", typeof(string))
			{
				EdmType = EdmCoreModel.Instance.GetString(true),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new SimpleEwsPropertyProvider(ContactSchema.CompanyName)
			};
			ContactSchema.Department = new PropertyDefinition("Department", typeof(string))
			{
				EdmType = EdmCoreModel.Instance.GetString(true),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new SimpleEwsPropertyProvider(ContactSchema.Department)
			};
			ContactSchema.OfficeLocation = new PropertyDefinition("OfficeLocation", typeof(string))
			{
				EdmType = EdmCoreModel.Instance.GetString(true),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new SimpleEwsPropertyProvider(ContactSchema.OfficeLocation)
			};
			ContactSchema.Profession = new PropertyDefinition("Profession", typeof(string))
			{
				EdmType = EdmCoreModel.Instance.GetString(true),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new SimpleEwsPropertyProvider(ContactSchema.Profession)
			};
			ContactSchema.BusinessHomePage = new PropertyDefinition("BusinessHomePage", typeof(string))
			{
				EdmType = EdmCoreModel.Instance.GetString(true),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new SimpleEwsPropertyProvider(ContactSchema.BusinessHomePage)
			};
			ContactSchema.AssistantName = new PropertyDefinition("AssistantName", typeof(string))
			{
				EdmType = EdmCoreModel.Instance.GetString(true),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new SimpleEwsPropertyProvider(ContactSchema.AssistantName)
			};
			ContactSchema.Manager = new PropertyDefinition("Manager", typeof(string))
			{
				EdmType = EdmCoreModel.Instance.GetString(true),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new SimpleEwsPropertyProvider(ContactSchema.Manager)
			};
			ContactSchema.HomePhone1 = new PropertyDefinition("HomePhone1", typeof(string))
			{
				EdmType = EdmCoreModel.Instance.GetString(true),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new SimpleEwsPropertyProvider(ContactSchema.PhoneNumberHomePhone)
			};
			ContactSchema.HomePhone2 = new PropertyDefinition("HomePhone2", typeof(string))
			{
				EdmType = EdmCoreModel.Instance.GetString(true),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new SimpleEwsPropertyProvider(ContactSchema.PhoneNumberHomePhone2)
			};
			ContactSchema.MobilePhone1 = new PropertyDefinition("MobilePhone1", typeof(string))
			{
				EdmType = EdmCoreModel.Instance.GetString(true),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new SimpleEwsPropertyProvider(ContactSchema.PhoneNumberMobilePhone)
			};
			ContactSchema.BusinessPhone1 = new PropertyDefinition("BusinessPhone1", typeof(string))
			{
				EdmType = EdmCoreModel.Instance.GetString(true),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new SimpleEwsPropertyProvider(ContactSchema.PhoneNumberBusinessPhone)
			};
			ContactSchema.BusinessPhone2 = new PropertyDefinition("BusinessPhone2", typeof(string))
			{
				EdmType = EdmCoreModel.Instance.GetString(true),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new SimpleEwsPropertyProvider(ContactSchema.PhoneNumberBusinessPhone2)
			};
			ContactSchema.OtherPhone = new PropertyDefinition("OtherPhone", typeof(string))
			{
				EdmType = EdmCoreModel.Instance.GetString(true),
				Flags = (PropertyDefinitionFlags.CanFilter | PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new SimpleEwsPropertyProvider(ContactSchema.PhoneNumberOtherTelephone)
			};
			ContactSchema.HomeAddress = new PropertyDefinition("HomeAddress", typeof(string))
			{
				EdmType = new EdmComplexTypeReference(PhysicalAddress.EdmComplexType.Member, true),
				Flags = (PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new HomeAddressPropertyProvider(),
				ODataPropertyValueConverter = new PhysicalAddressODataConverter()
			};
			ContactSchema.BusinessAddress = new PropertyDefinition("BusinessAddress", typeof(string))
			{
				EdmType = new EdmComplexTypeReference(PhysicalAddress.EdmComplexType.Member, true),
				Flags = (PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new BusinessAddressPropertyProvider(),
				ODataPropertyValueConverter = new PhysicalAddressODataConverter()
			};
			ContactSchema.OtherAddress = new PropertyDefinition("OtherAddress", typeof(string))
			{
				EdmType = new EdmComplexTypeReference(PhysicalAddress.EdmComplexType.Member, true),
				Flags = (PropertyDefinitionFlags.CanCreate | PropertyDefinitionFlags.CanUpdate),
				EwsPropertyProvider = new OtherAddressPropertyProvider(),
				ODataPropertyValueConverter = new PhysicalAddressODataConverter()
			};
			ContactSchema.DeclaredContactProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>
			{
				ContactSchema.ParentFolderId,
				ContactSchema.Birthday,
				ContactSchema.FileAs,
				ContactSchema.DisplayName,
				ContactSchema.GivenName,
				ContactSchema.Initials,
				ContactSchema.MiddleName,
				ContactSchema.NickName,
				ContactSchema.Surname,
				ContactSchema.Title,
				ContactSchema.Generation,
				ContactSchema.EmailAddress1,
				ContactSchema.EmailAddress2,
				ContactSchema.EmailAddress3,
				ContactSchema.ImAddress1,
				ContactSchema.ImAddress2,
				ContactSchema.ImAddress3,
				ContactSchema.JobTitle,
				ContactSchema.CompanyName,
				ContactSchema.Department,
				ContactSchema.OfficeLocation,
				ContactSchema.Profession,
				ContactSchema.BusinessHomePage,
				ContactSchema.AssistantName,
				ContactSchema.Manager,
				ContactSchema.HomePhone1,
				ContactSchema.HomePhone2,
				ContactSchema.BusinessPhone1,
				ContactSchema.BusinessPhone2,
				ContactSchema.MobilePhone1,
				ContactSchema.OtherPhone,
				ContactSchema.HomeAddress,
				ContactSchema.BusinessAddress,
				ContactSchema.OtherAddress,
				ItemSchema.DateTimeCreated,
				ItemSchema.LastModifiedTime
			});
			ContactSchema.AllContactProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>(ItemSchema.AllItemProperties.Union(ContactSchema.DeclaredContactProperties)));
			ContactSchema.DefaultContactProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>(ItemSchema.DefaultItemProperties)
			{
				ContactSchema.ParentFolderId,
				ContactSchema.Birthday,
				ContactSchema.FileAs,
				ContactSchema.DisplayName,
				ContactSchema.GivenName,
				ContactSchema.Initials,
				ContactSchema.MiddleName,
				ContactSchema.NickName,
				ContactSchema.Surname,
				ContactSchema.Title,
				ContactSchema.Generation,
				ContactSchema.EmailAddress1,
				ContactSchema.EmailAddress2,
				ContactSchema.EmailAddress3,
				ContactSchema.ImAddress1,
				ContactSchema.ImAddress2,
				ContactSchema.ImAddress3,
				ContactSchema.JobTitle,
				ContactSchema.CompanyName,
				ContactSchema.Department,
				ContactSchema.OfficeLocation,
				ContactSchema.Profession,
				ContactSchema.BusinessHomePage,
				ContactSchema.AssistantName,
				ContactSchema.Manager,
				ContactSchema.HomePhone1,
				ContactSchema.HomePhone2,
				ContactSchema.BusinessPhone1,
				ContactSchema.BusinessPhone2,
				ContactSchema.MobilePhone1,
				ContactSchema.OtherPhone,
				ContactSchema.HomeAddress,
				ContactSchema.BusinessAddress,
				ContactSchema.OtherAddress,
				ItemSchema.DateTimeCreated,
				ItemSchema.LastModifiedTime
			});
			ContactSchema.MandatoryContactCreationProperties = new ReadOnlyCollection<PropertyDefinition>(new List<PropertyDefinition>
			{
				ContactSchema.GivenName
			});
			ContactSchema.ContactSchemaInstance = new LazyMember<ContactSchema>(() => new ContactSchema());
		}

		public static readonly PropertyDefinition ParentFolderId;

		public static readonly PropertyDefinition Birthday;

		public static readonly PropertyDefinition FileAs;

		public static readonly PropertyDefinition DisplayName;

		public static readonly PropertyDefinition GivenName;

		public static readonly PropertyDefinition Initials;

		public static readonly PropertyDefinition MiddleName;

		public static readonly PropertyDefinition NickName;

		public static readonly PropertyDefinition Surname;

		public static readonly PropertyDefinition Title;

		public static readonly PropertyDefinition YomiFirstName;

		public static readonly PropertyDefinition YomiLastName;

		public static readonly PropertyDefinition Generation;

		public static readonly PropertyDefinition EmailAddress1;

		public static readonly PropertyDefinition EmailAddress2;

		public static readonly PropertyDefinition EmailAddress3;

		public static readonly PropertyDefinition ImAddress1;

		public static readonly PropertyDefinition ImAddress2;

		public static readonly PropertyDefinition ImAddress3;

		public static readonly PropertyDefinition JobTitle;

		public static readonly PropertyDefinition CompanyName;

		public static readonly PropertyDefinition Department;

		public static readonly PropertyDefinition OfficeLocation;

		public static readonly PropertyDefinition Profession;

		public static readonly PropertyDefinition BusinessHomePage;

		public static readonly PropertyDefinition AssistantName;

		public static readonly PropertyDefinition Manager;

		public static readonly PropertyDefinition HomePhone1;

		public static readonly PropertyDefinition HomePhone2;

		public static readonly PropertyDefinition MobilePhone1;

		public static readonly PropertyDefinition BusinessPhone1;

		public static readonly PropertyDefinition BusinessPhone2;

		public static readonly PropertyDefinition OtherPhone;

		public static readonly PropertyDefinition HomeAddress;

		public static readonly PropertyDefinition BusinessAddress;

		public static readonly PropertyDefinition OtherAddress;

		public static readonly ReadOnlyCollection<PropertyDefinition> DeclaredContactProperties;

		public static readonly ReadOnlyCollection<PropertyDefinition> AllContactProperties;

		public static readonly ReadOnlyCollection<PropertyDefinition> DefaultContactProperties;

		public static readonly ReadOnlyCollection<PropertyDefinition> MandatoryContactCreationProperties;

		private static readonly LazyMember<ContactSchema> ContactSchemaInstance;
	}
}
