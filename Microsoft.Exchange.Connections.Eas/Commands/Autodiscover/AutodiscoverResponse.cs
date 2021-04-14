using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Response.Autodiscover;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.Autodiscover
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlRoot(ElementName = "Autodiscover", Namespace = "http://schemas.microsoft.com/exchange/autodiscover/responseschema/2006")]
	public class AutodiscoverResponse : IHaveAnHttpStatus
	{
		[XmlElement(ElementName = "Response", Namespace = "http://schemas.microsoft.com/exchange/autodiscover/mobilesync/responseschema/2006")]
		public Response Response { get; set; }

		[XmlIgnore]
		public HttpStatus HttpStatus { get; set; }

		[XmlIgnore]
		internal AutodiscoverStatus AutodiscoverStatus { get; set; }

		[XmlIgnore]
		internal string AutodiscoverSteps { get; set; }

		[XmlIgnore]
		internal string AutodiscoveredDomain
		{
			get
			{
				if (this.Response == null || this.Response.Action == null || this.Response.Action.Settings == null || this.Response.Action.Settings.Server == null)
				{
					return string.Empty;
				}
				Server server = this.Response.Action.Settings.Server[0];
				string url = server.Url;
				Uri uri = new Uri(url);
				return uri.Host;
			}
		}

		private static Dictionary<byte, AutodiscoverStatus> StatusToEnumMap { get; set; } = new Dictionary<byte, AutodiscoverStatus>
		{
			{
				1,
				AutodiscoverStatus.Success
			},
			{
				2,
				AutodiscoverStatus.ProtocolError
			}
		};

		internal void ConvertStatusToEnum()
		{
			byte b = (this.Response == null || this.Response.Error == null) ? 1 : ((byte)this.Response.Error.ErrorCode);
			this.AutodiscoverStatus = (AutodiscoverResponse.StatusToEnumMap.ContainsKey(b) ? AutodiscoverResponse.StatusToEnumMap[b] : ((AutodiscoverStatus)b));
		}
	}
}
