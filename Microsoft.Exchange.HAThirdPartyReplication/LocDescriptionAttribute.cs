using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.ThirdPartyReplication
{
	[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
	[Serializable]
	internal sealed class LocDescriptionAttribute : LocalizedDescriptionAttribute
	{
		public LocDescriptionAttribute(ThirdPartyReplication.IDs ids) : base(ThirdPartyReplication.GetLocalizedString(ids))
		{
		}
	}
}
