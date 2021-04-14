using System;

namespace System.Resources
{
	internal struct ResourceLocator
	{
		internal ResourceLocator(int dataPos, object value)
		{
			this._dataPos = dataPos;
			this._value = value;
		}

		internal int DataPosition
		{
			get
			{
				return this._dataPos;
			}
		}

		internal object Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = value;
			}
		}

		internal static bool CanCache(ResourceTypeCode value)
		{
			return value <= ResourceTypeCode.TimeSpan;
		}

		internal object _value;

		internal int _dataPos;
	}
}
