﻿using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "PreviewItemBaseShapeType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum PreviewItemBaseShape
	{
		Default,
		Compact
	}
}
