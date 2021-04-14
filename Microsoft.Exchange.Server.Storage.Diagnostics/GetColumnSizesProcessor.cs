using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	internal sealed class GetColumnSizesProcessor : GetColumnSizesProcessorBase
	{
		private GetColumnSizesProcessor(IList<Column> arguments) : base(arguments, false)
		{
		}

		public static ProcessorCollection.QueryableProcessor Info
		{
			get
			{
				return ProcessorCollection.QueryableProcessor.Create("GetColumnSizes", "Column identifier(s)", "Uncompressed (logical) size information for the value of each column", "GetColumnSizes(Column1, Column2, ..., ColumnN)", new Func<IList<Column>, Processor>(GetColumnSizesProcessor.Create));
			}
		}

		private static Processor Create(IList<Column> arguments)
		{
			if (arguments.Count < 1)
			{
				throw new DiagnosticQueryException(DiagnosticQueryStrings.ProcessorEmptyArguments());
			}
			return new GetColumnSizesProcessor(arguments);
		}
	}
}
