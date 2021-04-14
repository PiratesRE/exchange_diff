using System;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Wic
{
	internal enum WICBitmapDitherType
	{
		WICBitmapDitherTypeNone,
		WICBitmapDitherTypeSolid = 0,
		WICBitmapDitherTypeOrdered4x4,
		WICBitmapDitherTypeOrdered8x8,
		WICBitmapDitherTypeOrdered16x16,
		WICBitmapDitherTypeSpiral4x4,
		WICBitmapDitherTypeSpiral8x8,
		WICBitmapDitherTypeDualSpiral4x4,
		WICBitmapDitherTypeDualSpiral8x8,
		WICBitmapDitherTypeErrorDiffusion,
		WICBITMAPDITHERTYPE_FORCE_DWORD = 2147483647
	}
}
