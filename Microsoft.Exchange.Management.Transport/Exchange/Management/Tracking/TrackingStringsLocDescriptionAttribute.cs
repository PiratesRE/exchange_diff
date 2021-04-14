using System;
using Microsoft.Exchange.Core;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Tracking
{
	[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class TrackingStringsLocDescriptionAttribute : LocalizedDescriptionAttribute
	{
		public TrackingStringsLocDescriptionAttribute(CoreStrings.IDs ids) : base(CoreStrings.GetLocalizedString(ids))
		{
		}
	}
}
