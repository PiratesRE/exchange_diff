using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	internal sealed class GetTopCompressedColumnSizesProcessor : GetTopColumnSizesProcessorBase
	{
		private GetTopCompressedColumnSizesProcessor(IList<Column> arguments) : base(arguments, true)
		{
		}

		public static ProcessorCollection.QueryableProcessor Info
		{
			get
			{
				return ProcessorCollection.QueryableProcessor.Create("GetTopCompressedColumnSizes", "Column identifier(s)", "Columns describing the largest values present based on compressed (physical) column size", "GetTopCompressedColumnSizes(Column1, Column2, ..., ColumnN)", new Func<IList<Column>, Processor>(GetTopCompressedColumnSizesProcessor.Create));
			}
		}

		private static Processor Create(IList<Column> arguments)
		{
			if (arguments.Count < 1)
			{
				throw new DiagnosticQueryException(DiagnosticQueryStrings.ProcessorEmptyArguments());
			}
			return new GetTopCompressedColumnSizesProcessor(arguments);
		}
	}
}
