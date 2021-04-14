using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Exchange.Management.DDIService
{
	public class DDIStrongTypeConverter : IOutputConverter, IDDIConverter
	{
		public DDIStrongTypeConverter(Type targetType, ConvertMode convertMode)
		{
			this.targetType = targetType;
			this.convertMode = convertMode;
		}

		public bool CanConvert(object sourceObject)
		{
			if (sourceObject == null || !this.targetType.IsDataContract())
			{
				return false;
			}
			IEnumerable enumerable = sourceObject as IEnumerable;
			if (this.convertMode == ConvertMode.PerItemInEnumerable && enumerable != null)
			{
				Type type = null;
				foreach (object obj in enumerable)
				{
					if (type == null)
					{
						type = obj.GetType();
					}
					else if (type != obj.GetType())
					{
						throw new InvalidOperationException("Can't convert the items in an enumerable. The items are not objects of the same type");
					}
				}
				return type == null || this.targetType.GetConstructor(new Type[]
				{
					type
				}) != null;
			}
			return this.targetType.GetConstructor(new Type[]
			{
				sourceObject.GetType()
			}) != null;
		}

		public object Convert(object sourceObject)
		{
			IEnumerable enumerable = sourceObject as IEnumerable;
			if (this.convertMode == ConvertMode.PerItemInEnumerable && enumerable != null)
			{
				List<object> list = new List<object>();
				foreach (object obj in enumerable)
				{
					list.Add(Activator.CreateInstance(this.targetType, new object[]
					{
						obj
					}));
				}
				return list;
			}
			return Activator.CreateInstance(this.targetType, new object[]
			{
				sourceObject
			});
		}

		private Type targetType;

		private ConvertMode convertMode;
	}
}
