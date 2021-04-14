using System;
using System.Net;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Transport.Logging;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class SmtpReceiveConnectorStub
	{
		public SmtpReceiveConnectorStub(ReceiveConnector connector, ISmtpReceivePerfCounters receivePerfCounters, ISmtpAvailabilityPerfCounters availabilityPerfCounters)
		{
			ArgumentValidator.ThrowIfNull("connector", connector);
			ArgumentValidator.ThrowIfNull("receivePerfCounters", receivePerfCounters);
			ArgumentValidator.ThrowIfNull("availabilityPerfCounters", availabilityPerfCounters);
			this.connector = connector;
			this.securityDescriptor = connector.GetSecurityDescriptor();
			this.smtpReceivePerfCounterInstance = receivePerfCounters;
			this.smtpAvailabilityPerfCounters = availabilityPerfCounters;
			this.checkMaxInboundConnection = !connector.MaxInboundConnection.IsUnlimited;
			if (this.checkMaxInboundConnection)
			{
				this.maxInboundConnection = connector.MaxInboundConnection.Value;
			}
			this.checkMaxInboundConnectionPerSource = !connector.MaxInboundConnectionPerSource.IsUnlimited;
			if (this.checkMaxInboundConnectionPerSource)
			{
				this.maxInboundConnectionPerSource = connector.MaxInboundConnectionPerSource.Value;
			}
			this.checkMaxInboundConnectionPercentagePerSource = (connector.MaxInboundConnectionPercentagePerSource < 100);
			if (this.checkMaxInboundConnectionPercentagePerSource)
			{
				this.maxInboundConnectionPercentagePerSource = connector.MaxInboundConnectionPercentagePerSource;
				this.maxInboundConnectionPercentagePerSourceFraction = (double)connector.MaxInboundConnectionPercentagePerSource / 100.0;
			}
			this.DetermineCapabilities();
		}

		public ClientIPTable ConnectionTable
		{
			get
			{
				return this.clientIpTable;
			}
			set
			{
				this.clientIpTable = value;
			}
		}

		public ReceiveConnector Connector
		{
			get
			{
				return this.connector;
			}
		}

		public ISmtpReceivePerfCounters SmtpReceivePerfCounterInstance
		{
			get
			{
				return this.smtpReceivePerfCounterInstance;
			}
		}

		public ISmtpAvailabilityPerfCounters SmtpAvailabilityPerfCounters
		{
			get
			{
				return this.smtpAvailabilityPerfCounters;
			}
		}

		public RawSecurityDescriptor SecurityDescriptor
		{
			get
			{
				return this.securityDescriptor;
			}
		}

		public SmtpReceiveCapabilities NoTlsCapabilities
		{
			get
			{
				return this.noTlsCapabilities;
			}
		}

		public bool ContainsTlsDomainCapabilities
		{
			get
			{
				return this.tlsDomainCapabilities != null;
			}
		}

		public ClientData AddConnection(IPAddress ipAddress, out bool maxConnectionsExceeded, out bool maxConnectionsPerSourceExceeded)
		{
			int totalConnections;
			ClientData clientData = this.clientIpTable.Add(ipAddress, out totalConnections);
			this.CheckIfConnectionThresholdsExceeded(ipAddress, clientData.Count, totalConnections, out maxConnectionsExceeded, out maxConnectionsPerSourceExceeded);
			return clientData;
		}

		public ClientData AddConnection(IPAddress ipAddress, ulong significantAddressBytes, out bool maxConnectionsExceeded, out bool maxConnectionsPerSourceExceeded)
		{
			int totalConnections;
			ClientData clientData = this.clientIpTable.Add(ipAddress, significantAddressBytes, out totalConnections);
			this.CheckIfConnectionThresholdsExceeded(ipAddress, clientData.Count, totalConnections, out maxConnectionsExceeded, out maxConnectionsPerSourceExceeded);
			return clientData;
		}

		public void RemoveConnection(IPAddress ip)
		{
			this.clientIpTable.Remove(ip);
		}

		public void RemoveConnection(ulong significantIPAddressBytes)
		{
			this.clientIpTable.Remove(significantIPAddressBytes);
		}

		public bool TryGetTlsDomainCapabilities(ICertificateValidator certificateValidator, X509Certificate2 tlsRemoteCertificate, IProtocolLogSession protocolLogSession, out SmtpReceiveDomainCapabilities smtpReceiveDomainCapabilities)
		{
			return this.TryGetTlsDomainCapabilities(certificateValidator, new X509Certificate2Wrapper(tlsRemoteCertificate), protocolLogSession, out smtpReceiveDomainCapabilities);
		}

		public bool TryGetTlsDomainCapabilities(ICertificateValidator certificateValidator, IX509Certificate2 tlsRemoteCertificate, IProtocolLogSession protocolLogSession, out SmtpReceiveDomainCapabilities smtpReceiveDomainCapabilities)
		{
			ArgumentValidator.ThrowIfNull("certificateValidator", certificateValidator);
			ArgumentValidator.ThrowIfNull("tlsRemoteCertificate", tlsRemoteCertificate);
			ArgumentValidator.ThrowIfNull("protocolLogSession", protocolLogSession);
			if (this.tlsDomainCapabilities == null)
			{
				smtpReceiveDomainCapabilities = null;
				return false;
			}
			string text;
			return certificateValidator.FindBestMatchingCertificateFqdn<SmtpReceiveDomainCapabilities>(this.tlsDomainCapabilities, tlsRemoteCertificate, MatchOptions.None, protocolLogSession, out smtpReceiveDomainCapabilities, out text);
		}

		private void DetermineCapabilities()
		{
			foreach (SmtpReceiveDomainCapabilities smtpReceiveDomainCapabilities in this.connector.TlsDomainCapabilities)
			{
				if (smtpReceiveDomainCapabilities.Domain.SmtpDomain == null)
				{
					ExTraceGlobals.SmtpReceiveTracer.TraceError<SmtpReceiveDomainCapabilities, string>((long)this.GetHashCode(), "Ignoring wildcard domain capabilities <{0}> of Receive Connector <{1}>", smtpReceiveDomainCapabilities, this.connector.Name);
				}
				else if (SmtpReceiveConnectorStub.NoTlsDomain.Equals(smtpReceiveDomainCapabilities.Domain.SmtpDomain))
				{
					this.noTlsCapabilities = smtpReceiveDomainCapabilities.Capabilities;
				}
				else if (smtpReceiveDomainCapabilities.Capabilities != SmtpReceiveCapabilities.None)
				{
					if (this.tlsDomainCapabilities == null)
					{
						this.tlsDomainCapabilities = new MatchableDomainMap<Tuple<X500DistinguishedName, SmtpReceiveDomainCapabilities>>(this.connector.TlsDomainCapabilities.Count);
					}
					MatchableDomain domain = new MatchableDomain(smtpReceiveDomainCapabilities.Domain);
					X500DistinguishedName item = null;
					if (smtpReceiveDomainCapabilities.SmtpX509Identifier != null && !string.IsNullOrEmpty(smtpReceiveDomainCapabilities.SmtpX509Identifier.CertificateIssuer))
					{
						item = new X500DistinguishedName(smtpReceiveDomainCapabilities.SmtpX509Identifier.CertificateIssuer);
					}
					this.tlsDomainCapabilities.Add(domain, Tuple.Create<X500DistinguishedName, SmtpReceiveDomainCapabilities>(item, smtpReceiveDomainCapabilities));
				}
			}
		}

		private void CheckIfConnectionThresholdsExceeded(IPAddress ipAddress, int clientIPCount, int totalConnections, out bool maxConnectionsExceeded, out bool maxConnectionsPerSourceExceeded)
		{
			if (this.checkMaxInboundConnection)
			{
				maxConnectionsExceeded = (totalConnections > this.maxInboundConnection);
				if (maxConnectionsExceeded)
				{
					ExTraceGlobals.SmtpReceiveTracer.TraceDebug<int, int>((long)ipAddress.GetHashCode(), "Connection from {0} rejected: {1} (total) > {2} (maximum)", totalConnections, this.maxInboundConnection);
					this.SmtpAvailabilityPerfCounters.UpdatePerformanceCounters(LegitimateSmtpAvailabilityCategory.RejectDueToMaxInboundConnectionLimit);
				}
			}
			else
			{
				maxConnectionsExceeded = false;
			}
			if (this.checkMaxInboundConnectionPerSource)
			{
				maxConnectionsPerSourceExceeded = (clientIPCount > this.maxInboundConnectionPerSource);
				if (maxConnectionsPerSourceExceeded)
				{
					ExTraceGlobals.SmtpReceiveTracer.TraceDebug<IPAddress, int, int>((long)ipAddress.GetHashCode(), "Connection from {0} rejected: {1} (total from this source) > {2} (maximum per source)", ipAddress, clientIPCount, this.maxInboundConnectionPerSource);
				}
			}
			else
			{
				maxConnectionsPerSourceExceeded = false;
			}
			if (!maxConnectionsPerSourceExceeded && this.checkMaxInboundConnectionPercentagePerSource)
			{
				if (this.maxInboundConnectionPercentagePerSource <= 0)
				{
					maxConnectionsPerSourceExceeded = true;
					return;
				}
				if (this.checkMaxInboundConnection)
				{
					int num = this.maxInboundConnection - totalConnections;
					double num2 = Math.Ceiling((double)num * this.maxInboundConnectionPercentagePerSourceFraction);
					maxConnectionsPerSourceExceeded = ((double)clientIPCount > num2);
					if (maxConnectionsPerSourceExceeded)
					{
						ExTraceGlobals.SmtpReceiveTracer.TraceDebug((long)ipAddress.GetHashCode(), "Connection from {0} rejected: connections from this source = {1}, avail = {2}, percentage of avail (limit) = {3}", new object[]
						{
							ipAddress,
							clientIPCount,
							num,
							num2
						});
					}
				}
			}
		}

		public static readonly SmtpDomain NoTlsDomain = new SmtpDomain("NO-TLS");

		private readonly ReceiveConnector connector;

		private ClientIPTable clientIpTable = new ClientIPTable();

		private readonly ISmtpReceivePerfCounters smtpReceivePerfCounterInstance;

		private readonly ISmtpAvailabilityPerfCounters smtpAvailabilityPerfCounters;

		private readonly RawSecurityDescriptor securityDescriptor;

		private readonly bool checkMaxInboundConnection;

		private readonly int maxInboundConnection;

		private readonly bool checkMaxInboundConnectionPerSource;

		private readonly int maxInboundConnectionPerSource;

		private readonly bool checkMaxInboundConnectionPercentagePerSource;

		private readonly int maxInboundConnectionPercentagePerSource;

		private readonly double maxInboundConnectionPercentagePerSourceFraction;

		private SmtpReceiveCapabilities noTlsCapabilities;

		private MatchableDomainMap<Tuple<X500DistinguishedName, SmtpReceiveDomainCapabilities>> tlsDomainCapabilities;
	}
}
