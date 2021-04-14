using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PropertyBlob;
using Microsoft.Exchange.Server.Storage.PropTags;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	internal sealed class GetTopPropertySizesProcessor : GetTopSizesProcessor
	{
		private GetTopPropertySizesProcessor(IList<Column> arguments) : base("Property", ConfigurationSchema.ProcessorNumberOfPropertyResults.Value, arguments)
		{
		}

		public static ProcessorCollection.QueryableProcessor Info
		{
			get
			{
				return ProcessorCollection.QueryableProcessor.Create("GetTopPropertySizes", "Property blob column(s)", "Columns describing the largest properties present", "GetTopPropertySizes(PropertyBlob1, ..., PropertyBlobN)", new Func<IList<Column>, Processor>(GetTopPropertySizesProcessor.Create));
			}
		}

		public override List<Tuple<string, int>> GetData(SimpleQueryOperator qop, Reader reader)
		{
			List<Tuple<string, int>> list = new List<Tuple<string, int>>(100);
			if (reader != null)
			{
				foreach (Column column in base.Arguments.Values)
				{
					byte[] array = null;
					object value = reader.GetValue(column);
					LargeValue largeValue = value as LargeValue;
					if (value is byte[])
					{
						array = (byte[])value;
					}
					else if (largeValue != null)
					{
						IColumnStreamAccess columnStreamAccess = qop as IColumnStreamAccess;
						PhysicalColumn physicalColumn = column as PhysicalColumn;
						if (columnStreamAccess != null && physicalColumn != null)
						{
							int columnSize = columnStreamAccess.GetColumnSize(physicalColumn);
							byte[] array2 = new byte[columnSize];
							if (columnStreamAccess.ReadStream(physicalColumn, 0L, array2, 0, array2.Length) == columnSize)
							{
								array = array2;
							}
						}
					}
					if (array != null && array.Length > 0)
					{
						try
						{
							PropertyBlob.BlobReader blobReader = new PropertyBlob.BlobReader(array, 0);
							for (int i = 0; i < blobReader.PropertyCount; i++)
							{
								StorePropTag tag = StorePropTag.CreateWithoutInfo(blobReader.GetPropertyTag(i));
								object propertyValue = blobReader.GetPropertyValue(i);
								Property property = new Property(tag, propertyValue);
								try
								{
									list.Add(new Tuple<string, int>(property.Tag.ToString(), ValueTypeHelper.ValueSize(propertyValue)));
								}
								catch (InvalidCastException exception)
								{
									NullExecutionDiagnostics.Instance.OnExceptionCatch(exception);
									throw new DiagnosticQueryException(DiagnosticQueryStrings.ProcessorCustomError(string.Format("Property {0} Type conflicts with value", property.Tag.PropName)));
								}
							}
						}
						catch (InvalidBlobException exception2)
						{
							NullExecutionDiagnostics.Instance.OnExceptionCatch(exception2);
							throw new DiagnosticQueryException(DiagnosticQueryStrings.ProcessorCustomError(string.Format("PropertyBlob.BlobReader cannot parse value of {0}", column.Name)));
						}
					}
				}
			}
			return list;
		}

		private static Processor Create(IList<Column> arguments)
		{
			if (arguments.Count < 1)
			{
				throw new DiagnosticQueryException(DiagnosticQueryStrings.ProcessorEmptyArguments());
			}
			foreach (Column column in arguments)
			{
				if (column.Type != typeof(byte[]))
				{
					throw new DiagnosticQueryException(DiagnosticQueryStrings.ProcessorCustomError(string.Format("{0} is not a binary column", column.Name)));
				}
			}
			return new GetTopPropertySizesProcessor(arguments);
		}
	}
}
