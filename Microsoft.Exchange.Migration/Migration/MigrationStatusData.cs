using System;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxReplicationService;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	internal class MigrationStatusData<TStatus> : IMigrationSerializable where TStatus : struct
	{
		public MigrationStatusData(TStatus status, long version, MigrationState? state = null) : this(version)
		{
			this.Status = status;
			this.StateLastUpdated = new ExDateTime?(ExDateTime.UtcNow);
			if (state != null)
			{
				this.State = state.Value;
			}
		}

		public MigrationStatusData(TStatus status, Exception error, long version, MigrationState? state = null) : this(status, version, state)
		{
			this.FailureRecord = MigrationStatusData<TStatus>.GetFailureRecord(error);
		}

		public MigrationStatusData(long version)
		{
			this.StatusHistory = string.Empty;
			this.Version = version;
		}

		internal MigrationStatusData(MigrationStatusData<TStatus> statusData)
		{
			this.Status = statusData.Status;
			this.State = statusData.State;
			this.StateLastUpdated = statusData.StateLastUpdated;
			this.TransientErrorCount = statusData.TransientErrorCount;
			this.PreviousStatus = statusData.PreviousStatus;
			this.InternalError = statusData.InternalError;
			this.InternalErrorTime = statusData.InternalErrorTime;
			this.StatusHistory = statusData.StatusHistory;
			this.SameStatusCount = statusData.SameStatusCount;
			this.FailureRecord = statusData.FailureRecord;
			this.WatsonHash = statusData.WatsonHash;
			this.Version = statusData.Version;
		}

		public FailureRec FailureRecord { get; private set; }

		public string InternalError { get; private set; }

		public int TransientErrorCount { get; private set; }

		public ExDateTime? StateLastUpdated { get; protected set; }

		public LocalizedString? LocalizedError
		{
			get
			{
				if (this.FailureRecord != null)
				{
					return new LocalizedString?(new LocalizedString(this.FailureRecord.Message));
				}
				return null;
			}
		}

		public ExDateTime? InternalErrorTime { get; private set; }

		public TStatus Status { get; private set; }

		public MigrationState State { get; private set; }

		public TStatus? PreviousStatus { get; private set; }

		public string StatusHistory { get; private set; }

		public int SameStatusCount { get; private set; }

		public string WatsonHash { get; private set; }

		public long Version { get; private set; }

		PropertyDefinition[] IMigrationSerializable.PropertyDefinitions
		{
			get
			{
				return MigrationStatusData<TStatus>.StatusPropertyDefinition;
			}
		}

		public override string ToString()
		{
			if (this.Version >= 2L)
			{
				return string.Format("{0}:{1}", this.State, this.LocalizedError);
			}
			return string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				this.Status,
				this.StateLastUpdated,
				this.LocalizedError,
				this.InternalError.ToTruncatedString()
			});
		}

		public bool ReadFromMessageItem(IMigrationStoreObject message)
		{
			MigrationUtil.ThrowOnNullArgument(message, "message");
			if (this.Version >= 2L)
			{
				this.State = MigrationHelper.GetEnumProperty<MigrationState>(message, MigrationBatchMessageSchema.MigrationState);
			}
			this.Status = MigrationHelper.GetEnumProperty<TStatus>(message, MigrationBatchMessageSchema.MigrationUserStatus);
			this.TransientErrorCount = message.GetValueOrDefault<int>(MigrationBatchMessageSchema.MigrationJobItemTransientErrorCount, 0);
			this.PreviousStatus = MigrationHelper.GetEnumPropertyOrNull<TStatus>(message, MigrationBatchMessageSchema.MigrationJobItemPreviousStatus);
			this.InternalError = message.GetValueOrDefault<string>(MigrationBatchMessageSchema.MigrationJobInternalError, null);
			this.StateLastUpdated = MigrationHelperBase.GetExDateTimeProperty(message, MigrationBatchMessageSchema.MigrationJobItemStateLastUpdated);
			this.InternalErrorTime = MigrationHelper.GetExDateTimePropertyOrNull(message, MigrationBatchMessageSchema.MigrationJobInternalErrorTime);
			this.StatusHistory = message.GetValueOrDefault<string>(MigrationBatchMessageSchema.MigrationJobItemStatusHistory, string.Empty);
			this.SameStatusCount = message.GetValueOrDefault<int>(MigrationBatchMessageSchema.MigrationSameStatusCount, 1);
			this.WatsonHash = message.GetValueOrDefault<string>(MigrationBatchMessageSchema.MigrationStatusDataFailureWatsonHash, null);
			string valueOrDefault = message.GetValueOrDefault<string>(MigrationBatchMessageSchema.MigrationFailureRecord, string.Empty);
			if (string.IsNullOrEmpty(valueOrDefault))
			{
				string valueOrDefault2 = message.GetValueOrDefault<string>(MigrationBatchMessageSchema.MigrationJobItemLocalizedError, null);
				if (!string.IsNullOrEmpty(valueOrDefault2))
				{
					LocalizedString localizedErrorMessage = new LocalizedString(valueOrDefault2);
					MigrationPermanentException ex = new MigrationPermanentException(localizedErrorMessage);
					this.FailureRecord = FailureRec.Create(ex);
				}
			}
			else
			{
				this.FailureRecord = XMLSerializableBase.Deserialize<FailureRec>(valueOrDefault, MigrationBatchMessageSchema.MigrationFailureRecord);
			}
			return true;
		}

		public void WriteToMessageItem(IMigrationStoreObject message, bool loaded)
		{
			MigrationUtil.ThrowOnNullArgument(message, "message");
			message[MigrationBatchMessageSchema.MigrationUserStatus] = this.Status;
			message[MigrationBatchMessageSchema.MigrationJobItemTransientErrorCount] = this.TransientErrorCount;
			message[MigrationBatchMessageSchema.MigrationJobItemStatusHistory] = this.StatusHistory;
			message[MigrationBatchMessageSchema.MigrationSameStatusCount] = this.SameStatusCount;
			if (this.PreviousStatus != null)
			{
				message[MigrationBatchMessageSchema.MigrationJobItemPreviousStatus] = this.PreviousStatus.Value;
			}
			if (this.StateLastUpdated != null)
			{
				MigrationHelperBase.SetExDateTimeProperty(message, MigrationBatchMessageSchema.MigrationJobItemStateLastUpdated, this.StateLastUpdated);
			}
			if (loaded && string.IsNullOrEmpty(this.InternalError))
			{
				message.Delete(MigrationBatchMessageSchema.MigrationJobInternalError);
			}
			else if (!string.IsNullOrEmpty(this.InternalError))
			{
				message[MigrationBatchMessageSchema.MigrationJobInternalError] = this.InternalError;
			}
			if (loaded && this.InternalErrorTime == null)
			{
				message.Delete(MigrationBatchMessageSchema.MigrationJobInternalErrorTime);
			}
			else if (this.InternalErrorTime != null)
			{
				MigrationHelperBase.SetExDateTimeProperty(message, MigrationBatchMessageSchema.MigrationJobInternalErrorTime, this.InternalErrorTime);
			}
			if (loaded && this.FailureRecord == null)
			{
				message.Delete(MigrationBatchMessageSchema.MigrationFailureRecord);
			}
			else if (this.FailureRecord != null)
			{
				message[MigrationBatchMessageSchema.MigrationFailureRecord] = MigrationXmlSerializer.Serialize(this.FailureRecord);
			}
			if (loaded && string.IsNullOrEmpty(this.WatsonHash))
			{
				message.Delete(MigrationBatchMessageSchema.MigrationStatusDataFailureWatsonHash);
			}
			else if (!string.IsNullOrEmpty(this.WatsonHash))
			{
				message[MigrationBatchMessageSchema.MigrationStatusDataFailureWatsonHash] = this.WatsonHash;
			}
			if (this.Version >= 2L)
			{
				message[MigrationBatchMessageSchema.MigrationState] = this.State;
			}
		}

		public XElement GetDiagnosticInfo(IMigrationDataProvider dataProvider, MigrationDiagnosticArgument argument)
		{
			XElement xelement = new XElement("MigrationStatusData", new object[]
			{
				new XElement("state", this.State),
				new XElement("status", this.Status),
				new XElement("stateLastUpdated", this.StateLastUpdated),
				new XElement("previousStatus", this.PreviousStatus),
				new XElement("error", this.LocalizedError),
				new XElement("transientErrorCount", this.TransientErrorCount),
				new XElement("SameStatusCount", this.SameStatusCount),
				new XElement("internalError", this.InternalError),
				new XElement("internalErrorTime", this.InternalErrorTime),
				new XElement("statusHistory", this.StatusHistory),
				new XElement("watsonHash", this.WatsonHash)
			});
			XElement xelement2 = new XElement("FailureRecord");
			if (this.FailureRecord != null)
			{
				xelement2.Add(this.FailureRecord.GetDiagnosticData());
			}
			xelement.Add(xelement2);
			return xelement;
		}

		public void ClearTransientErrorCount()
		{
			MigrationEventType eventType = (this.TransientErrorCount > 0) ? MigrationEventType.Information : MigrationEventType.Verbose;
			MigrationLogger.Log(eventType, "resetting transient error count, former {0}", new object[]
			{
				this.TransientErrorCount
			});
			this.TransientErrorCount = 0;
		}

		public void ClearError()
		{
			MigrationLogger.Log(MigrationEventType.Verbose, "clearing error, former {0}", new object[]
			{
				this
			});
			this.StateLastUpdated = new ExDateTime?(ExDateTime.UtcNow);
			this.InternalError = null;
			this.InternalErrorTime = null;
			this.FailureRecord = null;
			this.ClearTransientErrorCount();
		}

		public bool UpdateStatus(TStatus status, MigrationState? state = null)
		{
			return this.InternalUpdateStatus(status, null, null, false, state);
		}

		public bool RevertStatus()
		{
			if (this.PreviousStatus != null)
			{
				MigrationLogger.Log(MigrationEventType.Information, "reverting status: from {0} to {1}", new object[]
				{
					this.PreviousStatus,
					this.Status
				});
				return this.UpdateStatus(this.PreviousStatus.Value, null);
			}
			return false;
		}

		public void SetTransientError(Exception error, TStatus? status = null, MigrationState? state = null)
		{
			MigrationUtil.ThrowOnNullArgument(error, "error");
			if (this.StateLastUpdated == null || this.TransientErrorCount <= 0)
			{
				this.ResetStateLastUpdated();
			}
			if (status != null && state != null)
			{
				TStatus status2 = this.Status;
				if (!status2.Equals(status.Value) || !this.State.Equals(state.Value))
				{
					this.ResetStateLastUpdated();
					this.PreviousStatus = new TStatus?(this.Status);
					this.Status = status.Value;
					this.State = state.Value;
					this.SameStatusCount = 1;
				}
				else
				{
					this.SameStatusCount++;
				}
			}
			this.TransientErrorCount++;
			this.FailureRecord = FailureRec.Create(error);
			MigrationLogger.Log(MigrationEventType.Warning, "Set TransientError: {0} count {1} original time {2}", new object[]
			{
				this,
				this.TransientErrorCount,
				this.StateLastUpdated
			});
		}

		public void SetFailedStatus(TStatus failureStatus, Exception exception, string internalError, MigrationState? state = null)
		{
			MigrationUtil.ThrowOnNullOrEmptyArgument(internalError, "internalError");
			this.UpdateStatus(failureStatus, exception, internalError, false, state);
			MigrationLogger.Log(MigrationEventType.Warning, "set corrupt status {0}", new object[]
			{
				this
			});
		}

		public bool UpdateStatus(TStatus status, Exception localizedError, string internalError, MigrationState? state = null)
		{
			return this.UpdateStatus(status, localizedError, internalError, false, state);
		}

		public bool UpdateStatus(TStatus status, Exception exception, string internalError, bool forceUpdate, MigrationState? state = null)
		{
			if (internalError != null)
			{
				internalError = MigrationLogger.GetDiagnosticInfo(new StackTrace(), internalError);
			}
			return this.InternalUpdateStatus(status, exception, internalError, forceUpdate, state);
		}

		internal static MigrationStatusData<TStatus> Create(IMigrationStoreObject message, long version)
		{
			MigrationStatusData<TStatus> migrationStatusData = new MigrationStatusData<TStatus>(version);
			migrationStatusData.ReadFromMessageItem(message);
			return migrationStatusData;
		}

		private static FailureRec GetFailureRecord(Exception exception)
		{
			return FailureRec.Create(exception);
		}

		private bool InternalUpdateStatus(TStatus status, Exception exception, string internalError, bool forceUpdate, MigrationState? state = null)
		{
			if (exception != null && string.IsNullOrEmpty(internalError))
			{
				throw new ArgumentException("when setting localized error, always set internal error as well");
			}
			bool flag = false;
			FailureRec failureRec = FailureRec.Create(exception);
			string x = (this.FailureRecord == null) ? string.Empty : this.FailureRecord.FailureType;
			bool flag2 = failureRec == null || !StringComparer.InvariantCultureIgnoreCase.Equals(x, failureRec.FailureType);
			MigrationState migrationState = (state != null) ? state.Value : this.State;
			TStatus status2 = this.Status;
			if (!status2.Equals(status) || !this.State.Equals(migrationState))
			{
				this.ResetStateLastUpdated();
				this.PreviousStatus = new TStatus?(this.Status);
				this.Status = status;
				this.State = migrationState;
				this.SameStatusCount = 1;
				MigrationLogger.Log(MigrationEventType.Information, "update status: {0} previous status {1}", new object[]
				{
					this,
					this.PreviousStatus
				});
				if (flag2)
				{
					this.WatsonHash = null;
					this.ClearError();
				}
				flag = true;
			}
			else
			{
				if (forceUpdate)
				{
					this.ResetStateLastUpdated();
				}
				this.SameStatusCount++;
			}
			this.ClearTransientErrorCount();
			if (flag2)
			{
				this.FailureRecord = MigrationStatusData<TStatus>.GetFailureRecord(exception);
			}
			if (!string.IsNullOrEmpty(internalError))
			{
				this.SetInternalError(internalError, flag);
			}
			if (exception != null)
			{
				string watsonHash;
				if (ConfigBase<MigrationServiceConfigSchema>.GetConfig<bool>("SendGenericWatson"))
				{
					CommonUtils.SendGenericWatson(exception, internalError, out watsonHash);
				}
				else
				{
					watsonHash = CommonUtils.ComputeCallStackHash(exception, 5);
				}
				this.WatsonHash = watsonHash;
			}
			MigrationLogger.Log(MigrationEventType.Verbose, "set status {0}, did an update occur {1}", new object[]
			{
				this,
				flag
			});
			return flag;
		}

		private void ResetStateLastUpdated()
		{
			ExDateTime utcNow = ExDateTime.UtcNow;
			if (this.StateLastUpdated == null)
			{
				this.StateLastUpdated = new ExDateTime?(utcNow);
			}
			this.StatusHistory = MigrationHelper.AppendDiagnosticHistory(this.StatusHistory, new string[]
			{
				Convert.ToInt32(this.Status).ToString(CultureInfo.InvariantCulture),
				string.Format("{0:N0}", (utcNow - this.StateLastUpdated.Value).TotalSeconds)
			});
			this.StateLastUpdated = new ExDateTime?(utcNow);
		}

		private void SetInternalError(string internalError, bool useStateLastUpdated)
		{
			if (this.StateLastUpdated == null)
			{
				throw new MigrationDataCorruptionException(string.Format("expect state last updated to be set {0}", this));
			}
			ExDateTime value = useStateLastUpdated ? this.StateLastUpdated.Value : ExDateTime.UtcNow;
			this.InternalError = internalError;
			this.InternalErrorTime = new ExDateTime?(value);
		}

		internal const long MinimumVersion = 1L;

		internal const long PAWVersion = 2L;

		internal static readonly PropertyDefinition[] StatusPropertyDefinition = new PropertyDefinition[]
		{
			MigrationBatchMessageSchema.MigrationUserStatus,
			MigrationBatchMessageSchema.MigrationJobItemStateLastUpdated,
			MigrationBatchMessageSchema.MigrationJobItemTransientErrorCount,
			MigrationBatchMessageSchema.MigrationJobItemPreviousStatus,
			MigrationBatchMessageSchema.MigrationJobItemLocalizedError,
			MigrationBatchMessageSchema.MigrationJobItemLocalizedErrorID,
			MigrationBatchMessageSchema.MigrationJobInternalError,
			MigrationBatchMessageSchema.MigrationJobInternalErrorTime,
			MigrationBatchMessageSchema.MigrationJobItemStatusHistory,
			MigrationBatchMessageSchema.MigrationJobItemLocalizedMessage,
			MigrationBatchMessageSchema.MigrationJobItemLocalizedMessageID,
			MigrationBatchMessageSchema.MigrationSameStatusCount,
			MigrationBatchMessageSchema.MigrationFailureRecord,
			MigrationBatchMessageSchema.MigrationStatusDataFailureWatsonHash,
			MigrationBatchMessageSchema.MigrationState
		};
	}
}
