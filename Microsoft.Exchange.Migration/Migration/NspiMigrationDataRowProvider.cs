using System;
using System.Collections.Generic;
using Microsoft.Exchange.Migration.DataAccessLayer;

namespace Microsoft.Exchange.Migration
{
	internal sealed class NspiMigrationDataRowProvider : IMigrationDataRowProvider
	{
		public NspiMigrationDataRowProvider(ExchangeOutlookAnywhereEndpoint endpoint, MigrationJob job, bool discoverProvisioning = true)
		{
			this.discoverProvisioning = discoverProvisioning;
			this.nspiDataReader = endpoint.GetNspiDataReader(job);
		}

		public IEnumerable<IMigrationDataRow> GetNextBatchItem(string cursorPosition, int maxCountHint)
		{
			int delta = -1;
			if (!string.IsNullOrEmpty(cursorPosition) && !int.TryParse(cursorPosition, out delta))
			{
				throw new ArgumentException("cursorPosition");
			}
			IEnumerable<IMigrationDataRow> items = this.nspiDataReader.GetItems(delta, maxCountHint, this.discoverProvisioning);
			foreach (IMigrationDataRow row in items)
			{
				yield return row;
			}
			yield break;
		}

		internal string[] GetMembers(string groupSmtpAddress)
		{
			return this.nspiDataReader.GetMembers(groupSmtpAddress);
		}

		private readonly NspiMigrationDataReader nspiDataReader;

		private readonly bool discoverProvisioning;
	}
}
