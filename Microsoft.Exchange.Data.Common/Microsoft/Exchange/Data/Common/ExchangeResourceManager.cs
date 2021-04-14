using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace Microsoft.Exchange.Data.Common
{
	public class ExchangeResourceManager : ResourceManager
	{
		public static ExchangeResourceManager GetResourceManager(string baseName, Assembly assembly)
		{
			if (null == assembly)
			{
				throw new ArgumentNullException("assembly");
			}
			string key = baseName + assembly.GetName().Name;
			ExchangeResourceManager result;
			lock (ExchangeResourceManager.resourceManagers)
			{
				ExchangeResourceManager exchangeResourceManager = null;
				if (!ExchangeResourceManager.resourceManagers.TryGetValue(key, out exchangeResourceManager))
				{
					exchangeResourceManager = new ExchangeResourceManager(baseName, assembly);
					ExchangeResourceManager.resourceManagers.Add(key, exchangeResourceManager);
				}
				result = exchangeResourceManager;
			}
			return result;
		}

		private ExchangeResourceManager(string baseName, Assembly assembly) : base(baseName, assembly)
		{
			this.resourceReleaseStopwatch.Start();
		}

		public override string BaseName
		{
			get
			{
				return base.BaseName;
			}
		}

		public string AssemblyName
		{
			get
			{
				return this.MainAssembly.GetName().FullName;
			}
		}

		public override string GetString(string name)
		{
			return this.GetString(name, CultureInfo.CurrentUICulture);
		}

		public override string GetString(string name, CultureInfo culture)
		{
			CultureInfo cultureInfo = culture ?? CultureInfo.CurrentUICulture;
			SipCultureInfoBase sipCultureInfoBase = cultureInfo as SipCultureInfoBase;
			if (sipCultureInfoBase != null)
			{
				bool useSipName = sipCultureInfoBase.UseSipName;
				try
				{
					sipCultureInfoBase.UseSipName = true;
					return this.GetStringInternal(name, sipCultureInfoBase);
				}
				finally
				{
					sipCultureInfoBase.UseSipName = useSipName;
				}
			}
			return this.GetStringInternal(name, cultureInfo);
		}

		protected virtual string GetStringInternal(string name, CultureInfo culture)
		{
			string text = null;
			try
			{
				this.readerWriterLock.EnterReadLock();
				text = base.GetString(name, culture);
			}
			finally
			{
				this.readerWriterLock.ExitReadLock();
			}
			if (text == null)
			{
				try
				{
					this.readerWriterLock.EnterWriteLock();
					if (this.resourceReleaseStopwatch.Elapsed > this.resourceReleaseInterval)
					{
						base.ReleaseAllResources();
						this.resourceReleaseStopwatch.Restart();
					}
					text = base.GetString(name, culture);
				}
				finally
				{
					this.readerWriterLock.ExitWriteLock();
				}
			}
			return text;
		}

		private static Dictionary<string, ExchangeResourceManager> resourceManagers = new Dictionary<string, ExchangeResourceManager>();

		private readonly TimeSpan resourceReleaseInterval = TimeSpan.FromMinutes(1.0);

		private Stopwatch resourceReleaseStopwatch = new Stopwatch();

		private ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();

		public class Concurrent : ExchangeResourceManager
		{
			public Concurrent(ExchangeResourceManager resourceManager) : base(resourceManager.BaseName, resourceManager.MainAssembly)
			{
				this.resourceManager = resourceManager;
			}

			protected override string GetStringInternal(string name, CultureInfo culture)
			{
				Tuple<string, string> key = new Tuple<string, string>(name, culture.Name);
				string stringInternal;
				if (this.cache.TryGetValue(key, out stringInternal))
				{
					return stringInternal;
				}
				stringInternal = this.resourceManager.GetStringInternal(name, culture);
				if (stringInternal != null)
				{
					this.cache[key] = stringInternal;
				}
				return stringInternal;
			}

			private readonly ExchangeResourceManager resourceManager;

			private readonly ConcurrentDictionary<Tuple<string, string>, string> cache = new ConcurrentDictionary<Tuple<string, string>, string>();
		}
	}
}
