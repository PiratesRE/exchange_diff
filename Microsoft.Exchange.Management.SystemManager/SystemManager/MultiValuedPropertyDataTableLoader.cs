using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class MultiValuedPropertyDataTableLoader : DataTableLoader
	{
		[DefaultValue(null)]
		public MultiValuedPropertyBase Mvp
		{
			get
			{
				return this.mvp;
			}
			set
			{
				this.mvp = value;
			}
		}

		public MultiValuedPropertyDataTableLoader(string columnTitle, MultiValuedPropertyBase mvp)
		{
			base.Table = new DataTable();
			base.Table.Columns.Add(columnTitle);
			this.Mvp = mvp;
		}

		public MultiValuedPropertyDataTableLoader()
		{
		}

		protected override void OnFillTable(RefreshRequestEventArgs e)
		{
			DataTable dataTable = (DataTable)e.Result;
			if (this.Mvp != null)
			{
				foreach (object obj in ((IEnumerable)this.Mvp))
				{
					dataTable.Rows.Add(new object[]
					{
						obj
					});
				}
			}
			base.OnFillTable(e);
		}

		private MultiValuedPropertyBase mvp;
	}
}
