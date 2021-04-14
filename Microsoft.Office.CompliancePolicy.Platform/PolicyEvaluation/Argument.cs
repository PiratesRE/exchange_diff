using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public abstract class Argument
	{
		static Argument()
		{
			Argument.supportedComparableTypes.AddRange(Argument.supportedNumericalTypes);
			Argument.supportedEquatableTypes = new List<Type>
			{
				typeof(string),
				typeof(Guid),
				typeof(bool),
				typeof(Enum)
			};
			Argument.supportedEquatableTypes.AddRange(Argument.supportedComparableTypes);
		}

		public Argument(Type type)
		{
			this.type = type;
		}

		public Type Type
		{
			get
			{
				return this.type;
			}
		}

		public bool IsNumericalType
		{
			get
			{
				return Argument.supportedNumericalTypes.Contains(this.type);
			}
		}

		public bool IsComparableType
		{
			get
			{
				return Argument.supportedComparableTypes.Contains(this.type);
			}
		}

		public bool IsEquatableType
		{
			get
			{
				return Argument.IsTypeEquatable(this.type);
			}
		}

		public bool IsEquatableCollectionType
		{
			get
			{
				return Argument.IsTypeEquatableCollection(this.type);
			}
		}

		public bool IsEnumType
		{
			get
			{
				return this.type.BaseType == typeof(Enum);
			}
		}

		public bool IsCollectionOfType(Type elementType)
		{
			return Argument.IsTypeCollectionOfType(this.type, elementType);
		}

		public abstract object GetValue(PolicyEvaluationContext context);

		public bool IsComparableTo(Argument argument)
		{
			return this.IsComparableType && this.type == argument.type;
		}

		public bool IsEquatableTo(Argument argument)
		{
			bool result = false;
			if (this.IsEquatableType)
			{
				if (argument.IsEquatableType)
				{
					result = (this.Type == argument.Type);
				}
				else
				{
					result = argument.IsCollectionOfType(this.Type);
				}
			}
			else if (argument.IsEquatableType)
			{
				result = this.IsCollectionOfType(argument.Type);
			}
			else
			{
				foreach (Type elementType in Argument.supportedEquatableTypes)
				{
					if (this.IsCollectionOfType(elementType) && argument.IsCollectionOfType(elementType))
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}

		internal static bool IsTypeEquatable(Type type)
		{
			return Argument.supportedEquatableTypes.Contains(type) || type.BaseType == typeof(Enum);
		}

		internal static bool IsTypeEquatableCollection(Type type)
		{
			foreach (Type elementType in Argument.supportedEquatableTypes)
			{
				if (Argument.IsTypeCollectionOfType(type, elementType))
				{
					return true;
				}
			}
			return false;
		}

		internal static bool IsTypeCollectionOfType(Type type, Type elementType)
		{
			Type @interface = type.GetInterface(typeof(IEnumerable<>).Name);
			if (@interface != null)
			{
				Type[] genericArguments = @interface.GetGenericArguments();
				return genericArguments.Count<Type>() == 1 && (genericArguments.First<Type>() == elementType || genericArguments.First<Type>().BaseType == elementType);
			}
			return false;
		}

		private static List<Type> supportedNumericalTypes = new List<Type>
		{
			typeof(int),
			typeof(long),
			typeof(ulong)
		};

		private static List<Type> supportedComparableTypes = new List<Type>
		{
			typeof(DateTime)
		};

		private static List<Type> supportedEquatableTypes;

		private Type type;
	}
}
