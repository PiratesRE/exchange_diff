using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.AirSync;

namespace Microsoft.Exchange.AirSync
{
	internal class MeetingOrganizerSyncState : SyncStateDataInfo
	{
		public MeetingOrganizerSyncState(CustomSyncState wrappedSyncState) : base(wrappedSyncState)
		{
		}

		public MeetingOrganizerInfo MeetingOrganizerInfo
		{
			get
			{
				MeetingOrganizerInfo meetingOrganizerInfo = base.Fetch<MeetingOrganizerInfoData, MeetingOrganizerInfo>(CustomStateDatumType.MeetingOrganizerInfo, null);
				if (meetingOrganizerInfo == null)
				{
					meetingOrganizerInfo = new MeetingOrganizerInfo();
					base.Assign<MeetingOrganizerInfoData, MeetingOrganizerInfo>(CustomStateDatumType.MeetingOrganizerInfo, meetingOrganizerInfo);
				}
				return meetingOrganizerInfo;
			}
			set
			{
				base.Assign<MeetingOrganizerInfoData, MeetingOrganizerInfo>(CustomStateDatumType.MeetingOrganizerInfo, value);
			}
		}

		public static MeetingOrganizerSyncState LoadFromMailbox(MailboxSession mailboxSession, SyncStateStorage syncStateStorage, ProtocolLogger protocolLogger)
		{
			if (mailboxSession == null)
			{
				throw new ArgumentNullException("mailboxSession");
			}
			if (syncStateStorage == null)
			{
				throw new ArgumentNullException("syncStateStorage");
			}
			bool flag = false;
			MeetingOrganizerSyncState meetingOrganizerSyncState = null;
			CustomSyncState customSyncState = null;
			MeetingOrganizerSyncState result;
			try
			{
				MeetingOrganizerSyncStateInfo meetingOrganizerSyncStateInfo = new MeetingOrganizerSyncStateInfo();
				bool flag2 = false;
				ExTraceGlobals.FaultInjectionTracer.TraceTest<bool>(3104189757U, ref flag2);
				if (flag2)
				{
					throw new CorruptSyncStateException(meetingOrganizerSyncStateInfo.UniqueName, new LocalizedString("FaultInjection"));
				}
				bool isDirty = false;
				customSyncState = syncStateStorage.GetCustomSyncState(meetingOrganizerSyncStateInfo, new PropertyDefinition[0]);
				if (customSyncState == null)
				{
					customSyncState = syncStateStorage.CreateCustomSyncState(meetingOrganizerSyncStateInfo);
					isDirty = true;
				}
				else if (customSyncState.BackendVersion != null && customSyncState.BackendVersion.Value != customSyncState.Version)
				{
					isDirty = true;
				}
				meetingOrganizerSyncState = new MeetingOrganizerSyncState(customSyncState);
				meetingOrganizerSyncState.IsDirty = isDirty;
				flag = true;
				result = meetingOrganizerSyncState;
			}
			finally
			{
				if (!flag)
				{
					if (meetingOrganizerSyncState != null)
					{
						meetingOrganizerSyncState.Dispose();
					}
					else if (customSyncState != null)
					{
						customSyncState.Dispose();
					}
				}
			}
			return result;
		}

		private const uint lidCorruptMeetingOrganizerSyncState = 3104189757U;
	}
}
