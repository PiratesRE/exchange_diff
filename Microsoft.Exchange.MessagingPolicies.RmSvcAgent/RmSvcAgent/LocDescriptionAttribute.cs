using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.RmSvcAgent
{
	[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
	[Serializable]
	public sealed class LocDescriptionAttribute : LocalizedDescriptionAttribute
	{
		public LocDescriptionAttribute(RMSvcAgentStrings.IDs ids) : base(RMSvcAgentStrings.GetLocalizedString(ids))
		{
		}
	}
}
