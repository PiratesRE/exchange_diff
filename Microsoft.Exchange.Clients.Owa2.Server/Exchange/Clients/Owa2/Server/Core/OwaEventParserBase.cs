using System;
using System.Collections;
using System.Globalization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal abstract class OwaEventParserBase
	{
		internal OwaEventParserBase(OwaEventHandlerBase eventHandler, int parameterTableCapacity)
		{
			this.eventHandler = eventHandler;
			this.parameterTable = new Hashtable(parameterTableCapacity);
		}

		protected Hashtable ParameterTable
		{
			get
			{
				return this.parameterTable;
			}
		}

		protected ulong SetMask
		{
			get
			{
				return this.setMask;
			}
		}

		protected OwaEventHandlerBase EventHandler
		{
			get
			{
				return this.eventHandler;
			}
		}

		internal Hashtable Parse()
		{
			Hashtable result = this.ParseParameters();
			if ((this.EventHandler.EventInfo.RequiredMask & this.SetMask) != this.EventHandler.EventInfo.RequiredMask)
			{
				this.ThrowParserException("A required parameter of the event wasn't set");
			}
			return result;
		}

		protected abstract Hashtable ParseParameters();

		protected abstract void ThrowParserException(string description);

		protected void ThrowParserException()
		{
			this.ThrowParserException(null);
		}

		protected OwaEventParameterAttribute GetParamInfo(string name)
		{
			OwaEventParameterAttribute owaEventParameterAttribute = this.EventHandler.EventInfo.FindParameterInfo(name);
			if (owaEventParameterAttribute == null)
			{
				this.ThrowParserException(string.Format(CultureInfo.InvariantCulture, "Parameter '{0}' is unknown.", new object[]
				{
					name
				}));
			}
			return owaEventParameterAttribute;
		}

		protected void AddParameter(OwaEventParameterAttribute paramInfo, object value)
		{
			if (this.parameterTable.Count >= 64)
			{
				this.ThrowParserException("Reached maximum number of parameters");
			}
			if ((this.setMask & paramInfo.ParameterMask) != 0UL)
			{
				this.ThrowParserException(string.Format(CultureInfo.InvariantCulture, "Parameter '{0}' is found twice", new object[]
				{
					paramInfo.Name
				}));
			}
			this.setMask |= paramInfo.ParameterMask;
			this.parameterTable.Add(paramInfo.Name, value);
		}

		protected void AddSimpleTypeParameter(OwaEventParameterAttribute paramInfo, string value)
		{
			object obj = null;
			if (paramInfo.Type == typeof(string))
			{
				obj = value;
			}
			else if (paramInfo.Type == typeof(int))
			{
				obj = int.Parse(value, CultureInfo.InvariantCulture);
			}
			else if (paramInfo.Type == typeof(double))
			{
				obj = double.Parse(value, CultureInfo.InvariantCulture);
			}
			else if (paramInfo.Type == typeof(long))
			{
				obj = long.Parse(value, CultureInfo.InvariantCulture);
			}
			else if (paramInfo.Type == typeof(bool))
			{
				if (string.Equals(value, "0", StringComparison.Ordinal))
				{
					obj = false;
				}
				else if (string.Equals(value, "1", StringComparison.Ordinal))
				{
					obj = true;
				}
				else
				{
					this.ThrowParserException(string.Format(CultureInfo.InvariantCulture, "Failed to parse type. Type = {0}, Value = {1}", new object[]
					{
						paramInfo.Type,
						value
					}));
				}
			}
			if (obj != null)
			{
				this.AddParameter(paramInfo, obj);
			}
		}

		protected void AddArrayParameter(OwaEventParameterAttribute paramInfo, ArrayList itemArray)
		{
			Array value = itemArray.ToArray(paramInfo.Type);
			this.AddParameter(paramInfo, value);
		}

		protected void AddEmptyParameter(OwaEventParameterAttribute paramInfo)
		{
			if (paramInfo.IsArray)
			{
				this.AddArrayParameter(paramInfo, new ArrayList());
				return;
			}
			this.AddSimpleTypeParameter(paramInfo, string.Empty);
		}

		protected void AddSimpleTypeToArray(OwaEventParameterAttribute paramInfo, ArrayList itemArray, string value)
		{
			if (paramInfo.Type == typeof(string))
			{
				this.AddItemToArray(itemArray, value);
			}
		}

		protected void AddEmptyItemToArray(OwaEventParameterAttribute paramInfo, ArrayList itemArray)
		{
			this.AddSimpleTypeParameter(paramInfo, string.Empty);
		}

		private void AddItemToArray(ArrayList itemArray, object value)
		{
			if (itemArray.Count >= 2000)
			{
				this.ThrowParserException("Reached maximum number of items in an array");
			}
			itemArray.Add(value);
		}

		internal const int MaxStructFieldCount = 32;

		internal const int MaxParameterCount = 64;

		internal const int MaxParameterCountGet = 16;

		internal const int MaxArrayItemCount = 2000;

		protected const string BooleanTrue = "1";

		protected const string BooleanFalse = "0";

		private OwaEventHandlerBase eventHandler;

		private Hashtable parameterTable;

		private ulong setMask;
	}
}
