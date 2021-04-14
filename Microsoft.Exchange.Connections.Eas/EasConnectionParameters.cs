using System;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public sealed class EasConnectionParameters : ConnectionParameters
	{
		public EasConnectionParameters(INamedObject id, ILog log, EasProtocolVersion easProtocolVersion = EasProtocolVersion.Version140, bool acceptMultipart = false, bool requestCompression = false, string clientLanguage = null) : base(id, log, long.MaxValue, 300000)
		{
			this.EasProtocolVersion = easProtocolVersion;
			this.AcceptMultipart = acceptMultipart;
			this.RequestCompression = requestCompression;
			this.ClientLanguage = clientLanguage;
		}

		public EasProtocolVersion EasProtocolVersion { get; set; }

		internal bool AcceptMultipart { get; set; }

		internal bool RequestCompression { get; set; }

		internal string ClientLanguage { get; set; }
	}
}
