using System;
using Microsoft.Exchange.Cluster.Replay.Monitoring;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Serializable]
	public sealed class ServerRedundancy : IConfigurable
	{
		internal ServerRedundancy(HealthInfoPersisted healthInfo, ServerHealthInfoPersisted serverHealth, string serverContactedFqdn)
		{
			AmServerName amServerName = new AmServerName(serverHealth.ServerFqdn);
			this.Identity = new ConfigObjectId(amServerName.NetbiosName.ToUpperInvariant());
			this.ServerContactedFqdn = serverContactedFqdn.ToUpperInvariant();
			this.HealthInfoCreateTime = DateTimeHelper.ParseIntoNullableLocalDateTimeIfPossible(healthInfo.CreateTimeUtcStr);
			this.HealthInfoLastUpdateTime = DateTimeHelper.ParseIntoNullableLocalDateTimeIfPossible(healthInfo.LastUpdateTimeUtcStr);
			this.ServerFoundInAD = TransitionInfo.ConstructFromRemoteSerializable(serverHealth.ServerFoundInAD);
			this.IsServerFoundInAD = this.ServerFoundInAD.IsSuccess;
			this.CriticalForMaintainingAvailability = TransitionInfo.ConstructFromRemoteSerializable(serverHealth.CriticalForMaintainingAvailability);
			this.CriticalForMaintainingRedundancy = TransitionInfo.ConstructFromRemoteSerializable(serverHealth.CriticalForMaintainingRedundancy);
			this.CriticalForRestoringAvailability = TransitionInfo.ConstructFromRemoteSerializable(serverHealth.CriticalForRestoringAvailability);
			this.CriticalForRestoringRedundancy = TransitionInfo.ConstructFromRemoteSerializable(serverHealth.CriticalForRestoringRedundancy);
			this.HighForRestoringAvailability = TransitionInfo.ConstructFromRemoteSerializable(serverHealth.HighForRestoringAvailability);
			this.HighForRestoringRedundancy = TransitionInfo.ConstructFromRemoteSerializable(serverHealth.HighForRestoringRedundancy);
			this.SetRepairUrgency();
			this.SetSafeForMaintenance();
		}

		public ObjectId Identity { get; private set; }

		public bool IsServerFoundInAD { get; private set; }

		public RepairUrgency RepairUrgency { get; private set; }

		public bool SafeForMaintenance { get; private set; }

		public string ServerContactedFqdn { get; private set; }

		public DateTime? HealthInfoCreateTime { get; private set; }

		public DateTime? HealthInfoLastUpdateTime { get; private set; }

		public TransitionInfo ServerFoundInAD { get; private set; }

		public TransitionInfo CriticalForMaintainingAvailability { get; set; }

		public TransitionInfo CriticalForMaintainingRedundancy { get; set; }

		public TransitionInfo CriticalForRestoringAvailability { get; set; }

		public TransitionInfo CriticalForRestoringRedundancy { get; set; }

		public TransitionInfo HighForRestoringAvailability { get; set; }

		public TransitionInfo HighForRestoringRedundancy { get; set; }

		public static string GetRepairUrgencyDisplayString(RepairUrgency urgency)
		{
			string text = LocalizedDescriptionAttribute.FromEnum(typeof(RepairUrgency), urgency);
			if (!string.IsNullOrWhiteSpace(text))
			{
				return text;
			}
			return urgency.ToString();
		}

		internal bool IsValid
		{
			get
			{
				return true;
			}
		}

		bool IConfigurable.IsValid
		{
			get
			{
				return this.IsValid;
			}
		}

		internal ObjectState ObjectState
		{
			get
			{
				return ObjectState.Unchanged;
			}
		}

		ObjectState IConfigurable.ObjectState
		{
			get
			{
				return this.ObjectState;
			}
		}

		public ValidationError[] Validate()
		{
			return new ValidationError[0];
		}

		public void CopyChangesFrom(IConfigurable source)
		{
			throw new NotImplementedException();
		}

		public void ResetChangeTracking()
		{
			throw new NotImplementedException();
		}

		private void SetRepairUrgency()
		{
			if (this.CriticalForMaintainingAvailability.IsSuccess || this.CriticalForMaintainingRedundancy.IsSuccess)
			{
				this.RepairUrgency = RepairUrgency.Prohibited;
				return;
			}
			if (this.CriticalForRestoringAvailability.IsSuccess || this.CriticalForRestoringRedundancy.IsSuccess)
			{
				this.RepairUrgency = RepairUrgency.Critical;
				return;
			}
			if (this.HighForRestoringAvailability.IsSuccess || this.HighForRestoringRedundancy.IsSuccess)
			{
				this.RepairUrgency = RepairUrgency.High;
				return;
			}
			this.RepairUrgency = RepairUrgency.Normal;
		}

		private void SetSafeForMaintenance()
		{
			if (this.CriticalForMaintainingAvailability.IsSuccess || this.CriticalForMaintainingRedundancy.IsSuccess || this.CriticalForRestoringAvailability.IsSuccess || this.CriticalForRestoringRedundancy.IsSuccess)
			{
				this.SafeForMaintenance = false;
				return;
			}
			this.SafeForMaintenance = true;
		}
	}
}
