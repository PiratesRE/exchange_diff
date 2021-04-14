using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.ApplicationLogic;

namespace Microsoft.Exchange.Data.ApplicationLogic.Extension
{
	public static class ExtensionDataHelper
	{
		internal static string FormatExtensionId(string extensionId)
		{
			if (string.IsNullOrWhiteSpace(extensionId))
			{
				return extensionId;
			}
			string text = extensionId.Replace("urn:uuid:", string.Empty).Trim(ExtensionDataHelper.TrimCharArray);
			return text.ToLowerInvariant();
		}

		public static string GetDeploymentId(string domain)
		{
			string text = string.Empty;
			try
			{
				ADSessionSettings adsessionSettings = ExtensionDataHelper.CreateRootOrgOrSingleTenantFromAcceptedDomainAutoDetect(domain);
				if (adsessionSettings != null)
				{
					IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, adsessionSettings, 80, "GetDeploymentId", "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\extension\\ExtensionDataHelper.cs");
					AcceptedDomain defaultAcceptedDomain = tenantOrTopologyConfigurationSession.GetDefaultAcceptedDomain();
					if (defaultAcceptedDomain != null && defaultAcceptedDomain.DomainName != null && defaultAcceptedDomain.DomainName.Domain != null)
					{
						text = defaultAcceptedDomain.DomainName.Domain;
					}
					else
					{
						ExtensionDataHelper.Tracer.TraceError(0L, "Failed to get a valid default accepted domain for deployment id.");
					}
				}
			}
			catch (ADTransientException ex)
			{
				ExtensionDataHelper.Tracer.TraceError<string>(0L, "Failed to get default accepted domain for deployment id. Exception: {0}", ex.Message);
			}
			if (string.IsNullOrWhiteSpace(text))
			{
				ExtensionDataHelper.Tracer.TraceInformation<string>(0, 0L, "Can not get default authorative accepted domain, fall back to primary smtp domain: {0}.", domain);
				ExtensionDiagnostics.Logger.LogEvent(ApplicationLogicEventLogConstants.Tuple_FailedToGetDeploymentId, null, new object[]
				{
					"ProcessEntitlementToken",
					domain
				});
				text = domain;
			}
			return text;
		}

		internal static bool VerifyDeploymentId(string deploymentId, string domain)
		{
			if (!string.IsNullOrWhiteSpace(deploymentId))
			{
				try
				{
					ADSessionSettings adsessionSettings = ExtensionDataHelper.CreateRootOrgOrSingleTenantFromAcceptedDomainAutoDetect(domain);
					if (adsessionSettings != null)
					{
						IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, adsessionSettings, 142, "VerifyDeploymentId", "f:\\15.00.1497\\sources\\dev\\data\\src\\ApplicationLogic\\extension\\ExtensionDataHelper.cs");
						AcceptedDomain acceptedDomainByDomainName = tenantOrTopologyConfigurationSession.GetAcceptedDomainByDomainName(deploymentId);
						if (acceptedDomainByDomainName != null)
						{
							return true;
						}
					}
				}
				catch (ADTransientException ex)
				{
					ExtensionDataHelper.Tracer.TraceError<string>(0L, "Failed to get accepted domain by deployment id. Exception: {0}", ex.Message);
				}
				return false;
			}
			return false;
		}

		internal static bool TryGetAttributeValue(XmlAttribute attribute, out string value)
		{
			value = null;
			if (attribute == null)
			{
				return false;
			}
			value = attribute.Value;
			return !string.IsNullOrEmpty(value);
		}

		internal static bool TryGetNameSpaceStrippedAttributeValue(XmlAttribute attribute, out string value)
		{
			if (!ExtensionDataHelper.TryGetAttributeValue(attribute, out value))
			{
				return false;
			}
			int num = value.LastIndexOf(':');
			if (-1 != num)
			{
				num++;
				value = value.Substring(num, value.Length - num);
				if (string.IsNullOrEmpty(value))
				{
					return false;
				}
			}
			return true;
		}

		internal static void ValidateRegex(string regexPattern)
		{
			new Regex(regexPattern, RegexOptions.ECMAScript);
		}

		internal static bool ConvertXmlStringToBoolean(string value)
		{
			return value != null && (value.Equals("true", StringComparison.OrdinalIgnoreCase) || value == "1");
		}

		internal static bool IsValidUri(string uriString)
		{
			Uri uri;
			return Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out uri);
		}

		internal static SafeXmlDocument GetManifest(SafeXmlDocument xmlDoc)
		{
			if (ExtensionDataHelper.xmlSchemaSet.Count == 0)
			{
				ExtensionDataHelper.xmlSchemaSet = new XmlSchemaSet();
				foreach (string text in SchemaConstants.SchemaNamespaceUriToFile.Keys)
				{
					string schemaUri = Path.Combine(ExchangeSetupContext.InstallPath, "bin", SchemaConstants.SchemaNamespaceUriToFile[text]);
					ExtensionDataHelper.xmlSchemaSet.Add(text, schemaUri);
				}
			}
			xmlDoc.Schemas = ExtensionDataHelper.xmlSchemaSet;
			xmlDoc.Validate(new ValidationEventHandler(ExtensionDataHelper.InvalidManifestEventHandler));
			string uri;
			string text2;
			if (!ExtensionDataHelper.TryGetOfficeAppSchemaInfo(xmlDoc, "http://schemas.microsoft.com/office/appforoffice/", out uri, out text2))
			{
				throw new OwaExtensionOperationException(Strings.ErrorInvalidManifestData(Strings.ErrorReasonManifestSchemaUnknown));
			}
			XmlNamespaceManager xmlNamespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
			xmlNamespaceManager.AddNamespace("owe", uri);
			SafeXmlDocument safeXmlDocument = null;
			string xpath = "//owe:OfficeApp";
			XmlNode xmlNode = xmlDoc.SelectSingleNode(xpath, xmlNamespaceManager);
			if (xmlNode != null)
			{
				safeXmlDocument = new SafeXmlDocument();
				safeXmlDocument.PreserveWhitespace = true;
				safeXmlDocument.LoadXml(xmlNode.OuterXml);
			}
			return safeXmlDocument;
		}

		internal static bool TryGetOfficeAppSchemaInfo(XmlNode xmlDoc, string nonVersionSpecificNamspacePart, out string namespaceUri, out string schemaVersion)
		{
			namespaceUri = null;
			schemaVersion = null;
			for (XmlNode xmlNode = xmlDoc.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode.NamespaceURI.StartsWith(nonVersionSpecificNamspacePart, StringComparison.OrdinalIgnoreCase))
				{
					namespaceUri = xmlNode.NamespaceURI;
					schemaVersion = xmlNode.NamespaceURI.Substring(nonVersionSpecificNamspacePart.Length);
					return true;
				}
			}
			return false;
		}

		internal static SchemaParser GetSchemaParser(SafeXmlDocument xmlDoc, ExtensionInstallScope scope)
		{
			string text;
			string schemaVersion;
			if (!ExtensionDataHelper.TryGetOfficeAppSchemaInfo(xmlDoc, "http://schemas.microsoft.com/office/appforoffice/", out text, out schemaVersion))
			{
				throw new OwaExtensionOperationException(Strings.ErrorReasonManifestSchemaUnknown);
			}
			string a;
			if ((a = text) != null)
			{
				if (a == "http://schemas.microsoft.com/office/appforoffice/1.0")
				{
					return new SchemaParser1_0(xmlDoc, scope);
				}
				if (a == "http://schemas.microsoft.com/office/appforoffice/1.1")
				{
					return new SchemaParser1_1(xmlDoc, scope);
				}
			}
			throw new OwaExtensionOperationException(Strings.ErrorReasonManifestVersionNotSupported(schemaVersion, ExchangeSetupContext.InstalledVersion));
		}

		internal static string GetExceptionMessages(Exception e)
		{
			StringBuilder stringBuilder = new StringBuilder(2 * e.Message.Length);
			do
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" ");
				}
				stringBuilder.Append(e.Message);
				e = e.InnerException;
			}
			while (e != null);
			return stringBuilder.ToString();
		}

		private static void InvalidManifestEventHandler(object sender, ValidationEventArgs e)
		{
			if (e.Exception != null)
			{
				XmlSchemaValidationException ex = e.Exception as XmlSchemaValidationException;
				if (ex != null)
				{
					XmlElement xmlElement = ex.SourceObject as XmlElement;
					if (xmlElement != null && string.Equals(xmlElement.Name, "Signature", StringComparison.OrdinalIgnoreCase))
					{
						return;
					}
				}
			}
			throw new OwaExtensionOperationException(Strings.ErrorInvalidManifestData(Strings.ErrorReasonManifestValidationError(e.Exception.Message)), e.Exception);
		}

		private static ADSessionSettings CreateRootOrgOrSingleTenantFromAcceptedDomainAutoDetect(string domain)
		{
			ADSessionSettings result = null;
			try
			{
				if (Globals.IsDatacenter)
				{
					if (!string.IsNullOrWhiteSpace(domain))
					{
						result = ADSessionSettings.FromTenantAcceptedDomain(domain);
					}
				}
				else
				{
					result = ADSessionSettings.FromRootOrgScopeSet();
				}
			}
			catch (CannotResolveTenantNameException ex)
			{
				ExtensionDataHelper.Tracer.TraceError<string>(0L, "Failed to resolve tenant name based on provided domain", ex.Message);
			}
			return result;
		}

		private const string LeftBracket = "{";

		private const string RightBracket = "}";

		private const string BinFolderName = "bin";

		private static readonly char[] TrimCharArray = new char[]
		{
			'{',
			'}'
		};

		private static readonly Trace Tracer = ExTraceGlobals.ExtensionTracer;

		private static XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();

		internal delegate bool TryModifySourceLocationDelegate(IExchangePrincipal exchangePrincipal, XmlAttribute xmlAttribute, ExtensionData extensionData, out Exception exception);
	}
}
