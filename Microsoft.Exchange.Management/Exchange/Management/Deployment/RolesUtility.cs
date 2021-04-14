using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RolesUtility
	{
		private RolesUtility()
		{
		}

		public static Version GetUnpackedVersion(string roleName)
		{
			Version result = null;
			string keyName = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\" + RolesUtility.GetRoleKeyByName(roleName, false);
			string text = (string)Registry.GetValue(keyName, "UnpackedVersion", null);
			if (text == null)
			{
				keyName = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\" + RolesUtility.GetRoleKeyByName(roleName, true);
				text = (string)Registry.GetValue(keyName, "UnpackedVersion", null);
			}
			if (text != null)
			{
				result = new Version(text);
			}
			return result;
		}

		public static Version GetUnpackedDatacenterVersion(string roleName)
		{
			Version result = null;
			string keyName = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\" + RolesUtility.GetRoleKeyByName(roleName, false);
			string text = (string)Registry.GetValue(keyName, "UnpackedDatacenterVersion", null);
			if (text != null)
			{
				result = new Version(text);
			}
			return result;
		}

		public static string GetRoleKeyByName(string roleName, bool legacyRequested)
		{
			string result = roleName;
			if (!legacyRequested && roleName != null)
			{
				if (!(roleName == "BridgeheadRole"))
				{
					if (roleName == "GatewayRole")
					{
						result = "EdgeTransportRole";
					}
				}
				else
				{
					result = "HubTransportRole";
				}
			}
			if (roleName == "AdminToolsRole")
			{
				result = "AdminTools";
			}
			return result;
		}

		public static void GetConfiguringStatus(ref ConfigurationStatus status)
		{
			string name = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\" + RolesUtility.GetRoleKeyByName(status.Role, false);
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name))
			{
				if (registryKey != null)
				{
					InstallationModes action = InstallationModes.Unknown;
					string text = (string)registryKey.GetValue("Action", null);
					if (text != null)
					{
						action = (InstallationModes)Enum.Parse(typeof(InstallationModes), text);
					}
					status.Action = action;
					object value = registryKey.GetValue("Watermark", null);
					status.Watermark = ((value != null) ? value.ToString() : null);
				}
			}
		}

		public static void SetConfiguringStatus(ConfigurationStatus status)
		{
			string keyName = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\" + RolesUtility.GetRoleKeyByName(status.Role, false);
			Registry.SetValue(keyName, "Action", status.Action);
			Registry.SetValue(keyName, "Watermark", status.Watermark);
		}

		public static void ClearConfiguringStatus(ConfigurationStatus status)
		{
			string name = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\" + RolesUtility.GetRoleKeyByName(status.Role, false);
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name, true);
			if (registryKey != null)
			{
				registryKey.DeleteValue("Action", false);
				registryKey.DeleteValue("Watermark", false);
				if (registryKey.SubKeyCount == 0 && registryKey.ValueCount == 0)
				{
					registryKey.Close();
					string name2 = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\";
					RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey(name2, true);
					registryKey2.DeleteSubKey(RolesUtility.GetRoleKeyByName(status.Role, false), false);
					registryKey2.Close();
					return;
				}
				registryKey.Close();
			}
		}

		public static void SetPostSetupVersion(string roleName, Version version)
		{
			string keyName = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\" + RolesUtility.GetRoleKeyByName(roleName, false);
			string text = (string)Registry.GetValue(keyName, "PostSetupVersion", "<unset>");
			TaskLogger.Trace("Updating postsetup version from {0} to {1}", new object[]
			{
				text,
				version
			});
			Registry.SetValue(keyName, "PostSetupVersion", (version == null) ? "" : version.ToString());
		}

		public static Version GetPostSetupVersion(string roleName)
		{
			string keyName = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\" + RolesUtility.GetRoleKeyByName(roleName, false);
			string text = (string)Registry.GetValue(keyName, "PostSetupVersion", null);
			Version result = null;
			if (text != null && text != "")
			{
				result = new Version(text);
			}
			return result;
		}

		public static Version GetConfiguredVersion(string roleName)
		{
			Version result = null;
			string keyName = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\" + RolesUtility.GetRoleKeyByName(roleName, false);
			string text = (string)Registry.GetValue(keyName, "ConfiguredVersion", null);
			if (text == null)
			{
				keyName = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\" + RolesUtility.GetRoleKeyByName(roleName, true);
				text = (string)Registry.GetValue(keyName, "ConfiguredVersion", null);
			}
			if (text != null)
			{
				result = new Version(text);
			}
			else if (roleName == "AdminToolsRole")
			{
				Version unpackedVersion = RolesUtility.GetUnpackedVersion(roleName);
				if (unpackedVersion != null && unpackedVersion < AdminToolsRole.FirstConfiguredVersion)
				{
					result = unpackedVersion;
				}
			}
			return result;
		}

		public static void SetConfiguredVersion(string roleName, Version configuredVersion)
		{
			string keyName = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\" + RolesUtility.GetRoleKeyByName(roleName, false);
			string text = (string)Registry.GetValue(keyName, "ConfiguredVersion", "<unset>");
			TaskLogger.Trace("Updating configured version from {0} to {1}", new object[]
			{
				text,
				configuredVersion
			});
			Registry.SetValue(keyName, "ConfiguredVersion", configuredVersion.ToString());
			if (RolesUtility.GetPostSetupVersion(roleName) == null)
			{
				RolesUtility.SetPostSetupVersion(roleName, new Version("0.0.0.0"));
			}
			using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Wow6432Node\\Microsoft\\ExchangeServer\\v15\\" + RolesUtility.GetRoleKeyByName(roleName, false)))
			{
				registryKey.SetValue("ConfiguredVersion", configuredVersion.ToString());
			}
		}

		public static void DeleteConfiguredVersion(string roleName)
		{
			string name = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\" + RolesUtility.GetRoleKeyByName(roleName, false);
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(name, true);
			registryKey.DeleteValue("ConfiguredVersion", false);
			registryKey.DeleteValue("PostSetupVersion", false);
			registryKey.Close();
			try
			{
				Registry.LocalMachine.DeleteSubKeyTree("SOFTWARE\\Wow6432Node\\Microsoft\\ExchangeServer\\v15\\" + RolesUtility.GetRoleKeyByName(roleName, false));
			}
			catch (ArgumentException)
			{
			}
		}

		public static ParameterCollection ReadSetupParameters(bool isDatacenter)
		{
			string text = null;
			text = (Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeLabs", "ParametersFile", null) as string);
			if (string.IsNullOrEmpty(text))
			{
				if (isDatacenter)
				{
					throw new RegistryValueMissingOrInvalidException("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeLabs", "ParametersFile");
				}
				text = Path.Combine(Role.SetupComponentInfoFilePath, "bin\\EnterpriseServiceEndpointsConfig.xml");
			}
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.ConformanceLevel = ConformanceLevel.Document;
			xmlReaderSettings.ValidationType = ValidationType.Schema;
			Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("parameter.xsd");
			xmlReaderSettings.Schemas.Add(null, XmlReader.Create(manifestResourceStream));
			ParameterCollection parameterCollection = null;
			using (XmlReader xmlReader = XmlReader.Create(text, xmlReaderSettings))
			{
				try
				{
					parameterCollection = (ParameterCollection)RolesUtility.GetParameterSerializer().Deserialize(xmlReader);
				}
				catch (InvalidOperationException ex)
				{
					throw new XmlDeserializationException(text, ex.Message, (ex.InnerException == null) ? string.Empty : ex.InnerException.Message, ex);
				}
			}
			TaskLogger.Log(Strings.LoadedParameters(text, parameterCollection.Count));
			return parameterCollection;
		}

		public static SetupComponentInfo ReadSetupComponentInfoFile(string fileName)
		{
			SetupComponentInfo setupComponentInfo = null;
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.ConformanceLevel = ConformanceLevel.Document;
			xmlReaderSettings.ValidationType = ValidationType.Schema;
			Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("component.xsd");
			xmlReaderSettings.Schemas.Add(null, XmlReader.Create(manifestResourceStream));
			string text;
			using (XmlReader xmlReader = RolesUtility.CreateXmlReader(fileName, xmlReaderSettings, out text))
			{
				try
				{
					setupComponentInfo = (SetupComponentInfo)RolesUtility.GetComponentSerializer().Deserialize(xmlReader);
					setupComponentInfo.PopulateTasksProperty(Path.GetFileNameWithoutExtension(fileName));
					setupComponentInfo.ValidateDatacenterAttributes();
				}
				catch (InvalidOperationException ex)
				{
					throw new XmlDeserializationException(text, ex.Message, (ex.InnerException == null) ? string.Empty : ex.InnerException.Message);
				}
				TaskLogger.Log(Strings.LoadedComponentWithTasks(setupComponentInfo.Name, setupComponentInfo.Tasks.Count, text));
			}
			return setupComponentInfo;
		}

		public static XmlReader CreateXmlReader(string fileName, XmlReaderSettings settings, out string usedResourceName)
		{
			if (File.Exists(fileName))
			{
				usedResourceName = fileName;
				return XmlReader.Create(fileName, settings);
			}
			string fileName2 = Path.GetFileName(fileName);
			Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName2);
			usedResourceName = "res://" + fileName2;
			return XmlReader.Create(manifestResourceStream, settings);
		}

		public static XmlSerializer GetParameterSerializer()
		{
			if (RolesUtility.serializerParameter == null)
			{
				lock (RolesUtility.serializerLock)
				{
					if (RolesUtility.serializerParameter == null)
					{
						RolesUtility.serializerParameter = new XmlSerializer(typeof(ParameterCollection));
					}
				}
			}
			return RolesUtility.serializerParameter;
		}

		public static XmlSerializer GetComponentSerializer()
		{
			if (RolesUtility.serializerComponent == null)
			{
				lock (RolesUtility.serializerLock)
				{
					if (RolesUtility.serializerComponent == null)
					{
						RolesUtility.serializerComponent = new SetupComponentInfoSerializer();
					}
				}
			}
			return RolesUtility.serializerComponent;
		}

		private static XmlSerializer serializerParameter;

		private static SetupComponentInfoSerializer serializerComponent;

		private static object serializerLock = new object();
	}
}
