using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = false)]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class ClientStringsLocDescriptionAttribute : LocalizedDescriptionAttribute
	{
		public ClientStringsLocDescriptionAttribute(ClientStrings.IDs ids) : base(ClientStrings.GetLocalizedString(ids))
		{
		}
	}
}
