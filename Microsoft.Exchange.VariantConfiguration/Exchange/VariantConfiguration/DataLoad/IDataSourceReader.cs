using System;
using System.IO;

namespace Microsoft.Exchange.VariantConfiguration.DataLoad
{
	internal interface IDataSourceReader
	{
		Func<TextReader> GetContentReader(string dataSource);

		bool CanGetContentReader(string dataSource);
	}
}
