using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
	[Serializable]
	internal sealed class LocDescriptionAttribute : LocalizedDescriptionAttribute
	{
		public LocDescriptionAttribute(MigrationWorkflowServiceStrings.IDs ids) : base(MigrationWorkflowServiceStrings.GetLocalizedString(ids))
		{
		}
	}
}
