using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class EvaluatableFilter
	{
		public static bool Evaluate(QueryFilter filter, IReadOnlyPropertyBag propertyBag)
		{
			return EvaluatableFilter.Evaluate(filter, propertyBag, false);
		}

		public static bool Evaluate(QueryFilter filter, IReadOnlyPropertyBag propertyBag, bool shouldThrow)
		{
			AndFilter andFilter = filter as AndFilter;
			if (andFilter != null)
			{
				return EvaluatableFilter.EvaluateAndFilter(andFilter, propertyBag, shouldThrow);
			}
			OrFilter orFilter = filter as OrFilter;
			if (orFilter != null)
			{
				return EvaluatableFilter.EvaluateOrFilter(orFilter, propertyBag, shouldThrow);
			}
			NotFilter notFilter = filter as NotFilter;
			if (notFilter != null)
			{
				return !EvaluatableFilter.Evaluate(notFilter.Filter, propertyBag, shouldThrow);
			}
			TextFilter textFilter = filter as TextFilter;
			if (textFilter != null)
			{
				return EvaluatableFilter.EvaluateTextFilter(textFilter, propertyBag, shouldThrow);
			}
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			if (comparisonFilter != null)
			{
				return EvaluatableFilter.EvaluateComparisonFilter(comparisonFilter, propertyBag, shouldThrow);
			}
			if (filter is TrueFilter)
			{
				return true;
			}
			if (filter is FalseFilter)
			{
				return false;
			}
			CommentFilter commentFilter = filter as CommentFilter;
			if (commentFilter != null)
			{
				return EvaluatableFilter.Evaluate(commentFilter.Filter, propertyBag, shouldThrow);
			}
			BitMaskFilter bitMaskFilter = filter as BitMaskFilter;
			if (bitMaskFilter != null)
			{
				return EvaluatableFilter.EvaluateBitMaskFilter(bitMaskFilter, propertyBag, shouldThrow);
			}
			ExistsFilter existsFilter = filter as ExistsFilter;
			if (existsFilter != null)
			{
				return EvaluatableFilter.EvaluateExistsFilter(existsFilter, propertyBag);
			}
			PropertyComparisonFilter propertyComparisonFilter = filter as PropertyComparisonFilter;
			if (propertyComparisonFilter != null)
			{
				return EvaluatableFilter.EvaluatePropertyComparisonFilter(propertyComparisonFilter, propertyBag, shouldThrow);
			}
			throw new NotImplementedException();
		}

		private static bool EvaluateAndFilter(AndFilter andFilter, IReadOnlyPropertyBag propertyBag, bool shouldThrow)
		{
			foreach (QueryFilter filter in andFilter.Filters)
			{
				if (!EvaluatableFilter.Evaluate(filter, propertyBag, shouldThrow))
				{
					return false;
				}
			}
			return true;
		}

		private static bool EvaluateOrFilter(OrFilter filterOr, IReadOnlyPropertyBag propertyBag, bool shouldThrow)
		{
			foreach (QueryFilter filter in filterOr.Filters)
			{
				if (EvaluatableFilter.Evaluate(filter, propertyBag, shouldThrow))
				{
					return true;
				}
			}
			return false;
		}

		private static bool EvaluateTextFilter(TextFilter textFilter, IReadOnlyPropertyBag propertyBag, bool shouldThrow)
		{
			object obj = null;
			try
			{
				obj = propertyBag[textFilter.Property];
			}
			catch (PropertyErrorException)
			{
				if (shouldThrow)
				{
					throw;
				}
				return false;
			}
			if (EvaluatableFilter.IsPropertyError(propertyBag, textFilter.Property, shouldThrow))
			{
				return false;
			}
			if (obj == null)
			{
				return false;
			}
			string text = (string)obj;
			StringComparison comparisonType;
			if (textFilter.MatchFlags == MatchFlags.IgnoreCase)
			{
				comparisonType = StringComparison.OrdinalIgnoreCase;
			}
			else
			{
				if (textFilter.MatchFlags != MatchFlags.Default)
				{
					throw new NotSupportedException();
				}
				comparisonType = StringComparison.Ordinal;
			}
			switch (textFilter.MatchOptions)
			{
			case MatchOptions.FullString:
				return text.Equals(textFilter.Text, comparisonType);
			case MatchOptions.SubString:
				return text.IndexOf(textFilter.Text, comparisonType) >= 0;
			case MatchOptions.Prefix:
				return text.StartsWith(textFilter.Text, comparisonType);
			case MatchOptions.Suffix:
				return text.EndsWith(textFilter.Text, comparisonType);
			case MatchOptions.PrefixOnWords:
				return text.StartsWith(textFilter.Text, comparisonType) && (text.Length == textFilter.Text.Length || text[textFilter.Text.Length] == '.');
			default:
				throw new NotSupportedException();
			}
			bool result;
			return result;
		}

		private static bool EvaluateExistsFilter(ExistsFilter existsFilter, IReadOnlyPropertyBag propertyBag)
		{
			object obj = propertyBag[existsFilter.Property];
			PropertyError propertyError = obj as PropertyError;
			if (propertyError != null && propertyError.PropertyErrorCode == PropertyErrorCode.NotFound)
			{
				return false;
			}
			if (propertyBag is ADPropertyBag || propertyBag is ADRawEntry)
			{
				if (obj == null)
				{
					return false;
				}
				if (obj is string && string.IsNullOrEmpty((string)obj))
				{
					return false;
				}
				if (obj is SmtpAddress && !((SmtpAddress)obj).IsValidAddress)
				{
					return false;
				}
			}
			return true;
		}

		private static bool EvaluateComparisonFilter(ComparisonFilter comparisonFilter, IReadOnlyPropertyBag propertyBag, bool shouldThrow)
		{
			object obj;
			try
			{
				obj = propertyBag[comparisonFilter.Property];
			}
			catch (PropertyErrorException)
			{
				if (shouldThrow)
				{
					throw;
				}
				return false;
			}
			PropertyError propertyError = obj as PropertyError;
			if (propertyError != null)
			{
				if (shouldThrow)
				{
					throw PropertyErrorException.FromPropertyErrorsInternal(new PropertyError[]
					{
						propertyError
					});
				}
				return false;
			}
			else
			{
				if (EvaluatableFilter.IsPropertyError(propertyBag, comparisonFilter.Property, shouldThrow))
				{
					return false;
				}
				switch (comparisonFilter.ComparisonOperator)
				{
				case ComparisonOperator.Equal:
					if (comparisonFilter.PropertyValue is Participant)
					{
						return Participant.HasSameEmail(comparisonFilter.PropertyValue as Participant, obj as Participant, false);
					}
					if (obj is MultiValuedProperty<string> && comparisonFilter.PropertyValue is string)
					{
						MultiValuedProperty<string> multiValuedProperty = obj as MultiValuedProperty<string>;
						if (multiValuedProperty.Count == 1)
						{
							return multiValuedProperty[0].Equals(comparisonFilter.PropertyValue);
						}
					}
					if (obj is SmtpAddress && comparisonFilter.PropertyValue is EmailAddress)
					{
						return obj.Equals((comparisonFilter.PropertyValue as EmailAddress).Address);
					}
					return obj.Equals(comparisonFilter.PropertyValue);
				case ComparisonOperator.NotEqual:
					return !obj.Equals(comparisonFilter.PropertyValue);
				case ComparisonOperator.LessThan:
					if (comparisonFilter.PropertyValue is ExDateTime)
					{
						return (ExDateTime)obj < (ExDateTime)comparisonFilter.PropertyValue;
					}
					if (comparisonFilter.PropertyValue is int)
					{
						return (int)obj < (int)comparisonFilter.PropertyValue;
					}
					if (comparisonFilter.PropertyValue is long)
					{
						return (long)obj < (long)comparisonFilter.PropertyValue;
					}
					throw new InvalidOperationException("Only int, long, and ExDateTime supported for ComparisonFilter <");
				case ComparisonOperator.LessThanOrEqual:
					if (comparisonFilter.PropertyValue is ExDateTime)
					{
						return (ExDateTime)obj <= (ExDateTime)comparisonFilter.PropertyValue;
					}
					if (comparisonFilter.PropertyValue is int)
					{
						return (int)obj <= (int)comparisonFilter.PropertyValue;
					}
					if (comparisonFilter.PropertyValue is long)
					{
						return (long)obj <= (long)comparisonFilter.PropertyValue;
					}
					throw new InvalidOperationException("Only int, long, and ExDateTime supported for ComparisonFilter <=");
				case ComparisonOperator.GreaterThan:
					if (comparisonFilter.PropertyValue is ExDateTime)
					{
						return (ExDateTime)obj > (ExDateTime)comparisonFilter.PropertyValue;
					}
					if (comparisonFilter.PropertyValue is int)
					{
						return (int)obj > (int)comparisonFilter.PropertyValue;
					}
					if (comparisonFilter.PropertyValue is long)
					{
						return (long)obj > (long)comparisonFilter.PropertyValue;
					}
					throw new InvalidOperationException("Only int, long, and ExDateTime supported for ComparisonFilter >");
				case ComparisonOperator.GreaterThanOrEqual:
					if (comparisonFilter.PropertyValue is ExDateTime)
					{
						return (ExDateTime)obj >= (ExDateTime)comparisonFilter.PropertyValue;
					}
					if (comparisonFilter.PropertyValue is int)
					{
						return (int)obj >= (int)comparisonFilter.PropertyValue;
					}
					if (comparisonFilter.PropertyValue is long)
					{
						return (long)obj >= (long)comparisonFilter.PropertyValue;
					}
					throw new InvalidOperationException("Only int, long, and ExDateTime supported for ComparisonFilter >=");
				default:
					throw new InvalidOperationException("Unsupported ComparisonOperator: " + ((int)comparisonFilter.ComparisonOperator).ToString());
				}
			}
			bool result;
			return result;
		}

		private static bool EvaluateBitMaskFilter(BitMaskFilter bitMaskFilter, IReadOnlyPropertyBag propertyBag, bool shouldThrow)
		{
			object obj = null;
			try
			{
				obj = propertyBag[bitMaskFilter.Property];
			}
			catch (PropertyErrorException)
			{
				if (shouldThrow)
				{
					throw;
				}
				return false;
			}
			if (EvaluatableFilter.IsPropertyError(propertyBag, bitMaskFilter.Property, shouldThrow))
			{
				return false;
			}
			if (bitMaskFilter.IsNonZero)
			{
				return 0UL != ((ulong)obj & bitMaskFilter.Mask);
			}
			return 0UL == ((ulong)obj & bitMaskFilter.Mask);
		}

		private static bool EvaluatePropertyComparisonFilter(PropertyComparisonFilter propertyComparisonFilter, IReadOnlyPropertyBag propertyBag, bool shouldThrow)
		{
			object obj = propertyBag[propertyComparisonFilter.Property1];
			object obj2 = propertyBag[propertyComparisonFilter.Property2];
			if (obj is PropertyError || obj2 is PropertyError)
			{
				if (shouldThrow)
				{
					throw PropertyErrorException.FromPropertyErrorsInternal(new PropertyError[]
					{
						obj as PropertyError,
						obj2 as PropertyError
					});
				}
				return false;
			}
			else
			{
				switch (propertyComparisonFilter.ComparisonOperator)
				{
				case ComparisonOperator.Equal:
					return obj.Equals(obj2);
				case ComparisonOperator.NotEqual:
					return !obj.Equals(obj2);
				case ComparisonOperator.LessThan:
					if (obj is ExDateTime)
					{
						return (ExDateTime)obj < (ExDateTime)obj2;
					}
					if (obj is int)
					{
						return (int)obj < (int)obj2;
					}
					if (obj is long)
					{
						return (long)obj < (long)obj2;
					}
					throw new InvalidOperationException("Only int, long, and ExDateTime supported for PropertyComparisonFilter <");
				case ComparisonOperator.LessThanOrEqual:
					if (obj is ExDateTime)
					{
						return (ExDateTime)obj <= (ExDateTime)obj2;
					}
					if (obj is int)
					{
						return (int)obj <= (int)obj2;
					}
					if (obj is long)
					{
						return (long)obj <= (long)obj2;
					}
					throw new InvalidOperationException("Only int, long, and ExDateTime supported for PropertyComparisonFilter <=");
				case ComparisonOperator.GreaterThan:
					if (obj is ExDateTime)
					{
						return (ExDateTime)obj > (ExDateTime)obj2;
					}
					if (obj is int)
					{
						return (int)obj > (int)obj2;
					}
					if (obj is long)
					{
						return (long)obj > (long)obj2;
					}
					throw new InvalidOperationException("Only int, long, and ExDateTime supported for PropertyComparisonFilter >");
				case ComparisonOperator.GreaterThanOrEqual:
					if (obj is ExDateTime)
					{
						return (ExDateTime)obj >= (ExDateTime)obj2;
					}
					if (obj is int)
					{
						return (int)obj >= (int)obj2;
					}
					if (obj is long)
					{
						return (long)obj >= (long)obj2;
					}
					throw new InvalidOperationException("Only int, long, and ExDateTime supported for PropertyComparisonFilter >=");
				default:
					throw new InvalidOperationException("Unsupported ComparisonOperator: " + ((int)propertyComparisonFilter.ComparisonOperator).ToString());
				}
			}
		}

		private static bool IsPropertyError(IReadOnlyPropertyBag propertyBag, PropertyDefinition property, bool shouldThrow)
		{
			object obj = propertyBag[property];
			if (obj != null)
			{
				PropertyError propertyError = obj as PropertyError;
				if (propertyError != null)
				{
					if (shouldThrow)
					{
						throw PropertyErrorException.FromPropertyErrorsInternal(new PropertyError[]
						{
							obj as PropertyError
						});
					}
					return true;
				}
			}
			else if (propertyBag is ADPropertyBag || propertyBag is ADRawEntry)
			{
				if (shouldThrow)
				{
					throw PropertyErrorException.FromPropertyErrorsInternal(new PropertyError[]
					{
						new PropertyError(property, PropertyErrorCode.NotFound)
					});
				}
				return true;
			}
			return false;
		}
	}
}
