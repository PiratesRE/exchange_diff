using System;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.OneDrive
{
	public static class ClientContextFactory
	{
		public static IClientContext Create(string url)
		{
			if (ClientContextFactory.UseMockAttachmentDataProvider)
			{
				return new MockClientContext(url);
			}
			return new ClientContextWrapper(url);
		}

		private static readonly bool UseMockAttachmentDataProvider = new BoolAppSettingsEntry("UseMockAttachmentDataProvider", false, ExTraceGlobals.AttachmentHandlingTracer).Value;
	}
}
