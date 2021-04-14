using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	internal abstract class MessageListBulkOperation : BulkOperation
	{
		public MessageListBulkOperation(MapiFolder folder, IList<ExchangeId> messageIds, int chunkSize) : base(chunkSize)
		{
			this.folder = folder;
			this.messageIds = messageIds;
			this.firstChunk = true;
		}

		public MapiFolder Folder
		{
			get
			{
				return this.folder;
			}
		}

		public IList<ExchangeId> MessageIds
		{
			get
			{
				return this.messageIds;
			}
		}

		public override bool DoChunk(MapiContext context, out bool progress, out bool incomplete, out ErrorCode error)
		{
			progress = false;
			incomplete = false;
			error = ErrorCode.NoError;
			int num = 0;
			bool flag = false;
			if (this.firstChunk)
			{
				if (!this.CheckSourceFolder(context))
				{
					error = ErrorCode.CreateObjectDeleted((LID)37400U);
					return true;
				}
				int num2;
				if (!this.ProcessStart(context, out num2, ref error))
				{
					flag = true;
				}
				else
				{
					if (num2 != 0)
					{
						num += num2;
						progress = true;
					}
					this.firstChunk = false;
				}
			}
			else if (!this.CheckSourceFolder(context))
			{
				incomplete = true;
				flag = true;
			}
			while (!flag && num < base.ChunkSize)
			{
				int num3 = Math.Min(base.ChunkSize - num, (base.ChunkSize + 1) / 2);
				IList<ExchangeId> list = null;
				if (this.messageIds == null || this.messageIds.Count == 0)
				{
					if (this.messageView == null)
					{
						this.messageView = new MapiViewMessage();
						this.messageView.Configure(context, this.folder.Logon, this.folder, ViewMessageConfigureFlags.NoNotifications | ViewMessageConfigureFlags.DoNotUseLazyIndex);
						this.messageView.SetColumns(context, BulkOperation.ColumnsToFetchMid, MapiViewSetColumnsFlag.NoColumnValidation);
						Restriction filterRestriction = this.GetFilterRestriction(context);
						if (filterRestriction != null)
						{
							this.messageView.Restrict(context, 0, filterRestriction);
						}
					}
					IList<Properties> list2 = this.messageView.QueryRowsBatch(context, num3, QueryRowsFlags.None);
					if (list2 != null && list2.Count != 0)
					{
						if (this.tempMidsList == null)
						{
							this.tempMidsList = new List<ExchangeId>(list2.Count);
						}
						else
						{
							this.tempMidsList.Clear();
						}
						for (int i = 0; i < list2.Count; i++)
						{
							this.tempMidsList.Add(ExchangeId.CreateFrom26ByteArray(context, this.folder.Logon.StoreMailbox.ReplidGuidMap, (byte[])list2[i][0].Value));
						}
						list = this.tempMidsList;
					}
					else
					{
						flag = true;
					}
				}
				else if (this.currentIndex == 0 && this.messageIds.Count < num3)
				{
					list = this.messageIds;
					this.currentIndex += list.Count;
					flag = true;
				}
				else if (this.currentIndex == this.messageIds.Count)
				{
					flag = true;
				}
				else
				{
					num3 = Math.Min(this.messageIds.Count - this.currentIndex, num3);
					if (this.tempMidsList == null)
					{
						this.tempMidsList = new List<ExchangeId>(num3);
					}
					else
					{
						this.tempMidsList.Clear();
					}
					for (int j = 0; j < num3; j++)
					{
						this.tempMidsList.Add(this.messageIds[this.currentIndex++]);
					}
					list = this.tempMidsList;
					if (this.currentIndex == this.messageIds.Count)
					{
						flag = true;
					}
				}
				if (list != null && list.Count != 0)
				{
					int num2;
					if (!this.ProcessMessages(context, this.folder, list, out num2, ref incomplete, ref error))
					{
						flag = true;
					}
					else if (num2 != 0)
					{
						num += num2;
						progress = true;
					}
				}
			}
			if (flag)
			{
				this.ProcessEnd(context, incomplete, error);
			}
			return flag;
		}

		protected virtual bool CheckSourceFolder(MapiContext context)
		{
			return this.folder.CheckAlive(context);
		}

		protected virtual bool ProcessStart(MapiContext context, out int progressCount, ref ErrorCode error)
		{
			progressCount = 0;
			return true;
		}

		protected virtual void ProcessEnd(MapiContext context, bool incomplete, ErrorCode error)
		{
		}

		protected abstract bool ProcessMessages(MapiContext context, MapiFolder folder, IList<ExchangeId> midsToProcess, out int progressCount, ref bool incomplete, ref ErrorCode error);

		protected virtual Restriction GetFilterRestriction(MapiContext context)
		{
			return null;
		}

		protected override void InternalDispose(bool calledFromDispose)
		{
			if (calledFromDispose && this.messageView != null)
			{
				this.messageView.Dispose();
				this.messageView = null;
			}
		}

		private readonly MapiFolder folder;

		private readonly IList<ExchangeId> messageIds;

		private bool firstChunk;

		private int currentIndex;

		private List<ExchangeId> tempMidsList;

		private MapiViewMessage messageView;
	}
}
