using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Entities.TypeConversion.Converters
{
	public struct SimpleMappingConverter<TLeft, TRight> : IConverter<TLeft, TRight>
	{
		private SimpleMappingConverter(ICollection<Tuple<TLeft, TRight>> mappings, SimpleMappingConverter<TLeft, TRight>.MappingBehavior behavior = SimpleMappingConverter<TLeft, TRight>.MappingBehavior.None, TRight defaultLeftToRight = default(TRight), TLeft defaultRightToLeft = default(TLeft))
		{
			this.defaultLeftToRight = defaultLeftToRight;
			this.defaultRightToLeft = defaultRightToLeft;
			int count = mappings.Count;
			this.forwardMappingDictionary = new Dictionary<TLeft, TRight>(count);
			this.backwardMappingDictionary = new Dictionary<TRight, TLeft>(count);
			foreach (Tuple<TLeft, TRight> tuple in mappings)
			{
				SimpleMappingConverter<TLeft, TRight>.AddMapping<TLeft, TRight>(tuple.Item1, tuple.Item2, this.forwardMappingDictionary);
				SimpleMappingConverter<TLeft, TRight>.AddMapping<TRight, TLeft>(tuple.Item2, tuple.Item1, this.backwardMappingDictionary);
			}
			this.throwOnNullLeftValue = SimpleMappingConverter<TLeft, TRight>.CheckBehavior(behavior, SimpleMappingConverter<TLeft, TRight>.MappingBehavior.ThrowOnNullLeftValue);
			this.throwOnNullRightValue = SimpleMappingConverter<TLeft, TRight>.CheckBehavior(behavior, SimpleMappingConverter<TLeft, TRight>.MappingBehavior.ThrowOnNullRightValue);
			this.throwOnMissingLeftToRightMapping = SimpleMappingConverter<TLeft, TRight>.CheckBehavior(behavior, SimpleMappingConverter<TLeft, TRight>.MappingBehavior.ThrowOnMissingLeftToRightMapping);
			this.throwOnMissingRightToLeftMapping = SimpleMappingConverter<TLeft, TRight>.CheckBehavior(behavior, SimpleMappingConverter<TLeft, TRight>.MappingBehavior.ThrowOnMissingRightToLeftMapping);
		}

		public ICollection<TLeft> LeftKeyCollection
		{
			get
			{
				return this.forwardMappingDictionary.Keys;
			}
		}

		public ICollection<TRight> RightKeyCollection
		{
			get
			{
				return this.backwardMappingDictionary.Keys;
			}
		}

		public static SimpleMappingConverter<TLeft, TRight> CreateStrictConverter(ICollection<Tuple<TLeft, TRight>> mappings)
		{
			return new SimpleMappingConverter<TLeft, TRight>(mappings, SimpleMappingConverter<TLeft, TRight>.MappingBehavior.Strict, default(TRight), default(TLeft));
		}

		public static SimpleMappingConverter<TLeft, TRight> CreateRelaxedConverter(ICollection<Tuple<TLeft, TRight>> mappings)
		{
			return new SimpleMappingConverter<TLeft, TRight>(mappings, SimpleMappingConverter<TLeft, TRight>.MappingBehavior.None, default(TRight), default(TLeft));
		}

		public TRight Convert(TLeft value)
		{
			return SimpleMappingConverter<TLeft, TRight>.Convert<TLeft, TRight>(value, this.forwardMappingDictionary, this.throwOnNullLeftValue, this.throwOnMissingLeftToRightMapping, this.defaultLeftToRight);
		}

		public TLeft Reverse(TRight value)
		{
			return SimpleMappingConverter<TLeft, TRight>.Convert<TRight, TLeft>(value, this.backwardMappingDictionary, this.throwOnNullRightValue, this.throwOnMissingRightToLeftMapping, this.defaultRightToLeft);
		}

		private static void AddMapping<TKey, TValue>(TKey key, TValue value, IDictionary<TKey, TValue> mappingTable)
		{
			if (mappingTable.ContainsKey(key))
			{
				throw new ArgumentException(string.Format("Mapping duplicate: {0}", key));
			}
			mappingTable.Add(key, value);
		}

		private static TDestination Convert<TSource, TDestination>(TSource value, IReadOnlyDictionary<TSource, TDestination> mappingDictionary, bool throwOnNullValue, bool throwOnMissingMapping, TDestination defaultDestination)
		{
			if (value == null)
			{
				if (throwOnNullValue)
				{
					throw new ExArgumentNullException("value");
				}
			}
			else
			{
				TDestination result;
				if (mappingDictionary.TryGetValue(value, out result))
				{
					return result;
				}
				if (throwOnMissingMapping)
				{
					throw new ExArgumentOutOfRangeException("value", value, "There's no mapping provided for the specified value.");
				}
			}
			return defaultDestination;
		}

		private static bool CheckBehavior(SimpleMappingConverter<TLeft, TRight>.MappingBehavior behavior, SimpleMappingConverter<TLeft, TRight>.MappingBehavior valueToCheck)
		{
			return (behavior & valueToCheck) == valueToCheck;
		}

		private readonly Dictionary<TRight, TLeft> backwardMappingDictionary;

		private readonly TRight defaultLeftToRight;

		private readonly TLeft defaultRightToLeft;

		private readonly Dictionary<TLeft, TRight> forwardMappingDictionary;

		private readonly bool throwOnMissingLeftToRightMapping;

		private readonly bool throwOnMissingRightToLeftMapping;

		private readonly bool throwOnNullLeftValue;

		private readonly bool throwOnNullRightValue;

		[Flags]
		public enum MappingBehavior
		{
			None = 0,
			ThrowOnNullLeftValue = 1,
			ThrowOnNullRightValue = 2,
			ThrowOnNullValue = 3,
			ThrowOnMissingLeftToRightMapping = 4,
			ThrowOnMissingRightToLeftMapping = 8,
			ThrowOnMissingAnyMapping = 12,
			Strict = 15
		}
	}
}
