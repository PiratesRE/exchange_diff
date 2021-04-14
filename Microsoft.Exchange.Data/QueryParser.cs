using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Generated;

namespace Microsoft.Exchange.Data
{
	public class QueryParser
	{
		internal QueryParser(string query, ObjectSchema schema, QueryParser.Capabilities capabilities, QueryParser.EvaluateVariableDelegate evalDelegate, QueryParser.ConvertValueFromStringDelegate convertDelegate)
		{
			if (schema == null)
			{
				throw new ArgumentNullException("schema");
			}
			Hashtable allFilterable = new Hashtable(schema.AllFilterableProperties.Count, StringComparer.OrdinalIgnoreCase);
			foreach (PropertyDefinition propertyDefinition in schema.AllFilterableProperties)
			{
				allFilterable.Add(propertyDefinition.Name, propertyDefinition);
			}
			this.parser = new Parser(query, capabilities, (string propName) => (PropertyDefinition)allFilterable[propName], null, evalDelegate, convertDelegate);
		}

		internal QueryParser(string query, QueryParser.Capabilities capabilities, QueryParser.LookupPropertyDelegate schemaLookupDelegate, QueryParser.ListKnownPropertiesDelegate listKnownPropertiesDelegate, QueryParser.EvaluateVariableDelegate evalDelegate, QueryParser.ConvertValueFromStringDelegate convertDelegate)
		{
			this.parser = new Parser(query, capabilities, schemaLookupDelegate, listKnownPropertiesDelegate, evalDelegate, convertDelegate);
		}

		public QueryFilter ParseTree
		{
			get
			{
				return this.parser.ParseTree;
			}
		}

		private Parser parser;

		public delegate object EvaluateVariableDelegate(string varName);

		public delegate object ConvertValueFromStringDelegate(object value, Type targetType);

		internal delegate PropertyDefinition LookupPropertyDelegate(string propName);

		internal delegate IEnumerable<PropertyDefinition> ListKnownPropertiesDelegate();

		[Flags]
		public enum Capabilities
		{
			Or = 1,
			Like = 2,
			NotLike = 4,
			All = 65535
		}
	}
}
