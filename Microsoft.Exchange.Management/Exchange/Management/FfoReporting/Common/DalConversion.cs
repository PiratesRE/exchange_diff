using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.FfoReporting.Common
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	internal sealed class DalConversion : Attribute
	{
		static DalConversion()
		{
			DalConversion.conversionFunctions.Add("DefaultSerializer", delegate(DalConversion instance, object reportObject, PropertyInfo property, object dalObject, object task)
			{
				instance.DefaultSerializer(reportObject, property, dalObject, task);
			});
			DalConversion.conversionFunctions.Add("DateFromStringSerializer", delegate(DalConversion instance, object reportObject, PropertyInfo property, object dalObject, object task)
			{
				instance.DateFromStringSerializer(reportObject, property, dalObject, task);
			});
			DalConversion.conversionFunctions.Add("DateFromIntSerializer", delegate(DalConversion instance, object reportObject, PropertyInfo property, object dalObject, object task)
			{
				instance.DateFromIntSerializer(reportObject, property, dalObject, task);
			});
			DalConversion.conversionFunctions.Add("OrganizationFromTask", delegate(DalConversion instance, object reportObject, PropertyInfo property, object dalObject, object task)
			{
				instance.OrganizationFromTask(reportObject, property, dalObject, task);
			});
			DalConversion.conversionFunctions.Add("ValueFromTask", delegate(DalConversion instance, object reportObject, PropertyInfo property, object dalObject, object task)
			{
				instance.ValueFromTask(reportObject, property, dalObject, task);
			});
		}

		internal DalConversion(string method, string dalPropertyName, params string[] optionalDalPropertyNames)
		{
			this.methodName = method;
			this.dalPropertyName = dalPropertyName;
			this.optionalDalPropertyNames = optionalDalPropertyNames;
		}

		internal string DalPropertyName
		{
			get
			{
				return this.dalPropertyName;
			}
		}

		internal string MethodName
		{
			get
			{
				return this.methodName;
			}
		}

		internal void SetOutput(object reportObject, PropertyInfo property, object dalObject, object task)
		{
			DalConversion.ConversionDelegate conversionDelegate = DalConversion.conversionFunctions[this.methodName];
			conversionDelegate(this, reportObject, property, dalObject, task);
		}

		private void DefaultSerializer(object reportingObject, PropertyInfo property, object dalObject, object task)
		{
			Type type = dalObject.GetType();
			object value = type.GetProperty(this.dalPropertyName).GetValue(dalObject, null);
			if (value != null)
			{
				property.SetValue(reportingObject, value, null);
			}
		}

		private void DateFromStringSerializer(object reportingObject, PropertyInfo property, object dalObject, object task)
		{
			Type type = dalObject.GetType();
			object value = type.GetProperty(this.dalPropertyName).GetValue(dalObject, null);
			if (value != null)
			{
				property.SetValue(reportingObject, DateTime.Parse((string)value), null);
			}
		}

		private void DateFromIntSerializer(object reportingObject, PropertyInfo property, object dalObject, object task)
		{
			Type type = dalObject.GetType();
			object value = type.GetProperty(this.dalPropertyName).GetValue(dalObject, null);
			if (value != null)
			{
				int date = (int)value;
				int hour = 0;
				if (this.optionalDalPropertyNames.Length > 0)
				{
					value = type.GetProperty(this.optionalDalPropertyNames[0]).GetValue(dalObject, null);
					if (value != null)
					{
						hour = (int)value;
					}
				}
				DateTime dateTime = Schema.Utilities.FromQueryDate(date, hour);
				property.SetValue(reportingObject, dateTime, null);
			}
		}

		private void OrganizationFromTask(object reportingObject, PropertyInfo property, object dalObject, object task)
		{
			Type type = task.GetType();
			OrganizationIdParameter organizationIdParameter = type.GetProperty(this.dalPropertyName).GetValue(task, null) as OrganizationIdParameter;
			if (organizationIdParameter != null)
			{
				string value = (organizationIdParameter.InternalADObjectId != null) ? organizationIdParameter.InternalADObjectId.Name : organizationIdParameter.RawIdentity;
				property.SetValue(reportingObject, value, null);
			}
		}

		private void ValueFromTask(object reportingObject, PropertyInfo property, object dalObject, object task)
		{
			Type type = task.GetType();
			object value = type.GetProperty(this.dalPropertyName).GetValue(task, null);
			object value2 = ValueConvertor.ConvertValue(value, property.PropertyType, null);
			property.SetValue(reportingObject, value2, null);
		}

		private const string RegexSplitPattern = ",";

		private static readonly Dictionary<string, DalConversion.ConversionDelegate> conversionFunctions = new Dictionary<string, DalConversion.ConversionDelegate>();

		private readonly string methodName;

		private readonly string dalPropertyName;

		private string[] optionalDalPropertyNames;

		internal static class Methods
		{
			internal const string Default = "DefaultSerializer";

			internal const string DateFromString = "DateFromStringSerializer";

			internal const string DateFromInt = "DateFromIntSerializer";

			internal const string Organization = "OrganizationFromTask";

			internal const string FromTask = "ValueFromTask";
		}

		private delegate void ConversionDelegate(DalConversion instance, object reportObject, PropertyInfo property, object dalObject, object task);
	}
}
