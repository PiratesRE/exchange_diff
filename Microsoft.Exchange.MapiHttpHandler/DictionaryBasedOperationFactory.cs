using System;
using System.Collections.Generic;
using System.Web;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MapiHttp
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DictionaryBasedOperationFactory : IAsyncOperationFactory
	{
		public DictionaryBasedOperationFactory(IDictionary<string, Func<HttpContextBase, AsyncOperation>> operationFactoryMethods)
		{
			foreach (string text in operationFactoryMethods.Keys)
			{
				this.operationFactoryMethods[text.ToLowerInvariant()] = operationFactoryMethods[text];
			}
		}

		public AsyncOperation Create(string requestType, HttpContextBase context)
		{
			Func<HttpContextBase, AsyncOperation> func;
			if (this.operationFactoryMethods.TryGetValue(requestType.ToLowerInvariant(), out func))
			{
				return func(context);
			}
			throw ProtocolException.FromResponseCode((LID)64032, string.Format("Unknown request type {0}", requestType), ResponseCode.InvalidRequestType, null);
		}

		private readonly IDictionary<string, Func<HttpContextBase, AsyncOperation>> operationFactoryMethods = new Dictionary<string, Func<HttpContextBase, AsyncOperation>>();
	}
}
