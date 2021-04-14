using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class OWADispatchOperationSelector : WebHttpDispatchOperationSelector
	{
		internal OWADispatchOperationSelector(ServiceEndpoint endpoint) : base(endpoint)
		{
		}

		protected override string SelectOperation(ref Message message, out bool uriMatched)
		{
			string text = base.SelectOperation(ref message, out uriMatched);
			HttpRequestMessageProperty httpRequestMessageProperty = (HttpRequestMessageProperty)message.Properties[HttpRequestMessageProperty.Name];
			if (uriMatched)
			{
				httpRequestMessageProperty.Headers[OWADispatchOperationSelector.Action] = text;
				return text;
			}
			if (httpRequestMessageProperty == null)
			{
				throw new FaultException(Strings.MissingHttpRequestMessageProperty);
			}
			text = httpRequestMessageProperty.Headers[OWADispatchOperationSelector.Action];
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			throw new FaultException(Strings.MissingActionHeader);
		}

		internal static readonly string Action = "Action";
	}
}
