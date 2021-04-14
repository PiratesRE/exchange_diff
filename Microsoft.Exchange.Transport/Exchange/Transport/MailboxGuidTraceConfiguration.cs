using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal class MailboxGuidTraceConfiguration : TraceConfigurationBase
	{
		public override void OnLoad()
		{
			this.filteredMailboxs = MailboxGuidTraceConfiguration.GetGuidFilterList(this.exTraceConfiguration, "MailboxGuid");
			this.filteredMDBs = MailboxGuidTraceConfiguration.GetGuidFilterList(this.exTraceConfiguration, "MailboxDatabaseGuid");
		}

		public List<Guid> FilteredMailboxs
		{
			get
			{
				return this.filteredMailboxs;
			}
		}

		public List<Guid> FilteredMDBs
		{
			get
			{
				return this.filteredMDBs;
			}
		}

		private static List<Guid> GetGuidFilterList(ExTraceConfiguration configuration, string filterKey)
		{
			List<Guid> list = new List<Guid>();
			List<string> list2;
			if (configuration.CustomParameters.TryGetValue(filterKey, out list2))
			{
				foreach (string g in list2)
				{
					try
					{
						Guid item = new Guid(g);
						list.Add(item);
					}
					catch (FormatException)
					{
					}
				}
			}
			return list;
		}

		public const string MailboxGuidFilterKey = "MailboxGuid";

		public const string MailboxDatabaseGuidFilterKey = "MailboxDatabaseGuid";

		private List<Guid> filteredMDBs;

		private List<Guid> filteredMailboxs;
	}
}
