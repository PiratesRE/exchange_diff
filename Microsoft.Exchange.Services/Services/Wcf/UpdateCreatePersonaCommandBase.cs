using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Wcf
{
	internal abstract class UpdateCreatePersonaCommandBase : ServiceCommand<Persona>
	{
		public UpdateCreatePersonaCommandBase(CallContext callContext, UpdateCreatePersonaRequestBase request) : base(callContext)
		{
			WcfServiceCommandBase.ThrowIfNull(request, "request", "UpdateCreatePersonaCommandBase::UpdateCreatePersonaCommandBase");
			WcfServiceCommandBase.ThrowIfNull(request.PropertyUpdates, "UpdateCreatePersonaRequestBase.PropertyUpdates", "UpdateCreatePersonaCommandBase::UpdateCreatePersonaCommandBase");
			this.propertyChanges = this.GetStorePropertyChanges(request.PropertyUpdates);
			this.personType = PersonaTypeConverter.Parse(request.PersonTypeString);
		}

		protected ICollection<StoreObjectPropertyChange> GetStorePropertyChanges(PersonaPropertyUpdate[] propertyUpdates)
		{
			PersonaShape personaShape = PersonaShape.CreateShape();
			List<StoreObjectPropertyChange> list = new List<StoreObjectPropertyChange>();
			PropertyPath propertyPath = null;
			foreach (PersonaPropertyUpdate personaPropertyUpdate in propertyUpdates)
			{
				if (this.IsEmailNamePropertyUpdate(personaPropertyUpdate))
				{
					list.AddRange(this.ComputeChangesForEmailAddressFields(personaPropertyUpdate));
				}
				else if (((PropertyUri)personaPropertyUpdate.PropertyPath).Uri == PropertyUriEnum.PersonaBodies)
				{
					list.Add(new StoreObjectPropertyChange(PersonSchema.Bodies, personaPropertyUpdate.OldValue, personaPropertyUpdate.NewValue));
				}
				else if (((PropertyUri)personaPropertyUpdate.PropertyPath).Uri == PropertyUriEnum.PersonaType)
				{
					list.AddRange(this.ComputeChangesForPersonTypeFields(personaPropertyUpdate));
				}
				else if (((PropertyUri)personaPropertyUpdate.PropertyPath).Uri == PropertyUriEnum.PersonaMembers)
				{
					if (personaPropertyUpdate.AddMemberToPDL != null)
					{
						Participant newValue = MailboxHelper.ParsePDLMemberParticipant(personaPropertyUpdate.AddMemberToPDL, base.IdConverter, personaPropertyUpdate.PropertyPath);
						list.Add(new StoreObjectPropertyChange(PersonSchema.Members, null, newValue));
					}
					if (personaPropertyUpdate.DeleteMemberFromPDL != null)
					{
						Participant oldValue = MailboxHelper.ParsePDLMemberParticipant(personaPropertyUpdate.DeleteMemberFromPDL, base.IdConverter, personaPropertyUpdate.PropertyPath);
						list.Add(new StoreObjectPropertyChange(PersonSchema.Members, oldValue, null));
					}
				}
				else
				{
					PropertyInformation propertyInformation = null;
					if (!personaShape.TryGetPropertyInformation(personaPropertyUpdate.PropertyPath, out propertyInformation) || propertyInformation == null)
					{
						propertyPath = personaPropertyUpdate.PropertyPath;
						break;
					}
					PropertyDefinition[] propertyDefinitions = propertyInformation.GetPropertyDefinitions(null);
					if (propertyDefinitions == null || propertyDefinitions.Length != 1)
					{
						propertyPath = personaPropertyUpdate.PropertyPath;
						break;
					}
					ApplicationAggregatedProperty applicationAggregatedProperty = propertyDefinitions[0] as ApplicationAggregatedProperty;
					if (applicationAggregatedProperty == null || applicationAggregatedProperty.Dependencies == null || applicationAggregatedProperty.Dependencies.Length == 0)
					{
						propertyPath = personaPropertyUpdate.PropertyPath;
						break;
					}
					if (((PropertyUri)personaPropertyUpdate.PropertyPath).Uri == PropertyUriEnum.PersonaMobilePhones || applicationAggregatedProperty.Dependencies.Length == 1)
					{
						PropertyDefinition property = applicationAggregatedProperty.Dependencies[0].Property;
						StoreObjectPropertyChange item;
						if (property.Type == typeof(ExDateTime))
						{
							object oldValue2 = string.Empty;
							object newValue2 = string.Empty;
							if (personaPropertyUpdate.NewValue != string.Empty)
							{
								newValue2 = ExDateTimeConverter.Parse(personaPropertyUpdate.NewValue);
							}
							if (personaPropertyUpdate.OldValue != string.Empty)
							{
								oldValue2 = ExDateTimeConverter.Parse(personaPropertyUpdate.OldValue);
							}
							item = new StoreObjectPropertyChange(property, oldValue2, newValue2);
						}
						else
						{
							item = new StoreObjectPropertyChange(property, personaPropertyUpdate.OldValue, personaPropertyUpdate.NewValue);
						}
						list.Add(item);
					}
					else
					{
						ArrayPropertyInformation arrayPropertyInformation = propertyInformation as ArrayPropertyInformation;
						if (arrayPropertyInformation != null)
						{
							if (arrayPropertyInformation.ArrayItemLocalName != null && arrayPropertyInformation.ArrayItemLocalName == "PostalAddressAttributedValue")
							{
								string[] array = personaPropertyUpdate.OldValue.Split(UpdateCreatePersonaCommandBase.postalAddressSeparator, 5, StringSplitOptions.None);
								string[] array2 = personaPropertyUpdate.NewValue.Split(UpdateCreatePersonaCommandBase.postalAddressSeparator, 5, StringSplitOptions.None);
								for (int j = 0; j < array.Length; j++)
								{
									if (array[j] == string.Empty)
									{
										array[j] = null;
									}
								}
								for (int k = 0; k < array2.Length; k++)
								{
									if (array2[k] == string.Empty)
									{
										array2[k] = null;
									}
								}
								string localName;
								if ((localName = arrayPropertyInformation.LocalName) != null)
								{
									if (!(localName == "BusinessAddresses"))
									{
										if (!(localName == "HomeAddresses"))
										{
											if (localName == "OtherAddresses")
											{
												List<StoreObjectPropertyChange> collection = this.ComputeChangesForOtherAddresses(array, array2);
												list.AddRange(collection);
											}
										}
										else
										{
											List<StoreObjectPropertyChange> collection = this.ComputeChangesForHomeAddresses(array, array2);
											list.AddRange(collection);
										}
									}
									else
									{
										List<StoreObjectPropertyChange> collection = this.ComputeChangesForBusinessAddresses(array, array2);
										list.AddRange(collection);
									}
								}
							}
							else if (arrayPropertyInformation.ArrayItemLocalName != null && arrayPropertyInformation.ArrayItemLocalName == "EmailAddressAttributedValue")
							{
								list.AddRange(this.ComputeChangesForEmailAddressFields(personaPropertyUpdate));
							}
						}
					}
				}
			}
			if (propertyPath != null)
			{
				throw new InvalidPropertyRequestException(propertyPath);
			}
			return list;
		}

		private bool IsEmailNamePropertyUpdate(PersonaPropertyUpdate propertyUpdate)
		{
			return ((PropertyUri)propertyUpdate.PropertyPath).Uri == PropertyUriEnum.PersonaEmails1DisplayNames || ((PropertyUri)propertyUpdate.PropertyPath).Uri == PropertyUriEnum.PersonaEmails2DisplayNames || ((PropertyUri)propertyUpdate.PropertyPath).Uri == PropertyUriEnum.PersonaEmails3DisplayNames || ((PropertyUri)propertyUpdate.PropertyPath).Uri == PropertyUriEnum.PersonaEmails1OriginalDisplayNames || ((PropertyUri)propertyUpdate.PropertyPath).Uri == PropertyUriEnum.PersonaEmails2OriginalDisplayNames || ((PropertyUri)propertyUpdate.PropertyPath).Uri == PropertyUriEnum.PersonaEmails3OriginalDisplayNames;
		}

		private List<StoreObjectPropertyChange> ComputeChangesForPersonTypeFields(PersonaPropertyUpdate propertyUpdate)
		{
			List<StoreObjectPropertyChange> list = new List<StoreObjectPropertyChange>();
			PersonType personType = PersonType.Unknown;
			if (!string.IsNullOrEmpty(propertyUpdate.OldValue))
			{
				personType = PersonaTypeConverter.Parse(propertyUpdate.OldValue);
			}
			PersonType personType2 = PersonType.Unknown;
			if (!string.IsNullOrEmpty(propertyUpdate.NewValue))
			{
				personType2 = PersonaTypeConverter.Parse(propertyUpdate.NewValue);
			}
			if (personType != personType2)
			{
				list.Add(new StoreObjectPropertyChange(ContactSchema.PersonType, personType, personType2));
			}
			return list;
		}

		private List<StoreObjectPropertyChange> ComputeChangesForEmailAddressFields(PersonaPropertyUpdate emailUpdate)
		{
			List<StoreObjectPropertyChange> list = new List<StoreObjectPropertyChange>();
			switch (((PropertyUri)emailUpdate.PropertyPath).Uri)
			{
			case PropertyUriEnum.PersonaEmails1:
				list.Add(this.CreateStorePropertyChange(EmailAddressProperties.PropertySets[0].Address, emailUpdate.OldValue, emailUpdate.NewValue));
				break;
			case PropertyUriEnum.PersonaEmails1DisplayNames:
				list.Add(this.CreateStorePropertyChange(EmailAddressProperties.PropertySets[0].DisplayName, emailUpdate.OldValue, emailUpdate.NewValue));
				break;
			case PropertyUriEnum.PersonaEmails1OriginalDisplayNames:
				list.Add(this.CreateStorePropertyChange(EmailAddressProperties.PropertySets[0].OriginalDisplayName, emailUpdate.OldValue, emailUpdate.NewValue));
				break;
			case PropertyUriEnum.PersonaEmails2:
				list.Add(this.CreateStorePropertyChange(EmailAddressProperties.PropertySets[1].Address, emailUpdate.OldValue, emailUpdate.NewValue));
				break;
			case PropertyUriEnum.PersonaEmails2DisplayNames:
				list.Add(this.CreateStorePropertyChange(EmailAddressProperties.PropertySets[1].DisplayName, emailUpdate.OldValue, emailUpdate.NewValue));
				break;
			case PropertyUriEnum.PersonaEmails2OriginalDisplayNames:
				list.Add(this.CreateStorePropertyChange(EmailAddressProperties.PropertySets[1].OriginalDisplayName, emailUpdate.OldValue, emailUpdate.NewValue));
				break;
			case PropertyUriEnum.PersonaEmails3:
				list.Add(this.CreateStorePropertyChange(EmailAddressProperties.PropertySets[2].Address, emailUpdate.OldValue, emailUpdate.NewValue));
				break;
			case PropertyUriEnum.PersonaEmails3DisplayNames:
				list.Add(this.CreateStorePropertyChange(EmailAddressProperties.PropertySets[2].DisplayName, emailUpdate.OldValue, emailUpdate.NewValue));
				break;
			case PropertyUriEnum.PersonaEmails3OriginalDisplayNames:
				list.Add(this.CreateStorePropertyChange(EmailAddressProperties.PropertySets[2].OriginalDisplayName, emailUpdate.OldValue, emailUpdate.NewValue));
				break;
			}
			return list;
		}

		private StoreObjectPropertyChange CreateStorePropertyChange(NativeStorePropertyDefinition propertyToUpdate, string oldValue, string newValue)
		{
			return new StoreObjectPropertyChange(propertyToUpdate, oldValue, newValue);
		}

		private List<StoreObjectPropertyChange> ComputeChangesForBusinessAddresses(string[] oldValues, string[] newValues)
		{
			List<StoreObjectPropertyChange> list = new List<StoreObjectPropertyChange>();
			if (oldValues[0] != newValues[0])
			{
				list.Add(new StoreObjectPropertyChange(ContactSchema.WorkAddressStreet, oldValues[0], newValues[0]));
			}
			if (oldValues[1] != newValues[1])
			{
				list.Add(new StoreObjectPropertyChange(ContactSchema.WorkAddressCity, oldValues[1], newValues[1]));
			}
			if (oldValues[2] != newValues[2])
			{
				list.Add(new StoreObjectPropertyChange(ContactSchema.WorkAddressState, oldValues[2], newValues[2]));
			}
			if (oldValues[3] != newValues[3])
			{
				list.Add(new StoreObjectPropertyChange(ContactSchema.WorkAddressPostalCode, oldValues[3], newValues[3]));
			}
			if (oldValues[4] != newValues[4])
			{
				list.Add(new StoreObjectPropertyChange(ContactSchema.WorkAddressCountry, oldValues[4], newValues[4]));
			}
			return list;
		}

		private List<StoreObjectPropertyChange> ComputeChangesForHomeAddresses(string[] oldValues, string[] newValues)
		{
			List<StoreObjectPropertyChange> list = new List<StoreObjectPropertyChange>();
			if (oldValues[0] != newValues[0])
			{
				list.Add(new StoreObjectPropertyChange(ContactSchema.HomeStreet, oldValues[0], newValues[0]));
			}
			if (oldValues[1] != newValues[1])
			{
				list.Add(new StoreObjectPropertyChange(ContactSchema.HomeCity, oldValues[1], newValues[1]));
			}
			if (oldValues[2] != newValues[2])
			{
				list.Add(new StoreObjectPropertyChange(ContactSchema.HomeState, oldValues[2], newValues[2]));
			}
			if (oldValues[3] != newValues[3])
			{
				list.Add(new StoreObjectPropertyChange(ContactSchema.HomePostalCode, oldValues[3], newValues[3]));
			}
			if (oldValues[4] != newValues[4])
			{
				list.Add(new StoreObjectPropertyChange(ContactSchema.HomeCountry, oldValues[4], newValues[4]));
			}
			return list;
		}

		private List<StoreObjectPropertyChange> ComputeChangesForOtherAddresses(string[] oldValues, string[] newValues)
		{
			List<StoreObjectPropertyChange> list = new List<StoreObjectPropertyChange>();
			if (oldValues[0] != newValues[0])
			{
				list.Add(new StoreObjectPropertyChange(ContactSchema.OtherStreet, oldValues[0], newValues[0]));
			}
			if (oldValues[1] != newValues[1])
			{
				list.Add(new StoreObjectPropertyChange(ContactSchema.OtherCity, oldValues[1], newValues[1]));
			}
			if (oldValues[2] != newValues[2])
			{
				list.Add(new StoreObjectPropertyChange(ContactSchema.OtherState, oldValues[2], newValues[2]));
			}
			if (oldValues[3] != newValues[3])
			{
				list.Add(new StoreObjectPropertyChange(ContactSchema.OtherPostalCode, oldValues[3], newValues[3]));
			}
			if (oldValues[4] != newValues[4])
			{
				list.Add(new StoreObjectPropertyChange(ContactSchema.OtherCountry, oldValues[4], newValues[4]));
			}
			return list;
		}

		private const int MaxCountOfDelimitersInPostalAddress = 5;

		private static string[] postalAddressSeparator = new string[]
		{
			"$#$"
		};

		protected ICollection<StoreObjectPropertyChange> propertyChanges;

		protected PersonType personType;
	}
}
