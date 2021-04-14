using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;

namespace Microsoft.Exchange.UM.UMCore
{
	internal sealed class InfoMessage
	{
		public Dictionary<string, string> Headers
		{
			get
			{
				return this.headers;
			}
		}

		public byte[] Body
		{
			get
			{
				return this.body;
			}
			set
			{
				this.body = value;
			}
		}

		public ContentType ContentType
		{
			get
			{
				return this.contentType;
			}
			set
			{
				this.contentType = value;
			}
		}

		public override string ToString()
		{
			if (this.body != null && this.body.Length > 0)
			{
				return Encoding.UTF8.GetString(this.body);
			}
			return string.Empty;
		}

		private Dictionary<string, string> headers = new Dictionary<string, string>();

		private ContentType contentType;

		private byte[] body;

		internal class MessageReceivedEventArgs : EventArgs
		{
			public MessageReceivedEventArgs(InfoMessage message)
			{
				this.message = message;
			}

			public InfoMessage Message
			{
				get
				{
					return this.message;
				}
			}

			private InfoMessage message;
		}

		internal sealed class PlatformMessageReceivedEventArgs : InfoMessage.MessageReceivedEventArgs
		{
			public PlatformMessageReceivedEventArgs(PlatformCallInfo callInfo, InfoMessage message, bool isOptions) : base(message)
			{
				this.CallInfo = callInfo;
				this.IsOptions = isOptions;
			}

			public PlatformCallInfo CallInfo { get; private set; }

			public bool IsOptions { get; private set; }

			public int ResponseCode { get; set; }

			public PlatformSipUri ResponseContactUri { get; set; }
		}
	}
}
