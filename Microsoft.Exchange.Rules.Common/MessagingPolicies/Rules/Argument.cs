using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public abstract class Argument
	{
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

		public bool IsNumerical
		{
			get
			{
				return this.type == typeof(int) || this.type == typeof(ulong);
			}
		}

		public bool IsString
		{
			get
			{
				return Argument.IsStringType(this.type);
			}
		}

		public static Type GetTypeForName(string typeName)
		{
			Type result;
			if (Argument.knownTypes.TryGetValue(typeName, out result))
			{
				return result;
			}
			throw new RulesValidationException(RulesStrings.InvalidArgumentType(typeName));
		}

		public static string GetTypeName(Type type)
		{
			if (type.Equals(typeof(string)))
			{
				return "string";
			}
			if (type.Equals(typeof(int)))
			{
				return "integer";
			}
			if (type.Equals(typeof(ulong)))
			{
				return "ulong";
			}
			if (type.Equals(typeof(string[])))
			{
				return "string[]";
			}
			if (type.Equals(typeof(List<string>)))
			{
				return "stringlist";
			}
			if (type.Equals(typeof(ShortList<string>)))
			{
				return "stringshortlist";
			}
			if (type.Equals(typeof(ShortList<ShortList<KeyValuePair<string, string>>>)))
			{
				return "keyValue[][]";
			}
			throw new RulesValidationException(RulesStrings.InvalidArgumentType(type.Name));
		}

		public static bool IsStringType(Type type)
		{
			return type == typeof(string) || type == typeof(string[]) || type == typeof(List<string>) || type == typeof(ShortList<string>);
		}

		public abstract object GetValue(RulesEvaluationContext context);

		private static Dictionary<string, Type> InitializeKnownTypes()
		{
			return new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
			{
				{
					"string",
					typeof(string)
				},
				{
					"integer",
					typeof(int)
				},
				{
					"string[]",
					typeof(string[])
				},
				{
					"stringlist",
					typeof(List<string>)
				},
				{
					"stringshortlist",
					typeof(ShortList<string>)
				},
				{
					"ulong",
					typeof(ulong)
				},
				{
					"keyValue[][]",
					typeof(ShortList<ShortList<KeyValuePair<string, string>>>)
				}
			};
		}

		public virtual int GetEstimatedSize()
		{
			return 36;
		}

		private static Dictionary<string, Type> knownTypes = Argument.InitializeKnownTypes();

		private Type type;
	}
}
