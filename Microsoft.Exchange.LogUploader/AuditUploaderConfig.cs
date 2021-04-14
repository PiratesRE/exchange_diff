using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using Microsoft.Exchange.LogUploaderProxy;

namespace Microsoft.Exchange.LogUploader
{
	public static class AuditUploaderConfig
	{
		public static int RuleCount
		{
			get
			{
				return AuditUploaderConfig.filteringRules.Count;
			}
		}

		public static void Initialize(string fileName)
		{
			try
			{
				using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
				{
					XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fileStream, new XmlDictionaryReaderQuotas());
					AuditUploaderConfig.InitializeWithReader(reader);
					fileStream.Close();
				}
			}
			catch (DirectoryNotFoundException ex)
			{
				EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_ConfigFileNotFound, fileName, new object[]
				{
					ex.Message
				});
				AuditUploaderConfig.InitializeWithReader(null);
			}
			catch (FileNotFoundException ex2)
			{
				EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_ConfigFileNotFound, fileName, new object[]
				{
					ex2.Message
				});
				AuditUploaderConfig.InitializeWithReader(null);
			}
		}

		public static void InitializeWithReader(XmlDictionaryReader reader)
		{
			DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof(AuditUploaderConfigSchema));
			if (reader != null)
			{
				try
				{
					try
					{
						AuditUploaderConfig.config = (AuditUploaderConfigSchema)dataContractSerializer.ReadObject(reader, true);
					}
					catch (SerializationException ex)
					{
						EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_InvalidConfigFile, ex.Message, new object[0]);
						AuditUploaderConfig.InitializeDefault();
					}
					goto IL_58;
				}
				finally
				{
					reader.Close();
				}
			}
			AuditUploaderConfig.InitializeDefault();
			IL_58:
			AuditUploaderConfig.InitializeParsingRules();
			AuditUploaderConfig.GenerateDictionary();
		}

		public static void InitializeDefault()
		{
			FilteringPredicate filteringPredicate = new FilteringPredicate();
			filteringPredicate.Initialize();
			FilteringRule filteringRule = new FilteringRule();
			filteringRule.ActionToPerform = Actions.LetThrough;
			filteringRule.Predicate = filteringPredicate;
			filteringRule.Throttle = null;
			List<FilteringRule> list = new List<FilteringRule>();
			list.Add(filteringRule);
			AuditUploaderConfig.config = new AuditUploaderConfigSchema();
			AuditUploaderConfig.config.FilteringSection = list;
		}

		public static Actions GetAction(string component, string tenant, string user, string operation)
		{
			if (AuditUploaderConfig.filteringRules == null)
			{
				EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_InvalidRuleCollection, "Audit Uploader rules collection was not properly generated.", new object[0]);
				AuditUploaderConfig.InitializeWithReader(null);
			}
			return AuditUploaderConfig.filteringRules.GetOperation(component, tenant, user, operation, DateTime.UtcNow);
		}

		public static Actions GetAction(string component, string tenant, string user, string operation, DateTime currentTime)
		{
			if (AuditUploaderConfig.filteringRules == null)
			{
				EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_InvalidRuleCollection, "Audit Uploader rules collection was not properly generated.", new object[0]);
				AuditUploaderConfig.InitializeWithReader(null);
			}
			return AuditUploaderConfig.filteringRules.GetOperation(component, tenant, user, operation, currentTime);
		}

		public static Actions ParsingCheck(Parsing outcome)
		{
			return AuditUploaderConfig.parsingRules[outcome];
		}

		public static void Reset()
		{
			AuditUploaderConfig.config = null;
			AuditUploaderConfig.filteringRules = null;
			AuditUploaderConfig.parsingRules[Parsing.Failed] = Actions.LetThrough;
			AuditUploaderConfig.parsingRules[Parsing.Passed] = Actions.LetThrough;
			AuditUploaderConfig.parsingRules[Parsing.PartiallyPassed] = Actions.LetThrough;
		}

		private static void InitializeParsingRules()
		{
			if (AuditUploaderConfig.config.ParsingSection != null)
			{
				foreach (ParsingRule parsingRule in AuditUploaderConfig.config.ParsingSection)
				{
					AuditUploaderConfig.parsingRules[parsingRule.Predicate.ParsingOutcome] = parsingRule.ActionToPerform;
				}
			}
		}

		private static void GenerateDictionary()
		{
			if (AuditUploaderConfig.filteringRules == null)
			{
				AuditUploaderConfig.filteringRules = new AuditUploaderConfigRules();
			}
			foreach (FilteringRule filteringRule in AuditUploaderConfig.config.FilteringSection)
			{
				AuditUploaderDictionaryKey key = new AuditUploaderDictionaryKey((filteringRule.Predicate.Component != null) ? filteringRule.Predicate.Component : AuditUploaderDictionaryKey.WildCard, (filteringRule.Predicate.Tenant != null) ? filteringRule.Predicate.Tenant : AuditUploaderDictionaryKey.WildCard, (filteringRule.Predicate.User != null) ? filteringRule.Predicate.User : AuditUploaderDictionaryKey.WildCard, (filteringRule.Predicate.Operation != null) ? filteringRule.Predicate.Operation : AuditUploaderDictionaryKey.WildCard);
				if (!AuditUploaderConfig.filteringRules.Contains(key))
				{
					AuditUploaderConfig.filteringRules.Add(key, new AuditUploaderAction(filteringRule.ActionToPerform, (filteringRule.Throttle != null) ? filteringRule.Throttle.Interval : null));
				}
				else
				{
					EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_InvalidConfigFile, "Duplicate rule detected in the configuration", new object[0]);
				}
			}
			if (!AuditUploaderConfig.filteringRules.Contains(new AuditUploaderDictionaryKey(AuditUploaderDictionaryKey.WildCard, AuditUploaderDictionaryKey.WildCard, AuditUploaderDictionaryKey.WildCard, AuditUploaderDictionaryKey.WildCard)))
			{
				AuditUploaderConfig.filteringRules.Add(new AuditUploaderDictionaryKey(AuditUploaderDictionaryKey.WildCard, AuditUploaderDictionaryKey.WildCard, AuditUploaderDictionaryKey.WildCard, AuditUploaderDictionaryKey.WildCard), new AuditUploaderAction(Actions.LetThrough, null));
			}
		}

		private static AuditUploaderConfigSchema config;

		private static Dictionary<Parsing, Actions> parsingRules = new Dictionary<Parsing, Actions>(Enum.GetNames(typeof(Parsing)).Length)
		{
			{
				Parsing.Passed,
				Actions.LetThrough
			},
			{
				Parsing.Failed,
				Actions.LetThrough
			},
			{
				Parsing.PartiallyPassed,
				Actions.LetThrough
			}
		};

		private static AuditUploaderConfigRules filteringRules;
	}
}
