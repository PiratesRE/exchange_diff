using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.FfoReporting.Data
{
	internal static class DataProcessorDriver
	{
		internal static IReadOnlyList<TOutputObject> Process<TOutputObject>(IEnumerable inputs, IDataProcessor processor)
		{
			return DataProcessorDriver.Process<TOutputObject>(inputs, new IDataProcessor[]
			{
				processor
			});
		}

		internal static IReadOnlyList<TOutputObject> Process<TOutputObject>(IEnumerable inputs, IEnumerable<IDataProcessor> groupOfProcessors)
		{
			List<TOutputObject> list = new List<TOutputObject>();
			if (inputs != null)
			{
				IEnumerable<IDataProcessor> enumerable = (groupOfProcessors != null) ? groupOfProcessors : new List<IDataProcessor>();
				foreach (object obj in inputs)
				{
					object obj2 = obj;
					foreach (IDataProcessor dataProcessor in enumerable)
					{
						obj2 = dataProcessor.Process(obj2);
					}
					list.Add((TOutputObject)((object)obj2));
				}
			}
			return list;
		}
	}
}
