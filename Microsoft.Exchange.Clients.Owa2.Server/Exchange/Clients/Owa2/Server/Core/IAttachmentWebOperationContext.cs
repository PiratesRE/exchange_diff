using System;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal interface IAttachmentWebOperationContext : IOutgoingWebResponseContext
	{
		UserAgent UserAgent { get; }

		bool IsPublicLogon { get; }

		void SetNoCacheNoStore();

		string GetRequestHeader(string name);
	}
}
