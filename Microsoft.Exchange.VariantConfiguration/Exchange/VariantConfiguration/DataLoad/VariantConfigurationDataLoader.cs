using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Search.Platform.Parallax.DataLoad;

namespace Microsoft.Exchange.VariantConfiguration.DataLoad
{
	internal class VariantConfigurationDataLoader : DataLoader
	{
		public VariantConfigurationDataLoader(IDataSourceReader dataSourceReader, IDataTransformation transformation, IEnumerable<string> preloadDataSources) : base(transformation)
		{
			this.preloadDataSources = preloadDataSources;
			this.dataSourcesLock = new object();
			this.loadedDataSources = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			this.dataSourceReader = dataSourceReader;
		}

		public void ForceLoad(IEnumerable<string> dataSources)
		{
			lock (this.dataSourcesLock)
			{
				this.Load(dataSources);
			}
		}

		public void ReloadIfLoaded()
		{
			this.ReloadIfLoaded(this.loadedDataSources);
		}

		public void ReloadIfLoaded(IEnumerable<string> dataSources)
		{
			if (this.AreAnyLoaded(dataSources))
			{
				lock (this.dataSourcesLock)
				{
					IEnumerable<string> enumerable = dataSources.Intersect(this.loadedDataSources, StringComparer.OrdinalIgnoreCase);
					if (enumerable.Count<string>() > 0)
					{
						this.Load(enumerable);
					}
				}
			}
		}

		public void LoadIfNotLoaded(IEnumerable<string> dataSources)
		{
			if (!this.AreAllLoaded(dataSources))
			{
				lock (this.dataSourcesLock)
				{
					IEnumerable<string> enumerable = dataSources.Except(this.loadedDataSources, StringComparer.OrdinalIgnoreCase);
					if (enumerable.Count<string>() > 0)
					{
						this.Load(enumerable);
					}
				}
			}
		}

		protected override void OnInitialized()
		{
			base.OnInitialized();
			if (this.preloadDataSources != null && this.preloadDataSources.Count<string>() > 0)
			{
				this.ForceLoad(this.preloadDataSources);
			}
		}

		private void Load(IEnumerable<string> dataSources)
		{
			if (base.ExecuteTransaction(delegate(TransactionContext context)
			{
				foreach (string text in dataSources)
				{
					Func<TextReader> contentReader = this.dataSourceReader.GetContentReader(text);
					context.LoadDataSource(text, contentReader);
				}
				return 0;
			}, dataSources))
			{
				HashSet<string> hashSet = new HashSet<string>(this.loadedDataSources, StringComparer.OrdinalIgnoreCase);
				foreach (string item in dataSources)
				{
					hashSet.Add(item);
				}
				this.loadedDataSources = hashSet;
			}
		}

		private bool AreAnyLoaded(IEnumerable<string> dataSources)
		{
			return dataSources.Intersect(this.loadedDataSources, StringComparer.OrdinalIgnoreCase).Count<string>() > 0;
		}

		private bool AreAllLoaded(IEnumerable<string> dataSources)
		{
			return dataSources.Except(this.loadedDataSources, StringComparer.OrdinalIgnoreCase).Count<string>() == 0;
		}

		private readonly object dataSourcesLock;

		private readonly IEnumerable<string> preloadDataSources;

		private readonly IDataSourceReader dataSourceReader;

		private HashSet<string> loadedDataSources;
	}
}
