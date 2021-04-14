using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Audio
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("96406BDB-2B2B-11d3-B36B-00C04F6108FF")]
	[ComImport]
	internal interface IWMProfile
	{
		void GetVersion();

		void GetName();

		void SetName();

		void GetDescription();

		void SetDescription();

		void GetStreamCount();

		void GetStream([In] uint dwStreamIndex, [MarshalAs(UnmanagedType.Interface)] out IWMStreamConfig ppConfig);

		void GetStreamByNumber();

		void RemoveStream();

		void RemoveStreamByNumber();

		void AddStream();

		void ReconfigStream([MarshalAs(UnmanagedType.Interface)] [In] IWMStreamConfig pConfig);

		void CreateNewStream();

		void GetMutualExclusionCount();

		void GetMutualExclusion();

		void RemoveMutualExclusion();

		void AddMutualExclusion();

		void CreateNewMutualExclusion();
	}
}
