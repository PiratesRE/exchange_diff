using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.QueueViewerTasks
{
	[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
	[Serializable]
	internal sealed class LocDescriptionAttribute : LocalizedDescriptionAttribute
	{
		public LocDescriptionAttribute(QueueViewerStrings.IDs ids) : base(QueueViewerStrings.GetLocalizedString(ids))
		{
		}
	}
}
