using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Office.CompliancePolicy.ComplianceData;

namespace Microsoft.Exchange.Data.ApplicationLogic.Compliance
{
	internal class ExMailboxSearchComplianceItemPagedReader : ComplianceItemPagedReader
	{
		public ExMailboxSearchComplianceItemPagedReader(ExMailboxComplianceItemContainer mailboxContainer) : base(20, null)
		{
			this.mailboxContainer = mailboxContainer;
		}

		public override string PageCookie
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public override IEnumerable<ComplianceItem> GetNextPage()
		{
			if (this.reachedEnd)
			{
				return null;
			}
			List<ComplianceItem> items = new List<ComplianceItem>();
			AllItemsFolderHelper.RunQueryOnAllItemsFolder<bool>(this.mailboxContainer.Session, AllItemsFolderHelper.SupportedSortBy.ReceivedTime, delegate(QueryResult queryResults)
			{
				queryResults.SeekToOffset(SeekReference.OriginBeginning, this.currentPage++ * this.PageSize);
				object[][] rows = queryResults.GetRows(this.PageSize);
				if (rows.Length != this.PageSize)
				{
					this.reachedEnd = true;
				}
				for (int i = 0; i < rows.Length; i++)
				{
					items.Add(new ExMailComplianceItem(this.mailboxContainer.Session, rows[i]));
				}
				return true;
			}, ExMailComplianceItem.MailDataColumns);
			return items;
		}

		protected override string GenerateQuery()
		{
			return string.Empty;
		}

		protected override void Dispose(bool disposing)
		{
		}

		private const int DefaultPageSize = 20;

		private int currentPage;

		private bool reachedEnd;

		private ExMailboxComplianceItemContainer mailboxContainer;
	}
}
