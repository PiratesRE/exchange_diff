using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class RMSUtil
	{
		internal static bool IsRMSOnline(string[] unCompressedSLCCertChainStringArray)
		{
			if (unCompressedSLCCertChainStringArray == null || !unCompressedSLCCertChainStringArray.Any<string>())
			{
				throw new ArgumentNullException("unCompressedSLCCertChainStringArray");
			}
			XmlReaderSettings settings = new XmlReaderSettings
			{
				ConformanceLevel = ConformanceLevel.Auto,
				IgnoreComments = true,
				DtdProcessing = DtdProcessing.Prohibit,
				XmlResolver = null
			};
			bool result;
			using (StringReader stringReader = new StringReader(unCompressedSLCCertChainStringArray[0]))
			{
				try
				{
					using (XmlReader xmlReader = XmlReader.Create(stringReader, settings))
					{
						result = (null != XDocument.Load(xmlReader).XPathSelectElement("/XrML/BODY/ISSUEDPRINCIPALS/PRINCIPAL[1]/SECURITYLEVEL[@name='Tenant-ID']"));
					}
				}
				catch (XmlException)
				{
					result = false;
				}
			}
			return result;
		}
	}
}
