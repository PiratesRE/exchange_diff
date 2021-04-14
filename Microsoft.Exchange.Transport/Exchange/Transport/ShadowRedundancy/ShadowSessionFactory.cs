using System;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Protocols.Smtp;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal class ShadowSessionFactory
	{
		public ShadowSessionFactory(ShadowRedundancyManager shadowRedundancyManager)
		{
			this.shadowRedundancyManager = shadowRedundancyManager;
			this.configurationSource = shadowRedundancyManager.Configuration;
			this.transportConfigPicker = new TransportConfigBasedHubPicker(this.configurationSource);
		}

		public virtual IShadowSession GetShadowSession(ISmtpInSession inSession, bool isBdat)
		{
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug((long)this.GetHashCode(), "ShadowSessionFactory.GetShadowSession");
			if (inSession == null)
			{
				throw new ArgumentNullException("inSession");
			}
			if (!this.configurationSource.Enabled || this.configurationSource.CompatibilityVersion != ShadowRedundancyCompatibilityVersion.E15 || Components.Configuration.ProcessTransportRole != ProcessTransportRole.Hub)
			{
				return ShadowSessionFactory.nullSessionInstance;
			}
			ExTraceGlobals.ShadowRedundancyTracer.TraceDebug<bool>((long)this.GetHashCode(), "ShadowSessionFactory.GetShadowSession returning new ShadowSession isBdat={0}", isBdat);
			if (isBdat)
			{
				return this.GetBdatSession(inSession);
			}
			return this.GetDataSession(inSession);
		}

		private ShadowHubPickerBase GetShadowPicker()
		{
			if (this.transportConfigPicker.Enabled)
			{
				return this.transportConfigPicker;
			}
			return new RoutingBasedHubPicker(this.configurationSource, Components.RoutingComponent.MailRouter);
		}

		private ShadowBdatSession GetBdatSession(ISmtpInSession inSession)
		{
			return new ShadowBdatSession(inSession, this.shadowRedundancyManager, this.GetShadowPicker(), inSession.SmtpInServer.SmtpOutConnectionHandler);
		}

		private ShadowDataSession GetDataSession(ISmtpInSession inSession)
		{
			return new ShadowDataSession(inSession, this.shadowRedundancyManager, this.GetShadowPicker(), inSession.SmtpInServer.SmtpOutConnectionHandler);
		}

		private static readonly NullShadowSession nullSessionInstance = new NullShadowSession();

		private readonly IShadowRedundancyConfigurationSource configurationSource;

		private readonly TransportConfigBasedHubPicker transportConfigPicker;

		private readonly ShadowRedundancyManager shadowRedundancyManager;
	}
}
