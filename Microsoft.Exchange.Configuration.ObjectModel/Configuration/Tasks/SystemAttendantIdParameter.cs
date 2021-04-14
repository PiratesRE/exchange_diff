using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class SystemAttendantIdParameter : ServerIdParameter
	{
		public SystemAttendantIdParameter(string identity) : base(identity)
		{
		}

		public SystemAttendantIdParameter()
		{
		}

		public SystemAttendantIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public SystemAttendantIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public new static SystemAttendantIdParameter Parse(string identity)
		{
			return new SystemAttendantIdParameter(identity);
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(session.DomainController, true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 104, "GetObjects", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\IdentityParameter\\SystemAttendantIdParameter.cs");
			IEnumerable<Server> objects = base.GetObjects<Server>(rootId, topologyConfigurationSession, topologyConfigurationSession, null, out notFoundReason);
			List<T> list = new List<T>();
			foreach (Server server in objects)
			{
				ADObjectId childId = server.Id.GetChildId("Microsoft System Attendant");
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, "exchangeAdminService");
				IEnumerable<ADRecipient> enumerable = base.PerformPrimarySearch<ADRecipient>(filter, childId, session, true, optionalData);
				int num = 0;
				foreach (ADRecipient adrecipient in enumerable)
				{
					list.Add((T)((object)adrecipient));
					num++;
				}
			}
			return list;
		}
	}
}
