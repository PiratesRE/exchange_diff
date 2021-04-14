using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Xml;
using System.Xml.XPath;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Update", "AspnetConfig")]
	public sealed class UpdateAspnetConfig : Task
	{
		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			string[] configFilePaths = this.GetConfigFilePaths();
			if (configFilePaths == null || configFilePaths.Length == 0)
			{
				base.WriteError(new LocalizedException(Strings.AspnetConfigFileNotFound), ErrorCategory.ObjectNotFound, null);
				return;
			}
			bool flag = false;
			foreach (string filePath in configFilePaths)
			{
				flag |= this.UpdateSetting(filePath);
			}
			if (!flag)
			{
				base.WriteError(new LocalizedException(Strings.UpdateAspnetConfigFailed), ErrorCategory.WriteError, null);
				return;
			}
			TaskLogger.LogExit();
		}

		private string[] GetConfigFilePaths()
		{
			RegistryKey registryKey2;
			RegistryKey registryKey = registryKey2 = Registry.LocalMachine.OpenSubKey(UpdateAspnetConfig.aspnetRootKey, false);
			string[] result;
			try
			{
				if (registryKey == null)
				{
					result = null;
				}
				else
				{
					string[] subKeyNames = registryKey.GetSubKeyNames();
					if (subKeyNames == null || subKeyNames.Length == 0)
					{
						result = null;
					}
					else
					{
						result = this.GetPathsFromSubKeys(registryKey, subKeyNames);
					}
				}
			}
			finally
			{
				if (registryKey2 != null)
				{
					((IDisposable)registryKey2).Dispose();
				}
			}
			return result;
		}

		private string[] GetPathsFromSubKeys(RegistryKey rootKey, string[] subKeyNames)
		{
			List<string> list = new List<string>();
			foreach (string text in subKeyNames)
			{
				if (text.StartsWith(UpdateAspnetConfig.version2))
				{
					using (RegistryKey registryKey = rootKey.OpenSubKey(text, false))
					{
						if (registryKey != null)
						{
							object value = registryKey.GetValue(UpdateAspnetConfig.pathKey);
							if (value != null && value is string && !string.IsNullOrEmpty((string)value))
							{
								list.Add(Path.Combine((string)value, UpdateAspnetConfig.filename));
							}
						}
					}
				}
			}
			return list.ToArray();
		}

		private bool UpdateSetting(string filePath)
		{
			Exception ex = null;
			bool result = false;
			try
			{
				SafeXmlDocument safeXmlDocument = new SafeXmlDocument();
				safeXmlDocument.Load(filePath);
				XmlNode documentElement = safeXmlDocument.DocumentElement;
				if (documentElement == null)
				{
					TaskLogger.Trace("The aspnet.config file at {0} does not contain a root element. Skipping the file.", new object[]
					{
						filePath
					});
					return result;
				}
				XmlNode xmlNode = documentElement.SelectSingleNode("descendant::runtime/legacyImpersonationPolicy");
				if (xmlNode == null || xmlNode.Attributes == null || xmlNode.Attributes.Count <= 0)
				{
					TaskLogger.Trace("The aspnet.config file at {0} does not contain a valid legacyImpersonationPolicy setting. Skipping the file.", new object[]
					{
						filePath
					});
					return result;
				}
				xmlNode.Attributes[0].Value = "false";
				safeXmlDocument.Save(filePath);
				TaskLogger.Trace("Successfully changed the legacyImpersonationPolicy setting to false in aspnet.config file at {0}.", new object[]
				{
					filePath
				});
				result = true;
			}
			catch (XPathException ex2)
			{
				ex = ex2;
			}
			catch (XmlException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				TaskLogger.Trace("Failed to parse the aspnet.config file at {0}. Skipping the file. Exception: {1}.", new object[]
				{
					filePath,
					ex
				});
			}
			return result;
		}

		private static readonly string aspnetRootKey = "Software\\Microsoft\\ASP.NET\\";

		private static readonly string version2 = "2.";

		private static readonly string pathKey = "Path";

		private static readonly string filename = "aspnet.config";
	}
}
