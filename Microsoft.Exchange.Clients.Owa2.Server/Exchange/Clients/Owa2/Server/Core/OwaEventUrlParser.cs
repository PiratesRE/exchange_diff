using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class OwaEventUrlParser : OwaEventParserBase
	{
		internal OwaEventUrlParser(OwaEventHandlerBase eventHandler) : base(eventHandler, 4)
		{
		}

		protected override Hashtable ParseParameters()
		{
			NameValueCollection queryString = base.EventHandler.HttpContext.Request.QueryString;
			if (queryString.Count == 0)
			{
				return base.ParameterTable;
			}
			for (int i = 0; i < queryString.Count; i++)
			{
				string key = queryString.GetKey(i);
				if (string.IsNullOrEmpty(key))
				{
					this.ThrowParserException("Parameter name is empty.");
				}
				if (string.CompareOrdinal(key, "ns") != 0 && string.CompareOrdinal(key, "ev") != 0 && string.CompareOrdinal(key, "oeh2") != 0)
				{
					OwaEventParameterAttribute paramInfo = base.GetParamInfo(key);
					if (paramInfo.IsArray)
					{
						this.ThrowParserException("Arrays are not supported in GET requests");
					}
					string text = queryString[i];
					if (text == null)
					{
						base.AddEmptyParameter(paramInfo);
					}
					else
					{
						base.AddSimpleTypeParameter(paramInfo, text);
					}
				}
			}
			return base.ParameterTable;
		}

		protected override void ThrowParserException(string description)
		{
			throw new OwaInvalidRequestException(string.Format(CultureInfo.InvariantCulture, "Invalid request. Url: {0}. {1}.", new object[]
			{
				base.EventHandler.HttpContext.Request.RawUrl,
				(description != null) ? (" " + description) : string.Empty
			}), null, this);
		}
	}
}
