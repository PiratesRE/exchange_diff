using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Common.DiskManagement
{
	[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
	[Serializable]
	internal sealed class LocDescriptionAttribute : LocalizedDescriptionAttribute
	{
		public LocDescriptionAttribute(DiskManagementStrings.IDs ids) : base(DiskManagementStrings.GetLocalizedString(ids))
		{
		}
	}
}
