using System;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Wic
{
	internal enum WICBitmapPaletteType
	{
		WICBitmapPaletteTypeCustom,
		WICBitmapPaletteTypeMedianCut,
		WICBitmapPaletteTypeFixedBW,
		WICBitmapPaletteTypeFixedHalftone8,
		WICBitmapPaletteTypeFixedHalftone27,
		WICBitmapPaletteTypeFixedHalftone64,
		WICBitmapPaletteTypeFixedHalftone125,
		WICBitmapPaletteTypeFixedHalftone216,
		WICBitmapPaletteTypeFixedWebPalette = 7,
		WICBitmapPaletteTypeFixedHalftone252,
		WICBitmapPaletteTypeFixedHalftone256,
		WICBitmapPaletteTypeFixedGray4,
		WICBitmapPaletteTypeFixedGray16,
		WICBitmapPaletteTypeFixedGray256,
		WICBITMAPPALETTETYPE_FORCE_DWORD = 2147483647
	}
}
