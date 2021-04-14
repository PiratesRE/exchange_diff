using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using System.Web;

namespace Microsoft.Exchange.Data
{
	internal abstract class UMPartnerContext
	{
		protected abstract UMPartnerContext.UMPartnerContextSchema ContextSchema { get; }

		protected object this[UMPartnerContext.UMPartnerContextPropertyDefinition definition]
		{
			get
			{
				return this.propertyBag[definition.Name];
			}
			set
			{
				if (value != null)
				{
					this.propertyBag[definition.Name] = value;
					return;
				}
				this.propertyBag[definition.Name] = definition.DefaultValue;
			}
		}

		public static T Create<T>() where T : UMPartnerContext, new()
		{
			T t = Activator.CreateInstance<T>();
			List<UMPartnerContext.UMPartnerContextPropertyDefinition> propertyDefinitions = t.GetPropertyDefinitions();
			foreach (UMPartnerContext.UMPartnerContextPropertyDefinition umpartnerContextPropertyDefinition in propertyDefinitions)
			{
				t.propertyBag[umpartnerContextPropertyDefinition.Name] = umpartnerContextPropertyDefinition.DefaultValue;
			}
			return t;
		}

		public static T Parse<T>(string base64Data) where T : UMPartnerContext, new()
		{
			T result;
			if (!UMPartnerContext.TryParse<T>(base64Data, out result))
			{
				throw new ArgumentException("base64Data");
			}
			return result;
		}

		public static bool TryParse<T>(string base64Data, out T partnerContext) where T : UMPartnerContext, new()
		{
			partnerContext = default(T);
			try
			{
				if (!string.IsNullOrEmpty(base64Data))
				{
					string @string = Encoding.UTF8.GetString(Convert.FromBase64String(base64Data));
					NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(@string);
					if (nameValueCollection != null)
					{
						partnerContext = Activator.CreateInstance<T>();
						partnerContext.Initialize(nameValueCollection);
					}
				}
			}
			catch (FormatException)
			{
				partnerContext = default(T);
			}
			catch (ArgumentException)
			{
				partnerContext = default(T);
			}
			return partnerContext != null;
		}

		public override string ToString()
		{
			NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(string.Empty);
			foreach (object obj in this.propertyBag.Keys)
			{
				string text = (string)obj;
				object obj2 = this.propertyBag[text];
				if (obj2 != null)
				{
					nameValueCollection[text] = ValueConvertor.ConvertValueToString(obj2, null);
				}
			}
			return Convert.ToBase64String(Encoding.UTF8.GetBytes(nameValueCollection.ToString()));
		}

		private void Initialize(NameValueCollection keyValuePairs)
		{
			List<UMPartnerContext.UMPartnerContextPropertyDefinition> propertyDefinitions = this.GetPropertyDefinitions();
			foreach (UMPartnerContext.UMPartnerContextPropertyDefinition umpartnerContextPropertyDefinition in propertyDefinitions)
			{
				string text = keyValuePairs[umpartnerContextPropertyDefinition.Name];
				if (text == null)
				{
					this.propertyBag[umpartnerContextPropertyDefinition.Name] = umpartnerContextPropertyDefinition.DefaultValue;
				}
				else
				{
					this.propertyBag[umpartnerContextPropertyDefinition.Name] = umpartnerContextPropertyDefinition.Validate(text);
				}
			}
		}

		private List<UMPartnerContext.UMPartnerContextPropertyDefinition> GetPropertyDefinitions()
		{
			FieldInfo[] fields = this.ContextSchema.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			List<UMPartnerContext.UMPartnerContextPropertyDefinition> list = new List<UMPartnerContext.UMPartnerContextPropertyDefinition>(fields.Length);
			foreach (FieldInfo fieldInfo in fields)
			{
				if (typeof(UMPartnerContext.UMPartnerContextPropertyDefinition).Equals(fieldInfo.FieldType))
				{
					list.Add((UMPartnerContext.UMPartnerContextPropertyDefinition)fieldInfo.GetValue(this.ContextSchema));
				}
			}
			return list;
		}

		private Hashtable propertyBag = new Hashtable();

		protected class UMPartnerContextPropertyDefinition : PropertyDefinition
		{
			public UMPartnerContextPropertyDefinition(string name, Type type, object defaultValue) : base(name, type)
			{
				this.DefaultValue = defaultValue;
			}

			public object DefaultValue { get; private set; }

			public object Validate(string propertyValue)
			{
				object result;
				Exception innerException;
				if (!ValueConvertor.TryConvertValueFromString(propertyValue, base.Type, null, out result, out innerException))
				{
					throw new ArgumentException("Invalid property", base.Name, innerException);
				}
				return result;
			}
		}

		protected abstract class UMPartnerContextSchema
		{
		}
	}
}
