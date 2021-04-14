using System;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.HttpProxy.Common;

namespace Microsoft.Exchange.HttpProxy
{
	internal class ServerInfoAnchorMailbox : AnchorMailbox
	{
		public ServerInfoAnchorMailbox(string fqdn, IRequestContext requestContext) : base(AnchorSource.ServerInfo, fqdn, requestContext)
		{
			if (string.IsNullOrEmpty(fqdn))
			{
				throw new ArgumentNullException("fqdn");
			}
			base.NotFoundExceptionCreator = delegate()
			{
				string message = string.Format("Cannot find server {0}.", fqdn);
				return new ServerNotFoundException(message, fqdn);
			};
		}

		public ServerInfoAnchorMailbox(BackEndServer backendServer, IRequestContext requestContext) : base(AnchorSource.ServerInfo, backendServer.Fqdn, requestContext)
		{
			ServiceTopology currentServiceTopology = ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\AnchorMailbox\\ServerInfoAnchorMailbox.cs", ".ctor", 60);
			int num;
			if (!currentServiceTopology.TryGetServerVersion(backendServer.Fqdn, out num, "f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\AnchorMailbox\\ServerInfoAnchorMailbox.cs", ".ctor", 62))
			{
				throw new ArgumentException("Invalid value");
			}
			this.BackEndServer = backendServer;
		}

		public BackEndServer BackEndServer { get; private set; }

		public string Fqdn
		{
			get
			{
				return (string)base.SourceObject;
			}
		}

		public override BackEndServer TryDirectBackEndCalculation()
		{
			if (this.BackEndServer != null)
			{
				return this.BackEndServer;
			}
			int? num = ServerLookup.LookupVersion(this.Fqdn);
			if (num == null)
			{
				return base.CheckForNullAndThrowIfApplicable<BackEndServer>(null);
			}
			this.BackEndServer = new BackEndServer(this.Fqdn, num.Value);
			return this.BackEndServer;
		}
	}
}
