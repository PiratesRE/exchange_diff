using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
	[Serializable]
	internal sealed class LocDescriptionAttribute : LocalizedDescriptionAttribute
	{
		public LocDescriptionAttribute(DataStrings.IDs ids) : base(DataStrings.GetLocalizedString(ids))
		{
		}
	}
}
