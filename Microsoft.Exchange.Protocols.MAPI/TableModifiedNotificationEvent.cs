using System;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.LogicalDataModel;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public class TableModifiedNotificationEvent : LogicalModelNotificationEvent
	{
		public TableModifiedNotificationEvent(StoreDatabase database, int mailboxNumber, WindowsIdentity userIdentity, ClientType clientType, EventFlags eventFlags, TableEventType tableEventType, ExchangeId fid, ExchangeId mid, int inst, ExchangeId previousFid, ExchangeId previousMid, int previousInst, Properties row) : base(database, mailboxNumber, EventType.TableModified, userIdentity, clientType, eventFlags, null)
		{
			Statistics.NotificationTypes.TableModified.Bump();
			switch (tableEventType)
			{
			case TableEventType.Changed:
				Statistics.TableNotificationTypes.Changed.Bump();
				break;
			case TableEventType.Error:
				Statistics.TableNotificationTypes.Error.Bump();
				break;
			case TableEventType.RowAdded:
				Statistics.TableNotificationTypes.RowAdded.Bump();
				break;
			case TableEventType.RowDeleted:
				Statistics.TableNotificationTypes.RowDeleted.Bump();
				break;
			case TableEventType.RowModified:
				Statistics.TableNotificationTypes.RowModified.Bump();
				break;
			case TableEventType.SortDone:
				Statistics.TableNotificationTypes.SortDone.Bump();
				break;
			case TableEventType.RestrictDone:
				Statistics.TableNotificationTypes.RestrictDone.Bump();
				break;
			case TableEventType.SetcolDone:
				Statistics.TableNotificationTypes.SetcolDone.Bump();
				break;
			case TableEventType.Reload:
				Statistics.TableNotificationTypes.Reload.Bump();
				break;
			}
			this.tableEventType = tableEventType;
			this.fid = fid;
			this.mid = mid;
			this.inst = inst;
			this.previousFid = previousFid;
			this.previousMid = previousMid;
			this.previousInst = previousInst;
			this.row = row;
		}

		public TableEventType TableEventType
		{
			get
			{
				return this.tableEventType;
			}
		}

		public ExchangeId Fid
		{
			get
			{
				return this.fid;
			}
		}

		public ExchangeId Mid
		{
			get
			{
				return this.mid;
			}
		}

		public int Inst
		{
			get
			{
				return this.inst;
			}
		}

		public ExchangeId PreviousFid
		{
			get
			{
				return this.previousFid;
			}
		}

		public ExchangeId PreviousMid
		{
			get
			{
				return this.previousMid;
			}
		}

		public int PreviousInst
		{
			get
			{
				return this.previousInst;
			}
		}

		public Properties Row
		{
			get
			{
				return this.row;
			}
		}

		public override NotificationEvent.RedundancyStatus GetRedundancyStatus(NotificationEvent oldNev)
		{
			TableModifiedNotificationEvent tableModifiedNotificationEvent = oldNev as TableModifiedNotificationEvent;
			if (tableModifiedNotificationEvent != null)
			{
				if (this.TableEventType == TableEventType.Changed)
				{
					Statistics.MiscelaneousNotifications.NewTableChangedWashesAnyOld.Bump();
					return NotificationEvent.RedundancyStatus.FlagDropOld;
				}
				if (tableModifiedNotificationEvent.TableEventType == TableEventType.Changed)
				{
					Statistics.MiscelaneousNotifications.OldTableChangedWashesAnyNew.Bump();
					return NotificationEvent.RedundancyStatus.DropNewAndStop;
				}
				if (this.TableEventType == TableEventType.RowModified)
				{
					if (tableModifiedNotificationEvent.TableEventType == TableEventType.RowAdded)
					{
						if (this.Mid == tableModifiedNotificationEvent.Mid && this.Fid == tableModifiedNotificationEvent.Fid && this.Inst == tableModifiedNotificationEvent.Inst)
						{
							Statistics.MiscelaneousNotifications.NewRowModifiedWashesOldRowAdded.Bump();
							return NotificationEvent.RedundancyStatus.MergeReplaceOldAndStop;
						}
						if ((this.Mid == tableModifiedNotificationEvent.PreviousMid && this.Fid == tableModifiedNotificationEvent.PreviousFid && this.Inst == tableModifiedNotificationEvent.PreviousInst) || (this.PreviousMid == tableModifiedNotificationEvent.Mid && this.PreviousFid == tableModifiedNotificationEvent.Fid && this.PreviousInst == tableModifiedNotificationEvent.Inst))
						{
							return NotificationEvent.RedundancyStatus.FlagStopSearch;
						}
					}
					else if (tableModifiedNotificationEvent.TableEventType == TableEventType.RowModified)
					{
						if (this.Mid == tableModifiedNotificationEvent.Mid && this.Fid == tableModifiedNotificationEvent.Fid && this.Inst == tableModifiedNotificationEvent.Inst)
						{
							Statistics.MiscelaneousNotifications.NewRowModifiedWashesOldRowModified.Bump();
							return NotificationEvent.RedundancyStatus.ReplaceOldAndStop;
						}
						if ((this.Mid == tableModifiedNotificationEvent.PreviousMid && this.Fid == tableModifiedNotificationEvent.PreviousFid && this.Inst == tableModifiedNotificationEvent.PreviousInst) || (this.PreviousMid == tableModifiedNotificationEvent.Mid && this.PreviousFid == tableModifiedNotificationEvent.Fid && this.PreviousInst == tableModifiedNotificationEvent.Inst))
						{
							return NotificationEvent.RedundancyStatus.FlagStopSearch;
						}
					}
				}
				else
				{
					if (this.TableEventType != TableEventType.RowDeleted)
					{
						return NotificationEvent.RedundancyStatus.FlagStopSearch;
					}
					if (tableModifiedNotificationEvent.TableEventType == TableEventType.RowAdded)
					{
						if (this.Mid == tableModifiedNotificationEvent.Mid && this.Fid == tableModifiedNotificationEvent.Fid && this.Inst == tableModifiedNotificationEvent.Inst)
						{
							Statistics.MiscelaneousNotifications.NewRowDeletedWashesOldRowAdded.Bump();
							return NotificationEvent.RedundancyStatus.DropBothAndStop;
						}
						if (this.Mid == tableModifiedNotificationEvent.PreviousMid && this.Fid == tableModifiedNotificationEvent.PreviousFid && this.Inst == tableModifiedNotificationEvent.PreviousInst)
						{
							return NotificationEvent.RedundancyStatus.FlagStopSearch;
						}
					}
					else if (tableModifiedNotificationEvent.TableEventType == TableEventType.RowModified)
					{
						if (this.Mid == tableModifiedNotificationEvent.Mid && this.Fid == tableModifiedNotificationEvent.Fid && this.Inst == tableModifiedNotificationEvent.Inst)
						{
							Statistics.MiscelaneousNotifications.NewRowDeletedWashesOldRowModified.Bump();
							return NotificationEvent.RedundancyStatus.ReplaceOldAndStop;
						}
						if (this.Mid == tableModifiedNotificationEvent.PreviousMid && this.Fid == tableModifiedNotificationEvent.PreviousFid && this.Inst == tableModifiedNotificationEvent.PreviousInst)
						{
							return NotificationEvent.RedundancyStatus.FlagStopSearch;
						}
					}
				}
			}
			return NotificationEvent.RedundancyStatus.Continue;
		}

		public override NotificationEvent MergeWithOldEvent(NotificationEvent oldNev)
		{
			return new TableModifiedNotificationEvent(base.Database, base.MailboxNumber, base.UserIdentity, base.ClientType, base.EventFlags, TableEventType.RowAdded, this.Fid, this.Mid, this.Inst, this.PreviousFid, this.PreviousMid, this.PreviousInst, this.Row);
		}

		protected override void AppendClassName(StringBuilder sb)
		{
			sb.Append("TableModifiedNotificationEvent");
		}

		protected override void AppendFields(StringBuilder sb)
		{
			base.AppendFields(sb);
			sb.Append(" TableEventType:[");
			sb.Append(this.TableEventType);
			sb.Append("] Fid:[");
			sb.Append(this.Fid);
			sb.Append("] Mid:[");
			sb.Append(this.Mid);
			sb.Append("] Inst:[");
			sb.Append(this.Inst);
			sb.Append("] PreviousFid:[");
			sb.Append(this.PreviousFid);
			sb.Append("] PreviousMid:[");
			sb.Append(this.PreviousMid);
			sb.Append("] PreviousInst:[");
			sb.Append(this.PreviousInst);
			sb.Append("] Row:[");
			sb.Append(this.Row);
			sb.Append("]");
		}

		private TableEventType tableEventType;

		private ExchangeId fid;

		private ExchangeId mid;

		private int inst;

		private ExchangeId previousFid;

		private ExchangeId previousMid;

		private int previousInst;

		private Properties row;
	}
}
