using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.DataConverter;

namespace Microsoft.Exchange.Services.Core.Types
{
	public class MailboxHelper
	{
		internal static Participant ParsePDLMemberParticipant(EmailAddressWrapper member, IdConverter idConverter, PropertyPath propertyPath)
		{
			Participant result = null;
			if (member == null)
			{
				return result;
			}
			string name = member.Name;
			string emailAddress = member.EmailAddress;
			string routingType = member.RoutingType;
			MailboxHelper.MailboxTypeType? mailboxTypeType = new MailboxHelper.MailboxTypeType?(EnumUtilities.Parse<MailboxHelper.MailboxTypeType>(member.MailboxType));
			ItemId itemId = member.ItemId;
			if (itemId != null)
			{
				if (mailboxTypeType != MailboxHelper.MailboxTypeType.PrivateDL && mailboxTypeType != MailboxHelper.MailboxTypeType.Contact)
				{
					throw new InvalidMailboxException(propertyPath, CoreResources.IDs.MessageCannotUseItemAsRecipient);
				}
				StoreObjectType storeObjectType = (mailboxTypeType == MailboxHelper.MailboxTypeType.PrivateDL) ? StoreObjectType.DistributionList : StoreObjectType.Contact;
				StoreId originItemId = IdConverter.EwsIdToStoreObjectIdGivenStoreObjectType(itemId.Id, storeObjectType);
				EmailAddressIndex emailAddressIndex = MailboxHelper.ParseEmailAddressIndex(member.EmailAddressIndex);
				ParticipantOrigin origin = new StoreParticipantOrigin(originItemId, emailAddressIndex);
				result = new Participant(name, emailAddress, routingType, origin, new KeyValuePair<PropertyDefinition, object>[0]);
				return result;
			}
			else
			{
				if (emailAddress == null)
				{
					throw new InvalidMailboxException(propertyPath, CoreResources.IDs.ErrorMissingInformationEmailAddress);
				}
				if (mailboxTypeType != null)
				{
					return MailboxHelper.CreateParticipantBasedOnMailboxType(name, emailAddress, routingType, mailboxTypeType.Value, propertyPath, false, out result);
				}
				return MailboxHelper.CreateParticipantWhenNoMailboxType(name, emailAddress, routingType, false, out result);
			}
		}

		internal static Participant ParseMailbox(PropertyPath propertyPath, StoreObject storeObject, EmailAddressWrapper mailbox, IdConverter itemIdConverter, out Participant unconvertedParticipant, bool isContactEmailAddress)
		{
			ItemId itemId = mailbox.ItemId;
			string name = mailbox.Name;
			string routingType = mailbox.RoutingType;
			string emailAddress = mailbox.EmailAddress;
			MailboxHelper.MailboxTypeType? mailboxType = null;
			if (!string.IsNullOrEmpty(mailbox.MailboxType))
			{
				mailboxType = new MailboxHelper.MailboxTypeType?(EnumUtilities.Parse<MailboxHelper.MailboxTypeType>(mailbox.MailboxType));
			}
			unconvertedParticipant = null;
			if (!string.IsNullOrEmpty(routingType))
			{
				try
				{
					new CustomProxyAddressPrefix(routingType, name);
				}
				catch (ArgumentException ex)
				{
					ExTraceGlobals.CommonAlgorithmTracer.TraceError<string, string>((long)storeObject.GetHashCode(), "Invalid routing type: '{0}'.  ArgumentException encountered: {1}", routingType, ex.Message);
					throw new MalformedRoutingTypeException(propertyPath, ex);
				}
			}
			if (itemId != null)
			{
				IdAndSession idAndSession = itemIdConverter.ConvertItemIdToIdAndSessionReadOnly(itemId);
				StoreId id = idAndSession.Id;
				StoreObjectType objectType = idAndSession.GetAsStoreObjectId().ObjectType;
				if (objectType == StoreObjectType.DistributionList)
				{
					return MailboxHelper.CreateParticipantWhenStoreObjectIsPDL(idAndSession, mailboxType, propertyPath);
				}
				if (objectType == StoreObjectType.Contact)
				{
					ParticipantOrigin origin = new StoreParticipantOrigin(id);
					unconvertedParticipant = new Participant(name, emailAddress, routingType, origin, new KeyValuePair<PropertyDefinition, object>[0]);
					Participant participant = null;
					bool flag = false;
					using (Contact contact = (Contact)idAndSession.GetRootXsoItem(null))
					{
						foreach (EmailAddressIndex key in MailboxHelper.emailAddressIndexes)
						{
							if (contact.EmailAddresses[key] != null)
							{
								if (MailboxHelper.CompareOptionalString(emailAddress, contact.EmailAddresses[key].EmailAddress) && MailboxHelper.CompareOptionalString(routingType, contact.EmailAddresses[key].RoutingType) && MailboxHelper.CompareOptionalString(name, contact.EmailAddresses[key].DisplayName))
								{
									return contact.EmailAddresses[key];
								}
								if (contact.EmailAddresses[key].RoutingType == "EX")
								{
									if (!flag)
									{
										participant = MailboxHelper.TryConvertTo(unconvertedParticipant, "EX");
										flag = true;
									}
									if (participant != null && MailboxHelper.CompareOptionalString(participant.EmailAddress, contact.EmailAddresses[key].EmailAddress))
									{
										return contact.EmailAddresses[key];
									}
								}
							}
						}
						throw new InvalidMailboxException(propertyPath, CoreResources.IDs.MessageInvalidMailboxContactAddressNotFound);
					}
				}
				throw new InvalidMailboxException(propertyPath, CoreResources.IDs.MessageCannotUseItemAsRecipient);
			}
			else
			{
				if (emailAddress == null)
				{
					throw new InvalidMailboxException(propertyPath, CoreResources.IDs.ErrorMissingInformationEmailAddress);
				}
				if (mailboxType == null)
				{
					return MailboxHelper.CreateParticipantWhenNoMailboxType(name, emailAddress, routingType, true, out unconvertedParticipant);
				}
				if (isContactEmailAddress && (mailboxType.Value == MailboxHelper.MailboxTypeType.Contact || mailboxType.Value == MailboxHelper.MailboxTypeType.PrivateDL))
				{
					throw new InvalidMailboxException(propertyPath, "MailboxType", mailboxType.Value.ToString());
				}
				return MailboxHelper.CreateParticipantBasedOnMailboxType(name, emailAddress, routingType, mailboxType.Value, propertyPath, true, out unconvertedParticipant);
			}
		}

		internal static Participant TryConvertTo(Participant participant, string targetRoutingType)
		{
			Participant[] array = MailboxHelper.TryConvertTo(new Participant[]
			{
				participant
			}, targetRoutingType);
			return array[0];
		}

		internal static Participant[] TryConvertTo(Participant[] participants, string targetRoutingType)
		{
			return MailboxHelper.TryConvertTo(participants, targetRoutingType, CallContext.Current);
		}

		internal static Participant[] TryConvertTo(Participant[] participants, string targetRoutingType, CallContext callContext)
		{
			return Participant.TryConvertTo(participants, targetRoutingType, callContext.ADRecipientSessionContext.GetADRecipientSession());
		}

		internal static MailboxHelper.MailboxTypeType GetMailboxType(ParticipantOrigin participantOrigin, string participantRoutingType)
		{
			return MailboxHelper.GetMailboxType(participantOrigin, participantRoutingType, CallContext.Current != null && CallContext.Current.IsOwa);
		}

		internal static MailboxHelper.MailboxTypeType GetMailboxType(ParticipantOrigin participantOrigin, string participantRoutingType, bool isOwa)
		{
			if (participantOrigin is OneOffParticipantOrigin)
			{
				return MailboxHelper.MailboxTypeType.OneOff;
			}
			DirectoryParticipantOrigin directoryParticipantOrigin = participantOrigin as DirectoryParticipantOrigin;
			if (directoryParticipantOrigin != null)
			{
				if (directoryParticipantOrigin.ADEntry != null)
				{
					return MailboxHelper.ConvertToMailboxType((RecipientType)directoryParticipantOrigin.ADEntry[ADRecipientSchema.RecipientType], (RecipientTypeDetails)directoryParticipantOrigin.ADEntry[ADRecipientSchema.RecipientTypeDetails], isOwa);
				}
				if (directoryParticipantOrigin.ADContact != null)
				{
					PersonType valueOrDefault = directoryParticipantOrigin.ADContact.GetValueOrDefault<PersonType>(ContactSchema.PersonType, PersonType.Unknown);
					return MailboxHelper.ConvertToMailboxType(valueOrDefault);
				}
				return MailboxHelper.MailboxTypeType.OneOff;
			}
			else
			{
				StoreParticipantOrigin storeParticipantOrigin = participantOrigin as StoreParticipantOrigin;
				if (storeParticipantOrigin == null || storeParticipantOrigin.OriginItemId == null)
				{
					return MailboxHelper.MailboxTypeType.Unknown;
				}
				if (participantRoutingType == "MAPIPDL")
				{
					return MailboxHelper.MailboxTypeType.PrivateDL;
				}
				return MailboxHelper.MailboxTypeType.Contact;
			}
		}

		internal static MailboxHelper.MailboxTypeType ConvertToMailboxType(PersonType personType)
		{
			if (personType == PersonType.DistributionList)
			{
				return MailboxHelper.MailboxTypeType.PublicDL;
			}
			if (personType != PersonType.ModernGroup)
			{
				return MailboxHelper.MailboxTypeType.Mailbox;
			}
			return MailboxHelper.MailboxTypeType.GroupMailbox;
		}

		internal static bool IsFullyResolvedMailboxType(MailboxHelper.MailboxTypeType mailboxType)
		{
			return mailboxType != MailboxHelper.MailboxTypeType.OneOff && mailboxType != MailboxHelper.MailboxTypeType.Unknown;
		}

		internal static MailboxHelper.MailboxTypeType GetMailboxType(IParticipant participant, bool isOwa)
		{
			MailboxHelper.MailboxTypeType mailboxType = MailboxHelper.GetMailboxType(participant.Origin, participant.RoutingType, isOwa);
			if (MailboxHelper.IsFullyResolvedMailboxType(mailboxType))
			{
				return mailboxType;
			}
			if (participant.GetValueOrDefault<bool>(ParticipantSchema.IsGroupMailbox))
			{
				if (isOwa)
				{
					return MailboxHelper.MailboxTypeType.GroupMailbox;
				}
				return MailboxHelper.MailboxTypeType.Mailbox;
			}
			else if (participant.GetValueOrDefault<bool>(ParticipantSchema.IsDistributionList))
			{
				if (participant.Origin is StoreParticipantOrigin)
				{
					return MailboxHelper.MailboxTypeType.PrivateDL;
				}
				return MailboxHelper.MailboxTypeType.PublicDL;
			}
			else
			{
				if (participant.GetValueOrDefault<bool>(ParticipantSchema.IsResource))
				{
					return MailboxHelper.MailboxTypeType.Mailbox;
				}
				if (participant.GetValueOrDefault<bool>(ParticipantSchema.IsRoom))
				{
					return MailboxHelper.MailboxTypeType.Mailbox;
				}
				if (participant.GetValueOrDefault<bool>(ParticipantSchema.IsMailboxUser))
				{
					return MailboxHelper.MailboxTypeType.Mailbox;
				}
				return mailboxType;
			}
		}

		internal static MailboxHelper.MailboxTypeType ConvertToMailboxType(RecipientType recipientType, RecipientTypeDetails recipientTypeDetails)
		{
			return MailboxHelper.ConvertToMailboxType(recipientType, recipientTypeDetails, CallContext.Current != null && CallContext.Current.IsOwa);
		}

		internal static MailboxHelper.MailboxTypeType ConvertToMailboxType(RecipientType recipientType, RecipientTypeDetails recipientTypeDetails, bool isOwa)
		{
			switch (recipientType)
			{
			case RecipientType.UserMailbox:
			case RecipientType.MailUser:
				if (recipientTypeDetails != RecipientTypeDetails.GroupMailbox)
				{
					return MailboxHelper.MailboxTypeType.Mailbox;
				}
				if (isOwa)
				{
					return MailboxHelper.MailboxTypeType.GroupMailbox;
				}
				return MailboxHelper.MailboxTypeType.Mailbox;
			case RecipientType.MailContact:
				return MailboxHelper.MailboxTypeType.Contact;
			case RecipientType.MailUniversalDistributionGroup:
			case RecipientType.MailUniversalSecurityGroup:
			case RecipientType.MailNonUniversalGroup:
			case RecipientType.DynamicDistributionGroup:
				return MailboxHelper.MailboxTypeType.PublicDL;
			case RecipientType.PublicFolder:
				if (!ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2007SP1))
				{
					return MailboxHelper.MailboxTypeType.Mailbox;
				}
				return MailboxHelper.MailboxTypeType.PublicFolder;
			case RecipientType.SystemAttendantMailbox:
			case RecipientType.SystemMailbox:
			case RecipientType.MicrosoftExchange:
				if (!ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2013))
				{
					return MailboxHelper.MailboxTypeType.Unknown;
				}
				return MailboxHelper.MailboxTypeType.Mailbox;
			}
			return MailboxHelper.MailboxTypeType.Unknown;
		}

		internal static Participant GetParticipantFromAddress(EmailAddressWrapper address)
		{
			Participant participant = new Participant(address.Name, address.EmailAddress, address.RoutingType);
			Participant participant2 = MailboxHelper.TryConvertTo(participant, "EX");
			if (null != participant2)
			{
				return participant2;
			}
			return participant;
		}

		private static Participant CreateParticipantBasedOnMailboxType(string displayName, string emailAddress, string routingType, MailboxHelper.MailboxTypeType mailboxType, PropertyPath propertyPath, bool shouldValidate, out Participant unconvertedParticipant)
		{
			unconvertedParticipant = null;
			switch (mailboxType)
			{
			case MailboxHelper.MailboxTypeType.OneOff:
				unconvertedParticipant = new Participant(displayName, emailAddress, routingType, new OneOffParticipantOrigin(), new KeyValuePair<PropertyDefinition, object>[0]);
				goto IL_131;
			case MailboxHelper.MailboxTypeType.Mailbox:
			case MailboxHelper.MailboxTypeType.PublicDL:
			case MailboxHelper.MailboxTypeType.Contact:
			case MailboxHelper.MailboxTypeType.PublicFolder:
			case MailboxHelper.MailboxTypeType.GroupMailbox:
			{
				unconvertedParticipant = new Participant(displayName, emailAddress, routingType, new DirectoryParticipantOrigin(), new KeyValuePair<PropertyDefinition, object>[0]);
				Participant participant = MailboxHelper.TryConvertTo(unconvertedParticipant, "EX");
				if (shouldValidate)
				{
					if (null == participant)
					{
						throw new InvalidMailboxException(propertyPath, (CoreResources.IDs)2343198056U);
					}
					DirectoryParticipantOrigin directoryParticipantOrigin = participant.Origin as DirectoryParticipantOrigin;
					MailboxHelper.MailboxTypeType mailboxTypeType = MailboxHelper.ConvertToMailboxType((RecipientType)directoryParticipantOrigin.ADEntry[ADRecipientSchema.RecipientType], (RecipientTypeDetails)directoryParticipantOrigin.ADEntry[ADRecipientSchema.RecipientTypeDetails]);
					if (mailboxTypeType != mailboxType)
					{
						throw new InvalidMailboxException(propertyPath, "MailboxType", mailboxTypeType.ToString(), mailboxType.ToString(), CoreResources.IDs.MessageInvalidMailboxMailboxType);
					}
					return participant;
				}
				else
				{
					if (null != participant)
					{
						return participant;
					}
					goto IL_131;
				}
				break;
			}
			}
			if (shouldValidate)
			{
				throw new InvalidMailboxException(propertyPath, "MailboxType", mailboxType.ToString());
			}
			unconvertedParticipant = new Participant(displayName, emailAddress, routingType);
			IL_131:
			return unconvertedParticipant;
		}

		private static Participant CreateParticipantWhenNoMailboxType(string displayName, string emailAddress, string routingType, bool shouldValidate, out Participant unconvertedParticipant)
		{
			unconvertedParticipant = new Participant(displayName, emailAddress, routingType);
			if (shouldValidate)
			{
				Participant participant = MailboxHelper.TryConvertTo(unconvertedParticipant, "EX");
				if (null != participant)
				{
					return participant;
				}
			}
			return unconvertedParticipant;
		}

		private static Participant CreateParticipantWhenStoreObjectIsPDL(IdAndSession refIdAndSession, MailboxHelper.MailboxTypeType? mailboxType, PropertyPath propertyPath)
		{
			if (mailboxType != null && mailboxType.Value != MailboxHelper.MailboxTypeType.PrivateDL)
			{
				throw new InvalidMailboxException(propertyPath, "MailboxType", MailboxHelper.MailboxTypeType.PrivateDL.ToString(), mailboxType.Value.ToString(), CoreResources.IDs.MessageInvalidMailboxNotPrivateDL);
			}
			Participant asParticipant;
			using (DistributionList distributionList = (DistributionList)refIdAndSession.GetRootXsoItem(null))
			{
				asParticipant = distributionList.GetAsParticipant();
			}
			return asParticipant;
		}

		private static MailboxHelper.MailboxTypeType? ParseMailboxTypeType(string mailboxTypeString)
		{
			if (string.IsNullOrEmpty(mailboxTypeString))
			{
				return null;
			}
			switch (mailboxTypeString)
			{
			case "OneOff":
				return new MailboxHelper.MailboxTypeType?(MailboxHelper.MailboxTypeType.OneOff);
			case "Mailbox":
				return new MailboxHelper.MailboxTypeType?(MailboxHelper.MailboxTypeType.Mailbox);
			case "PublicDL":
				return new MailboxHelper.MailboxTypeType?(MailboxHelper.MailboxTypeType.PublicDL);
			case "PrivateDL":
				return new MailboxHelper.MailboxTypeType?(MailboxHelper.MailboxTypeType.PrivateDL);
			case "Contact":
				return new MailboxHelper.MailboxTypeType?(MailboxHelper.MailboxTypeType.Contact);
			case "PublicFolder":
				return new MailboxHelper.MailboxTypeType?(MailboxHelper.MailboxTypeType.PublicFolder);
			case "GroupMailbox":
				return new MailboxHelper.MailboxTypeType?(MailboxHelper.MailboxTypeType.GroupMailbox);
			}
			return new MailboxHelper.MailboxTypeType?(MailboxHelper.MailboxTypeType.Unknown);
		}

		private static EmailAddressIndex ParseEmailAddressIndex(string emailAddressIndexString)
		{
			if (string.IsNullOrEmpty(emailAddressIndexString))
			{
				return EmailAddressIndex.None;
			}
			switch (emailAddressIndexString)
			{
			case "Email1":
				return EmailAddressIndex.Email1;
			case "Email2":
				return EmailAddressIndex.Email2;
			case "Email3":
				return EmailAddressIndex.Email3;
			case "HomeFax":
				return EmailAddressIndex.HomeFax;
			case "BusinessFax":
				return EmailAddressIndex.BusinessFax;
			case "OtherFax":
				return EmailAddressIndex.OtherFax;
			}
			return EmailAddressIndex.None;
		}

		private static bool CompareOptionalString(string stringToCompare, string stringToCompareWith)
		{
			return stringToCompare == null || 0 == string.Compare(stringToCompare, stringToCompareWith);
		}

		internal static Participant ParseMailbox(PropertyPath propertyPath, StoreObject storeObject, XmlElement parentElement, IdConverter itemIdConverter, out Participant unconvertedParticipant, bool isContactEmailAddress)
		{
			XmlElement xmlElement = parentElement["ItemId", "http://schemas.microsoft.com/exchange/services/2006/types"];
			XmlElement xmlElement2 = parentElement["MailboxType", "http://schemas.microsoft.com/exchange/services/2006/types"];
			string text = null;
			string text2 = null;
			string text3 = null;
			string mailboxTypeString = null;
			if (isContactEmailAddress)
			{
				text = ServiceXml.GetXmlElementAttributeValueOptional(parentElement, "Name");
				text2 = ServiceXml.GetXmlElementAttributeValueOptional(parentElement, "RoutingType");
				mailboxTypeString = ServiceXml.GetXmlElementAttributeValueOptional(parentElement, "MailboxType");
				text3 = parentElement.InnerText;
			}
			else
			{
				text = ServiceXml.GetXmlTextNodeValue(parentElement, "Name", "http://schemas.microsoft.com/exchange/services/2006/types");
				text2 = ServiceXml.GetXmlTextNodeValue(parentElement, "RoutingType", "http://schemas.microsoft.com/exchange/services/2006/types");
				text3 = ServiceXml.GetXmlTextNodeValue(parentElement, "EmailAddress", "http://schemas.microsoft.com/exchange/services/2006/types");
				if (xmlElement2 != null)
				{
					mailboxTypeString = xmlElement2.InnerText;
				}
			}
			MailboxHelper.MailboxTypeType? mailboxTypeType = MailboxHelper.ParseMailboxTypeType(mailboxTypeString);
			unconvertedParticipant = null;
			if (text2 != null)
			{
				try
				{
					new CustomProxyAddressPrefix(text2, text);
				}
				catch (ArgumentException ex)
				{
					ExTraceGlobals.CommonAlgorithmTracer.TraceError<string, string>((long)storeObject.GetHashCode(), "Invalid routing type: '{0}'.  ArgumentException encountered: {1}", text2, ex.Message);
					throw new MalformedRoutingTypeException(propertyPath, ex);
				}
			}
			if (xmlElement != null)
			{
				IdAndSession idAndSession = itemIdConverter.ConvertXmlToIdAndSessionReadOnly(xmlElement, BasicTypes.Item);
				StoreId id = idAndSession.Id;
				StoreObjectType objectType = idAndSession.GetAsStoreObjectId().ObjectType;
				if (objectType == StoreObjectType.DistributionList)
				{
					if (mailboxTypeType != null && mailboxTypeType.Value != MailboxHelper.MailboxTypeType.PrivateDL)
					{
						throw new InvalidMailboxException(propertyPath, "MailboxType", MailboxHelper.MailboxTypeType.PrivateDL.ToString(), mailboxTypeType.Value.ToString(), CoreResources.IDs.MessageInvalidMailboxNotPrivateDL);
					}
					using (DistributionList distributionList = (DistributionList)idAndSession.GetRootXsoItem(null))
					{
						return distributionList.GetAsParticipant();
					}
				}
				if (objectType == StoreObjectType.Contact)
				{
					ParticipantOrigin origin = new StoreParticipantOrigin(id);
					unconvertedParticipant = new Participant(text, text3, text2, origin, new KeyValuePair<PropertyDefinition, object>[0]);
					Participant participant = null;
					bool flag = false;
					using (Contact contact = (Contact)idAndSession.GetRootXsoItem(null))
					{
						foreach (EmailAddressIndex key in MailboxHelper.emailAddressIndexes)
						{
							if (contact.EmailAddresses[key] != null)
							{
								if (MailboxHelper.CompareOptionalString(text3, contact.EmailAddresses[key].EmailAddress) && MailboxHelper.CompareOptionalString(text2, contact.EmailAddresses[key].RoutingType) && MailboxHelper.CompareOptionalString(text, contact.EmailAddresses[key].DisplayName))
								{
									return contact.EmailAddresses[key];
								}
								if (contact.EmailAddresses[key].RoutingType == "EX")
								{
									if (!flag)
									{
										participant = MailboxHelper.TryConvertTo(unconvertedParticipant, "EX");
										flag = true;
									}
									if (participant != null && MailboxHelper.CompareOptionalString(participant.EmailAddress, contact.EmailAddresses[key].EmailAddress))
									{
										return contact.EmailAddresses[key];
									}
								}
							}
						}
						throw new InvalidMailboxException(propertyPath, CoreResources.IDs.MessageInvalidMailboxContactAddressNotFound);
					}
				}
				throw new InvalidMailboxException(propertyPath, CoreResources.IDs.MessageCannotUseItemAsRecipient);
			}
			if (text3 == null)
			{
				throw new InvalidMailboxException(propertyPath, CoreResources.IDs.ErrorMissingInformationEmailAddress);
			}
			if (mailboxTypeType != null)
			{
				if (isContactEmailAddress && (mailboxTypeType.Value == MailboxHelper.MailboxTypeType.Contact || mailboxTypeType.Value == MailboxHelper.MailboxTypeType.PrivateDL))
				{
					throw new InvalidMailboxException(propertyPath, "MailboxType", mailboxTypeType.Value.ToString());
				}
				switch (mailboxTypeType.Value)
				{
				case MailboxHelper.MailboxTypeType.OneOff:
					unconvertedParticipant = new Participant(text, text3, text2, new OneOffParticipantOrigin(), new KeyValuePair<PropertyDefinition, object>[0]);
					goto IL_4AE;
				case MailboxHelper.MailboxTypeType.Mailbox:
				case MailboxHelper.MailboxTypeType.PublicDL:
				case MailboxHelper.MailboxTypeType.Contact:
				case MailboxHelper.MailboxTypeType.PublicFolder:
				case MailboxHelper.MailboxTypeType.GroupMailbox:
				{
					unconvertedParticipant = new Participant(text, text3, text2, new DirectoryParticipantOrigin(), new KeyValuePair<PropertyDefinition, object>[0]);
					Participant participant2 = MailboxHelper.TryConvertTo(unconvertedParticipant, "EX");
					if (null == participant2)
					{
						throw new InvalidMailboxException(propertyPath, (CoreResources.IDs)2343198056U);
					}
					DirectoryParticipantOrigin directoryParticipantOrigin = participant2.Origin as DirectoryParticipantOrigin;
					MailboxHelper.MailboxTypeType mailboxTypeType2 = MailboxHelper.ConvertToMailboxType((RecipientType)directoryParticipantOrigin.ADEntry[ADRecipientSchema.RecipientType], (RecipientTypeDetails)directoryParticipantOrigin.ADEntry[ADRecipientSchema.RecipientTypeDetails]);
					if (mailboxTypeType2 != mailboxTypeType.Value)
					{
						throw new InvalidMailboxException(propertyPath, "MailboxType", mailboxTypeType2.ToString(), mailboxTypeType.Value.ToString(), CoreResources.IDs.MessageInvalidMailboxMailboxType);
					}
					return participant2;
				}
				}
				throw new InvalidMailboxException(propertyPath, "MailboxType", mailboxTypeType.Value.ToString());
			}
			else
			{
				unconvertedParticipant = new Participant(text, text3, text2);
				Participant participant3 = MailboxHelper.TryConvertTo(unconvertedParticipant, "EX");
				if (null != participant3)
				{
					return participant3;
				}
			}
			IL_4AE:
			return unconvertedParticipant;
		}

		private const string MailboxTypeOneOff = "OneOff";

		private const string MailboxTypeMailbox = "Mailbox";

		private const string MailboxTypePublicDL = "PublicDL";

		private const string MailboxTypePrivateDL = "PrivateDL";

		private const string MailboxTypeContact = "Contact";

		private const string MailboxTypePublicFolder = "PublicFolder";

		private const string MailboxTypeGroupMailbox = "GroupMailbox";

		private static EmailAddressIndex[] emailAddressIndexes = new EmailAddressIndex[]
		{
			EmailAddressIndex.Email1,
			EmailAddressIndex.Email2,
			EmailAddressIndex.Email3,
			EmailAddressIndex.BusinessFax,
			EmailAddressIndex.HomeFax,
			EmailAddressIndex.OtherFax
		};

		public enum MailboxTypeType
		{
			Unknown,
			OneOff,
			Mailbox,
			PublicDL,
			PrivateDL,
			Contact,
			PublicFolder,
			GroupMailbox
		}
	}
}
