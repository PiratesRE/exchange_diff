using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	internal sealed class GetTopColumnSizesProcessor : GetTopColumnSizesProcessorBase
	{
		private GetTopColumnSizesProcessor(IList<Column> arguments) : base(arguments, false)
		{
		}

		public static ProcessorCollection.QueryableProcessor Info
		{
			get
			{
				return ProcessorCollection.QueryableProcessor.Create("GetTopColumnSizes", "Column identifier(s)", "Columns describing the largest values present based on uncompressed (logical) column size", "GetTopColumnSizes(Column1, Column2, ..., ColumnN)", new Func<IList<Column>, Processor>(GetTopColumnSizesProcessor.Create));
			}
		}

		private static Processor Create(IList<Column> arguments)
		{
			if (arguments.Count < 1)
			{
				throw new DiagnosticQueryException(DiagnosticQueryStrings.ProcessorEmptyArguments());
			}
			return new GetTopColumnSizesProcessor(arguments);
		}
	}
}
