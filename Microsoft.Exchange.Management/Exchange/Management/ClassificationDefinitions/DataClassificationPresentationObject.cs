using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	[Serializable]
	public sealed class DataClassificationPresentationObject : IConfigurable
	{
		public ObjectId Identity { get; private set; }

		public string Name
		{
			get
			{
				return this.defaultDetails.Name;
			}
		}

		public string Description
		{
			get
			{
				return ClassificationDefinitionUtils.GetMatchingLocalizedInfo<string>(this.localizedDescriptions, this.defaultDetails.Description);
			}
		}

		public MultiValuedProperty<Fingerprint> Fingerprints
		{
			get
			{
				return this.fingerprints;
			}
			set
			{
				if (object.ReferenceEquals(this.fingerprints, value))
				{
					return;
				}
				if (this.fingerprints != null && value != null && this.fingerprints.Count == value.Count && this.fingerprints.Intersect(value).ToList<Fingerprint>().Count == this.fingerprints.Count)
				{
					return;
				}
				this.fingerprints = value;
				this.IsDirty = true;
			}
		}

		public string LocalizedName
		{
			get
			{
				return ClassificationDefinitionUtils.GetMatchingLocalizedInfo<string>(this.localizedNames, this.defaultDetails.Name);
			}
		}

		public string Publisher
		{
			get
			{
				return this.ClassificationRuleCollection.Publisher;
			}
		}

		public ClassificationTypeEnum ClassificationType { get; private set; }

		public bool? IsEncrypted
		{
			get
			{
				if (this.ClassificationRuleCollection == null)
				{
					return null;
				}
				return new bool?(this.ClassificationRuleCollection.IsEncrypted);
			}
		}

		public uint? RecommendedConfidence { get; private set; }

		public ClassificationRuleCollectionPresentationObject ClassificationRuleCollection { get; private set; }

		public ExchangeBuild MinEngineVersion { get; private set; }

		public DateTime? WhenChanged
		{
			get
			{
				if (this.ClassificationRuleCollection == null)
				{
					return null;
				}
				return this.ClassificationRuleCollection.WhenChanged;
			}
		}

		public CultureInfo DefaultCulture
		{
			get
			{
				return this.defaultDetails.Culture;
			}
		}

		public Dictionary<CultureInfo, string> AllLocalizedNames
		{
			get
			{
				return this.localizedNames;
			}
		}

		public Dictionary<CultureInfo, string> AllLocalizedDescriptions
		{
			get
			{
				return this.localizedDescriptions;
			}
		}

		internal bool IsDirty { get; private set; }

		internal void SetLocalizedName(CultureInfo locale, string value)
		{
			ArgumentValidator.ThrowIfNull("locale", locale);
			if (locale.Equals(this.defaultDetails.Culture) && !string.Equals(this.defaultDetails.Name, value))
			{
				this.defaultDetails.Name = value;
				this.IsDirty = true;
			}
			if (!this.localizedNames.ContainsKey(locale) || !string.Equals(this.localizedNames[locale], value, StringComparison.Ordinal))
			{
				DataClassificationPresentationObject.SetLocalizedResource(this.localizedNames, locale, value);
				this.IsDirty = true;
			}
		}

		internal void SetLocalizedDescription(CultureInfo locale, string value)
		{
			ArgumentValidator.ThrowIfNull("locale", locale);
			if (locale.Equals(this.defaultDetails.Culture) && !string.Equals(this.defaultDetails.Description, value))
			{
				this.defaultDetails.Description = value;
				this.IsDirty = true;
			}
			if (!this.localizedDescriptions.ContainsKey(locale) || !string.Equals(this.localizedDescriptions[locale], value, StringComparison.Ordinal))
			{
				DataClassificationPresentationObject.SetLocalizedResource(this.localizedDescriptions, locale, value);
				this.IsDirty = true;
			}
		}

		internal void SetDefaultResource(CultureInfo locale, string name, string description)
		{
			ArgumentValidator.ThrowIfNull("locale", locale);
			ArgumentValidator.ThrowIfNullOrEmpty("name", name);
			if (this.defaultDetails != null && this.defaultDetails.Culture != null && this.defaultDetails.Culture.Equals(locale) && string.Equals(this.defaultDetails.Name, name, StringComparison.Ordinal) && string.Equals(this.defaultDetails.Description, description, StringComparison.Ordinal))
			{
				return;
			}
			this.SetLocalizedName(locale, name);
			this.SetLocalizedDescription(locale, description);
			this.defaultDetails = new DataClassificationLocalizableDetails
			{
				Culture = locale,
				Name = name,
				Description = description
			};
			this.IsDirty = true;
		}

		private XElement GetRuleXElement()
		{
			List<XElement> list = new List<XElement>();
			foreach (Fingerprint fingerprint in this.Fingerprints)
			{
				list.Add(new XElement(XmlProcessingUtils.GetMceNsQualifiedNodeName("Match"), new XAttribute("idRef", fingerprint.Identity)));
			}
			XElement xelement = list[0];
			if (list.Count > 1)
			{
				xelement = new XElement(XmlProcessingUtils.GetMceNsQualifiedNodeName("Any"), new object[]
				{
					new XAttribute("minMatches", 1),
					list
				});
			}
			XElement xelement2 = new XElement(XmlProcessingUtils.GetMceNsQualifiedNodeName("Evidence"), new object[]
			{
				new XAttribute("confidenceLevel", 75),
				xelement
			});
			return new XElement(XmlProcessingUtils.GetMceNsQualifiedNodeName("Affinity"), new object[]
			{
				new XAttribute("id", ((DataClassificationObjectId)this.Identity).Name),
				new XAttribute("evidencesProximity", 300),
				new XAttribute("thresholdConfidenceLevel", 75),
				xelement2
			});
		}

		private XElement GetResourceXElement()
		{
			List<XElement> list = new List<XElement>();
			list.Add(new XElement(XmlProcessingUtils.GetMceNsQualifiedNodeName("Name"), new object[]
			{
				new XAttribute("default", "true"),
				new XAttribute("langcode", this.defaultDetails.Culture.Name),
				this.defaultDetails.Name
			}));
			foreach (KeyValuePair<CultureInfo, string> keyValuePair in this.localizedNames)
			{
				if (!this.defaultDetails.Culture.Equals(keyValuePair.Key))
				{
					list.Add(new XElement(XmlProcessingUtils.GetMceNsQualifiedNodeName("Name"), new object[]
					{
						new XAttribute("langcode", keyValuePair.Key.Name),
						keyValuePair.Value
					}));
				}
			}
			list.Add(new XElement(XmlProcessingUtils.GetMceNsQualifiedNodeName("Description"), new object[]
			{
				new XAttribute("default", "true"),
				new XAttribute("langcode", this.defaultDetails.Culture.Name),
				this.defaultDetails.Description
			}));
			foreach (KeyValuePair<CultureInfo, string> keyValuePair2 in this.localizedDescriptions)
			{
				if (!this.defaultDetails.Culture.Equals(keyValuePair2.Key))
				{
					list.Add(new XElement(XmlProcessingUtils.GetMceNsQualifiedNodeName("Description"), new object[]
					{
						new XAttribute("langcode", keyValuePair2.Key.Name),
						keyValuePair2.Value
					}));
				}
			}
			return new XElement(XmlProcessingUtils.GetMceNsQualifiedNodeName("Resource"), new object[]
			{
				new XAttribute("idRef", ((DataClassificationObjectId)this.Identity).Name),
				list
			});
		}

		private DataClassificationPresentationObject()
		{
		}

		private static void SetLocalizedResource(Dictionary<CultureInfo, string> resourceDictionary, CultureInfo locale, string value)
		{
			ArgumentValidator.ThrowIfNull("resourceDictionary", resourceDictionary);
			ArgumentValidator.ThrowIfNull("locale", locale);
			if (string.IsNullOrEmpty(value))
			{
				if (resourceDictionary.ContainsKey(locale))
				{
					resourceDictionary.Remove(locale);
					return;
				}
			}
			else
			{
				if (resourceDictionary.ContainsKey(locale))
				{
					resourceDictionary[locale] = value;
					return;
				}
				resourceDictionary.Add(locale, value);
			}
		}

		private static ClassificationTypeEnum Parse(string ruleElementName)
		{
			ExAssert.RetailAssert(!string.IsNullOrWhiteSpace(ruleElementName), "The rule element name being parsed into ClassificationTypeEnum cannot be null nor consists of white-spaces only");
			if (ruleElementName.Equals("Entity", StringComparison.Ordinal))
			{
				return ClassificationTypeEnum.Entity;
			}
			if (ruleElementName.Equals("Affinity", StringComparison.Ordinal))
			{
				return ClassificationTypeEnum.Affinity;
			}
			ExAssert.RetailAssert(false, "Invalid element name \"{0}\" being parsed into ClassificationTypeEnum", new object[]
			{
				ruleElementName
			});
			throw new ArgumentException(string.Empty, "ruleElementName");
		}

		private static DataClassificationObjectId CreateDataClassificationIdentifier(string ruleIdentifier, ClassificationRuleCollectionPresentationObject rulePackPresentationObject)
		{
			if (rulePackPresentationObject != null && rulePackPresentationObject.Identity != null)
			{
				string text = rulePackPresentationObject.Identity.ToString();
				int num = text.LastIndexOf(ClassificationDefinitionConstants.HierarchicalIdentitySeparatorChar);
				if (num != -1)
				{
					string organizationHierarchy = text.Substring(0, num);
					return new DataClassificationObjectId(organizationHierarchy, ruleIdentifier);
				}
			}
			return new DataClassificationObjectId(ruleIdentifier);
		}

		internal static DataClassificationPresentationObject Create(ClassificationRuleCollectionPresentationObject rulePackPresentationObject)
		{
			return new DataClassificationPresentationObject
			{
				defaultDetails = new DataClassificationLocalizableDetails(),
				localizedNames = new Dictionary<CultureInfo, string>(),
				localizedDescriptions = new Dictionary<CultureInfo, string>(),
				ClassificationRuleCollection = rulePackPresentationObject,
				Identity = DataClassificationPresentationObject.CreateDataClassificationIdentifier(Guid.NewGuid().ToString(), rulePackPresentationObject),
				ClassificationType = (rulePackPresentationObject.IsFingerprintRuleCollection ? ClassificationTypeEnum.Fingerprint : ClassificationTypeEnum.Affinity),
				RecommendedConfidence = new uint?(75U),
				MinEngineVersion = ClassificationDefinitionConstants.TextProcessorTypeToVersions[TextProcessorType.Fingerprint],
				IsDirty = true
			};
		}

		internal static DataClassificationPresentationObject Create(string ruleIdentifier, XElement ruleElement, XElement resourceElement, ClassificationRuleCollectionPresentationObject rulePackPresentationObject)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("ruleIdentifier", ruleIdentifier);
			ArgumentValidator.ThrowIfNull("ruleElement", ruleElement);
			ArgumentValidator.ThrowIfNull("resourceElement", resourceElement);
			return DataClassificationPresentationObject.Create(ruleIdentifier, XmlProcessingUtils.ReadDefaultRuleMetadata(resourceElement), ruleElement, resourceElement, rulePackPresentationObject);
		}

		internal static DataClassificationPresentationObject Create(string ruleIdentifier, DataClassificationLocalizableDetails defaultRuleDetails, XElement ruleElement, XElement resourceElement, ClassificationRuleCollectionPresentationObject rulePackPresentationObject)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("ruleIdentifier", ruleIdentifier);
			ArgumentValidator.ThrowIfNull("defaultRuleDetails", defaultRuleDetails);
			ArgumentValidator.ThrowIfNull("ruleElement", ruleElement);
			ArgumentValidator.ThrowIfNull("resourceElement", resourceElement);
			ArgumentValidator.ThrowIfNull("rulePackPresentationObject", rulePackPresentationObject);
			MultiValuedProperty<Fingerprint> multiValuedProperty = null;
			if (rulePackPresentationObject.IsFingerprintRuleCollection && ruleElement.Document != null)
			{
				multiValuedProperty = XmlProcessingUtils.ReadAllReferredFingerprints(ruleElement);
			}
			return new DataClassificationPresentationObject
			{
				defaultDetails = defaultRuleDetails,
				localizedNames = XmlProcessingUtils.ReadAllRuleNames(resourceElement),
				localizedDescriptions = XmlProcessingUtils.ReadAllRuleDescriptions(resourceElement),
				fingerprints = multiValuedProperty,
				Identity = DataClassificationPresentationObject.CreateDataClassificationIdentifier(ruleIdentifier, rulePackPresentationObject),
				ClassificationType = (rulePackPresentationObject.IsFingerprintRuleCollection ? ClassificationTypeEnum.Fingerprint : DataClassificationPresentationObject.Parse(ruleElement.Name.LocalName)),
				ClassificationRuleCollection = rulePackPresentationObject,
				RecommendedConfidence = XmlProcessingUtils.ReadRuleRecommendedConfidence(ruleElement),
				MinEngineVersion = XmlProcessingUtils.GetRulePackElementVersion(ruleElement)
			};
		}

		internal void Save(XDocument rulePackXDoc)
		{
			ArgumentValidator.ThrowIfNull("rulePackXDoc", rulePackXDoc);
			if (this.Fingerprints == null || this.Fingerprints.Count <= 0)
			{
				throw new DataClassificationFingerprintsMissingException(this.Name);
			}
			if (this.Fingerprints.Count((Fingerprint fingerprint) => string.IsNullOrEmpty(fingerprint.Description)) > 0)
			{
				throw new DataClassificationFingerprintsDescriptionMissingException(this.Name);
			}
			if (this.Fingerprints.Distinct(Fingerprint.Comparer).Count<Fingerprint>() != this.Fingerprints.Count)
			{
				throw new DataClassificationFingerprintsDuplicatedException(this.Name);
			}
			DataClassificationObjectId dataClassificationObjectId = this.Identity as DataClassificationObjectId;
			foreach (Fingerprint fingerprint2 in this.Fingerprints)
			{
				if (string.IsNullOrEmpty(fingerprint2.Identity))
				{
					fingerprint2.Identity = XmlProcessingUtils.AddFingerprintTextProcessor(rulePackXDoc, fingerprint2);
				}
			}
			XmlProcessingUtils.AddDataClassification(rulePackXDoc, dataClassificationObjectId.Name, this.MinEngineVersion.ToString(), this.GetRuleXElement());
			XmlProcessingUtils.AddLocalizedResource(rulePackXDoc, dataClassificationObjectId.Name, this.GetResourceXElement());
		}

		bool IConfigurable.IsValid
		{
			get
			{
				return true;
			}
		}

		ObjectState IConfigurable.ObjectState
		{
			get
			{
				return ObjectState.Unchanged;
			}
		}

		ValidationError[] IConfigurable.Validate()
		{
			return ValidationError.None;
		}

		void IConfigurable.CopyChangesFrom(IConfigurable source)
		{
		}

		void IConfigurable.ResetChangeTracking()
		{
		}

		private Dictionary<CultureInfo, string> localizedNames;

		private Dictionary<CultureInfo, string> localizedDescriptions;

		private DataClassificationLocalizableDetails defaultDetails;

		private MultiValuedProperty<Fingerprint> fingerprints;
	}
}
