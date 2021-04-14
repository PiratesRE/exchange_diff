﻿using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum CalendarPermissionReadAccess
	{
		None,
		TimeOnly,
		TimeAndSubjectAndLocation,
		FullDetails
	}
}
