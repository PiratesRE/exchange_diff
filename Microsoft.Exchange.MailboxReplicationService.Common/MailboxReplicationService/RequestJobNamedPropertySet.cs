using System;
using Microsoft.Mapi;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class RequestJobNamedPropertySet
	{
		static RequestJobNamedPropertySet()
		{
			RequestJobNamedPropertySet.npData = new NamedPropertyData[26];
			RequestJobNamedPropertySet.npData[0] = new NamedPropertyData(new Guid("A7B04B86-0B51-4511-A381-ABC6D4E3C1DB"), "MailboxMoveStatus", PropType.Int);
			RequestJobNamedPropertySet.npData[1] = new NamedPropertyData(new Guid("686C4F8A-F3DC-40a9-B078-EC253E481166"), "MoveState", PropType.Int);
			RequestJobNamedPropertySet.npData[2] = new NamedPropertyData(new Guid("7B307DDD-13BF-4eeb-B44C-D9C8384D4372"), "MoveServerName", PropType.String);
			RequestJobNamedPropertySet.npData[3] = new NamedPropertyData(new Guid("8A481DE6-1A0C-4896-A5DC-7E0E5C90C98D"), "AllowedToFinishMove", PropType.Boolean);
			RequestJobNamedPropertySet.npData[4] = new NamedPropertyData(new Guid("55AEADFF-F6D7-4033-A4AC-8AD176E5D5A4"), "CancelMove", PropType.Boolean);
			RequestJobNamedPropertySet.npData[5] = new NamedPropertyData(new Guid("E9A77F63-CEC9-469a-9B01-25740A1BA47A"), "ExchangeGuid", PropType.Guid);
			RequestJobNamedPropertySet.npData[7] = new NamedPropertyData(new Guid("D82803CC-312B-4ce1-BFB9-62FC6852987F"), "LastUpdateTimestamp", PropType.SysTime);
			RequestJobNamedPropertySet.npData[8] = new NamedPropertyData(new Guid("1553C2E6-0E94-4805-A478-5781B04D83B5"), "CreationTimestamp", PropType.SysTime);
			RequestJobNamedPropertySet.npData[9] = new NamedPropertyData(new Guid("32779377-b9a0-45c5-b45c-a71f2f7e2fd0"), "JobType", PropType.Int);
			RequestJobNamedPropertySet.npData[10] = new NamedPropertyData(new Guid("920D906B-8F43-4abe-9B32-B4197E0BEE8F"), "MailboxMoveFlags", PropType.Int);
			RequestJobNamedPropertySet.npData[11] = new NamedPropertyData(new Guid("94216575-14A4-42ae-A19C-84A02166C7B8"), "MailboxMoveSourceMDB", PropType.Guid);
			RequestJobNamedPropertySet.npData[12] = new NamedPropertyData(new Guid("90EC3564-448C-4b89-9856-3BA248CD6E75"), "MailboxMoveTargetMDB", PropType.Guid);
			RequestJobNamedPropertySet.npData[13] = new NamedPropertyData(new Guid("18BE3CE8-7B58-47f1-802C-F678EC1F919F"), "DoNotPickUntilTimestamp", PropType.SysTime);
			RequestJobNamedPropertySet.npData[14] = new NamedPropertyData(new Guid("42688e07-12bc-43a1-a5d0-301065db781d"), "RequestType", PropType.Int);
			RequestJobNamedPropertySet.npData[15] = new NamedPropertyData(new Guid("763F3EAB-D6FA-41fe-A317-7A971B529B92"), "SourceArchiveDatabase", PropType.Guid);
			RequestJobNamedPropertySet.npData[16] = new NamedPropertyData(new Guid("42688e07-12bc-43a1-a5d0-301065db781d"), "TargetArchiveDatabase", PropType.Guid);
			RequestJobNamedPropertySet.npData[17] = new NamedPropertyData(new Guid("efdbed05-c980-437f-a785-d7ab7f045451"), "Priority", PropType.Int);
			RequestJobNamedPropertySet.npData[18] = new NamedPropertyData(new Guid("{6d4203d5-0234-48bb-a2a2-38cb9a4051df}"), "SourceExchangeGuid", PropType.Guid);
			RequestJobNamedPropertySet.npData[19] = new NamedPropertyData(new Guid("{cbeef357-24db-4730-9bde-da2081bb043b}"), "TargetExchangeGuid", PropType.Guid);
			RequestJobNamedPropertySet.npData[20] = new NamedPropertyData(RequestJobNamedPropertySet.MRSNamedPropGuid, "RehomeRequest", PropType.Boolean);
			RequestJobNamedPropertySet.npData[21] = new NamedPropertyData(RequestJobNamedPropertySet.MRSNamedPropGuid, "InternalFlags", PropType.Int);
			RequestJobNamedPropertySet.npData[22] = new NamedPropertyData(RequestJobNamedPropertySet.MRSNamedPropGuid, "PartitionHint", PropType.Binary);
			RequestJobNamedPropertySet.npData[23] = new NamedPropertyData(RequestJobNamedPropertySet.MRSNamedPropGuid, "PoisonCount", PropType.Int);
			RequestJobNamedPropertySet.npData[6] = new NamedPropertyData(RequestJobNamedPropertySet.MRSNamedPropGuid, "ArchiveGuid", PropType.Guid);
			RequestJobNamedPropertySet.npData[24] = new NamedPropertyData(RequestJobNamedPropertySet.MRSNamedPropGuid, "FailureType", PropType.String);
			RequestJobNamedPropertySet.npData[25] = new NamedPropertyData(RequestJobNamedPropertySet.MRSNamedPropGuid, "WorkloadType", PropType.Int);
		}

		private RequestJobNamedPropertySet(MapiStore systemMailbox)
		{
			NamedProp[] array = new NamedProp[26];
			int num = 0;
			foreach (NamedPropertyData namedPropertyData in RequestJobNamedPropertySet.npData)
			{
				array[num] = new NamedProp(namedPropertyData.NPGuid, namedPropertyData.NPName);
				num++;
			}
			PropTag[] idsFromNames = systemMailbox.GetIDsFromNames(true, array);
			this.PropTags = new PropTag[28];
			num = 0;
			foreach (NamedPropertyData namedPropertyData2 in RequestJobNamedPropertySet.npData)
			{
				this.PropTags[num] = PropTagHelper.PropTagFromIdAndType(idsFromNames[num].Id(), namedPropertyData2.NPType);
				num++;
			}
			this.PropTags[26] = PropTag.ReplyTemplateID;
			this.PropTags[27] = PropTag.EntryId;
		}

		public PropTag[] PropTags { get; private set; }

		public static RequestJobNamedPropertySet Get(MapiStore systemMailbox)
		{
			byte[] signatureBytes = MapiUtils.GetSignatureBytes(systemMailbox);
			RequestJobNamedPropertySet result;
			lock (RequestJobNamedPropertySet.locker)
			{
				RequestJobNamedPropertySet requestJobNamedPropertySet;
				if (signatureBytes != null && RequestJobNamedPropertySet.mappings.TryGetValue(signatureBytes, out requestJobNamedPropertySet))
				{
					result = requestJobNamedPropertySet;
				}
				else
				{
					requestJobNamedPropertySet = new RequestJobNamedPropertySet(systemMailbox);
					if (signatureBytes != null)
					{
						RequestJobNamedPropertySet.mappings.Add(signatureBytes, requestJobNamedPropertySet);
					}
					result = requestJobNamedPropertySet;
				}
			}
			return result;
		}

		public PropValue[] GetValuesFromRequestJob(RequestJobBase requestJob)
		{
			PropValue[] result = new PropValue[26];
			this.MakePropValue(result, MapiPropertiesIndex.RequestStatus, requestJob.Status);
			this.MakePropValue(result, MapiPropertiesIndex.JobProcessingState, requestJob.RequestJobState);
			this.MakePropValue(result, MapiPropertiesIndex.MRSServerName, requestJob.MRSServerName);
			this.MakePropValue(result, MapiPropertiesIndex.AllowedToFinishMove, requestJob.AllowedToFinishMove);
			this.MakePropValue(result, MapiPropertiesIndex.CancelRequest, requestJob.CancelRequest);
			this.MakePropValue(result, MapiPropertiesIndex.ExchangeGuid, requestJob.ExchangeGuid);
			this.MakePropValue(result, MapiPropertiesIndex.ArchiveGuid, requestJob.ArchiveGuid ?? Guid.Empty);
			this.MakePropValue(result, MapiPropertiesIndex.LastUpdateTimestamp, requestJob.TimeTracker.GetTimestamp(RequestJobTimestamp.LastUpdate) ?? DateTime.MinValue);
			this.MakePropValue(result, MapiPropertiesIndex.CreationTimestamp, requestJob.TimeTracker.GetTimestamp(RequestJobTimestamp.Creation) ?? DateTime.MinValue);
			this.MakePropValue(result, MapiPropertiesIndex.JobType, requestJob.JobType);
			this.MakePropValue(result, MapiPropertiesIndex.Flags, requestJob.Flags);
			this.MakePropValue(result, MapiPropertiesIndex.SourceDatabase, (requestJob.SourceDatabase != null) ? requestJob.SourceDatabase.ObjectGuid : Guid.Empty);
			this.MakePropValue(result, MapiPropertiesIndex.TargetDatabase, (requestJob.TargetDatabase != null) ? requestJob.TargetDatabase.ObjectGuid : Guid.Empty);
			this.MakePropValue(result, MapiPropertiesIndex.DoNotPickUntilTimestamp, requestJob.NextPickupTime ?? DateTime.MinValue);
			this.MakePropValue(result, MapiPropertiesIndex.RequestType, requestJob.RequestType);
			this.MakePropValue(result, MapiPropertiesIndex.SourceArchiveDatabase, (requestJob.SourceArchiveDatabase != null) ? requestJob.SourceArchiveDatabase.ObjectGuid : Guid.Empty);
			this.MakePropValue(result, MapiPropertiesIndex.TargetArchiveDatabase, (requestJob.TargetArchiveDatabase != null) ? requestJob.TargetArchiveDatabase.ObjectGuid : Guid.Empty);
			this.MakePropValue(result, MapiPropertiesIndex.Priority, requestJob.Priority);
			this.MakePropValue(result, MapiPropertiesIndex.SourceExchangeGuid, requestJob.SourceExchangeGuid);
			this.MakePropValue(result, MapiPropertiesIndex.TargetExchangeGuid, requestJob.TargetExchangeGuid);
			this.MakePropValue(result, MapiPropertiesIndex.RehomeRequest, requestJob.RehomeRequest);
			this.MakePropValue(result, MapiPropertiesIndex.InternalFlags, requestJob.RequestJobInternalFlags);
			this.MakePropValue(result, MapiPropertiesIndex.PartitionHint, (requestJob.PartitionHint != null) ? requestJob.PartitionHint.GetPersistablePartitionHint() : null);
			this.MakePropValue(result, MapiPropertiesIndex.PoisonCount, requestJob.PoisonCount);
			this.MakePropValue(result, MapiPropertiesIndex.FailureType, requestJob.FailureType);
			this.MakePropValue(result, MapiPropertiesIndex.WorkloadType, requestJob.WorkloadType);
			return result;
		}

		private void MakePropValue(PropValue[] result, MapiPropertiesIndex idx, object value)
		{
			result[(int)idx] = new PropValue(this.PropTags[(int)idx], value);
		}

		private static readonly NamedPropertyData[] npData;

		private static readonly object locker = new object();

		private static readonly EntryIdMap<RequestJobNamedPropertySet> mappings = new EntryIdMap<RequestJobNamedPropertySet>();

		private static readonly Guid MRSNamedPropGuid = new Guid("{338d7e39-3d57-479e-a8bc-bba17f0df824}");
	}
}
