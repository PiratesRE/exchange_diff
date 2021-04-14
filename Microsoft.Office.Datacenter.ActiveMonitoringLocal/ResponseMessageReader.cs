using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal class ResponseMessageReader
	{
		public void AddObject<T>(string objectName, T @object)
		{
			this.AddObjectResolver<T>(objectName, () => @object);
		}

		public void AddObjectResolver<T>(string objectName, Func<T> objectResolver)
		{
			this.objectNameToLazilyResolvedInstance[objectName] = new Lazy<ResponseMessageReader.ReplaceableObject>(() => new ResponseMessageReader.ReplaceableObject(objectResolver(), new Func<object, string, object>(ResponseMessageReader.Schema<T>.GetValue)));
		}

		public object GetObject(string objectName)
		{
			return this.GetReplaceableObject(objectName).InnerObject;
		}

		public string ReplaceValues(string replacementString)
		{
			return this.replacementExpression.Replace(replacementString, (Match match) => this.GetValue(match.Groups[1].Value, match.Groups[3].Value));
		}

		private ResponseMessageReader.ReplaceableObject GetReplaceableObject(string objectName)
		{
			Lazy<ResponseMessageReader.ReplaceableObject> lazy;
			if (this.objectNameToLazilyResolvedInstance.TryGetValue(objectName, out lazy))
			{
				return lazy.Value;
			}
			throw new ArgumentException(string.Format("Object resolver is not registered for name '{0}'", objectName));
		}

		private string GetValue(string objectName, string propertyOrFieldName)
		{
			object member = this.GetReplaceableObject(objectName).GetMember(propertyOrFieldName);
			if (member == null)
			{
				return string.Empty;
			}
			return member.ToString();
		}

		private readonly Dictionary<string, Lazy<ResponseMessageReader.ReplaceableObject>> objectNameToLazilyResolvedInstance = new Dictionary<string, Lazy<ResponseMessageReader.ReplaceableObject>>(StringComparer.InvariantCultureIgnoreCase);

		private readonly Regex replacementExpression = new Regex("\\{([a-zA-Z0-9_]*)(\\.([a-zA-Z0-9_]+))?\\}", RegexOptions.Compiled);

		private struct ReplaceableObject
		{
			public ReplaceableObject(object innerObject, Func<object, string, object> getter)
			{
				this = default(ResponseMessageReader.ReplaceableObject);
				this.getter = getter;
				this.InnerObject = innerObject;
			}

			public object InnerObject { get; private set; }

			public object GetMember(string name)
			{
				return this.getter(this.InnerObject, name);
			}

			private readonly Func<object, string, object> getter;
		}

		private static class Schema<T>
		{
			public static object GetValue(object inputObject, string propertyOrFieldName)
			{
				Func<object, object> func;
				if (!ResponseMessageReader.Schema<T>.propertyAndFieldGetters.TryGetValue(propertyOrFieldName, out func))
				{
					throw new ArgumentException(string.Format("Objects of type {0} don't have a field or property {1}", typeof(T), propertyOrFieldName));
				}
				if (inputObject == null)
				{
					return string.Empty;
				}
				return func(inputObject);
			}

			private static Dictionary<string, Func<object, object>> GetPropertiesAndFieldsGetters(Type objectType)
			{
				Dictionary<string, Func<object, object>> dictionary = new Dictionary<string, Func<object, object>>();
				PropertyInfo[] properties = objectType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
				for (int i = 0; i < properties.Length; i++)
				{
					PropertyInfo property = properties[i];
					dictionary[property.Name] = ((object subject) => property.GetValue(subject, null));
				}
				FieldInfo[] fields = objectType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
				for (int j = 0; j < fields.Length; j++)
				{
					FieldInfo field = fields[j];
					dictionary[field.Name] = ((object subject) => field.GetValue(subject));
				}
				dictionary.Add(string.Empty, (object subject) => subject.ToString());
				return dictionary;
			}

			private static readonly Dictionary<string, Func<object, object>> propertyAndFieldGetters = ResponseMessageReader.Schema<T>.GetPropertiesAndFieldsGetters(typeof(T));
		}
	}
}
