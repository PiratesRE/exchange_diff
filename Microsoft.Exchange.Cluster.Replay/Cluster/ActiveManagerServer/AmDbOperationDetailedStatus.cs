using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Microsoft.Exchange.Data.HA.DirectoryServices;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmDbOperationDetailedStatus
	{
		public AmDbOperationDetailedStatus(IADDatabase db)
		{
			this.Database = db;
			this.AttemptedServerSubStatuses = new OrderedDictionary(5);
		}

		internal IADDatabase Database { get; private set; }

		internal AmDbStateInfo InitialDbState { get; set; }

		internal AmDbStateInfo FinalDbState { get; set; }

		private OrderedDictionary AttemptedServerSubStatuses { get; set; }

		public void AddSubstatus(AmDbOperationSubStatus subStatus)
		{
			this.AttemptedServerSubStatuses.Add(subStatus.ServerAttempted, subStatus);
		}

		public IEnumerable<AmDbOperationSubStatus> GetAllSubStatuses()
		{
			foreach (object obj in this.AttemptedServerSubStatuses)
			{
				DictionaryEntry de = (DictionaryEntry)obj;
				DictionaryEntry dictionaryEntry = de;
				yield return (AmDbOperationSubStatus)dictionaryEntry.Value;
			}
			yield break;
		}
	}
}
