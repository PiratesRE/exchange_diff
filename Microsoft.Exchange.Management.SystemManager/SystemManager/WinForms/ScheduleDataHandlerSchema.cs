using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class ScheduleDataHandlerSchema : ObjectSchema
	{
		public static readonly ProviderPropertyDefinition IntervalHours = new ADPropertyDefinition("IntervalHours", ExchangeObjectVersion.Exchange2003, typeof(Unlimited<uint>), "IntervalHours", ADPropertyDefinitionFlags.None, Unlimited<uint>.UnlimitedValue, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<uint>(1U, 48U)
		}, null, null);
	}
}
