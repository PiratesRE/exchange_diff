using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal static class TextProcessorUtils
	{
		private static Dictionary<TextProcessorType, TextProcessorGrouping> GetDiskOobProcessorsGroupedByType()
		{
			return TextProcessorUtils.GetOnDiskMceConfigData<Dictionary<TextProcessorType, TextProcessorGrouping>>((TextProcessorUtils.MceConfigManagerBase diskMceConfigManager) => diskMceConfigManager.OobTextProcessorGrouping);
		}

		private static T GetOnDiskMceConfigData<T>(Func<TextProcessorUtils.MceConfigManagerBase, T> onDiskMceConfigDataExtractorFunc) where T : class
		{
			T result;
			try
			{
				TextProcessorUtils.MceConfigManagerBase value = TextProcessorUtils.onDiskMceConfigManager.Value;
				T t = onDiskMceConfigDataExtractorFunc(value);
				result = t;
			}
			catch (SystemException underlyingException)
			{
				ClassificationDefinitionsDiagnosticsReporter.Instance.WriteClassificationEngineConfigurationErrorInformation(0, underlyingException);
				result = default(T);
			}
			return result;
		}

		internal static Dictionary<TextProcessorType, TextProcessorGrouping> OobProcessorsGroupedByType
		{
			get
			{
				return TextProcessorUtils.GetDiskOobProcessorsGroupedByType() ?? TextProcessorUtils.embeddedMceConfigManager.Value.OobTextProcessorGrouping;
			}
		}

		internal static IDictionary<string, ExchangeBuild> GetTextProcessorsFromTextProcessorGrouping(IDictionary<TextProcessorType, TextProcessorGrouping> textProcessorsGroupings, Func<TextProcessorType, bool> predicate = null)
		{
			if (textProcessorsGroupings == null)
			{
				throw new ArgumentNullException("textProcessorsGroupings");
			}
			Dictionary<string, ExchangeBuild> dictionary = new Dictionary<string, ExchangeBuild>(ClassificationDefinitionConstants.TextProcessorIdComparer);
			foreach (KeyValuePair<TextProcessorType, TextProcessorGrouping> keyValuePair in textProcessorsGroupings)
			{
				if (predicate == null || predicate(keyValuePair.Key))
				{
					foreach (KeyValuePair<string, ExchangeBuild> keyValuePair2 in ((IEnumerable<KeyValuePair<string, ExchangeBuild>>)keyValuePair.Value))
					{
						dictionary.Add(keyValuePair2.Key, keyValuePair2.Value);
					}
				}
			}
			return dictionary;
		}

		internal static IEnumerable<KeyValuePair<string, ExchangeBuild>> GetTextProcessorReferences(XElement rulePackElement, ISet<string> matchElementNames)
		{
			if (rulePackElement == null)
			{
				throw new ArgumentNullException("rulePackElement");
			}
			if (matchElementNames == null)
			{
				throw new ArgumentNullException("matchElementNames");
			}
			return from rulePackChildElement in rulePackElement.Descendants().AsParallel<XElement>()
			where matchElementNames.Contains(rulePackChildElement.Name.LocalName)
			let textProcessorId = rulePackChildElement.Attribute("idRef").Value
			let version = XmlProcessingUtils.GetRulePackElementVersion(rulePackChildElement)
			select new KeyValuePair<string, ExchangeBuild>(textProcessorId, version);
		}

		internal static IEnumerable<KeyValuePair<string, ExchangeBuild>> GetTextProcessorReferences(XElement rulePackElement)
		{
			return TextProcessorUtils.GetTextProcessorReferences(rulePackElement, ClassificationDefinitionConstants.MceMatchElementNames);
		}

		internal static IEnumerable<TextProcessorGrouping> GetRulePackScopedTextProcessorsGroupedByType(XDocument rulePackXDoc)
		{
			if (rulePackXDoc == null)
			{
				throw new ArgumentNullException("rulePackXDoc");
			}
			return TextProcessorUtils.MceConfigManagerBase.GetTextProcessorsGroupedByType(rulePackXDoc, ClassificationDefinitionConstants.MceCustomProcessorElementNames);
		}

		internal static ExchangeBuild GetTextProcessorVersion(TextProcessorType textProcessorType, XElement textProcessorElement)
		{
			if (textProcessorElement == null)
			{
				throw new ArgumentNullException("textProcessorElement");
			}
			ExAssert.RetailAssert(TextProcessorType.Function <= textProcessorType && textProcessorType <= TextProcessorType.Fingerprint, "The specified textProcessorType '{0}' is out-of-range", new object[]
			{
				textProcessorType.ToString()
			});
			ExchangeBuild result = ClassificationDefinitionConstants.DefaultVersion;
			if (ClassificationDefinitionConstants.TextProcessorTypeToVersions.ContainsKey(textProcessorType))
			{
				result = ClassificationDefinitionConstants.TextProcessorTypeToVersions[textProcessorType];
			}
			else if (textProcessorType == TextProcessorType.Function)
			{
				string value = textProcessorElement.Attribute("id").Value;
				ExAssert.RetailAssert(value != null, "The functionName in the specfied textProcessorElement is null", new object[]
				{
					textProcessorElement.ToString()
				});
				if (ClassificationDefinitionConstants.FunctionNameToVersions.ContainsKey(value))
				{
					result = ClassificationDefinitionConstants.FunctionNameToVersions[value];
				}
			}
			return result;
		}

		private static readonly Lazy<TextProcessorUtils.MceConfigManagerBase> embeddedMceConfigManager = new Lazy<TextProcessorUtils.MceConfigManagerBase>(new Func<TextProcessorUtils.MceConfigManagerBase>(TextProcessorUtils.EmbeddedMceConfigManager.Create), LazyThreadSafetyMode.PublicationOnly);

		private static readonly Lazy<TextProcessorUtils.MceConfigManagerBase> onDiskMceConfigManager = new Lazy<TextProcessorUtils.MceConfigManagerBase>(new Func<TextProcessorUtils.MceConfigManagerBase>(TextProcessorUtils.OnDiskMceConfigManager.Create), LazyThreadSafetyMode.PublicationOnly);

		private abstract class MceConfigManagerBase
		{
			internal virtual Dictionary<TextProcessorType, TextProcessorGrouping> OobTextProcessorGrouping
			{
				get
				{
					return this.cachedOobTextProcessorsGroupedByType;
				}
				private set
				{
					this.cachedOobTextProcessorsGroupedByType = value;
				}
			}

			private static TextProcessorType ParseTextProcessorType(string textProcessorElementName)
			{
				ExAssert.RetailAssert(textProcessorElementName != null, "The text processor element name passed to ParseTextProcessorType should not be null!");
				TextProcessorType result;
				bool condition = Enum.TryParse<TextProcessorType>(textProcessorElementName, false, out result);
				ExAssert.RetailAssert(condition, "The text processor element name '{0}' cannot be parsed into TextProcessorType enum", new object[]
				{
					textProcessorElementName
				});
				return result;
			}

			internal static IEnumerable<TextProcessorGrouping> GetTextProcessorsGroupedByType(XDocument rulePackXDoc, ISet<string> textProcessorElementNames)
			{
				ExAssert.RetailAssert(rulePackXDoc != null, "The rule pack XDocument instance passed to GetTextProcessorIdsGroupedByType cannot be null!");
				ExAssert.RetailAssert(textProcessorElementNames != null, "Must specify the text processor element names when calling GetTextProcessorIdsGroupedByType");
				return from rulePackElement in rulePackXDoc.Descendants().AsParallel<XElement>()
				let rulePackElementName = rulePackElement.Name.LocalName
				where textProcessorElementNames.Contains(rulePackElementName)
				let textProcessorType = TextProcessorUtils.MceConfigManagerBase.ParseTextProcessorType(rulePackElementName)
				let versionedTextProcessor = new KeyValuePair<string, ExchangeBuild>(rulePackElement.Attribute("id").Value, TextProcessorUtils.GetTextProcessorVersion(textProcessorType, rulePackElement))
				group versionedTextProcessor by textProcessorType into textProcessorGrouping
				select new TextProcessorGrouping(textProcessorGrouping, null);
			}

			private static IEnumerable<TextProcessorGrouping> GetOobProcessorsGroupedByType(XDocument rulePackXDoc)
			{
				return TextProcessorUtils.MceConfigManagerBase.GetTextProcessorsGroupedByType(rulePackXDoc, ClassificationDefinitionConstants.MceProcessorElementNames);
			}

			private XDocument DeserializeMceConfig()
			{
				XmlReaderSettings xmlReaderSettings = ClassificationDefinitionUtils.CreateSafeXmlReaderSettings();
				xmlReaderSettings.IgnoreComments = true;
				xmlReaderSettings.ValidationType = ValidationType.None;
				XDocument result;
				using (Stream stream = this.OpenSourceStream())
				{
					using (XmlReader xmlReader = XmlReader.Create(stream, xmlReaderSettings))
					{
						result = XDocument.Load(xmlReader);
					}
				}
				return result;
			}

			protected virtual void ReadConfigStream()
			{
				XDocument rulePackXDoc = this.DeserializeMceConfig();
				try
				{
					Dictionary<TextProcessorType, TextProcessorGrouping> dictionary = TextProcessorUtils.MceConfigManagerBase.GetOobProcessorsGroupedByType(rulePackXDoc).ToDictionary((TextProcessorGrouping textProcessorGroup) => textProcessorGroup.Key);
					this.cachedOobTextProcessorsGroupedByType = dictionary;
				}
				catch (AggregateException ex)
				{
					throw new XmlException(Strings.ClassificationRuleCollectionConfigValidationUnexpectedContents, ex.Flatten());
				}
			}

			protected abstract Stream OpenSourceStream();

			private Dictionary<TextProcessorType, TextProcessorGrouping> cachedOobTextProcessorsGroupedByType;
		}

		private class EmbeddedMceConfigManager : TextProcessorUtils.MceConfigManagerBase
		{
			protected override Stream OpenSourceStream()
			{
				return ClassificationDefinitionUtils.LoadStreamFromEmbeddedResource("MceConfig.xml");
			}

			internal static TextProcessorUtils.EmbeddedMceConfigManager Create()
			{
				TextProcessorUtils.EmbeddedMceConfigManager embeddedMceConfigManager = new TextProcessorUtils.EmbeddedMceConfigManager();
				embeddedMceConfigManager.ReadConfigStream();
				return embeddedMceConfigManager;
			}
		}

		private class OnDiskMceConfigManager : TextProcessorUtils.MceConfigManagerBase
		{
			private OnDiskMceConfigManager(FileInfo configFileInfo)
			{
				ExAssert.RetailAssert(configFileInfo != null, "The FileInfo passed into OnDiskMceConfigManager should not be null!");
				this.readerWriterLock = new ReaderWriterLockSlim();
				this.configFileInfo = configFileInfo;
			}

			internal override Dictionary<TextProcessorType, TextProcessorGrouping> OobTextProcessorGrouping
			{
				get
				{
					return this.GetCachedConfigData<Dictionary<TextProcessorType, TextProcessorGrouping>>(() => this.<>n__FabricatedMethod22());
				}
			}

			private T GetCachedConfigData<T>(Func<T> getCachedDataFunc)
			{
				T result;
				using (UpgradeableReadLockScope upgradeableReadLockScope = UpgradeableReadLockScope.Create(this.readerWriterLock))
				{
					if (this.IsDirty())
					{
						using (upgradeableReadLockScope.Upgrade())
						{
							this.configFileInfo.Refresh();
							this.ReadConfigStream();
							return getCachedDataFunc();
						}
					}
					using (upgradeableReadLockScope.Downgrade())
					{
						result = getCachedDataFunc();
					}
				}
				return result;
			}

			private bool IsDirty()
			{
				FileInfo fileInfo = new FileInfo(this.configFileInfo.FullName);
				return !fileInfo.Exists || !this.configFileInfo.LastWriteTimeUtc.Equals(fileInfo.LastWriteTimeUtc);
			}

			private static string GetOnDiskMceConfigFilePath()
			{
				string localMachineMceConfigDirectory = ClassificationDefinitionUtils.GetLocalMachineMceConfigDirectory(false);
				return Path.Combine(localMachineMceConfigDirectory, ClassificationDefinitionConstants.OnDiskMceConfigFileName);
			}

			internal static TextProcessorUtils.OnDiskMceConfigManager Create()
			{
				string onDiskMceConfigFilePath = TextProcessorUtils.OnDiskMceConfigManager.GetOnDiskMceConfigFilePath();
				FileInfo fileInfo = new FileInfo(onDiskMceConfigFilePath);
				if (!fileInfo.Exists)
				{
					throw new FileNotFoundException(new FileNotFoundException().Message, onDiskMceConfigFilePath);
				}
				TextProcessorUtils.OnDiskMceConfigManager onDiskMceConfigManager = new TextProcessorUtils.OnDiskMceConfigManager(fileInfo);
				onDiskMceConfigManager.ReadConfigStream();
				return onDiskMceConfigManager;
			}

			protected override Stream OpenSourceStream()
			{
				return this.configFileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
			}

			private readonly ReaderWriterLockSlim readerWriterLock;

			private readonly FileInfo configFileInfo;
		}
	}
}
