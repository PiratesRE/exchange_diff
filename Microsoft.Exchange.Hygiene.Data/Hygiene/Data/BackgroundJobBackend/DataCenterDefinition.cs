using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Hygiene.Data.BackgroundJobBackend
{
	internal sealed class DataCenterDefinition : BackgroundJobBackendBase
	{
		public long DataCenterId
		{
			get
			{
				return (long)this[DataCenterDefinition.DataCenterIdProperty];
			}
			set
			{
				this[DataCenterDefinition.DataCenterIdProperty] = value;
			}
		}

		public string Name
		{
			get
			{
				return (string)this[DataCenterDefinition.NameProperty];
			}
			set
			{
				this[DataCenterDefinition.NameProperty] = value;
			}
		}

		internal static readonly BackgroundJobBackendPropertyDefinition NameProperty = new BackgroundJobBackendPropertyDefinition("Name", typeof(string), PropertyDefinitionFlags.Mandatory, null);

		internal static readonly BackgroundJobBackendPropertyDefinition DataCenterIdProperty = new BackgroundJobBackendPropertyDefinition("DCId", typeof(long), PropertyDefinitionFlags.Mandatory | PropertyDefinitionFlags.PersistDefaultValue, 0L);
	}
}
