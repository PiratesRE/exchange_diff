using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Office.CompliancePolicy.ComplianceData;

namespace Microsoft.Exchange.Data.ApplicationLogic.Compliance
{
	internal class ExFolderSearchComplianceItemPagedReader : ComplianceItemPagedReader
	{
		public ExFolderSearchComplianceItemPagedReader(ExFolderComplianceItemContainer folderContainer) : base(20, null)
		{
			this.folderContainer = folderContainer;
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
			List<ComplianceItem> list = new List<ComplianceItem>();
			QueryResult queryResult = this.folderContainer.Folder.ItemQuery(ItemQueryType.None, null, null, ExMailComplianceItem.MailDataColumns);
			queryResult.SeekToOffset(SeekReference.OriginBeginning, this.currentPage++ * base.PageSize);
			object[][] rows = queryResult.GetRows(base.PageSize);
			if (rows.Length != base.PageSize)
			{
				this.reachedEnd = true;
			}
			for (int i = 0; i < rows.Length; i++)
			{
				list.Add(new ExMailComplianceItem(this.folderContainer.Session, rows[i]));
			}
			return list;
		}

		protected override void Dispose(bool isDisposing)
		{
			throw new NotImplementedException();
		}

		protected override string GenerateQuery()
		{
			return string.Empty;
		}

		private const int DefaultPageSize = 20;

		private int currentPage;

		private bool reachedEnd;

		private ExFolderComplianceItemContainer folderContainer;
	}
}
