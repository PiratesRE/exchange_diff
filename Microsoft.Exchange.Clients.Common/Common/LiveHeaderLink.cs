using System;

namespace Microsoft.Exchange.Clients.Common
{
	public class LiveHeaderLink : ILiveHeaderElement
	{
		public LiveHeaderLink()
		{
		}

		public LiveHeaderLink(string linkText)
		{
			this.Text = linkText;
		}

		public string Title
		{
			get
			{
				return this.title;
			}
			set
			{
				this.title = value;
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
				this.text = value;
			}
		}

		public string Href
		{
			get
			{
				return this.href;
			}
			set
			{
				this.href = value;
			}
		}

		public bool OpenInNewWindow
		{
			get
			{
				return this.openInNewWindow;
			}
			set
			{
				this.openInNewWindow = value;
			}
		}

		private string text;

		private string title;

		private string href;

		private bool openInNewWindow;
	}
}
