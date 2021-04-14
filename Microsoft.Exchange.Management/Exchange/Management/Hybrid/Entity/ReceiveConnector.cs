using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Hybrid.Entity
{
	internal class ReceiveConnector : IReceiveConnector, IEntity<IReceiveConnector>
	{
		public ReceiveConnector()
		{
		}

		public ReceiveConnector(ReceiveConnector rc)
		{
			this.Identity = (ADObjectId)rc.Identity;
			this.Server = rc.Server;
			this.TlsCertificateName = rc.TlsCertificateName;
			this.TlsDomainCapabilities = ((rc.TlsDomainCapabilities != null && rc.TlsDomainCapabilities.Count > 0) ? rc.TlsDomainCapabilities[0] : null);
		}

		public ADObjectId Identity { get; set; }

		public ADObjectId Server { get; set; }

		public SmtpX509Identifier TlsCertificateName { get; set; }

		public SmtpReceiveDomainCapabilities TlsDomainCapabilities { get; set; }

		private static bool AreEqual(SmtpReceiveDomainCapabilities a, SmtpReceiveDomainCapabilities b)
		{
			return a == b || (a != null && b != null && a.Equals(b));
		}

		public override string ToString()
		{
			if (this.Identity != null)
			{
				return this.Identity.ToString();
			}
			return "<New>";
		}

		public bool Equals(IReceiveConnector obj)
		{
			return obj != null && string.Equals(this.Server.Name, obj.Server.Name, StringComparison.InvariantCultureIgnoreCase) && TaskCommon.AreEqual(this.TlsCertificateName, obj.TlsCertificateName) && ReceiveConnector.AreEqual(this.TlsDomainCapabilities, obj.TlsDomainCapabilities);
		}

		public IReceiveConnector Clone(ADObjectId identity)
		{
			ReceiveConnector receiveConnector = new ReceiveConnector();
			receiveConnector.UpdateFrom(this);
			receiveConnector.Identity = identity;
			return receiveConnector;
		}

		public void UpdateFrom(IReceiveConnector obj)
		{
			this.Server = obj.Server;
			this.TlsCertificateName = obj.TlsCertificateName;
			this.TlsDomainCapabilities = obj.TlsDomainCapabilities;
		}
	}
}
