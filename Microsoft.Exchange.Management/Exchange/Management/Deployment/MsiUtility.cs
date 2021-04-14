using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class MsiUtility
	{
		public static Guid GetProductCode(string packagePath)
		{
			TaskLogger.LogEnter();
			StringBuilder stringBuilder = new StringBuilder();
			Guid guid = Guid.Empty;
			MsiUtility.PushUILevel(InstallUILevel.None);
			try
			{
				SafeMsiHandle safeMsiHandle;
				uint num = MsiNativeMethods.OpenPackageEx(packagePath, OpenPackageFlags.IgnoreMachineState, out safeMsiHandle);
				if (num != 0U)
				{
					Win32Exception ex = new Win32Exception((int)num);
					throw new TaskException(Strings.MsiCouldNotOpenPackage(packagePath, ex.Message, (int)num), ex);
				}
				using (safeMsiHandle)
				{
					uint num2 = 38U;
					for (;;)
					{
						num2 += 1U;
						if (num2 > 2147483647U)
						{
							break;
						}
						stringBuilder.EnsureCapacity((int)num2);
						num = MsiNativeMethods.GetProductProperty(safeMsiHandle, "ProductCode", stringBuilder, ref num2);
						if (234U != num)
						{
							goto Block_7;
						}
					}
					throw new TaskException(Strings.MsiPropertyTooLarge);
					Block_7:
					if (num != 0U)
					{
						Win32Exception ex2 = new Win32Exception((int)num);
						throw new TaskException(Strings.MsiCouldNotGetProdcutProperty("ProductCode", ex2.Message, (int)num), ex2);
					}
					guid = new Guid(stringBuilder.ToString());
					TaskLogger.Log(Strings.MsiProductCode(guid));
				}
			}
			finally
			{
				MsiUtility.PopUILevel();
			}
			TaskLogger.LogExit();
			return guid;
		}

		public static bool IsInstalled(string packagePath)
		{
			TaskLogger.LogEnter();
			if (packagePath == null || packagePath == string.Empty)
			{
				throw new ArgumentNullException("packagePath", Strings.ExceptionProductInfoRequired);
			}
			Guid productCode = MsiUtility.GetProductCode(packagePath);
			bool result = MsiUtility.IsInstalled(productCode);
			TaskLogger.LogExit();
			return result;
		}

		public static bool IsInstalled(Guid ProductCode)
		{
			TaskLogger.LogEnter();
			bool result = false;
			if (Guid.Empty == ProductCode)
			{
				throw new ArgumentNullException("ProductCode", Strings.ExceptionProductInfoRequired);
			}
			MsiUtility.PushUILevel(InstallUILevel.None);
			try
			{
				InstallState installState = MsiNativeMethods.QueryProductState(ProductCode.ToString("B").ToUpper(CultureInfo.InvariantCulture));
				result = (installState != InstallState.Unknown);
			}
			finally
			{
				MsiUtility.PopUILevel();
			}
			TaskLogger.LogExit();
			return result;
		}

		public static string GetProductInfo(Guid productCode, string propertyName)
		{
			TaskLogger.LogEnter();
			StringBuilder stringBuilder = new StringBuilder();
			string productCodeString = productCode.ToString("B").ToUpper(CultureInfo.InvariantCulture);
			uint num = (uint)stringBuilder.Capacity;
			MsiUtility.PushUILevel(InstallUILevel.None);
			try
			{
				uint productInfo;
				for (;;)
				{
					num += 1U;
					if (num > 2147483647U)
					{
						break;
					}
					stringBuilder.EnsureCapacity((int)num);
					productInfo = MsiNativeMethods.GetProductInfo(productCodeString, propertyName, stringBuilder, ref num);
					if (234U != productInfo)
					{
						goto Block_3;
					}
				}
				throw new TaskException(Strings.MsiPropertyTooLarge);
				Block_3:
				if (productInfo != 0U)
				{
					Win32Exception ex = new Win32Exception((int)productInfo);
					throw new TaskException(Strings.MsiCouldNotGetProdcutProperty(propertyName, ex.Message, (int)productInfo), ex);
				}
			}
			finally
			{
				MsiUtility.PopUILevel();
			}
			TaskLogger.Log(Strings.MsiProperty(propertyName, stringBuilder.ToString()));
			TaskLogger.LogExit();
			return stringBuilder.ToString();
		}

		private static void PushUILevel(InstallUILevel uiLevel, ref IntPtr window)
		{
			TaskLogger.LogEnter();
			InstallUILevel installUILevel = MsiNativeMethods.SetInternalUI(uiLevel, ref window);
			if (installUILevel == InstallUILevel.NoChange)
			{
				throw new ArgumentOutOfRangeException("uiLevel", installUILevel, Strings.ExceptionInvalidUILevel);
			}
			MsiUtility.uiSettings.Push(new MsiUtility.InternalUISettings(uiLevel, window, null));
			TaskLogger.LogExit();
		}

		public static void PushUILevel(InstallUILevel uiLevel)
		{
			TaskLogger.LogEnter();
			IntPtr zero = IntPtr.Zero;
			MsiUtility.PushUILevel(uiLevel, ref zero);
			TaskLogger.LogExit();
		}

		public static void PopUILevel()
		{
			TaskLogger.LogEnter();
			MsiUtility.InternalUISettings internalUISettings = (MsiUtility.InternalUISettings)MsiUtility.uiSettings.Pop();
			MsiNativeMethods.SetInternalUI(internalUISettings.UILevel, ref internalUISettings.Window);
			TaskLogger.LogExit();
		}

		public static void PushExternalUI(MsiUIHandler handler, InstallLogMode logMode)
		{
			TaskLogger.LogEnter();
			IntPtr zero = IntPtr.Zero;
			InstallUILevel installUILevel = MsiNativeMethods.SetInternalUI(InstallUILevel.None | InstallUILevel.SourceResOnly, ref zero);
			if (installUILevel == InstallUILevel.NoChange)
			{
				throw new ArgumentOutOfRangeException("uiLevel", installUILevel, Strings.ExceptionInvalidUILevel);
			}
			MsiUIHandlerDelegate handlerDelegate = MsiNativeMethods.SetExternalUI(handler.UIHandlerDelegate, logMode, null);
			MsiUtility.uiSettings.Push(new MsiUtility.InternalUISettings(InstallUILevel.None | InstallUILevel.SourceResOnly, zero, handlerDelegate));
		}

		public static void PopExternalUI()
		{
			TaskLogger.LogEnter();
			MsiUtility.InternalUISettings internalUISettings = (MsiUtility.InternalUISettings)MsiUtility.uiSettings.Pop();
			MsiNativeMethods.SetExternalUI(internalUISettings.UIHandlerDelegate, InstallLogMode.None, null);
			MsiNativeMethods.SetInternalUI(internalUISettings.UILevel, ref internalUISettings.Window);
			TaskLogger.LogExit();
		}

		private static Stack uiSettings = new Stack();

		private class InternalUISettings
		{
			public InternalUISettings(InstallUILevel uiLevel, IntPtr window, MsiUIHandlerDelegate handlerDelegate)
			{
				this.UILevel = uiLevel;
				this.Window = window;
				this.UIHandlerDelegate = handlerDelegate;
			}

			public InstallUILevel UILevel;

			public IntPtr Window;

			public MsiUIHandlerDelegate UIHandlerDelegate;
		}
	}
}
