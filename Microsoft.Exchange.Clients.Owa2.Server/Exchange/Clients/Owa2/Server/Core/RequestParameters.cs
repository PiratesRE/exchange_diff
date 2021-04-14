using System;
using System.Globalization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class RequestParameters
	{
		public RequestParameters(Guid requestId, string tag, MobileSpeechRecoRequestType requestType, CultureInfo culture, ExTimeZone timeZone, Guid userObjectGuid, Guid tenantGuid, OrganizationId orgId)
		{
			this.RequestId = requestId;
			this.Tag = tag;
			this.RequestType = requestType;
			this.Culture = culture;
			this.TimeZone = timeZone;
			this.UserObjectGuid = userObjectGuid;
			this.TenantGuid = tenantGuid;
			this.OrgId = orgId;
		}

		public Guid RequestId { get; private set; }

		public string Tag { get; private set; }

		public MobileSpeechRecoRequestType RequestType { get; private set; }

		public CultureInfo Culture { get; private set; }

		public ExTimeZone TimeZone { get; private set; }

		public Guid UserObjectGuid { get; private set; }

		public Guid TenantGuid { get; private set; }

		public OrganizationId OrgId { get; private set; }
	}
}
