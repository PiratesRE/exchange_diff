using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.BackgroundJobBackend
{
	internal sealed class RegionDefinition : BackgroundJobBackendBase
	{
		public Regions RegionId
		{
			get
			{
				return (Regions)this[RegionDefinition.RegionIdProperty];
			}
			set
			{
				this[RegionDefinition.RegionIdProperty] = (int)value;
			}
		}

		public string Name
		{
			get
			{
				return (string)this[RegionDefinition.NameProperty];
			}
			set
			{
				this[RegionDefinition.NameProperty] = value;
			}
		}

		internal static readonly BackgroundJobBackendPropertyDefinition NameProperty = new BackgroundJobBackendPropertyDefinition("Name", typeof(string), PropertyDefinitionFlags.Mandatory, null);

		internal static readonly BackgroundJobBackendPropertyDefinition RegionIdProperty = new BackgroundJobBackendPropertyDefinition("RegionId", typeof(int), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, 0);
	}
}
