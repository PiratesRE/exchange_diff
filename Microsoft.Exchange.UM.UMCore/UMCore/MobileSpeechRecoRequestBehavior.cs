using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class MobileSpeechRecoRequestBehavior : IMobileSpeechRecoRequestBehavior
	{
		public static IMobileSpeechRecoRequestBehavior Create(MobileSpeechRecoRequestType requestType, Guid requestId, CultureInfo culture, Guid userObjectGuid, Guid tenantGuid, ExTimeZone timeZone)
		{
			ValidateArgument.NotNull(culture, "culture");
			MobileSpeechRecoTracer.TraceDebug(null, requestId, "Entering MobileSpeechRecoRequestBehavior.Create culture='{0}' userObjectGuid='{1}' tenantGuid='{2}' timeZone='{3}'", new object[]
			{
				culture,
				userObjectGuid,
				tenantGuid,
				timeZone
			});
			IMobileSpeechRecoRequestBehavior result = null;
			switch (requestType)
			{
			case MobileSpeechRecoRequestType.FindInGAL:
				return new FindInGALRequestBehavior(requestId, culture, userObjectGuid, tenantGuid);
			case MobileSpeechRecoRequestType.FindInPersonalContacts:
				return new FindInPersonalContactsRequestBehavior(requestId, culture, userObjectGuid, tenantGuid);
			case MobileSpeechRecoRequestType.StaticGrammarsCombined:
				return new CombinedStaticGrammarScenariosRequestBehavior(requestId, culture, userObjectGuid, tenantGuid, timeZone);
			case MobileSpeechRecoRequestType.FindPeople:
				throw new ArgumentOutOfRangeException("requestType", requestType, "Invalid value");
			case MobileSpeechRecoRequestType.DaySearch:
				return new DaySearchBehavior(requestId, culture, userObjectGuid, tenantGuid, timeZone);
			case MobileSpeechRecoRequestType.AppointmentCreation:
				return new DateTimeAndDurationRecognitionBehavior(requestId, culture, userObjectGuid, tenantGuid, timeZone);
			}
			ExAssert.RetailAssert(false, "Invalid scenario value '{0}'", new object[]
			{
				requestType
			});
			return result;
		}

		public static Dictionary<string, string> GetKeywordsFromGrammar(string keywordGrammarId, CultureInfo culture)
		{
			string text = GrammarUtils.GetLocString(keywordGrammarId, culture);
			text = MobileSpeechRecoRequestBehavior.keywordsSpecialCharReplacer.Replace(text, " ");
			string[] array = text.Split(null, StringSplitOptions.RemoveEmptyEntries);
			Dictionary<string, string> dictionary = new Dictionary<string, string>(array.Length, StringComparer.OrdinalIgnoreCase);
			foreach (string key in array)
			{
				if (!dictionary.ContainsKey(key))
				{
					dictionary.Add(key, "1");
				}
			}
			return dictionary;
		}

		public MobileSpeechRecoRequestBehavior(Guid id, CultureInfo culture, Guid userObjectGuid, Guid tenantGuid)
		{
			ValidateArgument.NotNull(culture, "culture");
			ExAssert.RetailAssert(userObjectGuid != Guid.Empty, "userObjectGuid = '{0}' (Guid.Empty)", new object[]
			{
				userObjectGuid
			});
			MobileSpeechRecoTracer.TraceDebug(this, id, "Entering MobileSpeechRecoRequestBehavior constructor culture='{0}' userObjectGuid='{1}', tenantGuid='{2}'", new object[]
			{
				culture,
				userObjectGuid,
				tenantGuid
			});
			this.id = id;
			this.culture = culture;
			this.userObjectGuid = userObjectGuid;
			this.tenantGuid = tenantGuid;
		}

		public Guid Id
		{
			get
			{
				return this.id;
			}
		}

		public CultureInfo Culture
		{
			get
			{
				return this.culture;
			}
		}

		public abstract SpeechRecognitionEngineType EngineType { get; }

		public abstract int MaxAlternates { get; }

		public abstract int MaxProcessingTime { get; }

		public abstract List<UMGrammar> PrepareGrammars();

		public abstract string ProcessRecoResults(List<IMobileRecognitionResult> results);

		public bool CanProcessResultType(MobileSpeechRecoResultType resultType)
		{
			return this.SupportedResultTypes.Contains(resultType);
		}

		protected abstract MobileSpeechRecoResultType[] SupportedResultTypes { get; }

		protected Guid UserObjectGuid
		{
			get
			{
				return this.userObjectGuid;
			}
		}

		protected Guid TenantGuid
		{
			get
			{
				return this.tenantGuid;
			}
		}

		protected virtual void ProcessSemanticTags(Dictionary<string, string> semanticTags)
		{
		}

		protected virtual bool ShouldAcceptBasedOnSmartConfidenceThreshold(IUMRecognitionPhrase phrase, MobileSpeechRecoResultType resultType)
		{
			return true;
		}

		protected string ConvertResultsToXml(List<IMobileRecognitionResult> results, List<string> requiredTags)
		{
			string result = string.Empty;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, new UTF8Encoding()))
				{
					xmlTextWriter.Formatting = Formatting.Indented;
					xmlTextWriter.WriteStartDocument();
					xmlTextWriter.WriteStartElement("MobileReco");
					if (results.Count > 0)
					{
						IMobileRecognitionResult mobileRecognitionResult = results[0];
						List<IUMRecognitionPhrase> list = new List<IUMRecognitionPhrase>();
						foreach (IUMRecognitionPhrase iumrecognitionPhrase in mobileRecognitionResult.GetRecognitionResults())
						{
							if (this.ShouldAcceptBasedOnSmartConfidenceThreshold(iumrecognitionPhrase, mobileRecognitionResult.MobileScenarioResultType))
							{
								MobileSpeechRecoTracer.TraceDebug(this, this.Id, "ConvertResultsToXml, Adding recognition phrase '{0}', confidence '{1}' to result because it is above the smart Confidence threshold", new object[]
								{
									iumrecognitionPhrase.Text,
									iumrecognitionPhrase.Confidence
								});
								list.Add(iumrecognitionPhrase);
							}
							else
							{
								MobileSpeechRecoTracer.TraceDebug(this, this.Id, "ConvertResultsToXml, phrase '{0}', confidence '{1}' will not be added to result because it is below the smart Confidence threshold", new object[]
								{
									iumrecognitionPhrase.Text,
									iumrecognitionPhrase.Confidence
								});
							}
						}
						if (list.Count == 0)
						{
							xmlTextWriter.WriteAttributeString("ResultType", MobileSpeechRecoResultType.None.ToString());
							goto IL_28A;
						}
						xmlTextWriter.WriteAttributeString("ResultType", mobileRecognitionResult.MobileScenarioResultType.ToString());
						int num = 0;
						using (List<IUMRecognitionPhrase>.Enumerator enumerator2 = list.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								IUMRecognitionPhrase iumrecognitionPhrase2 = enumerator2.Current;
								if (num >= this.MaxAlternates)
								{
									break;
								}
								num++;
								xmlTextWriter.WriteStartElement("Alternate");
								xmlTextWriter.WriteAttributeString("text", iumrecognitionPhrase2.Text);
								xmlTextWriter.WriteAttributeString("confidence", iumrecognitionPhrase2.Confidence.ToString("F4", CultureInfo.InvariantCulture));
								Dictionary<string, string> dictionary = new Dictionary<string, string>(requiredTags.Count);
								foreach (string key in requiredTags)
								{
									dictionary.Add(key, iumrecognitionPhrase2[key].ToString());
								}
								this.ProcessSemanticTags(dictionary);
								foreach (string text in dictionary.Keys)
								{
									xmlTextWriter.WriteElementString(text, dictionary[text]);
								}
								xmlTextWriter.WriteEndElement();
							}
							goto IL_28A;
						}
					}
					xmlTextWriter.WriteAttributeString("ResultType", MobileSpeechRecoResultType.None.ToString());
					IL_28A:
					xmlTextWriter.WriteEndElement();
					xmlTextWriter.WriteEndDocument();
					xmlTextWriter.Flush();
					using (StreamReader streamReader = new StreamReader(memoryStream))
					{
						memoryStream.Seek(0L, SeekOrigin.Begin);
						result = streamReader.ReadToEnd();
					}
				}
			}
			return result;
		}

		protected ADRecipient GetADRecipient()
		{
			Guid guid = this.UserObjectGuid;
			ADRecipient result;
			try
			{
				MobileSpeechRecoTracer.TracePerformance(this, this.Id, "Entering MobileSpeechRecoRequestBehavior.GetADRecipient for '{0}'", new object[]
				{
					guid
				});
				IADRecipientLookup iadrecipientLookup = ADRecipientLookupFactory.CreateFromTenantGuid(this.TenantGuid);
				ADRecipient adrecipient = iadrecipientLookup.LookupByObjectId(new ADObjectId(guid));
				if (adrecipient == null)
				{
					MobileSpeechRecoTracer.TraceError(this, this.Id, "User with object guid '{0}' was not found", new object[]
					{
						guid
					});
					throw new UserNotFoundException(guid);
				}
				result = adrecipient;
			}
			catch (LocalizedException ex)
			{
				MobileSpeechRecoTracer.TraceError(this, this.Id, "Error looking up user with object guid '{0}' -- {1}", new object[]
				{
					guid,
					ex
				});
				throw new UserNotFoundException(guid, ex);
			}
			finally
			{
				MobileSpeechRecoTracer.TracePerformance(this, this.Id, "Leaving MobileSpeechRecoRequestBehaviorr.GetADRecipient for '{0}'", new object[]
				{
					guid
				});
			}
			return result;
		}

		private static Regex keywordsSpecialCharReplacer = new Regex("({\\d+})|;|\\(|\\)");

		private readonly Guid tenantGuid;

		private Guid id;

		private CultureInfo culture;

		private Guid userObjectGuid;
	}
}
