using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Hygiene.Data;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Dal
{
	public class SaveOperation : ConfigDataProviderOperation
	{
		public string Object { get; set; }

		protected override void ExecuteConfigDataProviderOperation(IConfigDataProvider configDataProvider, IDictionary<string, object> variables)
		{
			IConfigurable configurable = (IConfigurable)DalProbeOperation.GetValue(this.Object, variables);
			ADObject adobject = configurable as ADObject;
			if (adobject != null)
			{
				adobject.SetId(new ADObjectId(DalHelper.GetTenantDistinguishedName(base.OrganizationTag), Guid.Empty));
			}
			configDataProvider.Save(configurable);
		}
	}
}
