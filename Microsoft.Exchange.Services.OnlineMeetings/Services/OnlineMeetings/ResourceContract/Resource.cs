using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	internal class Resource : IResource
	{
		public Resource(string selfUri)
		{
			if (!string.IsNullOrEmpty(selfUri))
			{
				this.links.Add(new Link("self", selfUri, "self"));
			}
		}

		public ICollection<Link> Links
		{
			get
			{
				return new LinksCollection(this.links);
			}
		}

		public string SelfUri
		{
			get
			{
				Link link2 = (from link in this.links
				where link.Relationship == "self"
				select link).FirstOrDefault<Link>();
				if (link2 == null)
				{
					return null;
				}
				return link2.Href;
			}
			set
			{
				this.links.RemoveAll((Link link) => link.Token == "self");
				this.links.Add(new Link("ignore-this-token", value, "self"));
			}
		}

		public Dictionary<string, object> MultipartAttachments
		{
			get
			{
				return this.multipartAttachments;
			}
			set
			{
				this.multipartAttachments = value;
			}
		}

		public virtual bool CanBeEmbedded
		{
			get
			{
				return true;
			}
		}

		protected ICollection<string> Keys
		{
			get
			{
				return this.properties.Keys;
			}
		}

		public static Resource FromDictionary(Type resourceType, Dictionary<string, object> dictionary)
		{
			dictionary = new Dictionary<string, object>(dictionary, StringComparer.OrdinalIgnoreCase);
			Resource resource = Activator.CreateInstance(resourceType, new object[]
			{
				string.Empty
			}) as Resource;
			if (resource != null)
			{
				resource.properties = dictionary;
				if (dictionary.ContainsKey("_links"))
				{
					IDictionary dictionary2 = dictionary["_links"] as IDictionary;
					if (dictionary2 != null)
					{
						foreach (object obj in dictionary2.Keys)
						{
							string text = (string)obj;
							IEnumerable enumerable = dictionary2[text] as IEnumerable;
							IDictionary dictionary3 = enumerable as IDictionary;
							if (dictionary3 != null)
							{
								enumerable = new IDictionary[]
								{
									dictionary3
								};
							}
							foreach (object obj2 in enumerable)
							{
								IDictionary dictionary4 = (IDictionary)obj2;
								string href = dictionary4["href"] as string;
								resource.links.Add(new Link(text, href, text));
							}
						}
					}
				}
				if (dictionary.ContainsKey("_embedded"))
				{
					dictionary["_embedded"] = Resource.ConvertDictionaryToCaseInsensitive(dictionary["_embedded"]);
				}
				resource.PopulateDataFromDictionary();
			}
			return resource;
		}

		public void AddLink(string relationship, string uri, object target = null)
		{
			this.links.Add(new Link(relationship, uri, relationship)
			{
				Target = target
			});
		}

		public virtual object ToDictionary(List<EmbeddedPart> mimeParts)
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
			foreach (IGrouping<string, Link> grouping in from link in this.links
			group link by link.Token)
			{
				if (grouping.All((Link link) => link.CanBeEmbedded))
				{
					object value = Resource.UnwrapIfOneItem(grouping.Select((Link link) => ((Resource)link.Target).ToDictionary(mimeParts)).ToArray<object>());
					dictionary3.Add(grouping.Key, value);
				}
				else
				{
					foreach (Link link2 in grouping)
					{
						if (link2.Target is ExternalResource && mimeParts != null)
						{
							ExternalResource externalResource = (ExternalResource)link2.Target;
							string text = externalResource.ContentId ?? Guid.NewGuid().ToString();
							mimeParts.Add(new EmbeddedPart
							{
								Content = externalResource.Value,
								ContentId = text
							});
							link2.Href = string.Format("{0}{1}", "cid:", text);
						}
					}
					object value2 = Resource.UnwrapIfOneItem(grouping.Select((Link link) => new Dictionary<string, string>
					{
						{
							"href",
							link.Href
						}
					}).ToArray<Dictionary<string, string>>());
					dictionary2.Add(grouping.Key, value2);
				}
			}
			foreach (KeyValuePair<string, object> keyValuePair in this.properties)
			{
				dictionary[keyValuePair.Key] = keyValuePair.Value;
			}
			if (dictionary2 != null)
			{
				dictionary["_links"] = dictionary2;
			}
			if (dictionary3.Count > 0)
			{
				dictionary["_embedded"] = dictionary3;
			}
			else
			{
				dictionary.Remove("_embedded");
			}
			return dictionary;
		}

		public virtual void PopulateDataFromDictionary()
		{
		}

		protected T GetValue<T>(string name)
		{
			object value = this.GetValue(typeof(T), name);
			if (value == null)
			{
				return default(T);
			}
			return (T)((object)value);
		}

		protected void SetValue<T>(string name, object value)
		{
			Type typeFromHandle = typeof(T);
			this.SetValue(name, value, typeFromHandle);
		}

		private static object ConvertDictionaryToCaseInsensitive(object dict)
		{
			IDictionary dictionary = dict as IDictionary;
			if (dictionary != null)
			{
				Dictionary<string, object> dictionary2 = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
				foreach (object obj in dictionary.Keys)
				{
					string key = (string)obj;
					dictionary2[key] = dictionary[key];
				}
				return dictionary2;
			}
			return dict;
		}

		private static object ConvertTo(Type type, object value)
		{
			object result = null;
			if (value == null)
			{
				return null;
			}
			if (type.GetTypeInfo().IsAssignableFrom(value.GetType().GetTypeInfo()))
			{
				return value;
			}
			string text = value.ToString();
			if (type == typeof(DateTime))
			{
				if (text != null)
				{
					result = DateTime.Parse(text);
				}
			}
			else if (type == typeof(TimeSpan))
			{
				if (text != null)
				{
					result = TimeSpan.Parse(text);
				}
			}
			else if (type.GetTypeInfo().IsEnum)
			{
				if (text != null)
				{
					result = Enum.Parse(type, text);
				}
			}
			else if (type == typeof(bool))
			{
				if (text != null)
				{
					result = bool.Parse(text);
				}
			}
			else if (type == typeof(int))
			{
				if (text != null)
				{
					result = int.Parse(text);
				}
			}
			else
			{
				result = value;
			}
			return result;
		}

		private static Resource ResourceCollectionFromDictionary(Type collectionType, IEnumerable collectionElements)
		{
			Type type = collectionType;
			while (type != null && type.GetTypeInfo().BaseType != typeof(Resource))
			{
				type = type.GetTypeInfo().BaseType;
			}
			Type typeArg = type.GetGenericArguments().First<Type>();
			IResourceCollection resourceCollection = (IResourceCollection)Activator.CreateInstance(collectionType);
			Resource[] array = (from dict in collectionElements.OfType<Dictionary<string, object>>()
			select Resource.FromDictionary(typeArg, dict)).ToArray<Resource>();
			foreach (Resource target in array)
			{
				resourceCollection.AddItem(target);
			}
			return resourceCollection as Resource;
		}

		private static object UnwrapIfOneItem(object value)
		{
			if (value is IEnumerable && !(value is string))
			{
				IEnumerable<object> source = ((IEnumerable)value).Cast<object>();
				if (source.Any<object>() && !source.Skip(1).Any<object>())
				{
					return source.First<object>();
				}
			}
			return value;
		}

		private object GetValue(Type t, string name)
		{
			object obj2 = null;
			if (t == typeof(string))
			{
				return this.GetRawValue<string>(name);
			}
			if (t == typeof(int))
			{
				obj2 = this.GetRawValue<int>(name);
			}
			else if (t == typeof(bool))
			{
				obj2 = this.GetRawValue<bool>(name);
			}
			else if (t == typeof(DateTime))
			{
				obj2 = this.GetRawValue<DateTime>(name);
			}
			else if (t == typeof(TimeSpan))
			{
				string rawValue = this.GetRawValue<string>(name);
				if (rawValue != null)
				{
					obj2 = TimeSpan.Parse(rawValue);
				}
			}
			else if (t.GetTypeInfo().IsEnum)
			{
				string rawValue2 = this.GetRawValue<string>(name);
				if (rawValue2 != null)
				{
					obj2 = Enum.Parse(t, this.GetRawValue<string>(name), true);
				}
			}
			else if (t.GetTypeInfo().IsGenericType && t.GetTypeInfo().GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				if (!this.IsValueSet(name))
				{
					obj2 = null;
				}
				else
				{
					Type t2 = t.GetGenericArguments().First<Type>();
					obj2 = this.GetValue(t2, name);
				}
			}
			else if (typeof(IResource).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo()))
			{
				obj2 = this.GetResource(name, t);
			}
			else if (t.IsArray)
			{
				IEnumerable rawValue3 = this.GetRawValue<IEnumerable>(name);
				if (rawValue3 != null)
				{
					Type elementType = t.GetElementType();
					object[] array = (from obj in rawValue3.OfType<object>()
					select Resource.ConvertTo(elementType, obj)).ToArray<object>();
					Array array2 = Array.CreateInstance(elementType, array.Length);
					array.CopyTo(array2, 0);
					obj2 = array2;
				}
			}
			if (obj2 != null)
			{
				return obj2;
			}
			return null;
		}

		private void SetValue(string name, object value, Type t)
		{
			if (value == null)
			{
				this.SetRawValue(name, null);
				return;
			}
			if (t == typeof(string) || t.GetTypeInfo().IsPrimitive || t == typeof(DateTime) || t == typeof(bool))
			{
				this.SetRawValue(name, value);
				return;
			}
			if (typeof(IResource).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo()))
			{
				this.SetResource(name, value);
				return;
			}
			if (t.IsArray)
			{
				Type elementType = t.GetElementType();
				if (elementType.GetTypeInfo().IsEnum)
				{
					string[] value2 = (from e in ((Array)value).OfType<Enum>()
					select e.ToString()).ToArray<string>();
					this.SetRawValue(name, value2);
					return;
				}
				IEnumerable enumerable = value as IEnumerable;
				List<string> list = new List<string>();
				foreach (object obj in enumerable)
				{
					list.Add(obj.ToString());
				}
				this.SetRawValue(name, list.ToArray());
				return;
			}
			else
			{
				if (t.GetTypeInfo().IsGenericType && t.GetTypeInfo().GetGenericTypeDefinition() == typeof(Nullable<>))
				{
					Type t2 = t.GetGenericArguments().First<Type>();
					this.SetValue(name, value, t2);
					return;
				}
				this.SetRawValue(name, value.ToString());
				return;
			}
		}

		private bool IsValueSet(string name)
		{
			return this.properties.ContainsKey(name) && this.properties[name] != null;
		}

		private T GetRawValue<T>(string name)
		{
			object obj = null;
			if (!this.properties.TryGetValue(name, out obj))
			{
				return default(T);
			}
			if (obj != null && !(obj is T))
			{
				if (typeof(T) == typeof(int) && obj is string)
				{
					obj = int.Parse((string)obj);
				}
				else if (typeof(T) == typeof(bool) && obj is string)
				{
					obj = bool.Parse((string)obj);
				}
				else
				{
					if (!(typeof(T) == typeof(DateTime)) || !(obj is string))
					{
						throw new ArgumentException("The property value is not valid for the specified type");
					}
					obj = DateTime.Parse((string)obj);
				}
			}
			return (T)((object)obj);
		}

		private void SetRawValue(string name, object value)
		{
			if (value != null)
			{
				this.properties[name] = value;
				return;
			}
			this.properties.Remove(name);
		}

		private void SetResource(string name, object value)
		{
			this.links.RemoveAll((Link link) => link.Relationship == name);
			if (value != null)
			{
				this.AddLink(name, ((IResource)value).SelfUri, value);
			}
		}

		private object GetResource(string name, Type resourceType)
		{
			Link link4 = (from link in this.Links
			where link.Relationship == name
			select link).FirstOrDefault<Link>();
			if (link4 == null && this.properties.ContainsKey("_embedded"))
			{
				IDictionary dictionary = this.properties["_embedded"] as IDictionary;
				if (dictionary != null && dictionary.Contains(name))
				{
					Resource resource;
					if (typeof(IResourceCollection).GetTypeInfo().IsAssignableFrom(resourceType.GetTypeInfo()))
					{
						resource = Resource.ResourceCollectionFromDictionary(resourceType, dictionary[name] as IEnumerable);
					}
					else
					{
						resource = Resource.FromDictionary(resourceType, dictionary[name] as Dictionary<string, object>);
					}
					Link link2 = new Link(name, resource.SelfUri, name)
					{
						Target = resource
					};
					this.Links.Add(link2);
					link4 = link2;
				}
			}
			if (link4 == null && typeof(ExternalResource).GetTypeInfo().IsAssignableFrom(resourceType.GetTypeInfo()))
			{
				string rawValue = this.GetRawValue<string>(name);
				if (rawValue != null && rawValue.StartsWith("cid:", StringComparison.OrdinalIgnoreCase))
				{
					string key = rawValue.Substring(4);
					if (this.multipartAttachments != null && this.multipartAttachments.ContainsKey(key))
					{
						object value = this.multipartAttachments[key];
						Link link3 = new Link(name, rawValue, name)
						{
							Target = new ExternalResource
							{
								Value = value
							}
						};
						this.Links.Add(link3);
						link4 = link3;
					}
				}
			}
			if (link4 == null)
			{
				return null;
			}
			IResource resource2 = link4.Target as IResource;
			if (resource2 == null)
			{
				return null;
			}
			return resource2;
		}

		private readonly List<Link> links = new List<Link>();

		private Dictionary<string, object> properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

		private Dictionary<string, object> multipartAttachments;

		public static class StringConstants
		{
			public const string Self = "self";

			public const string Links = "_links";

			public const string Embedded = "_embedded";

			public const string CidPrefix = "cid:";

			public const string Href = "href";

			public const string Rel = "rel";

			public const string Title = "title";

			public const string OperationId = "operationId";
		}
	}
}
