using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ExtensionToContentTypeMapper
	{
		private ExtensionToContentTypeMapper()
		{
		}

		public static ExtensionToContentTypeMapper Instance
		{
			get
			{
				if (ExtensionToContentTypeMapper.instance == null)
				{
					lock (ExtensionToContentTypeMapper.lockContentType)
					{
						ExtensionToContentTypeMapper.instance = ExtensionToContentTypeMapper.Create();
					}
				}
				return ExtensionToContentTypeMapper.instance;
			}
		}

		public string GetContentTypeByExtension(string extension)
		{
			string result = null;
			if (this.contentTypeDictionary.TryGetValue(extension.ToLowerInvariant(), out result))
			{
				return result;
			}
			return "application/octet-stream";
		}

		public string GetExtensionByContentType(string contentType)
		{
			string result = null;
			if (this.extensionDictionary.TryGetValue(contentType.ToLowerInvariant(), out result))
			{
				return result;
			}
			return null;
		}

		private static ExtensionToContentTypeMapper Create()
		{
			if (ExtensionToContentTypeMapper.instance != null)
			{
				return ExtensionToContentTypeMapper.instance;
			}
			ExtensionToContentTypeMapper extensionToContentTypeMapper = new ExtensionToContentTypeMapper();
			extensionToContentTypeMapper.BuildContentTypesFromAD();
			extensionToContentTypeMapper.BuildContentTypesFromRegistry();
			return extensionToContentTypeMapper;
		}

		private static void AddNewEntry(Dictionary<string, string> dict, string key, string value)
		{
			if (!dict.ContainsKey(key))
			{
				dict.Add(key, value);
			}
		}

		private void BuildContentTypesFromAD()
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 133, "BuildContentTypesFromAD", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ExtensionToContentType.cs");
			Organization organization = null;
			try
			{
				organization = tenantOrTopologyConfigurationSession.GetOrgContainer();
				ExtensionToContentTypeMapper.organizationName = organization.Name.ToLowerInvariant();
			}
			catch (OrgContainerNotFoundException)
			{
				organization = null;
				ExtensionToContentTypeMapper.organizationName = string.Empty;
			}
			if (organization == null)
			{
				return;
			}
			foreach (string text in organization.MimeTypes)
			{
				int num = text.IndexOf(";");
				if (num >= 0)
				{
					string text2 = text.Substring(0, num).ToLowerInvariant();
					string text3 = text.Substring(num + 1).ToLowerInvariant();
					if (!(text2 == string.Empty) && !(text3 == string.Empty))
					{
						ExtensionToContentTypeMapper.AddNewEntry(this.contentTypeDictionary, text3, text2);
						ExtensionToContentTypeMapper.AddNewEntry(this.extensionDictionary, text2, text3);
					}
				}
			}
		}

		private void BuildContentTypesFromRegistry()
		{
			try
			{
				string[] subKeyNames = Registry.ClassesRoot.GetSubKeyNames();
				foreach (string text in subKeyNames)
				{
					if (text.StartsWith(".") && text.Length != 1)
					{
						string key = text.Substring(1).ToLowerInvariant();
						try
						{
							using (RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(text))
							{
								string value = registryKey.GetValue("Content Type") as string;
								if (!string.IsNullOrEmpty(value))
								{
									ExtensionToContentTypeMapper.AddNewEntry(this.contentTypeDictionary, key, value);
								}
							}
						}
						catch (IOException arg)
						{
							ExTraceGlobals.ContentTypeMappingTracer.TraceError<string, IOException>(0L, "ExtensionToContentTypeMapper::BuildContentTypeFromRegistry. I/O error opening the registry key {0}. Exception = {1}.", text, arg);
						}
						catch (SecurityException arg2)
						{
							ExTraceGlobals.ContentTypeMappingTracer.TraceError<string, SecurityException>(0L, "ExtensionToContentTypeMapper::BuildContentTypeFromRegistry. We are not allowed to open the registry key {0}. Exception = {1}.", text, arg2);
						}
					}
				}
			}
			catch (SecurityException ex)
			{
				ExTraceGlobals.ContentTypeMappingTracer.TraceError<SecurityException>(0L, "ExtensionToContentTypeMapper::BuildContentTypeFromRegistry. We are not allowed to open the registry keys. Exception = {0}.", ex);
				throw new RegistryContentTypeException(ex);
			}
		}

		public const string DefaultContentType = "application/octet-stream";

		private const string ContentTypeRegSubKey = "Content Type";

		private const int MaxOrganizationCount = 1000;

		private static string organizationName;

		private readonly Dictionary<string, string> contentTypeDictionary = new Dictionary<string, string>();

		private readonly Dictionary<string, string> extensionDictionary = new Dictionary<string, string>();

		private static ExtensionToContentTypeMapper instance = null;

		private static object lockContentType = new object();
	}
}
