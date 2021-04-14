using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager
{
	[Serializable]
	public class AggregateDataTableLoadersRefreshableSource : Component, IRefreshableNotification, IRefreshable, ISupportFastRefresh
	{
		public AggregateDataTableLoadersRefreshableSource(params DataTableLoader[] dataTableLoaders)
		{
			this.DataTableLoaders.AddRange(dataTableLoaders);
		}

		public List<DataTableLoader> DataTableLoaders
		{
			get
			{
				return this.dataTableLoaders;
			}
		}

		public bool Refreshing
		{
			get
			{
				return this.refreshing;
			}
			private set
			{
				if (this.Refreshing != value)
				{
					if (value)
					{
						this.refreshed = true;
					}
					this.refreshing = value;
					this.OnRefreshingChanged(EventArgs.Empty);
				}
			}
		}

		public bool Refreshed
		{
			get
			{
				if (!this.refreshed)
				{
					bool flag = true;
					foreach (DataTableLoader dataTableLoader in this.DataTableLoaders)
					{
						flag = (flag && dataTableLoader.Refreshed);
						if (!flag)
						{
							break;
						}
					}
					this.refreshed = flag;
				}
				return this.refreshed;
			}
		}

		public event EventHandler RefreshingChanged;

		protected virtual void OnRefreshingChanged(EventArgs e)
		{
			EventHandler refreshingChanged = this.RefreshingChanged;
			if (refreshingChanged != null)
			{
				refreshingChanged(this, e);
			}
		}

		void ISupportFastRefresh.Refresh(IProgress progress, object identity)
		{
			this.RefreshCore(progress, identity);
		}

		void ISupportFastRefresh.Refresh(IProgress progress, object[] identities, RefreshRequestPriority priority)
		{
			if (identities.Length == 1)
			{
				this.RefreshCore(progress, identities[0]);
				return;
			}
			throw new NotImplementedException();
		}

		public void Refresh(IProgress progress)
		{
			this.RefreshCore(progress, null);
		}

		private void RefreshCore(IProgress progress, object identity)
		{
			MultiRefreshableSource multiRefreshableSource = this.CreateRefresher(identity);
			this.refreshers.Add(multiRefreshableSource);
			this.Refreshing = true;
			multiRefreshableSource.RefreshingChanged += this.Refresher_RefreshingChanged;
			multiRefreshableSource.Refresh(progress);
		}

		private void Refresher_RefreshingChanged(object sender, EventArgs e)
		{
			MultiRefreshableSource multiRefreshableSource = sender as MultiRefreshableSource;
			if (!multiRefreshableSource.Refreshing)
			{
				multiRefreshableSource.RefreshingChanged -= this.Refresher_RefreshingChanged;
				this.refreshers.Remove(multiRefreshableSource);
				if (this.refreshers.Count == 0)
				{
					this.Refreshing = false;
				}
			}
		}

		private MultiRefreshableSource CreateRefresher(object identity)
		{
			MultiRefreshableSource multiRefreshableSource = new MultiRefreshableSource();
			if (identity == null)
			{
				multiRefreshableSource.RefreshableSources.AddRange(this.DataTableLoaders.ToArray());
			}
			else
			{
				foreach (DataTableLoader dataTableLoader in this.DataTableLoaders)
				{
					multiRefreshableSource.RefreshableSources.Add(new SingleRowRefreshObject(identity, dataTableLoader));
				}
			}
			return multiRefreshableSource;
		}

		void ISupportFastRefresh.Remove(object identity)
		{
			foreach (DataTableLoader dataTableLoader in this.DataTableLoaders)
			{
				dataTableLoader.Remove(identity);
			}
		}

		public override ISite Site
		{
			get
			{
				return base.Site;
			}
			set
			{
				if (base.Site != value)
				{
					base.Site = value;
					if (this.dataTableLoaders != null)
					{
						foreach (DataTableLoader dataTableLoader in this.dataTableLoaders)
						{
							dataTableLoader.Site = value;
						}
					}
				}
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.dataTableLoaders != null)
			{
				this.dataTableLoaders.Clear();
			}
			base.Dispose(disposing);
		}

		private List<MultiRefreshableSource> refreshers = new List<MultiRefreshableSource>();

		private List<DataTableLoader> dataTableLoaders = new List<DataTableLoader>();

		private bool refreshing;

		private bool refreshed;
	}
}
