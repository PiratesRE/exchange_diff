using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Channels;
using System.Web;
using System.Xml;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class FindPeopleSpeechRecognitionResultHandler : IMobileSpeechRecognitionResultHandler
	{
		public FindPeopleSpeechRecognitionResultHandler(RequestParameters parameters, UserContext userContext, HttpContext httpContext)
		{
			IADRecipientLookup iadrecipientLookup = ADRecipientLookupFactory.CreateFromTenantGuid(parameters.TenantGuid);
			this.userRecipient = iadrecipientLookup.LookupByObjectId(new ADObjectId(parameters.UserObjectGuid));
			this.userContext = userContext;
			this.httpContext = httpContext;
		}

		public virtual void ProcessAndFormatSpeechRecognitionResults(string result, out string jsonResponse, out SpeechRecognitionProcessor.SpeechHttpStatus httpStatus)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<string>((long)this.GetHashCode(), "Entering FindPeopleSearchRecognitionResultHandler.ProcessAndFormatSpeechRecognitionResults with results '{0}'", result);
			jsonResponse = null;
			httpStatus = SpeechRecognitionProcessor.SpeechHttpStatus.Success;
			List<Persona> uniquePersonaList = this.GetUniquePersonaList(result);
			jsonResponse = SpeechRecognitionResultHandler.JsonSerialize(uniquePersonaList.ToArray());
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<string>((long)this.GetHashCode(), "Persona array json:{0}", jsonResponse);
		}

		protected List<Persona> GetUniquePersonaList(string result)
		{
			List<Persona> list = new List<Persona>();
			List<string> galLinksToRemove;
			list.AddRange(this.GetPersonalContactPersonas(result, out galLinksToRemove));
			list.AddRange(this.GetGALPersonas(result, galLinksToRemove));
			list.Sort(new FindPeopleSpeechRecognitionResultHandler.PersonaComparer());
			return list;
		}

		private static Persona GetPersonaFromUser(ADRecipient recipient)
		{
			Persona persona = new Persona();
			persona.PersonaId = new ItemId(IdConverter.ADObjectIdToEwsId(recipient.Id), null);
			persona.DisplayName = recipient.DisplayName;
			EmailAddressWrapper emailAddressWrapper = new EmailAddressWrapper();
			emailAddressWrapper.EmailAddress = recipient.PrimarySmtpAddress.ToString();
			emailAddressWrapper.Name = recipient.PrimarySmtpAddress.ToString();
			persona.EmailAddress = emailAddressWrapper;
			persona.EmailAddresses = new EmailAddressWrapper[]
			{
				emailAddressWrapper
			};
			persona.Attributions = new Attribution[]
			{
				new Attribution("0", new ItemId(), WellKnownNetworkNames.GAL)
				{
					IsWritable = false,
					IsQuickContact = false,
					IsHidden = false,
					FolderId = null
				}
			};
			IADOrgPerson iadorgPerson = recipient as IADOrgPerson;
			if (iadorgPerson != null)
			{
				persona.GivenName = iadorgPerson.FirstName;
				persona.CompanyName = iadorgPerson.Company;
				persona.Surname = iadorgPerson.LastName;
				persona.Title = iadorgPerson.Title;
				Microsoft.Exchange.Services.Core.Types.PhoneNumber phoneNumber = new Microsoft.Exchange.Services.Core.Types.PhoneNumber();
				phoneNumber.Number = iadorgPerson.Phone;
				phoneNumber.Type = PersonPhoneNumberType.Business;
				persona.PhoneNumber = phoneNumber;
				persona.DisplayNames = FindPeopleSpeechRecognitionResultHandler.GetStringAttribValue(persona.DisplayName);
				persona.GivenNames = FindPeopleSpeechRecognitionResultHandler.GetStringAttribValue(persona.GivenName);
				persona.Surnames = FindPeopleSpeechRecognitionResultHandler.GetStringAttribValue(persona.Surname);
				persona.CompanyNames = FindPeopleSpeechRecognitionResultHandler.GetStringAttribValue(persona.CompanyName);
				persona.Titles = FindPeopleSpeechRecognitionResultHandler.GetStringAttribValue(persona.Title);
				persona.OfficeLocations = FindPeopleSpeechRecognitionResultHandler.GetStringAttribValue(iadorgPerson.Office);
				persona.Emails1 = FindPeopleSpeechRecognitionResultHandler.GetEmailAddressAttribValue(emailAddressWrapper);
				persona.BusinessPhoneNumbers = FindPeopleSpeechRecognitionResultHandler.GetPhoneNumberAttribValue(phoneNumber);
			}
			return persona;
		}

		private static StringAttributedValue[] GetStringAttribValue(string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				return new StringAttributedValue[]
				{
					new StringAttributedValue(value, new string[]
					{
						"0"
					})
				};
			}
			return null;
		}

		private static EmailAddressAttributedValue[] GetEmailAddressAttribValue(EmailAddressWrapper value)
		{
			if (value != null)
			{
				return new EmailAddressAttributedValue[]
				{
					new EmailAddressAttributedValue(value, new string[]
					{
						"0"
					})
				};
			}
			return null;
		}

		private static PhoneNumberAttributedValue[] GetPhoneNumberAttribValue(Microsoft.Exchange.Services.Core.Types.PhoneNumber value)
		{
			if (value != null)
			{
				return new PhoneNumberAttributedValue[]
				{
					new PhoneNumberAttributedValue(value, new string[]
					{
						"0"
					})
				};
			}
			return null;
		}

		private static Dictionary<string, PeopleSpeechPersonObject> RetrieveUniquePersonalContactsFromXML(string result, List<string> galLinksToRemove)
		{
			return FindPeopleSpeechRecognitionResultHandler.RetrieveContactFromXMLHelper(result, "PersonalContactSearch", "PersonId", "GALLinkID", delegate(Dictionary<string, PeopleSpeechPersonObject> personalContactPersons, PeopleSpeechPersonObject personObject)
			{
				FindPeopleSpeechRecognitionResultHandler.AddContactToUniqueDictionary(personObject.Identifier, personObject, personalContactPersons);
				galLinksToRemove.Add(personObject.GALLinkId);
			});
		}

		private static Dictionary<string, PeopleSpeechPersonObject> RetrieveGALContactsFromXML(string result)
		{
			return FindPeopleSpeechRecognitionResultHandler.RetrieveContactFromXMLHelper(result, "GALSearch", "SMTP", "ObjectGuid", delegate(Dictionary<string, PeopleSpeechPersonObject> galContactPersons, PeopleSpeechPersonObject personObject)
			{
				FindPeopleSpeechRecognitionResultHandler.AddContactToUniqueDictionary(personObject.GALLinkId, personObject, galContactPersons);
			});
		}

		private static Dictionary<string, PeopleSpeechPersonObject> RetrieveContactFromXMLHelper(string result, string contactSearchElementName, string identiferElementName, string galLinkIdElementName, FindPeopleSpeechRecognitionResultHandler.AddContactToDictionary addToDictionary)
		{
			Dictionary<string, PeopleSpeechPersonObject> dictionary = new Dictionary<string, PeopleSpeechPersonObject>();
			using (XmlReader xmlReader = XmlReader.Create(new StringReader(result)))
			{
				if (xmlReader.ReadToFollowing(contactSearchElementName))
				{
					using (XmlReader xmlReader2 = xmlReader.ReadSubtree())
					{
						while (xmlReader2.ReadToFollowing("Alternate"))
						{
							PeopleSpeechPersonObject peopleSpeechPersonObject = new PeopleSpeechPersonObject();
							if (xmlReader2.MoveToAttribute("confidence"))
							{
								peopleSpeechPersonObject.Confidence = xmlReader2.ReadContentAsFloat();
							}
							if (xmlReader2.ReadToFollowing(identiferElementName))
							{
								peopleSpeechPersonObject.Identifier = xmlReader2.ReadString();
							}
							if (xmlReader2.ReadToFollowing(galLinkIdElementName))
							{
								peopleSpeechPersonObject.GALLinkId = xmlReader2.ReadString();
							}
							addToDictionary(dictionary, peopleSpeechPersonObject);
						}
					}
				}
			}
			return dictionary;
		}

		private static void AddContactToUniqueDictionary(string key, PeopleSpeechPersonObject personObject, Dictionary<string, PeopleSpeechPersonObject> contactPersons)
		{
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<string, float, string>(0L, "Entering PeopleSearchRecognitionResultHandler.AddPersonalContactToUniqueDictionary with Person:'{0}', confidence:'{1}', LinkId: '{2}'", personObject.Identifier, personObject.Confidence, personObject.GALLinkId);
			PeopleSpeechPersonObject peopleSpeechPersonObject;
			if (!contactPersons.TryGetValue(key, out peopleSpeechPersonObject))
			{
				ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<string>(0L, "Person:'{0}' not in unique dictionary, adding to dictionary", personObject.Identifier);
				contactPersons.Add(key, personObject);
				return;
			}
			if (personObject.Confidence > peopleSpeechPersonObject.Confidence)
			{
				ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<float, float>(0L, "uniquePersonObject confidence '{0}' is lower than personObject confidence '{0}', updating the value of the uniquePersonObject", peopleSpeechPersonObject.Confidence, personObject.Confidence);
				peopleSpeechPersonObject.Confidence = personObject.Confidence;
			}
			contactPersons[key] = peopleSpeechPersonObject;
		}

		private static void MakeGalContactsUnique(Dictionary<string, PeopleSpeechPersonObject> galContactPersons, List<string> galLinksToRemove)
		{
			foreach (string text in galLinksToRemove)
			{
				if (galContactPersons.ContainsKey(text))
				{
					ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<string>(0L, "Removing the galLink with userObjectId:{0} since its already present in personal contacts", text);
					galContactPersons.Remove(text);
				}
			}
		}

		private static CallContext CreateAndSetCallContext(HttpContext httpContext)
		{
			HttpContext.Current = httpContext;
			CallContext result;
			using (Message message = Message.CreateMessage(MessageVersion.Default, string.Empty))
			{
				message.Properties.Add("WebMethodEntry", WebMethodEntry.JsonWebMethodEntry);
				result = CallContextUtilities.CreateAndSetCallContext(message, WorkloadType.OwaVoice, false, "");
			}
			return result;
		}

		private static void DisposeContext(CallContext callContext)
		{
			CallContext.SetCurrent(null);
			if (callContext != null)
			{
				callContext.Dispose();
				callContext = null;
			}
		}

		private List<Persona> GetGALPersonas(string result, List<string> galLinksToRemove)
		{
			Dictionary<string, PeopleSpeechPersonObject> dictionary = FindPeopleSpeechRecognitionResultHandler.RetrieveGALContactsFromXML(result);
			FindPeopleSpeechRecognitionResultHandler.MakeGalContactsUnique(dictionary, galLinksToRemove);
			List<Persona> list = new List<Persona>(dictionary.Count);
			IADRecipientLookup iadrecipientLookup = ADRecipientLookupFactory.CreateFromADRecipient(this.userRecipient, true);
			List<string> list2 = new List<string>(dictionary.Count);
			Dictionary<string, PeopleSpeechPersonObject> dictionary2 = new Dictionary<string, PeopleSpeechPersonObject>(dictionary.Count);
			foreach (KeyValuePair<string, PeopleSpeechPersonObject> keyValuePair in dictionary)
			{
				list2.Add(keyValuePair.Value.Identifier);
				dictionary2.Add(keyValuePair.Value.Identifier, keyValuePair.Value);
			}
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<int>((long)this.GetHashCode(), "Looking up ADRecipient by smtpAddresses count:{0}", list2.Count);
			ADRecipient[] array = iadrecipientLookup.LookupBySmtpAddresses(list2);
			ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<int>((long)this.GetHashCode(), "Finished looking up ADRecipient by smtpAddresses count:{0}", list2.Count);
			foreach (ADRecipient adrecipient in array)
			{
				Persona personaFromUser = FindPeopleSpeechRecognitionResultHandler.GetPersonaFromUser(adrecipient);
				personaFromUser.SpeechConfidence = dictionary2[adrecipient.PrimarySmtpAddress.ToString()].Confidence;
				list.Add(personaFromUser);
			}
			return list;
		}

		private List<Persona> GetPersonalContactPersonas(string result, out List<string> galLinksToRemove)
		{
			galLinksToRemove = new List<string>();
			Dictionary<string, PeopleSpeechPersonObject> personalContactPersons = FindPeopleSpeechRecognitionResultHandler.RetrieveUniquePersonalContactsFromXML(result, galLinksToRemove);
			List<Persona> personas = new List<Persona>(personalContactPersons.Count);
			if (personalContactPersons.Count > 0)
			{
				CallContext callContext = null;
				try
				{
					callContext = FindPeopleSpeechRecognitionResultHandler.CreateAndSetCallContext(this.httpContext);
					this.userContext.LockAndReconnectMailboxSession(3000);
					ExchangeVersion.ExecuteWithSpecifiedVersion(ExchangeVersion.Exchange2012, delegate
					{
						ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<int>((long)this.GetHashCode(), "Creating Personas from Person Id, PersonId count:{0}", personalContactPersons.Count);
						foreach (string text in personalContactPersons.Keys)
						{
							PersonId personId = PersonId.Create(text);
							ItemId personaId = IdConverter.PersonaIdFromPersonId(this.userContext.MailboxSession.MailboxGuid, personId);
							Persona persona = Persona.LoadFromPersonaId(this.userContext.MailboxSession, null, personaId, FindPeopleSpeechRecognitionResultHandler.DefaultPersonaShape, null, null);
							persona.SpeechConfidence = personalContactPersons[text].Confidence;
							personas.Add(persona);
						}
						ExTraceGlobals.SpeechRecognitionTracer.TraceDebug<int>((long)this.GetHashCode(), "Created Personas from Person Id, PersonId count:{0}", personalContactPersons.Count);
					});
				}
				finally
				{
					if (this.userContext.MailboxSessionLockedByCurrentThread())
					{
						this.userContext.UnlockAndDisconnectMailboxSession();
					}
					FindPeopleSpeechRecognitionResultHandler.DisposeContext(callContext);
				}
			}
			return personas;
		}

		private const string AttributionID = "0";

		private static readonly PersonaResponseShape DefaultPersonaShape = new PersonaResponseShape(ShapeEnum.Default);

		private ADRecipient userRecipient;

		private UserContext userContext;

		private HttpContext httpContext;

		private delegate void AddContactToDictionary(Dictionary<string, PeopleSpeechPersonObject> contactDictionary, PeopleSpeechPersonObject personObject);

		private class PersonaComparer : IComparer<Persona>
		{
			public int Compare(Persona x, Persona y)
			{
				if (x.SpeechConfidence > y.SpeechConfidence)
				{
					return -1;
				}
				if (x.SpeechConfidence < y.SpeechConfidence)
				{
					return 1;
				}
				return 0;
			}
		}
	}
}
