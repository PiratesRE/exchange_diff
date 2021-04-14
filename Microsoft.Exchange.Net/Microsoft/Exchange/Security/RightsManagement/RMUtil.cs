using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Microsoft.com.IPC.WSService;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.RightsManagementServices.Provider;

namespace Microsoft.Exchange.Security.RightsManagement
{
	internal static class RMUtil
	{
		public static bool TryCreateUri(string toBeCreated, out Uri uriCreated)
		{
			uriCreated = null;
			if (string.IsNullOrEmpty(toBeCreated))
			{
				return false;
			}
			if (!Uri.TryCreate(toBeCreated, UriKind.Absolute, out uriCreated))
			{
				uriCreated = null;
				return false;
			}
			return true;
		}

		public static bool IsWellFormedRmServiceUrl(Uri toBeChecked)
		{
			return !(toBeChecked == null) && toBeChecked.IsAbsoluteUri && (!(Uri.UriSchemeHttp != toBeChecked.Scheme) || !(Uri.UriSchemeHttps != toBeChecked.Scheme)) && string.IsNullOrEmpty(toBeChecked.Query) && string.IsNullOrEmpty(toBeChecked.Fragment) && toBeChecked.AbsoluteUri.Length <= 1024 && toBeChecked.AbsoluteUri.IndexOf(',') == -1;
		}

		public static string ConvertUriToLicenseUrl(Uri offeredUri)
		{
			return RMUtil.ConvertUriToRmServiceUrl(offeredUri, "_wmcs/licensing/license.asmx");
		}

		public static string ConvertUriToPublishUrl(Uri offeredUri)
		{
			return RMUtil.ConvertUriToRmServiceUrl(offeredUri, "_wmcs/licensing/publish.asmx");
		}

		public static string ConvertUriToServerCertificationUrl(Uri offeredUri)
		{
			return RMUtil.ConvertUriToRmServiceUrl(offeredUri, "_wmcs/certification/servercertification.asmx");
		}

		public static string ConvertUriToTemplateDistributionUrl(Uri offeredUri)
		{
			return RMUtil.ConvertUriToRmServiceUrl(offeredUri, "_wmcs/licensing/templatedistribution.asmx");
		}

		public static string ConvertUriToServerUrl(Uri offeredUri, bool vdirCertification)
		{
			return RMUtil.ConvertUriToRmServiceUrl(offeredUri, vdirCertification ? "_wmcs/certification/server.asmx" : "_wmcs/licensing/server.asmx");
		}

		public static Uri ConvertUriToLicenseLocationDistributionPoint(Uri offeredUri)
		{
			string text = RMUtil.ConvertUriToRmServiceUrl(offeredUri, "_wmcs/licensing");
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			return new Uri(text.ToLowerInvariant(), UriKind.Absolute);
		}

		public static Uri ConvertUriToCertificateLocationDistributionPoint(Uri offeredUri)
		{
			string text = RMUtil.ConvertUriToRmServiceUrl(offeredUri, "_wmcs/certification");
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			return new Uri(text.ToLowerInvariant(), UriKind.Absolute);
		}

		private static string ConvertUriToRmServiceUrl(Uri offeredUri, string localPart)
		{
			if (!RMUtil.IsWellFormedRmServiceUrl(offeredUri))
			{
				return null;
			}
			string text = offeredUri.ToString();
			if (text.EndsWith(localPart, StringComparison.OrdinalIgnoreCase))
			{
				return text;
			}
			string components = offeredUri.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped);
			if (string.IsNullOrEmpty(components))
			{
				return null;
			}
			return string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
			{
				components,
				localPart
			});
		}

		private static DateTime GetLicenseExpirationTime(XmlNode license)
		{
			DateTime maxValue = DateTime.MaxValue;
			XPathNavigator xpathNavigator = license.CreateNavigator();
			XPathNodeIterator xpathNodeIterator = xpathNavigator.Select("./BODY/VALIDITYTIME/UNTIL");
			if (xpathNodeIterator.MoveNext() && xpathNodeIterator.Current != null)
			{
				string value = xpathNodeIterator.Current.Value;
				if (!string.IsNullOrEmpty(value) && !DateTime.TryParseExact(value, "yyyy-MM-ddTHH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out maxValue))
				{
					maxValue = DateTime.MaxValue;
				}
			}
			return maxValue;
		}

		public static DateTime GetRacExpirationTime(XmlNode rac)
		{
			return RMUtil.GetLicenseExpirationTime(rac);
		}

		public static DateTime GetClcExpirationTime(XmlNode clc)
		{
			return RMUtil.GetLicenseExpirationTime(clc);
		}

		public static bool TryConvertStringToXmlNode(string source, out XmlNode xmlNode)
		{
			xmlNode = null;
			bool result = false;
			if (!string.IsNullOrEmpty(source))
			{
				XmlDocument xmlDocument = new SafeXmlDocument();
				try
				{
					xmlDocument.InnerXml = source;
					xmlNode = xmlDocument.DocumentElement;
					if (xmlNode != null)
					{
						result = true;
					}
				}
				catch (XmlException)
				{
					xmlNode = null;
				}
			}
			return result;
		}

		public static bool IsValidCertificateChain(XmlNode[] certChain)
		{
			if (certChain == null || certChain.Length == 0)
			{
				return false;
			}
			for (int i = 0; i < certChain.Length; i++)
			{
				if (certChain[i] == null || string.IsNullOrEmpty(certChain[i].OuterXml))
				{
					return false;
				}
			}
			return true;
		}

		public static string ConvertXmlNodeArrayToString(XmlNode[] nodes)
		{
			if (nodes == null || nodes.Length == 0)
			{
				return null;
			}
			StringBuilder stringBuilder = new StringBuilder(nodes[0].OuterXml);
			for (int i = 1; i < nodes.Length; i++)
			{
				stringBuilder.Append(nodes[i].OuterXml);
			}
			return stringBuilder.ToString();
		}

		public static string ConvertXrmlCertificateChainToString(XrmlCertificateChain certs)
		{
			if (certs == null)
			{
				throw new ArgumentNullException("certs");
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string value in certs.CertificateChain)
			{
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}

		public static XrmlCertificateChain ConvertStringToXrmlCertificateChain(string appendedCerts)
		{
			if (appendedCerts == null)
			{
				throw new ArgumentNullException("appendedCerts");
			}
			int num = 0;
			int num2 = appendedCerts.IndexOf("<?xml", num, StringComparison.OrdinalIgnoreCase);
			if (num2 == -1)
			{
				throw new FormatException("Unable to find the xml version tag for the first certificate");
			}
			List<string> list = new List<string>();
			while ((num = appendedCerts.IndexOf("</XrML>", num, StringComparison.OrdinalIgnoreCase)) != -1)
			{
				num += "</XrML>".Length;
				string item = appendedCerts.Substring(num2, num - num2);
				list.Add(item);
				num2 = appendedCerts.IndexOf("<?xml", num, StringComparison.OrdinalIgnoreCase);
				if (num2 == -1)
				{
					break;
				}
			}
			if (list.Count != 0)
			{
				XrmlCertificateChain xrmlCertificateChain = new XrmlCertificateChain();
				string[] array = new string[list.Count];
				list.CopyTo(array, 0);
				xrmlCertificateChain.CertificateChain = array;
				return xrmlCertificateChain;
			}
			throw new FormatException("No valid certificates");
		}

		public static bool TryConvertAppendedCertsToXmlNodeArray(string appendedCerts, out XmlNode[] nodes)
		{
			int num = 0;
			int num2 = 0;
			LinkedList<XmlNode> linkedList = new LinkedList<XmlNode>();
			while ((num2 = appendedCerts.IndexOf("</XrML>", num2, StringComparison.OrdinalIgnoreCase)) != -1)
			{
				num2 += "</XrML>".Length;
				string source = appendedCerts.Substring(num, num2 - num);
				XmlNode value;
				if (!RMUtil.TryConvertStringToXmlNode(source, out value))
				{
					nodes = null;
					return false;
				}
				linkedList.AddLast(value);
				num = num2;
			}
			int count = linkedList.Count;
			if (count == 0)
			{
				nodes = null;
				return false;
			}
			nodes = new XmlNode[count];
			linkedList.CopyTo(nodes, 0);
			return true;
		}

		public static bool TryConvertCertChainToXmlNodeArray(string certChain, out XmlNode[] nodeArray)
		{
			nodeArray = null;
			bool result = false;
			if (!string.IsNullOrEmpty(certChain))
			{
				int startIndex = 0;
				if (certChain.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase))
				{
					startIndex = "?>".Length + certChain.IndexOf("?>", StringComparison.Ordinal);
				}
				StringBuilder stringBuilder = new StringBuilder("<SunShine>");
				stringBuilder.Append(certChain.Substring(startIndex));
				stringBuilder.Append("</SunShine>");
				XmlNode xmlNode;
				if (RMUtil.TryConvertStringToXmlNode(stringBuilder.ToString(), out xmlNode))
				{
					using (XmlNodeList childNodes = xmlNode.ChildNodes)
					{
						if (childNodes.Count > 0)
						{
							nodeArray = new XmlNode[childNodes.Count];
							for (int i = 0; i < childNodes.Count; i++)
							{
								nodeArray[i] = childNodes[i];
							}
							result = true;
						}
					}
				}
			}
			return result;
		}

		public static bool TryConvertCertChainStringArrayToXmlNodeArray(string[] stringArray, out XmlNode[] nodeArray)
		{
			if (stringArray == null || stringArray.Length < 1 || string.IsNullOrEmpty(stringArray[0]))
			{
				nodeArray = null;
				return false;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string text in stringArray)
			{
				if (!string.IsNullOrEmpty(text))
				{
					if (text.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase))
					{
						int num = text.IndexOf("?>", StringComparison.Ordinal);
						if (num == -1)
						{
							nodeArray = null;
							return false;
						}
						int startIndex = "?>".Length + num;
						stringBuilder.Append(text.Substring(startIndex));
					}
					else
					{
						stringBuilder.Append(text);
					}
				}
			}
			string appendedCerts = stringBuilder.ToString();
			return RMUtil.TryConvertAppendedCertsToXmlNodeArray(appendedCerts, out nodeArray);
		}

		public static bool TryGetIssuanceLicenseAndUrls(string publishLicense, out Uri intranetUri, out Uri extranetUri, out XmlNode[] license)
		{
			license = null;
			intranetUri = null;
			extranetUri = null;
			Exception ex = null;
			if (string.IsNullOrEmpty(publishLicense))
			{
				throw new ArgumentNullException("publishLicense");
			}
			bool result;
			try
			{
				if (string.IsNullOrEmpty(publishLicense))
				{
					ExTraceGlobals.RightsManagementTracer.TraceError(0L, "The issuance license is not present or is empty.");
					result = false;
				}
				else if (publishLicense.Length >= 1047552)
				{
					ExTraceGlobals.RightsManagementTracer.TraceError(0L, "Issuance license's length exceed the max allowed size.");
					result = false;
				}
				else
				{
					DrmClientUtils.ParsePublishLicense(publishLicense, out intranetUri, out extranetUri);
					if (!RMUtil.TryConvertCertChainToXmlNodeArray(publishLicense, out license) || license == null)
					{
						ExTraceGlobals.RightsManagementTracer.TraceError(0L, "Invalid issuance license");
						result = false;
					}
					else
					{
						result = true;
					}
				}
			}
			catch (RightsManagementException ex2)
			{
				ex = ex2;
				result = false;
			}
			finally
			{
				if (ex != null)
				{
					ExTraceGlobals.RightsManagementTracer.TraceError<string>(0L, "Exception thrown while trying to retrieve Issuance License and URI. Error is {0}", ex.Message);
				}
			}
			return result;
		}

		public static string CompressSLCCertificateChain(string[] certs)
		{
			if (certs == null || certs.Length == 0)
			{
				throw new ArgumentNullException("certs");
			}
			string certificate = RMUtil.ConvertXrmlCertificateChainToString(new XrmlCertificateChain
			{
				CertificateChain = certs
			});
			return RMUtil.CompressAndBase64EncodeCertificate(certificate);
		}

		public static XrmlCertificateChain DecompressSLCCertificate(string compressedCerts)
		{
			if (string.IsNullOrEmpty(compressedCerts))
			{
				throw new ArgumentNullException("compressedCerts");
			}
			string appendedCerts = RMUtil.Base64DecodeAndDecompressCertificate(compressedCerts);
			XrmlCertificateChain xrmlCertificateChain = RMUtil.ConvertStringToXrmlCertificateChain(appendedCerts);
			return new XrmlCertificateChain(xrmlCertificateChain.CertificateChain);
		}

		public static string CompressTemplate(string templateXrml, RmsTemplateType type)
		{
			if (string.IsNullOrEmpty(templateXrml))
			{
				throw new ArgumentNullException("templateXrml");
			}
			return RMUtil.CompressAndBase64EncodeCertificate(string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", new object[]
			{
				(int)type,
				":",
				templateXrml.Trim()
			}));
		}

		public static string DecompressTemplate(string encodedTemplate, out RmsTemplateType templateType)
		{
			if (string.IsNullOrEmpty(encodedTemplate))
			{
				throw new ArgumentNullException("encodedTemplate");
			}
			string text = RMUtil.Base64DecodeAndDecompressCertificate(encodedTemplate);
			int num = text.IndexOf(":<?xml", 0, StringComparison.OrdinalIgnoreCase);
			if (num == -1)
			{
				throw new FormatException("Failed to parse type information from the template. Could not find type index.");
			}
			string s = text.Substring(0, num);
			int num2;
			if (!int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out num2))
			{
				throw new FormatException("Failed to parse type information from the template. Type Value not a valid RmsTemplateType.");
			}
			if (!Enum.IsDefined(typeof(RmsTemplateType), num2))
			{
				templateType = RmsTemplateType.Archived;
			}
			else
			{
				templateType = (RmsTemplateType)num2;
			}
			return text.Substring(num + ":".Length);
		}

		public static string Base64DecodeAndDecompressCertificate(string encodedCert)
		{
			if (string.IsNullOrEmpty(encodedCert))
			{
				throw new ArgumentNullException("encodedCert");
			}
			byte[] compressedBytes = Convert.FromBase64String(encodedCert);
			return DrmEmailCompression.DecompressString(compressedBytes);
		}

		private static string CompressAndBase64EncodeCertificate(string certificate)
		{
			if (string.IsNullOrEmpty(certificate))
			{
				throw new ArgumentNullException("certificate");
			}
			return Convert.ToBase64String(DrmEmailCompression.CompressString(certificate));
		}

		private const string RacTimeXPathQuery = "./BODY/VALIDITYTIME/UNTIL";

		private const string RacTimeFormat = "yyyy-MM-ddTHH:mm";

		private const string XmlDeclarationStart = "<?xml";

		private const string XmlDeclarationEnd = "?>";

		private const string TempBeginTag = "<SunShine>";

		private const string TempEndTag = "</SunShine>";

		private const int IssuanceLicenseMaxLength = 1047552;

		private const char Comma = ',';
	}
}
