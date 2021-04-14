using System;
using Microsoft.Exchange.Connections.Eas.Commands.Autodiscover;
using Microsoft.Exchange.Connections.Eas.Commands.Options;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.Connect
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class ConnectResponse : IHaveAnHttpStatus
	{
		public ConnectResponse()
		{
		}

		public ConnectResponse(AutodiscoverResponse autodiscoverResponse, AutodiscoverOption autodiscoverOption)
		{
			this.AutodiscoverResponse = autodiscoverResponse;
			this.AutodiscoverOption = autodiscoverOption;
			this.HttpStatus = autodiscoverResponse.HttpStatus;
			this.ConnectStatus = ((autodiscoverResponse.HttpStatus == HttpStatus.OK && autodiscoverResponse.AutodiscoverStatus == AutodiscoverStatus.Success) ? ConnectStatus.Success : ConnectStatus.AutodiscoverFailed);
		}

		public HttpStatus HttpStatus { get; set; }

		public string HttpStatusString
		{
			get
			{
				return this.HttpStatus.ToString();
			}
		}

		public ConnectStatus ConnectStatus { get; set; }

		public string ConnectStatusString
		{
			get
			{
				if (this.ConnectStatus != ConnectStatus.AutodiscoverFailed)
				{
					return this.ConnectStatus.ToString();
				}
				return this.ConnectStatus.ToString() + ":" + this.AutodiscoverResponse.AutodiscoverSteps;
			}
		}

		public string UserSmtpAddressString { get; set; }

		public AutodiscoverResponse AutodiscoverResponse { get; set; }

		public AutodiscoverOption AutodiscoverOption { get; set; }

		public OptionsResponse OptionsResponse { get; set; }
	}
}
