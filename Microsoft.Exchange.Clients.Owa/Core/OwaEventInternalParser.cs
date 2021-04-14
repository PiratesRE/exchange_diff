using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class OwaEventInternalParser : OwaEventParserBase
	{
		internal OwaEventInternalParser(OwaEventHandlerBase eventHandler) : base(eventHandler, 4)
		{
		}

		internal OwaEventInternalParser(OwaEventHandlerBase eventHandler, int parameterTableCapacity) : base(eventHandler, parameterTableCapacity)
		{
		}

		protected override Hashtable ParseParameters()
		{
			Dictionary<string, object> internalHandlerParameters = OwaContext.Current.InternalHandlerParameters;
			if (internalHandlerParameters == null)
			{
				this.ThrowParserException("Internal parameters are not set");
			}
			foreach (string text in internalHandlerParameters.Keys)
			{
				object obj = internalHandlerParameters[text];
				OwaEventParameterAttribute paramInfo = base.GetParamInfo(text);
				Type type = obj.GetType();
				if (type != paramInfo.Type && !type.IsSubclassOf(paramInfo.Type))
				{
					this.ThrowParserException("Parameter is not of the correct type");
				}
				base.AddParameter(paramInfo, obj);
			}
			return base.ParameterTable;
		}

		protected override void ThrowParserException(string description)
		{
			throw new OwaInvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Invalid internal handler call. {0}", new object[]
			{
				(description != null) ? (" " + description) : string.Empty
			}), null, this);
		}
	}
}
