﻿using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IncludeInSchema = false)]
	[Serializable]
	public enum NotificationTypeEnum
	{
		CopiedEvent,
		CreatedEvent,
		DeletedEvent,
		FreeBusyChangedEvent,
		ModifiedEvent,
		MovedEvent,
		NewMailEvent,
		StatusEvent
	}
}