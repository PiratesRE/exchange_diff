using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.RightsManagement
{
	[Cmdlet("Get", "IRMConfiguration")]
	public sealed class GetIRMConfiguration : GetMultitenancySingletonSystemConfigurationObjectTask<IRMConfiguration>
	{
		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override IEnumerable<IRMConfiguration> GetPagedData()
		{
			IEnumerable<IRMConfiguration> enumerable = base.GetPagedData().ToList<IRMConfiguration>();
			if (!enumerable.Any<IRMConfiguration>())
			{
				IRMConfiguration item = IRMConfiguration.Read((IConfigurationSession)base.DataSession);
				enumerable = new List<IRMConfiguration>
				{
					item
				};
			}
			return enumerable;
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			IRMConfiguration irmconfiguration = (IRMConfiguration)dataObject;
			base.WriteResult(dataObject);
		}
	}
}
