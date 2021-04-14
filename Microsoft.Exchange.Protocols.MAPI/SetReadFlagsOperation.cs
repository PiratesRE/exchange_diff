using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	internal class SetReadFlagsOperation : MessageListBulkOperation
	{
		public SetReadFlagsOperation(MapiFolder folder, IList<ExchangeId> messageIds, SetReadFlagFlags flags, IList<ExchangeId> readCns, int chunkSize) : base(folder, messageIds, chunkSize)
		{
			this.flags = flags;
			this.readCns = readCns;
		}

		public SetReadFlagsOperation(MapiFolder folder, IList<ExchangeId> messageIds, SetReadFlagFlags flags, int chunkSize) : this(folder, messageIds, flags, null, chunkSize)
		{
		}

		public SetReadFlagsOperation(MapiFolder folder, IList<ExchangeId> messageIds, SetReadFlagFlags flags, IList<ExchangeId> readCns) : this(folder, messageIds, flags, readCns, 500)
		{
		}

		public SetReadFlagsOperation(MapiFolder folder, IList<ExchangeId> messageIds, SetReadFlagFlags flags) : this(folder, messageIds, flags, 500)
		{
		}

		public int MessagesProcessedForTest
		{
			get
			{
				return this.messagesProcessed;
			}
		}

		protected override bool ProcessStart(MapiContext context, out int progressCount, ref ErrorCode error)
		{
			progressCount = 0;
			return true;
		}

		protected override bool ProcessMessages(MapiContext context, MapiFolder folder, IList<ExchangeId> midsToProcess, out int progressCount, ref bool incomplete, ref ErrorCode error)
		{
			this.messagesProcessed += midsToProcess.Count;
			return BulkOperation.SetReadFlags(context, folder, this.flags, midsToProcess, BulkErrorAction.Incomplete, BulkErrorAction.Incomplete, this.readCns, out progressCount, ref incomplete, ref error);
		}

		protected override Restriction GetFilterRestriction(MapiContext context)
		{
			Restriction restriction = null;
			if ((byte)(this.flags & SetReadFlagFlags.GenerateReceiptOnly) == 0)
			{
				if ((byte)(this.flags & (SetReadFlagFlags.ClearReadNotificationPending | SetReadFlagFlags.ClearNonReadNotificationPending)) == 0)
				{
					if ((byte)(this.flags & SetReadFlagFlags.ClearReadFlag) != 0)
					{
						restriction = new RestrictionProperty(PropTag.Message.Read, RelationOperator.Equal, true);
					}
					else
					{
						restriction = new RestrictionProperty(PropTag.Message.Read, RelationOperator.Equal, false);
						if ((byte)(this.flags & SetReadFlagFlags.SuppressReceipt) == 0)
						{
							restriction = new RestrictionOR(new Restriction[]
							{
								restriction,
								new RestrictionAND(new Restriction[]
								{
									new RestrictionBitmask(PropTag.Message.MailFlags, 6L, BitmaskOperation.NotEqualToZero),
									new RestrictionBitmask(PropTag.Message.MailFlags, 32L, BitmaskOperation.EqualToZero)
								})
							});
						}
					}
					if ((byte)(this.flags & SetReadFlagFlags.SuppressReceipt) != 0 && (context.ClientType == ClientType.MoMT || context.ClientType == ClientType.User || context.ClientType == ClientType.RpcHttp))
					{
						restriction = new RestrictionOR(new Restriction[]
						{
							restriction,
							new RestrictionAND(new Restriction[]
							{
								new RestrictionBitmask(PropTag.Message.MailFlags, 6L, BitmaskOperation.NotEqualToZero),
								new RestrictionBitmask(PropTag.Message.MailFlags, 32L, BitmaskOperation.EqualToZero)
							})
						});
					}
				}
				else
				{
					short num = 0;
					if ((byte)(this.flags & SetReadFlagFlags.ClearReadNotificationPending) != 0)
					{
						num |= 2;
					}
					if ((byte)(this.flags & SetReadFlagFlags.ClearNonReadNotificationPending) != 0)
					{
						num |= 4;
					}
					restriction = new RestrictionBitmask(PropTag.Message.MailFlags, (long)num, BitmaskOperation.NotEqualToZero);
				}
			}
			return restriction;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<SetReadFlagsOperation>(this);
		}

		private const int ReasonableSetReadChunkSize = 500;

		private SetReadFlagFlags flags;

		private IList<ExchangeId> readCns;

		private int messagesProcessed;
	}
}
