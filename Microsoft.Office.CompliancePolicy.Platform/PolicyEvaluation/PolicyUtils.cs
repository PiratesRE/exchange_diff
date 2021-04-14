using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	internal static class PolicyUtils
	{
		internal static bool CompareStringValues(object leftValue, object rightValue, IStringComparer comparer)
		{
			if (leftValue == null && rightValue == null)
			{
				return true;
			}
			if (leftValue == null || rightValue == null)
			{
				return false;
			}
			string text = leftValue as string;
			if (text != null)
			{
				string text2 = rightValue as string;
				if (text2 != null)
				{
					return comparer.Equals(text, text2);
				}
				IEnumerable<string> enumerable = rightValue as IEnumerable<string>;
				if (enumerable != null)
				{
					return PolicyUtils.MatchAny(enumerable, text, comparer);
				}
			}
			IEnumerable<string> enumerable2 = leftValue as IEnumerable<string>;
			if (enumerable2 != null)
			{
				string text3 = rightValue as string;
				if (text3 != null)
				{
					return PolicyUtils.MatchAny(enumerable2, text3, comparer);
				}
				IEnumerable<string> enumerable3 = rightValue as IEnumerable<string>;
				if (enumerable3 != null)
				{
					return PolicyUtils.MatchAny(enumerable2, enumerable3, comparer);
				}
			}
			throw new CompliancePolicyException("Only string values are supported!");
		}

		internal static bool CompareGuidValues(object leftValue, object rightValue)
		{
			if (leftValue == null && rightValue == null)
			{
				return true;
			}
			if (leftValue == null || rightValue == null)
			{
				return false;
			}
			Guid? guid = leftValue as Guid?;
			if (guid != null)
			{
				Guid? guid2 = rightValue as Guid?;
				if (guid2 != null)
				{
					return guid.Equals(guid2);
				}
				IEnumerable<Guid> enumerable = rightValue as IEnumerable<Guid>;
				if (enumerable != null)
				{
					return PolicyUtils.MatchAny<Guid>(enumerable, guid.Value);
				}
			}
			IEnumerable<Guid> enumerable2 = leftValue as IEnumerable<Guid>;
			if (enumerable2 != null)
			{
				Guid? guid3 = rightValue as Guid?;
				if (guid3 != null)
				{
					return PolicyUtils.MatchAny<Guid>(enumerable2, guid3.Value);
				}
				IEnumerable<Guid> enumerable3 = rightValue as IEnumerable<Guid>;
				if (enumerable3 != null)
				{
					return PolicyUtils.MatchAny<Guid>(enumerable2, enumerable3);
				}
			}
			throw new CompliancePolicyException("Only Guid values are supported!");
		}

		internal static bool CompareValues(object leftValue, object rightValue)
		{
			if (leftValue == null && rightValue == null)
			{
				return true;
			}
			if (leftValue == null || rightValue == null)
			{
				return false;
			}
			bool flag = PolicyUtils.IsTypeCollection(leftValue.GetType());
			bool flag2 = PolicyUtils.IsTypeCollection(rightValue.GetType());
			if (!flag)
			{
				if (!flag2)
				{
					return leftValue.Equals(rightValue);
				}
				return PolicyUtils.MatchAny((IEnumerable)rightValue, leftValue);
			}
			else
			{
				if (!flag2)
				{
					return PolicyUtils.MatchAny((IEnumerable)leftValue, rightValue);
				}
				return PolicyUtils.CollectionMatchAny((IEnumerable)leftValue, (IEnumerable)rightValue);
			}
		}

		internal static bool MatchAny<T>(IEnumerable<T> collection, T item)
		{
			return collection != null && collection.Any((T entry) => entry.Equals(item));
		}

		internal static bool MatchAny<T>(IEnumerable<T> listX, IEnumerable<T> listY)
		{
			return listX != null && listY != null && (from <>h__TransparentIdentifier3 in (from x in listX
			from y in listY
			select new
			{
				x,
				y
			}).Where(delegate(<>h__TransparentIdentifier3)
			{
				T x = <>h__TransparentIdentifier3.x;
				return x.Equals(<>h__TransparentIdentifier3.y);
			})
			select <>h__TransparentIdentifier3.x).Any<T>();
		}

		internal static bool MatchAny(IEnumerable<string> stringCollection, string str, IStringComparer comparer)
		{
			return stringCollection != null && stringCollection.Any((string entry) => comparer.Equals(entry, str));
		}

		internal static bool MatchAny(IEnumerable<string> listX, IEnumerable<string> listY, IStringComparer comparer)
		{
			if (listX == null || listY == null)
			{
				return false;
			}
			return (from x in listX
			from y in listY
			where comparer.Equals(x, y)
			select x).Any<string>();
		}

		internal static bool TryParseNullableDateTimeUtc(string input, out DateTime? outputDate)
		{
			if (string.IsNullOrEmpty(input))
			{
				outputDate = null;
				return true;
			}
			DateTime value;
			bool flag = DateTime.TryParse(input, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AssumeUniversal, out value);
			if (flag)
			{
				outputDate = new DateTime?(value);
				return true;
			}
			outputDate = null;
			return false;
		}

		internal static string DateTimeToUtcString(DateTime input)
		{
			return input.ToUniversalTime().ToString("u", CultureInfo.InvariantCulture);
		}

		internal static bool TryParseBool(string input, bool defaultValue, out bool output)
		{
			if (string.IsNullOrEmpty(input))
			{
				output = defaultValue;
				return true;
			}
			if (bool.TryParse(input, out output))
			{
				return true;
			}
			output = defaultValue;
			return false;
		}

		internal static bool IsTypeCollection(Type type)
		{
			return type != typeof(string) && type.GetInterface(typeof(IEnumerable).FullName) != null;
		}

		private static bool MatchAny(IEnumerable collection, object item)
		{
			foreach (object obj in collection)
			{
				if (obj != null && obj.Equals(item))
				{
					return true;
				}
			}
			return false;
		}

		private static bool CollectionMatchAny(IEnumerable collection1, IEnumerable collection2)
		{
			foreach (object obj in collection1)
			{
				foreach (object obj2 in collection2)
				{
					if (obj == null && obj2 == null)
					{
						return true;
					}
					if (obj != null && obj.Equals(obj2))
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}
