using System;
using System.Text;
using Microsoft.Exchange.Security;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public sealed class SanitizedEventHandlerString : SanitizedStringBase<OwaHtml>
	{
		public SanitizedEventHandlerString(string eventName, string handlerCode, bool returnFalse)
		{
			if (eventName == null)
			{
				throw new ArgumentNullException("eventName");
			}
			if (handlerCode == null)
			{
				throw new ArgumentNullException("handlerCode");
			}
			string text = Utilities.HtmlEncode(handlerCode);
			int capacity = eventName.Length * 3 + text.Length + 100;
			StringBuilder stringBuilder = new StringBuilder(capacity);
			stringBuilder.Append(eventName);
			stringBuilder.Append("='_e(this,this.getAttribute(\"_e_");
			stringBuilder.Append(eventName);
			stringBuilder.Append("\"), event)");
			if (returnFalse)
			{
				stringBuilder.Append(";return false;");
			}
			stringBuilder.Append("' _e_");
			stringBuilder.Append(eventName);
			stringBuilder.Append("=\"");
			stringBuilder.Append(text);
			stringBuilder.Append("\"");
			base.TrustedValue = stringBuilder.ToString();
		}

		public SanitizedEventHandlerString(string eventName, string handlerCode) : this(eventName, handlerCode, false)
		{
		}

		private SanitizedEventHandlerString()
		{
		}

		protected override string Sanitize(string rawValue)
		{
			return base.TrustedValue;
		}
	}
}
