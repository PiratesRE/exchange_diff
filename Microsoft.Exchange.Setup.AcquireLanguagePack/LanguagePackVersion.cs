using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;

namespace Microsoft.Exchange.Setup.AcquireLanguagePack
{
	internal class LanguagePackVersion
	{
		public static bool SkipSignVerfForTesting
		{
			get
			{
				return LanguagePackVersion.skipSignVerfForTesting;
			}
			set
			{
				LanguagePackVersion.skipSignVerfForTesting = value;
			}
		}

		public string LanguagePackVersioningPath
		{
			get
			{
				return this.langPackVersioningPath;
			}
		}

		private static bool VerifyXmlSignature(SafeXmlDocument doc)
		{
			return LanguagePackVersion.SkipSignVerfForTesting || LanguagePackXmlHelper.VerifyXmlSignature(doc);
		}

		public LanguagePackVersion(string pathToLocalXML, string pathToLangPackBundleXML)
		{
			ValidationHelper.ThrowIfFileNotExist(pathToLocalXML, "pathToLocalXML");
			ValidationHelper.ThrowIfFileNotExist(pathToLangPackBundleXML, "pathToLangPackBundleXML");
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			safeXmlDocument.PreserveWhitespace = true;
			safeXmlDocument.Load(pathToLocalXML);
			if (!LanguagePackVersion.VerifyXmlSignature(safeXmlDocument))
			{
				throw new LPVersioningValueException(Strings.SignatureFailed1(pathToLocalXML));
			}
			safeXmlDocument.Load(pathToLangPackBundleXML);
			if (!LanguagePackVersion.VerifyXmlSignature(safeXmlDocument))
			{
				throw new LPVersioningValueException(Strings.SignatureFailed1(pathToLangPackBundleXML));
			}
			Version version = new Version(LanguagePackVersion.GetBuildVersion(Path.GetFullPath(pathToLocalXML)));
			Version v = new Version(LanguagePackVersion.GetBuildVersion(Path.GetFullPath(pathToLangPackBundleXML)));
			if (version != null && v != null)
			{
				if (v >= version)
				{
					this.langPackVersioningPath = pathToLangPackBundleXML;
				}
				else
				{
					this.langPackVersioningPath = pathToLocalXML;
				}
				this.xmlDoc.Load(this.langPackVersioningPath);
				this.xmlElement = this.xmlDoc.DocumentElement;
			}
		}

		internal static string GetBuildVersion(string inputPath)
		{
			ValidationHelper.ThrowIfFileNotExist(inputPath, "inputPath");
			SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
			safeXmlDocument.Load(inputPath);
			XmlElement documentElement = safeXmlDocument.DocumentElement;
			if (documentElement.HasAttribute("BuildVersion"))
			{
				return documentElement.GetAttribute("BuildVersion");
			}
			throw new LPVersioningValueException(Strings.UnableToFindBuildVersion1(inputPath));
		}

		public bool IsExchangeInApplicableRange(Version lpVersion)
		{
			Version exchangeVersion = new Version(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion);
			return this.IsExchangeInApplicableRange(lpVersion, exchangeVersion);
		}

		public bool IsExchangeInApplicableRange(Version lpVersion, Version exchangeVersion)
		{
			ValidationHelper.ThrowIfNull(this.xmlElement, "this.xmlElement");
			ValidationHelper.ThrowIfNull(this.xmlDoc, "this.xmlDoc");
			ValidationHelper.ThrowIfNull(lpVersion, "lpVersion");
			ValidationHelper.ThrowIfNull(exchangeVersion, "exchangeVersion");
			Version version = null;
			Version version2 = null;
			using (XmlNodeList elementsByTagName = this.xmlElement.GetElementsByTagName("ExVersion"))
			{
				ValidationHelper.ThrowIfNull(elementsByTagName, "xmlNodeList");
				foreach (object obj in elementsByTagName)
				{
					XmlNode xmlNode = (XmlNode)obj;
					XmlAttributeCollection attributes = xmlNode.Attributes;
					Version version3 = new Version(attributes[0].Value.ToString());
					if (!(lpVersion >= version3))
					{
						version2 = version3;
						break;
					}
					version = version3;
				}
			}
			return version != null && exchangeVersion >= version && (version2 == null || exchangeVersion < version2);
		}

		private static bool skipSignVerfForTesting;

		private SafeXmlDocument xmlDoc = new SafeXmlDocument();

		private XmlElement xmlElement;

		private string langPackVersioningPath;
	}
}
