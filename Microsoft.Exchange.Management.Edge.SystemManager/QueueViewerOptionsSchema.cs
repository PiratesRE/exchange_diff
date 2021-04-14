using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Management.SystemManager;

namespace Microsoft.Exchange.Management.Edge.SystemManager
{
	internal class QueueViewerOptionsSchema : ObjectSchema
	{
		private static readonly uint TotalSeconds = (uint)EnhancedTimeSpan.FromMilliseconds(2147483647.0).TotalSeconds;

		public static readonly AdminPropertyDefinition RefreshInterval = new AdminPropertyDefinition("RefreshInterval", ExchangeObjectVersion.Exchange2003, typeof(EnhancedTimeSpan), EnhancedTimeSpan.FromSeconds(30.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.FromSeconds(30.0), EnhancedTimeSpan.FromSeconds(QueueViewerOptionsSchema.TotalSeconds))
		}, PropertyDefinitionConstraint.None);

		public static readonly AdminPropertyDefinition AutoRefreshEnabled = new AdminPropertyDefinition("AutoRefreshEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool), true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly AdminPropertyDefinition PageSize = new AdminPropertyDefinition("PageSize", ExchangeObjectVersion.Exchange2003, typeof(uint), 1000U, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<uint>(1U, 10000U)
		});
	}
}
