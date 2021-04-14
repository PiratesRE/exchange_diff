using System;
using System.Data;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class ReportingFilterTable : IDisposable
	{
		public ReportingFilterTable()
		{
			this.dataTable = new DataTable("ReportingFilterTable");
			this.dataTable.Columns.Add(new DataColumn("ReportFilter", typeof(string)));
		}

		~ReportingFilterTable()
		{
			this.Dispose(false);
		}

		public DataTable DataTable
		{
			get
			{
				this.ThrowIfDisposed();
				return this.dataTable;
			}
		}

		public void AddRow(string value)
		{
			this.ThrowIfDisposed();
			DataRow dataRow = this.DataTable.NewRow();
			dataRow[0] = value;
			this.DataTable.Rows.Add(dataRow);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					this.dataTable.Dispose();
				}
				this.disposed = true;
			}
		}

		private void ThrowIfDisposed()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException(base.GetType().ToString());
			}
		}

		private bool disposed;

		private DataTable dataTable;
	}
}
