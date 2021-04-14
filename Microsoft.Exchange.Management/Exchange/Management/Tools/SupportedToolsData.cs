using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Tools
{
	internal sealed class SupportedToolsData
	{
		private SupportedToolsData(string dataFile, string schemaFile)
		{
			this.dataFile = dataFile;
			this.schemaFile = schemaFile;
		}

		internal static SupportedToolsData GetSupportedToolData(string dataFile, string schemaFile)
		{
			SupportedToolsData supportedToolsData = new SupportedToolsData(dataFile, schemaFile);
			supportedToolsData.ReadFile();
			supportedToolsData.Validate();
			return supportedToolsData;
		}

		internal bool RequiresTenantVersion()
		{
			foreach (ToolInfoData toolInfo in this.toolData.toolInformation)
			{
				if (SupportedToolsData.ContainsTenantVersionInformation(toolInfo))
				{
					return true;
				}
			}
			return false;
		}

		internal SupportedVersion GetSupportedVersion(ToolId toolId, Version tenantVersion)
		{
			foreach (ToolInfoData toolInfoData in this.toolData.toolInformation)
			{
				if (toolInfoData.id == toolId)
				{
					return SupportedToolsData.GetSupportedVersion(toolInfoData, tenantVersion);
				}
			}
			return null;
		}

		private static SupportedVersion GetSupportedVersion(ToolInfoData toolInfo, Version tenantVersion)
		{
			SupportedVersion supportedVersion = null;
			if (tenantVersion != null && toolInfo.customSupportedVersion != null)
			{
				foreach (CustomSupportedVersion customSupportedVersion2 in toolInfo.customSupportedVersion)
				{
					if (SupportedToolsData.IsInRange(customSupportedVersion2, tenantVersion))
					{
						supportedVersion = customSupportedVersion2;
						break;
					}
				}
			}
			return supportedVersion ?? toolInfo.defaultSupportedVersion;
		}

		private static bool IsInRange(CustomSupportedVersion versionInfo, Version tenantVersion)
		{
			if (versionInfo == null || tenantVersion == null)
			{
				return false;
			}
			Version version = SupportedToolsData.GetVersion(versionInfo.minTenantVersion, SupportedToolsData.MinimumVersion);
			Version version2 = SupportedToolsData.GetVersion(versionInfo.maxTenantVersion, SupportedToolsData.MaximumVersion);
			return version2 >= tenantVersion && version <= tenantVersion;
		}

		private static bool ContainsTenantVersionInformation(ToolInfoData toolInfo)
		{
			return toolInfo.customSupportedVersion != null && toolInfo.customSupportedVersion.Length > 0;
		}

		private void ReadFile()
		{
			if (!File.Exists(this.dataFile))
			{
				throw new FileNotFoundException(Strings.SupportedToolsCannotFindFile, this.dataFile);
			}
			if (!File.Exists(this.schemaFile))
			{
				throw new FileNotFoundException(Strings.SupportedToolsCannotFindFile, this.schemaFile);
			}
			try
			{
				XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
				xmlReaderSettings.Schemas.Add(string.Empty, this.schemaFile);
				xmlReaderSettings.ValidationType = ValidationType.Schema;
				using (XmlReader xmlReader = XmlReader.Create(this.dataFile, xmlReaderSettings))
				{
					XmlSerializer xmlSerializer = new XmlSerializer(typeof(supportedTools));
					this.toolData = (supportedTools)xmlSerializer.Deserialize(xmlReader);
				}
			}
			catch (IOException e)
			{
				SupportedToolsData.HandleDataReadException(e);
			}
			catch (XmlException e2)
			{
				SupportedToolsData.HandleDataReadException(e2);
			}
			catch (XmlSchemaException e3)
			{
				SupportedToolsData.HandleDataReadException(e3);
			}
			catch (InvalidOperationException ex)
			{
				SupportedToolsData.HandleDataReadException(ex.InnerException ?? ex);
			}
		}

		private void Validate()
		{
			this.ValidateDuplicatedToolEntries();
			this.ValidateToolEntries();
		}

		private void ValidateToolEntries()
		{
			foreach (ToolInfoData toolInfoData in this.toolData.toolInformation)
			{
				if (toolInfoData.defaultSupportedVersion != null && !SupportedToolsData.IsValidVersionRange(toolInfoData.defaultSupportedVersion.minSupportedVersion, toolInfoData.defaultSupportedVersion.latestVersion))
				{
					SupportedToolsData.ReportInconsistentDataException(Strings.SupportedToolsDataInvalidToolVersionRange(toolInfoData.id.ToString()));
				}
				if (toolInfoData.customSupportedVersion != null)
				{
					foreach (CustomSupportedVersion customSupportedVersion2 in toolInfoData.customSupportedVersion)
					{
						if (!SupportedToolsData.IsValidVersionRange(customSupportedVersion2.minSupportedVersion, customSupportedVersion2.latestVersion))
						{
							SupportedToolsData.ReportInconsistentDataException(Strings.SupportedToolsDataInvalidToolVersionRange(toolInfoData.id.ToString()));
						}
						if (!SupportedToolsData.IsValidVersionRange(customSupportedVersion2.minTenantVersion, customSupportedVersion2.maxTenantVersion))
						{
							SupportedToolsData.ReportInconsistentDataException(Strings.SupportedToolsDataInvalidTenantVersionRange(toolInfoData.id.ToString()));
						}
					}
					if (SupportedToolsData.ContainsOverlappingVersionRanges(toolInfoData.customSupportedVersion))
					{
						SupportedToolsData.ReportInconsistentDataException(Strings.SupportedToolsDataOverlappingTenantVersionRanges(toolInfoData.id.ToString()));
					}
				}
			}
		}

		private static bool ContainsOverlappingVersionRanges(CustomSupportedVersion[] versions)
		{
			List<CustomSupportedVersion> list = new List<CustomSupportedVersion>();
			foreach (CustomSupportedVersion customSupportedVersion in versions)
			{
				if (SupportedToolsData.Overlap(customSupportedVersion, list))
				{
					return true;
				}
				list.Add(customSupportedVersion);
			}
			return false;
		}

		private static bool Overlap(CustomSupportedVersion newRange, IEnumerable<CustomSupportedVersion> existingRanges)
		{
			foreach (CustomSupportedVersion range in existingRanges)
			{
				if (SupportedToolsData.Overlap(newRange, range))
				{
					return true;
				}
			}
			return false;
		}

		private static bool Overlap(CustomSupportedVersion range1, CustomSupportedVersion range2)
		{
			Version version = SupportedToolsData.GetVersion(range1.minTenantVersion, SupportedToolsData.MinimumVersion);
			Version version2 = SupportedToolsData.GetVersion(range1.maxTenantVersion, SupportedToolsData.MaximumVersion);
			Version version3 = SupportedToolsData.GetVersion(range2.minTenantVersion, SupportedToolsData.MinimumVersion);
			Version version4 = SupportedToolsData.GetVersion(range2.maxTenantVersion, SupportedToolsData.MaximumVersion);
			return version2 >= version3 && version <= version4;
		}

		private static bool IsValidVersionRange(string minVersionString, string maxVersionString)
		{
			Version version = SupportedToolsData.GetVersion(minVersionString, SupportedToolsData.MinimumVersion);
			Version version2 = SupportedToolsData.GetVersion(maxVersionString, SupportedToolsData.MaximumVersion);
			return version <= version2;
		}

		private void ValidateDuplicatedToolEntries()
		{
			HashSet<ToolId> hashSet = new HashSet<ToolId>();
			foreach (ToolInfoData toolInfoData in this.toolData.toolInformation)
			{
				if (hashSet.Contains(toolInfoData.id))
				{
					SupportedToolsData.ReportInconsistentDataException(Strings.SupportedToolsDataMultipleToolEntries(toolInfoData.id.ToString()));
				}
				else
				{
					hashSet.Add(toolInfoData.id);
				}
			}
		}

		private static void ReportInconsistentDataException(LocalizedString errorMessage)
		{
			throw new InvalidDataException(errorMessage);
		}

		private static void HandleDataReadException(Exception e)
		{
			throw new FormatException(Strings.SupportedToolsCannotReadFile, e);
		}

		internal static Version GetVersion(string versionString, Version defaultValue)
		{
			if (string.IsNullOrEmpty(versionString))
			{
				return defaultValue;
			}
			return new Version(versionString);
		}

		private const int MaxVersionValue = 999999999;

		internal static readonly Version MinimumVersion = new Version(0, 0, 0, 0);

		internal static readonly Version MaximumVersion = new Version(999999999, 999999999, 999999999, 999999999);

		private readonly string dataFile;

		private readonly string schemaFile;

		private supportedTools toolData;
	}
}
