using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabaseAlertInfoTable<TAlert> : IDatabaseAlertInfoTable, IEnumerable<KeyValuePair<Guid, TAlert>>, IEnumerable where TAlert : MonitoringAlert
	{
		protected static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.MonitoringTracer;
			}
		}

		public DatabaseAlertInfoTable(Func<IHealthValidationResultMinimal, TAlert> createAlertDelegate)
		{
			this.m_createAlertDelegate = createAlertDelegate;
			this.m_alertTable = new Dictionary<Guid, TAlert>(48);
		}

		public void RaiseAppropriateAlertIfNecessary(IHealthValidationResultMinimal result)
		{
			MonitoringAlert existingOrNewAlertInfo = this.GetExistingOrNewAlertInfo(result);
			existingOrNewAlertInfo.RaiseAppropriateAlertIfNecessary(result);
		}

		public void ResetState(Guid dbGuid)
		{
			TAlert talert = default(TAlert);
			if (this.m_alertTable.TryGetValue(dbGuid, out talert))
			{
				talert.ResetState();
			}
		}

		protected void RemoveDatabaseAlert(Guid dbGuid)
		{
			if (this.m_alertTable.ContainsKey(dbGuid))
			{
				this.m_alertTable.Remove(dbGuid);
			}
		}

		public void Cleanup(HashSet<Guid> currentlyKnownDatabaseGuids)
		{
			List<Guid> list = new List<Guid>();
			foreach (Guid item in this.m_alertTable.Keys)
			{
				if (!currentlyKnownDatabaseGuids.Contains(item))
				{
					list.Add(item);
				}
			}
			if (list.Count > 0)
			{
				foreach (Guid guid in list)
				{
					this.m_alertTable.Remove(guid);
					DatabaseAlertInfoTable<TAlert>.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "DatabaseAlertInfoTable: Cleanup removed database: {0}", guid);
				}
			}
		}

		IEnumerator<KeyValuePair<Guid, TAlert>> IEnumerable<KeyValuePair<Guid, !0>>.GetEnumerator()
		{
			return this.m_alertTable.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)this.m_alertTable).GetEnumerator();
		}

		protected virtual MonitoringAlert GetExistingOrNewAlertInfo(IHealthValidationResultMinimal result)
		{
			TAlert talert = default(TAlert);
			Guid identityGuid = result.IdentityGuid;
			if (!this.m_alertTable.TryGetValue(identityGuid, out talert))
			{
				talert = this.m_createAlertDelegate(result);
				this.m_alertTable[identityGuid] = talert;
			}
			return talert;
		}

		private Dictionary<Guid, TAlert> m_alertTable;

		private Func<IHealthValidationResultMinimal, TAlert> m_createAlertDelegate;
	}
}
