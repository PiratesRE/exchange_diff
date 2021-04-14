using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Net
{
	[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
	[Serializable]
	internal sealed class LocDescriptionAttribute : LocalizedDescriptionAttribute
	{
		public LocDescriptionAttribute(DrmStrings.IDs ids) : base(DrmStrings.GetLocalizedString(ids))
		{
		}
	}
}
