using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal class SessionParameters
	{
		public int Count
		{
			get
			{
				return this.dictionary.Count<KeyValuePair<string, SessionParameters.SessionParameter>>();
			}
		}

		public IEnumerable<string> LoggingText
		{
			get
			{
				return from kvp in this.dictionary
				select kvp.Value.LoggingText;
			}
		}

		public IDictionary ToDictionary()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (SessionParameters.SessionParameter sessionParameter in this.dictionary.Values)
			{
				dictionary[sessionParameter.Name] = sessionParameter.Value;
			}
			return dictionary;
		}

		public void Set(string name, string value)
		{
			this.SetValue<string>(name, value);
		}

		public void Set(string name, bool value)
		{
			this.SetValue<bool>(name, value);
		}

		public void Set(string name, Guid value)
		{
			this.SetValue<Guid>(name, value);
		}

		public void Set(string name, Uri value)
		{
			this.SetValue<Uri>(name, value);
		}

		public void Set(string name, Enum value)
		{
			this.SetValue<Enum>(name, value);
		}

		public void SetNull<T>(string name)
		{
			this.SetValue<T>(name, default(T));
		}

		public void Set<T>(string name, IEnumerable<T> list, Func<T, string> projection)
		{
			if (list != null && list.Count<T>() > 0)
			{
				this.SetValue<string[]>(name, list.Select(projection).ToArray<string>());
			}
		}

		public void Set<T>(string name, IEnumerable<T> list)
		{
			this.Set<T>(name, list, (T x) => x.ToString());
		}

		public override string ToString()
		{
			return string.Join(" ", this.LoggingText);
		}

		private void SetValue<T>(string name, T value)
		{
			this.dictionary[name] = new SessionParameters.SessionParameter(name, typeof(T), value);
		}

		private Dictionary<string, SessionParameters.SessionParameter> dictionary = new Dictionary<string, SessionParameters.SessionParameter>();

		private class SessionParameter
		{
			public SessionParameter(string name, Type type, object value)
			{
				this.Name = name;
				this.Type = type;
				this.Value = value;
			}

			public string Name { get; private set; }

			public object Value { get; private set; }

			public Type Type { get; private set; }

			public string LoggingText
			{
				get
				{
					if (this.Type == typeof(bool))
					{
						return string.Format("-{0}: {1}", this.Name, this.ValueLoggingText);
					}
					return string.Format("-{0} {1}", this.Name, this.ValueLoggingText);
				}
			}

			private string ValueLoggingText
			{
				get
				{
					if (this.Type == typeof(bool))
					{
						if (!(bool)this.Value)
						{
							return "$false";
						}
						return "$true";
					}
					else
					{
						if (this.Value == null)
						{
							return "$null";
						}
						if (this.Type.IsArray)
						{
							return "{" + string.Join<object>(",", this.Value as IEnumerable<object>) + "}";
						}
						return string.Format("'{0}'", this.Value);
					}
				}
			}

			public override string ToString()
			{
				return this.LoggingText;
			}
		}
	}
}
