using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class EnumListSource : ObjectListSource
	{
		public EnumListSource(Type enumType) : this(Enum.GetValues(enumType), enumType)
		{
		}

		public EnumListSource(Array values, Type enumType) : base(values)
		{
			this.enumType = enumType;
		}

		public Type EnumType
		{
			get
			{
				return this.enumType;
			}
		}

		protected override string GetValueText(object objectValue)
		{
			string result = string.Empty;
			if (this.enumType.IsInstanceOfType(objectValue))
			{
				result = LocalizedDescriptionAttribute.FromEnum(this.enumType, objectValue);
			}
			return result;
		}

		private Type enumType;
	}
}
