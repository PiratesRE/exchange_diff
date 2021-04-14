using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Audio
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("96406BD4-2B2B-11d3-B36B-00C04F6108FF")]
	[ComImport]
	internal interface IWMWriter
	{
		void SetProfileByID();

		void SetProfile([MarshalAs(UnmanagedType.Interface)] [In] IWMProfile pProfile);

		void SetOutputFilename([MarshalAs(UnmanagedType.LPWStr)] [In] string pwszFilename);

		void GetInputCount(out uint pcInputs);

		void GetInputProps([In] uint dwInputNum, [MarshalAs(UnmanagedType.Interface)] out IWMInputMediaProps ppInput);

		void SetInputProps([In] uint dwInputNum, [MarshalAs(UnmanagedType.Interface)] [In] IWMInputMediaProps pInput);

		void GetInputFormatCount();

		void GetInputFormat();

		void BeginWriting();

		void EndWriting();

		void AllocateSample([In] uint dwSampleSize, [MarshalAs(UnmanagedType.Interface)] out INSSBuffer ppSample);

		void WriteSample([In] uint dwInputNum, [In] ulong cnsSampleTime, [In] uint dwFlags, [MarshalAs(UnmanagedType.Interface)] [In] INSSBuffer pSample);

		void Flush();
	}
}
