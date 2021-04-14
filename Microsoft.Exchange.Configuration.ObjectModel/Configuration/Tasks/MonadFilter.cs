using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MonadFilter
	{
		public MonadFilter(string filterText, PSCmdlet cmdlet, ObjectSchema schema) : this(filterText, cmdlet, schema, QueryParser.Capabilities.All)
		{
		}

		public MonadFilter(string filterText, PSCmdlet cmdlet, ObjectSchema schema, QueryParser.Capabilities capabilities)
		{
			QueryParser.EvaluateVariableDelegate evalDelegate = null;
			QueryParser.ConvertValueFromStringDelegate convertDelegate = new QueryParser.ConvertValueFromStringDelegate(MonadFilter.ConvertValueFromString);
			if (cmdlet != null)
			{
				evalDelegate = new QueryParser.EvaluateVariableDelegate(cmdlet.GetVariableValue);
			}
			QueryParser queryParser = new QueryParser(filterText, schema, capabilities, evalDelegate, convertDelegate);
			this.innerFilter = queryParser.ParseTree;
		}

		public static object ConvertValueFromString(object valueToConvert, Type resultType)
		{
			string text = valueToConvert as string;
			if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				bool flag = text == null || "null".Equals(text, StringComparison.OrdinalIgnoreCase) || "$null".Equals(text, StringComparison.OrdinalIgnoreCase);
				if (flag)
				{
					return null;
				}
			}
			if (resultType.Equals(typeof(ADObjectId)) && !string.IsNullOrEmpty(text) && !ADObjectId.IsValidDistinguishedName(text))
			{
				try
				{
					text = NativeHelpers.DistinguishedNameFromCanonicalName(text);
				}
				catch (NameConversionException)
				{
					throw new FormatException(DirectoryStrings.InvalidDNFormat(text));
				}
			}
			if (!resultType.Equals(typeof(bool)) && !resultType.Equals(typeof(bool?)))
			{
				return LanguagePrimitives.ConvertTo(text, resultType);
			}
			if (text == null)
			{
				return false;
			}
			return bool.Parse(text);
		}

		public QueryFilter InnerFilter
		{
			get
			{
				return this.innerFilter;
			}
		}

		public override string ToString()
		{
			if (this.innerFilter == null)
			{
				return string.Empty;
			}
			return this.innerFilter.GenerateInfixString(FilterLanguage.Monad);
		}

		private QueryFilter innerFilter;
	}
}
