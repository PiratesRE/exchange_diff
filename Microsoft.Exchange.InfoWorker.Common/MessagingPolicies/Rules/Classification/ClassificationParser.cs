using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Compliance.Xml;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Classification
{
	internal sealed class ClassificationParser
	{
		public static ClassificationParser Instance
		{
			get
			{
				return ClassificationParser.instance;
			}
		}

		public ClassificationRulePackage GetRulePackage(byte[] ruleStream)
		{
			byte[] bytes;
			try
			{
				this.UncompressStream(ruleStream, "config", out bytes);
			}
			catch (InvalidOperationException ex)
			{
				throw new ParserException(ex.Message);
			}
			catch (IOException ex2)
			{
				throw new ParserException(ex2.Message);
			}
			string @string;
			try
			{
				@string = Encoding.Unicode.GetString(bytes);
			}
			catch (ArgumentException ex3)
			{
				throw new ParserException(ex3.Message);
			}
			return this.GetRulePackage(@string);
		}

		public ClassificationRulePackage GetRulePackage(string ruleString)
		{
			ClassificationRulePackage result = null;
			XmlReader reader = null;
			try
			{
				XmlReaderSettings settings = new XmlReaderSettings
				{
					ConformanceLevel = ConformanceLevel.Auto,
					IgnoreComments = true,
					DtdProcessing = DtdProcessing.Prohibit,
					XmlResolver = null
				};
				using (StringReader stringReader = new StringReader(ruleString))
				{
					using (XmlTextReader xmlTextReader = SafeXmlFactory.CreateSafeXmlTextReader(stringReader))
					{
						XmlReader xmlReader;
						reader = (xmlReader = XmlReader.Create(xmlTextReader, settings));
						try
						{
							result = this.ParseRule(reader);
						}
						finally
						{
							if (xmlReader != null)
							{
								((IDisposable)xmlReader).Dispose();
							}
						}
					}
				}
			}
			catch (XmlException e)
			{
				throw new ParserException(e);
			}
			catch (RulesValidationException e2)
			{
				throw new ParserException(e2, reader);
			}
			return result;
		}

		private static bool IsVersionedDataClassification(XElement dataClassificationElement)
		{
			return dataClassificationElement != null && dataClassificationElement.Parent != null && dataClassificationElement.Parent.Name != null && dataClassificationElement.Parent.Name.LocalName == "Version";
		}

		private ClassificationRulePackage ParseRule(XmlReader reader)
		{
			XDocument xdocument = XDocument.Load(reader);
			ClassificationRulePackage classificationRulePackage = new ClassificationRulePackage();
			classificationRulePackage.RuleXml = xdocument.Root.ToString(SaveOptions.DisableFormatting);
			classificationRulePackage.VersionedDataClassificationIds = new HashSet<string>(from dataClassificationElement in xdocument.Descendants().AsParallel<XElement>()
			where ClassificationConstants.DataClassificationElementNames.Contains(dataClassificationElement.Name.LocalName) && ClassificationParser.IsVersionedDataClassification(dataClassificationElement)
			let dataClassificationId = dataClassificationElement.Attribute("id").Value.Trim()
			select dataClassificationId, StringComparer.OrdinalIgnoreCase);
			return classificationRulePackage;
		}

		private void UncompressStream(byte[] compressedStream, string packagePart, out byte[] uncompressedStream)
		{
			if (compressedStream == null)
			{
				throw new ArgumentNullException("compressedStream");
			}
			using (Stream stream = new MemoryStream(compressedStream))
			{
				using (Package package = Package.Open(stream))
				{
					Uri partUri = PackUriHelper.CreatePartUri(new Uri(packagePart, UriKind.Relative));
					PackagePart part = package.GetPart(partUri);
					Stream stream2 = part.GetStream(FileMode.Open, FileAccess.Read);
					using (MemoryStream memoryStream = new MemoryStream(compressedStream.Length))
					{
						stream2.CopyTo(memoryStream);
						uncompressedStream = memoryStream.ToArray();
					}
				}
			}
		}

		private static readonly ClassificationParser instance = new ClassificationParser();
	}
}
