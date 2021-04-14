using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal class ContactDictionaryEntryProperty : SimpleProperty, IPregatherParticipants, IToXmlCommand, IToXmlForPropertyBagCommand, IToServiceObjectCommand, IToServiceObjectForPropertyBagCommand, IPropertyCommand
	{
		protected ContactDictionaryEntryProperty(CommandContext commandContext, string[] xmlNestedLocalNames) : base(commandContext)
		{
			this.xmlLocalNames = xmlNestedLocalNames;
		}

		public static ContactDictionaryEntryProperty CreateCommandForEmailAddresses(CommandContext commandContext)
		{
			return ContactDictionaryEntryProperty.CreateCommand(commandContext, new string[]
			{
				"EmailAddresses",
				"Entry"
			});
		}

		public static ContactDictionaryEntryProperty CreateCommandForImAddresses(CommandContext commandContext)
		{
			return ContactDictionaryEntryProperty.CreateCommand(commandContext, new string[]
			{
				"ImAddresses",
				"Entry"
			});
		}

		public static ContactDictionaryEntryProperty CreateCommandForPhoneNumbers(CommandContext commandContext)
		{
			return ContactDictionaryEntryProperty.CreateCommand(commandContext, new string[]
			{
				"PhoneNumbers",
				"Entry"
			});
		}

		void IPregatherParticipants.Pregather(StoreObject storeObject, List<Participant> participants)
		{
			Contact contact = (Contact)storeObject;
			foreach (EmailAddressIndex key in new EmailAddressIndex[]
			{
				EmailAddressIndex.Email1,
				EmailAddressIndex.Email2,
				EmailAddressIndex.Email3
			})
			{
				Participant participant = contact.EmailAddresses[key];
				if (participant != null)
				{
					participants.Add(participant);
				}
			}
		}

		public new void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			Contact contact = (Contact)commandSettings.StoreObject;
			ServiceObject serviceObject = commandSettings.ServiceObject;
			if (this.StorePropertyExists(contact))
			{
				string text = (string)contact[this.propertyDefinition];
				DictionaryPropertyUri dictionaryPropertyUri = this.commandContext.PropertyInformation.PropertyPath as DictionaryPropertyUri;
				if (dictionaryPropertyUri != null && dictionaryPropertyUri.FieldUri == DictionaryUriEnum.EmailAddress)
				{
					EmailAddressKeyType key = ContactDictionaryEntryProperty.emailAddressPropInfoMap[this.commandContext.PropertyInformation];
					EmailAddressDictionaryEntryType emailAddressDictionaryEntryType = new EmailAddressDictionaryEntryType(key, text);
					serviceObject[this.commandContext.PropertyInformation] = emailAddressDictionaryEntryType;
					EmailAddressIndex key2 = this.ParseEmailAddressIndex(this.commandContext.PropertyInformation.PropertyPath, this.propertyDefinition.Name);
					Participant participant = contact.EmailAddresses[key2];
					if (participant != null)
					{
						ParticipantInformationDictionary participantInformation = EWSSettings.ParticipantInformation;
						ParticipantInformation participant2 = participantInformation.GetParticipant(participant);
						emailAddressDictionaryEntryType.Value = participant2.EmailAddress;
						if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010))
						{
							emailAddressDictionaryEntryType.MailboxType = participant2.MailboxType.ToString();
							if (!string.IsNullOrEmpty(participant2.DisplayName))
							{
								emailAddressDictionaryEntryType.Name = participant2.DisplayName;
							}
							if (!string.IsNullOrEmpty(participant2.RoutingType))
							{
								emailAddressDictionaryEntryType.RoutingType = participant2.RoutingType;
								return;
							}
						}
					}
				}
				else
				{
					serviceObject[this.commandContext.PropertyInformation] = text;
				}
			}
		}

		public new void ToServiceObjectForPropertyBag()
		{
			ToServiceObjectForPropertyBagCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectForPropertyBagCommandSettings>();
			IDictionary<PropertyDefinition, object> propertyBag = commandSettings.PropertyBag;
			string text = null;
			if (PropertyCommand.TryGetValueFromPropertyBag<string>(propertyBag, this.propertyDefinition, out text))
			{
				EmailAddressKeyType key;
				if (ContactDictionaryEntryProperty.emailAddressPropInfoMap.TryGetValue(this.commandContext.PropertyInformation, out key))
				{
					commandSettings.ServiceObject[this.commandContext.PropertyInformation] = new EmailAddressDictionaryEntryType(key, text);
					return;
				}
				commandSettings.ServiceObject[this.commandContext.PropertyInformation] = text;
			}
		}

		public override void SetUpdate(SetPropertyUpdate setPropertyUpdate, UpdateCommandSettings updateCommandSettings)
		{
			StoreObject storeObject = updateCommandSettings.StoreObject;
			ServiceObject serviceObject = setPropertyUpdate.ServiceObject;
			this.SetProperty(serviceObject, storeObject);
		}

		protected override void SetProperty(ServiceObject serviceObject, StoreObject storeObject)
		{
			DictionaryPropertyUri dictionaryPropertyUri = this.commandContext.PropertyInformation.PropertyPath as DictionaryPropertyUri;
			if (dictionaryPropertyUri != null && dictionaryPropertyUri.FieldUri == DictionaryUriEnum.EmailAddress)
			{
				EmailAddressDictionaryEntryType valueOrDefault = serviceObject.GetValueOrDefault<EmailAddressDictionaryEntryType>(this.commandContext.PropertyInformation);
				if (valueOrDefault != null)
				{
					this.SetEmailAddressProperty(this.commandContext.PropertyInformation.PropertyPath, storeObject as Contact, dictionaryPropertyUri.Key, valueOrDefault);
					return;
				}
			}
			else
			{
				base.SetProperty(serviceObject, storeObject);
			}
		}

		private static ContactDictionaryEntryProperty CreateCommand(CommandContext commandContext, string[] xmlNestedLocalNames)
		{
			return new ContactDictionaryEntryProperty(commandContext, xmlNestedLocalNames);
		}

		private void SetEmailAddressProperty(PropertyPath propertyPath, Contact contact, string key, EmailAddressDictionaryEntryType emailAddress)
		{
			if (key != null)
			{
				EmailAddressIndex key2;
				StorePropertyDefinition propertyDefinition;
				if (!(key == "EmailAddress1"))
				{
					if (!(key == "EmailAddress2"))
					{
						if (!(key == "EmailAddress3"))
						{
							goto IL_50;
						}
						key2 = EmailAddressIndex.Email3;
						propertyDefinition = ContactSchema.Email3OriginalDisplayName;
					}
					else
					{
						key2 = EmailAddressIndex.Email2;
						propertyDefinition = ContactSchema.Email2OriginalDisplayName;
					}
				}
				else
				{
					key2 = EmailAddressIndex.Email1;
					propertyDefinition = ContactSchema.Email1OriginalDisplayName;
				}
				Participant participant2;
				Participant participant = MailboxHelper.ParseMailbox(this.commandContext.PropertyInformation.PropertyPath, contact, emailAddress, this.commandContext.IdConverter, out participant2, true);
				Participant participant3 = null;
				Participant.Builder builder;
				if (!contact.EmailAddresses.TryGetValue(key2, out participant3))
				{
					builder = new Participant.Builder(participant.DisplayName, participant.EmailAddress, participant.RoutingType);
				}
				else
				{
					builder = new Participant.Builder(participant);
				}
				participant3 = builder.ToParticipant();
				if (participant3.ValidationStatus != ParticipantValidationStatus.NoError)
				{
					throw new ServiceArgumentException((CoreResources.IDs)3156759755U);
				}
				contact.EmailAddresses[key2] = participant3;
				if (string.Compare(participant2.RoutingType, "SMTP") == 0)
				{
					contact[propertyDefinition] = emailAddress.Value;
					return;
				}
				Participant participant4 = MailboxHelper.TryConvertTo(participant2, "SMTP");
				if (participant4 == null)
				{
					contact[propertyDefinition] = emailAddress.Value;
					return;
				}
				contact[propertyDefinition] = participant4.EmailAddress;
				return;
			}
			IL_50:
			throw new InvalidMailboxException(propertyPath, (CoreResources.IDs)2886480659U);
		}

		private EmailAddressIndex ParseEmailAddressIndex(PropertyPath propertyPath, string emailAddressIndex)
		{
			if (string.Compare(emailAddressIndex, ContactSchema.Email1EmailAddress.Name) == 0)
			{
				return EmailAddressIndex.Email1;
			}
			if (string.Compare(emailAddressIndex, ContactSchema.Email2EmailAddress.Name) == 0)
			{
				return EmailAddressIndex.Email2;
			}
			if (string.Compare(emailAddressIndex, ContactSchema.Email3EmailAddress.Name) == 0)
			{
				return EmailAddressIndex.Email3;
			}
			throw new InvalidMailboxException(propertyPath, (CoreResources.IDs)2886480659U);
		}

		public new void ToXml()
		{
			throw new InvalidOperationException("ContactDictionaryEntryProperty.ToXml should not be called");
		}

		public new void ToXmlForPropertyBag()
		{
			throw new InvalidOperationException("ContactDictionaryEntryProperty.ToXmlForPropertyBag should not be called");
		}

		private void SetEmailAddressProperty(PropertyPath propertyPath, Contact contact, string key, XmlElement emailAddress)
		{
			if (key != null)
			{
				EmailAddressIndex key2;
				StorePropertyDefinition propertyDefinition;
				if (!(key == "EmailAddress1"))
				{
					if (!(key == "EmailAddress2"))
					{
						if (!(key == "EmailAddress3"))
						{
							goto IL_50;
						}
						key2 = EmailAddressIndex.Email3;
						propertyDefinition = ContactSchema.Email3OriginalDisplayName;
					}
					else
					{
						key2 = EmailAddressIndex.Email2;
						propertyDefinition = ContactSchema.Email2OriginalDisplayName;
					}
				}
				else
				{
					key2 = EmailAddressIndex.Email1;
					propertyDefinition = ContactSchema.Email1OriginalDisplayName;
				}
				Participant participant2;
				Participant participant = MailboxHelper.ParseMailbox(this.commandContext.PropertyInformation.PropertyPath, contact, emailAddress, this.commandContext.IdConverter, out participant2, true);
				Participant participant3 = null;
				Participant.Builder builder;
				if (!contact.EmailAddresses.TryGetValue(key2, out participant3))
				{
					builder = new Participant.Builder(participant.DisplayName, participant.EmailAddress, participant.RoutingType);
				}
				else
				{
					builder = new Participant.Builder(participant);
				}
				participant3 = builder.ToParticipant();
				if (participant3.ValidationStatus != ParticipantValidationStatus.NoError)
				{
					throw new ServiceArgumentException((CoreResources.IDs)3156759755U);
				}
				contact.EmailAddresses[key2] = participant3;
				if (string.Compare(participant2.RoutingType, "SMTP") == 0)
				{
					contact[propertyDefinition] = emailAddress.InnerText;
					return;
				}
				Participant participant4 = MailboxHelper.TryConvertTo(participant2, "SMTP");
				if (participant4 == null)
				{
					contact[propertyDefinition] = emailAddress.InnerText;
					return;
				}
				contact[propertyDefinition] = participant4.EmailAddress;
				return;
			}
			IL_50:
			throw new InvalidMailboxException(propertyPath, (CoreResources.IDs)2886480659U);
		}

		protected const string XmlAttributeNameKey = "Key";

		protected const string XmlAttributeNameDisplayName = "Name";

		protected const string XmlAttributeNameRoutingType = "RoutingType";

		protected const string XmlAttributeNameMailboxType = "MailboxType";

		protected const string XmlElementNameEntry = "Entry";

		protected const string XmlElementNameEmailAddresses = "EmailAddresses";

		protected const string XmlElementNameImAddresses = "ImAddresses";

		protected const string XmlElementNamePhoneNumbers = "PhoneNumbers";

		protected const string XmlElementNamePhysicalAddresses = "PhysicalAddresses";

		protected const string XmlElementNameCity = "City";

		protected const string XmlElementNameCountryOrRegion = "CountryOrRegion";

		protected const string XmlElementNamePostalCode = "PostalCode";

		protected const string XmlElementNameState = "State";

		protected const string XmlElementNameStreet = "Street";

		protected const string EmailAddress1Name = "EmailAddress1";

		protected const string EmailAddress2Name = "EmailAddress2";

		protected const string EmailAddress3Name = "EmailAddress3";

		private const int XmlElementWithKeyAttributeLevel = 1;

		protected string[] xmlLocalNames;

		private static Dictionary<PropertyInformation, EmailAddressKeyType> emailAddressPropInfoMap = new Dictionary<PropertyInformation, EmailAddressKeyType>
		{
			{
				ContactSchema.EmailAddressEmailAddress1,
				EmailAddressKeyType.EmailAddress1
			},
			{
				ContactSchema.EmailAddressEmailAddress2,
				EmailAddressKeyType.EmailAddress2
			},
			{
				ContactSchema.EmailAddressEmailAddress3,
				EmailAddressKeyType.EmailAddress3
			}
		};
	}
}
