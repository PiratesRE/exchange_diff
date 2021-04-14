using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class PersonPropertyAggregationStrategy
	{
		public static PropertyAggregationStrategy CreateNameProperty(StorePropertyDefinition sourceProperty)
		{
			return new PropertyAggregationStrategy.SingleValuePropertyAggregation(ContactSelectionStrategy.CreatePersonNameProperty(sourceProperty));
		}

		public static PropertyAggregationStrategy CreatePriorityPropertyAggregation(StorePropertyDefinition sourceProperty)
		{
			return new PropertyAggregationStrategy.SingleValuePropertyAggregation(ContactSelectionStrategy.CreateSingleSourceProperty(sourceProperty));
		}

		public static PropertyAggregationStrategy CreateExtendedPropertiesAggregation(params StorePropertyDefinition[] extendedProperties)
		{
			return new PersonPropertyAggregationStrategy.ExtendedPropertiesAggregation(extendedProperties);
		}

		private static bool TryGetArrayResult<T>(IList<T> list, out object value)
		{
			if (list.Count > 0)
			{
				value = list.ToArray<T>();
				return true;
			}
			value = null;
			return false;
		}

		private static bool IsValidEmailAddress(EmailAddress emailAddress)
		{
			if (emailAddress != null)
			{
				if (StringComparer.OrdinalIgnoreCase.Equals(emailAddress.RoutingType, "EX") && !string.IsNullOrEmpty(emailAddress.OriginalDisplayName) && SmtpAddress.IsValidSmtpAddress(emailAddress.OriginalDisplayName))
				{
					return true;
				}
				if (StringComparer.OrdinalIgnoreCase.Equals(emailAddress.RoutingType, "SMTP") && !string.IsNullOrEmpty(emailAddress.Address))
				{
					return true;
				}
				if (!string.IsNullOrEmpty(emailAddress.RoutingType))
				{
					return true;
				}
				if (StringComparer.OrdinalIgnoreCase.Equals(emailAddress.RoutingType, string.Empty) && !string.IsNullOrEmpty(emailAddress.OriginalDisplayName))
				{
					return true;
				}
			}
			return false;
		}

		private static readonly Trace Tracer = ExTraceGlobals.PersonTracer;

		public static readonly PropertyAggregationStrategy FileAsIdProperty = new PropertyAggregationStrategy.SingleValuePropertyAggregation(ContactSelectionStrategy.FileAsIdProperty);

		public static readonly PropertyAggregationStrategy EmailAddressProperty = new PersonPropertyAggregationStrategy.EmailAddressAggregation();

		public static readonly PropertyAggregationStrategy EmailAddressesProperty = new PersonPropertyAggregationStrategy.EmailAddressesAggregation();

		public static readonly PropertyAggregationStrategy IMAddressProperty = new PersonPropertyAggregationStrategy.IMAddressAggregation();

		public static readonly PropertyAggregationStrategy GALLinkIDProperty = new PersonPropertyAggregationStrategy.GALLinkIDAggregation();

		public static readonly PropertyAggregationStrategy PostalAddressesProperty = new PersonPropertyAggregationStrategy.PostalAddressesAggregation(PostalAddressProperties.AllPropertiesForConversationView, new Func<PostalAddressProperties, IStorePropertyBag, PostalAddress>(PersonPropertyAggregationStrategy.PostalAddressesAggregation.GetFromAllPropertiesForConversationView));

		public static readonly PropertyAggregationStrategy PostalAddressesWithDetailsProperty = new PersonPropertyAggregationStrategy.PostalAddressesAggregation(PostalAddressProperties.AllProperties, new Func<PostalAddressProperties, IStorePropertyBag, PostalAddress>(PersonPropertyAggregationStrategy.PostalAddressesAggregation.GetFromAllProperties));

		public static readonly PropertyAggregationStrategy PostalAddressProperty = new PersonPropertyAggregationStrategy.PostalAddressAggregation();

		public static readonly PropertyAggregationStrategy IsPersonalContactProperty = new PersonPropertyAggregationStrategy.IsPersonalContactAggregation();

		public static readonly PropertyAggregationStrategy CreationTimeProperty = new PropertyAggregationStrategy.CreationTimeAggregation();

		public static readonly PropertyAggregationStrategy IsFavoriteProperty = new PersonPropertyAggregationStrategy.IsFavoriteAggregation();

		public static readonly PropertyAggregationStrategy RelevanceScoreProperty = new PersonPropertyAggregationStrategy.RelevanceScoreAggregation();

		public static readonly PropertyAggregationStrategy PhotoContactEntryIdProperty = new PropertyAggregationStrategy.SingleValuePropertyAggregation(ContactSelectionStrategy.PhotoContactIdProperty);

		public static readonly PropertyAggregationStrategy AttributedThirdPartyPhotoUrlsProperty = new PersonPropertyAggregationStrategy.AttributedThirdPartyPhotoUrlPropertyAggregation();

		public static readonly PropertyAggregationStrategy PersonIdProperty = new PersonPropertyAggregationStrategy.PersonIdAggregation();

		public static readonly PropertyAggregationStrategy PersonTypeProperty = new PersonPropertyAggregationStrategy.PersonTypeAggregation();

		public static readonly PropertyAggregationStrategy FolderIdsProperty = new PersonPropertyAggregationStrategy.FolderIdsAggregation();

		public static readonly PropertyAggregationStrategy AttributionsProperty = new PersonPropertyAggregationStrategy.AttributionsAggregation();

		public static readonly PropertyAggregationStrategy AttributedDisplayNamesProperty = new PersonPropertyAggregationStrategy.AttributedStringPropertyAggregation(StoreObjectSchema.DisplayName);

		public static readonly PropertyAggregationStrategy AttributedFileAsesProperty = new PersonPropertyAggregationStrategy.AttributedStringPropertyAggregation(ContactBaseSchema.FileAs);

		public static readonly PropertyAggregationStrategy AttributedFileAsIdsProperty = new PersonPropertyAggregationStrategy.AttributedAsStringPropertyAggregation<FileAsMapping>(ContactSchema.FileAsId);

		public static readonly PropertyAggregationStrategy AttributedChildrenProperty = new PersonPropertyAggregationStrategy.AttributedStringArrayPropertyAggregation(ContactSchema.Children);

		public static readonly PropertyAggregationStrategy AttributedDisplayNamePrefixesProperty = new PersonPropertyAggregationStrategy.AttributedStringPropertyAggregation(ContactSchema.DisplayNamePrefix);

		public static readonly PropertyAggregationStrategy AttributedGivenNamesProperty = new PersonPropertyAggregationStrategy.AttributedStringPropertyAggregation(ContactSchema.GivenName);

		public static readonly PropertyAggregationStrategy AttributedMiddleNamesProperty = new PersonPropertyAggregationStrategy.AttributedStringPropertyAggregation(ContactSchema.MiddleName);

		public static readonly PropertyAggregationStrategy AttributedSurnamesProperty = new PersonPropertyAggregationStrategy.AttributedStringPropertyAggregation(ContactSchema.Surname);

		public static readonly PropertyAggregationStrategy AttributedGenerationsProperty = new PersonPropertyAggregationStrategy.AttributedStringPropertyAggregation(ContactSchema.Generation);

		public static readonly PropertyAggregationStrategy AttributedNicknamesProperty = new PersonPropertyAggregationStrategy.AttributedStringPropertyAggregation(ContactSchema.Nickname);

		public static readonly PropertyAggregationStrategy AttributedInitialsProperty = new PersonPropertyAggregationStrategy.AttributedStringPropertyAggregation(ContactSchema.Initials);

		public static readonly PropertyAggregationStrategy AttributedYomiCompanyNamesProperty = new PersonPropertyAggregationStrategy.AttributedStringPropertyAggregation(ContactSchema.YomiCompany);

		public static readonly PropertyAggregationStrategy AttributedYomiFirstNamesProperty = new PersonPropertyAggregationStrategy.AttributedStringPropertyAggregation(ContactSchema.YomiFirstName);

		public static readonly PropertyAggregationStrategy AttributedYomiLastNamesProperty = new PersonPropertyAggregationStrategy.AttributedStringPropertyAggregation(ContactSchema.YomiLastName);

		public static readonly PropertyAggregationStrategy AttributedBusinessPhoneNumbersProperty = new PersonPropertyAggregationStrategy.AttributedPhoneNumberPropertyAggregation(ContactSchema.BusinessPhoneNumber, PersonPhoneNumberType.Business);

		public static readonly PropertyAggregationStrategy AttributedBusinessPhoneNumbers2Property = new PersonPropertyAggregationStrategy.AttributedPhoneNumberPropertyAggregation(ContactSchema.BusinessPhoneNumber2, PersonPhoneNumberType.Business);

		public static readonly PropertyAggregationStrategy AttributedHomePhonesProperty = new PersonPropertyAggregationStrategy.AttributedPhoneNumberPropertyAggregation(ContactSchema.HomePhone, PersonPhoneNumberType.Home);

		public static readonly PropertyAggregationStrategy AttributedHomePhones2Property = new PersonPropertyAggregationStrategy.AttributedPhoneNumberPropertyAggregation(ContactSchema.HomePhone2, PersonPhoneNumberType.Home);

		public static readonly PropertyAggregationStrategy AttributedMobilePhonesProperty = new PersonPropertyAggregationStrategy.AttributedPhoneNumberPropertyPlusProtectedPropertyAggregation(ContactSchema.MobilePhone, PersonPhoneNumberType.Mobile, InternalSchema.ProtectedPhoneNumber);

		public static readonly PropertyAggregationStrategy AttributedMobilePhones2Property = new PersonPropertyAggregationStrategy.AttributedPhoneNumberPropertyAggregation(ContactSchema.MobilePhone2, PersonPhoneNumberType.Mobile);

		public static readonly PropertyAggregationStrategy AttributedAssistantPhoneNumbersProperty = new PersonPropertyAggregationStrategy.AttributedPhoneNumberPropertyAggregation(ContactSchema.AssistantPhoneNumber, PersonPhoneNumberType.Assistant);

		public static readonly PropertyAggregationStrategy AttributedCallbackPhonesProperty = new PersonPropertyAggregationStrategy.AttributedPhoneNumberPropertyAggregation(ContactSchema.CallbackPhone, PersonPhoneNumberType.Callback);

		public static readonly PropertyAggregationStrategy AttributedCarPhonesProperty = new PersonPropertyAggregationStrategy.AttributedPhoneNumberPropertyAggregation(ContactSchema.CarPhone, PersonPhoneNumberType.Car);

		public static readonly PropertyAggregationStrategy AttributedHomeFaxesProperty = new PersonPropertyAggregationStrategy.AttributedPhoneNumberPropertyAggregation(ContactSchema.HomeFax, PersonPhoneNumberType.HomeFax);

		public static readonly PropertyAggregationStrategy AttributedOrganizationMainPhonesProperty = new PersonPropertyAggregationStrategy.AttributedPhoneNumberPropertyAggregation(ContactSchema.OrganizationMainPhone, PersonPhoneNumberType.OrganizationMain);

		public static readonly PropertyAggregationStrategy AttributedOtherFaxesProperty = new PersonPropertyAggregationStrategy.AttributedPhoneNumberPropertyAggregation(ContactSchema.OtherFax, PersonPhoneNumberType.OtherFax);

		public static readonly PropertyAggregationStrategy AttributedOtherTelephonesProperty = new PersonPropertyAggregationStrategy.AttributedPhoneNumberPropertyAggregation(ContactSchema.OtherTelephone, PersonPhoneNumberType.Other);

		public static readonly PropertyAggregationStrategy AttributedOtherPhones2Property = new PersonPropertyAggregationStrategy.AttributedPhoneNumberPropertyAggregation(ContactSchema.OtherPhone2, PersonPhoneNumberType.Other);

		public static readonly PropertyAggregationStrategy AttributedPagersProperty = new PersonPropertyAggregationStrategy.AttributedPhoneNumberPropertyAggregation(ContactSchema.Pager, PersonPhoneNumberType.Pager);

		public static readonly PropertyAggregationStrategy AttributedRadioPhonesProperty = new PersonPropertyAggregationStrategy.AttributedPhoneNumberPropertyAggregation(ContactSchema.RadioPhone, PersonPhoneNumberType.Radio);

		public static readonly PropertyAggregationStrategy AttributedTelexNumbersProperty = new PersonPropertyAggregationStrategy.AttributedPhoneNumberPropertyAggregation(ContactSchema.TelexNumber, PersonPhoneNumberType.Telex);

		public static readonly PropertyAggregationStrategy AttributedTtyTddPhoneNumbersProperty = new PersonPropertyAggregationStrategy.AttributedPhoneNumberPropertyAggregation(ContactSchema.TtyTddPhoneNumber, PersonPhoneNumberType.TTYTDD);

		public static readonly PropertyAggregationStrategy AttributedWorkFaxesProperty = new PersonPropertyAggregationStrategy.AttributedPhoneNumberPropertyAggregation(ContactSchema.WorkFax, PersonPhoneNumberType.BusinessFax);

		public static readonly PropertyAggregationStrategy AttributedEmails1Property = new PersonPropertyAggregationStrategy.AttributedEmailAddressPropertyPlusProtectedPropertyAggregation(EmailAddressProperties.Email1, InternalSchema.ProtectedEmailAddress);

		public static readonly PropertyAggregationStrategy AttributedEmails2Property = new PersonPropertyAggregationStrategy.AttributedEmailAddressPropertyAggregation(EmailAddressProperties.Email2);

		public static readonly PropertyAggregationStrategy AttributedEmails3Property = new PersonPropertyAggregationStrategy.AttributedEmailAddressPropertyAggregation(EmailAddressProperties.Email3);

		public static readonly PropertyAggregationStrategy AttributedBusinessHomePagesProperty = new PersonPropertyAggregationStrategy.AttributedStringPropertyAggregation(ContactSchema.BusinessHomePage);

		public static readonly PropertyAggregationStrategy AttributedSchoolsProperty = new PersonPropertyAggregationStrategy.AttributedStringPropertyAggregation(ContactSchema.Schools);

		public static readonly PropertyAggregationStrategy AttributedPersonalHomePagesProperty = new PersonPropertyAggregationStrategy.AttributedStringPropertyAggregation(ContactSchema.PersonalHomePage);

		public static readonly PropertyAggregationStrategy AttributedOfficeLocationsProperty = new PersonPropertyAggregationStrategy.AttributedStringPropertyAggregation(ContactSchema.OfficeLocation);

		public static readonly PropertyAggregationStrategy AttributedIMAddressesProperty = new PersonPropertyAggregationStrategy.AttributedStringPropertyAggregation(ContactSchema.IMAddress);

		public static readonly PropertyAggregationStrategy AttributedIMAddresses2Property = new PersonPropertyAggregationStrategy.AttributedStringPropertyAggregation(ContactSchema.IMAddress2);

		public static readonly PropertyAggregationStrategy AttributedIMAddresses3Property = new PersonPropertyAggregationStrategy.AttributedStringPropertyAggregation(ContactSchema.IMAddress3);

		public static readonly PropertyAggregationStrategy AttributedWorkAddressesProperty = new PersonPropertyAggregationStrategy.AttributedPostalAddressPropertyAggregation(PostalAddressProperties.WorkAddress);

		public static readonly PropertyAggregationStrategy AttributedHomeAddressesProperty = new PersonPropertyAggregationStrategy.AttributedPostalAddressPropertyAggregation(PostalAddressProperties.HomeAddress);

		public static readonly PropertyAggregationStrategy AttributedOtherAddressesProperty = new PersonPropertyAggregationStrategy.AttributedPostalAddressPropertyAggregation(PostalAddressProperties.OtherAddress);

		public static readonly PropertyAggregationStrategy AttributedTitlesProperty = new PersonPropertyAggregationStrategy.AttributedStringPropertyAggregation(ContactSchema.Title);

		public static readonly PropertyAggregationStrategy AttributedDepartmentsProperty = new PersonPropertyAggregationStrategy.AttributedStringPropertyAggregation(ContactSchema.Department);

		public static readonly PropertyAggregationStrategy AttributedCompanyNamesProperty = new PersonPropertyAggregationStrategy.AttributedStringPropertyAggregation(ContactSchema.CompanyName);

		public static readonly PropertyAggregationStrategy AttributedManagersProperty = new PersonPropertyAggregationStrategy.AttributedStringPropertyAggregation(ContactSchema.Manager);

		public static readonly PropertyAggregationStrategy AttributedAssistantNamesProperty = new PersonPropertyAggregationStrategy.AttributedStringPropertyAggregation(ContactSchema.AssistantName);

		public static readonly PropertyAggregationStrategy AttributedProfessionsProperty = new PersonPropertyAggregationStrategy.AttributedStringPropertyAggregation(ContactSchema.Profession);

		public static readonly PropertyAggregationStrategy AttributedSpouseNamesProperty = new PersonPropertyAggregationStrategy.AttributedStringPropertyAggregation(ContactSchema.SpouseName);

		public static readonly PropertyAggregationStrategy AttributedHobbiesProperty = new PersonPropertyAggregationStrategy.AttributedStringPropertyAggregation(ContactSchema.Hobbies);

		public static readonly PropertyAggregationStrategy AttributedWeddingAnniversariesProperty = new PersonPropertyAggregationStrategy.AttributedDateTimePropertyAggregation(ContactSchema.WeddingAnniversary);

		public static readonly PropertyAggregationStrategy AttributedBirthdaysProperty = new PersonPropertyAggregationStrategy.AttributedDateTimePropertyAggregation(ContactSchema.Birthday);

		public static readonly PropertyAggregationStrategy AttributedWeddingAnniversariesLocalProperty = new PersonPropertyAggregationStrategy.AttributedDateTimePropertyAggregation(ContactSchema.WeddingAnniversaryLocal);

		public static readonly PropertyAggregationStrategy AttributedBirthdaysLocalProperty = new PersonPropertyAggregationStrategy.AttributedDateTimePropertyAggregation(ContactSchema.BirthdayLocal);

		public static readonly PropertyAggregationStrategy AttributedLocationsProperty = new PersonPropertyAggregationStrategy.AttributedStringPropertyAggregation(ContactSchema.Location);

		private sealed class IsFavoriteAggregation : PropertyAggregationStrategy
		{
			public IsFavoriteAggregation() : base(new StorePropertyDefinition[]
			{
				InternalSchema.IsFavorite
			})
			{
			}

			protected override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				bool flag = false;
				foreach (IStorePropertyBag storePropertyBag in context.Sources)
				{
					bool valueOrDefault = storePropertyBag.GetValueOrDefault<bool>(InternalSchema.IsFavorite, false);
					if (valueOrDefault)
					{
						flag = true;
						break;
					}
				}
				value = flag;
				return true;
			}
		}

		private sealed class RelevanceScoreAggregation : PropertyAggregationStrategy
		{
			public RelevanceScoreAggregation() : base(new StorePropertyDefinition[]
			{
				InternalSchema.RelevanceScore
			})
			{
			}

			protected override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				int num = int.MaxValue;
				foreach (IStorePropertyBag storePropertyBag in context.Sources)
				{
					int valueOrDefault = storePropertyBag.GetValueOrDefault<int>(InternalSchema.RelevanceScore, int.MaxValue);
					if (valueOrDefault < num)
					{
						num = valueOrDefault;
					}
				}
				value = num;
				return true;
			}
		}

		private sealed class PersonIdAggregation : PropertyAggregationStrategy
		{
			public PersonIdAggregation() : base(new StorePropertyDefinition[]
			{
				ContactSchema.PersonId
			})
			{
			}

			protected override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				PersonId personId = null;
				foreach (IStorePropertyBag storePropertyBag in context.Sources)
				{
					PersonId valueOrDefault = storePropertyBag.GetValueOrDefault<PersonId>(ContactSchema.PersonId, null);
					if (personId != null && valueOrDefault != null && !valueOrDefault.Equals(personId))
					{
						throw new ArgumentException("sources", "Property bag collection should have same personId");
					}
					if (personId == null)
					{
						personId = valueOrDefault;
					}
				}
				value = personId;
				return true;
			}
		}

		private sealed class EmailAddressAggregation : PropertyAggregationStrategy
		{
			public EmailAddressAggregation() : base(PropertyDefinitionCollection.Merge<StorePropertyDefinition>(ContactEmailAddressesEnumerator.Properties, new StorePropertyDefinition[]
			{
				ContactSchema.PartnerNetworkId,
				ContactSchema.RelevanceScore,
				ContactBaseSchema.DisplayNameFirstLast,
				StoreObjectSchema.ItemClass,
				StoreObjectSchema.EntryId,
				StoreObjectSchema.ChangeKey,
				ContactSchema.SmtpAddressCache
			}))
			{
			}

			internal static int GetAdjustedRelevanceScore(IStorePropertyBag source, string partnerNetworkId)
			{
				int result;
				if (string.Equals(partnerNetworkId, WellKnownNetworkNames.GAL))
				{
					result = 2147483645;
				}
				else
				{
					result = source.GetValueOrDefault<int>(ContactSchema.RelevanceScore, int.MaxValue);
				}
				return result;
			}

			internal static bool HasSMTPAddressCacheMatch(string[] smtpAddressCache, string stmpAddress)
			{
				string y = "SMTP:" + stmpAddress;
				foreach (string x in smtpAddressCache)
				{
					if (StringComparer.OrdinalIgnoreCase.Equals(x, y))
					{
						return true;
					}
				}
				return false;
			}

			protected override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				value = null;
				int num = int.MaxValue;
				foreach (IStorePropertyBag storePropertyBag in context.Sources)
				{
					if (context.Sources.Count == 1)
					{
						string valueOrDefault = storePropertyBag.GetValueOrDefault<string>(StoreObjectSchema.ItemClass, string.Empty);
						if (ObjectClass.IsDistributionList(valueOrDefault))
						{
							Participant valueOrDefault2 = storePropertyBag.GetValueOrDefault<Participant>(InternalSchema.DistributionListParticipant, null);
							if (valueOrDefault2 != null)
							{
								value = valueOrDefault2;
								break;
							}
						}
					}
					string valueOrDefault3 = storePropertyBag.GetValueOrDefault<string>(ContactSchema.PartnerNetworkId, string.Empty);
					string[] valueOrDefault4 = storePropertyBag.GetValueOrDefault<string[]>(ContactSchema.SmtpAddressCache, Array<string>.Empty);
					int adjustedRelevanceScore = PersonPropertyAggregationStrategy.EmailAddressAggregation.GetAdjustedRelevanceScore(storePropertyBag, valueOrDefault3);
					PersonPropertyAggregationContext personPropertyAggregationContext = (PersonPropertyAggregationContext)context;
					ContactEmailAddressesEnumerator contactEmailAddressesEnumerator = new ContactEmailAddressesEnumerator(storePropertyBag, personPropertyAggregationContext.ClientInfoString);
					foreach (Tuple<EmailAddress, EmailAddressIndex> tuple in contactEmailAddressesEnumerator)
					{
						EmailAddress item = tuple.Item1;
						EmailAddressIndex item2 = tuple.Item2;
						if (!string.IsNullOrEmpty(item.Address))
						{
							string text = null;
							if (string.Equals(item.RoutingType, "SMTP", StringComparison.OrdinalIgnoreCase))
							{
								if (SmtpAddress.IsValidSmtpAddress(item.Address))
								{
									text = item.Address;
								}
							}
							else if (string.Equals(item.RoutingType, "EX", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(item.OriginalDisplayName) && SmtpAddress.IsValidSmtpAddress(item.OriginalDisplayName))
							{
								text = item.OriginalDisplayName;
							}
							else if (!string.IsNullOrEmpty(item.RoutingType))
							{
								text = item.Address;
							}
							if (!string.IsNullOrEmpty(text) && (num > adjustedRelevanceScore || value == null))
							{
								ParticipantOrigin origin;
								if (valueOrDefault3.Equals(WellKnownNetworkNames.GAL) || PersonPropertyAggregationStrategy.EmailAddressAggregation.HasSMTPAddressCacheMatch(valueOrDefault4, text))
								{
									origin = new DirectoryParticipantOrigin(storePropertyBag);
								}
								else
								{
									origin = new StoreParticipantOrigin(storePropertyBag.GetValueOrDefault<VersionedId>(ItemSchema.Id, null), item2);
								}
								value = new Participant(item.Name, item.Address, item.RoutingType, item.OriginalDisplayName, origin, new KeyValuePair<PropertyDefinition, object>[0]);
								num = adjustedRelevanceScore;
							}
						}
					}
				}
				return value != null;
			}

			private const int DefaultGalContactRelevanceScore = 2147483645;
		}

		private sealed class EmailAddressesAggregation : PropertyAggregationStrategy
		{
			public EmailAddressesAggregation() : base(PropertyDefinitionCollection.Merge<StorePropertyDefinition>(ContactEmailAddressesEnumerator.Properties, new StorePropertyDefinition[]
			{
				ContactSchema.PartnerNetworkId,
				ContactSchema.RelevanceScore,
				ContactBaseSchema.DisplayNameFirstLast,
				StoreObjectSchema.ItemClass,
				StoreObjectSchema.EntryId,
				StoreObjectSchema.ChangeKey,
				ContactSchema.SmtpAddressCache
			}))
			{
			}

			protected override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				Dictionary<string, Pair<Participant, int>> emailAddressDictionary = new Dictionary<string, Pair<Participant, int>>(context.Sources.Count * EmailAddressProperties.PropertySets.Length, StringComparer.OrdinalIgnoreCase);
				foreach (IStorePropertyBag storePropertyBag in context.Sources)
				{
					if (context.Sources.Count == 1)
					{
						string valueOrDefault = storePropertyBag.GetValueOrDefault<string>(StoreObjectSchema.ItemClass, string.Empty);
						if (ObjectClass.IsDistributionList(valueOrDefault))
						{
							Participant valueOrDefault2 = storePropertyBag.GetValueOrDefault<Participant>(InternalSchema.DistributionListParticipant, null);
							if (valueOrDefault2 != null)
							{
								value = new Participant[]
								{
									valueOrDefault2
								};
								return true;
							}
							value = null;
							return false;
						}
					}
					string[] valueOrDefault3 = storePropertyBag.GetValueOrDefault<string[]>(ContactSchema.SmtpAddressCache, Array<string>.Empty);
					string valueOrDefault4 = storePropertyBag.GetValueOrDefault<string>(ContactSchema.PartnerNetworkId, string.Empty);
					PersonPropertyAggregationContext personPropertyAggregationContext = (PersonPropertyAggregationContext)context;
					ContactEmailAddressesEnumerator contactEmailAddressesEnumerator = new ContactEmailAddressesEnumerator(storePropertyBag, personPropertyAggregationContext.ClientInfoString);
					foreach (Tuple<EmailAddress, EmailAddressIndex> tuple in contactEmailAddressesEnumerator)
					{
						EmailAddress item = tuple.Item1;
						EmailAddressIndex item2 = tuple.Item2;
						if (PersonPropertyAggregationStrategy.IsValidEmailAddress(item))
						{
							ParticipantOrigin origin;
							if (valueOrDefault4.Equals(WellKnownNetworkNames.GAL) || PersonPropertyAggregationStrategy.EmailAddressAggregation.HasSMTPAddressCacheMatch(valueOrDefault3, item.Address))
							{
								origin = new DirectoryParticipantOrigin(storePropertyBag);
							}
							else
							{
								origin = new StoreParticipantOrigin(storePropertyBag.GetValueOrDefault<VersionedId>(ItemSchema.Id, null), item2);
							}
							Participant participant = new Participant(item.Name, item.Address, item.RoutingType, item.OriginalDisplayName, origin, new KeyValuePair<PropertyDefinition, object>[0]);
							int adjustedRelevanceScore = PersonPropertyAggregationStrategy.EmailAddressAggregation.GetAdjustedRelevanceScore(storePropertyBag, valueOrDefault4);
							string text;
							if (string.Equals(item.RoutingType, string.Empty, StringComparison.OrdinalIgnoreCase) || (string.Equals(item.RoutingType, "EX", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(participant.OriginalDisplayName)))
							{
								text = participant.OriginalDisplayName;
							}
							else
							{
								text = participant.EmailAddress;
							}
							if (!string.IsNullOrEmpty(text))
							{
								if (emailAddressDictionary.ContainsKey(text))
								{
									if (adjustedRelevanceScore < emailAddressDictionary[text].Second)
									{
										emailAddressDictionary[text] = new Pair<Participant, int>(participant, adjustedRelevanceScore);
									}
								}
								else
								{
									emailAddressDictionary.Add(text, new Pair<Participant, int>(participant, adjustedRelevanceScore));
								}
							}
						}
					}
				}
				IEnumerable<Participant> source = from k in emailAddressDictionary.Keys
				orderby emailAddressDictionary[k].Second
				select emailAddressDictionary[k].First;
				if (source.Count<Participant>() > 0)
				{
					value = source.ToArray<Participant>();
					return true;
				}
				value = null;
				return false;
			}
		}

		private sealed class IMAddressAggregation : PropertyAggregationStrategy
		{
			public IMAddressAggregation() : base(PropertyDefinitionCollection.Merge<StorePropertyDefinition>(PersonPropertyAggregationStrategy.IMAddressAggregation.IMProperties, new StorePropertyDefinition[]
			{
				ContactSchema.PartnerNetworkId
			}))
			{
			}

			protected override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				value = null;
				string text = string.Empty;
				foreach (IStorePropertyBag storePropertyBag in context.Sources)
				{
					string valueOrDefault = storePropertyBag.GetValueOrDefault<string>(ContactSchema.PartnerNetworkId, string.Empty);
					bool flag = StringComparer.OrdinalIgnoreCase.Equals(valueOrDefault, WellKnownNetworkNames.GAL);
					bool flag2 = StringComparer.OrdinalIgnoreCase.Equals(valueOrDefault, WellKnownNetworkNames.RecipientCache);
					if (flag || flag2 || value == null)
					{
						StorePropertyDefinition[] improperties = PersonPropertyAggregationStrategy.IMAddressAggregation.IMProperties;
						int i = 0;
						while (i < improperties.Length)
						{
							PropertyDefinition propertyDefinition = improperties[i];
							string valueOrDefault2 = storePropertyBag.GetValueOrDefault<string>(propertyDefinition, string.Empty);
							if (!string.IsNullOrEmpty(valueOrDefault2))
							{
								value = valueOrDefault2;
								if (flag)
								{
									return true;
								}
								if (flag2)
								{
									text = valueOrDefault2;
									break;
								}
								break;
							}
							else
							{
								i++;
							}
						}
					}
				}
				if (!text.Equals(string.Empty))
				{
					value = text;
				}
				return value != null;
			}

			private static readonly StorePropertyDefinition[] IMProperties = new StorePropertyDefinition[]
			{
				ContactSchema.IMAddress,
				ContactSchema.IMAddress2,
				ContactSchema.IMAddress3
			};
		}

		private sealed class GALLinkIDAggregation : PropertyAggregationStrategy
		{
			public GALLinkIDAggregation() : base(PersonPropertyAggregationStrategy.GALLinkIDAggregation.GALLinkIDProperties)
			{
			}

			protected override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				value = null;
				foreach (IStorePropertyBag storePropertyBag in context.Sources)
				{
					string valueOrDefault = storePropertyBag.GetValueOrDefault<string>(ContactSchema.PartnerNetworkId, string.Empty);
					bool flag = StringComparer.OrdinalIgnoreCase.Equals(valueOrDefault, WellKnownNetworkNames.GAL);
					Guid? valueOrDefault2 = storePropertyBag.GetValueOrDefault<Guid?>(ContactSchema.GALLinkID, null);
					if (valueOrDefault2 != null)
					{
						value = valueOrDefault2;
						if (flag)
						{
							return true;
						}
					}
				}
				return value != null;
			}

			private static readonly StorePropertyDefinition[] GALLinkIDProperties = new StorePropertyDefinition[]
			{
				ContactSchema.GALLinkID,
				ContactSchema.PartnerNetworkId
			};
		}

		private sealed class PostalAddressesAggregation : PropertyAggregationStrategy
		{
			public static PostalAddress GetFromAllProperties(PostalAddressProperties properties, IStorePropertyBag propertyBag)
			{
				return properties.GetFromAllPropertiesForConversationView(propertyBag);
			}

			public static PostalAddress GetFromAllPropertiesForConversationView(PostalAddressProperties properties, IStorePropertyBag propertyBag)
			{
				return properties.GetFromAllPropertiesForConversationView(propertyBag);
			}

			public PostalAddressesAggregation(NativeStorePropertyDefinition[] properties, Func<PostalAddressProperties, IStorePropertyBag, PostalAddress> postalAddressFactory) : base(properties)
			{
				this.postalAddressFactory = postalAddressFactory;
			}

			protected override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				List<PostalAddress> list = new List<PostalAddress>(context.Sources.Count * PostalAddressProperties.PropertySets.Length);
				foreach (IStorePropertyBag arg in context.Sources)
				{
					foreach (PostalAddressProperties arg2 in PostalAddressProperties.PropertySets)
					{
						PostalAddress postalAddress = this.postalAddressFactory(arg2, arg);
						if (postalAddress != null)
						{
							list.Add(postalAddress);
						}
					}
				}
				return PersonPropertyAggregationStrategy.TryGetArrayResult<PostalAddress>(list, out value);
			}

			private Func<PostalAddressProperties, IStorePropertyBag, PostalAddress> postalAddressFactory;
		}

		private sealed class IsPersonalContactAggregation : PropertyAggregationStrategy
		{
			public IsPersonalContactAggregation() : base(new StorePropertyDefinition[0])
			{
			}

			protected sealed override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				value = true;
				return true;
			}
		}

		private sealed class PostalAddressAggregation : PropertyAggregationStrategy
		{
			public PostalAddressAggregation() : base(PostalAddressProperties.AllProperties)
			{
			}

			protected sealed override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				foreach (IStorePropertyBag propertyBag in context.Sources)
				{
					PostalAddress fromAllProperties = PostalAddressProperties.HomeAddress.GetFromAllProperties(propertyBag);
					if (fromAllProperties == null)
					{
						fromAllProperties = PostalAddressProperties.WorkAddress.GetFromAllProperties(propertyBag);
						if (fromAllProperties == null)
						{
							fromAllProperties = PostalAddressProperties.OtherAddress.GetFromAllProperties(propertyBag);
						}
					}
					if (fromAllProperties != null)
					{
						value = fromAllProperties.ToString();
						return true;
					}
				}
				value = null;
				return false;
			}
		}

		private sealed class PersonTypeAggregation : PropertyAggregationStrategy
		{
			public PersonTypeAggregation() : base(new StorePropertyDefinition[]
			{
				ContactSchema.PersonType
			})
			{
			}

			protected sealed override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				value = PersonType.Person;
				foreach (IStorePropertyBag storePropertyBag in context.Sources)
				{
					object obj = storePropertyBag.TryGetProperty(ContactSchema.PersonType);
					if (obj is PersonType)
					{
						value = (PersonType)obj;
						break;
					}
				}
				return true;
			}
		}

		private sealed class FolderIdsAggregation : PropertyAggregationStrategy
		{
			public FolderIdsAggregation() : base(new StorePropertyDefinition[]
			{
				StoreObjectSchema.ParentItemId,
				ContactSchema.IsFavorite
			})
			{
			}

			protected sealed override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				List<StoreObjectId> list = new List<StoreObjectId>();
				foreach (IStorePropertyBag storePropertyBag in context.Sources)
				{
					StoreObjectId valueOrDefault = storePropertyBag.GetValueOrDefault<StoreObjectId>(StoreObjectSchema.ParentItemId, null);
					if (valueOrDefault != null && !list.Contains(valueOrDefault))
					{
						list.Add(valueOrDefault);
						this.CalculateFolderIdBasedOnMyContactsFolderInfo(context as PersonPropertyAggregationContext, list, valueOrDefault);
						this.CalculateFolderIdBasedOnFavoriteInfo(context as PersonPropertyAggregationContext, storePropertyBag, list, valueOrDefault);
					}
				}
				return PersonPropertyAggregationStrategy.TryGetArrayResult<StoreObjectId>(list, out value);
			}

			private void CalculateFolderIdBasedOnMyContactsFolderInfo(PersonPropertyAggregationContext context, List<StoreObjectId> folderIdList, StoreObjectId parentId)
			{
				if (context != null && context.ContactFolders != null && context.ContactFolders.MyContactFolders.Contains(parentId) && !folderIdList.Contains(context.ContactFolders.MyContactsSearchFolderId))
				{
					folderIdList.Add(context.ContactFolders.MyContactsSearchFolderId);
				}
			}

			private void CalculateFolderIdBasedOnFavoriteInfo(PersonPropertyAggregationContext context, IStorePropertyBag propertyBag, List<StoreObjectId> folderIdList, StoreObjectId parentId)
			{
				if (context != null && context.ContactFolders != null && context.ContactFolders.QuickContactsFolderId != null && context.ContactFolders.FavoritesFolderId != null)
				{
					bool valueOrDefault = propertyBag.GetValueOrDefault<bool>(ContactSchema.IsFavorite, false);
					if (context.ContactFolders.QuickContactsFolderId.Equals(parentId) && valueOrDefault && !folderIdList.Contains(context.ContactFolders.FavoritesFolderId))
					{
						folderIdList.Add(context.ContactFolders.FavoritesFolderId);
					}
				}
			}
		}

		private sealed class AttributionsAggregation : PropertyAggregationStrategy
		{
			public AttributionsAggregation() : base(new StorePropertyDefinition[]
			{
				ItemSchema.Id,
				ContactSchema.PartnerNetworkId,
				StoreObjectSchema.ParentItemId,
				ItemSchema.ParentDisplayName,
				ContactSchema.GALLinkID
			})
			{
			}

			protected sealed override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				List<Attribution> list = new List<Attribution>(4);
				PersonPropertyAggregationContext personPropertyAggregationContext = (PersonPropertyAggregationContext)context;
				int i = 0;
				while (i < context.Sources.Count)
				{
					Attribution attribution = new Attribution();
					IStorePropertyBag storePropertyBag = context.Sources[i];
					string valueOrDefault = storePropertyBag.GetValueOrDefault<string>(ContactSchema.PartnerNetworkId, null);
					attribution.Id = i.ToString();
					if (valueOrDefault != null && valueOrDefault.Equals(WellKnownNetworkNames.GAL, StringComparison.OrdinalIgnoreCase))
					{
						Guid valueOrDefault2 = storePropertyBag.GetValueOrDefault<Guid>(ContactSchema.GALLinkID, Guid.Empty);
						if (!(valueOrDefault2 == Guid.Empty))
						{
							attribution.SourceId = AttributionSourceId.CreateFrom(valueOrDefault2);
							goto IL_C9;
						}
						PersonPropertyAggregationStrategy.Tracer.TraceError(0L, "Invalid GAL Contact since it doesn't have GALLinkID set, skip aggregating it.");
					}
					else
					{
						VersionedId valueOrDefault3 = storePropertyBag.GetValueOrDefault<VersionedId>(ItemSchema.Id, null);
						if (valueOrDefault3 == null)
						{
							throw new CorruptDataException(ServerStrings.ExContactHasNoId);
						}
						attribution.SourceId = AttributionSourceId.CreateFrom(valueOrDefault3);
						goto IL_C9;
					}
					IL_17B:
					i++;
					continue;
					IL_C9:
					attribution.IsHidden = false;
					attribution.DisplayName = storePropertyBag.GetValueOrDefault<string>(ContactSchema.AttributionDisplayName, WellKnownNetworkNames.Outlook);
					attribution.IsWritable = storePropertyBag.GetValueOrDefault<bool>(ContactSchema.IsWritable, true);
					StoreObjectId valueOrDefault4 = storePropertyBag.GetValueOrDefault<StoreObjectId>(StoreObjectSchema.ParentItemId, null);
					attribution.IsQuickContact = (valueOrDefault4 != null && personPropertyAggregationContext.ContactFolders != null && personPropertyAggregationContext.ContactFolders.QuickContactsFolderId != null && valueOrDefault4.Equals(personPropertyAggregationContext.ContactFolders.QuickContactsFolderId));
					if (attribution.DisplayName.Equals(WellKnownNetworkNames.Outlook, StringComparison.OrdinalIgnoreCase))
					{
						attribution.FolderId = valueOrDefault4;
						attribution.FolderName = storePropertyBag.GetValueOrDefault<string>(ItemSchema.ParentDisplayName, null);
					}
					else
					{
						attribution.FolderId = null;
					}
					list.Add(attribution);
					goto IL_17B;
				}
				return PersonPropertyAggregationStrategy.TryGetArrayResult<Attribution>(list, out value);
			}
		}

		private abstract class AttributedPropertyAggregation<T> : PropertyAggregationStrategy
		{
			protected AttributedPropertyAggregation(params StorePropertyDefinition[] propertyDefinitions) : base(propertyDefinitions)
			{
			}

			protected sealed override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				List<AttributedValue<T>> list = new List<AttributedValue<T>>(context.Sources.Count);
				IList<IStorePropertyBag> list2 = this.SortSources(context.Sources);
				for (int i = 0; i < list2.Count; i++)
				{
					T value2;
					if (this.TryGetSingleValue(list2[i], out value2))
					{
						AttributedValue<T> attributedValue = new AttributedValue<T>(value2, new string[]
						{
							i.ToString(CultureInfo.InvariantCulture)
						});
						AttributedValue<T>.AddToList(list, attributedValue);
					}
				}
				return PersonPropertyAggregationStrategy.TryGetArrayResult<AttributedValue<T>>(list, out value);
			}

			protected abstract bool TryGetSingleValue(IStorePropertyBag source, out T singleValue);

			protected virtual IList<IStorePropertyBag> SortSources(IList<IStorePropertyBag> sources)
			{
				return sources;
			}
		}

		private abstract class AttributedSinglePropertyAggregation<T> : PersonPropertyAggregationStrategy.AttributedPropertyAggregation<T>
		{
			protected AttributedSinglePropertyAggregation(StorePropertyDefinition property, StorePropertyDefinition[] additionalDependencies) : base(PropertyDefinitionCollection.Merge<StorePropertyDefinition>(additionalDependencies, new StorePropertyDefinition[]
			{
				property
			}))
			{
				this.property = property;
			}

			protected AttributedSinglePropertyAggregation(StorePropertyDefinition property) : this(property, Array<StorePropertyDefinition>.Empty)
			{
			}

			public StorePropertyDefinition Property
			{
				get
				{
					return this.property;
				}
			}

			private readonly StorePropertyDefinition property;
		}

		private class AttributedStringPropertyAggregation : PersonPropertyAggregationStrategy.AttributedSinglePropertyAggregation<string>
		{
			public AttributedStringPropertyAggregation(StorePropertyDefinition property, StorePropertyDefinition[] additionalDependencies) : base(property, PropertyDefinitionCollection.Merge<StorePropertyDefinition>(additionalDependencies, new StorePropertyDefinition[]
			{
				property
			}))
			{
			}

			public AttributedStringPropertyAggregation(StorePropertyDefinition property) : this(property, Array<StorePropertyDefinition>.Empty)
			{
			}

			protected override bool TryGetSingleValue(IStorePropertyBag source, out string singleValue)
			{
				string text = source.TryGetProperty(base.Property) as string;
				if (!string.IsNullOrWhiteSpace(text))
				{
					singleValue = text;
					return true;
				}
				singleValue = null;
				return false;
			}
		}

		private sealed class AttributedStringArrayPropertyAggregation : PersonPropertyAggregationStrategy.AttributedSinglePropertyAggregation<string[]>
		{
			public AttributedStringArrayPropertyAggregation(StorePropertyDefinition property) : base(property)
			{
			}

			protected override bool TryGetSingleValue(IStorePropertyBag source, out string[] singleValue)
			{
				string[] valueOrDefault = source.GetValueOrDefault<string[]>(base.Property, null);
				if (valueOrDefault != null && valueOrDefault.Length > 0)
				{
					singleValue = valueOrDefault;
					return true;
				}
				singleValue = null;
				return false;
			}
		}

		private sealed class AttributedAsStringPropertyAggregation<T> : PersonPropertyAggregationStrategy.AttributedSinglePropertyAggregation<string>
		{
			public AttributedAsStringPropertyAggregation(StorePropertyDefinition property) : base(property)
			{
			}

			protected override bool TryGetSingleValue(IStorePropertyBag source, out string singleValue)
			{
				object obj = source.TryGetProperty(base.Property);
				if (obj != null && !(obj is PropertyError))
				{
					T t = (T)((object)obj);
					if (t != null)
					{
						singleValue = t.ToString();
						return true;
					}
				}
				singleValue = null;
				return false;
			}
		}

		private sealed class AttributedDateTimePropertyAggregation : PersonPropertyAggregationStrategy.AttributedSinglePropertyAggregation<ExDateTime>
		{
			public AttributedDateTimePropertyAggregation(StorePropertyDefinition property) : base(property)
			{
			}

			protected override bool TryGetSingleValue(IStorePropertyBag source, out ExDateTime singleValue)
			{
				ExDateTime valueOrDefault = source.GetValueOrDefault<ExDateTime>(base.Property, PersonPropertyAggregationStrategy.AttributedDateTimePropertyAggregation.nullTime);
				if (!valueOrDefault.Equals(PersonPropertyAggregationStrategy.AttributedDateTimePropertyAggregation.nullTime))
				{
					singleValue = valueOrDefault;
					return true;
				}
				singleValue = default(ExDateTime);
				return false;
			}

			private static readonly ExDateTime nullTime = default(ExDateTime);
		}

		private sealed class AttributedPhoneNumberPropertyAggregation : PersonPropertyAggregationStrategy.AttributedSinglePropertyAggregation<PhoneNumber>
		{
			public AttributedPhoneNumberPropertyAggregation(StorePropertyDefinition property, PersonPhoneNumberType type) : base(property)
			{
				this.type = type;
			}

			protected override bool TryGetSingleValue(IStorePropertyBag source, out PhoneNumber singleValue)
			{
				string text = source.TryGetProperty(base.Property) as string;
				if (!string.IsNullOrWhiteSpace(text))
				{
					singleValue = new PhoneNumber
					{
						Number = text,
						Type = this.type
					};
					return true;
				}
				singleValue = null;
				return false;
			}

			private readonly PersonPhoneNumberType type;
		}

		private abstract class AttributedProtectedPropertyAggregation<T> : PropertyAggregationStrategy
		{
			public AttributedProtectedPropertyAggregation(params StorePropertyDefinition[] properties) : base(PropertyDefinitionCollection.Merge<StorePropertyDefinition>(properties, new StorePropertyDefinition[]
			{
				ContactSchema.PartnerNetworkId
			}))
			{
			}

			protected sealed override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				PersonPropertyAggregationContext personPropertyAggregationContext = (PersonPropertyAggregationContext)context;
				List<AttributedValue<T>> list = new List<AttributedValue<T>>(context.Sources.Count);
				for (int i = 0; i < context.Sources.Count; i++)
				{
					IStorePropertyBag storePropertyBag = context.Sources[i];
					string valueOrDefault = storePropertyBag.GetValueOrDefault<string>(ContactSchema.PartnerNetworkId, string.Empty);
					T value2;
					bool flag;
					if (StringComparer.OrdinalIgnoreCase.Equals(valueOrDefault, WellKnownNetworkNames.Facebook))
					{
						if (ClientInfo.OWA.IsMatch(personPropertyAggregationContext.ClientInfoString))
						{
							flag = this.TryGetProtectedValue(storePropertyBag, out value2);
						}
						else
						{
							flag = false;
							value2 = default(T);
						}
					}
					else
					{
						flag = this.TryGetRegularValue(storePropertyBag, out value2);
					}
					if (flag)
					{
						AttributedValue<T> attributedValue = new AttributedValue<T>(value2, new string[]
						{
							i.ToString()
						});
						AttributedValue<T>.AddToList(list, attributedValue);
					}
				}
				return PersonPropertyAggregationStrategy.TryGetArrayResult<AttributedValue<T>>(list, out value);
			}

			protected abstract bool TryGetRegularValue(IStorePropertyBag source, out T value);

			protected abstract bool TryGetProtectedValue(IStorePropertyBag source, out T value);
		}

		private sealed class AttributedEmailAddressPropertyPlusProtectedPropertyAggregation : PersonPropertyAggregationStrategy.AttributedProtectedPropertyAggregation<Participant>
		{
			public AttributedEmailAddressPropertyPlusProtectedPropertyAggregation(EmailAddressProperties properties, StorePropertyDefinition protectedProperty) : base(PropertyDefinitionCollection.Merge<StorePropertyDefinition>(properties.Properties, new StorePropertyDefinition[]
			{
				protectedProperty
			}))
			{
				this.properties = properties;
				this.protectedProperty = protectedProperty;
			}

			protected override bool TryGetRegularValue(IStorePropertyBag source, out Participant singleValue)
			{
				EmailAddress from = this.properties.GetFrom(source);
				singleValue = PersonPropertyAggregationStrategy.AttributedEmailAddressPropertyAggregation.GetParticipant(source, from);
				return singleValue != null;
			}

			protected override bool TryGetProtectedValue(IStorePropertyBag source, out Participant singleValue)
			{
				string text = source.TryGetProperty(this.protectedProperty) as string;
				if (!string.IsNullOrWhiteSpace(text))
				{
					EmailAddress emailAddress = new EmailAddress
					{
						RoutingType = "smtp",
						Address = text,
						Name = text
					};
					singleValue = PersonPropertyAggregationStrategy.AttributedEmailAddressPropertyAggregation.GetParticipant(source, emailAddress);
					return singleValue != null;
				}
				singleValue = null;
				return false;
			}

			private readonly EmailAddressProperties properties;

			private readonly PropertyDefinition protectedProperty;
		}

		private sealed class AttributedEmailAddressPropertyAggregation : PersonPropertyAggregationStrategy.AttributedPropertyAggregation<Participant>
		{
			public AttributedEmailAddressPropertyAggregation(EmailAddressProperties properties) : base(properties.Properties)
			{
				this.properties = properties;
			}

			internal static Participant GetParticipant(IStorePropertyBag source, EmailAddress emailAddress)
			{
				Participant result = null;
				if (PersonPropertyAggregationStrategy.IsValidEmailAddress(emailAddress))
				{
					string[] valueOrDefault = source.GetValueOrDefault<string[]>(ContactSchema.SmtpAddressCache, Array<string>.Empty);
					string valueOrDefault2 = source.GetValueOrDefault<string>(ContactSchema.PartnerNetworkId, string.Empty);
					ParticipantOrigin origin;
					if (valueOrDefault2.Equals(WellKnownNetworkNames.GAL) || PersonPropertyAggregationStrategy.EmailAddressAggregation.HasSMTPAddressCacheMatch(valueOrDefault, emailAddress.Address))
					{
						origin = new DirectoryParticipantOrigin(source);
					}
					else
					{
						origin = new StoreParticipantOrigin(source.GetValueOrDefault<VersionedId>(ItemSchema.Id, null));
					}
					result = new Participant(emailAddress.Name, emailAddress.Address, emailAddress.RoutingType, emailAddress.OriginalDisplayName, origin, new KeyValuePair<PropertyDefinition, object>[0]);
				}
				return result;
			}

			protected override bool TryGetSingleValue(IStorePropertyBag source, out Participant singleValue)
			{
				EmailAddress from = this.properties.GetFrom(source);
				singleValue = PersonPropertyAggregationStrategy.AttributedEmailAddressPropertyAggregation.GetParticipant(source, from);
				return singleValue != null;
			}

			private readonly EmailAddressProperties properties;
		}

		private sealed class AttributedPostalAddressPropertyAggregation : PersonPropertyAggregationStrategy.AttributedPropertyAggregation<PostalAddress>
		{
			public AttributedPostalAddressPropertyAggregation(PostalAddressProperties properties) : base(properties.Properties)
			{
				this.properties = properties;
			}

			protected override bool TryGetSingleValue(IStorePropertyBag source, out PostalAddress singleValue)
			{
				PostalAddress fromAllProperties = this.properties.GetFromAllProperties(source);
				if (fromAllProperties != null)
				{
					bool flag = fromAllProperties.Latitude != null && fromAllProperties.Longitude != null;
					if (!string.IsNullOrWhiteSpace(fromAllProperties.Street) || !string.IsNullOrWhiteSpace(fromAllProperties.City) || !string.IsNullOrWhiteSpace(fromAllProperties.State) || !string.IsNullOrWhiteSpace(fromAllProperties.Country) || !string.IsNullOrWhiteSpace(fromAllProperties.PostalCode) || !string.IsNullOrWhiteSpace(fromAllProperties.PostOfficeBox) || flag)
					{
						singleValue = fromAllProperties;
						return true;
					}
				}
				singleValue = null;
				return false;
			}

			private readonly PostalAddressProperties properties;
		}

		private sealed class ExtendedPropertiesAggregation : PropertyAggregationStrategy
		{
			public ExtendedPropertiesAggregation(StorePropertyDefinition[] extendedProperties) : base(extendedProperties)
			{
				this.extendedProperties = extendedProperties;
			}

			protected override bool TryAggregate(PropertyAggregationContext context, out object value)
			{
				List<AttributedValue<ContactExtendedPropertyData>> list = new List<AttributedValue<ContactExtendedPropertyData>>(context.Sources.Count * this.extendedProperties.Length);
				for (int i = 0; i < context.Sources.Count; i++)
				{
					string[] attributions = new string[]
					{
						i.ToString()
					};
					IStorePropertyBag storePropertyBag = context.Sources[i];
					foreach (StorePropertyDefinition propertyDefinition in this.extendedProperties)
					{
						object obj = storePropertyBag.TryGetProperty(propertyDefinition);
						if (obj != null && !(obj is PropertyError))
						{
							AttributedValue<ContactExtendedPropertyData> attributedValue = new AttributedValue<ContactExtendedPropertyData>(new ContactExtendedPropertyData(propertyDefinition, obj), attributions);
							AttributedValue<ContactExtendedPropertyData>.AddToList(list, attributedValue);
						}
					}
				}
				return PersonPropertyAggregationStrategy.TryGetArrayResult<AttributedValue<ContactExtendedPropertyData>>(list, out value);
			}

			private StorePropertyDefinition[] extendedProperties;
		}

		private sealed class AttributedPhoneNumberPropertyPlusProtectedPropertyAggregation : PersonPropertyAggregationStrategy.AttributedProtectedPropertyAggregation<PhoneNumber>
		{
			public AttributedPhoneNumberPropertyPlusProtectedPropertyAggregation(StorePropertyDefinition regularProperty, PersonPhoneNumberType type, StorePropertyDefinition protectedProperty) : base(new StorePropertyDefinition[]
			{
				regularProperty,
				protectedProperty,
				ContactSchema.PartnerNetworkId
			})
			{
				this.regularProperty = regularProperty;
				this.protectedProperty = protectedProperty;
				this.type = type;
			}

			protected override bool TryGetRegularValue(IStorePropertyBag source, out PhoneNumber value)
			{
				return this.TryGetPropertyValue(source, this.regularProperty, out value);
			}

			protected override bool TryGetProtectedValue(IStorePropertyBag source, out PhoneNumber value)
			{
				return this.TryGetPropertyValue(source, this.protectedProperty, out value);
			}

			private bool TryGetPropertyValue(IStorePropertyBag source, StorePropertyDefinition property, out PhoneNumber value)
			{
				string text = source.TryGetProperty(property) as string;
				if (!string.IsNullOrWhiteSpace(text))
				{
					value = new PhoneNumber
					{
						Number = text,
						Type = this.type
					};
					return true;
				}
				value = null;
				return false;
			}

			private readonly StorePropertyDefinition regularProperty;

			private readonly StorePropertyDefinition protectedProperty;

			private readonly PersonPhoneNumberType type;
		}

		private sealed class AttributedThirdPartyPhotoUrlPropertyAggregation : PersonPropertyAggregationStrategy.AttributedStringPropertyAggregation
		{
			public AttributedThirdPartyPhotoUrlPropertyAggregation() : base(ContactSchema.PartnerNetworkThumbnailPhotoUrl, PersonPropertyAggregationStrategy.AttributedThirdPartyPhotoUrlPropertyAggregation.DependantProperties)
			{
			}

			protected override IList<IStorePropertyBag> SortSources(IList<IStorePropertyBag> sources)
			{
				if (this.IsConsumerUser())
				{
					return (from source in sources
					orderby this.GetConsumerRank(source.GetValueOrDefault<string>(ContactSchema.PartnerNetworkThumbnailPhotoUrl, string.Empty), source.GetValueOrDefault<string>(ContactSchema.PartnerNetworkId, string.Empty))
					select source).ToList<IStorePropertyBag>();
				}
				return (from b in sources
				orderby b.GetValueOrDefault<ExDateTime>(ContactSchema.PeopleConnectionCreationTime, ExDateTime.MaxValue)
				select b).ToList<IStorePropertyBag>();
			}

			private int GetConsumerRank(string thirdPartyUrl, string partnerNetworkId)
			{
				int result = int.MaxValue;
				if (!string.IsNullOrWhiteSpace(thirdPartyUrl))
				{
					if (StringComparer.OrdinalIgnoreCase.Equals(partnerNetworkId, WellKnownNetworkNames.GAL))
					{
						result = 0;
					}
					else if (StringComparer.OrdinalIgnoreCase.Equals(partnerNetworkId, WellKnownNetworkNames.Facebook))
					{
						result = 1;
					}
					else if (StringComparer.OrdinalIgnoreCase.Equals(partnerNetworkId, WellKnownNetworkNames.LinkedIn))
					{
						result = 2;
					}
					else
					{
						result = 3;
					}
				}
				return result;
			}

			private bool IsConsumerUser()
			{
				return false;
			}

			private static readonly StorePropertyDefinition[] DependantProperties = new StorePropertyDefinition[]
			{
				ContactSchema.PeopleConnectionCreationTime,
				ContactSchema.PartnerNetworkId
			};
		}
	}
}
