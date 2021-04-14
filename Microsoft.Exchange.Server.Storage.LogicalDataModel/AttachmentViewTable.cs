using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;
using Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseUpgraders;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public sealed class AttachmentViewTable : ViewTable
	{
		public AttachmentViewTable(Context context, Mailbox mailbox, Message parentMessage) : base(mailbox, DatabaseSchema.AttachmentTable(mailbox.Database).Table)
		{
			this.parentMessage = parentMessage;
			AttachmentTable attachmentTable = DatabaseSchema.AttachmentTable(mailbox.Database);
			AttachmentTableFunctionTableFunction attachmentTableFunctionTableFunction = DatabaseSchema.AttachmentTableFunctionTableFunction(mailbox.Database);
			this.UpdatePseudoIndexIfNeeded(context);
			SearchCriteria implicitCriteria = Factory.CreateSearchCriteriaCompare(attachmentTable.MailboxPartitionNumber, SearchCriteriaCompare.SearchRelOp.Equal, Factory.CreateConstantColumn(mailbox.MailboxPartitionNumber, attachmentTable.MailboxPartitionNumber));
			base.SetImplicitCriteria(implicitCriteria);
			this.SortTable(attachmentTableFunctionTableFunction.TableFunction.PrimaryKeyIndex.SortOrder);
		}

		public AttachmentViewTable(Context context, Mailbox mailbox, Message parentMessage, List<Column> columns, SortOrder sortOrder, SearchCriteria criteria) : this(context, mailbox, parentMessage)
		{
			base.SetColumns(context, columns);
			this.SortTable(sortOrder);
			this.Restrict(context, criteria);
		}

		protected override Index LogicalKeyIndex
		{
			get
			{
				return this.inScopePseudoIndexes[0].IndexTable.PrimaryKeyIndex;
			}
		}

		protected override bool MustUseLazyIndex
		{
			get
			{
				return true;
			}
		}

		protected internal override IList<IIndex> GetInScopePseudoIndexes(Context context, SearchCriteria findRowCriteria, out IList<IIndex> masterIndexes)
		{
			masterIndexes = null;
			this.UpdatePseudoIndexIfNeeded(context);
			return this.inScopePseudoIndexes;
		}

		protected override void BringIndexesToCurrent(Context context, IList<IIndex> indexList, DataAccessOperator queryPlan)
		{
		}

		private void UpdatePseudoIndexIfNeeded(Context context)
		{
			if (this.parentMessage.IsDead)
			{
				return;
			}
			int subobjectsChangeCookie = this.parentMessage.SubobjectsChangeCookie;
			if (this.currentSubobjectsChangeCookie == subobjectsChangeCookie)
			{
				return;
			}
			AttachmentTable attachmentTable = DatabaseSchema.AttachmentTable(context.Database);
			AttachmentTableFunctionTableFunction attachmentTableFunctionTableFunction = DatabaseSchema.AttachmentTableFunctionTableFunction(context.Database);
			byte[] attachmentsBlob = this.parentMessage.GetAttachmentsBlob();
			Dictionary<Column, Column> dictionary = new Dictionary<Column, Column>(3);
			dictionary.Add(attachmentTable.MailboxPartitionNumber, Factory.CreateConstantColumn(this.parentMessage.Mailbox.MailboxPartitionNumber, attachmentTable.MailboxPartitionNumber));
			dictionary.Add(attachmentTable.Inid, attachmentTableFunctionTableFunction.Inid);
			if (UnifiedMailbox.IsReady(context, context.Database))
			{
				dictionary.Add(attachmentTable.MailboxNumber, Factory.CreateConstantColumn(this.parentMessage.Mailbox.MailboxNumber, attachmentTable.MailboxNumber));
			}
			SimplePseudoIndex item = new SimplePseudoIndex(attachmentTable.Table, attachmentTableFunctionTableFunction.TableFunction, new object[]
			{
				attachmentsBlob
			}, attachmentTableFunctionTableFunction.TableFunction.PrimaryKeyIndex.SortOrder, dictionary, null, true);
			this.inScopePseudoIndexes.Clear();
			this.inScopePseudoIndexes.Add(item);
			this.currentSubobjectsChangeCookie = subobjectsChangeCookie;
		}

		private readonly List<IIndex> inScopePseudoIndexes = new List<IIndex>(1);

		private readonly Message parentMessage;

		private int currentSubobjectsChangeCookie = -1;
	}
}
