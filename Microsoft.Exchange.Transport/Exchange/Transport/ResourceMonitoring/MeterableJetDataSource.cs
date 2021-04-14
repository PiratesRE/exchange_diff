using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Storage;

namespace Microsoft.Exchange.Transport.ResourceMonitoring
{
	internal class MeterableJetDataSource : IMeterableJetDataSource
	{
		public MeterableJetDataSource(DataSource dataSource)
		{
			ArgumentValidator.ThrowIfNull("dataSource", dataSource);
			this.dataSource = dataSource;
		}

		public string DatabasePath
		{
			get
			{
				return this.dataSource.DatabasePath;
			}
		}

		public long GetAvailableFreeSpace()
		{
			ulong availableFreeSpace = this.dataSource.GetAvailableFreeSpace();
			if (availableFreeSpace > 9223372036854775807UL)
			{
				throw new OverflowException("The data source returned available free space value larger than long.MaxValue");
			}
			return (long)availableFreeSpace;
		}

		public long GetDatabaseFileSize()
		{
			long result;
			try
			{
				if (this.databaseFileInfo == null)
				{
					this.databaseFileInfo = new FileInfo(this.DatabasePath);
				}
				this.databaseFileInfo.Refresh();
				result = this.databaseFileInfo.Length;
			}
			catch (IOException)
			{
				this.databaseFileInfo = null;
				result = 0L;
			}
			return result;
		}

		public long GetCurrentVersionBuckets()
		{
			return this.dataSource.GetCurrentVersionBuckets();
		}

		private readonly DataSource dataSource;

		private FileInfo databaseFileInfo;
	}
}
