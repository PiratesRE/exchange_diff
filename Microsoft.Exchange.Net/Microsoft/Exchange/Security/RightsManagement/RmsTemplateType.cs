using System;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Security.RightsManagement
{
	public enum RmsTemplateType
	{
		[LocDescription(DrmStrings.IDs.TemplateTypeArchived)]
		Archived,
		[LocDescription(DrmStrings.IDs.TemplateTypeDistributed)]
		Distributed,
		[LocDescription(DrmStrings.IDs.TemplateTypeAll)]
		All = 100
	}
}
