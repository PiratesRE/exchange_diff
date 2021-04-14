using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	internal abstract class GetTopColumnSizesProcessorBase : GetTopSizesProcessor
	{
		protected GetTopColumnSizesProcessorBase(IList<Column> arguments, bool compressedSize) : base(compressedSize ? "CompressedColumn" : "Column", ConfigurationSchema.ProcessorNumberOfColumnResults.Value, GetTopColumnSizesProcessorBase.GetArgumentColumns(arguments, compressedSize))
		{
		}

		public override List<Tuple<string, int>> GetData(SimpleQueryOperator qop, Reader reader)
		{
			List<Tuple<string, int>> list = new List<Tuple<string, int>>(100);
			if (reader != null)
			{
				foreach (Column column in base.Arguments.Values)
				{
					int item = (int)reader.GetValue(column);
					list.Add(new Tuple<string, int>(column.Name, item));
				}
			}
			return list;
		}

		private static IList<Column> GetArgumentColumns(IList<Column> arguments, bool compressedSize)
		{
			Column[] array = new Column[arguments.Count];
			for (int i = 0; i < arguments.Count; i++)
			{
				array[i] = Factory.CreateSizeOfColumn(arguments[i].Name, arguments[i], compressedSize);
			}
			return array;
		}
	}
}
