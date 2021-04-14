using System;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Wic
{
	[Guid("00000040-A8F2-4877-BA0A-FD2B6645FB94")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	internal interface IWICPalette
	{
		void InitializePredefined([In] WICBitmapPaletteType ePaletteType, [In] int fAddTransparentColor);

		void InitializeCustom([In] ref int pColors, [In] int cCount);

		void InitializeFromBitmap([MarshalAs(UnmanagedType.Interface)] [In] IWICBitmapSource pISurface, [In] int cCount, [In] int fAddTransparentColor);

		void InitializeFromPalette([MarshalAs(UnmanagedType.Interface)] [In] IWICPalette pIPalette);

		void GetType(out WICBitmapPaletteType pePaletteType);

		void GetColorCount(out int pcCount);

		void GetColors([In] int cCount, out int pColors, out int pcActualColors);

		void IsBlackWhite(out int pfIsBlackWhite);

		void IsGrayscale(out int pfIsGrayscale);

		void HasAlpha(out int pfHasAlpha);
	}
}
