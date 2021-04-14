using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Clients.Owa2.Server.Diagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class RmsLoadingLogEvent : ILogEvent
	{
		public RmsLoadingLogEvent(OrganizationId organizationId, Exception exception)
		{
			this.organizationId = organizationId;
			this.exception = exception;
		}

		public string EventId
		{
			get
			{
				return "RmsLoadingLogEvent";
			}
		}

		public ICollection<KeyValuePair<string, object>> GetEventData()
		{
			return new KeyValuePair<string, object>[]
			{
				new KeyValuePair<string, object>("Org", (this.organizationId == null) ? "null" : this.organizationId.ToString()),
				new KeyValuePair<string, object>("Exc", (this.exception == null) ? "null" : this.exception.ToString())
			};
		}

		private readonly OrganizationId organizationId;

		private readonly Exception exception;
	}
}
