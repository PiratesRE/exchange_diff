using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.EventMessages;
using Microsoft.Exchange.Management.FfoReporting.Common;

namespace Microsoft.Exchange.Management.FfoReporting.Data
{
	internal class ConversionProcessor : IDataProcessor
	{
		private ConversionProcessor(Type outputType, object parentInstance, int? startIndex)
		{
			this.targetOutputType = outputType;
			this.parentInstance = parentInstance;
			this.pagingIndex = startIndex;
			this.converters = Schema.Utilities.GetProperties<DalConversion>(this.targetOutputType);
		}

		internal static ConversionProcessor Create<TTargetConversionType>(object parentInstance)
		{
			if (parentInstance == null)
			{
				throw new ArgumentNullException("parentInstance");
			}
			return new ConversionProcessor(typeof(TTargetConversionType), parentInstance, null);
		}

		internal static ConversionProcessor CreatePageable<TTargetConversionType>(object parentInstance, int startIndex)
		{
			if (parentInstance == null)
			{
				throw new ArgumentNullException("parentInstance");
			}
			if (startIndex < 0)
			{
				throw new ArgumentOutOfRangeException("startIndex");
			}
			return new ConversionProcessor(typeof(TTargetConversionType), parentInstance, new int?(startIndex));
		}

		public object Process(object input)
		{
			object obj = null;
			if (input != null)
			{
				obj = Activator.CreateInstance(this.targetOutputType);
				foreach (Tuple<PropertyInfo, DalConversion> tuple in this.converters)
				{
					DalConversion item = tuple.Item2;
					PropertyInfo item2 = tuple.Item1;
					try
					{
						item.SetOutput(obj, item2, input, this.parentInstance);
					}
					catch (NullReferenceException)
					{
						string arg = (item2 != null) ? item2.Name : "Unknown";
						string text = string.Format("Null dalObj {0} reportObj {1}", item.DalPropertyName, arg);
						ExManagementApplicationLogger.LogEvent(ManagementEventLogConstants.Tuple_FfoReportingTaskFailure, new string[]
						{
							text
						});
					}
				}
				IPageableObject pageableObject = obj as IPageableObject;
				if (pageableObject != null && this.pagingIndex != null)
				{
					pageableObject.Index = this.pagingIndex.Value;
					this.pagingIndex++;
				}
			}
			return obj;
		}

		private readonly IList<Tuple<PropertyInfo, DalConversion>> converters;

		private readonly object parentInstance;

		private readonly Type targetOutputType;

		private int? pagingIndex = new int?(0);
	}
}
