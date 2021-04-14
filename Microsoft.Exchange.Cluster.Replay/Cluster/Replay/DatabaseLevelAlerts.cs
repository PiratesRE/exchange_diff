using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabaseLevelAlerts
	{
		public DatabaseAlertInfoTable<DatabaseRedundancyTwoCopyAlert> TwoCopy { get; private set; }

		public DatabaseAlertInfoTable<DatabaseRedundancyAlert> OneRedundantCopy { get; private set; }

		public DatabaseAlertInfoTable<DatabaseRedundancySiteAlert> OneRedundantCopySite { get; private set; }

		public DatabaseAlertInfoTable<DatabaseAvailabilityAlert> OneAvailableCopy { get; private set; }

		public DatabaseAlertInfoTable<DatabaseAvailabilitySiteAlert> OneAvailableCopySite { get; private set; }

		public DatabaseAlertInfoTable<DatabaseStaleStatusAlert> StaleStatus { get; private set; }

		public PotentialOneRedundantCopyAlertTable PotentialOneRedundantCopy { get; private set; }

		public DatabaseLevelAlerts()
		{
			this.TwoCopy = new DatabaseAlertInfoTable<DatabaseRedundancyTwoCopyAlert>((IHealthValidationResultMinimal validationResult) => new DatabaseRedundancyTwoCopyAlert(validationResult.Identity, validationResult.IdentityGuid));
			this.OneRedundantCopy = new DatabaseAlertInfoTable<DatabaseRedundancyAlert>((IHealthValidationResultMinimal validationResult) => new DatabaseRedundancyAlert(validationResult.Identity, validationResult.IdentityGuid));
			this.OneRedundantCopySite = new DatabaseAlertInfoTable<DatabaseRedundancySiteAlert>((IHealthValidationResultMinimal validationResult) => new DatabaseRedundancySiteAlert(validationResult.Identity, validationResult.IdentityGuid));
			this.OneAvailableCopy = new DatabaseAlertInfoTable<DatabaseAvailabilityAlert>((IHealthValidationResultMinimal validationResult) => new DatabaseAvailabilityAlert(validationResult.Identity, validationResult.IdentityGuid));
			this.OneAvailableCopySite = new DatabaseAlertInfoTable<DatabaseAvailabilitySiteAlert>((IHealthValidationResultMinimal validationResult) => new DatabaseAvailabilitySiteAlert(validationResult.Identity, validationResult.IdentityGuid));
			this.StaleStatus = new DatabaseAlertInfoTable<DatabaseStaleStatusAlert>((IHealthValidationResultMinimal validationResult) => new DatabaseStaleStatusAlert(validationResult.Identity, validationResult.IdentityGuid));
			this.PotentialOneRedundantCopy = new PotentialOneRedundantCopyAlertTable();
			this.m_alertsArray = new IDatabaseAlertInfoTable[]
			{
				this.TwoCopy,
				this.OneRedundantCopy,
				this.OneRedundantCopySite,
				this.OneAvailableCopy,
				this.OneAvailableCopySite,
				this.StaleStatus,
				this.PotentialOneRedundantCopy
			};
		}

		internal void ResetState(Guid dbGuid)
		{
			for (int i = 0; i < this.m_alertsArray.Length; i++)
			{
				this.m_alertsArray[i].ResetState(dbGuid);
			}
		}

		internal void Cleanup(HashSet<Guid> currentlyKnownDatabaseGuids)
		{
			for (int i = 0; i < this.m_alertsArray.Length; i++)
			{
				this.m_alertsArray[i].Cleanup(currentlyKnownDatabaseGuids);
			}
		}

		private IDatabaseAlertInfoTable[] m_alertsArray;
	}
}
