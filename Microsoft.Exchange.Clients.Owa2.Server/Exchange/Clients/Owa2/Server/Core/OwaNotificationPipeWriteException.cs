﻿using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[Serializable]
	public sealed class OwaNotificationPipeWriteException : OwaNotificationPipeException
	{
		public OwaNotificationPipeWriteException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
