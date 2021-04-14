using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.GlobalLocatorService
{
	internal class PropertyValue
	{
		internal static PropertyValue Create(string rawStringValue, GlsProperty propertyDefinition)
		{
			string text = null;
			int num = -1;
			bool flag = false;
			ValidationError validationError = null;
			if (propertyDefinition.DataType == typeof(string))
			{
				if (string.IsNullOrWhiteSpace(rawStringValue))
				{
					text = (propertyDefinition.DefaultValue as string);
				}
				else
				{
					text = rawStringValue;
				}
			}
			else if (propertyDefinition.DataType == typeof(int))
			{
				if (string.IsNullOrWhiteSpace(rawStringValue))
				{
					num = (int)propertyDefinition.DefaultValue;
				}
				else if (!int.TryParse(rawStringValue, out num))
				{
					string text2 = string.Format("ArgumentException: Unsupported {0} property data format:{1}", "int", rawStringValue);
					ExTraceGlobals.GLSTracer.TraceError(0L, text2);
					validationError = new GlsPropertyValidationError(new LocalizedString(text2), propertyDefinition, rawStringValue);
					num = (int)propertyDefinition.DefaultValue;
				}
			}
			else
			{
				if (!(propertyDefinition.DataType == typeof(bool)))
				{
					throw new ArgumentException(string.Format("unsupported PropertyDataType:{0}", propertyDefinition.DataType), "propertyDefinition.DataType");
				}
				if (string.IsNullOrWhiteSpace(rawStringValue))
				{
					flag = (bool)propertyDefinition.DefaultValue;
				}
				else if (!bool.TryParse(rawStringValue, out flag))
				{
					int num2;
					if (int.TryParse(rawStringValue, out num2))
					{
						flag = (num2 != 0);
					}
					else
					{
						string text3 = string.Format("ArgumentException: Unsupported {0} property data format:{1}", "boolean", rawStringValue);
						ExTraceGlobals.GLSTracer.TraceError(0L, text3);
						validationError = new GlsPropertyValidationError(new LocalizedString(text3), propertyDefinition, rawStringValue);
						flag = (bool)propertyDefinition.DefaultValue;
					}
				}
			}
			return new PropertyValue(propertyDefinition.DataType, text, num, flag, validationError);
		}

		internal PropertyValue(string stringValue)
		{
			this.dataType = typeof(string);
			this.stringValue = stringValue;
			this.intValue = -1;
			this.boolValue = false;
		}

		internal PropertyValue(int intValue)
		{
			this.dataType = typeof(int);
			this.stringValue = null;
			this.intValue = intValue;
			this.boolValue = false;
		}

		internal PropertyValue(bool boolValue)
		{
			this.dataType = typeof(bool);
			this.stringValue = null;
			this.intValue = -1;
			this.boolValue = boolValue;
		}

		private PropertyValue(Type dataType, string stringValue, int intValue, bool boolValue, ValidationError validationError)
		{
			this.dataType = dataType;
			this.stringValue = stringValue;
			this.intValue = intValue;
			this.boolValue = boolValue;
			this.validationError = validationError;
		}

		internal Type DataType
		{
			get
			{
				return this.dataType;
			}
		}

		internal string GetStringValue()
		{
			if (this.dataType != typeof(string))
			{
				throw new InvalidOperationException("property is not of dataType string");
			}
			return this.stringValue;
		}

		internal int GetIntValue()
		{
			if (this.dataType != typeof(int))
			{
				throw new InvalidOperationException("property is not of dataType int");
			}
			return this.intValue;
		}

		internal bool GetBoolValue()
		{
			if (this.dataType != typeof(bool))
			{
				throw new InvalidOperationException("property is not of dataType bool");
			}
			return this.boolValue;
		}

		internal bool IsValid
		{
			get
			{
				return this.validationError == null;
			}
		}

		internal ValidationError GetValidationError
		{
			get
			{
				return this.validationError;
			}
		}

		public override string ToString()
		{
			string result;
			if (this.dataType == typeof(string))
			{
				result = this.stringValue;
			}
			else if (this.dataType == typeof(int))
			{
				result = Convert.ToString(this.intValue);
			}
			else
			{
				if (!(this.dataType == typeof(bool)))
				{
					throw new InvalidOperationException(string.Format("unsupported PropertyDataType:{0}", this.dataType));
				}
				result = Convert.ToString(this.boolValue);
			}
			return result;
		}

		private const string validationErrorFormat = "ArgumentException: Unsupported {0} property data format:{1}";

		private readonly Type dataType;

		private readonly string stringValue;

		private readonly int intValue;

		private readonly bool boolValue;

		private readonly ValidationError validationError;
	}
}
