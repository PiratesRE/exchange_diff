using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Threading;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Directory
{
	internal class ADObjectCache<TResult, TLoaderException> : DisposeTrackableBase where TResult : ADObject, new() where TLoaderException : Exception
	{
		public ADObjectCache(Func<TResult[], TResult[]> loadOperator, string refreshIntervalRegistryKey = null)
		{
			this.RefreshInterval = ADObjectCache<TResult, TLoaderException>.DefaultRefreshInterval;
			this.refreshLock = new object();
			this.IsInitialized = false;
			this.LoadOperator = loadOperator;
			this.RefreshIntervalRegistryKey = refreshIntervalRegistryKey;
		}

		public bool IsInitialized { get; protected set; }

		public bool IsAutoRefreshed
		{
			get
			{
				return this.RefreshInterval != TimeSpan.Zero;
			}
		}

		public TResult[] Value
		{
			get
			{
				return Interlocked.CompareExchange<TResult[]>(ref this.internalValue, null, null);
			}
		}

		public DateTime LastModified { get; private set; }

		public void SetRefreshInterval(TimeSpan refreshInterval)
		{
			this.RefreshInterval = refreshInterval;
			this.RefreshTimer.Change(this.RefreshInterval, this.RefreshInterval);
		}

		public TimeSpan RefreshInterval { get; private set; }

		private protected string RefreshIntervalRegistryKey { protected get; private set; }

		private Func<TResult[], TResult[]> LoadOperator { get; set; }

		private GuardedTimer RefreshTimer { get; set; }

		public void Initialize(bool refreshNow = true)
		{
			this.Initialize(ADObjectCache<TResult, TLoaderException>.DefaultRefreshInterval, refreshNow);
		}

		public void Initialize(TimeSpan refreshInterval, bool refreshNow = true)
		{
			if (this.IsInitialized)
			{
				return;
			}
			if (!string.IsNullOrEmpty(this.RefreshIntervalRegistryKey))
			{
				int value = RegistryReader.Instance.GetValue<int>(Registry.LocalMachine, this.RefreshIntervalRegistryKey, "RefreshInterval", (int)ADObjectCache<TResult, TLoaderException>.DefaultRefreshInterval.TotalSeconds);
				refreshInterval = new TimeSpan(0, 0, value);
			}
			if (refreshNow)
			{
				this.Refresh(null);
			}
			this.RefreshTimer = new GuardedTimer(new TimerCallback(this.Refresh));
			this.SetRefreshInterval(refreshInterval);
			this.IsInitialized = true;
		}

		public void Refresh(object unusedState)
		{
			try
			{
				if (Monitor.TryEnter(this.refreshLock))
				{
					TResult[] value;
					try
					{
						value = this.LoadOperator(this.Value);
						this.LastModified = DateTime.UtcNow;
					}
					catch (TLoaderException)
					{
						value = this.Value;
					}
					Interlocked.Exchange<TResult[]>(ref this.internalValue, value);
				}
			}
			finally
			{
				if (Monitor.IsEntered(this.refreshLock))
				{
					Monitor.Exit(this.refreshLock);
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ADObjectCache<TResult, TLoaderException>>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.RefreshTimer != null)
				{
					this.RefreshTimer.Dispose(false);
				}
				this.RefreshTimer = null;
				this.IsInitialized = false;
			}
		}

		public const string RefreshIntervalName = "RefreshInterval";

		private static readonly TimeSpan DefaultRefreshInterval = TimeSpan.FromMinutes(15.0);

		private object refreshLock;

		private TResult[] internalValue;
	}
}
