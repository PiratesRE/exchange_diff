using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.NaturalLanguage;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Conversations;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core.DataConverter
{
	internal sealed class EntityExtractionResultProperty : ComplexPropertyBase, IToServiceObjectCommand, IPropertyCommand
	{
		private EntityExtractionResultProperty(CommandContext commandContext) : base(commandContext)
		{
		}

		public static EntityExtractionResultProperty CreateCommand(CommandContext commandContext)
		{
			return new EntityExtractionResultProperty(commandContext);
		}

		public void ToServiceObject()
		{
			ToServiceObjectCommandSettings commandSettings = base.GetCommandSettings<ToServiceObjectCommandSettings>();
			ServiceObject serviceObject = commandSettings.ServiceObject;
			PropertyInformation propertyInformation = this.commandContext.PropertyInformation;
			StoreObject storeObject = commandSettings.StoreObject;
			Func<StorePropertyDefinition, object> propGetterFunc = (StorePropertyDefinition propDef) => storeObject.GetValueOrDefault<object>(propDef, null);
			ExTimeZone exTimeZone = null;
			if (storeObject != null && storeObject.Session != null && storeObject.Session.MailboxOwner != null)
			{
				string smtpAddress = storeObject.Session.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString();
				MailboxSession mailboxSessionBySmtpAddress = CallContext.Current.SessionCache.GetMailboxSessionBySmtpAddress(smtpAddress, storeObject.Session.MailboxOwner.MailboxInfo.IsArchive);
				if (mailboxSessionBySmtpAddress != null)
				{
					exTimeZone = mailboxSessionBySmtpAddress.ExTimeZone;
				}
			}
			serviceObject.PropertyBag[propertyInformation] = EntityExtractionResultProperty.CreateResults(propGetterFunc, exTimeZone);
		}

		private static EntityExtractionResultType CreateResults(Func<StorePropertyDefinition, object> propGetterFunc, ExTimeZone exTimeZone)
		{
			EntityExtractionResultType result = new EntityExtractionResultType();
			EntityExtractionResultProperty.RunEntityFetchAction(delegate
			{
				EntityExtractionResultProperty.AddAddressesToResults(propGetterFunc, result);
			}, "Addresses");
			EntityExtractionResultProperty.RunEntityFetchAction(delegate
			{
				EntityExtractionResultProperty.AddUrlsToResults(propGetterFunc, result);
			}, "Urls");
			EntityExtractionResultProperty.RunEntityFetchAction(delegate
			{
				EntityExtractionResultProperty.AddContactsToResults(propGetterFunc, result);
			}, "Contacts");
			EntityExtractionResultProperty.RunEntityFetchAction(delegate
			{
				EntityExtractionResultProperty.AddMeetingSuggestionsToResults(propGetterFunc, result, exTimeZone);
			}, "MeetingSuggestions");
			EntityExtractionResultProperty.RunEntityFetchAction(delegate
			{
				EntityExtractionResultProperty.AddTaskSuggestionsToResults(propGetterFunc, result);
			}, "TaskSuggestions");
			EntityExtractionResultProperty.RunEntityFetchAction(delegate
			{
				EntityExtractionResultProperty.AddEmailAddressesToResults(propGetterFunc, result);
			}, "EmailAddresses");
			EntityExtractionResultProperty.RunEntityFetchAction(delegate
			{
				EntityExtractionResultProperty.AddPhoneNumbersResults(propGetterFunc, result);
			}, "PhoneNumbers");
			return result;
		}

		private static void RunEntityFetchAction(Action action, string propertyName)
		{
			try
			{
				action();
			}
			catch (CorruptDataException arg)
			{
				ExTraceGlobals.CommonAlgorithmTracer.TraceDebug<string, CorruptDataException>(0L, "Cannot get NLG property '{0}' exception: {1}", propertyName, arg);
			}
		}

		internal static EntityExtractionResultType Render(ItemPart itemPart, ExTimeZone exTimeZone)
		{
			new EntityExtractionResultType();
			Func<StorePropertyDefinition, object> propGetterFunc = (StorePropertyDefinition propDef) => itemPart.StorePropertyBag.GetValueOrDefault<object>(propDef, null);
			return EntityExtractionResultProperty.CreateResults(propGetterFunc, exTimeZone);
		}

		private static bool HasSupportedPosition(IPositionedExtraction positionedExtraction)
		{
			return positionedExtraction.Position == EmailPosition.LatestReply || positionedExtraction.Position == EmailPosition.Subject || positionedExtraction.Position == EmailPosition.Signature || positionedExtraction.Position == EmailPosition.Other;
		}

		private static bool TryGetEntityArray<T, TEntity>(Func<StorePropertyDefinition, object> propGetterFunc, StorePropertyDefinition propertyDefinition, EntityExtractionResultProperty.TryExtractEntity<T, TEntity> tryExtractEntityCallback, out TEntity[] extractedEntities) where T : IPositionedExtraction
		{
			extractedEntities = null;
			T[] array = (T[])propGetterFunc(propertyDefinition);
			if (array != null && array.Length > 0)
			{
				List<TEntity> list = new List<TEntity>(Math.Min(100, array.Length));
				foreach (T t in array)
				{
					TEntity item;
					if (EntityExtractionResultProperty.HasSupportedPosition(t) && tryExtractEntityCallback(t, out item))
					{
						list.Add(item);
						if (list.Count >= 100)
						{
							break;
						}
					}
				}
				if (list.Count > 0)
				{
					extractedEntities = list.ToArray();
					return true;
				}
			}
			return false;
		}

		private static void AddAddressesToResults(Func<StorePropertyDefinition, object> propGetterFunc, EntityExtractionResultType result)
		{
			AddressEntityType[] addresses;
			if (EntityExtractionResultProperty.TryGetEntityArray<Address, AddressEntityType>(propGetterFunc, ItemSchema.ExtractedAddresses, delegate(Address nlgObject, out AddressEntityType extractedEntity)
			{
				extractedEntity = null;
				int num = nlgObject.AddressString.Length + 1;
				if (num < 3072)
				{
					extractedEntity = new AddressEntityType
					{
						Address = nlgObject.AddressString,
						Position = (EmailPositionType)nlgObject.Position
					};
					return true;
				}
				return false;
			}, out addresses))
			{
				result.Addresses = addresses;
			}
		}

		private static void AddEmailAddressesToResults(Func<StorePropertyDefinition, object> propGetterFunc, EntityExtractionResultType result)
		{
			EmailAddressEntityType[] emailAddresses;
			if (EntityExtractionResultProperty.TryGetEntityArray<Email, EmailAddressEntityType>(propGetterFunc, ItemSchema.ExtractedEmails, delegate(Email nlgObject, out EmailAddressEntityType extractedEntity)
			{
				extractedEntity = null;
				int num = nlgObject.EmailString.Length + 1;
				if (num < 3072)
				{
					extractedEntity = new EmailAddressEntityType
					{
						EmailAddress = nlgObject.EmailString,
						Position = (EmailPositionType)nlgObject.Position
					};
					return true;
				}
				return false;
			}, out emailAddresses))
			{
				result.EmailAddresses = emailAddresses;
			}
		}

		private static void AddUrlsToResults(Func<StorePropertyDefinition, object> propGetterFunc, EntityExtractionResultType result)
		{
			UrlEntityType[] urls;
			if (EntityExtractionResultProperty.TryGetEntityArray<Url, UrlEntityType>(propGetterFunc, ItemSchema.ExtractedUrls, delegate(Url nlgObject, out UrlEntityType extractedEntity)
			{
				extractedEntity = null;
				int num = nlgObject.UrlString.Length + 1;
				if (num < 3072)
				{
					extractedEntity = new UrlEntityType
					{
						Url = nlgObject.UrlString,
						Position = (EmailPositionType)nlgObject.Position
					};
					return true;
				}
				return false;
			}, out urls))
			{
				result.Urls = urls;
			}
		}

		private static void AddPhoneNumbersResults(Func<StorePropertyDefinition, object> propGetterFunc, EntityExtractionResultType result)
		{
			PhoneEntityType[] phoneNumbers;
			if (EntityExtractionResultProperty.TryGetEntityArray<Phone, PhoneEntityType>(propGetterFunc, ItemSchema.ExtractedPhones, delegate(Phone nlgObject, out PhoneEntityType extractedEntity)
			{
				extractedEntity = null;
				string text = EnumUtilities.ToString<Microsoft.Exchange.Data.NaturalLanguage.PhoneType>(nlgObject.Type);
				int num = ((nlgObject.PhoneString == null) ? 0 : nlgObject.PhoneString.Length) + ((nlgObject.OriginalPhoneString == null) ? 0 : nlgObject.OriginalPhoneString.Length) + text.Length + 1;
				if (num < 3072)
				{
					extractedEntity = new PhoneEntityType
					{
						PhoneString = nlgObject.PhoneString,
						OriginalPhoneString = nlgObject.OriginalPhoneString,
						Type = text,
						Position = (EmailPositionType)nlgObject.Position
					};
					return true;
				}
				return false;
			}, out phoneNumbers))
			{
				result.PhoneNumbers = phoneNumbers;
			}
		}

		private static bool TryGetPhoneNumbers(Phone[] phones, out int totalLength, out Microsoft.Exchange.Services.Core.Types.PhoneType[] extractedPhoneNumbers)
		{
			totalLength = 0;
			extractedPhoneNumbers = null;
			if (phones != null && phones.Length > 0)
			{
				List<Microsoft.Exchange.Services.Core.Types.PhoneType> list = new List<Microsoft.Exchange.Services.Core.Types.PhoneType>(Math.Min(100, phones.Length));
				foreach (Phone phone in phones)
				{
					string text = EnumUtilities.ToString<Microsoft.Exchange.Data.NaturalLanguage.PhoneType>(phone.Type);
					int num = ((phone.PhoneString == null) ? 0 : phone.PhoneString.Length) + ((phone.OriginalPhoneString == null) ? 0 : phone.OriginalPhoneString.Length) + text.Length;
					if (num < 3072)
					{
						Microsoft.Exchange.Services.Core.Types.PhoneType item = new Microsoft.Exchange.Services.Core.Types.PhoneType
						{
							PhoneString = phone.PhoneString,
							OriginalPhoneString = phone.OriginalPhoneString,
							Type = text
						};
						list.Add(item);
						if (list.Count >= 100)
						{
							break;
						}
					}
				}
				if (list.Count > 0)
				{
					extractedPhoneNumbers = list.ToArray();
					return true;
				}
			}
			return false;
		}

		private static void AddTaskSuggestionsToResults(Func<StorePropertyDefinition, object> propGetterFunc, EntityExtractionResultType result)
		{
			Task[] array = (Task[])propGetterFunc(ItemSchema.ExtractedTasks);
			if (array != null && array.Length > 0)
			{
				List<TaskSuggestionType> list = new List<TaskSuggestionType>(Math.Min(100, array.Length));
				foreach (Task task in array)
				{
					if (EntityExtractionResultProperty.HasSupportedPosition(task) && (task.TaskString == null || task.TaskString.Length < 3072))
					{
						TaskSuggestionType item = new TaskSuggestionType
						{
							TaskString = task.TaskString,
							Assignees = EntityExtractionResultProperty.CreateEmailUsers(task.Assignees),
							Position = (EmailPositionType)task.Position
						};
						list.Add(item);
						if (list.Count >= 100)
						{
							break;
						}
					}
				}
				if (list.Count > 0)
				{
					result.TaskSuggestions = list.ToArray();
				}
			}
		}

		private static void AddMeetingSuggestionsToResults(Func<StorePropertyDefinition, object> propGetterFunc, EntityExtractionResultType result, ExTimeZone exTimeZone)
		{
			Meeting[] array = (Meeting[])propGetterFunc(ItemSchema.ExtractedMeetings);
			if (array != null && array.Length > 0)
			{
				List<MeetingSuggestionType> list = new List<MeetingSuggestionType>(Math.Min(100, array.Length));
				foreach (Meeting meeting in array)
				{
					if (EntityExtractionResultProperty.HasSupportedPosition(meeting) && (meeting.MeetingString == null || meeting.MeetingString.Length < 3072))
					{
						MeetingSuggestionType meetingSuggestionType = new MeetingSuggestionType
						{
							Location = meeting.Location,
							Subject = meeting.Subject,
							MeetingString = meeting.MeetingString,
							Attendees = EntityExtractionResultProperty.CreateEmailUsers(meeting.Attendees),
							Position = (EmailPositionType)meeting.Position
						};
						if (meeting.StartTime != null)
						{
							meetingSuggestionType.StartTime = EntityExtractionResultProperty.ConvertToUtc(meeting.StartTime.Value, exTimeZone);
						}
						if (meeting.EndTime != null)
						{
							meetingSuggestionType.EndTime = EntityExtractionResultProperty.ConvertToUtc(meeting.EndTime.Value, exTimeZone);
						}
						list.Add(meetingSuggestionType);
						if (list.Count >= 100)
						{
							break;
						}
					}
				}
				if (list.Count > 0)
				{
					result.MeetingSuggestions = list.ToArray();
				}
			}
		}

		private static DateTime ConvertToUtc(DateTime dateTime, ExTimeZone timeZone)
		{
			if (dateTime.Kind != DateTimeKind.Unspecified)
			{
				return (DateTime)((ExDateTime)dateTime).ToUtc();
			}
			if (timeZone == null)
			{
				throw new InvalidOperationException("[EntityExtractionResultProperty.ConvertToUtc] DateTimeKind is Unspecified but timeZone to use for conversion is null");
			}
			return (DateTime)timeZone.Assign((ExDateTime)dateTime).ToUtc();
		}

		private static void AddContactsToResults(Func<StorePropertyDefinition, object> propGetterFunc, EntityExtractionResultType result)
		{
			Contact[] array = (Contact[])propGetterFunc(ItemSchema.ExtractedContacts);
			if (array != null && array.Length > 0)
			{
				List<ContactType> list = new List<ContactType>(Math.Min(100, array.Length));
				foreach (Contact contact in array)
				{
					if (EntityExtractionResultProperty.HasSupportedPosition(contact))
					{
						int num = 0;
						ContactType contactType = new ContactType();
						contactType.Position = (EmailPositionType)contact.Position;
						num++;
						if (contact.Person != null && !string.IsNullOrEmpty(contact.Person.PersonString))
						{
							contactType.PersonName = contact.Person.PersonString;
							num += contact.Person.PersonString.Length;
						}
						if (contact.Business != null && !string.IsNullOrEmpty(contact.Business.BusinessString))
						{
							contactType.BusinessName = contact.Business.BusinessString;
							num += contact.Business.BusinessString.Length;
						}
						if (num < 3072)
						{
							int num2;
							Microsoft.Exchange.Services.Core.Types.PhoneType[] phoneNumbers;
							if (EntityExtractionResultProperty.TryGetPhoneNumbers(contact.Phones, out num2, out phoneNumbers))
							{
								contactType.PhoneNumbers = phoneNumbers;
								num += num2;
							}
							if (num < 3072)
							{
								string[] array3;
								if (EntityExtractionResultProperty.TryGetStringArrayForType<Address>(contact.Addresses, (Address address) => address.AddressString, out num2, out array3))
								{
									contactType.Addresses = array3;
									num += num2;
								}
								if (num < 3072)
								{
									if (EntityExtractionResultProperty.TryGetStringArrayForType<Url>(contact.Urls, (Url url) => url.UrlString, out num2, out array3))
									{
										contactType.Urls = array3;
										num += num2;
									}
									if (num < 3072)
									{
										if (EntityExtractionResultProperty.TryGetStringArrayForType<Email>(contact.Emails, (Email email) => email.EmailString, out num2, out array3))
										{
											contactType.EmailAddresses = array3;
											num += num2;
										}
										if (num < 3072)
										{
											contactType.ContactString = contact.ContactString;
											list.Add(contactType);
											if (list.Count >= 100)
											{
												break;
											}
										}
									}
								}
							}
						}
					}
				}
				result.Contacts = list.ToArray();
			}
		}

		private static bool TryGetStringArrayForType<T>(Func<StorePropertyDefinition, object> propGetterFunc, StorePropertyDefinition propertyDefinition, EntityExtractionResultProperty.EntityPropertyExtractor<T> extractor, out string[] extractedValues)
		{
			extractedValues = null;
			T[] nlgArray = (T[])propGetterFunc(propertyDefinition);
			int num;
			return EntityExtractionResultProperty.TryGetStringArrayForType<T>(nlgArray, extractor, out num, out extractedValues);
		}

		private static bool TryGetStringArrayForType<T>(T[] nlgArray, EntityExtractionResultProperty.EntityPropertyExtractor<T> extractor, out int totalLength, out string[] extractedValues)
		{
			totalLength = 0;
			extractedValues = null;
			if (nlgArray != null && nlgArray.Length > 0)
			{
				List<string> list = new List<string>(Math.Min(100, nlgArray.Length));
				foreach (T nlgObject in nlgArray)
				{
					string text = extractor(nlgObject);
					if (!string.IsNullOrEmpty(text) && text.Length < 3072)
					{
						list.Add(text);
						totalLength += text.Length;
						if (list.Count >= 100)
						{
							break;
						}
					}
				}
				if (list.Count > 0)
				{
					extractedValues = list.ToArray();
					return true;
				}
			}
			return false;
		}

		private static EmailUserType[] CreateEmailUsers(EmailUser[] emailUsers)
		{
			if (emailUsers == null)
			{
				return null;
			}
			EmailUserType[] array = new EmailUserType[emailUsers.Length];
			for (int i = 0; i < emailUsers.Length; i++)
			{
				array[i] = new EmailUserType
				{
					Name = emailUsers[i].Name,
					UserId = emailUsers[i].UserId
				};
			}
			return array;
		}

		private const int MaxValues = 100;

		private const int MaxLengthPerEntity = 3072;

		private delegate string EntityPropertyExtractor<T>(T nlgObject);

		private delegate bool TryExtractEntity<T, TEntity>(T nlgObject, out TEntity extractedEntity);
	}
}
