using System;
using Microsoft.Exchange.Data.Mime;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal static class XHeaderUtils
	{
		internal static string GetProperty(HeaderList mimeHeaders, string name)
		{
			Header header = mimeHeaders.FindFirst(name);
			if (header == null || header.Value == null)
			{
				return null;
			}
			return header.Value;
		}

		internal static void SetProperty(HeaderList mimeHeaders, string name, string value)
		{
			mimeHeaders.RemoveAll(name);
			if (!string.IsNullOrEmpty(value))
			{
				mimeHeaders.AppendChild(new TextHeader(name, value));
			}
		}
	}
}
