using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Cryptography.Xml;
using System.Xml;
using Microsoft.Exchange.CabUtility;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Setup.SignatureVerification;

namespace Microsoft.Exchange.Setup.AcquireLanguagePack
{
	internal static class LanguagePackXmlHelper
	{
		public static bool VerifyXmlSignature(SafeXmlDocument doc)
		{
			string location = Assembly.GetExecutingAssembly().Location;
			SignVerfWrapper signVerfWrapper = new SignVerfWrapper();
			bool flag = signVerfWrapper.VerifyEmbeddedSignature(location, false);
			if (flag)
			{
				bool result = false;
				SignedXml signedXml = new SignedXml(doc);
				using (XmlNodeList elementsByTagName = doc.GetElementsByTagName("Signature"))
				{
					if (elementsByTagName != null && elementsByTagName.Count > 0)
					{
						signedXml.LoadXml((XmlElement)elementsByTagName[0]);
						result = signedXml.CheckSignature();
					}
				}
				return result;
			}
			return true;
		}

		public static List<DownloadFileInfo> GetDownloadFileInfoFromXml(string lpVersioningXml, bool skipSignatureCheckForTestOnly)
		{
			ValidationHelper.ThrowIfFileNotExist(lpVersioningXml, "lpVersioningXml");
			Version version = new Version(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion);
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			safeXmlDocument.PreserveWhitespace = true;
			safeXmlDocument.Load(lpVersioningXml);
			if (!skipSignatureCheckForTestOnly && !LanguagePackXmlHelper.VerifyXmlSignature(safeXmlDocument))
			{
				throw new SignatureVerificationException(Strings.SignatureFailed1(lpVersioningXml));
			}
			safeXmlDocument.PreserveWhitespace = false;
			safeXmlDocument.Load(lpVersioningXml);
			XmlElement documentElement = safeXmlDocument.DocumentElement;
			List<DownloadFileInfo> list = new List<DownloadFileInfo>();
			list.Add(LanguagePackXmlHelper.GetLPDownloadFileInfoFromXmlElement(documentElement, version));
			list.AddRange(LanguagePackXmlHelper.GetMspsDownloadFileInfoFromXmlElement(documentElement));
			if (!LanguagePackXmlHelper.VerifyDownloadFileInfo(list))
			{
				throw new LPVersioningValueException(Strings.fWLinkNotFound(version.ToString(), lpVersioningXml));
			}
			return list;
		}

		public static string ExtractXMLFromBundle(string bundleFileName, string toPath)
		{
			ValidationHelper.ThrowIfFileNotExist(bundleFileName, "bundleFileName");
			ValidationHelper.ThrowIfDirectoryNotExist(toPath, "toPath");
			EmbeddedCabWrapper.ExtractFiles(bundleFileName, toPath, "LPVersioning.xml");
			return Path.Combine(toPath, "LPVersioning.xml");
		}

		public static bool ContainsOnlyDownloadedFiles(string path)
		{
			if (!Directory.Exists(path))
			{
				return false;
			}
			string[] files = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly);
			if (files == null || files.Length == 0)
			{
				return false;
			}
			foreach (string filePath in files)
			{
				if (!DownloadFileInfo.IsFileNameValid(filePath, "\\.msp") && !DownloadFileInfo.IsFileNameValid(filePath, "LanguagePackBundle.exe".ToLower()))
				{
					return false;
				}
			}
			return true;
		}

		private static DownloadFileInfo GetLPDownloadFileInfoFromXmlElement(XmlElement xmlElement, Version ceilingVersion)
		{
			ValidationHelper.ThrowIfNull(xmlElement, "xmlElement");
			DownloadFileInfo result = null;
			using (XmlNodeList elementsByTagName = xmlElement.GetElementsByTagName("ExVersion"))
			{
				foreach (object obj in elementsByTagName)
				{
					XmlNode xmlNode = (XmlNode)obj;
					XmlAttributeCollection attributes = xmlNode.Attributes;
					Version v = new Version(attributes[0].Value.ToString());
					if (ceilingVersion >= v)
					{
						string text = xmlNode.FirstChild.InnerText.ToString();
						if (!string.IsNullOrEmpty(text))
						{
							result = new DownloadFileInfo(text, "LanguagePackBundle.exe".ToLower(), true);
						}
					}
				}
			}
			return result;
		}

		private static List<DownloadFileInfo> GetMspsDownloadFileInfoFromXmlElement(XmlElement xmlElement)
		{
			ValidationHelper.ThrowIfNull(xmlElement, "xmlElement");
			List<DownloadFileInfo> list = new List<DownloadFileInfo>();
			using (XmlNodeList elementsByTagName = xmlElement.GetElementsByTagName("ExchangeUpdate"))
			{
				foreach (object obj in elementsByTagName)
				{
					XmlNode xmlNode = (XmlNode)obj;
					foreach (object obj2 in xmlNode.ChildNodes)
					{
						XmlNode xmlNode2 = (XmlNode)obj2;
						if (xmlNode2.NodeType == XmlNodeType.Element)
						{
							string text = xmlNode2.InnerText.ToString();
							if (!string.IsNullOrEmpty(text))
							{
								list.Add(new DownloadFileInfo(text, "\\.msp", true));
							}
						}
					}
				}
			}
			return list;
		}

		private static bool VerifyDownloadFileInfo(List<DownloadFileInfo> downloads)
		{
			if (downloads == null || downloads.Count < 3)
			{
				return false;
			}
			foreach (DownloadFileInfo downloadFileInfo in downloads)
			{
				if (downloadFileInfo == null)
				{
					return false;
				}
				if (!downloadFileInfo.UriLink.ToString().StartsWith("http://go.microsoft.com/", true, CultureInfo.CurrentCulture))
				{
					return false;
				}
			}
			return true;
		}

		public const int SizeOfBundle = 500000000;

		public const string LPVersioningXml = "LPVersioning.xml";

		public const string LanguagePackBundleEXE = "LanguagePackBundle.exe";

		public const string LPNode = "ExVersion";

		public const string LanguageBundleCompatibilityNode = "LanguageBundleCompatibility";

		public const string BuildVersionAttribute = "BuildVersion";

		public const string MspNode = "ExchangeUpdate";

		public const string SignatureNode = "Signature";

		public const string MspFileFilter = "*.msp";

		private const string MspFileNamePattern = "\\.msp";

		private const int MinLinks = 3;
	}
}
