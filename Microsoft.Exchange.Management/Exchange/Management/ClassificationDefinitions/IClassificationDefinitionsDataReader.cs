using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal interface IClassificationDefinitionsDataReader
	{
		IEnumerable<TransportRule> GetAllClassificationRuleCollection(OrganizationId organizationId, IConfigurationSession currentDataSession, QueryFilter additionalFilter);

		DataClassificationConfig GetDataClassificationConfig(OrganizationId organizationId, IConfigurationSession currentDataSession);
	}
}
