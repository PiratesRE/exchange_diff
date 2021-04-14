using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.UpdatableHelp
{
	[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
	[Serializable]
	internal sealed class LocDescriptionAttribute : LocalizedDescriptionAttribute
	{
		public LocDescriptionAttribute(UpdatableHelpStrings.IDs ids) : base(UpdatableHelpStrings.GetLocalizedString(ids))
		{
		}
	}
}
