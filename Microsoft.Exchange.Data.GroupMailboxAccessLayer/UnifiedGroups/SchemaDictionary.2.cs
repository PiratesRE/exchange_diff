using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.UnifiedGroups
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Office.Server.Directory")]
	public class SchemaDictionary<T> : SchemaDictionary
	{
		private SchemaDictionary()
		{
		}

		internal SchemaDictionary(T owner, string bagName, Action<string> propertyKeyValidator, Action<string, object> propertyValueValidator, Func<string, SchemaDictionary, object> propertyValueGetterTransfom, Func<string, object, bool, object> propertyValueSetterTransform)
		{
			this._owner = owner;
			this._PropertyKeyValidator = propertyKeyValidator;
			this._PropertyValueValidator = propertyValueValidator;
			this._PropertyValueGetterTransform = propertyValueGetterTransfom;
			this._PropertyValueSetterTransform = propertyValueSetterTransform;
			if (!SchemaDictionary<T>._SchemaDictionaryNameToAccessDelegates.TryGetValue(bagName, out this._accessDelegates))
			{
				this._accessDelegates = SchemaDictionary<T>._emptyAccessDelegates;
			}
		}

		public override object this[string key]
		{
			get
			{
				this.ValidateKey(key);
				Func<T, object> func;
				if (this._accessDelegates._Getters.TryGetValue(key, out func))
				{
					return func(this._owner);
				}
				if (this._PropertyValueGetterTransform != null)
				{
					return this._PropertyValueGetterTransform(key, this);
				}
				return this._Properties[key];
			}
			set
			{
				if (this._accessDelegates._ForbiddenSetters.Contains(key))
				{
					throw new FieldAccessException(key + " is not accessible because its setter is protected or private.");
				}
				this.ValidateValue(key, value);
				this.InternalSet(key, value);
			}
		}

		public override bool ContainsKey(string key)
		{
			return this._Properties.ContainsKey(key);
		}

		public override bool TryGetValue(string key, out object value)
		{
			if (this._Properties.TryGetValue(key, out value))
			{
				return true;
			}
			value = null;
			return false;
		}

		public override IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			foreach (KeyValuePair<string, object> pair in this._Properties)
			{
				IDictionary<string, Func<T, object>> getters = this._accessDelegates._Getters;
				KeyValuePair<string, object> keyValuePair = pair;
				if (!getters.ContainsKey(keyValuePair.Key))
				{
					KeyValuePair<string, object> keyValuePair2 = pair;
					string key = keyValuePair2.Key;
					KeyValuePair<string, object> keyValuePair3 = pair;
					yield return new KeyValuePair<string, object>(key, this[keyValuePair3.Key]);
				}
			}
			foreach (KeyValuePair<string, Func<T, object>> getter in this._accessDelegates._Getters)
			{
				KeyValuePair<string, Func<T, object>> keyValuePair4 = getter;
				string key2 = keyValuePair4.Key;
				KeyValuePair<string, Func<T, object>> keyValuePair5 = getter;
				yield return new KeyValuePair<string, object>(key2, keyValuePair5.Value(this._owner));
			}
			yield break;
		}

		protected static Dictionary<string, Func<T, object>> _AllGetters
		{
			get
			{
				return SchemaDictionary<T>._allGetters;
			}
		}

		protected static Dictionary<string, Action<T, object>> _AllSetters
		{
			get
			{
				return SchemaDictionary<T>._allSetters;
			}
		}

		private static Dictionary<string, SchemaDictionary<T>.AccessDelegates> _SchemaDictionaryNameToAccessDelegates
		{
			get
			{
				return SchemaDictionary<T>._schemaDictionaryNameToAccessDelegates;
			}
		}

		protected void ValidateKey(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentException("key is null or empty", "key");
			}
			if (this._PropertyKeyValidator != null)
			{
				this._PropertyKeyValidator(key);
			}
		}

		protected void ValidateValue(string key, object value)
		{
			if (value != null && !base.IsSupported(value.GetType()))
			{
				throw new ArgumentException("value is of a unsupported Type. The supported types are the primitive ones plus " + string.Join(", ", from type in SchemaDictionary.GetNonPrimitiveSupportedTypes()
				select type.ToString()) + ".", "value");
			}
			if (this._PropertyValueValidator != null)
			{
				this._PropertyValueValidator(key, value);
			}
		}

		internal override void SetSchemaObject(string key, SchemaObject value)
		{
			this.ValidateKey(key);
			this._Properties.AddOrUpdate(key, value, (string existingKey, object existingVale) => value);
		}

		internal override SchemaObject GetSchemaObject(string key)
		{
			this.ValidateKey(key);
			return this._Properties[key] as SchemaObject;
		}

		internal override bool TryGetSchemaObject(string key, out SchemaObject schemaObject)
		{
			schemaObject = null;
			this.ValidateKey(key);
			object obj;
			if (this._Properties.TryGetValue(key, out obj))
			{
				schemaObject = (obj as SchemaObject);
				return true;
			}
			return false;
		}

		internal override IDictionary<string, object> GetDictionaryForSerialization()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (KeyValuePair<string, object> keyValuePair in this._Properties)
			{
				if (!dictionary.ContainsKey(keyValuePair.Key))
				{
					dictionary.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
			foreach (KeyValuePair<string, Func<T, object>> keyValuePair2 in this._accessDelegates._GettersForSerialization)
			{
				if (!dictionary.ContainsKey(keyValuePair2.Key))
				{
					dictionary.Add(keyValuePair2.Key, keyValuePair2.Value(this._owner));
				}
			}
			return dictionary;
		}

		internal bool IsReadOnlyProperty(string key)
		{
			Func<T, object> func;
			Action<T, object> action;
			return this._accessDelegates._Getters.TryGetValue(key, out func) && !this._accessDelegates._Setters.TryGetValue(key, out action);
		}

		internal override void InternalSet(string key, object value)
		{
			this.ValidateKey(key);
			Action<T, object> action;
			if (this._accessDelegates._Setters.TryGetValue(key, out action))
			{
				action(this._owner, value);
				return;
			}
			Func<T, object> func;
			if (this._accessDelegates._Getters.TryGetValue(key, out func))
			{
				throw new FieldAccessException(string.Format(CultureInfo.InvariantCulture, "Property '{0}' has a public getter but not setter", new object[]
				{
					key
				}));
			}
			if (this._PropertyValueSetterTransform != null)
			{
				this._PropertyValueSetterTransform(key, value, true);
				return;
			}
			this._Properties.AddOrUpdate(key, value, (string existingKey, object existingVale) => value);
		}

		internal static void BuildGetterSetterDictionaries(IDictionary<string, SchemaDictionary<T>.AccessDelegates> schemaDictionaryNameToAccessDelegates, IDictionary<string, Func<T, object>> allGetters, IDictionary<string, Action<T, object>> allSetters)
		{
			foreach (PropertyInfo propertyInfo in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				if (!SchemaDictionary<T>.ShouldIgnoreProperty(propertyInfo))
				{
					Func<T, object> func = SchemaDictionary<T>.MakeGetterDelegate(propertyInfo, true);
					if (func != null)
					{
						allGetters.Add(propertyInfo.Name, func);
					}
					Action<T, object> action = SchemaDictionary<T>.MakeSetterDelegate(propertyInfo, true);
					if (action != null)
					{
						allSetters.Add(propertyInfo.Name, action);
					}
					string schemaDictionaryName = SchemaDictionary<T>.GetSchemaDictionaryName(propertyInfo);
					SchemaDictionary<T>.AccessDelegates accessDelegates = null;
					if (!schemaDictionaryNameToAccessDelegates.TryGetValue(schemaDictionaryName, out accessDelegates))
					{
						accessDelegates = new SchemaDictionary<T>.AccessDelegates();
						schemaDictionaryNameToAccessDelegates.Add(schemaDictionaryName, accessDelegates);
					}
					func = SchemaDictionary<T>.MakeGetterDelegate(propertyInfo, false);
					if (func != null)
					{
						accessDelegates._Getters.Add(propertyInfo.Name, func);
					}
					if (SchemaDictionary<T>.ShouldSerialize(propertyInfo))
					{
						accessDelegates._GettersForSerialization.Add(propertyInfo.Name, func);
					}
					action = SchemaDictionary<T>.MakeSetterDelegate(propertyInfo, false);
					if (action != null)
					{
						accessDelegates._Setters.Add(propertyInfo.Name, action);
					}
					MethodInfo setMethod = propertyInfo.GetSetMethod(true);
					if (setMethod != null && (setMethod.IsPrivate || setMethod.IsFamily))
					{
						accessDelegates._ForbiddenSetters.Add(propertyInfo.Name);
					}
				}
			}
			foreach (PropertyInfo propertyInfo2 in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.NonPublic))
			{
				Func<T, object> func2 = SchemaDictionary<T>.MakeGetterDelegate(propertyInfo2, true);
				if (func2 != null)
				{
					allGetters.Add(propertyInfo2.Name, func2);
				}
				Action<T, object> action2 = SchemaDictionary<T>.MakeSetterDelegate(propertyInfo2, true);
				if (action2 != null)
				{
					allSetters.Add(propertyInfo2.Name, action2);
				}
				MethodInfo setMethod2 = propertyInfo2.GetSetMethod(true);
				if (setMethod2 != null)
				{
					foreach (SchemaDictionary<T>.AccessDelegates accessDelegates2 in schemaDictionaryNameToAccessDelegates.Values)
					{
						accessDelegates2._ForbiddenSetters.Add(propertyInfo2.Name);
					}
				}
			}
		}

		private static string GetSchemaDictionaryName(PropertyInfo property)
		{
			return string.Empty;
		}

		private static bool ShouldIgnoreProperty(PropertyInfo property)
		{
			return true;
		}

		private static bool ShouldSerialize(PropertyInfo property)
		{
			return true;
		}

		private static Action<T, object> MakeSetterDelegate(PropertyInfo property, bool includePrivate = false)
		{
			MethodInfo setMethod = property.GetSetMethod(true);
			if (setMethod != null && (includePrivate || !setMethod.IsPrivate) && setMethod.GetParameters().Length == 1)
			{
				ParameterExpression parameterExpression = Expression.Parameter(typeof(T));
				ParameterExpression parameterExpression2 = Expression.Parameter(typeof(object));
				MethodCallExpression body = Expression.Call(parameterExpression, setMethod, new Expression[]
				{
					Expression.Convert(parameterExpression2, property.PropertyType)
				});
				return Expression.Lambda<Action<T, object>>(body, new ParameterExpression[]
				{
					parameterExpression,
					parameterExpression2
				}).Compile();
			}
			return null;
		}

		private static Func<T, object> MakeGetterDelegate(PropertyInfo property, bool includePrivate = false)
		{
			MethodInfo getMethod = property.GetGetMethod(true);
			if (getMethod != null && (includePrivate || !getMethod.IsPrivate) && getMethod.GetParameters().Length == 0)
			{
				ParameterExpression parameterExpression = Expression.Parameter(typeof(T));
				UnaryExpression body = Expression.Convert(Expression.Call(parameterExpression, getMethod), typeof(object));
				return Expression.Lambda<Func<T, object>>(body, new ParameterExpression[]
				{
					parameterExpression
				}).Compile();
			}
			return null;
		}

		private static Dictionary<string, Func<T, object>> _allGetters = new Dictionary<string, Func<T, object>>(StringComparer.OrdinalIgnoreCase);

		private static Dictionary<string, Action<T, object>> _allSetters = new Dictionary<string, Action<T, object>>(StringComparer.OrdinalIgnoreCase);

		protected T _owner;

		private SchemaDictionary<T>.AccessDelegates _accessDelegates;

		private Action<string> _PropertyKeyValidator;

		private Action<string, object> _PropertyValueValidator;

		private Func<string, SchemaDictionary, object> _PropertyValueGetterTransform;

		private Func<string, object, bool, object> _PropertyValueSetterTransform;

		private static Dictionary<string, SchemaDictionary<T>.AccessDelegates> _schemaDictionaryNameToAccessDelegates = new Dictionary<string, SchemaDictionary<T>.AccessDelegates>();

		private static SchemaDictionary<T>.AccessDelegates _emptyAccessDelegates = new SchemaDictionary<T>.AccessDelegates();

		internal class AccessDelegates
		{
			internal IDictionary<string, Action<T, object>> _Setters
			{
				get
				{
					return this._setters;
				}
			}

			internal IDictionary<string, Func<T, object>> _Getters
			{
				get
				{
					return this._getters;
				}
			}

			internal IDictionary<string, Func<T, object>> _GettersForSerialization
			{
				get
				{
					return this._gettersForSerialization;
				}
			}

			internal ICollection<string> _ForbiddenSetters
			{
				get
				{
					return this._forbiddenSetters;
				}
			}

			private Dictionary<string, Action<T, object>> _setters = new Dictionary<string, Action<T, object>>(StringComparer.OrdinalIgnoreCase);

			private Dictionary<string, Func<T, object>> _getters = new Dictionary<string, Func<T, object>>(StringComparer.OrdinalIgnoreCase);

			private Dictionary<string, Func<T, object>> _gettersForSerialization = new Dictionary<string, Func<T, object>>(StringComparer.OrdinalIgnoreCase);

			private HashSet<string> _forbiddenSetters = new HashSet<string>();
		}
	}
}
