using System;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MsiNativeMethods
	{
		private MsiNativeMethods()
		{
		}

		[DllImport("msi", CharSet = CharSet.Unicode, EntryPoint = "MsiInstallProductW")]
		internal static extern uint InstallProduct(string packagePath, string commandLine);

		[DllImport("msi", CharSet = CharSet.Unicode, EntryPoint = "MsiDetermineApplicablePatchesW")]
		internal static extern uint DetermineApplicablePatches(string packagePath, int count, [In] [Out] MsiNativeMethods.PatchSequenceInfo[] patches);

		[DllImport("msi", CharSet = CharSet.Unicode, EntryPoint = "MsiConfigureProductExW")]
		internal static extern uint ConfigureProduct(string productCodeString, InstallLevel installLevel, InstallState installState, string commandLine);

		[DllImport("msi", EntryPoint = "MsiSetInternalUI")]
		internal static extern InstallUILevel SetInternalUI(InstallUILevel uiLevel, [In] [Out] ref IntPtr window);

		[DllImport("msi", EntryPoint = "MsiSetExternalUI")]
		internal static extern MsiUIHandlerDelegate SetExternalUI([MarshalAs(UnmanagedType.FunctionPtr)] MsiUIHandlerDelegate handler, InstallLogMode filter, object context);

		[DllImport("msi", CharSet = CharSet.Unicode, EntryPoint = "MsiEnableLogW")]
		internal static extern uint EnableLog(InstallLogMode logMode, string logFile, InstallLogAttributes logAttributes);

		[DllImport("msi", CharSet = CharSet.Unicode, EntryPoint = "MsiGetProductInfoW")]
		internal static extern uint GetProductInfo(string productCodeString, string propertyName, StringBuilder propertyValue, ref uint propertyValueSize);

		[DllImport("msi", CharSet = CharSet.Unicode, EntryPoint = "MsiGetProductPropertyW")]
		internal static extern uint GetProductProperty(SafeMsiHandle product, string propertyName, StringBuilder propertyValue, ref uint propertyValueSize);

		[DllImport("msi", CharSet = CharSet.Unicode, EntryPoint = "MsiQueryProductStateW")]
		internal static extern InstallState QueryProductState(string productCodeString);

		[DllImport("msi", CharSet = CharSet.Unicode, EntryPoint = "MsiOpenPackageW")]
		internal static extern uint OpenPackage(string packagePath, out SafeMsiHandle product);

		[DllImport("msi", CharSet = CharSet.Unicode, EntryPoint = "MsiOpenPackageExW")]
		internal static extern uint OpenPackageEx(string packagePath, OpenPackageFlags options, out SafeMsiHandle product);

		internal static int ComparePatchSequence(MsiNativeMethods.PatchSequenceInfo p1, MsiNativeMethods.PatchSequenceInfo p2)
		{
			if (p1.order < p2.order)
			{
				return -1;
			}
			if (p1.order == p2.order)
			{
				return 0;
			}
			return 1;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct PatchSequenceInfo
		{
			public string patchData;

			public PatchDataType patchDataType;

			public int order;

			public uint status;
		}
	}
}
