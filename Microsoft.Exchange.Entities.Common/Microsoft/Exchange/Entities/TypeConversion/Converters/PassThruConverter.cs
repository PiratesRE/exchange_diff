using System;

namespace Microsoft.Exchange.Entities.TypeConversion.Converters
{
	internal sealed class PassThruConverter<TValue> : IConverter<TValue, TValue>
	{
		private PassThruConverter()
		{
		}

		public static PassThruConverter<TValue> SingletonInstance
		{
			get
			{
				return PassThruConverter<TValue>.Instance;
			}
		}

		public TValue Convert(TValue value)
		{
			return value;
		}

		private static readonly PassThruConverter<TValue> Instance = new PassThruConverter<TValue>();
	}
}
