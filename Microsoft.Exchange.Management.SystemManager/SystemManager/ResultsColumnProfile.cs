using System;
using System.Collections;
using Microsoft.Exchange.Management.SystemManager.WinForms;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class ResultsColumnProfile
	{
		public ResultsColumnProfile(string name, bool isDefault, string text)
		{
			this.name = name;
			this.isDefault = isDefault;
			this.text = text;
			this.SortMode = SortMode.NotSpecified;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			internal set
			{
				this.name = value;
			}
		}

		public bool IsDefault
		{
			get
			{
				return this.isDefault;
			}
		}

		public string Text
		{
			get
			{
				return this.text;
			}
		}

		public SortMode SortMode { get; set; }

		public IComparer CustomComparer { get; set; }

		public ICustomFormatter CustomFormatter { get; set; }

		public IFormatProvider FormatProvider { get; set; }

		public string FormatString { get; set; }

		public string DefaultEmptyText { get; set; }

		public IToColorFormatter ColorFormatter { get; set; }

		private string name;

		private bool isDefault;

		private string text;
	}
}
