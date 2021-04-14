using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	[Serializable]
	public class ClassificationRuleCollectionPresentationObject : RulePresentationObjectBase
	{
		public string RuleCollectionName
		{
			get
			{
				return this.defaultDetails.Name;
			}
		}

		public string LocalizedName
		{
			get
			{
				return ClassificationDefinitionUtils.GetMatchingLocalizedInfo<ClassificationRuleCollectionLocalizableDetails>(this.localizableDetails, this.defaultDetails).Name;
			}
		}

		public string Description
		{
			get
			{
				return ClassificationDefinitionUtils.GetMatchingLocalizedInfo<ClassificationRuleCollectionLocalizableDetails>(this.localizableDetails, this.defaultDetails).Description;
			}
		}

		public string Publisher
		{
			get
			{
				return ClassificationDefinitionUtils.GetMatchingLocalizedInfo<ClassificationRuleCollectionLocalizableDetails>(this.localizableDetails, this.defaultDetails).PublisherName;
			}
		}

		public Version Version { get; private set; }

		public bool IsEncrypted { get; private set; }

		public bool IsFingerprintRuleCollection { get; private set; }

		private protected TransportRule StoredRuleCollection { protected get; private set; }

		private new Guid Guid
		{
			get
			{
				return base.Guid;
			}
		}

		private new Guid ImmutableId
		{
			get
			{
				return base.ImmutableId;
			}
		}

		internal CultureInfo DefaultCulture
		{
			get
			{
				return this.defaultDetails.Culture;
			}
		}

		protected ClassificationRuleCollectionPresentationObject(TransportRule persistedRule) : base(persistedRule)
		{
			this.StoredRuleCollection = persistedRule;
		}

		protected virtual void Initialize()
		{
			ExAssert.RetailAssert(this.StoredRuleCollection != null, "The stored transport rule instance for classification rule collection presentation object must not be null");
			XDocument ruleCollectionDocumentFromTransportRule = ClassificationDefinitionUtils.GetRuleCollectionDocumentFromTransportRule(this.StoredRuleCollection);
			this.Initialize(ruleCollectionDocumentFromTransportRule);
		}

		protected virtual void Initialize(XDocument rulePackXDoc)
		{
			Version rulePackVersion = XmlProcessingUtils.GetRulePackVersion(rulePackXDoc);
			XElement rulePackageMetadataElement = XmlProcessingUtils.GetRulePackageMetadataElement(rulePackXDoc);
			bool isEncrypted = RulePackageDecrypter.IsRulePackageEncrypted(rulePackXDoc);
			this.Initialize(rulePackVersion, rulePackageMetadataElement, isEncrypted, XmlProcessingUtils.IsFingerprintRuleCollection(rulePackXDoc));
		}

		protected virtual void Initialize(Version rulePackVersion, XElement rulePackageDetailsElement, bool isEncrypted, bool isFingerprintRuleCollection)
		{
			this.Version = rulePackVersion;
			this.defaultDetails = XmlProcessingUtils.ReadDefaultRulePackageMetadata(rulePackageDetailsElement);
			this.localizableDetails = XmlProcessingUtils.ReadAllRulePackageMetadata(rulePackageDetailsElement);
			this.IsEncrypted = isEncrypted;
			this.IsFingerprintRuleCollection = isFingerprintRuleCollection;
		}

		internal static ClassificationRuleCollectionPresentationObject Create(TransportRule transportRule, Version rulePackageVersion, XElement rulePackageDetailsElement, bool isEncrypted)
		{
			if (transportRule == null)
			{
				throw new ArgumentNullException("transportRule");
			}
			if (null == rulePackageVersion)
			{
				throw new ArgumentNullException("rulePackageVersion");
			}
			ClassificationRuleCollectionPresentationObject classificationRuleCollectionPresentationObject = new ClassificationRuleCollectionPresentationObject(transportRule);
			classificationRuleCollectionPresentationObject.Initialize(rulePackageVersion, rulePackageDetailsElement, isEncrypted, XmlProcessingUtils.IsFingerprintRuleCollection(rulePackageDetailsElement.Document));
			return classificationRuleCollectionPresentationObject;
		}

		internal static ClassificationRuleCollectionPresentationObject Create(TransportRule transportRule, XDocument rulePackXDoc)
		{
			if (transportRule == null)
			{
				throw new ArgumentNullException("transportRule");
			}
			ClassificationRuleCollectionPresentationObject classificationRuleCollectionPresentationObject = new ClassificationRuleCollectionPresentationObject(transportRule);
			classificationRuleCollectionPresentationObject.Initialize(rulePackXDoc);
			return classificationRuleCollectionPresentationObject;
		}

		internal static ClassificationRuleCollectionPresentationObject Create(TransportRule transportRule)
		{
			if (transportRule == null)
			{
				throw new ArgumentNullException("transportRule");
			}
			ClassificationRuleCollectionPresentationObject classificationRuleCollectionPresentationObject = new ClassificationRuleCollectionPresentationObject(transportRule);
			classificationRuleCollectionPresentationObject.Initialize();
			return classificationRuleCollectionPresentationObject;
		}

		public override ValidationError[] Validate()
		{
			return ValidationError.None;
		}

		public override string ToString()
		{
			return this.RuleCollectionName;
		}

		private Dictionary<CultureInfo, ClassificationRuleCollectionLocalizableDetails> localizableDetails;

		private ClassificationRuleCollectionLocalizableDetails defaultDetails;
	}
}
