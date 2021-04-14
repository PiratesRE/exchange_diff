using System;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class Breadcrumb
	{
		internal Breadcrumb(ExDateTime timestamp, string text)
		{
			this.Timestamp = timestamp;
			this.Text = text;
		}

		internal ExDateTime Timestamp { get; private set; }

		internal string Text { get; private set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(128);
			stringBuilder.Append(this.Timestamp.ToString("hh:mm:ss:ff", DateTimeFormatInfo.InvariantInfo));
			stringBuilder.Append(" ");
			stringBuilder.AppendLine((this.Text != null) ? this.Text : "<null>");
			return stringBuilder.ToString();
		}
	}
}
