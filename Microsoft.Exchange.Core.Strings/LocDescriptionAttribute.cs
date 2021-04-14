using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Core
{
	[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
	[Serializable]
	public sealed class LocDescriptionAttribute : LocalizedDescriptionAttribute
	{
		public LocDescriptionAttribute(CoreStrings.IDs ids) : base(CoreStrings.GetLocalizedString(ids))
		{
		}
	}
}
