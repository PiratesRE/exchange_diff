using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.ABProviderFramework
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ABPropertyDefinitionCollection : IEnumerable<ABPropertyDefinition>, IEnumerable
	{
		public ABPropertyDefinitionCollection(ICollection<ABPropertyDefinition> properties)
		{
			if (properties == null || properties.Count == 0)
			{
				throw new ArgumentNullException("propertyDefinitionCollection");
			}
			using (IEnumerator<ABPropertyDefinition> enumerator = properties.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == null)
					{
						throw new ArgumentException("propertyDefinitionCollection contains null property.", "propertyDefinitionCollection");
					}
				}
			}
			this.propertyDefinitionCollection = properties;
		}

		public static ABPropertyDefinitionCollection FromPropertyDefinitionCollection(ICollection<PropertyDefinition> properties)
		{
			if (properties == null || properties.Count == 0)
			{
				throw new ArgumentNullException("properties");
			}
			ICollection<ABPropertyDefinition> collection = new List<ABPropertyDefinition>(properties.Count);
			foreach (PropertyDefinition propertyDefinition in properties)
			{
				ABPropertyDefinition abpropertyDefinition = (ABPropertyDefinition)propertyDefinition;
				if (abpropertyDefinition == null)
				{
					throw new ArgumentException("propertyDefinitionCollection contains null property.", "propertyDefinitionCollection");
				}
				collection.Add(abpropertyDefinition);
			}
			return new ABPropertyDefinitionCollection(collection);
		}

		public int Count
		{
			get
			{
				return this.propertyDefinitionCollection.Count;
			}
		}

		public bool Contains(ABPropertyDefinition propertyDefinition)
		{
			if (propertyDefinition == null)
			{
				throw new ArgumentNullException("propertyDefinition");
			}
			if (this.propertyNames == null)
			{
				this.BuildPropertyNamesHashSet();
			}
			return this.propertyNames.Contains(propertyDefinition.Name);
		}

		public object GetNativePropertyCollection(string providerToken)
		{
			if (string.IsNullOrEmpty(providerToken))
			{
				throw new ArgumentNullException("providerToken");
			}
			Dictionary<string, object> dictionary = (Dictionary<string, object>)Thread.VolatileRead(ref this.cache);
			object result;
			if (!dictionary.TryGetValue(providerToken, out result))
			{
				return null;
			}
			return result;
		}

		public void SetNativePropertyCollection(string providerToken, object nativeCollection)
		{
			if (string.IsNullOrEmpty(providerToken))
			{
				throw new ArgumentNullException("providerToken");
			}
			lock (this.updatingCacheLock)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>((Dictionary<string, object>)this.cache, StringComparer.OrdinalIgnoreCase);
				dictionary[providerToken] = nativeCollection;
				this.cache = dictionary;
			}
		}

		public override string ToString()
		{
			bool flag = true;
			StringBuilder stringBuilder = new StringBuilder(20 * this.propertyDefinitionCollection.Count);
			stringBuilder.Append("[");
			foreach (ABPropertyDefinition abpropertyDefinition in this.propertyDefinitionCollection)
			{
				if (flag)
				{
					flag = false;
				}
				else
				{
					stringBuilder.Append(", ");
				}
				stringBuilder.Append(abpropertyDefinition.Name ?? "<null>");
			}
			stringBuilder.Append("]");
			return stringBuilder.ToString();
		}

		public IEnumerator<ABPropertyDefinition> GetEnumerator()
		{
			foreach (ABPropertyDefinition propertyDefinition in this.propertyDefinitionCollection)
			{
				yield return propertyDefinition;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			foreach (ABPropertyDefinition propertyDefinition in this.propertyDefinitionCollection)
			{
				yield return propertyDefinition;
			}
			yield break;
		}

		private void BuildPropertyNamesHashSet()
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (ABPropertyDefinition abpropertyDefinition in this.propertyDefinitionCollection)
			{
				hashSet.Add(abpropertyDefinition.Name);
			}
			this.propertyNames = hashSet;
		}

		private static Dictionary<string, object> emptyCache = new Dictionary<string, object>(0, StringComparer.OrdinalIgnoreCase);

		private ICollection<ABPropertyDefinition> propertyDefinitionCollection;

		private HashSet<string> propertyNames;

		private object cache = ABPropertyDefinitionCollection.emptyCache;

		private object updatingCacheLock = new object();
	}
}
