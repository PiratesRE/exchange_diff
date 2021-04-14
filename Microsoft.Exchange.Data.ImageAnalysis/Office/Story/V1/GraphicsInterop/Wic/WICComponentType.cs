using System;

namespace Microsoft.Office.Story.V1.GraphicsInterop.Wic
{
	internal enum WICComponentType
	{
		WICDecoder = 1,
		WICEncoder,
		WICPixelFormatConverter = 4,
		WICMetadataReader = 8,
		WICMetadataWriter = 16,
		WICPixelFormat = 32,
		WICAllComponents = 63,
		WICCOMPONENTTYPE_FORCE_DWORD = 2147483647
	}
}
