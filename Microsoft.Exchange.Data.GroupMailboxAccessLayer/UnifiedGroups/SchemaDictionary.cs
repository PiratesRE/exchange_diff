using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.UnifiedGroups
{
	[KnownType("GetExplicitKnownTypes")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Office.Server.Directory")]
	public abstract class SchemaDictionary
	{
		public abstract object this[string key]
		{
			get;
			set;
		}

		public abstract bool ContainsKey(string key);

		public abstract bool TryGetValue(string key, out object value);

		public virtual bool TryGetValue<T>(string key, out T value)
		{
			bool result = false;
			object value2 = null;
			value = default(T);
			if (!this.TryGetValue(key, out value2))
			{
				result = false;
			}
			else
			{
				try
				{
					value = (T)((object)Convert.ChangeType(value2, typeof(T), CultureInfo.CurrentCulture));
					result = true;
				}
				catch (Exception)
				{
					result = false;
				}
			}
			return result;
		}

		public abstract IEnumerator<KeyValuePair<string, object>> GetEnumerator();

		public bool IsSupported(Type type)
		{
			return !(type == null) && (type.IsPrimitive || type.IsEnum || SchemaDictionary.WcfDefaultKnownTypes.Contains(type) || SchemaDictionary.ExplicitKnownTypes.Contains(type));
		}

		internal abstract void SetSchemaObject(string key, SchemaObject value);

		internal abstract SchemaObject GetSchemaObject(string key);

		internal abstract bool TryGetSchemaObject(string key, out SchemaObject schemaObject);

		internal abstract IDictionary<string, object> GetDictionaryForSerialization();

		internal abstract void InternalSet(string key, object value);

		internal static List<Type> GetExplicitKnownTypes()
		{
			return SchemaDictionary.ExplicitKnownTypes;
		}

		internal static IEnumerable<Type> GetNonPrimitiveSupportedTypes()
		{
			return SchemaDictionary.WcfDefaultKnownTypes.Concat(SchemaDictionary.ExplicitKnownTypes);
		}

		[DataMember]
		internal ConcurrentDictionary<string, object> InternalStorage
		{
			get
			{
				return this._Properties;
			}
			set
			{
				this._Properties = value;
			}
		}

		private static readonly List<Type> WcfDefaultKnownTypes = new List<Type>
		{
			typeof(DateTime),
			typeof(string),
			typeof(Guid)
		};

		private static readonly List<Type> ExplicitKnownTypes = new List<Type>
		{
			typeof(List<string>),
			typeof(StringCollection),
			typeof(List<Guid>),
			typeof(SchemaDictionary<Relation>),
			typeof(Relation)
		};

		protected ConcurrentDictionary<string, object> _Properties = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);
	}
}
