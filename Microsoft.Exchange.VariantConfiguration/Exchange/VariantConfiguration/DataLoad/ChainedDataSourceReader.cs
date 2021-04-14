using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.Exchange.VariantConfiguration.DataLoad
{
	internal class ChainedDataSourceReader : IDataSourceReader
	{
		public ChainedDataSourceReader()
		{
			this.dataSourceReaders = new LinkedList<IDataSourceReader>();
		}

		public Func<TextReader> GetContentReader(string dataSource)
		{
			if (string.IsNullOrEmpty(dataSource))
			{
				throw new ArgumentNullException("dataSource");
			}
			foreach (IDataSourceReader dataSourceReader in this.dataSourceReaders)
			{
				if (dataSourceReader.CanGetContentReader(dataSource))
				{
					return dataSourceReader.GetContentReader(dataSource);
				}
			}
			throw new ArgumentException(string.Format("Unable to get content reader for data source '{0}'", dataSource));
		}

		public bool CanGetContentReader(string dataSource)
		{
			foreach (IDataSourceReader dataSourceReader in this.dataSourceReaders)
			{
				if (dataSourceReader.CanGetContentReader(dataSource))
				{
					return true;
				}
			}
			return false;
		}

		public void AddDataSourceReader(IDataSourceReader dataSourceReader)
		{
			if (dataSourceReader == null)
			{
				throw new ArgumentNullException("dataSourceReader");
			}
			this.dataSourceReaders.AddLast(dataSourceReader);
		}

		private readonly LinkedList<IDataSourceReader> dataSourceReaders;
	}
}
