using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;

namespace Microsoft.Exchange.Clients.Owa.Core
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
					this.ThrowParserException("Parameter name is empty. Url=" + base.EventHandler.HttpContext.Request.RawUrl);
				}
				if (string.CompareOrdinal(key, "ns") != 0 && string.CompareOrdinal(key, "ev") != 0 && string.CompareOrdinal(key, "oeh") != 0 && string.CompareOrdinal(key, "cpc") != 0 && string.CompareOrdinal(key, "calist") != 0 && string.CompareOrdinal(key, "pfmk") != 0 && string.CompareOrdinal(key, Globals.RealmParameter) != 0)
				{
					OwaEventParameterAttribute paramInfo = base.GetParamInfo(key);
					if (paramInfo.IsStruct || paramInfo.IsArray)
					{
						this.ThrowParserException("Structs and arrays are not supported in GET requests");
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
