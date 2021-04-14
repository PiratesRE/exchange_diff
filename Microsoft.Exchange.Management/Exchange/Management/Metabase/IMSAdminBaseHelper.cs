using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.Metabase
{
	internal class IMSAdminBaseHelper
	{
		internal static IMSAdminBase Create(string server)
		{
			if (server == null)
			{
				MSAdminBase msadminBase = new MSAdminBase();
				return (IMSAdminBase)msadminBase;
			}
			Type typeFromCLSID = Type.GetTypeFromCLSID(IMSAdminBaseHelper.MSAdminBaseGuid, server, false);
			IMSAdminBase imsadminBase = (IMSAdminBase)Activator.CreateInstance(typeFromCLSID);
			if (IMSAdminBaseHelper.CheckPermission(imsadminBase))
			{
				return imsadminBase;
			}
			object obj = null;
			if (COMHelper.Create(IMSAdminBaseHelper.MSAdminBaseGuid, IMSAdminBaseHelper.IIMSAdminBaseGuid, server, ref obj) == 0U)
			{
				return (IMSAdminBase)obj;
			}
			return imsadminBase;
		}

		internal static int OpenKey(IMSAdminBase adminBase, SafeMetadataHandle handle, string Path, MBKeyAccess AccessRequested, int TimeOut, out SafeMetadataHandle newHandle)
		{
			IntPtr handle2;
			int num = adminBase.OpenKey(handle, Path, AccessRequested, TimeOut, out handle2);
			if (num == 0)
			{
				newHandle = new SafeMetadataHandle(handle2, adminBase);
			}
			else
			{
				newHandle = null;
			}
			return num;
		}

		private static bool CheckPermission(IMSAdminBase iisAdmin)
		{
			MetadataRecord metadataRecord = new MetadataRecord(0);
			bool result;
			using (metadataRecord)
			{
				metadataRecord.Identifier = MBIdentifier.KeyType;
				metadataRecord.Attributes = MBAttributes.None;
				metadataRecord.UserType = MBUserType.Server;
				metadataRecord.DataType = MBDataType.String;
				SafeMetadataHandle key = new SafeMetadataHandle(IntPtr.Zero, null);
				int num;
				int data = iisAdmin.GetData(key, IMSAdminBaseHelper.W3SVC, metadataRecord, out num);
				result = (data != -2147024891);
			}
			return result;
		}

		internal static IMSAdminBase Create()
		{
			return IMSAdminBaseHelper.Create(null);
		}

		internal static int GetDataPaths(string searchPath, MBIdentifier propertyID, MBDataType dataType, ref List<string> paths)
		{
			IMSAdminBase iisAdmin = IMSAdminBaseHelper.Create();
			return IMSAdminBaseHelper.GetDataPaths(iisAdmin, searchPath, propertyID, dataType, ref paths);
		}

		internal static int GetDataPaths(IMSAdminBase iisAdmin, string searchPath, MBIdentifier propertyID, MBDataType dataType, ref List<string> paths)
		{
			int num = 4096;
			int num2 = 0;
			char[] array;
			int dataPaths;
			do
			{
				array = new char[num];
				dataPaths = iisAdmin.GetDataPaths(SafeMetadataHandle.MetadataMasterRootHandle, searchPath, propertyID, dataType, num, array, out num);
			}
			while (dataPaths == -2147024774 && num2 < 5);
			if (dataPaths == -2147024774 && num2 == 5)
			{
				throw new IMSAdminHelperGetDataPathsCouldNotAllocateException();
			}
			if (dataPaths == -2147024888)
			{
				throw new OutOfMemoryException();
			}
			if (dataPaths < 0)
			{
				return dataPaths;
			}
			int num3 = 0;
			while (array[num3] != '\0')
			{
				int num4 = num3;
				while (array[num4] != '\0')
				{
					num4++;
				}
				int num5 = num4 - num3;
				while (num5 > 1 && array[num3 + num5 - 1] == '/')
				{
					num5--;
				}
				paths.Add(new string(array, num3, num5));
				num3 = num4 + 1;
			}
			return 0;
		}

		internal static readonly Guid MSAdminBaseGuid = new Guid("a9e69610-b80d-11d0-b9b9-00a0c922e750");

		internal static readonly Guid IIMSAdminBaseGuid = new Guid("70b51430-b6ca-11d0-b9b9-00a0c922e750");

		internal static readonly string W3SVC = "/LM/W3SVC";
	}
}
