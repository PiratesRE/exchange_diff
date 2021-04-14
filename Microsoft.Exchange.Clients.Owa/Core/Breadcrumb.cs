using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal class Breadcrumb
	{
		internal Breadcrumb(ExDateTime timestamp, string text)
		{
			this.timestamp = timestamp;
			this.text = text;
		}

		internal ExDateTime Timestamp
		{
			get
			{
				return this.timestamp;
			}
		}

		internal string Text
		{
			get
			{
				return this.text;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(128);
			stringBuilder.Append(this.Timestamp.ToString("hh:mm:ss:ff", DateTimeFormatInfo.InvariantInfo));
			stringBuilder.Append(" ");
			stringBuilder.AppendLine((this.Text != null) ? this.Text : "<null>");
			return stringBuilder.ToString();
		}

		private string text;

		private ExDateTime timestamp;
	}
}
