using System;
using System.Management;

namespace Microsoft.Exchange.Common.DiskManagement.Utilities
{
	public static class WMIUtil
	{
		public static int CallWMIMethod(ManagementObject encryptableVolume, string methodName, string[] inParamNameList, object[] inParamValueList, out ManagementBaseObject outParamsReturnValue)
		{
			ManagementBaseObject outParams = null;
			outParamsReturnValue = null;
			if (inParamNameList == null)
			{
				inParamNameList = new string[0];
			}
			if (inParamValueList == null)
			{
				inParamValueList = new string[0];
			}
			if (inParamNameList.Length != inParamValueList.Length)
			{
				throw new InvalidCallWMIMethodArgumentsException(inParamNameList, inParamValueList, inParamNameList.Length, inParamValueList.Length);
			}
			int returnValue = -1;
			Exception ex = Util.HandleExceptions(delegate
			{
				ManagementBaseObject methodParameters = encryptableVolume.GetMethodParameters(methodName);
				for (int i = 0; i < inParamNameList.Length; i++)
				{
					methodParameters[inParamNameList[i]] = inParamValueList[i];
				}
				outParams = encryptableVolume.InvokeMethod(methodName, methodParameters, null);
				returnValue = (int)uint.Parse(outParams["ReturnValue"].ToString());
			});
			outParamsReturnValue = outParams;
			Util.ThrowIfNotNull(ex);
			return returnValue;
		}

		public static ManagementObjectCollection GetManagementObjectCollection(string className, string pathName, out Exception ex)
		{
			ManagementObjectCollection managementObjectCollection = null;
			ex = Util.HandleExceptions(delegate
			{
				using (ManagementClass managementClass = new ManagementClass(new ManagementPath
				{
					NamespacePath = pathName,
					ClassName = className
				}))
				{
					managementClass.Get();
					managementObjectCollection = managementClass.GetInstances();
				}
			});
			return managementObjectCollection;
		}
	}
}
