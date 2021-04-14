using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.TextProcessing;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public class Value : Argument
	{
		private Value(object value, ShortList<string> rawValues) : base(value.GetType())
		{
			this.value = value;
			this.rawValues = rawValues;
		}

		private Value(object value) : base(value.GetType())
		{
			this.value = value;
			this.rawValues = new ShortList<string>();
		}

		public Value(string value) : base(typeof(string))
		{
			this.value = value;
			this.rawValues = new ShortList<string>();
			this.rawValues.Add(value);
		}

		internal ShortList<string> RawValues
		{
			get
			{
				return this.rawValues;
			}
		}

		public object ParsedValue
		{
			get
			{
				return this.value;
			}
		}

		public static Value CreateValue(IMatch parsedObject, IList<string> rawValues)
		{
			return new Value(parsedObject, new ShortList<string>(rawValues));
		}

		internal static Value CreateValue(IMatch parsedObject, ShortList<string> rawValues)
		{
			return new Value(parsedObject, rawValues);
		}

		internal static Value CreateValue(Type type, ShortList<string> rawValues)
		{
			if (rawValues.Count == 0)
			{
				throw new RulesValidationException(RulesStrings.MissingValue);
			}
			if (rawValues.Count <= 1)
			{
				object obj = Value.ParseSingleValue(type, rawValues[0]);
				return new Value(obj, rawValues);
			}
			if (!Argument.IsStringType(type))
			{
				throw new RulesValidationException(RulesStrings.StringArrayPropertyRequiredForMultiValue);
			}
			List<string> list = new List<string>(rawValues);
			return new Value(list, rawValues);
		}

		public static Value CreateValue(ShortList<ShortList<KeyValuePair<string, string>>> rawValues)
		{
			if (rawValues.Count == 0)
			{
				throw new RulesValidationException(RulesStrings.MissingValue);
			}
			return new Value(rawValues);
		}

		public static Value CreateValue(object value)
		{
			if (value == null)
			{
				throw new RulesValidationException(RulesStrings.MissingValue);
			}
			return new Value(value);
		}

		internal static object ParseSingleValue(Type type, string unparsedValue)
		{
			object result = null;
			try
			{
				if (type.Equals(typeof(int)))
				{
					result = Convert.ToInt32(unparsedValue);
				}
				else if (type.Equals(typeof(ulong)))
				{
					result = Convert.ToUInt64(unparsedValue);
				}
				else if (Argument.IsStringType(type))
				{
					result = unparsedValue;
				}
				else
				{
					if (!type.Equals(typeof(IPAddress)))
					{
						throw new InvalidOperationException("Invalid Property Type.");
					}
					result = IPAddress.Parse(unparsedValue);
				}
			}
			catch (FormatException)
			{
				throw new RulesValidationException(RulesStrings.InvalidArgumentForType(unparsedValue, type.ToString()));
			}
			catch (OverflowException)
			{
				throw new RulesValidationException(RulesStrings.InvalidArgumentForType(unparsedValue, type.ToString()));
			}
			catch (ArgumentException)
			{
				throw new RulesValidationException(RulesStrings.InvalidArgumentForType(unparsedValue, type.ToString()));
			}
			return result;
		}

		public override object GetValue(RulesEvaluationContext context)
		{
			return this.value;
		}

		public override int GetEstimatedSize()
		{
			int num = 0;
			if (this.value != null)
			{
				string text = this.value as string;
				if (text != null)
				{
					num += 18;
					num += text.Length * 2;
				}
				else
				{
					ICollection<string> collection = this.value as ICollection<string>;
					if (collection != null)
					{
						num += 18;
						using (IEnumerator<string> enumerator = collection.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								string text2 = enumerator.Current;
								num += 18;
								num += text2.Length * 2;
							}
							goto IL_7D;
						}
					}
					num += 18;
				}
			}
			IL_7D:
			if (this.rawValues != null)
			{
				num += 18;
				foreach (string text3 in this.rawValues)
				{
					num += text3.Length * 2;
					num += 18;
				}
			}
			return num + base.GetEstimatedSize();
		}

		private object value;

		private ShortList<string> rawValues;

		public static Value Empty = new Value(new object(), new ShortList<string>());
	}
}
