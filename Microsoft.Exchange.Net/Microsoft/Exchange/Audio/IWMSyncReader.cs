using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Audio
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("9397F121-7705-4dc9-B049-98B698188414")]
	[ComImport]
	internal interface IWMSyncReader
	{
		void Open([MarshalAs(UnmanagedType.LPWStr)] [In] string pwszFilename);

		void Close();

		void SetRange();

		void SetRangeByFrame();

		void GetNextSample([In] ushort wStreamNum, out INSSBuffer ppSample, out ulong pcnsSampleTime, out ulong pcnsDuration, out uint pdwFlags, out uint pdwOutputNum, out ushort pwStreamNum);

		void SetStreamsSelected();

		void GetStreamSelected();

		void SetReadStreamSamples();

		void GetReadStreamSamples();

		void GetOutputSetting();

		void SetOutputSetting();

		void GetOutputCount(out uint pcOutputs);

		void GetOutputProps();

		void SetOutputProps();

		void GetOutputFormatCount();

		void GetOutputFormat([In] uint dwOutputNum, [In] uint dwFormatNum, [MarshalAs(UnmanagedType.Interface)] out IWMOutputMediaProps ppProps);

		void GetOutputNumberForStream();

		void GetStreamNumberForOutput();

		void GetMaxOutputSampleSize();

		void GetMaxStreamSampleSize();

		void OpenStream();
	}
}
