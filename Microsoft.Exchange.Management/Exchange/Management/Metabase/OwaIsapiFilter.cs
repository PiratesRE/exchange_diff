using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Management.ClientAccess;
using Microsoft.Exchange.Management.Clients;
using Microsoft.Exchange.Management.IisTasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Metabase
{
	internal static class OwaIsapiFilter
	{
		private static void AllowIsapiExtension(DirectoryEntry virtualDirectory, string groupId)
		{
			string iisserverName = IsapiFilterCommon.GetIISServerName(virtualDirectory);
			OwaIsapiFilter.AllowIsapiExtension(iisserverName, groupId);
		}

		private static void AllowIsapiExtension(string hostName, string groupId)
		{
			ManageIsapiExtensions.SetStatus(hostName, groupId, OwaIsapiFilter.extensionBinary, true);
		}

		private static void ProhibitIsapiExtension(string hostName, string groupId)
		{
			ManageIsapiExtensions.SetStatus(hostName, groupId, OwaIsapiFilter.extensionBinary, false);
		}

		private static void RemoveFilter(string adsiWebSitePath)
		{
			IsapiFilter.RemoveIsapiFilter(adsiWebSitePath, OwaIsapiFilter.filterName);
		}

		internal static void RemoveFilters(string hostName)
		{
			string iisDirectoryEntryPath = IisUtility.CreateAbsolutePath(IisUtility.AbsolutePathType.WebServicesRoot, hostName, null, null);
			using (DirectoryEntry directoryEntry = IisUtility.CreateIISDirectoryEntry(iisDirectoryEntryPath))
			{
				foreach (object obj in directoryEntry.Children)
				{
					DirectoryEntry directoryEntry2 = (DirectoryEntry)obj;
					if (directoryEntry2.SchemaClassName == "IIsWebServer")
					{
						OwaIsapiFilter.RemoveFilter(directoryEntry2.Path);
					}
				}
			}
		}

		private static bool IsFbaEnabled(string server, string localMetabasePath)
		{
			IMSAdminBase iisAdmin = IMSAdminBaseHelper.Create(server);
			string metabasePath = "/LM" + localMetabasePath;
			OwaIsapiFilter.FormsAuthPropertyFlags formsAuthPropertyFlags;
			int flags = OwaIsapiFilter.GetFlags(iisAdmin, metabasePath, out formsAuthPropertyFlags);
			if (flags == -2146646015)
			{
				return false;
			}
			if (flags == -2147024891)
			{
				throw new OwaIsapiFilterException(Strings.FormsAuthenticationIsEnabledAccessDeniedException(metabasePath, 45054), flags);
			}
			if (flags == -2147024893)
			{
				throw new OwaIsapiFilterException(Strings.FormsAuthenticationIsEnabledPathNotFoundException(metabasePath, 45054), flags);
			}
			if (flags < 0)
			{
				throw new OwaIsapiFilterException(Strings.FormsAuthenticationIsEnabledUnknownErrorException(metabasePath, 45054), flags);
			}
			return (formsAuthPropertyFlags & OwaIsapiFilter.FormsAuthPropertyFlags.FbaEnabled) != OwaIsapiFilter.FormsAuthPropertyFlags.None;
		}

		private static MetadataRecord CreateFormsRecord(MBAttributes attributes)
		{
			return new MetadataRecord(4)
			{
				Identifier = MBIdentifier.FormsAuthenticationEnabledProperty,
				Attributes = attributes,
				UserType = MBUserType.Server,
				DataType = MBDataType.Dword,
				DataTag = 0
			};
		}

		private static int GetFlags(IMSAdminBase iisAdmin, string metabasePath, out OwaIsapiFilter.FormsAuthPropertyFlags flags)
		{
			flags = OwaIsapiFilter.FormsAuthPropertyFlags.None;
			int result = 0;
			MetadataRecord metadataRecord = OwaIsapiFilter.CreateFormsRecord(MBAttributes.None);
			using (metadataRecord)
			{
				int num;
				result = iisAdmin.GetData(SafeMetadataHandle.MetadataMasterRootHandle, metabasePath, metadataRecord, out num);
				flags = (OwaIsapiFilter.FormsAuthPropertyFlags)Marshal.ReadInt32(metadataRecord.DataBuf.DangerousGetHandle());
			}
			return result;
		}

		private static void SetFlags(string server, string path, OwaIsapiFilter.FormsAuthPropertyFlags flags)
		{
			IMSAdminBase imsadminBase = IMSAdminBaseHelper.Create(server);
			string text = "/LM" + path;
			SafeMetadataHandle safeMetadataHandle;
			int num = IMSAdminBaseHelper.OpenKey(imsadminBase, SafeMetadataHandle.MetadataMasterRootHandle, text, MBKeyAccess.Write, 15000, out safeMetadataHandle);
			using (safeMetadataHandle)
			{
				if (num == -2147024748)
				{
					throw new FormsAuthenticationErrorPathBusyException(text);
				}
				if (num == -2147024893)
				{
					throw new FormsAuthenticationMarkPathErrorPathNotFoundException(text);
				}
				if (num < 0)
				{
					throw new OwaIsapiFilterException(Strings.FormsAuthenticationMarkPathErrorUnknownOpenError(text), num);
				}
				MetadataRecord metadataRecord = OwaIsapiFilter.CreateFormsRecord(MBAttributes.Inherit);
				using (metadataRecord)
				{
					Marshal.WriteInt32(metadataRecord.DataBuf.DangerousGetHandle(), (int)flags);
					num = imsadminBase.SetData(safeMetadataHandle, string.Empty, metadataRecord);
				}
				if (num == -2147024891)
				{
					throw new FormsAuthenticationMarkPathAccessDeniedException(text, 45054);
				}
				if (num == -2147024888)
				{
					throw new OutOfMemoryException();
				}
				if (num == -2147024893)
				{
					throw new FormsAuthenticationMarkPathErrorPathNotFoundException(text);
				}
				if (num == -2146646008)
				{
					throw new FormsAuthenticationMarkPathCannotMarkSecureAttributeException(text, 45054);
				}
				if (num < 0)
				{
					throw new FormsAuthenticationMarkPathUnknownSetError(text, 45054, num);
				}
			}
			num = IisUtility.CommitMetabaseChanges(server);
			if (num < 0)
			{
				throw new OwaIsapiFilterException(Strings.CommitMetabaseChangesException(server), num);
			}
		}

		private static int GetMarkedPathCount(string server, string webSitePath)
		{
			int num = 0;
			IMSAdminBase iisAdmin = IMSAdminBaseHelper.Create(server);
			webSitePath = "/LM" + webSitePath;
			List<string> list = new List<string>();
			int num2 = IMSAdminBaseHelper.GetDataPaths(iisAdmin, webSitePath, MBIdentifier.FormsAuthenticationEnabledProperty, MBDataType.Dword, ref list);
			if (num2 == -2147024893)
			{
				throw new OwaIsapiFilterException(Strings.FormsAuthenticationDeleteMarksIfUnusedPathNotFoundException(webSitePath), num2);
			}
			if (num2 < 0)
			{
				throw new OwaIsapiFilterException(Strings.FormsAuthenticationDeleteMarksIfUnusedUnknownErrorException(webSitePath, 45054), num2);
			}
			int[] array = new int[list.Count];
			for (int i = 0; i < list.Count; i++)
			{
				OwaIsapiFilter.FormsAuthPropertyFlags formsAuthPropertyFlags;
				num2 = OwaIsapiFilter.GetFlags(iisAdmin, list[i], out formsAuthPropertyFlags);
				if (num2 == -2147024891)
				{
					throw new OwaIsapiFilterException(Strings.FormsAuthenticationDeleteMarksIfUnusedCheckMarkAccessDeniedException(list[i]), num2);
				}
				if (num2 != -2147024893 && num2 != -2146646015)
				{
					if (num2 < 0)
					{
						throw new OwaIsapiFilterException(Strings.FormsAuthenticationDeleteMarksIfUnusedUnknownCheckErrorException(list[i]), num2);
					}
					num++;
				}
			}
			if (num == 1 && string.Compare(list[array[0]], webSitePath, true, CultureInfo.InvariantCulture) == 0)
			{
				num = 0;
			}
			return num;
		}

		public static void Install(DirectoryEntry virtualDirectory)
		{
			OwaIsapiFilter.AllowIsapiExtension(virtualDirectory, "MSExchangeClientAccess");
			IsapiFilterCommon.CreateFilter(virtualDirectory, OwaIsapiFilter.filterName, OwaIsapiFilter.filterDirectory, OwaIsapiFilter.extensionBinary);
		}

		public static void InstallForCafe(DirectoryEntry virtualDirectory)
		{
			OwaIsapiFilter.AllowIsapiExtension(virtualDirectory, "MSExchangeCafe");
			IsapiFilterCommon.CreateFilter(virtualDirectory, OwaIsapiFilter.filterName, OwaIsapiFilter.cafeFilterDirectory, OwaIsapiFilter.cafeExtensionBinary);
		}

		public static void UninstallIfLastVdir(DirectoryEntry virtualDirectory)
		{
			string iisserverName = IsapiFilterCommon.GetIISServerName(virtualDirectory);
			string iislocalPath = IsapiFilterCommon.GetIISLocalPath(virtualDirectory);
			string text = null;
			string text2 = null;
			string text3 = null;
			IisUtility.ParseApplicationRootPath(iislocalPath, ref text, ref text2, ref text3);
			if (OwaIsapiFilter.GetMarkedPathCount(iisserverName, text2) <= 1)
			{
				OwaIsapiFilter.RemoveFilter("IIS://" + iisserverName + text2);
			}
		}

		public static bool IsFbaEnabled(DirectoryEntry virtualDirectory)
		{
			string iisserverName = IsapiFilterCommon.GetIISServerName(virtualDirectory);
			string iislocalPath = IsapiFilterCommon.GetIISLocalPath(virtualDirectory);
			return OwaIsapiFilter.IsFbaEnabled(iisserverName, iislocalPath);
		}

		public static void EnableFba(DirectoryEntry virtualDirectory)
		{
			string iisserverName = IsapiFilterCommon.GetIISServerName(virtualDirectory);
			string iislocalPath = IsapiFilterCommon.GetIISLocalPath(virtualDirectory);
			OwaIsapiFilter.SetFlags(iisserverName, iislocalPath, OwaIsapiFilter.FormsAuthPropertyFlags.FbaEnabled);
		}

		public static void DisableFba(DirectoryEntry virtualDirectory)
		{
			string iisserverName = IsapiFilterCommon.GetIISServerName(virtualDirectory);
			string iislocalPath = IsapiFilterCommon.GetIISLocalPath(virtualDirectory);
			OwaIsapiFilter.SetFlags(iisserverName, iislocalPath, OwaIsapiFilter.FormsAuthPropertyFlags.None);
		}

		private const string metabaseRoot = "/LM";

		private const string adsiPrefix = "IIS://";

		private const int timeoutValue = 15000;

		private const int extensionIndex = 0;

		private const int cafeExtensionIndex = 0;

		private static readonly string extensionBinary = Microsoft.Exchange.Management.ClientAccess.IisWebServiceExtension.AllExtensions[0].ExecutableName;

		private static readonly string filterName = "Exchange OWA Cookie Authentication ISAPI Filter";

		private static readonly string filterDirectory = Microsoft.Exchange.Management.ClientAccess.IisWebServiceExtension.AllExtensions[0].RelativePath;

		private static readonly string cafeExtensionBinary = CafeIisWebServiceExtension.AllExtensions[0].ExecutableName;

		private static readonly string cafeFilterDirectory = CafeIisWebServiceExtension.AllExtensions[0].RelativePath;

		[Flags]
		private enum FormsAuthPropertyFlags
		{
			None = 0,
			FbaEnabled = 1
		}
	}
}
