using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class FxProxyPoolCreateFolderMessage : DataMessageBase
	{
		public FxProxyPoolCreateFolderMessage(FolderRec folderRec)
		{
			this.Data = folderRec;
		}

		public FxProxyPoolCreateFolderMessage(byte[] blob)
		{
			FolderRec folderRec = CommonUtils.DataContractDeserialize<FolderRec>(blob);
			if (folderRec != null)
			{
				this.Data = folderRec;
			}
		}

		public static DataMessageOpcode[] SupportedOpcodes
		{
			get
			{
				return new DataMessageOpcode[]
				{
					DataMessageOpcode.FxProxyPoolCreateFolder
				};
			}
		}

		public FolderRec Data { get; private set; }

		public static IDataMessage Deserialize(DataMessageOpcode opcode, byte[] data, bool useCompression)
		{
			return new FxProxyPoolCreateFolderMessage(data);
		}

		protected override int GetSizeInternal()
		{
			int num = 0;
			if (this.Data != null)
			{
				num = 13;
				if (this.Data.EntryId != null)
				{
					num += this.Data.EntryId.Length;
				}
				if (this.Data.ParentId != null)
				{
					num += this.Data.ParentId.Length;
				}
				if (this.Data.FolderName != null)
				{
					num += this.Data.FolderName.Length * 2;
				}
				if (this.Data.FolderClass != null)
				{
					num += this.Data.FolderClass.Length * 2;
				}
				if (this.Data.AdditionalProps != null)
				{
					foreach (PropValueData propValueData in this.Data.AdditionalProps)
					{
						num += propValueData.GetApproximateSize();
					}
				}
				if (this.Data.PromotedPropertiesList != null)
				{
					num += this.Data.PromotedPropertiesList.Length * 4;
				}
				if (this.Data.Views != null)
				{
					foreach (SortOrderData sortOrderData in this.Data.Views)
					{
						num += 6;
						if (sortOrderData.Members != null)
						{
							num += sortOrderData.Members.Length * 9;
						}
					}
				}
				if (this.Data.ICSViews != null)
				{
					foreach (ICSViewData icsviewData in this.Data.ICSViews)
					{
						num++;
						if (icsviewData.CoveringPropertyTags != null)
						{
							num += 4 * icsviewData.CoveringPropertyTags.Length;
						}
					}
				}
				if (this.Data.Restrictions != null)
				{
					foreach (RestrictionData restrictionData in this.Data.Restrictions)
					{
						num += restrictionData.GetApproximateSize();
					}
				}
			}
			return num;
		}

		protected override void SerializeInternal(bool useCompression, out DataMessageOpcode opcode, out byte[] data)
		{
			opcode = DataMessageOpcode.FxProxyPoolCreateFolder;
			data = CommonUtils.DataContractSerialize<FolderRec>(this.Data);
		}
	}
}
