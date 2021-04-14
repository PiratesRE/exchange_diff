using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Exchange.Setup.AcquireLanguagePack
{
	internal sealed class MsiNativeMethods
	{
		private MsiNativeMethods()
		{
		}

		[DllImport("msi", CharSet = CharSet.Unicode, EntryPoint = "MsiDetermineApplicablePatchesW")]
		public static extern uint DetermineApplicablePatches(string packagePath, int count, [In] [Out] MsiNativeMethods.PatchSequenceInfo[] patches);

		[DllImport("msi", CharSet = CharSet.Unicode, EntryPoint = "MsiOpenDatabaseW")]
		public static extern uint OpenDatabase(string databasePath, IntPtr pPersist, out SafeMsiHandle database);

		[DllImport("msi", CharSet = CharSet.Unicode, EntryPoint = "MsiDatabaseOpenViewW")]
		public static extern uint DatabaseOpenView(SafeMsiHandle database, string query, out SafeMsiHandle view);

		[DllImport("msi", CharSet = CharSet.Auto, EntryPoint = "MsiViewExecute")]
		public static extern uint ViewExecute(SafeMsiHandle view, IntPtr record);

		[DllImport("msi", CharSet = CharSet.Auto, EntryPoint = "MsiViewFetch")]
		public static extern uint ViewFetch(SafeMsiHandle view, out SafeMsiHandle record);

		[DllImport("msi", CharSet = CharSet.Unicode, EntryPoint = "MsiRecordGetStringW")]
		public static extern uint RecordGetString(SafeMsiHandle record, uint field, StringBuilder data, ref uint count);

		[DllImport("msi", CharSet = CharSet.Auto, EntryPoint = "MsiRecordDataSize")]
		public static extern uint RecordDataSize(SafeMsiHandle record, uint field);

		[DllImport("msi", CharSet = CharSet.Auto, EntryPoint = "MsiRecordReadStream")]
		public static extern uint RecordReadStream(SafeMsiHandle record, uint field, byte[] buffer, ref int bufferLength);

		public static int ComparePatchSequence(MsiNativeMethods.PatchSequenceInfo p1, MsiNativeMethods.PatchSequenceInfo p2)
		{
			if (p1.Order < p2.Order)
			{
				return -1;
			}
			if (p1.Order == p2.Order)
			{
				return 0;
			}
			return 1;
		}

		public enum PatchDataType
		{
			PatchFile,
			XmlPath,
			XmlBlob
		}

		public enum ReturnCode
		{
			ErrorSuccess,
			ErrorMoreData = 234,
			ErrorNoMoreItems = 259
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct PatchSequenceInfo
		{
			public string PatchData;

			public MsiNativeMethods.PatchDataType PatchDataType;

			public int Order;

			public uint Status;
		}
	}
}
