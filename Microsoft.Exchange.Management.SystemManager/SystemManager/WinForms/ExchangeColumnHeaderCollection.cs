using System;
using System.Globalization;
using Microsoft.ManagementGUI;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class ExchangeColumnHeaderCollection : ChangeNotifyingCollection<ExchangeColumnHeader>
	{
		public ExchangeColumnHeader Add(string name, string text)
		{
			return this.Add(name, text, -2);
		}

		public ExchangeColumnHeader Add(string name, string text, int width)
		{
			return this.Add(name, text, width, false);
		}

		public ExchangeColumnHeader Add(string name, string text, int width, bool isDefault)
		{
			return this.Add(name, text, width, isDefault, string.Empty);
		}

		public ExchangeColumnHeader Add(string name, string text, bool isDefault)
		{
			return this.Add(name, text, -2, isDefault, string.Empty);
		}

		public ExchangeColumnHeader Add(string name, string text, int width, bool isDefault, string defaultEmptyText)
		{
			return this.Add(name, text, width, isDefault, defaultEmptyText, null, null, null);
		}

		public ExchangeColumnHeader Add(string name, string text, bool isDefault, string defaultEmptyText)
		{
			return this.Add(name, text, -2, isDefault, defaultEmptyText, null, null, null);
		}

		public ExchangeColumnHeader Add(string name, string text, bool isDefault, string defaultEmptyText, ICustomFormatter customFormatter, string formatString, IFormatProvider formatProvider)
		{
			ExchangeColumnHeader exchangeColumnHeader = new ExchangeColumnHeader(name, text, -2, isDefault, defaultEmptyText, customFormatter, formatString, formatProvider);
			base.Add(exchangeColumnHeader);
			return exchangeColumnHeader;
		}

		public ExchangeColumnHeader Add(string name, string text, int width, bool isDefault, string defaultEmptyText, ICustomFormatter customFormatter, string formatString, IFormatProvider formatProvider)
		{
			ExchangeColumnHeader exchangeColumnHeader = new ExchangeColumnHeader(name, text, width, isDefault, defaultEmptyText, customFormatter, formatString, formatProvider);
			base.Add(exchangeColumnHeader);
			return exchangeColumnHeader;
		}

		public void AddRange(ExchangeColumnHeader[] columns)
		{
			foreach (ExchangeColumnHeader item in columns)
			{
				base.Add(item);
			}
		}

		public ExchangeColumnHeader this[string name]
		{
			get
			{
				ExchangeColumnHeader result = null;
				if (!string.IsNullOrEmpty(name))
				{
					foreach (ExchangeColumnHeader exchangeColumnHeader in this)
					{
						if (string.Compare(name, exchangeColumnHeader.Name, false, CultureInfo.InvariantCulture) == 0)
						{
							result = exchangeColumnHeader;
							break;
						}
					}
				}
				return result;
			}
		}
	}
}
