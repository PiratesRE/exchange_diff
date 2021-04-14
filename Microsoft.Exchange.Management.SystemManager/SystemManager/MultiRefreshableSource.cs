using System;
using System.Collections.Generic;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager
{
	[Serializable]
	public class MultiRefreshableSource : IRefreshableNotification, IRefreshable
	{
		public MultiRefreshableSource()
		{
		}

		public MultiRefreshableSource(params IRefreshable[] refreshableSources)
		{
			if (refreshableSources == null)
			{
				throw new ArgumentNullException("refreshableSources");
			}
			for (int i = 0; i < refreshableSources.Length; i++)
			{
				if (refreshableSources[i] == null)
				{
					throw new ArgumentNullException();
				}
			}
			this.refreshableSources.AddRange(refreshableSources);
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
						this.Refreshed = true;
					}
					this.refreshing = value;
					this.OnRefreshingChanged(this, EventArgs.Empty);
				}
			}
		}

		public bool Refreshed
		{
			get
			{
				return this.refreshed;
			}
			private set
			{
				this.refreshed = value;
			}
		}

		public event EventHandler RefreshingChanged;

		public List<IRefreshable> RefreshableSources
		{
			get
			{
				return this.refreshableSources;
			}
		}

		public void Refresh(IProgress progress)
		{
			if (this.refreshableSources.Count > 0)
			{
				if (this.lastRefresher != null)
				{
					this.lastRefresher.RefreshingChanged -= this.lastRefresher_RefreshingChanged;
					if (this.lastRefresher.IsRefreshing)
					{
						this.lastRefresher.Cancel();
					}
				}
				this.lastRefresher = new MultiRefreshableSource.AggregateRefresh(this.refreshableSources, progress);
				this.lastRefresher.RefreshingChanged += this.lastRefresher_RefreshingChanged;
				this.lastRefresher.StartRefresh();
				return;
			}
			progress.ReportProgress(100, 100, string.Empty);
		}

		private void lastRefresher_RefreshingChanged(object sender, EventArgs e)
		{
			this.Refreshing = ((MultiRefreshableSource.AggregateRefresh)sender).IsRefreshing;
		}

		private void OnRefreshingChanged(object sender, EventArgs e)
		{
			if (this.RefreshingChanged != null)
			{
				this.RefreshingChanged(this, e);
			}
		}

		private List<IRefreshable> refreshableSources = new List<IRefreshable>();

		private MultiRefreshableSource.AggregateRefresh lastRefresher;

		private bool refreshing;

		private bool refreshed;

		[Serializable]
		private class AggregateRefresh : IProgress
		{
			public AggregateRefresh(List<IRefreshable> dataSources, IProgress progress)
			{
				this.refreshableSources.AddRange(dataSources);
				this.progress = progress;
			}

			public event EventHandler RefreshingChanged;

			public void StartRefresh()
			{
				this.IsRefreshing = true;
				IRefreshable refreshable = this.refreshableSources[this.currentDataSourceIndex];
				if (refreshable is IRefreshableNotification)
				{
					(refreshable as IRefreshableNotification).RefreshingChanged += this.RefreshableNotification_RefreshingChanged;
					refreshable.Refresh(this);
					return;
				}
				refreshable.Refresh(NullProgress.Value);
				this.ReportProgress(100, 100, "");
				this.RefreshNextDataSource();
			}

			private void OnRefreshingChanged()
			{
				if (this.RefreshingChanged != null)
				{
					this.RefreshingChanged(this, EventArgs.Empty);
				}
			}

			private void RefreshNextDataSource()
			{
				if (this.IsRefreshing)
				{
					this.currentDataSourceIndex++;
					if (this.currentDataSourceIndex < this.refreshableSources.Count)
					{
						this.StartRefresh();
						return;
					}
					this.IsRefreshing = false;
				}
			}

			public void Cancel()
			{
				if (this.IsRefreshing && this.currentDataSourceIndex < this.refreshableSources.Count)
				{
					IRefreshable refreshable = this.refreshableSources[this.currentDataSourceIndex];
					if (refreshable is RefreshableComponent)
					{
						(refreshable as RefreshableComponent).CancelRefresh();
					}
					if (refreshable is IRefreshableNotification)
					{
						(refreshable as IRefreshableNotification).RefreshingChanged -= this.RefreshableNotification_RefreshingChanged;
					}
					this.cancelled = true;
					this.IsRefreshing = false;
				}
			}

			private void RefreshableNotification_RefreshingChanged(object sender, EventArgs e)
			{
				IRefreshableNotification refreshableNotification = sender as IRefreshableNotification;
				if (!refreshableNotification.Refreshing)
				{
					refreshableNotification.RefreshingChanged -= this.RefreshableNotification_RefreshingChanged;
					this.RefreshNextDataSource();
				}
			}

			public bool IsRefreshing
			{
				get
				{
					return this.isRefreshing;
				}
				private set
				{
					if (this.IsRefreshing != value)
					{
						this.isRefreshing = value;
						this.OnRefreshingChanged();
					}
				}
			}

			public bool Canceled
			{
				get
				{
					return this.cancelled;
				}
			}

			public void ReportProgress(int workProcessed, int totalWork, string statusText)
			{
				if (this.IsRefreshing && this.progress != null)
				{
					if (this.currentDataSourceIndex == this.refreshableSources.Count - 1 && workProcessed == totalWork)
					{
						this.progress.ReportProgress(100, 100, statusText);
						return;
					}
					if (totalWork > 0 && this.refreshableSources.Count > 0)
					{
						int num = this.currentDataSourceIndex * 100 / this.refreshableSources.Count + workProcessed * 100 / totalWork / this.refreshableSources.Count;
						this.progress.ReportProgress(num, 100, statusText);
					}
				}
			}

			private bool cancelled;

			private bool isRefreshing;

			private IProgress progress;

			private List<IRefreshable> refreshableSources = new List<IRefreshable>();

			private int currentDataSourceIndex;
		}
	}
}
