using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	internal abstract class GetColumnSizesProcessorBase : Processor
	{
		protected GetColumnSizesProcessorBase(IList<Column> arguments, bool compressedSize) : base(GetColumnSizesProcessorBase.GetArgumentColumns(arguments, compressedSize))
		{
			this.generated = new List<Processor.ColumnDefinition>(arguments.Count);
			foreach (Column column in base.Arguments.Values)
			{
				this.generated.Add(new Processor.ColumnDefinition(column.Name, typeof(int), Visibility.Public));
			}
		}

		public override IEnumerable<Processor.ColumnDefinition> GetGeneratedColumns()
		{
			return this.generated;
		}

		public override object GetValue(SimpleQueryOperator qop, Reader reader, Column column)
		{
			Column column2 = null;
			if (!base.Arguments.TryGetValue(column.Name, out column2))
			{
				throw new DiagnosticQueryException(DiagnosticQueryStrings.ProcessorColumnNotFound(column.Name));
			}
			return reader.GetValue(column2);
		}

		private static IList<Column> GetArgumentColumns(IList<Column> arguments, bool compressedSize)
		{
			Column[] array = new Column[arguments.Count];
			string str = compressedSize ? "_CompressedSize" : "_Size";
			for (int i = 0; i < arguments.Count; i++)
			{
				array[i] = Factory.CreateSizeOfColumn(arguments[i].Name + str, arguments[i], compressedSize);
			}
			return array;
		}

		private readonly IList<Processor.ColumnDefinition> generated;
	}
}
