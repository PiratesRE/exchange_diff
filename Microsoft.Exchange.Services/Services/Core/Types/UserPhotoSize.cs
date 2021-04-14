using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "UserPhotoSizeType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum UserPhotoSize
	{
		HR48x48,
		HR64x64,
		HR96x96,
		HR120x120,
		HR240x240,
		HR360x360,
		HR432x432,
		HR504x504,
		HR648x648
	}
}
