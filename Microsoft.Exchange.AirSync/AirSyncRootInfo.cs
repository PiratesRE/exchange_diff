using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AirSync;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.AirSync
{
	internal class AirSyncRootInfo : SyncStateDataInfo
	{
		public AirSyncRootInfo(CustomSyncState wrappedState) : base(wrappedState)
		{
		}

		public ExDateTime? LastMaxDevicesExceededMailSentTime
		{
			get
			{
				return base.Fetch<NullableData<DateTimeData, ExDateTime>, ExDateTime?>(CustomStateDatumType.LastMaxDevicesExceededMailSentTime, null);
			}
			set
			{
				base.Assign<NullableData<DateTimeData, ExDateTime>, ExDateTime?>(CustomStateDatumType.LastMaxDevicesExceededMailSentTime, value);
			}
		}

		public static AirSyncRootInfo LoadFromMailbox(MailboxSession mailboxSession, SyncStateRootStorage syncStateRootStorage)
		{
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			ArgumentValidator.ThrowIfNull("syncStateRootStorage", syncStateRootStorage);
			AirSyncRootInfo airSyncRootInfo = null;
			CustomSyncState customSyncState = null;
			bool flag = false;
			AirSyncRootInfo result;
			try
			{
				bool isDirty = false;
				AirSyncRootSyncStateInfo syncStateInfo = new AirSyncRootSyncStateInfo();
				customSyncState = syncStateRootStorage.GetCustomSyncState(syncStateInfo);
				if (customSyncState == null)
				{
					AirSyncDiagnostics.TraceDebug(ExTraceGlobals.RequestsTracer, null, "[AirSyncRootInfo.LoadFromMailbox] Had to create root sync state.");
					customSyncState = syncStateRootStorage.CreateCustomSyncState(syncStateInfo);
					isDirty = true;
				}
				airSyncRootInfo = new AirSyncRootInfo(customSyncState);
				airSyncRootInfo.IsDirty = isDirty;
				flag = true;
				result = airSyncRootInfo;
			}
			finally
			{
				if (!flag)
				{
					if (airSyncRootInfo != null)
					{
						airSyncRootInfo.Dispose();
					}
					else if (customSyncState != null)
					{
						customSyncState.Dispose();
					}
				}
			}
			return result;
		}
	}
}
