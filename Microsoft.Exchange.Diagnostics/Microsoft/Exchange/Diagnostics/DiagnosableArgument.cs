using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;

namespace Microsoft.Exchange.Diagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class DiagnosableArgument
	{
		public DiagnosableArgument()
		{
			Dictionary<string, Type> dictionary = new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);
			this.InitializeSchema(dictionary);
			this.ArgumentSchema = dictionary;
		}

		public int ArgumentCount
		{
			get
			{
				return this.Arguments.Count;
			}
		}

		protected Dictionary<string, object> Arguments { get; set; }

		protected virtual bool FailOnMissingArgument
		{
			get
			{
				return false;
			}
		}

		private protected Dictionary<string, Type> ArgumentSchema { protected get; private set; }

		public void Initialize(DiagnosableParameters parameters)
		{
			this.Initialize(parameters.Argument);
		}

		public void Initialize(string argument)
		{
			this.Arguments = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
			if (argument == null)
			{
				return;
			}
			string[] array = argument.Trim().Split(new string[]
			{
				","
			}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text in array)
			{
				string[] array3 = text.Split(new string[]
				{
					"="
				}, StringSplitOptions.RemoveEmptyEntries);
				if (array3.Length > 0)
				{
					string text2 = array3[0].Trim();
					if (this.HasArgument(text2))
					{
						throw new ArgumentDuplicatedException(text2);
					}
					this.AddArgument(text2, (array3.Length > 1) ? array3[1].Trim() : null);
				}
			}
		}

		public bool TryGetArgument<T>(string argumentToGet, out T val)
		{
			object obj;
			if (!this.HasArgument(argumentToGet) || (obj = this.Arguments[argumentToGet]) == null)
			{
				val = default(T);
				return false;
			}
			if (!(obj is T))
			{
				throw new ArgumentValueCannotBeParsedException(argumentToGet, obj.ToString(), typeof(T).Name);
			}
			val = (T)((object)obj);
			return true;
		}

		public T GetArgument<T>(string argumentToGet)
		{
			T result;
			this.TryGetArgument<T>(argumentToGet, out result);
			return result;
		}

		public T GetArgumentOrDefault<T>(string argumentToGet, T defaultValue)
		{
			T result;
			if (this.TryGetArgument<T>(argumentToGet, out result))
			{
				return result;
			}
			return defaultValue;
		}

		public bool HasArgument(string argumentToCheck)
		{
			return this.Arguments.ContainsKey(argumentToCheck);
		}

		public virtual XElement RunDiagnosticOperation(Func<XElement> operation)
		{
			XElement result;
			try
			{
				result = operation();
			}
			catch (DiagnosticArgumentException ex)
			{
				result = new XElement("Error", "Encountered exception: " + ex.Message);
			}
			return result;
		}

		public string GetSupportedArguments()
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, Type> keyValuePair in this.ArgumentSchema)
			{
				if (keyValuePair.Value == typeof(bool))
				{
					list.Add(keyValuePair.Key);
				}
				else
				{
					list.Add(string.Format("{0}={1}", keyValuePair.Key, keyValuePair.Value.Name));
				}
			}
			return string.Join(", ", list);
		}

		protected abstract void InitializeSchema(Dictionary<string, Type> schema);

		protected virtual void AddArgument(string key, string value)
		{
			Type type;
			if (!this.ArgumentSchema.TryGetValue(key, out type))
			{
				if (this.FailOnMissingArgument)
				{
					throw new ArgumentNotSupportedException(key, this.GetSupportedArguments());
				}
				return;
			}
			else
			{
				if (value == null || type == typeof(bool))
				{
					this.Arguments.Add(key, null);
					return;
				}
				if (type == typeof(string))
				{
					this.Arguments.Add(key, value);
					return;
				}
				TypeConverter converter = TypeDescriptor.GetConverter(type);
				if (converter == null)
				{
					throw new ArgumentValueCannotBeParsedException(key, value, type.Name);
				}
				try
				{
					this.Arguments.Add(key, converter.ConvertFromInvariantString(value));
				}
				catch (FormatException)
				{
					throw new ArgumentValueCannotBeParsedException(key, value, type.Name);
				}
				catch (NotSupportedException)
				{
					throw new ArgumentValueCannotBeParsedException(key, value, type.Name);
				}
				return;
			}
		}
	}
}
