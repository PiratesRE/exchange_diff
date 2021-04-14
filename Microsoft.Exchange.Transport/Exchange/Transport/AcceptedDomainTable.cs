using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Transport.Configuration;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Transport
{
	internal class AcceptedDomainTable : AcceptedDomainMap
	{
		public AcceptedDomainTable(List<string> internalDomains, AcceptedDomainEntry defaultDomain, List<AcceptedDomainEntry> entries) : base(entries)
		{
			this.DefaultDomain = defaultDomain;
			this.edgeToBHDomains = new ReadOnlyCollection<string>(internalDomains);
		}

		public ReadOnlyCollection<string> EdgeToBHDomains
		{
			get
			{
				return this.edgeToBHDomains;
			}
		}

		public AcceptedDomainEntry DefaultDomain { get; private set; }

		public string DefaultDomainName
		{
			get
			{
				return AcceptedDomainTable.GetDomainName(this.DefaultDomain);
			}
		}

		private static string GetDomainName(AcceptedDomainEntry entry)
		{
			if (entry != null)
			{
				return entry.DomainName.Domain;
			}
			return null;
		}

		public const string TransportSettingsContainerName = "Transport Settings";

		public const string AcceptedDomainsContainerName = "Accepted Domains";

		private static readonly ExEventLog Log = new ExEventLog(ExTraceGlobals.ConfigurationTracer.Category, TransportEventLog.GetEventSource());

		private readonly ReadOnlyCollection<string> edgeToBHDomains;

		public class Builder : ConfigurationLoader<AcceptedDomainTable, AcceptedDomainTable.Builder>.SimpleBuilder<AcceptedDomain>
		{
			public bool IsBridgehead
			{
				get
				{
					return this.bridgehead;
				}
				set
				{
					this.bridgehead = value;
				}
			}

			public static int CreateAcceptedDomainEntries(IEnumerable<AcceptedDomain> domains, out List<AcceptedDomainEntry> entries, out AcceptedDomainEntry defaultDomain, out List<string> internalDomains)
			{
				int num = 0;
				entries = new List<AcceptedDomainEntry>();
				defaultDomain = null;
				internalDomains = new List<string>();
				if (domains != null)
				{
					foreach (AcceptedDomain acceptedDomain in domains)
					{
						if (acceptedDomain.DomainName == null)
						{
							string text = string.Format("Accepted domain name is null for the Distinguished Name '{0}'.", acceptedDomain.DistinguishedName ?? "not available");
							ExTraceGlobals.ConfigurationTracer.TraceError(0L, text);
							AcceptedDomainTable.Log.LogEvent(TransportEventLogConstants.Tuple_InvalidAcceptedDomain, null, new object[]
							{
								acceptedDomain.DistinguishedName
							});
							EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "InvalidAcceptedDomain", null, text, ResultSeverityLevel.Warning, false);
						}
						else
						{
							try
							{
								AcceptedDomainEntry acceptedDomainEntry = new AcceptedDomainEntry(acceptedDomain, acceptedDomain.OrganizationId);
								entries.Add(acceptedDomainEntry);
								if (acceptedDomainEntry.IsDefault)
								{
									defaultDomain = acceptedDomainEntry;
								}
								if (acceptedDomainEntry.IsInternal)
								{
									internalDomains.Add(acceptedDomainEntry.DomainName.Domain);
								}
								num += acceptedDomainEntry.EstimatedSize;
							}
							catch (ExchangeDataException ex)
							{
								ExTraceGlobals.ConfigurationTracer.TraceError<SmtpDomainWithSubdomains, ExchangeDataException>(0L, "Entry for {0} is invalid {1}", acceptedDomain.DomainName, ex);
								AcceptedDomainTable.Log.LogEvent(TransportEventLogConstants.Tuple_RejectedAcceptedDomain, acceptedDomain.DomainName.ToString(), new object[]
								{
									acceptedDomain.DomainName,
									ex
								});
							}
						}
					}
				}
				return num;
			}

			public override void LoadData(ITopologyConfigurationSession session, QueryScope scope)
			{
				base.RootId = session.GetOrgContainerId().GetChildId("Transport Settings").GetChildId("Accepted Domains");
				base.LoadData(session, QueryScope.OneLevel);
			}

			protected override AcceptedDomainTable BuildCache(List<AcceptedDomain> domains)
			{
				List<AcceptedDomainEntry> entries;
				AcceptedDomainEntry acceptedDomainEntry;
				List<string> internalDomains;
				AcceptedDomainTable.Builder.CreateAcceptedDomainEntries(domains, out entries, out acceptedDomainEntry, out internalDomains);
				if (this.IsBridgehead && (acceptedDomainEntry == null || acceptedDomainEntry.DomainName == null || acceptedDomainEntry.DomainName.Equals(SmtpDomainWithSubdomains.StarDomain)))
				{
					AcceptedDomainTable.Log.LogEvent(TransportEventLogConstants.Tuple_DefaultAuthoritativeDomainInvalid, null, new object[0]);
					EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "TransportServiceStartError", null, "There is no default authoritative domain or the domain name is empty.", ResultSeverityLevel.Warning, false);
					return null;
				}
				return new AcceptedDomainTable(internalDomains, acceptedDomainEntry, entries);
			}

			protected override ADOperationResult TryRegisterChangeNotification<TConfigObject>(Func<ADObjectId> rootIdGetter, out ADNotificationRequestCookie cookie)
			{
				return TransportADNotificationAdapter.TryRegisterNotifications(new Func<ADObjectId>(ConfigurationLoader<AcceptedDomainTable, AcceptedDomainTable.Builder>.Builder.GetFirstOrgContainerId), new ADNotificationCallback(base.Reload), new TransportADNotificationAdapter.TransportADNotificationRegister(TransportADNotificationAdapter.Instance.RegisterForAcceptedDomainNotifications), 3, out cookie);
			}

			private bool bridgehead;
		}
	}
}
