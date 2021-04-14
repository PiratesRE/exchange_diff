using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Hygiene.Data;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Dal
{
	public class DeleteOperation : ConfigDataProviderOperation
	{
		[XmlAttribute]
		public Guid Id { get; set; }

		protected override void ExecuteConfigDataProviderOperation(IConfigDataProvider configDataProvider, IDictionary<string, object> variables)
		{
			Type type = DalProbeOperation.ResolveDataType(base.DataType);
			IConfigurable configurable = (IConfigurable)Activator.CreateInstance(type);
			DalHelper.SetPropertyValue(new ADObjectId(this.Id), ADObjectSchema.Id, configurable);
			configDataProvider.Delete(configurable);
		}
	}
}
