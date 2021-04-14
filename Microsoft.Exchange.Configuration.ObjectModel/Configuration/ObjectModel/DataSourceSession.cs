using System;
using Microsoft.Exchange.Diagnostics.Components.ObjectModel;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	internal abstract class DataSourceSession : IDisposable
	{
		public DataSourceSession(DataSourceInfo dataSourceInfo)
		{
			ExTraceGlobals.DataSourceSessionTracer.Information((long)this.GetHashCode(), "DataSourceSession::DataSourceSession - initializing data source session with data source info type {0}.", new object[]
			{
				(dataSourceInfo == null) ? "null" : dataSourceInfo.GetType()
			});
			this.dataSourceInfo = dataSourceInfo;
		}

		~DataSourceSession()
		{
			ExTraceGlobals.DataSourceSessionTracer.Information((long)this.GetHashCode(), "DataSourceSession::~DataSourceSession - disposing of data source session.");
			this.Dispose(false);
		}

		public DataSourceInfo DataSourceInfo
		{
			get
			{
				return this.dataSourceInfo;
			}
		}

		public string ConnectionString
		{
			get
			{
				return this.DataSourceInfo.ConnectionString;
			}
		}

		public virtual void Dispose()
		{
			ExTraceGlobals.DataSourceSessionTracer.Information((long)this.GetHashCode(), "DataSourceSession::Dispose - disposing of data source session.");
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			ExTraceGlobals.DataSourceSessionTracer.Information((long)this.GetHashCode(), "DataSourceSession::Dispose - disposing of data source session.");
			if (!this.isDisposed)
			{
				this.isDisposed = true;
			}
		}

		public virtual bool IsDisposed
		{
			get
			{
				return this.isDisposed;
			}
		}

		private bool isDisposed;

		private DataSourceInfo dataSourceInfo;
	}
}
