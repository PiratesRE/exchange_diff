using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class InstantMessageChat
	{
		internal InstantMessageChat(string contentType, string message)
		{
			this.contentType = contentType;
			this.message = message;
		}

		public string ContentType
		{
			get
			{
				return this.contentType;
			}
		}

		public string Message
		{
			get
			{
				return this.message;
			}
		}

		private string contentType;

		private string message;
	}
}
