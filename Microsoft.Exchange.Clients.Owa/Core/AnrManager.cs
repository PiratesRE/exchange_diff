using System;
using System.Collections.Generic;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.Owa.Core.Directory;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class AnrManager
	{
		private AnrManager()
		{
		}

		public static RecipientAddress ResolveAnrStringToOneOffEmail(string name, AnrManager.Options options)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			RecipientAddress recipientAddress = null;
			Participant participant;
			if (AnrManager.TryParseParticipant(name, options, out participant) && (!options.OnlyAllowDefaultRoutingType || options.IsDefaultRoutingType(participant.RoutingType)))
			{
				recipientAddress = new RecipientAddress();
				recipientAddress.DisplayName = participant.DisplayName;
				recipientAddress.AddressOrigin = RecipientAddress.ToAddressOrigin(participant);
				recipientAddress.RoutingAddress = (AnrManager.IsMobileNumberInput(participant, options) ? participant.DisplayName : participant.EmailAddress);
				recipientAddress.RoutingType = participant.RoutingType;
				recipientAddress.SmtpAddress = ((participant.RoutingType == "SMTP") ? participant.EmailAddress : null);
				recipientAddress.MobilePhoneNumber = ((participant.RoutingType == "MOBILE") ? participant.EmailAddress : null);
				StoreParticipantOrigin storeParticipantOrigin = participant.Origin as StoreParticipantOrigin;
				if (storeParticipantOrigin != null)
				{
					recipientAddress.StoreObjectId = storeParticipantOrigin.OriginItemId;
					recipientAddress.EmailAddressIndex = storeParticipantOrigin.EmailAddressIndex;
				}
			}
			return recipientAddress;
		}

		public static RecipientAddress ResolveAnrStringToOneOffEmail(string name)
		{
			return AnrManager.ResolveAnrStringToOneOffEmail(name, new AnrManager.Options());
		}

		public static void ResolveOneRecipient(string name, UserContext userContext, List<RecipientAddress> addresses)
		{
			AnrManager.ResolveOneRecipient(name, userContext, addresses, new AnrManager.Options());
		}

		public static void ResolveOneRecipient(string name, UserContext userContext, List<RecipientAddress> addresses, AnrManager.Options options)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (addresses == null)
			{
				throw new ArgumentNullException("addresses");
			}
			AnrManager.NameParsingResult parsingResult = AnrManager.ParseNameBeforeAnr(name, options);
			if (!options.ResolveOnlyFromAddressBook && userContext.IsFeatureEnabled(Feature.Contacts))
			{
				AnrManager.GetNamesByAnrFromContacts(userContext, parsingResult, options, addresses);
			}
			AnrManager.GetNamesByAnrFromAD(userContext, parsingResult, options, addresses);
			if (AnrManager.IsMobileNumberInput(parsingResult, options) && addresses.Count > 0)
			{
				bool flag = false;
				foreach (RecipientAddress address in addresses)
				{
					if (AnrManager.IsMobileAddressExactMatch(parsingResult, address))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					RecipientAddress recipientAddress = AnrManager.ResolveAnrStringToOneOffEmail(name, options);
					if (recipientAddress != null)
					{
						addresses.Add(recipientAddress);
					}
				}
			}
			addresses.Sort();
		}

		public static RecipientAddress ResolveAnrString(string name, bool resolveContactsFirst, UserContext userContext)
		{
			return AnrManager.ResolveAnrString(name, new AnrManager.Options
			{
				ResolveContactsFirst = resolveContactsFirst
			}, userContext);
		}

		public static RecipientAddress ResolveAnrString(string name, AnrManager.Options options, UserContext userContext)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			if (Globals.ArePerfCountersEnabled)
			{
				OwaSingleCounters.NamesChecked.Increment();
			}
			AnrManager.NameParsingResult parsingResult = AnrManager.ParseNameBeforeAnr(name, options);
			if (options.ResolveContactsFirst)
			{
				return AnrManager.ResolveAnrStringFromContacts(userContext, parsingResult, options, () => AnrManager.ResolveAnrStringFromAD(userContext, parsingResult, options, () => AnrManager.ResolveAnrStringToOneOffEmail(name, options)));
			}
			return AnrManager.ResolveAnrStringFromAD(userContext, parsingResult, options, () => AnrManager.ResolveAnrStringFromContacts(userContext, parsingResult, options, () => AnrManager.ResolveAnrStringToOneOffEmail(name, options)));
		}

		private static RecipientAddress ResolveAnrStringFromContacts(UserContext userContext, AnrManager.NameParsingResult parsingResult, AnrManager.Options options, AnrManager.NextOperation nextOperation)
		{
			List<RecipientAddress> list = new List<RecipientAddress>();
			if (!options.ResolveOnlyFromAddressBook && userContext.IsFeatureEnabled(Feature.Contacts))
			{
				AnrManager.GetNamesByAnrFromContacts(userContext, parsingResult, options, list);
			}
			if (list.Count == 1)
			{
				if (!AnrManager.IsMobileNumberInput(parsingResult, options) || AnrManager.IsMobileAddressExactMatch(parsingResult, list[0]))
				{
					return list[0];
				}
				return null;
			}
			else
			{
				if (list.Count > 1)
				{
					return null;
				}
				if (nextOperation != null)
				{
					return nextOperation();
				}
				return null;
			}
		}

		private static RecipientAddress ResolveAnrStringFromAD(UserContext userContext, AnrManager.NameParsingResult parsingResult, AnrManager.Options options, AnrManager.NextOperation nextOperation)
		{
			List<RecipientAddress> list = new List<RecipientAddress>();
			AnrManager.GetNamesByAnrFromAD(userContext, parsingResult, options, list);
			if (list.Count == 1)
			{
				if (!AnrManager.IsMobileNumberInput(parsingResult, options) || AnrManager.IsMobileAddressExactMatch(parsingResult, list[0]))
				{
					return list[0];
				}
				return null;
			}
			else
			{
				if (list.Count > 1)
				{
					return null;
				}
				if (nextOperation != null)
				{
					return nextOperation();
				}
				return null;
			}
		}

		private static bool AreNameAndAddressTheSameNumber(AnrManager.Options options, string routingType, string displayName, string routingAddress)
		{
			if (!options.IsDefaultRoutingType("MOBILE"))
			{
				return false;
			}
			if (!Utilities.IsMobileRoutingType(routingType))
			{
				return false;
			}
			E164Number objA = null;
			E164Number objB = null;
			return E164Number.TryParse(displayName, out objA) && E164Number.TryParse(routingAddress, out objB) && object.Equals(objA, objB);
		}

		private static bool IsMobileNumberInput(AnrManager.NameParsingResult parsingResult, AnrManager.Options options)
		{
			return parsingResult.ParsedSuccessfully && AnrManager.AreNameAndAddressTheSameNumber(options, parsingResult.RoutingType, parsingResult.Name, parsingResult.RoutingAddress);
		}

		private static bool IsMobileNumberInput(Participant parsedParticipant, AnrManager.Options options)
		{
			return parsedParticipant.ValidationStatus == ParticipantValidationStatus.NoError && AnrManager.AreNameAndAddressTheSameNumber(options, parsedParticipant.RoutingType, parsedParticipant.DisplayName, parsedParticipant.EmailAddress);
		}

		private static bool IsMobileAddressExactMatch(AnrManager.NameParsingResult parsingResult, RecipientAddress address)
		{
			if (!parsingResult.ParsedSuccessfully)
			{
				return false;
			}
			if (!Utilities.IsMobileRoutingType(parsingResult.RoutingType))
			{
				return false;
			}
			if (!Utilities.IsMobileRoutingType(address.RoutingType))
			{
				return false;
			}
			E164Number objA = null;
			E164Number objB = null;
			return E164Number.TryParse(parsingResult.RoutingAddress, out objA) && E164Number.TryParse(address.RoutingAddress, out objB) && object.Equals(objA, objB);
		}

		private static object FindFromResultsMapping(PropertyDefinition property, PropertyDefinition[] properties, object[] results)
		{
			if (properties.Length != results.Length)
			{
				throw new InvalidOperationException("The lengths should be the same");
			}
			for (int i = 0; i < properties.Length; i++)
			{
				if (properties[i] == property)
				{
					return results[i];
				}
			}
			return null;
		}

		private static void GetNamesByAnrFromContacts(UserContext userContext, AnrManager.NameParsingResult parsingResult, AnrManager.Options options, List<RecipientAddress> addresses)
		{
			if (string.IsNullOrEmpty(parsingResult.Name))
			{
				return;
			}
			if (userContext.TryGetMyDefaultFolderId(DefaultFolderType.Contacts) == null)
			{
				return;
			}
			string ambiguousName = parsingResult.ParsedSuccessfully ? parsingResult.RoutingAddress : parsingResult.Name;
			using (ContactsFolder contactsFolder = ContactsFolder.Bind(userContext.MailboxSession, DefaultFolderType.Contacts))
			{
				if (contactsFolder.IsValidAmbiguousName(ambiguousName))
				{
					PropertyDefinition[] array;
					object[][] results;
					if (AnrManager.IsMobileNumberInput(parsingResult, options))
					{
						array = AnrManager.AnrProperties.Get(AnrManager.AnrProperties.PropertiesType.ContactFindSomeone, options);
						results = contactsFolder.FindNamesView(new Dictionary<PropertyDefinition, object>
						{
							{
								ContactSchema.MobilePhone,
								parsingResult.Name
							}
						}, AnrManager.nameLimit, null, array);
					}
					else if (options.ResolveAgainstAllContacts || options.IsDefaultRoutingType("MOBILE"))
					{
						array = AnrManager.AnrProperties.Get(AnrManager.AnrProperties.PropertiesType.ContactFindSomeone, options);
						results = contactsFolder.FindSomeoneView(ambiguousName, AnrManager.nameLimit, null, array);
					}
					else
					{
						array = AnrManager.AnrProperties.Get(AnrManager.AnrProperties.PropertiesType.ContactAnr, options);
						results = contactsFolder.ResolveAmbiguousNameView(ambiguousName, AnrManager.nameLimit, null, array);
					}
					AnrManager.AddContacts(userContext, options, array, results, addresses);
				}
			}
		}

		private static void AddContacts(UserContext userContext, AnrManager.Options options, PropertyDefinition[] properties, object[][] results, List<RecipientAddress> addresses)
		{
			if (results != null && results.GetLength(0) > 0)
			{
				int i = 0;
				while (i < results.GetLength(0))
				{
					object[] results2 = results[i];
					Participant participant = null;
					string displayName = null;
					string text = Utilities.NormalizePhoneNumber(AnrManager.FindFromResultsMapping(ContactSchema.MobilePhone, properties, results2) as string);
					VersionedId versionedId = AnrManager.FindFromResultsMapping(ItemSchema.Id, properties, results2) as VersionedId;
					if (!options.ResolveAgainstAllContacts && !options.IsDefaultRoutingType("MOBILE"))
					{
						participant = (AnrManager.FindFromResultsMapping(ContactBaseSchema.AnrViewParticipant, properties, results2) as Participant);
						displayName = participant.DisplayName;
						goto IL_1AB;
					}
					Participant participant2 = AnrManager.FindFromResultsMapping(DistributionListSchema.AsParticipant, properties, results2) as Participant;
					if (participant2 != null)
					{
						participant = participant2;
						displayName = participant.DisplayName;
						goto IL_1AB;
					}
					if (options.IsDefaultRoutingType("MOBILE"))
					{
						if (!string.IsNullOrEmpty(text))
						{
							displayName = (AnrManager.FindFromResultsMapping(StoreObjectSchema.DisplayName, properties, results2) as string);
							participant = new Participant(displayName, text, "MOBILE", new StoreParticipantOrigin(versionedId), new KeyValuePair<PropertyDefinition, object>[0]);
						}
						else if (options.OnlyAllowDefaultRoutingType)
						{
							goto IL_339;
						}
					}
					if (!(participant == null))
					{
						goto IL_1AB;
					}
					Participant participant3 = AnrManager.FindFromResultsMapping(ContactSchema.Email1, properties, results2) as Participant;
					Participant participant4 = AnrManager.FindFromResultsMapping(ContactSchema.Email2, properties, results2) as Participant;
					Participant participant5 = AnrManager.FindFromResultsMapping(ContactSchema.Email3, properties, results2) as Participant;
					if (participant3 != null && !string.IsNullOrEmpty(participant3.EmailAddress))
					{
						participant = participant3;
						displayName = participant.DisplayName;
						goto IL_1AB;
					}
					if (participant4 != null && !string.IsNullOrEmpty(participant4.EmailAddress))
					{
						participant = participant4;
						displayName = participant.DisplayName;
						goto IL_1AB;
					}
					if (participant5 != null && !string.IsNullOrEmpty(participant5.EmailAddress))
					{
						participant = participant5;
						displayName = participant.DisplayName;
						goto IL_1AB;
					}
					goto IL_1AB;
					IL_339:
					i++;
					continue;
					IL_1AB:
					RecipientAddress recipientAddress = new RecipientAddress();
					recipientAddress.MobilePhoneNumber = text;
					recipientAddress.DisplayName = displayName;
					recipientAddress.AddressOrigin = AddressOrigin.Store;
					if (participant != null)
					{
						if (Utilities.IsMapiPDL(participant.RoutingType) && Utilities.IsFlagSet((int)options.RecipientBlockType, 2))
						{
							goto IL_339;
						}
						recipientAddress.RoutingType = participant.RoutingType;
						recipientAddress.EmailAddressIndex = ((StoreParticipantOrigin)participant.Origin).EmailAddressIndex;
						if (!string.IsNullOrEmpty(participant.EmailAddress))
						{
							recipientAddress.RoutingAddress = participant.EmailAddress;
							if (string.CompareOrdinal(recipientAddress.RoutingType, "EX") == 0)
							{
								string text2 = participant.TryGetProperty(ParticipantSchema.SmtpAddress) as string;
								if (string.IsNullOrEmpty(text2))
								{
									IRecipientSession recipientSession = Utilities.CreateADRecipientSession(Culture.GetUserCulture().LCID, true, ConsistencyMode.IgnoreInvalid, true, userContext);
									ADRecipient adrecipient = null;
									try
									{
										adrecipient = recipientSession.FindByLegacyExchangeDN(recipientAddress.RoutingAddress);
									}
									catch (NonUniqueRecipientException ex)
									{
										ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "AnrManager.GetNamesByAnrFromContacts: NonUniqueRecipientException was thrown by FindByLegacyExchangeDN: {0}", ex.Message);
									}
									if (adrecipient == null || adrecipient.HiddenFromAddressListsEnabled)
									{
										goto IL_339;
									}
									recipientAddress.SmtpAddress = adrecipient.PrimarySmtpAddress.ToString();
								}
								else
								{
									recipientAddress.SmtpAddress = text2;
								}
							}
							else if (string.CompareOrdinal(recipientAddress.RoutingType, "SMTP") == 0)
							{
								recipientAddress.SmtpAddress = recipientAddress.RoutingAddress;
							}
						}
					}
					if (Utilities.IsMapiPDL(recipientAddress.RoutingType))
					{
						recipientAddress.IsDistributionList = true;
					}
					if (versionedId != null)
					{
						recipientAddress.StoreObjectId = versionedId.ObjectId;
					}
					addresses.Add(recipientAddress);
					goto IL_339;
				}
			}
		}

		private static void GetNamesByAnrFromAD(UserContext userContext, AnrManager.NameParsingResult parsingResult, AnrManager.Options options, List<RecipientAddress> addresses)
		{
			IRecipientSession recipientSession = Utilities.CreateADRecipientSession(Culture.GetUserCulture().LCID, true, ConsistencyMode.IgnoreInvalid, true, userContext);
			ADRawEntry[] array = null;
			bool flag = parsingResult.ParsedSuccessfully && !Utilities.IsMobileRoutingType(parsingResult.RoutingType);
			string text = flag ? string.Format("{0}:{1}", parsingResult.RoutingType, parsingResult.RoutingAddress) : parsingResult.Name;
			if (flag)
			{
				ADRawEntry adrawEntry = recipientSession.FindByProxyAddress(ProxyAddress.Parse(text), AnrManager.AnrProperties.Get(AnrManager.AnrProperties.PropertiesType.AD, options));
				if (adrawEntry != null)
				{
					if ((bool)adrawEntry[ADRecipientSchema.HiddenFromAddressListsEnabled])
					{
						array = new ADRawEntry[0];
						ExTraceGlobals.CoreTracer.TraceDebug<ADObjectId>(0L, "AnrManager.GetNamesByAnrFromAD: Recipient ignored because it is hiddem from address lists: {0}", adrawEntry.Id);
					}
					else
					{
						array = new ADRawEntry[]
						{
							adrawEntry
						};
					}
				}
			}
			if (array == null)
			{
				QueryFilter filter = new AndFilter(new QueryFilter[]
				{
					new AmbiguousNameResolutionFilter(text),
					AnrManager.addressListMembershipExists
				});
				array = recipientSession.Find(null, QueryScope.SubTree, filter, null, AnrManager.nameLimit, AnrManager.AnrProperties.Get(AnrManager.AnrProperties.PropertiesType.AD, options));
			}
			AnrManager.AddADRecipients(array, options, addresses);
		}

		private static void AddADRecipients(ADRawEntry[] adRecipients, AnrManager.Options options, List<RecipientAddress> addresses)
		{
			if (adRecipients != null)
			{
				foreach (ADRawEntry adrawEntry in adRecipients)
				{
					RecipientType recipientType = (RecipientType)adrawEntry[ADRecipientSchema.RecipientType];
					if (recipientType == RecipientType.UserMailbox || recipientType == RecipientType.MailUniversalDistributionGroup || recipientType == RecipientType.MailUniversalSecurityGroup || recipientType == RecipientType.MailNonUniversalGroup || recipientType == RecipientType.MailUser || recipientType == RecipientType.MailContact || recipientType == RecipientType.DynamicDistributionGroup || recipientType == RecipientType.PublicFolder)
					{
						bool flag = Utilities.IsADDistributionList((MultiValuedProperty<string>)adrawEntry[ADObjectSchema.ObjectClass]);
						string text = Utilities.NormalizePhoneNumber((string)adrawEntry[ADOrgPersonSchema.MobilePhone]);
						if (!flag || !Utilities.IsFlagSet((int)options.RecipientBlockType, 1))
						{
							bool isRoom = DirectoryAssistance.IsADRecipientRoom((RecipientDisplayType?)adrawEntry[ADRecipientSchema.RecipientDisplayType]);
							RecipientAddress recipientAddress = null;
							if (!flag && options.IsDefaultRoutingType("MOBILE"))
							{
								if (!string.IsNullOrEmpty(text))
								{
									recipientAddress = new RecipientAddress();
									recipientAddress.RoutingType = "MOBILE";
									recipientAddress.RoutingAddress = text;
								}
								else if (options.OnlyAllowDefaultRoutingType)
								{
									goto IL_1BD;
								}
							}
							if (recipientAddress == null)
							{
								recipientAddress = new RecipientAddress();
								recipientAddress.Alias = (string)adrawEntry[ADRecipientSchema.Alias];
								recipientAddress.RoutingAddress = (string)adrawEntry[ADRecipientSchema.LegacyExchangeDN];
								recipientAddress.RoutingType = "EX";
								recipientAddress.SmtpAddress = adrawEntry[ADRecipientSchema.PrimarySmtpAddress].ToString();
							}
							recipientAddress.AddressOrigin = AddressOrigin.Directory;
							recipientAddress.ADObjectId = (ADObjectId)adrawEntry[ADObjectSchema.Id];
							recipientAddress.IsRoom = isRoom;
							recipientAddress.DisplayName = (string)adrawEntry[ADRecipientSchema.DisplayName];
							recipientAddress.IsDistributionList = flag;
							recipientAddress.RecipientType = recipientType;
							recipientAddress.SipUri = InstantMessageUtilities.GetSipUri((ProxyAddressCollection)adrawEntry[ADRecipientSchema.EmailAddresses]);
							recipientAddress.MobilePhoneNumber = text;
							addresses.Add(recipientAddress);
						}
					}
					IL_1BD:;
				}
			}
		}

		private static AnrManager.NameParsingResult ParseNameBeforeAnr(string name, AnrManager.Options options)
		{
			AnrManager.NameParsingResult result = new AnrManager.NameParsingResult(name);
			Participant participant;
			if (AnrManager.TryParseParticipant(name, options, out participant))
			{
				result.ParsedSuccessfully = true;
				result.RoutingType = participant.RoutingType;
				result.RoutingAddress = participant.EmailAddress;
			}
			return result;
		}

		private static bool TryParseParticipant(string input, AnrManager.Options options, out Participant participant)
		{
			if (string.IsNullOrEmpty(input))
			{
				participant = null;
				return false;
			}
			return Participant.TryParse(input, out participant) && participant.ValidationStatus == ParticipantValidationStatus.NoError && participant.RoutingType != null;
		}

		private const string ADAnrLookupFormat = "{0}:{1}";

		private static readonly int nameLimit = 100;

		private static readonly AndFilter addressListMembershipExists = new AndFilter(new QueryFilter[]
		{
			new ExistsFilter(ADRecipientSchema.AddressListMembership),
			new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.HiddenFromAddressListsEnabled, false)
		});

		public sealed class Options
		{
			public bool ResolveContactsFirst { get; set; }

			public bool ResolveOnlyFromAddressBook { get; set; }

			public bool ResolveAgainstAllContacts { get; set; }

			public bool OnlyAllowDefaultRoutingType { get; set; }

			public RecipientBlockType RecipientBlockType { get; set; }

			public string DefaultRoutingType { get; set; }

			public bool IsDefaultRoutingType(string routingType)
			{
				return !string.IsNullOrEmpty(this.DefaultRoutingType) && string.Equals(routingType, this.DefaultRoutingType, StringComparison.OrdinalIgnoreCase);
			}
		}

		private static class AnrProperties
		{
			public static PropertyDefinition[] Get(AnrManager.AnrProperties.PropertiesType type, AnrManager.Options options)
			{
				switch (type)
				{
				case AnrManager.AnrProperties.PropertiesType.AD:
					return AnrManager.AnrProperties.adWithMobilePhoneNumber;
				case AnrManager.AnrProperties.PropertiesType.ContactAnr:
					if (options.IsDefaultRoutingType("MOBILE"))
					{
						throw new InvalidOperationException("Should not call contact ANR for mobile numbers");
					}
					return AnrManager.AnrProperties.contactAnr;
				case AnrManager.AnrProperties.PropertiesType.ContactFindSomeone:
					return AnrManager.AnrProperties.contactEmailAndMobilePhoneNumber;
				default:
					throw new ArgumentOutOfRangeException("type");
				}
			}

			private static PropertyDefinition[] contactAnr = new PropertyDefinition[]
			{
				ParticipantSchema.DisplayName,
				ContactBaseSchema.AnrViewParticipant,
				ItemSchema.Id
			};

			private static PropertyDefinition[] contactEmailAndMobilePhoneNumber = new PropertyDefinition[]
			{
				StoreObjectSchema.DisplayName,
				DistributionListSchema.AsParticipant,
				ItemSchema.Id,
				ContactSchema.Email1,
				ContactSchema.Email2,
				ContactSchema.Email3,
				ContactSchema.MobilePhone
			};

			private static PropertyDefinition[] adWithMobilePhoneNumber = new PropertyDefinition[]
			{
				ADRecipientSchema.RecipientType,
				ADRecipientSchema.Alias,
				ADRecipientSchema.DisplayName,
				ADRecipientSchema.LegacyExchangeDN,
				ADRecipientSchema.PrimarySmtpAddress,
				ADRecipientSchema.RecipientDisplayType,
				ADRecipientSchema.HiddenFromAddressListsEnabled,
				ADOrgPersonSchema.MobilePhone,
				ADObjectSchema.Id
			};

			public enum PropertiesType
			{
				None,
				AD,
				ContactAnr,
				ContactFindSomeone
			}
		}

		private delegate RecipientAddress NextOperation();

		private struct NameParsingResult
		{
			public NameParsingResult(string name)
			{
				this.Name = name;
				this.ParsedSuccessfully = false;
				this.RoutingType = (this.RoutingAddress = string.Empty);
			}

			public bool ParsedSuccessfully;

			public string Name;

			public string RoutingType;

			public string RoutingAddress;
		}
	}
}
