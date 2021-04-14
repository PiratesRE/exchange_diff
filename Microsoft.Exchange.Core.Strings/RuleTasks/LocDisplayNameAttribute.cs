using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Core.RuleTasks
{
	[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
	[Serializable]
	public sealed class LocDisplayNameAttribute : LocalizedDisplayNameAttribute
	{
		public LocDisplayNameAttribute(RulesTasksStrings.IDs ids) : base(RulesTasksStrings.GetLocalizedString(ids))
		{
		}
	}
}
