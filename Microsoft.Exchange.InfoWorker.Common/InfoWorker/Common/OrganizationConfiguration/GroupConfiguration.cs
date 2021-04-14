using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration
{
	internal class GroupConfiguration
	{
		internal Guid Id { get; private set; }

		internal DateTime Version { get; private set; }

		internal IEnumerable<Guid> GroupGuids { get; private set; }

		internal GroupConfiguration(Guid id, DateTime version, IEnumerable<Guid> groups)
		{
			this.Id = id;
			this.Version = version;
			this.GroupGuids = groups;
		}
	}
}
