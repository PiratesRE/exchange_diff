using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal static class ClassificationDefinitionUtils
	{
		private static ExchangeBuild GetCurrentAssemblyVersion()
		{
			if (ClassificationDefinitionUtils.currentAssemblyVersion == null)
			{
				FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
				ClassificationDefinitionUtils.currentAssemblyVersion = new ExchangeBuild?(new ExchangeBuild((byte)versionInfo.FileMajorPart, (byte)versionInfo.FileMinorPart, (ushort)versionInfo.FileBuildPart, (ushort)versionInfo.FilePrivatePart));
			}
			return ClassificationDefinitionUtils.currentAssemblyVersion.Value;
		}

		internal static bool TryCompressXmlBytes(byte[] uncompressedXmlBytes, out byte[] compressedXmlBytes)
		{
			compressedXmlBytes = null;
			if (uncompressedXmlBytes == null)
			{
				return false;
			}
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (Package package = Package.Open(memoryStream, FileMode.Create))
				{
					PackagePart packagePart = package.CreatePart(ClassificationDefinitionUtils.defaultXmlContentPackagePartUri, "text/xml", CompressionOption.Normal);
					try
					{
						packagePart.GetStream().Write(uncompressedXmlBytes, 0, uncompressedXmlBytes.Length);
					}
					catch (IOException ex)
					{
						TaskLogger.Trace("IOException encountered while compressing classification rule collection payload: {0}", new object[]
						{
							ex.Message
						});
						return false;
					}
				}
				compressedXmlBytes = memoryStream.ToArray();
			}
			return true;
		}

		internal static Stream LoadStreamFromEmbeddedResource(string embeddedResourceName)
		{
			if (string.IsNullOrWhiteSpace(embeddedResourceName))
			{
				throw new ArgumentException(new ArgumentException().Message, "embeddedResourceName");
			}
			Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(embeddedResourceName);
			if (manifestResourceStream == null)
			{
				throw new FileNotFoundException(new FileNotFoundException().Message, embeddedResourceName);
			}
			return manifestResourceStream;
		}

		internal static bool TryUncompressXmlBytes(byte[] compressedXmlBytes, out byte[] uncompressedXmlBytes, out Exception operationException)
		{
			uncompressedXmlBytes = null;
			operationException = null;
			if (compressedXmlBytes == null)
			{
				operationException = new ArgumentNullException("compressedXmlBytes");
				return false;
			}
			using (Stream stream = new MemoryStream(compressedXmlBytes))
			{
				using (Package package = Package.Open(stream, FileMode.Open))
				{
					PackagePart part;
					try
					{
						part = package.GetPart(ClassificationDefinitionUtils.defaultXmlContentPackagePartUri);
					}
					catch (InvalidOperationException ex)
					{
						operationException = ex;
						return false;
					}
					using (MemoryStream memoryStream = new MemoryStream(compressedXmlBytes.Length))
					{
						try
						{
							part.GetStream().CopyTo(memoryStream);
						}
						catch (IOException ex2)
						{
							operationException = ex2;
							return false;
						}
						uncompressedXmlBytes = memoryStream.ToArray();
					}
				}
			}
			return true;
		}

		internal static ADObjectId GetClassificationRuleCollectionContainerId(IConfigDataProvider session)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			return ((IConfigurationSession)session).GetOrgContainerId().GetDescendantId(ClassificationDefinitionConstants.ClassificationDefinitionsRdn);
		}

		internal static bool IsAdObjectAClassificationRuleCollection(TransportRule transportRule)
		{
			if (transportRule == null)
			{
				throw new ArgumentNullException("transportRule");
			}
			return ((ADObjectId)transportRule.Identity).Parent.DistinguishedName.Equals(transportRule.Session.GetOrgContainerId().GetDescendantId(ClassificationDefinitionConstants.ClassificationDefinitionsRdn).ToDNString(), StringComparison.OrdinalIgnoreCase);
		}

		internal static XDocument GetRuleCollectionDocumentFromTransportRule(TransportRule transportRule)
		{
			if (transportRule == null)
			{
				throw new ArgumentNullException("transportRule");
			}
			if (!ClassificationDefinitionUtils.IsAdObjectAClassificationRuleCollection(transportRule))
			{
				throw new InvalidOperationException();
			}
			if (transportRule.ReplicationSignature == null)
			{
				throw new ArgumentException(new ArgumentException().Message, "transportRule");
			}
			byte[] bytes;
			Exception ex;
			if (!ClassificationDefinitionUtils.TryUncompressXmlBytes(transportRule.ReplicationSignature, out bytes, out ex))
			{
				throw new AggregateException(new Exception[]
				{
					ex
				});
			}
			XDocument result;
			try
			{
				result = XDocument.Parse(Encoding.Unicode.GetString(bytes));
			}
			catch (ArgumentException ex2)
			{
				throw new AggregateException(new Exception[]
				{
					ex2
				});
			}
			return result;
		}

		internal static XDocument CreateRuleCollectionDocumentFromTemplate(string rulePackId, string organizationId, string organizationName)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("rulePackId", rulePackId);
			ArgumentValidator.ThrowIfNullOrEmpty("organizationId", organizationId);
			ArgumentValidator.ThrowIfNullOrEmpty("organizationName", organizationName);
			string text = string.Empty;
			using (Stream stream = ClassificationDefinitionUtils.LoadStreamFromEmbeddedResource("FingerprintRulePackTemplate.xml"))
			{
				using (StreamReader streamReader = new StreamReader(stream))
				{
					text = streamReader.ReadToEnd();
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				throw new FingerprintRulePackTemplateCorruptedException("FingerprintRulePackTemplate.xml");
			}
			XDocument xdocument = XDocument.Parse(text);
			XElement xelement = xdocument.Root.Element(XmlProcessingUtils.GetMceNsQualifiedNodeName("RulePack"));
			xelement.SetAttributeValue("id", rulePackId);
			XElement xelement2 = xelement.Element(XmlProcessingUtils.GetMceNsQualifiedNodeName("Publisher"));
			xelement2.SetAttributeValue("id", organizationId);
			foreach (XElement xelement3 in xelement.Descendants(XmlProcessingUtils.GetMceNsQualifiedNodeName("PublisherName")))
			{
				xelement3.SetValue(organizationName);
			}
			XmlProcessingUtils.SetRulePackVersionFromAssemblyFileVersion(xdocument);
			return xdocument;
		}

		internal static string CreateHierarchicalIdentityString(string organizationHierarchy, string objectName)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(organizationHierarchy ?? string.Empty);
			stringBuilder.Append(ClassificationDefinitionConstants.HierarchicalIdentitySeparatorChar);
			stringBuilder.Append(objectName ?? string.Empty);
			return stringBuilder.ToString();
		}

		internal static string GetMceForAdminToolInstallBase(bool shouldHandleException = false)
		{
			string result;
			try
			{
				result = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			}
			catch (PathTooLongException underlyingException)
			{
				if (!shouldHandleException)
				{
					throw;
				}
				ClassificationDefinitionsDiagnosticsReporter.Instance.WriteClassificationEngineConfigurationErrorInformation(0, underlyingException);
				result = null;
			}
			return result;
		}

		internal static string GetMceExecutablePath(bool shouldHandleException = false)
		{
			string mceForAdminToolInstallBase = ClassificationDefinitionUtils.GetMceForAdminToolInstallBase(shouldHandleException);
			if (mceForAdminToolInstallBase != null)
			{
				return Path.Combine(mceForAdminToolInstallBase, ClassificationDefinitionConstants.MceExecutableFileName);
			}
			return null;
		}

		internal static string GetLocalMachineMceConfigDirectory(bool shouldHandleException = false)
		{
			string mceForAdminToolInstallBase = ClassificationDefinitionUtils.GetMceForAdminToolInstallBase(shouldHandleException);
			if (mceForAdminToolInstallBase != null)
			{
				return Path.Combine(mceForAdminToolInstallBase, ClassificationDefinitionConstants.OnDiskMceConfigurationDirName);
			}
			return null;
		}

		internal static XmlReaderSettings CreateSafeXmlReaderSettings()
		{
			return new XmlReaderSettings
			{
				DtdProcessing = DtdProcessing.Prohibit,
				XmlResolver = null
			};
		}

		internal static T GetMatchingLocalizedInfo<T>(Dictionary<CultureInfo, T> localizedInfoRepository, T defaultValue) where T : class
		{
			if (localizedInfoRepository == null)
			{
				throw new ArgumentNullException("localizedInfoRepository");
			}
			CultureInfo cultureInfo = CultureInfo.CurrentCulture;
			T t = default(T);
			while (!cultureInfo.Equals(CultureInfo.InvariantCulture) && !localizedInfoRepository.TryGetValue(cultureInfo, out t))
			{
				cultureInfo = cultureInfo.Parent;
			}
			T result;
			if ((result = t) == null)
			{
				result = defaultValue;
			}
			return result;
		}

		internal static TException PopulateExceptionSource<TException, TSource>(TException exception, TSource exceptionSource) where TException : Exception
		{
			if (exception != null)
			{
				exception.Data[ClassificationDefinitionConstants.ExceptionSourcesListKey] = exceptionSource;
			}
			return exception;
		}

		internal static IEnumerable<DataClassificationPresentationObject> FilterHigherVersionRules(IEnumerable<DataClassificationPresentationObject> unfilteredRules)
		{
			if (unfilteredRules == null)
			{
				throw new ArgumentNullException("unfilteredRules");
			}
			return from rule in unfilteredRules
			where rule.MinEngineVersion <= ClassificationDefinitionUtils.GetCurrentAssemblyVersion()
			select rule;
		}

		private static readonly Uri defaultXmlContentPackagePartUri = PackUriHelper.CreatePartUri(new Uri("config", UriKind.Relative));

		private static ExchangeBuild? currentAssemblyVersion;
	}
}
