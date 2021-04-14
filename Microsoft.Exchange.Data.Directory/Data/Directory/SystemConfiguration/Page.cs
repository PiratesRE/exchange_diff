using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public sealed class Page : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private void NotifyPropertyChanged(string info)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}

		public string Text
		{
			get
			{
				return this.text;
			}
			set
			{
				if (value != this.text)
				{
					DetailsTemplateControl.ValidateText(value, DetailsTemplateControl.TextLengths.Page);
					this.text = value;
					this.NotifyPropertyChanged("Text");
				}
			}
		}

		public int HelpContext
		{
			get
			{
				return this.helpContext;
			}
			set
			{
				DetailsTemplateControl.ValidateRange(value, 0, Page.maxHelpContextLength);
				this.helpContext = value;
			}
		}

		public ICollection<DetailsTemplateControl> Controls
		{
			get
			{
				return this.controls;
			}
		}

		public override string ToString()
		{
			return this.text;
		}

		private static int maxHelpContextLength = 99999;

		private int helpContext;

		private string text = string.Empty;

		private Collection<DetailsTemplateControl> controls = new Collection<DetailsTemplateControl>();
	}
}
