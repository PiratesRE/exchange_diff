using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Clients.Owa.Core.Transcoding
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct TranscodingInitOption
	{
		public int RowNumberPerExcelPage;

		public int MaxOutputSize;

		[MarshalAs(UnmanagedType.Bool)]
		public bool IsImageMode;

		public HtmlFormat HtmlOutputFormat;
	}
}
