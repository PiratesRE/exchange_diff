using System;

namespace Microsoft.Exchange.Management.SystemManager
{
	public class TextComparer : ITextComparer, ISupportTextComparer
	{
		public virtual int Compare(object x, object y, ICustomFormatter customFormatter, IFormatProvider formatProvider, string formatString, string defaultEmptyText)
		{
			return string.Compare(this.Format(x, customFormatter, formatProvider, formatString, defaultEmptyText), this.Format(y, customFormatter, formatProvider, formatString, defaultEmptyText), StringComparison.CurrentCulture);
		}

		public virtual string Format(object item, ICustomFormatter customFormatter, IFormatProvider formatProvider, string formatString, string defaultEmptyText)
		{
			string text = string.Empty;
			if (customFormatter != null)
			{
				text = customFormatter.Format(formatString, item, formatProvider);
			}
			else
			{
				text = item.ToUserFriendText();
			}
			if (string.IsNullOrEmpty(text))
			{
				text = defaultEmptyText;
			}
			return text;
		}

		public static TextComparer DefaultTextComparer = new TextComparer();
	}
}
