using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Agent.SharedMailboxSentItemsRoutingAgent
{
	[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
	[Serializable]
	internal sealed class LocDescriptionAttribute : LocalizedDescriptionAttribute
	{
		public LocDescriptionAttribute(AgentStrings.IDs ids) : base(AgentStrings.GetLocalizedString(ids))
		{
		}
	}
}
