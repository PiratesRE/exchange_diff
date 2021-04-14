using System;
using System.IO;

namespace Microsoft.Exchange.VariantConfiguration.DataLoad
{
	internal class FilesDataSourceReader : IDataSourceReader
	{
		internal FilesDataSourceReader(string directory)
		{
			if (string.IsNullOrEmpty(directory))
			{
				throw new ArgumentNullException("directory");
			}
			if (!Directory.Exists(directory))
			{
				throw new ArgumentException(string.Format("Directory '{0}' does not exist.", directory));
			}
			this.directory = directory;
		}

		public Func<TextReader> GetContentReader(string dataSource)
		{
			if (string.IsNullOrEmpty(dataSource))
			{
				throw new ArgumentNullException("dataSource");
			}
			string filePath = Path.Combine(this.directory, dataSource);
			if (!File.Exists(filePath))
			{
				throw new ArgumentException(string.Format("'{0}' does not exist, therefore data source '{1}' cannot be read.", filePath, dataSource));
			}
			return () => new StreamReader(File.OpenRead(filePath));
		}

		public bool CanGetContentReader(string dataSource)
		{
			return !string.IsNullOrEmpty(dataSource) && File.Exists(Path.Combine(this.directory, dataSource));
		}

		private readonly string directory;
	}
}
