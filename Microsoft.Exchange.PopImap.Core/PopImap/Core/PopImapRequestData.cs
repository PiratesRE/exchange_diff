using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.PopImap.Core
{
	public class PopImapRequestData
	{
		public Guid Id { get; private set; }

		public string ServerName { get; set; }

		public string UserEmail { get; set; }

		public string CommandName { get; set; }

		public string Parameters { get; set; }

		public string Response { get; set; }

		public string ResponseType { get; set; }

		public string LightLogContext { get; set; }

		public string Message { get; set; }

		public double RequestTime { get; set; }

		public double RpcLatency { get; set; }

		public double LdapLatency { get; set; }

		public bool HasErrors { get; set; }

		public List<ErrorDetail> ErrorDetails { get; set; }

		internal PopImapRequestData(Guid id)
		{
			this.Id = id;
		}
	}
}
