using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Reflection;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Hygiene.Data;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.Dal
{
	public class FindOperation : ConfigDataProviderOperation
	{
		[XmlAttribute]
		public string QueryString { get; set; }

		[XmlAttribute]
		public string RootId { get; set; }

		[XmlArrayItem("Assert")]
		public string[] Asserts { get; set; }

		protected override void ExecuteConfigDataProviderOperation(IConfigDataProvider configDataProvider, IDictionary<string, object> variables)
		{
			Type type = DalProbeOperation.ResolveDataType(base.DataType);
			IConfigurable iConfigurable = (IConfigurable)Activator.CreateInstance(type);
			QueryFilter queryFilter = null;
			if (!string.IsNullOrEmpty(this.QueryString))
			{
				string text = this.QueryString.Trim();
				if (!string.IsNullOrEmpty(text))
				{
					ObjectSchema objectSchema = this.GetObjectSchema(iConfigurable);
					queryFilter = this.CreateQueryFilter(text, objectSchema);
				}
			}
			MethodInfo methodInfo = FindOperation.findMethod.MakeGenericMethod(new Type[]
			{
				type
			});
			MethodBase methodBase = methodInfo;
			object[] array = new object[4];
			array[0] = queryFilter;
			array[2] = false;
			IConfigurable[] value = (IConfigurable[])methodBase.Invoke(configDataProvider, array);
			variables[base.Return] = value;
		}

		private QueryFilter CreateQueryFilter(string queryString, ObjectSchema objectSchema)
		{
			QueryParser queryParser = new QueryParser(queryString, objectSchema, QueryParser.Capabilities.All, new QueryParser.EvaluateVariableDelegate(this.EvalDelegate), new QueryParser.ConvertValueFromStringDelegate(this.ConvertDelegate));
			return queryParser.ParseTree;
		}

		private ObjectSchema GetObjectSchema(IConfigurable iConfigurable)
		{
			ConfigurablePropertyBag configurablePropertyBag = iConfigurable as ConfigurablePropertyBag;
			if (configurablePropertyBag == null)
			{
				ConfigurableObject configurableObject = iConfigurable as ConfigurableObject;
				if (configurableObject == null)
				{
					return null;
				}
				return configurableObject.ObjectSchema;
			}
			else
			{
				Type schemaType = configurablePropertyBag.GetSchemaType();
				if (schemaType == null)
				{
					return null;
				}
				return (ObjectSchema)Activator.CreateInstance(schemaType);
			}
		}

		private object ConvertDelegate(object valueToConvert, Type resultType)
		{
			string text = valueToConvert as string;
			if (resultType == typeof(ADObjectId) && !string.IsNullOrEmpty(text) && !ADObjectId.IsValidDistinguishedName(text))
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
			return LanguagePrimitives.ConvertTo(text, resultType);
		}

		private object EvalDelegate(string varName)
		{
			return null;
		}

		private static MethodInfo findMethod = typeof(IConfigDataProvider).GetMethod("Find");
	}
}
