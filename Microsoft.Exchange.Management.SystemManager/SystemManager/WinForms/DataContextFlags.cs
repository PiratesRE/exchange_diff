using System;
using System.ComponentModel;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public class DataContextFlags
	{
		public DataContextFlags()
		{
			this.pages = new ExchangePageCollection(this);
		}

		[DefaultValue(true)]
		public bool NeedToShowVersionWarning
		{
			get
			{
				return this.needToShowVersionWarning;
			}
			set
			{
				this.needToShowVersionWarning = value;
			}
		}

		public ExchangePageCollection Pages
		{
			get
			{
				return this.pages;
			}
		}

		public int SelectedObjectsCount { get; set; }

		public string SelectedObjectDetailsType { get; set; }

		private bool needToShowVersionWarning = true;

		private ExchangePageCollection pages;
	}
}
