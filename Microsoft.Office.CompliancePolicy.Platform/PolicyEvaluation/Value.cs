using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public class Value : Argument
	{
		public Value(string value) : base(typeof(string))
		{
			this.value = value;
			this.rawValues = new List<string>();
			this.rawValues.Add(value);
		}

		public Value(List<string> rawValues) : base(typeof(List<string>))
		{
			this.value = rawValues;
			this.rawValues = rawValues;
		}

		private Value(object value, List<string> rawValues) : base(value.GetType())
		{
			this.value = value;
			this.rawValues = rawValues;
		}

		private Value(object value) : base(value.GetType())
		{
			this.value = value;
			this.rawValues = new List<string>();
		}

		public static Value Empty
		{
			get
			{
				return Value.emptyValue;
			}
		}

		public object ParsedValue
		{
			get
			{
				return this.value;
			}
		}

		internal List<string> RawValues
		{
			get
			{
				return this.rawValues;
			}
		}

		public static Value CreateValue(List<List<KeyValuePair<string, string>>> rawValues)
		{
			if (rawValues.Count == 0)
			{
				throw new CompliancePolicyValidationException("Value is not set");
			}
			return new Value(rawValues);
		}

		public static Value CreateValue(object value)
		{
			if (value == null)
			{
				throw new CompliancePolicyValidationException("Value is not set");
			}
			return new Value(value);
		}

		public override object GetValue(PolicyEvaluationContext context)
		{
			return this.value;
		}

		internal static Value CreateValue(Type type, List<string> rawValues)
		{
			if (rawValues.Count == 0)
			{
				throw new CompliancePolicyValidationException("Value is not set");
			}
			if (rawValues.Count > 1 || PolicyUtils.IsTypeCollection(type))
			{
				IList list = (IList)Value.ConstructCollection(type);
				foreach (string unparsedValue in rawValues)
				{
					object obj = Value.ParseSingleValue(type, unparsedValue);
					list.Add(obj);
				}
				return new Value(list, rawValues);
			}
			object obj2 = Value.ParseSingleValue(type, rawValues[0]);
			return new Value(obj2, rawValues);
		}

		internal static object ParseSingleValue(Type type, string unparsedValue)
		{
			object result = null;
			try
			{
				if (type == typeof(int))
				{
					result = Convert.ToInt32(unparsedValue);
				}
				else if (type == typeof(ulong))
				{
					result = Convert.ToUInt64(unparsedValue);
				}
				else if (type == typeof(string) || Argument.IsTypeCollectionOfType(type, typeof(string)))
				{
					result = unparsedValue;
				}
				else if (type == typeof(IPAddress))
				{
					result = IPAddress.Parse(unparsedValue);
				}
				else if (type == typeof(DateTime))
				{
					result = DateTime.Parse(unparsedValue);
				}
				else if (type == typeof(Guid) || Argument.IsTypeCollectionOfType(type, typeof(Guid)))
				{
					result = Guid.Parse(unparsedValue);
				}
				else if (type == typeof(AccessScope))
				{
					result = Enum.Parse(typeof(AccessScope), unparsedValue);
				}
				else if (type == typeof(bool))
				{
					result = bool.Parse(unparsedValue);
				}
				else
				{
					if (!(type == typeof(long)))
					{
						throw new CompliancePolicyValidationException("Invalid Property Type.");
					}
					result = long.Parse(unparsedValue);
				}
			}
			catch (FormatException)
			{
				throw new CompliancePolicyValidationException("Invalid argument type '{0}' for value '{1}'", new object[]
				{
					type.ToString(),
					unparsedValue
				});
			}
			catch (OverflowException)
			{
				throw new CompliancePolicyValidationException("Invalid argument type '{0}' for value '{1}'", new object[]
				{
					type.ToString(),
					unparsedValue
				});
			}
			catch (ArgumentException)
			{
				throw new CompliancePolicyValidationException("Invalid argument type '{0}' for value '{1}'", new object[]
				{
					type.ToString(),
					unparsedValue
				});
			}
			return result;
		}

		internal static object ConstructCollection(Type type)
		{
			if (type == typeof(int))
			{
				return new List<int>();
			}
			if (type == typeof(ulong))
			{
				return new List<ulong>();
			}
			if (type == typeof(string) || Argument.IsTypeCollectionOfType(type, typeof(string)))
			{
				return new List<string>();
			}
			if (type == typeof(IPAddress))
			{
				return new List<IPAddress>();
			}
			if (type == typeof(DateTime))
			{
				return new List<DateTime>();
			}
			if (type == typeof(Guid) || Argument.IsTypeCollectionOfType(type, typeof(Guid)))
			{
				return new List<Guid>();
			}
			if (type == typeof(AccessScope))
			{
				return new List<AccessScope>();
			}
			if (type == typeof(bool))
			{
				return new List<bool>();
			}
			if (type == typeof(long))
			{
				return new List<long>();
			}
			throw new CompliancePolicyValidationException("Invalid Property Type.");
		}

		private const string ValueNotSet = "Value is not set";

		private const string InvalidArgumentTypeFormat = "Invalid argument type '{0}' for value '{1}'";

		private static readonly Value emptyValue = new Value(new object(), new List<string>());

		private readonly object value;

		private readonly List<string> rawValues;
	}
}
