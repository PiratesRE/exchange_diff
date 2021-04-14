using System;
using System.Collections.Generic;
using Cobalt;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class CobaltServerLockingStore : HostLockingStore
	{
		public CobaltServerLockingStore(CobaltStore blobStore)
		{
			this.blobStore = blobStore;
			this.editorsTable = new Dictionary<string, EditorsTableEntry>();
			this.editorsTableWaterline = 0UL;
		}

		public override ulong GetEditorsTableWaterline()
		{
			return this.editorsTableWaterline;
		}

		public override AmIAloneRequest.OutputType HandleAmIAlone(AmIAloneRequest.InputType input)
		{
			return new AmIAloneRequest.OutputType
			{
				AmIAlone = true
			};
		}

		public override CheckCoauthLockAvailabilityRequest.OutputType HandleCheckCoauthLockAvailability(CheckCoauthLockAvailabilityRequest.InputType input)
		{
			return new CheckCoauthLockAvailabilityRequest.OutputType();
		}

		public override CheckExclusiveLockAvailabilityRequest.OutputType HandleCheckExclusiveLockAvailability(CheckExclusiveLockAvailabilityRequest.InputType input)
		{
			return new CheckExclusiveLockAvailabilityRequest.OutputType();
		}

		public override CheckSchemaLockAvailabilityRequest.OutputType HandleCheckSchemaLockAvailability(CheckSchemaLockAvailabilityRequest.InputType input)
		{
			return new CheckSchemaLockAvailabilityRequest.OutputType();
		}

		public override ConvertCoauthLockToExclusiveLockRequest.OutputType HandleConvertCoauthLockToExclusiveLock(ConvertCoauthLockToExclusiveLockRequest.InputType input)
		{
			return new ConvertCoauthLockToExclusiveLockRequest.OutputType();
		}

		public override ConvertExclusiveLockToSchemaLockRequest.OutputType HandleConvertExclusiveLockToSchemaLock(ConvertExclusiveLockToSchemaLockRequest.InputType input, int protocolMajorVersion, int protocolMinorVersion)
		{
			return new ConvertExclusiveLockToSchemaLockRequest.OutputType();
		}

		public override ConvertExclusiveLockWithCoauthTransitionRequest.OutputType HandleConvertExclusiveLockWithCoauthTransition(ConvertExclusiveLockWithCoauthTransitionRequest.InputType input, int protocolMajorVersion, int protocolMinorVersion)
		{
			return new ConvertExclusiveLockWithCoauthTransitionRequest.OutputType();
		}

		public override ConvertSchemaLockToExclusiveLockRequest.OutputType HandleConvertSchemaLockToExclusiveLock(ConvertSchemaLockToExclusiveLockRequest.InputType input)
		{
			return new ConvertSchemaLockToExclusiveLockRequest.OutputType();
		}

		public override DocMetaInfoRequest.OutputType HandleDocMetaInfo(DocMetaInfoRequest.InputType input)
		{
			return new DocMetaInfoRequest.OutputType();
		}

		public override ExitCoauthoringRequest.OutputType HandleExitCoauthoring(ExitCoauthoringRequest.InputType input, int protocolMajorVersion, int protocolMinorVersion)
		{
			if (this.editorsTable.ContainsKey(input.ClientId.ToString()))
			{
				this.editorsTable.Remove(input.ClientId.ToString());
			}
			return new ExitCoauthoringRequest.OutputType();
		}

		public override GetCoauthoringStatusRequest.OutputType HandleGetCoauthoringStatus(GetCoauthoringStatusRequest.InputType input)
		{
			return new GetCoauthoringStatusRequest.OutputType
			{
				CoauthStatus = 1
			};
		}

		public override GetExclusiveLockRequest.OutputType HandleGetExclusiveLock(GetExclusiveLockRequest.InputType input)
		{
			return new GetExclusiveLockRequest.OutputType();
		}

		public override GetSchemaLockRequest.OutputType HandleGetSchemaLock(GetSchemaLockRequest.InputType input, int protocolMajorVersion, int protocolMinorVersion)
		{
			return new GetSchemaLockRequest.OutputType
			{
				ExclusiveLockReturnReason = 3,
				Lock = 1
			};
		}

		public override JoinCoauthoringRequest.OutputType HandleJoinCoauthoring(JoinCoauthoringRequest.InputType input, int protocolMajorVersion, int protocolMinorVersion)
		{
			EditorsTableEntry editorsTableEntry = this.CreateEditorsTableEntry();
			this.editorsTable.Add(editorsTableEntry.ClientId, editorsTableEntry);
			this.editorsTableWaterline += 1UL;
			return new JoinCoauthoringRequest.OutputType
			{
				CoauthStatus = 1,
				ExclusiveLockReturnReason = 0,
				Lock = 1,
				TransitionId = Guid.NewGuid()
			};
		}

		public override JoinEditingSessionRequest.OutputType HandleJoinEditingSession(JoinEditingSessionRequest.InputType input)
		{
			return new JoinEditingSessionRequest.OutputType();
		}

		public override LeaveEditingSessionRequest.OutputType HandleLeaveEditingSession(LeaveEditingSessionRequest.InputType input)
		{
			return new LeaveEditingSessionRequest.OutputType();
		}

		public override LockAndCheckOutStatusRequest.OutputType HandleLockAndCheckOutStatus(LockAndCheckOutStatusRequest.InputType input)
		{
			return new LockAndCheckOutStatusRequest.OutputType();
		}

		public override MarkCoauthTransitionCompleteRequest.OutputType HandleMarkCoauthTransitionComplete(MarkCoauthTransitionCompleteRequest.InputType input)
		{
			return new MarkCoauthTransitionCompleteRequest.OutputType();
		}

		public override RefreshCoauthoringSessionRequest.OutputType HandleRefreshCoauthoring(RefreshCoauthoringSessionRequest.InputType input, int protocolMajorVersion, int protocolMinorVersion)
		{
			return new RefreshCoauthoringSessionRequest.OutputType();
		}

		public override RefreshEditingSessionRequest.OutputType HandleRefreshEditingSession(RefreshEditingSessionRequest.InputType input)
		{
			return new RefreshEditingSessionRequest.OutputType();
		}

		public override RefreshExclusiveLockRequest.OutputType HandleRefreshExclusiveLock(RefreshExclusiveLockRequest.InputType input)
		{
			return new RefreshExclusiveLockRequest.OutputType();
		}

		public override RefreshSchemaLockRequest.OutputType HandleRefreshSchemaLock(RefreshSchemaLockRequest.InputType input, int protocolMajorVersion, int protocolMinorVersion)
		{
			return new RefreshSchemaLockRequest.OutputType();
		}

		public override ReleaseExclusiveLockRequest.OutputType HandleReleaseExclusiveLock(ReleaseExclusiveLockRequest.InputType input)
		{
			return new ReleaseExclusiveLockRequest.OutputType();
		}

		public override ReleaseSchemaLockRequest.OutputType HandleReleaseSchemaLock(ReleaseSchemaLockRequest.InputType input, int protocolMajorVersion, int protocolMinorVersion)
		{
			return new ReleaseSchemaLockRequest.OutputType();
		}

		public override RemoveEditorMetadataRequest.OutputType HandleRemoveEditorMetadata(RemoveEditorMetadataRequest.InputType input)
		{
			RemoveEditorMetadataRequest.OutputType result = new RemoveEditorMetadataRequest.OutputType();
			EditorsTableEntry editorsTableEntry;
			if (this.editorsTable.TryGetValue(input.ClientId.ToString(), out editorsTableEntry))
			{
				editorsTableEntry.Properties.Remove(input.Key);
			}
			return result;
		}

		public override ServerTimeRequest.OutputType HandleServerTime(ServerTimeRequest.InputType input)
		{
			return new ServerTimeRequest.OutputType
			{
				ServerTime = (DateTime)ExDateTime.Now
			};
		}

		public override UpdateEditorMetadataRequest.OutputType HandleUpdateEditorMetadata(UpdateEditorMetadataRequest.InputType input)
		{
			UpdateEditorMetadataRequest.OutputType result = new UpdateEditorMetadataRequest.OutputType();
			EditorsTableEntry editorsTableEntry;
			if (this.editorsTable.TryGetValue(input.ClientId.ToString(), out editorsTableEntry))
			{
				editorsTableEntry.Properties[input.Key] = input.Value;
			}
			return result;
		}

		public override VersionsRequest.OutputType HandleVersions(VersionsRequest.InputType input)
		{
			return new VersionsRequest.OutputType
			{
				Enabled = false
			};
		}

		public override WhoAmIRequest.OutputType HandleWhoAmI(WhoAmIRequest.InputType input)
		{
			return new WhoAmIRequest.OutputType
			{
				UserEmailAddress = this.blobStore.OwnerEmailAddress,
				UserIsAnonymous = false,
				UserLogin = this.blobStore.OwnerEmailAddress,
				UserName = this.blobStore.OwnerEmailAddress,
				UserSipAddress = "sip:" + this.blobStore.OwnerEmailAddress
			};
		}

		public override Dictionary<string, EditorsTableEntry> QueryEditorsTable()
		{
			return this.editorsTable;
		}

		private EditorsTableEntry CreateEditorsTableEntry()
		{
			return new EditorsTableEntry
			{
				ClientId = Guid.NewGuid().ToString(),
				DtTimeout = (DateTime)ExDateTime.Now.Add(new TimeSpan(1, 0, 0, 0)),
				EmailAddress = this.blobStore.OwnerEmailAddress,
				HasEditPermission = true,
				Properties = new Dictionary<string, Atom>(),
				SipAddress = "sip:" + this.blobStore.OwnerEmailAddress,
				UserLogin = this.blobStore.OwnerEmailAddress,
				UserName = this.blobStore.OwnerEmailAddress
			};
		}

		private CobaltStore blobStore;

		private Dictionary<string, EditorsTableEntry> editorsTable;

		private ulong editorsTableWaterline;
	}
}
