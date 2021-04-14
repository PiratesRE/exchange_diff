using System;
using System.Collections.Specialized;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.OData.Web
{
	internal class ODataRawQueryOptions
	{
		public ODataRawQueryOptions(NameValueCollection queryString)
		{
			ArgumentValidator.ThrowIfNull("queryString", queryString);
			foreach (string text in queryString.AllKeys)
			{
				string text2 = queryString[text];
				string key;
				switch (key = text)
				{
				case "$filter":
					this.Filter = text2;
					break;
				case "$orderby":
					this.OrderBy = text2;
					break;
				case "$top":
					this.Top = text2;
					break;
				case "$skip":
					this.Skip = text2;
					break;
				case "$select":
					this.Select = text2;
					break;
				case "$inlinecount":
					this.InlineCount = text2;
					break;
				case "$expand":
					this.Expand = text2;
					break;
				case "$format":
					this.Format = text2;
					break;
				case "$skiptoken":
					this.SkipToken = text2;
					break;
				}
			}
		}

		public string Filter { get; private set; }

		public string OrderBy { get; private set; }

		public string Top { get; private set; }

		public string Skip { get; private set; }

		public string Select { get; private set; }

		public string Expand { get; private set; }

		public string InlineCount { get; private set; }

		public string Format { get; private set; }

		public string SkipToken { get; internal set; }
	}
}
