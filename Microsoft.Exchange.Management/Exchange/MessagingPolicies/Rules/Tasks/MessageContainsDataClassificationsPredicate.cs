using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Management.ClassificationDefinitions;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public class MessageContainsDataClassificationsPredicate : TransportRulePredicate, IEquatable<MessageContainsDataClassificationsPredicate>
	{
		public override int GetHashCode()
		{
			return Utils.GetHashCodeForArray<Hashtable>(this.DataClassifications);
		}

		public override bool Equals(object right)
		{
			return !object.ReferenceEquals(right, null) && (object.ReferenceEquals(this, right) || (!(base.GetType() != right.GetType()) && this.Equals(right as MessageContainsDataClassificationsPredicate)));
		}

		public bool Equals(MessageContainsDataClassificationsPredicate other)
		{
			if (this.DataClassifications == null)
			{
				return null == other.DataClassifications;
			}
			return this.DataClassifications.SequenceEqual(other.DataClassifications);
		}

		[ExceptionParameterName("ExceptIfMessageContainsDataClassifications")]
		[LocDisplayName(RulesTasksStrings.IDs.MessageDataClassificationDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.MessageDataClassificationDescription)]
		[ConditionParameterName("MessageContainsDataClassifications")]
		public Hashtable[] DataClassifications
		{
			get
			{
				return this.dataClassifications;
			}
			set
			{
				this.dataClassifications = value;
			}
		}

		internal OrganizationId OrganizationId { get; set; }

		internal override string Description
		{
			get
			{
				IEnumerable<TargetDataClassification> source = MessageContainsDataClassificationsPredicate.HashtablesToDataClassifications(this.DataClassifications);
				List<string> list;
				if (this.OrganizationId != null)
				{
					list = (from v in DlpUtils.QueryDataClassification(from value in source
					select value.Id, this.OrganizationId, null, null, null)
					select v.LocalizedName).ToList<string>();
				}
				else
				{
					list = new List<string>(this.DataClassifications.Count<Hashtable>());
					list.AddRange(from dataClassification in this.DataClassifications
					select (string)dataClassification["Name"]);
				}
				string lists = RuleDescription.BuildDescriptionStringFromStringArray(list, RulesTasksStrings.RuleDescriptionOrDelimiter, base.MaxDescriptionListLength);
				return RulesTasksStrings.RuleDescriptionMessageContainsDataClassifications(lists);
			}
		}

		[LocDisplayName(RulesTasksStrings.IDs.SubTypeDisplayName)]
		[LocDescription(RulesTasksStrings.IDs.SubTypeDescription)]
		public override IEnumerable<RuleSubType> RuleSubTypes
		{
			get
			{
				return new RuleSubType[]
				{
					RuleSubType.None,
					RuleSubType.Dlp
				};
			}
		}

		public MessageContainsDataClassificationsPredicate()
		{
		}

		internal MessageContainsDataClassificationsPredicate(Hashtable[] classifications)
		{
			this.dataClassifications = classifications;
		}

		internal override Condition ToInternalCondition()
		{
			IEnumerable<TargetDataClassification> enumerable = MessageContainsDataClassificationsPredicate.HashtablesToDataClassifications(this.DataClassifications);
			ShortList<ShortList<KeyValuePair<string, string>>> shortList = new ShortList<ShortList<KeyValuePair<string, string>>>();
			foreach (TargetDataClassification targetDataClassification in enumerable)
			{
				shortList.Add(targetDataClassification.ToKeyValueCollection());
			}
			return TransportRuleParser.Instance.CreatePredicate("containsDataClassification", TransportRuleParser.Instance.CreateProperty("Message.DataClassifications"), shortList);
		}

		internal static TransportRulePredicate CreateFromInternalCondition(Condition condition)
		{
			ContainsDataClassificationPredicate containsDataClassificationPredicate = condition as ContainsDataClassificationPredicate;
			if (containsDataClassificationPredicate == null || !containsDataClassificationPredicate.Name.Equals("containsDataClassification"))
			{
				return null;
			}
			ShortList<ShortList<KeyValuePair<string, string>>> shortList = (ShortList<ShortList<KeyValuePair<string, string>>>)containsDataClassificationPredicate.Value.GetValue(null);
			List<Hashtable> list = new List<Hashtable>(shortList.Count);
			foreach (ShortList<KeyValuePair<string, string>> shortList2 in shortList)
			{
				Hashtable hashtable = new Hashtable();
				foreach (KeyValuePair<string, string> keyValuePair in shortList2)
				{
					hashtable.Add(MessageContainsDataClassificationsPredicate.EngineKeyToCmdletParameter(keyValuePair.Key).ToUpper(), keyValuePair.Value);
				}
				list.Add(hashtable);
			}
			return new MessageContainsDataClassificationsPredicate(list.ToArray());
		}

		internal static string EngineKeyToCmdletParameter(string engineKey)
		{
			return MessageContainsDataClassificationsPredicate.engineKeysToCmdletParameterNames[engineKey];
		}

		internal override void Reset()
		{
			this.DataClassifications = null;
			base.Reset();
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			if (this.DataClassifications == null || this.DataClassifications.Length == 0)
			{
				errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.ArgumentNotSet, base.Name));
				return;
			}
			base.ValidateRead(errors);
		}

		internal static TargetDataClassification HashtableToDataClassification(Hashtable parameter)
		{
			string text = (string)parameter["Name".ToUpper()];
			if (text == null)
			{
				return null;
			}
			TargetDataClassification defaults = MessageContainsDataClassificationsPredicate.GetDefaults(text);
			if (defaults == null)
			{
				return null;
			}
			return new TargetDataClassification(defaults.Id, MessageContainsDataClassificationsPredicate.GetValueOrDefault(parameter, "MinCount".ToUpper(), defaults.MinCount), MessageContainsDataClassificationsPredicate.GetValueOrDefault(parameter, "MaxCount".ToUpper(), defaults.MaxCount), MessageContainsDataClassificationsPredicate.GetValueOrDefault(parameter, "MinConfidence".ToUpper(), defaults.MinConfidence), MessageContainsDataClassificationsPredicate.GetValueOrDefault(parameter, "MaxConfidence".ToUpper(), defaults.MaxConfidence), MessageContainsDataClassificationsPredicate.GetValueOrDefault(parameter, TargetDataClassification.OpaqueDataKey.ToUpper(), string.Empty));
		}

		internal static IEnumerable<TargetDataClassification> HashtablesToDataClassifications(IEnumerable<Hashtable> parameters)
		{
			return Utils.UppercaseHashtableKeys(parameters).Select(new Func<Hashtable, TargetDataClassification>(MessageContainsDataClassificationsPredicate.HashtableToDataClassification)).ToList<TargetDataClassification>();
		}

		internal static IEnumerable<string> HashtablesToStrings(IEnumerable<Hashtable> parameters, OrganizationId orgId)
		{
			IEnumerable<TargetDataClassification> source = MessageContainsDataClassificationsPredicate.HashtablesToDataClassifications(parameters);
			if (orgId == null)
			{
				return from classification in source
				select classification.ToString();
			}
			string empty = string.Empty;
			IEnumerable<string> result;
			try
			{
				Dictionary<string, Tuple<string, string>> resolvedClassifications = DlpUtils.QueryDataClassification(from value in source
				select value.Id, orgId, null, null, null).ToDictionary((DataClassificationPresentationObject k) => k.Identity.ToString(), (DataClassificationPresentationObject v) => new Tuple<string, string>(v.Name, v.LocalizedName));
				result = from classificationObject in source
				select MessageContainsDataClassificationsPredicate.FormatClassificationString(classificationObject, resolvedClassifications);
			}
			catch (KeyNotFoundException)
			{
				throw new ArgumentException(RulesTasksStrings.InvalidMessageClassification(empty));
			}
			return result;
		}

		internal static string FormatClassificationString(TargetDataClassification classification, Dictionary<string, Tuple<string, string>> classificationIdToNames)
		{
			return string.Format("{{{0}:\"{1}\", {2}:\"{3}\", {4}:\"{5}\", {6}:{7}, {8}:{9}, {10}:{11}, {12}:{13}}}", new object[]
			{
				TargetDataClassification.IdKey,
				classificationIdToNames[classification.Id].Item1,
				"guid",
				classification.Id,
				"displayName",
				classificationIdToNames[classification.Id].Item2,
				TargetDataClassification.MinCountKey,
				classification.MinCount,
				TargetDataClassification.MaxCountKey,
				(classification.MaxCount == TargetDataClassification.IgnoreMaxCount) ? "Infinity" : classification.MaxCount.ToString(CultureInfo.InvariantCulture),
				TargetDataClassification.MinConfidenceKey,
				(classification.MinConfidence == TargetDataClassification.UseRecommendedMinConfidence) ? "Recommended" : classification.MinConfidence.ToString(CultureInfo.InvariantCulture),
				TargetDataClassification.MaxConfidenceKey,
				classification.MaxConfidence
			});
		}

		internal static int GetValueOrDefault(Hashtable parameter, string keyName, int defaultValue)
		{
			if (!parameter.ContainsKey(keyName))
			{
				return defaultValue;
			}
			int result;
			if (int.TryParse(parameter[keyName].ToString(), out result))
			{
				return result;
			}
			return int.MinValue;
		}

		internal static string GetValueOrDefault(Hashtable parameter, string keyName, string defaultValue)
		{
			if (parameter.ContainsKey(keyName))
			{
				return (string)parameter[keyName];
			}
			return defaultValue;
		}

		internal static TargetDataClassification GetDefaults(string classificationId)
		{
			return new TargetDataClassification(classificationId, 1, TargetDataClassification.IgnoreMaxCount, TargetDataClassification.UseRecommendedMinConfidence, TargetDataClassification.MaxAllowedConfidenceValue, string.Empty);
		}

		internal static ArgumentException ValidateDataClassificationParameters(OrganizationId orgId, IEnumerable<Hashtable> classifications)
		{
			if (classifications == null || classifications.Count<Hashtable>() == 0)
			{
				return null;
			}
			ArgumentException ex = MessageContainsDataClassificationsPredicate.ValidateDataClassificationStaticParameters(classifications);
			if (ex != null)
			{
				return ex;
			}
			HashSet<string> hashSet = new HashSet<string>();
			foreach (Hashtable hashtable in classifications)
			{
				TargetDataClassification targetDataClassification = MessageContainsDataClassificationsPredicate.HashtableToDataClassification(hashtable);
				ArgumentException ex2 = MessageContainsDataClassificationsPredicate.ValidateDataClassificationParameter(targetDataClassification, hashtable);
				if (ex2 != null)
				{
					return ex2;
				}
				if (!hashSet.Add(targetDataClassification.Id))
				{
					return new ArgumentException(RulesTasksStrings.DuplicateDataClassificationSpecified);
				}
			}
			try
			{
				var source = from value in DlpUtils.QueryDataClassification(hashSet, orgId, null, null, null)
				select new
				{
					InvariantName = value.Name,
					LocalizedName = value.LocalizedName,
					Identity = value.Identity.ToString(),
					ClassificationRuleCollectionId = value.ClassificationRuleCollection.Name
				};
				foreach (Hashtable hashtable2 in classifications)
				{
					string classificationName = (string)hashtable2["Name".ToUpper()];
					var <>f__AnonymousType = source.FirstOrDefault(dataClassification => dataClassification.InvariantName == classificationName || dataClassification.LocalizedName == classificationName || string.Equals(dataClassification.Identity, classificationName, StringComparison.OrdinalIgnoreCase));
					if (<>f__AnonymousType == null)
					{
						return new ArgumentException(RulesTasksStrings.InvalidMessageDataClassification(classificationName));
					}
					hashtable2["Name".ToUpper()] = <>f__AnonymousType.Identity;
					hashtable2[TargetDataClassification.OpaqueDataKey.ToUpper()] = <>f__AnonymousType.ClassificationRuleCollectionId;
				}
			}
			catch (ArgumentException result)
			{
				return result;
			}
			return null;
		}

		internal static ArgumentException ValidateDataClassificationStaticParameters(IEnumerable<Hashtable> classifications)
		{
			if (classifications == null || classifications.Count<Hashtable>() == 0)
			{
				return new ArgumentException(RulesTasksStrings.MissingDataClassificationsParameter);
			}
			HashSet<string> hashSet = new HashSet<string>();
			foreach (Hashtable hashtable in classifications)
			{
				TargetDataClassification targetDataClassification = MessageContainsDataClassificationsPredicate.HashtableToDataClassification(hashtable);
				if (targetDataClassification == null)
				{
					return new ArgumentException(RulesTasksStrings.MissingDataClassificationsName);
				}
				ArgumentException ex = MessageContainsDataClassificationsPredicate.ValidateDataClassificationParameter(targetDataClassification, hashtable);
				if (ex != null)
				{
					return ex;
				}
				if (!hashSet.Add(targetDataClassification.Id))
				{
					return new ArgumentException(RulesTasksStrings.DuplicateDataClassificationSpecified);
				}
			}
			return null;
		}

		internal static ArgumentException ValidateDataClassificationParameter(TargetDataClassification targetClassification, Hashtable userInput)
		{
			if (string.IsNullOrEmpty(targetClassification.Id))
			{
				return new ArgumentException(RulesTasksStrings.InvalidMessageDataClassificationEmptyName);
			}
			if (targetClassification.MinConfidence != TargetDataClassification.UseRecommendedMinConfidence && targetClassification.MinConfidence < 1)
			{
				return new ArgumentException(RulesTasksStrings.InvalidMessageDataClassificationParameterLessThanOne("MinConfidence"));
			}
			if (targetClassification.MaxConfidence < 1)
			{
				return new ArgumentException(RulesTasksStrings.InvalidMessageDataClassificationParameterLessThanOne("MaxConfidence"));
			}
			if (targetClassification.MinConfidence != TargetDataClassification.UseRecommendedMinConfidence && targetClassification.MinConfidence > targetClassification.MaxConfidence)
			{
				return new ArgumentException(RulesTasksStrings.InvalidMessageDataClassificationParameterMinGreaterThanMax("MinConfidence", "MaxConfidence"));
			}
			if (targetClassification.MinConfidence > TargetDataClassification.MaxAllowedConfidenceValue)
			{
				return new ArgumentException(RulesTasksStrings.InvalidMessageDataClassificationParameterConfidenceExceedsMaxAllowed("MinConfidence", TargetDataClassification.MaxAllowedConfidenceValue));
			}
			if (targetClassification.MaxConfidence > TargetDataClassification.MaxAllowedConfidenceValue)
			{
				return new ArgumentException(RulesTasksStrings.InvalidMessageDataClassificationParameterConfidenceExceedsMaxAllowed("MaxConfidence", TargetDataClassification.MaxAllowedConfidenceValue));
			}
			if (targetClassification.MinCount < 1)
			{
				return new ArgumentException(RulesTasksStrings.InvalidMessageDataClassificationParameterLessThanOne("MinCount"));
			}
			if ((targetClassification.MaxCount != TargetDataClassification.IgnoreMaxCount && targetClassification.MaxCount < 1) || (targetClassification.MaxCount == TargetDataClassification.IgnoreMaxCount && userInput.ContainsKey("MaxCount".ToUpper())))
			{
				return new ArgumentException(RulesTasksStrings.InvalidMessageDataClassificationParameterLessThanOne("MaxCount"));
			}
			if (targetClassification.MaxCount != TargetDataClassification.IgnoreMaxCount && targetClassification.MinCount > targetClassification.MaxCount)
			{
				return new ArgumentException(RulesTasksStrings.InvalidMessageDataClassificationParameterMinGreaterThanMax("MinCount", "MaxCount"));
			}
			return null;
		}

		internal override string GetPredicateParameters()
		{
			bool flag = true;
			StringBuilder stringBuilder = new StringBuilder();
			List<TargetDataClassification> list = MessageContainsDataClassificationsPredicate.HashtablesToDataClassifications(this.DataClassifications).ToList<TargetDataClassification>();
			Dictionary<string, string> dictionary = null;
			if (this.OrganizationId != null)
			{
				dictionary = DlpUtils.QueryDataClassification(from value in list
				select value.Id, this.OrganizationId, null, null, null).ToDictionary((DataClassificationPresentationObject k) => k.Identity.ToString(), (DataClassificationPresentationObject v) => v.Name);
			}
			foreach (TargetDataClassification targetDataClassification in list)
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					stringBuilder.Append(", ");
				}
				string input = targetDataClassification.Id;
				if (dictionary != null)
				{
					input = dictionary[targetDataClassification.Id];
				}
				stringBuilder.Append(string.Format("@{{'Name'={0}; 'MinCount'={1};{2}'MinConfidence'={3}; 'MaxConfidence'={4}}}", new object[]
				{
					Utils.QuoteCmdletParameter(input),
					targetDataClassification.MinCount,
					(targetDataClassification.MaxCount > -1) ? string.Format(" 'MaxCount'={0}; ", targetDataClassification.MaxCount) : " ",
					targetDataClassification.MinConfidence,
					targetDataClassification.MaxConfidence
				}));
			}
			return stringBuilder.ToString();
		}

		internal override void SuppressPiiData()
		{
		}

		private const int invalidIntegerParameterValue = -2147483648;

		private Hashtable[] dataClassifications;

		private static readonly Dictionary<string, string> engineKeysToCmdletParameterNames = new Dictionary<string, string>
		{
			{
				TargetDataClassification.IdKey,
				"Name"
			},
			{
				TargetDataClassification.MinCountKey,
				"MinCount"
			},
			{
				TargetDataClassification.MaxCountKey,
				"MaxCount"
			},
			{
				TargetDataClassification.MinConfidenceKey,
				"MinConfidence"
			},
			{
				TargetDataClassification.MaxConfidenceKey,
				"MaxConfidence"
			},
			{
				TargetDataClassification.OpaqueDataKey,
				TargetDataClassification.OpaqueDataKey
			}
		};

		private static class McdcKeyNames
		{
			internal const string GuidKey = "guid";

			internal const string DisplayNameKey = "displayName";
		}
	}
}
