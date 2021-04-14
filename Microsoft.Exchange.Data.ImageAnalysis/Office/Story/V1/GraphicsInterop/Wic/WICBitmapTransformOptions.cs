using System;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Wic
{
	[Flags]
	internal enum WICBitmapTransformOptions
	{
		WICBitmapTransformRotate0 = 0,
		WICBitmapTransformRotate90 = 1,
		WICBitmapTransformRotate180 = 2,
		WICBitmapTransformRotate270 = 3,
		WICBitmapTransformFlipHorizontal = 8,
		WICBitmapTransformFlipVertical = 16,
		WICBITMAPTRANSFORMOPTIONS_FORCE_DWORD = 2147483647
	}
}
