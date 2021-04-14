using System;
using System.Linq;
using System.Net;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.Options
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class OptionsCommand : EasPseudoCommand<OptionsRequest, OptionsResponse>
	{
		protected internal OptionsCommand(EasConnectionSettings easConnectionSettings) : base(Command.Options, easConnectionSettings)
		{
			string domain = base.EasConnectionSettings.EasEndpointSettings.Domain;
			base.UriString = string.Format("{0}//{1}/Microsoft-Server-ActiveSync", this.UseSsl ? "https:" : "http:", domain);
			base.InitializeExpectedHttpStatusCodes(typeof(HttpStatus));
		}

		internal OptionsStatus OptionsStatus { get; private set; }

		protected override string RequestMethodName
		{
			get
			{
				return "OPTIONS";
			}
		}

		protected override void AddWebRequestHeaders(HttpWebRequest webRequest)
		{
			webRequest.Headers.Add("MS-ASProtocolVersion", base.ProtocolVersion);
			webRequest.Host = base.EasConnectionSettings.EasEndpointSettings.Domain;
		}

		protected override void AddWebRequestBody(HttpWebRequest webRequest, OptionsRequest easRequest)
		{
		}

		protected override OptionsResponse ExtractResponse(HttpWebResponse webResponse)
		{
			base.LogInfoHeaders(webResponse.Headers);
			string responseHeader = webResponse.GetResponseHeader("MS-ASProtocolCommands");
			string responseHeader2 = webResponse.GetResponseHeader("MS-ASProtocolVersions");
			string responseHeader3 = webResponse.GetResponseHeader("X-OLK-Extensions");
			if (string.IsNullOrWhiteSpace(responseHeader) || string.IsNullOrWhiteSpace(responseHeader2))
			{
				return new OptionsResponse
				{
					OptionsStatus = OptionsStatus.MissingHeaderInResponse
				};
			}
			string[] first = (from sValue in responseHeader.Split(new char[]
			{
				','
			})
			select sValue.Trim()).ToArray<string>();
			string[] second = (from sValue in responseHeader2.Split(new char[]
			{
				','
			})
			select sValue.Trim()).ToArray<string>();
			string[] capabilities = (from sValue in responseHeader3.Split(new char[]
			{
				','
			})
			select sValue.Trim()).ToArray<string>();
			EasServerCapabilities easServerCapabilities = new EasServerCapabilities(first.Union(second));
			EasExtensionCapabilities easExtensionCapabilities = new EasExtensionCapabilities(capabilities);
			this.OptionsStatus = OptionsStatus.Success;
			return new OptionsResponse
			{
				OptionsStatus = OptionsStatus.Success,
				EasServerCapabilities = easServerCapabilities,
				EasExtensionCapabilities = easExtensionCapabilities
			};
		}

		private const string UriFormatString = "{0}//{1}/Microsoft-Server-ActiveSync";
	}
}
