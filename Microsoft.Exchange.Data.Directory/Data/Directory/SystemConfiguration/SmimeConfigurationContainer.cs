using System;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public class SmimeConfigurationContainer : ADConfigurationObject, ISmimeSettingsProvider
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return SmimeConfigurationContainer.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return "msExchContainer";
			}
		}

		internal override ADObjectId ParentPath
		{
			get
			{
				return SmimeConfigurationContainer.parentPath;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, this.MostDerivedObjectClass);
			}
		}

		private string SmimeConfigurationXML
		{
			get
			{
				return (string)this[SmimeConfigurationContainerSchema.SmimeConfigurationXML];
			}
			set
			{
				this[SmimeConfigurationContainerSchema.SmimeConfigurationXML] = value;
			}
		}

		internal override void Initialize()
		{
			this.LoadSettings();
		}

		internal static ADObjectId GetWellKnownParentLocation(ADObjectId orgContainerId)
		{
			ADObjectId defaultsRoot = SmimeConfigurationContainer.DefaultsRoot;
			return orgContainerId.GetDescendantId(defaultsRoot);
		}

		public void SaveSettings()
		{
			this.PackSmimeSettings();
		}

		public bool OWACheckCRLOnSend
		{
			get
			{
				string valueForTag = this.GetValueForTag("OWACheckCRLOnSend");
				return !string.IsNullOrWhiteSpace(valueForTag) && bool.Parse(valueForTag);
			}
			set
			{
				string text = value.ToString();
				if (text != this.GetValueForTag("OWACheckCRLOnSend"))
				{
					this.SetValueForTag("OWACheckCRLOnSend", text);
				}
			}
		}

		public uint OWADLExpansionTimeout
		{
			get
			{
				string valueForTag = this.GetValueForTag("OWADLExpansionTimeout");
				if (string.IsNullOrWhiteSpace(valueForTag))
				{
					return 60000U;
				}
				return uint.Parse(valueForTag);
			}
			set
			{
				string text = value.ToString();
				if (text != this.GetValueForTag("OWADLExpansionTimeout"))
				{
					this.SetValueForTag("OWADLExpansionTimeout", text);
				}
			}
		}

		public bool OWAUseSecondaryProxiesWhenFindingCertificates
		{
			get
			{
				string valueForTag = this.GetValueForTag("OWAUseSecondaryProxiesWhenFindingCertificates");
				return string.IsNullOrWhiteSpace(valueForTag) || bool.Parse(valueForTag);
			}
			set
			{
				string text = value.ToString();
				if (text != this.GetValueForTag("OWAUseSecondaryProxiesWhenFindingCertificates"))
				{
					this.SetValueForTag("OWAUseSecondaryProxiesWhenFindingCertificates", text);
				}
			}
		}

		public uint OWACRLConnectionTimeout
		{
			get
			{
				string valueForTag = this.GetValueForTag("OWACRLConnectionTimeout");
				if (string.IsNullOrWhiteSpace(valueForTag))
				{
					return 60000U;
				}
				return uint.Parse(valueForTag);
			}
			set
			{
				string text = value.ToString();
				if (text != this.GetValueForTag("OWACRLConnectionTimeout"))
				{
					this.SetValueForTag("OWACRLConnectionTimeout", text);
				}
			}
		}

		public uint OWACRLRetrievalTimeout
		{
			get
			{
				string valueForTag = this.GetValueForTag("OWACRLRetrievalTimeout");
				if (string.IsNullOrWhiteSpace(valueForTag))
				{
					return 10000U;
				}
				return uint.Parse(valueForTag);
			}
			set
			{
				string text = value.ToString();
				if (text != this.GetValueForTag("OWACRLRetrievalTimeout"))
				{
					this.SetValueForTag("OWACRLRetrievalTimeout", text);
				}
			}
		}

		public bool OWADisableCRLCheck
		{
			get
			{
				string valueForTag = this.GetValueForTag("OWADisableCRLCheck");
				return !string.IsNullOrWhiteSpace(valueForTag) && bool.Parse(valueForTag);
			}
			set
			{
				string text = value.ToString();
				if (text != this.GetValueForTag("OWADisableCRLCheck"))
				{
					this.SetValueForTag("OWADisableCRLCheck", text);
				}
			}
		}

		public bool OWAAlwaysSign
		{
			get
			{
				string valueForTag = this.GetValueForTag("OWAAlwaysSign");
				return !string.IsNullOrWhiteSpace(valueForTag) && bool.Parse(valueForTag);
			}
			set
			{
				string text = value.ToString();
				if (text != this.GetValueForTag("OWAAlwaysSign"))
				{
					this.SetValueForTag("OWAAlwaysSign", text);
				}
			}
		}

		public bool OWAAlwaysEncrypt
		{
			get
			{
				string valueForTag = this.GetValueForTag("OWAAlwaysEncrypt");
				return !string.IsNullOrWhiteSpace(valueForTag) && bool.Parse(valueForTag);
			}
			set
			{
				string text = value.ToString();
				if (text != this.GetValueForTag("OWAAlwaysEncrypt"))
				{
					this.SetValueForTag("OWAAlwaysEncrypt", text);
				}
			}
		}

		public bool OWAClearSign
		{
			get
			{
				string valueForTag = this.GetValueForTag("OWAClearSign");
				return string.IsNullOrWhiteSpace(valueForTag) || bool.Parse(valueForTag);
			}
			set
			{
				string text = value.ToString();
				if (text != this.GetValueForTag("OWAClearSign"))
				{
					this.SetValueForTag("OWAClearSign", text);
				}
			}
		}

		public bool OWAIncludeCertificateChainWithoutRootCertificate
		{
			get
			{
				string valueForTag = this.GetValueForTag("OWAIncludeCertificateChainWithoutRootCertificate");
				return !string.IsNullOrWhiteSpace(valueForTag) && bool.Parse(valueForTag);
			}
			set
			{
				string text = value.ToString();
				if (text != this.GetValueForTag("OWAIncludeCertificateChainWithoutRootCertificate"))
				{
					this.SetValueForTag("OWAIncludeCertificateChainWithoutRootCertificate", text);
				}
			}
		}

		public bool OWAIncludeCertificateChainAndRootCertificate
		{
			get
			{
				string valueForTag = this.GetValueForTag("OWAIncludeCertificateChainAndRootCertificate");
				return !string.IsNullOrWhiteSpace(valueForTag) && bool.Parse(valueForTag);
			}
			set
			{
				string text = value.ToString();
				if (text != this.GetValueForTag("OWAIncludeCertificateChainAndRootCertificate"))
				{
					this.SetValueForTag("OWAIncludeCertificateChainAndRootCertificate", text);
				}
			}
		}

		public bool OWAEncryptTemporaryBuffers
		{
			get
			{
				string valueForTag = this.GetValueForTag("OWAEncryptTemporaryBuffers");
				return string.IsNullOrWhiteSpace(valueForTag) || bool.Parse(valueForTag);
			}
			set
			{
				string text = value.ToString();
				if (text != this.GetValueForTag("OWAEncryptTemporaryBuffers"))
				{
					this.SetValueForTag("OWAEncryptTemporaryBuffers", text);
				}
			}
		}

		public bool OWASignedEmailCertificateInclusion
		{
			get
			{
				string valueForTag = this.GetValueForTag("OWASignedEmailCertificateInclusion");
				return string.IsNullOrWhiteSpace(valueForTag) || bool.Parse(valueForTag);
			}
			set
			{
				string text = value.ToString();
				if (text != this.GetValueForTag("OWASignedEmailCertificateInclusion"))
				{
					this.SetValueForTag("OWASignedEmailCertificateInclusion", text);
				}
			}
		}

		public uint OWABCCEncryptedEmailForking
		{
			get
			{
				string valueForTag = this.GetValueForTag("OWABCCEncryptedEmailForking");
				if (string.IsNullOrWhiteSpace(valueForTag))
				{
					return 0U;
				}
				return uint.Parse(valueForTag);
			}
			set
			{
				string text = value.ToString();
				if (text != this.GetValueForTag("OWABCCEncryptedEmailForking"))
				{
					this.SetValueForTag("OWABCCEncryptedEmailForking", text);
				}
			}
		}

		public bool OWAIncludeSMIMECapabilitiesInMessage
		{
			get
			{
				string valueForTag = this.GetValueForTag("OWAIncludeSMIMECapabilitiesInMessage");
				return !string.IsNullOrWhiteSpace(valueForTag) && bool.Parse(valueForTag);
			}
			set
			{
				string text = value.ToString();
				if (text != this.GetValueForTag("OWAIncludeSMIMECapabilitiesInMessage"))
				{
					this.SetValueForTag("OWAIncludeSMIMECapabilitiesInMessage", text);
				}
			}
		}

		public bool OWACopyRecipientHeaders
		{
			get
			{
				string valueForTag = this.GetValueForTag("OWACopyRecipientHeaders");
				return !string.IsNullOrWhiteSpace(valueForTag) && bool.Parse(valueForTag);
			}
			set
			{
				string text = value.ToString();
				if (text != this.GetValueForTag("OWACopyRecipientHeaders"))
				{
					this.SetValueForTag("OWACopyRecipientHeaders", text);
				}
			}
		}

		public bool OWAOnlyUseSmartCard
		{
			get
			{
				string valueForTag = this.GetValueForTag("OWAOnlyUseSmartCard");
				return !string.IsNullOrWhiteSpace(valueForTag) && bool.Parse(valueForTag);
			}
			set
			{
				string text = value.ToString();
				if (text != this.GetValueForTag("OWAOnlyUseSmartCard"))
				{
					this.SetValueForTag("OWAOnlyUseSmartCard", text);
				}
			}
		}

		public bool OWATripleWrapSignedEncryptedMail
		{
			get
			{
				string valueForTag = this.GetValueForTag("OWATripleWrapSignedEncryptedMail");
				return !string.IsNullOrWhiteSpace(valueForTag) && bool.Parse(valueForTag);
			}
			set
			{
				string text = value.ToString();
				if (text != this.GetValueForTag("OWATripleWrapSignedEncryptedMail"))
				{
					this.SetValueForTag("OWATripleWrapSignedEncryptedMail", text);
				}
			}
		}

		public bool OWAUseKeyIdentifier
		{
			get
			{
				string valueForTag = this.GetValueForTag("OWAUseKeyIdentifier");
				return !string.IsNullOrWhiteSpace(valueForTag) && bool.Parse(valueForTag);
			}
			set
			{
				string text = value.ToString();
				if (text != this.GetValueForTag("OWAUseKeyIdentifier"))
				{
					this.SetValueForTag("OWAUseKeyIdentifier", text);
				}
			}
		}

		public string OWAEncryptionAlgorithms
		{
			get
			{
				string valueForTag = this.GetValueForTag("OWAEncryptionAlgorithms");
				if (string.IsNullOrWhiteSpace(valueForTag))
				{
					return "6610";
				}
				return valueForTag;
			}
			set
			{
				if (value != this.GetValueForTag("OWAEncryptionAlgorithms"))
				{
					this.SetValueForTag("OWAEncryptionAlgorithms", value);
				}
			}
		}

		public string OWASigningAlgorithms
		{
			get
			{
				string valueForTag = this.GetValueForTag("OWASigningAlgorithms");
				if (string.IsNullOrWhiteSpace(valueForTag))
				{
					return "8004";
				}
				return valueForTag;
			}
			set
			{
				if (value != this.GetValueForTag("OWASigningAlgorithms"))
				{
					this.SetValueForTag("OWASigningAlgorithms", value);
				}
			}
		}

		public bool OWAForceSMIMEClientUpgrade
		{
			get
			{
				string valueForTag = this.GetValueForTag("OWAForceSMIMEClientUpgrade");
				return string.IsNullOrWhiteSpace(valueForTag) || bool.Parse(valueForTag);
			}
			set
			{
				string text = value.ToString();
				if (text != this.GetValueForTag("OWAForceSMIMEClientUpgrade"))
				{
					this.SetValueForTag("OWAForceSMIMEClientUpgrade", text);
				}
			}
		}

		public string OWASenderCertificateAttributesToDisplay
		{
			get
			{
				string valueForTag = this.GetValueForTag("OWASenderCertificateAttributesToDisplay");
				if (string.IsNullOrWhiteSpace(valueForTag))
				{
					return "";
				}
				return valueForTag;
			}
			set
			{
				if (value != this.GetValueForTag("OWASenderCertificateAttributesToDisplay"))
				{
					this.SetValueForTag("OWASenderCertificateAttributesToDisplay", value);
				}
			}
		}

		public bool OWAAllowUserChoiceOfSigningCertificate
		{
			get
			{
				string valueForTag = this.GetValueForTag("OWAAllowUserChoiceOfSigningCertificate");
				return !string.IsNullOrWhiteSpace(valueForTag) && bool.Parse(valueForTag);
			}
			set
			{
				string text = value.ToString();
				if (text != this.GetValueForTag("OWAAllowUserChoiceOfSigningCertificate"))
				{
					this.SetValueForTag("OWAAllowUserChoiceOfSigningCertificate", text);
				}
			}
		}

		public byte[] SMIMECertificateIssuingCA
		{
			get
			{
				string valueForTag = this.GetValueForTag("SMIMECertificateIssuingCA");
				if (string.IsNullOrWhiteSpace(valueForTag))
				{
					return null;
				}
				return Convert.FromBase64String(valueForTag);
			}
			set
			{
				string valueForTag = this.GetValueForTag("SMIMECertificateIssuingCA");
				string text = string.Empty;
				if (value != null)
				{
					text = Convert.ToBase64String(value);
				}
				if (text != valueForTag)
				{
					this.SetValueForTag("SMIMECertificateIssuingCA", text);
				}
			}
		}

		public string SMIMECertificateIssuingCAFull()
		{
			string valueForTag = this.GetValueForTag("SMIMECertificateIssuingCA");
			if (string.IsNullOrWhiteSpace(valueForTag))
			{
				return "";
			}
			return valueForTag;
		}

		public DateTime? SMIMECertificatesExpiryDate
		{
			get
			{
				string valueForTag = this.GetValueForTag("SMIMECertificatesExpiryDate");
				if (string.IsNullOrWhiteSpace(valueForTag))
				{
					return this.SMIMECertificatesExpiryDateDefaultValue;
				}
				return new DateTime?(DateTime.ParseExact(valueForTag, "s", null));
			}
			set
			{
				if (value == null)
				{
					this.SetValueForTag("SMIMECertificatesExpiryDate", "");
					return;
				}
				string text = value.Value.ToString("s");
				if (text != this.GetValueForTag("SMIMECertificatesExpiryDate"))
				{
					this.SetValueForTag("SMIMECertificatesExpiryDate", text);
				}
			}
		}

		public string SMIMEExpiredCertificateThumbprint
		{
			get
			{
				string valueForTag = this.GetValueForTag("SMIMEExpiredCertificateThumbprint");
				if (string.IsNullOrWhiteSpace(valueForTag))
				{
					return "";
				}
				return valueForTag;
			}
			set
			{
				if (value != this.GetValueForTag("SMIMEExpiredCertificateThumbprint"))
				{
					this.SetValueForTag("SMIMEExpiredCertificateThumbprint", value);
				}
			}
		}

		private string GetValueForTag(string tag)
		{
			if (this.xmlDoc != null && this.xmlDoc.DocumentElement != null)
			{
				XmlElement documentElement = this.xmlDoc.DocumentElement;
				if (documentElement.HasChildNodes)
				{
					for (int i = 0; i < documentElement.ChildNodes.Count; i++)
					{
						if (documentElement.ChildNodes[i].Name == tag)
						{
							return ((XmlElement)documentElement.ChildNodes[i]).GetAttribute("Value");
						}
					}
				}
			}
			return null;
		}

		private void SetValueForTag(string tag, string value)
		{
			if (this.xmlDoc != null)
			{
				XmlElement xmlElement;
				if (this.xmlDoc.DocumentElement != null)
				{
					xmlElement = this.xmlDoc.DocumentElement;
				}
				else
				{
					xmlElement = this.xmlDoc.CreateElement("SMIMEConfiguration");
					this.xmlDoc.AppendChild(xmlElement);
				}
				XmlElement xmlElement2 = null;
				if (xmlElement.HasChildNodes)
				{
					for (int i = 0; i < xmlElement.ChildNodes.Count; i++)
					{
						if (string.Compare(xmlElement.ChildNodes[i].Name.ToLower(), tag.ToLower()) == 0)
						{
							xmlElement2 = (XmlElement)xmlElement.ChildNodes[i];
						}
					}
				}
				if (value == null)
				{
					if (xmlElement2 != null)
					{
						xmlElement.RemoveChild(xmlElement2);
						return;
					}
				}
				else
				{
					if (xmlElement2 == null)
					{
						xmlElement2 = this.xmlDoc.CreateElement(tag);
						xmlElement.AppendChild(xmlElement2);
					}
					xmlElement2.SetAttribute("Value", value);
				}
			}
		}

		private void LoadSettings()
		{
			if (this.xmlDoc == null)
			{
				string smimeConfigurationXML = this.SmimeConfigurationXML;
				this.xmlDoc = new SafeXmlDocument();
				if (string.IsNullOrEmpty(smimeConfigurationXML))
				{
					XmlElement newChild = this.xmlDoc.CreateElement("SMIMEConfiguration");
					this.xmlDoc.AppendChild(newChild);
					return;
				}
				this.xmlDoc.LoadXml(smimeConfigurationXML);
			}
		}

		private void PackSmimeSettings()
		{
			if (this.xmlDoc != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder))
				{
					this.xmlDoc.Save(xmlWriter);
					xmlWriter.Close();
				}
				this.SmimeConfigurationXML = stringBuilder.ToString();
			}
		}

		internal const string MostDerivedClass = "msExchContainer";

		private const string ValueAttribute = "Value";

		private const string SMimeConfigurationRootNodeName = "SMIMEConfiguration";

		public const string OWACheckCRLOnSendProperty = "OWACheckCRLOnSend";

		public const string OWADLExpansionTimeoutProperty = "OWADLExpansionTimeout";

		public const string OWAUseSecondaryProxiesWhenFindingCertificatesProperty = "OWAUseSecondaryProxiesWhenFindingCertificates";

		public const string OWACRLConnectionTimeoutProperty = "OWACRLConnectionTimeout";

		public const string OWACRLRetrievalTimeoutProperty = "OWACRLRetrievalTimeout";

		public const string OWADisableCRLCheckProperty = "OWADisableCRLCheck";

		public const string OWAAlwaysSignProperty = "OWAAlwaysSign";

		public const string OWAAlwaysEncryptProperty = "OWAAlwaysEncrypt";

		public const string OWAClearSignProperty = "OWAClearSign";

		public const string OWAIncludeCertificateChainWithoutRootCertificateProperty = "OWAIncludeCertificateChainWithoutRootCertificate";

		public const string OWAIncludeCertificateChainAndRootCertificateProperty = "OWAIncludeCertificateChainAndRootCertificate";

		public const string OWAEncryptTemporaryBuffersProperty = "OWAEncryptTemporaryBuffers";

		public const string OWASignedEmailCertificateInclusionProperty = "OWASignedEmailCertificateInclusion";

		public const string OWABCCEncryptedEmailForkingProperty = "OWABCCEncryptedEmailForking";

		public const string OWAIncludeSMIMECapabilitiesInMessageProperty = "OWAIncludeSMIMECapabilitiesInMessage";

		public const string OWACopyRecipientHeadersProperty = "OWACopyRecipientHeaders";

		public const string OWAOnlyUseSmartCardProperty = "OWAOnlyUseSmartCard";

		public const string OWATripleWrapSignedEncryptedMailProperty = "OWATripleWrapSignedEncryptedMail";

		public const string OWAUseKeyIdentifierProperty = "OWAUseKeyIdentifier";

		public const string OWAEncryptionAlgorithmsProperty = "OWAEncryptionAlgorithms";

		public const string OWASigningAlgorithmsProperty = "OWASigningAlgorithms";

		public const string OWAForceSMIMEClientUpgradeProperty = "OWAForceSMIMEClientUpgrade";

		public const string OWASenderCertificateAttributesToDisplayProperty = "OWASenderCertificateAttributesToDisplay";

		public const string OWAAllowUserChoiceOfSigningCertificateProperty = "OWAAllowUserChoiceOfSigningCertificate";

		public const string SMIMECertificateIssuingCAProperty = "SMIMECertificateIssuingCA";

		public const string SMIMECertificatesExpiryDateStringProperty = "SMIMECertificatesExpiryDate";

		public const string SMIMEExpiredCertificateThumbprintProperty = "SMIMEExpiredCertificateThumbprint";

		private const bool OWACheckCRLOnSendDefaultValue = false;

		private const int OWADLExpansionTimeoutDefaultValue = 60000;

		private const bool OWAUseSecondaryProxiesWhenFindingCertificatesDefaultValue = true;

		private const int OWACRLConnectionTimeoutDefaultValue = 60000;

		private const int OWACRLRetrievalTimeoutDefaultValue = 10000;

		private const bool OWADisableCRLCheckDefaultValue = false;

		private const bool OWAAlwaysSignDefaultValue = false;

		private const bool OWAAlwaysEncryptDefaultValue = false;

		private const bool OWAClearSignDefaultValue = true;

		private const bool OWAIncludeCertificateChainWithoutRootCertificateDefaultValue = false;

		private const bool OWAIncludeCertificateChainAndRootCertificateDefaultValue = false;

		private const bool OWAEncryptTemporaryBuffersDefaultValue = true;

		private const bool OWASignedEmailCertificateInclusionDefaultValue = true;

		private const int OWABCCEncryptedEmailForkingDefaultValue = 0;

		private const bool OWAIncludeSMIMECapabilitiesInMessageDefaultValue = false;

		private const bool OWACopyRecipientHeadersDefaultValue = false;

		private const bool OWAOnlyUseSmartCardDefaultValue = false;

		private const bool OWATripleWrapSignedEncryptedMailDefaultValue = false;

		private const bool OWAUseKeyIdentifierDefaultValue = false;

		private const string OWAEncryptionAlgorithmsDefaultValue = "6610";

		private const string OWASigningAlgorithmsDefaultValue = "8004";

		private const bool OWAForceSMIMEClientUpgradeDefaultValue = true;

		private const string OWASenderCertificateAttributesToDisplayDefaultValue = "";

		private const bool OWAAllowUserChoiceOfSigningCertificateDefaultValue = false;

		private const string SMIMECertificateIssuingCADefaultValue = "";

		private const string SMIMECertificatesExpiryDateStringDefaultValue = "";

		private const string SMIMEExpiredCertificateThumbprintDefaultValue = "";

		private static SmimeConfigurationContainerSchema schema = ObjectSchema.GetInstance<SmimeConfigurationContainerSchema>();

		public static ADObjectId DefaultsRoot = new ADObjectId("CN=Smime Configuration,CN=Global Settings");

		private static ADObjectId parentPath = new ADObjectId("CN=Global Settings");

		private readonly DateTime? SMIMECertificatesExpiryDateDefaultValue = null;

		private SafeXmlDocument xmlDoc;
	}
}
