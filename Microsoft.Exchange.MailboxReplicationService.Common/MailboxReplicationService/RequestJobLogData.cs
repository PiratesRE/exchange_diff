using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class RequestJobLogData : RequestJobBase
	{
		public RequestJobLogData(RequestJobBase request, RequestState statusDetail) : this(request)
		{
			this.SetOverride("StatusDetail", statusDetail.ToString());
		}

		public RequestJobLogData(RequestJobBase request) : base((SimpleProviderPropertyBag)request.propertyBag)
		{
			this.overrides = new Dictionary<string, string>();
			this.Request = request;
		}

		public RequestJobBase Request { get; private set; }

		internal bool TryGetOverride(string key, out string value)
		{
			return this.overrides.TryGetValue(key, out value);
		}

		private void SetOverride(string key, string value)
		{
			this.overrides[key] = value;
		}

		public const string StatusDetailKey = "StatusDetail";

		private Dictionary<string, string> overrides;
	}
}
