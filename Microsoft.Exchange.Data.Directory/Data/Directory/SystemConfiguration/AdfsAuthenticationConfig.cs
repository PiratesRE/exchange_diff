using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public class AdfsAuthenticationConfig
	{
		public AdfsAuthenticationConfig(string encodedRawConfig)
		{
			this.encodedRawConfig = encodedRawConfig;
		}

		internal static bool Validate(string encodedRawConfig)
		{
			AdfsAuthenticationConfig adfsAuthenticationConfig = new AdfsAuthenticationConfig(encodedRawConfig);
			List<ValidationError> list = new List<ValidationError>();
			adfsAuthenticationConfig.Validate(list);
			return list.Count == 0;
		}

		internal static string Encode(string inputString)
		{
			string result = null;
			if (inputString != null)
			{
				byte[] bytes = Encoding.UTF8.GetBytes(inputString);
				result = Convert.ToBase64String(bytes);
			}
			return result;
		}

		internal static bool TryDecode(string inputString, out string outputString)
		{
			outputString = null;
			bool result = false;
			if (inputString != null)
			{
				try
				{
					byte[] bytes = Convert.FromBase64String(inputString);
					outputString = Encoding.UTF8.GetString(bytes);
					result = true;
				}
				catch (FormatException)
				{
				}
			}
			return result;
		}

		private static bool StringParser(string inputValue, out string outputValue)
		{
			outputValue = inputValue;
			return true;
		}

		private static bool UriParser(string inputValue, out Uri uri)
		{
			return Uri.TryCreate(inputValue, UriKind.Absolute, out uri);
		}

		private static XmlNode GetOrAppendNode(XmlNode parentNode, bool isAttribute, string name, string value = null)
		{
			IEnumerable enumerable;
			if (!isAttribute)
			{
				IEnumerable childNodes = parentNode.ChildNodes;
				enumerable = childNodes;
			}
			else
			{
				enumerable = parentNode.Attributes;
			}
			IEnumerable enumerable2 = enumerable;
			XmlNode xmlNode = null;
			using (enumerable2 as IDisposable)
			{
				xmlNode = AdfsAuthenticationConfig.SearchNodeByName(enumerable2, name);
			}
			if (xmlNode == null)
			{
				if (isAttribute)
				{
					xmlNode = parentNode.OwnerDocument.CreateAttribute(name);
					parentNode.Attributes.Append((XmlAttribute)xmlNode);
				}
				else
				{
					xmlNode = parentNode.OwnerDocument.CreateElement(name);
					parentNode.AppendChild((XmlElement)xmlNode);
				}
				if (isAttribute && value != null)
				{
					xmlNode.Value = value;
				}
			}
			return xmlNode;
		}

		private static XmlNode SearchNodeByName(IEnumerable list, string name)
		{
			XmlNode result = null;
			foreach (object obj in list)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode != null && xmlNode.Name == name)
				{
					result = xmlNode;
					break;
				}
			}
			return result;
		}

		internal void Validate(List<ValidationError> errors)
		{
			if (string.IsNullOrEmpty(this.encodedRawConfig))
			{
				return;
			}
			if (this.EnsureXmlObject(false))
			{
				this.ValidateNodeList<Uri>(errors, "/service/audienceUris/add/@value", new AdfsAuthenticationConfig.TryParseValueDelegate<Uri>(AdfsAuthenticationConfig.UriParser), OrganizationSchema.AdfsAudienceUris, DirectoryStrings.ErrorAdfsAudienceUris, (string x) => DirectoryStrings.ErrorAdfsAudienceUriFormat(x), (string x) => DirectoryStrings.ErrorAdfsAudienceUriDup(x));
				this.ValidateNodeList<string>(errors, "/service/issuerNameRegistry/trustedIssuers/add/@thumbprint", delegate(string input, out string output)
				{
					output = input;
					return !string.IsNullOrEmpty(input);
				}, OrganizationSchema.AdfsSignCertificateThumbprints, DirectoryStrings.ErrorAdfsTrustedIssuers, (string x) => DirectoryStrings.ErrorAdfsTrustedIssuerFormat(x), null);
				XmlNode xmlNode = this.configXmlDoc.SelectSingleNode("/service/federatedAuthentication/wsFederation/@issuer");
				Uri uri;
				if (xmlNode == null || !Uri.TryCreate(xmlNode.Value, UriKind.Absolute, out uri))
				{
					errors.Add(new PropertyValidationError(DirectoryStrings.ErrorAdfsIssuer, OrganizationSchema.AdfsIssuer, null));
					return;
				}
			}
			else
			{
				errors.Add(new PropertyValidationError(DirectoryStrings.ErrorAdfsConfigFormat, OrganizationSchema.AdfsAuthenticationRawConfiguration, null));
			}
		}

		public string ConfigXml
		{
			get
			{
				string outerXml;
				if (this.configXmlDoc == null)
				{
					AdfsAuthenticationConfig.TryDecode(this.encodedRawConfig, out outerXml);
				}
				else
				{
					outerXml = this.configXmlDoc.OuterXml;
				}
				return outerXml;
			}
		}

		public string EncodedConfig
		{
			get
			{
				return AdfsAuthenticationConfig.Encode(this.ConfigXml);
			}
		}

		public Uri Issuer
		{
			get
			{
				return this.GetValue<Uri>("/service/federatedAuthentication/wsFederation/@issuer", new AdfsAuthenticationConfig.TryParseValueDelegate<Uri>(AdfsAuthenticationConfig.UriParser));
			}
			set
			{
				this.SetValue("/service/federatedAuthentication/wsFederation/@issuer", (value == null) ? string.Empty : value.ToString());
			}
		}

		public MultiValuedProperty<Uri> AudienceUris
		{
			get
			{
				return this.GetValues<Uri>("/service/audienceUris/add/@value", new AdfsAuthenticationConfig.TryParseValueDelegate<Uri>(AdfsAuthenticationConfig.UriParser));
			}
			set
			{
				if (value != null && this.EnsureXmlObject(true))
				{
					XmlElement nodePrototype = this.configXmlDoc.CreateElement("add");
					this.SetValues<Uri>("/service/audienceUris", value, nodePrototype, delegate(XmlNode newNode, Uri uri)
					{
						AdfsAuthenticationConfig.GetOrAppendNode(newNode, true, "value", uri.ToString());
					});
				}
			}
		}

		public MultiValuedProperty<string> SignCertificateThumbprints
		{
			get
			{
				return this.GetValues<string>("/service/issuerNameRegistry/trustedIssuers/add/@thumbprint", new AdfsAuthenticationConfig.TryParseValueDelegate<string>(AdfsAuthenticationConfig.StringParser));
			}
			set
			{
				if (value != null && this.EnsureXmlObject(true))
				{
					XmlElement nodePrototype = this.configXmlDoc.CreateElement("add");
					this.SetValues<string>("/service/issuerNameRegistry/trustedIssuers", value, nodePrototype, delegate(XmlNode newNode, string thumbprint)
					{
						AdfsAuthenticationConfig.GetOrAppendNode(newNode, true, "thumbprint", thumbprint);
						AdfsAuthenticationConfig.GetOrAppendNode(newNode, true, "name", "Adfs");
					});
				}
			}
		}

		public string EncryptCertificateThumbprint
		{
			get
			{
				return this.GetValue<string>("/service/serviceCertificate/certificateReference/@findValue", new AdfsAuthenticationConfig.TryParseValueDelegate<string>(AdfsAuthenticationConfig.StringParser));
			}
			set
			{
				if (this.EnsureXmlObject(true))
				{
					if (string.IsNullOrEmpty(value))
					{
						XmlNode xmlNode = this.configXmlDoc.SelectSingleNode("/service/serviceCertificate");
						if (xmlNode != null)
						{
							xmlNode.ParentNode.RemoveChild(xmlNode);
							return;
						}
					}
					else
					{
						XmlNode xmlNode2 = this.configXmlDoc.SelectSingleNode("service");
						if (xmlNode2 != null)
						{
							XmlNode orAppendNode = AdfsAuthenticationConfig.GetOrAppendNode(xmlNode2, false, "serviceCertificate", null);
							XmlNode orAppendNode2 = AdfsAuthenticationConfig.GetOrAppendNode(orAppendNode, false, "certificateReference", null);
							AdfsAuthenticationConfig.GetOrAppendNode(orAppendNode2, true, "x509FindType", "FindByThumbprint");
							AdfsAuthenticationConfig.GetOrAppendNode(orAppendNode2, true, "findValue", null);
							AdfsAuthenticationConfig.GetOrAppendNode(orAppendNode2, true, "storeLocation", "LocalMachine");
							AdfsAuthenticationConfig.GetOrAppendNode(orAppendNode2, true, "storeName", "My");
							this.SetValue("/service/serviceCertificate/certificateReference/@findValue", value ?? string.Empty);
						}
					}
				}
			}
		}

		private void ValidateNodeList<T>(List<ValidationError> errors, string xPath, AdfsAuthenticationConfig.TryParseValueDelegate<T> parser, ADPropertyDefinition propertyDefinition, LocalizedString nodeCountErrorString, AdfsAuthenticationConfig.NodeValueErrorDelegate nodeValueError, AdfsAuthenticationConfig.NodeValueErrorDelegate nodeValueDupError)
		{
			using (XmlNodeList xmlNodeList = this.configXmlDoc.SelectNodes(xPath))
			{
				if (xmlNodeList.Count == 0)
				{
					errors.Add(new PropertyValidationError(nodeCountErrorString, propertyDefinition, null));
				}
				else
				{
					MultiValuedProperty<T> multiValuedProperty = new MultiValuedProperty<T>();
					foreach (object obj in xmlNodeList)
					{
						XmlNode xmlNode = (XmlNode)obj;
						T item;
						if (!parser(xmlNode.Value, out item))
						{
							errors.Add(new PropertyValidationError(nodeValueError(xmlNode.Value), propertyDefinition, null));
						}
						else if (nodeValueDupError != null)
						{
							try
							{
								multiValuedProperty.Add(item);
							}
							catch (InvalidOperationException)
							{
								errors.Add(new PropertyValidationError(nodeValueDupError(xmlNode.Value), propertyDefinition, null));
							}
						}
					}
				}
			}
		}

		private MultiValuedProperty<T> GetValues<T>(string xPath, AdfsAuthenticationConfig.TryParseValueDelegate<T> parser)
		{
			MultiValuedProperty<T> multiValuedProperty = new MultiValuedProperty<T>();
			if (this.EnsureXmlObject(false))
			{
				using (XmlNodeList xmlNodeList = this.configXmlDoc.SelectNodes(xPath))
				{
					foreach (object obj in xmlNodeList)
					{
						XmlNode xmlNode = (XmlNode)obj;
						string value = xmlNode.Value;
						T item;
						if (parser(value, out item))
						{
							try
							{
								multiValuedProperty.Add(item);
							}
							catch (InvalidOperationException)
							{
							}
						}
					}
				}
			}
			return multiValuedProperty;
		}

		private T GetValue<T>(string xPath, AdfsAuthenticationConfig.TryParseValueDelegate<T> parser)
		{
			MultiValuedProperty<T> values = this.GetValues<T>(xPath, parser);
			if (values.Count <= 0)
			{
				return default(T);
			}
			return values[0];
		}

		private void SetValue(string xPath, string value)
		{
			if (this.EnsureXmlObject(true))
			{
				XmlNode xmlNode = this.configXmlDoc.SelectSingleNode(xPath);
				if (xmlNode != null)
				{
					xmlNode.Value = value;
				}
			}
		}

		private void SetValues<T>(string xPath, MultiValuedProperty<T> values, XmlNode nodePrototype, AdfsAuthenticationConfig.AddXmlNodeDelegate<T> processor)
		{
			if (this.EnsureXmlObject(true))
			{
				XmlNode xmlNode = this.configXmlDoc.SelectSingleNode(xPath);
				if (xmlNode != null)
				{
					xmlNode.RemoveAll();
					foreach (T value in values)
					{
						XmlNode xmlNode2 = nodePrototype.Clone();
						processor(xmlNode2, value);
						xmlNode.AppendChild(xmlNode2);
					}
				}
			}
		}

		private bool EnsureXmlObject(bool forceCreateDefaultObject)
		{
			bool result = false;
			string text = null;
			if (this.configXmlDoc != null)
			{
				result = true;
			}
			else
			{
				if (string.IsNullOrEmpty(this.encodedRawConfig) || !AdfsAuthenticationConfig.TryDecode(this.encodedRawConfig, out text))
				{
					if (!forceCreateDefaultObject)
					{
						return result;
					}
				}
				try
				{
					this.configXmlDoc = new SafeXmlDocument();
					this.configXmlDoc.LoadXml(string.IsNullOrEmpty(text) ? "<service>  <federatedAuthentication>      <wsFederation passiveRedirectEnabled=\"true\" issuer=\"\" realm=\"https://fakerealm/\" requireHttps=\"true\" />      <cookieHandler requireSsl=\"true\" path=\"/\" />  </federatedAuthentication>  <certificateValidation certificateValidationMode=\"PeerOrChainTrust\" />  <audienceUris></audienceUris>  <issuerNameRegistry type=\"Microsoft.IdentityModel.Tokens.ConfigurationBasedIssuerNameRegistry, Microsoft.IdentityModel, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\">      <trustedIssuers></trustedIssuers>  </issuerNameRegistry></service>" : text);
					result = true;
				}
				catch (XmlException)
				{
					this.configXmlDoc = null;
				}
			}
			return result;
		}

		private const string IssuerXPath = "/service/federatedAuthentication/wsFederation/@issuer";

		private const string AudienceUrisXPath = "/service/audienceUris/add/@value";

		private const string AudienceUrisParentXPath = "/service/audienceUris";

		private const string EncryptCertXPath = "/service/serviceCertificate/certificateReference/@findValue";

		private const string ServiceCertificateXPath = "/service/serviceCertificate";

		private const string TrustedIssuersXPath = "/service/issuerNameRegistry/trustedIssuers/add/@thumbprint";

		private const string TrustedIssuersParentXPath = "/service/issuerNameRegistry/trustedIssuers";

		private const string WsFederationIssuerXPath = "/service/federatedAuthentication/wsFederation/@issuer";

		private const string DefaultTemplate = "<service>  <federatedAuthentication>      <wsFederation passiveRedirectEnabled=\"true\" issuer=\"\" realm=\"https://fakerealm/\" requireHttps=\"true\" />      <cookieHandler requireSsl=\"true\" path=\"/\" />  </federatedAuthentication>  <certificateValidation certificateValidationMode=\"PeerOrChainTrust\" />  <audienceUris></audienceUris>  <issuerNameRegistry type=\"Microsoft.IdentityModel.Tokens.ConfigurationBasedIssuerNameRegistry, Microsoft.IdentityModel, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\">      <trustedIssuers></trustedIssuers>  </issuerNameRegistry></service>";

		private readonly string encodedRawConfig;

		private XmlDocument configXmlDoc;

		private delegate bool TryParseValueDelegate<T>(string inputValue, out T outputValue);

		private delegate void AddXmlNodeDelegate<T>(XmlNode newNode, T value);

		private delegate LocalizedString NodeValueErrorDelegate(string value);
	}
}
