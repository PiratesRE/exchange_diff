using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.InfoWorker.Common.ELC
{
	internal class AdPolicyReader : AdReader
	{
		internal static List<RetentionPolicy> GetAllRetentionPolicies(IConfigurationSession session, OrganizationId organizationId)
		{
			ADPagedReader<RetentionPolicy> source = session.FindPaged<RetentionPolicy>(null, QueryScope.SubTree, null, null, 0);
			List<RetentionPolicy> list = source.ToList<RetentionPolicy>();
			string arg = (organizationId.ConfigurationUnit == null) ? "First Organization" : organizationId.ConfigurationUnit.ToString();
			AdReader.Tracer.TraceDebug<int, string>(0L, "Found {0} retention policies for {1} in AD.", list.Count, arg);
			return list;
		}
	}
}
