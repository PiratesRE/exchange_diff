using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.InfoWorker.Common.MailTips;
using Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetMailTips : SingleStepServiceCommand<GetMailTipsRequest, MailTipsResponseMessage[]>
	{
		public GetMailTips(CallContext callContext, GetMailTipsRequest request) : base(callContext, request)
		{
		}

		internal override Offer ExpectedOffer
		{
			get
			{
				return Offer.MailTips;
			}
		}

		internal override bool SupportsExternalUsers
		{
			get
			{
				return true;
			}
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new GetMailTipsResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		private static string MailTipTypesToXmlString(Microsoft.Exchange.InfoWorker.Common.MailTips.MailTipTypes value)
		{
			if (value == Microsoft.Exchange.InfoWorker.Common.MailTips.MailTipTypes.None)
			{
				return string.Empty;
			}
			string text = value.ToString();
			return text.Replace(",", string.Empty);
		}

		internal override ServiceResult<MailTipsResponseMessage[]> Execute()
		{
			ExTraceGlobals.GetMailTipsCallTracer.TraceDebug((long)this.GetHashCode(), "GetMailTips.Execute() Begin.");
			ServiceResult<MailTipsResponseMessage[]> result;
			try
			{
				base.RequireExchange14OrLater();
				this.GetParameters();
				ClientContext clientContext = this.GetClientContext();
				CachedOrganizationConfiguration andCheckConfiguration = this.GetAndCheckConfiguration(clientContext);
				ExTraceGlobals.GetMailTipsCallTracer.TraceDebug((long)this.GetHashCode(), "GetMailTips.Execute() Invoking MailTipsApi.GetMailTips (sendingAs = {0}, traceid = {1}, count = {2}, tipsRequested = {3}, lcid = {4}", new object[]
				{
					(null == this.sendingAs) ? "null" : this.sendingAs.ToString(),
					this.proxyAddresses.GetHashCode(),
					this.proxyAddresses.Length,
					this.tipsRequested,
					this.lcid
				});
				bool flag = false;
				GetMailTipsQuery getMailTipsQuery;
				try
				{
					base.CallerBudget.EndLocal();
					flag = true;
					getMailTipsQuery = new GetMailTipsQuery(this.GetHashCode(), clientContext, this.sendingAs, andCheckConfiguration, this.proxyAddresses, this.tipsRequested, this.lcid, base.CallerBudget, base.CallContext.HttpContext.Response);
				}
				finally
				{
					if (flag)
					{
						base.CallerBudget.StartLocal("GetMailTips.ServiceResult", default(TimeSpan));
					}
				}
				IEnumerable<MailTips> apiResults;
				try
				{
					ExTraceGlobals.GetMailTipsCallTracer.TraceDebug((long)this.GetHashCode(), "GetMailTips.Execute() Invoking GetMailTipsQuery.");
					apiResults = getMailTipsQuery.Execute();
					ExTraceGlobals.GetMailTipsCallTracer.TraceDebug((long)this.GetHashCode(), "GetMailTips.Execute() GetMailTipsQuery succeeded.");
				}
				catch (ClientDisconnectedException ex)
				{
					ExTraceGlobals.GetMailTipsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "GetMailTips.Execute() GetMailTipsQuery failed: {0}", ex.Message);
					ExceptionHandler<XmlNode>.HandleClientDisconnect();
					return null;
				}
				finally
				{
					getMailTipsQuery.RequestLogger.LogToResponse(base.CallContext.HttpContext.Response);
				}
				MailTipsResponseMessage[] value = this.CreateResponseMessages(apiResults);
				result = new ServiceResult<MailTipsResponseMessage[]>(value);
			}
			finally
			{
				ExTraceGlobals.GetMailTipsCallTracer.TraceDebug((long)this.GetHashCode(), "GetMailTips.Execute() End.");
			}
			return result;
		}

		private void GetParameters()
		{
			ExTraceGlobals.GetMailTipsCallTracer.TraceDebug((long)this.GetHashCode(), "GetMailTips.GetParameters() Begin.");
			if (base.Request.Recipients == null || base.Request.Recipients.Length == 0)
			{
				throw new ServiceArgumentException(CoreResources.IDs.MessageRecipientsArrayMustNotBeEmpty);
			}
			if (base.Request.Recipients.Length > 50)
			{
				throw new ServiceArgumentException((CoreResources.IDs)3113724054U);
			}
			this.sendingAs = ((base.Request.SendingAs == null) ? null : ProxyAddress.Parse(base.Request.SendingAs.RoutingType ?? string.Empty, base.Request.SendingAs.EmailAddress ?? string.Empty));
			ExTraceGlobals.GetMailTipsCallTracer.TraceDebug<ProxyAddress>((long)this.GetHashCode(), "GetMailTips.GetParameters() SendingAs: {0}", this.sendingAs);
			this.proxyAddresses = this.CreateRecipientProxyAddressArray();
			this.tipsRequested = (Microsoft.Exchange.InfoWorker.Common.MailTips.MailTipTypes)base.Request.MailTipsRequested;
			if (this.tipsRequested == Microsoft.Exchange.InfoWorker.Common.MailTips.MailTipTypes.AllUseThisForSerializationOnly)
			{
				this.tipsRequested = Microsoft.Exchange.InfoWorker.Common.MailTips.MailTipTypes.All;
			}
			ExTraceGlobals.GetMailTipsCallTracer.TraceDebug<Microsoft.Exchange.InfoWorker.Common.MailTips.MailTipTypes>((long)this.GetHashCode(), "GetMailTips.GetParameters() Tips requested: {0}", this.tipsRequested);
			if (EWSSettings.ClientCulture != null)
			{
				this.lcid = EWSSettings.ClientCulture.LCID;
				ExTraceGlobals.GetMailTipsCallTracer.TraceDebug<int>((long)this.GetHashCode(), "GetMailTips.GetParameters() LCID from request client culture: {0}", this.lcid);
				return;
			}
			if (base.CallContext.AccessingPrincipal != null && base.CallContext.AccessingPrincipal.PreferredCultures.Any<CultureInfo>())
			{
				this.lcid = base.CallContext.AccessingPrincipal.PreferredCultures.First<CultureInfo>().LCID;
				ExTraceGlobals.GetMailTipsCallTracer.TraceDebug<int>((long)this.GetHashCode(), "GetMailTips.GetParameters() LCID from accessing principal: {0}", this.lcid);
				return;
			}
			this.lcid = CultureInfo.InvariantCulture.LCID;
			ExTraceGlobals.GetMailTipsCallTracer.TraceDebug<int>((long)this.GetHashCode(), "GetMailTips.GetParameters() LCID from invarient culture: {0}", this.lcid);
		}

		private ClientContext GetClientContext()
		{
			ClientContext result;
			try
			{
				base.CallerBudget.EndLocal();
				string messageId = base.CallContext.MessageId ?? AvailabilityQuery.CreateNewMessageId();
				ExternalCallContext externalCallContext = base.CallContext as ExternalCallContext;
				ClientContext clientContext;
				if (externalCallContext != null)
				{
					clientContext = ClientContext.Create(externalCallContext.EmailAddress, externalCallContext.ExternalId, externalCallContext.WSSecurityHeader, externalCallContext.SharingSecurityHeader, externalCallContext.Budget, EWSSettings.RequestTimeZone, EWSSettings.ClientCulture, messageId);
					ExTraceGlobals.GetMailTipsCallTracer.TraceDebug<string, SmtpAddress>((long)this.GetHashCode(), "GetMailTips.GetClientContext() Created external client context {0} for address {1}", clientContext.GetType().Name, externalCallContext.EmailAddress);
				}
				else
				{
					clientContext = ClientContext.Create(base.CallContext.EffectiveCaller.ClientSecurityContext, base.CallContext.ADRecipientSessionContext.OrganizationId, base.CallContext.Budget, EWSSettings.RequestTimeZone, EWSSettings.ClientCulture, messageId);
					ExTraceGlobals.GetMailTipsCallTracer.TraceDebug<OrganizationId>((long)this.GetHashCode(), "GetMailTips.GetClientContext() Created internal client context for organization {0}", base.CallContext.ADRecipientSessionContext.OrganizationId);
				}
				result = clientContext;
			}
			catch (AuthzException ex)
			{
				ExTraceGlobals.GetMailTipsCallTracer.TraceError<AuthzException>((long)this.GetHashCode(), "GetMailTips.GetClientContext() encountered authz exception {0}", ex);
				throw new ServiceAccessDeniedException(ex);
			}
			finally
			{
				base.CallerBudget.StartLocal("GetMailTips.GetClientContext", default(TimeSpan));
			}
			return result;
		}

		private CachedOrganizationConfiguration GetAndCheckConfiguration(ClientContext clientContext)
		{
			ExTraceGlobals.GetMailTipsCallTracer.TraceDebug((long)this.GetHashCode(), "GetMailTips.GetAndCheckConfiguration() selecting forest wide configuration to read forest wide MailTips enabled flag first");
			OrganizationId organizationId = OrganizationId.ForestWideOrgId;
			CachedOrganizationConfiguration instance = CachedOrganizationConfiguration.GetInstance(organizationId, CachedOrganizationConfiguration.ConfigurationTypes.All);
			if (!instance.OrganizationConfiguration.Configuration.MailTipsAllTipsEnabled)
			{
				ExTraceGlobals.GetMailTipsCallTracer.TraceDebug((long)this.GetHashCode(), "GetMailTips.GetAndCheckConfiguration() MailTips disabled by forest wide (first) organization configuration");
				throw new MailTipsDisabledException();
			}
			ExTraceGlobals.GetMailTipsCallTracer.TraceDebug((long)this.GetHashCode(), "GetMailTips.GetAndCheckConfiguration() forest wide (first) MailTips are enabled");
			bool flag = base.CallContext.AvailabilityProxyRequestType != null && base.CallContext.AvailabilityProxyRequestType.Value == ProxyRequestType.CrossForest;
			if (!(clientContext is ExternalClientContext) && !flag)
			{
				organizationId = clientContext.OrganizationId;
				instance = CachedOrganizationConfiguration.GetInstance(organizationId, CachedOrganizationConfiguration.ConfigurationTypes.All);
				ExTraceGlobals.GetMailTipsCallTracer.TraceDebug((long)this.GetHashCode(), "GetMailTips.GetAndCheckConfiguration() setting the current organization id and current configuration to be clientContext.OrganizationId for internal non cross forest request.");
			}
			bool enabled = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Global.MultiTenancy.Enabled;
			ExTraceGlobals.GetMailTipsCallTracer.TraceDebug((long)this.GetHashCode(), "GetMailTips.GetAndCheckConfiguration() isInDatacenter={0}, IsForestWide={1}, IsNull={2}, OU={3}, CU={4}", new object[]
			{
				enabled,
				organizationId == OrganizationId.ForestWideOrgId,
				organizationId == null,
				(organizationId != null) ? ((organizationId.OrganizationalUnit != null) ? organizationId.OrganizationalUnit.ToCanonicalName() : null) : null,
				(organizationId != null) ? ((organizationId.ConfigurationUnit != null) ? organizationId.ConfigurationUnit.ToCanonicalName() : null) : null
			});
			if (enabled && (organizationId == null || organizationId == OrganizationId.ForestWideOrgId || organizationId.OrganizationalUnit == null || organizationId.ConfigurationUnit == null))
			{
				bool flag2 = false;
				if (this.sendingAs != null)
				{
					ExTraceGlobals.GetMailTipsCallTracer.TraceDebug<ProxyAddress>((long)this.GetHashCode(), "GetMailTips.GetAndCheckConfiguration() trying to scope organization based on sending as address {0}", this.sendingAs);
					if (this.TryScopeOrganizationBasedOnAddress(this.sendingAs, ref instance))
					{
						ExTraceGlobals.GetMailTipsCallTracer.TraceDebug<ProxyAddress>((long)this.GetHashCode(), "GetMailTips.GetAndCheckConfiguration() successfully scoped organization based on sending as address {0}", this.sendingAs);
						flag2 = true;
					}
				}
				if (!flag2)
				{
					ExTraceGlobals.GetMailTipsCallTracer.TraceDebug<ProxyAddress>((long)this.GetHashCode(), "GetMailTips.GetAndCheckConfiguration() failed to scope organization based on sending as address {0}, trying to scope based on recipients", this.sendingAs);
					foreach (ProxyAddress proxyAddress in this.proxyAddresses)
					{
						if (this.TryScopeOrganizationBasedOnAddress(proxyAddress, ref instance))
						{
							ExTraceGlobals.GetMailTipsCallTracer.TraceDebug<ProxyAddress>((long)this.GetHashCode(), "GetMailTips.GetAndCheckConfiguration() successfully scoped organization based on recipient address {0}", proxyAddress);
							flag2 = true;
							break;
						}
					}
				}
				if (!flag2)
				{
					ExTraceGlobals.GetMailTipsCallTracer.TraceError((long)this.GetHashCode(), "GetMailTips.GetAndCheckConfiguration() failed to scope organization based sending as and all recipient addresses, failing the GetMailTips call");
					throw new MailTipsDisabledException();
				}
			}
			return instance;
		}

		private ProxyAddress[] CreateRecipientProxyAddressArray()
		{
			ExTraceGlobals.GetMailTipsCallTracer.TraceDebug((long)this.GetHashCode(), "GetMailTips.CreateRecipientProxyAddressArray() Begin.");
			int num = base.Request.Recipients.Length;
			ProxyAddress[] array = new ProxyAddress[num];
			for (int i = 0; i < num; i++)
			{
				string addressString = base.Request.Recipients[i].EmailAddress ?? string.Empty;
				string prefixString = base.Request.Recipients[i].RoutingType ?? string.Empty;
				ProxyAddress proxyAddress = ProxyAddress.Parse(prefixString, addressString);
				array[i] = proxyAddress;
				ExTraceGlobals.GetMailTipsCallTracer.TraceDebug<ProxyAddress>((long)this.GetHashCode(), "GetMailTips.CreateRecipientProxyAddressArray() Caller provided {0}", proxyAddress);
			}
			ExTraceGlobals.GetMailTipsCallTracer.TraceDebug((long)this.GetHashCode(), "GetMailTips.CreateRecipientProxyAddressArray() End.");
			return array;
		}

		private MailTipsResponseMessage[] CreateResponseMessages(IEnumerable<MailTips> apiResults)
		{
			ExTraceGlobals.GetMailTipsCallTracer.TraceDebug((long)this.GetHashCode(), "GetMailTips.CreateResponseMessages() Begin.");
			int num = base.Request.Recipients.Length;
			MailTipsResponseMessage[] array = new MailTipsResponseMessage[num];
			int num2 = 0;
			foreach (MailTips tips in apiResults)
			{
				XmlElement xmlElement = ServiceXml.CreateElement(base.XmlDocument, "MailTips", "http://schemas.microsoft.com/exchange/services/2006/messages");
				this.PopulateMailTipsElement(xmlElement, tips);
				array[num2] = new MailTipsResponseMessage(ServiceResultCode.Success, null, xmlElement);
				num2++;
			}
			ExTraceGlobals.GetMailTipsCallTracer.TraceDebug((long)this.GetHashCode(), "GetMailTips.CreateResponseMessages() End.");
			return array;
		}

		private void PopulateMailTipsElement(XmlElement element, MailTips tips)
		{
			ExTraceGlobals.GetMailTipsCallTracer.TraceDebug<EmailAddress>((long)this.GetHashCode(), "GetMailTips.PopulateMailTipsElement() for {0}", tips.EmailAddress);
			this.AddRecipientAddress(element, tips.EmailAddress.Name, tips.EmailAddress.RoutingType, tips.EmailAddress.Address);
			this.AddElement(element, "PendingMailTips", GetMailTips.MailTipTypesToXmlString(tips.PendingMailTips));
			if (this.IncludeInResults(tips, Microsoft.Exchange.InfoWorker.Common.MailTips.MailTipTypes.OutOfOfficeMessage))
			{
				XmlElement parentElement = ServiceXml.CreateElement(element, "OutOfOffice", "http://schemas.microsoft.com/exchange/services/2006/types");
				XmlElement xmlElement = ServiceXml.CreateElement(parentElement, "ReplyBody", "http://schemas.microsoft.com/exchange/services/2006/types");
				this.AddElement(xmlElement, "Message", tips.OutOfOfficeMessage);
				if (!string.IsNullOrEmpty(tips.OutOfOfficeMessageLanguage))
				{
					xmlElement.SetAttribute("xml:lang", tips.OutOfOfficeMessageLanguage);
				}
				if (tips.OutOfOfficeDuration != null)
				{
					XmlElement parent = ServiceXml.CreateElement(parentElement, "Duration", "http://schemas.microsoft.com/exchange/services/2006/types");
					this.AddElement(parent, "StartTime", XmlConvert.ToString(tips.OutOfOfficeDuration.StartTime, XmlDateTimeSerializationMode.Utc));
					this.AddElement(parent, "EndTime", XmlConvert.ToString(tips.OutOfOfficeDuration.EndTime, XmlDateTimeSerializationMode.Utc));
				}
			}
			if (this.IncludeInResults(tips, Microsoft.Exchange.InfoWorker.Common.MailTips.MailTipTypes.MailboxFullStatus))
			{
				this.AddElement(element, "MailboxFull", XmlConvert.ToString(tips.MailboxFull));
			}
			if (this.IncludeInResults(tips, Microsoft.Exchange.InfoWorker.Common.MailTips.MailTipTypes.CustomMailTip))
			{
				this.AddElement(element, "CustomMailTip", tips.CustomMailTip);
			}
			if (this.IncludeInResults(tips, Microsoft.Exchange.InfoWorker.Common.MailTips.MailTipTypes.TotalMemberCount))
			{
				this.AddElement(element, "TotalMemberCount", tips.TotalMemberCount.ToString(CultureInfo.InvariantCulture));
			}
			if (this.IncludeInResults(tips, Microsoft.Exchange.InfoWorker.Common.MailTips.MailTipTypes.ExternalMemberCount))
			{
				this.AddElement(element, "ExternalMemberCount", tips.ExternalMemberCount.ToString(CultureInfo.InvariantCulture));
			}
			if (this.IncludeInResults(tips, Microsoft.Exchange.InfoWorker.Common.MailTips.MailTipTypes.MaxMessageSize))
			{
				this.AddElement(element, "MaxMessageSize", XmlConvert.ToString(tips.MaxMessageSize));
			}
			if (this.IncludeInResults(tips, Microsoft.Exchange.InfoWorker.Common.MailTips.MailTipTypes.DeliveryRestriction))
			{
				this.AddElement(element, "DeliveryRestricted", XmlConvert.ToString(tips.DeliveryRestricted));
			}
			if (this.IncludeInResults(tips, Microsoft.Exchange.InfoWorker.Common.MailTips.MailTipTypes.ModerationStatus))
			{
				this.AddElement(element, "IsModerated", XmlConvert.ToString(tips.IsModerated));
			}
			if (this.IncludeInResults(tips, Microsoft.Exchange.InfoWorker.Common.MailTips.MailTipTypes.InvalidRecipient))
			{
				this.AddElement(element, "InvalidRecipient", XmlConvert.ToString(tips.InvalidRecipient));
			}
			if (this.IncludeInResults(tips, Microsoft.Exchange.InfoWorker.Common.MailTips.MailTipTypes.Scope))
			{
				this.AddElement(element, "Scope", ((int)tips.Scope).ToString());
			}
		}

		private void AddRecipientAddress(XmlElement parent, string displayName, string routingType, string address)
		{
			XmlElement parent2 = ServiceXml.CreateElement(parent, "RecipientAddress", "http://schemas.microsoft.com/exchange/services/2006/types");
			this.AddElement(parent2, "Name", displayName);
			if (!string.IsNullOrEmpty(address))
			{
				this.AddElement(parent2, "EmailAddress", address);
			}
			if (!string.IsNullOrEmpty(routingType))
			{
				this.AddElement(parent2, "RoutingType", routingType);
			}
		}

		private void AddElement(XmlElement parent, string name, string value)
		{
			ServiceXml.CreateTextElement(parent, name, value, "http://schemas.microsoft.com/exchange/services/2006/types");
		}

		private bool IncludeInResults(MailTips tips, Microsoft.Exchange.InfoWorker.Common.MailTips.MailTipTypes type)
		{
			return (this.tipsRequested & type) != Microsoft.Exchange.InfoWorker.Common.MailTips.MailTipTypes.None && (tips.UnavailableMailTips & type) == Microsoft.Exchange.InfoWorker.Common.MailTips.MailTipTypes.None && (tips.PendingMailTips & type) == Microsoft.Exchange.InfoWorker.Common.MailTips.MailTipTypes.None;
		}

		private bool TryScopeOrganizationBasedOnAddress(ProxyAddress address, ref CachedOrganizationConfiguration configuration)
		{
			ExTraceGlobals.GetMailTipsCallTracer.TraceFunction<ProxyAddress>((long)this.GetHashCode(), "GetMailTips.ScopeOrganizationBasedOnAddress() enter, for address {0}", address);
			bool flag = false;
			SmtpProxyAddress smtpProxyAddress = address as SmtpProxyAddress;
			if (null == smtpProxyAddress)
			{
				ExTraceGlobals.GetMailTipsCallTracer.TraceDebug<ProxyAddress>((long)this.GetHashCode(), "GetMailTips.ScopeOrganizationBasedOnAddress() skipping over non SMTP proxy address {0} for external client context in datacenter tenant determination.", address);
			}
			else
			{
				SmtpAddress smtpAddress = (SmtpAddress)smtpProxyAddress.SmtpAddress;
				string domain = smtpAddress.Domain;
				ExTraceGlobals.GetMailTipsCallTracer.TraceDebug<string, SmtpAddress>((long)this.GetHashCode(), "GetMailTips.ScopeOrganizationBasedOnAddress() obtained domain {0} from smtp address {1}.", domain, smtpAddress);
				if (string.IsNullOrEmpty(domain))
				{
					ExTraceGlobals.GetMailTipsCallTracer.TraceDebug<SmtpProxyAddress>((long)this.GetHashCode(), "GetMailTips.ScopeOrganizationBasedOnAddress() skipping over SMTP proxy address {0} since domain in address was empty.", smtpProxyAddress);
				}
				else
				{
					OrganizationId organizationId = DomainToOrganizationIdCache.Singleton.Get(new SmtpDomain(domain));
					ExTraceGlobals.GetMailTipsCallTracer.TraceDebug((long)this.GetHashCode(), "GetMailTips.ScopeOrganizationBasedOnAddress() lookup domain {0} in DomainToOrganizationIdCache resulted in organization IsForestWide={1}, IsNull={2}, OU={3}, CU={4}.", new object[]
					{
						domain,
						organizationId == OrganizationId.ForestWideOrgId,
						organizationId == null,
						(organizationId != null) ? ((organizationId.OrganizationalUnit != null) ? organizationId.OrganizationalUnit.ToCanonicalName() : null) : null,
						(organizationId != null) ? ((organizationId.ConfigurationUnit != null) ? organizationId.ConfigurationUnit.ToCanonicalName() : null) : null
					});
					if (null == organizationId || organizationId == OrganizationId.ForestWideOrgId || organizationId.OrganizationalUnit == null || organizationId.ConfigurationUnit == null)
					{
						ExTraceGlobals.GetMailTipsCallTracer.TraceError<SmtpAddress, string>((long)this.GetHashCode(), "GetMailTips.ScopeOrganizationBasedOnAddress() didn't find a valid organization in DomainToOrganizationIdCache for recipient {0} with domain {1}, skipping the address.", smtpAddress, domain);
					}
					else
					{
						ExTraceGlobals.GetMailTipsCallTracer.TraceDebug<OrganizationId, SmtpAddress>((long)this.GetHashCode(), "GetMailTips.ScopeOrganizationBasedOnAddress() found recipient specific organization {0} for recipient {1}, loading configuration.", organizationId, smtpAddress);
						configuration = CachedOrganizationConfiguration.GetInstance(organizationId, CachedOrganizationConfiguration.ConfigurationTypes.All);
						if (!configuration.OrganizationConfiguration.Configuration.MailTipsAllTipsEnabled)
						{
							ExTraceGlobals.GetMailTipsCallTracer.TraceDebug<OrganizationId>((long)this.GetHashCode(), "GetMailTips.ScopeOrganizationBasedOnAddress() MailTips disabled by organization {0} configuration", configuration.OrganizationId);
							throw new MailTipsDisabledException();
						}
						flag = true;
					}
				}
			}
			ExTraceGlobals.GetMailTipsCallTracer.TraceFunction<bool, ProxyAddress>((long)this.GetHashCode(), "GetMailTips.ScopeOrganizationBasedOnAddress() exit, scoped={0} for address {1}", flag, address);
			return flag;
		}

		private const string XmlElementNameMailTips = "MailTips";

		private const string XmlElementNameRecipientAddress = "RecipientAddress";

		private const string XmlElementNamePendingMailTips = "PendingMailTips";

		private const string XmlElementNameOutOfOffice = "OutOfOffice";

		private const string XmlElementNameReplyBody = "ReplyBody";

		private const string XmlElementNameMessage = "Message";

		private const string XmlElementNameLanguageTag = "LanguageTag";

		private const string XmlElementNameDuration = "Duration";

		private const string XmlElementNameStartTime = "StartTime";

		private const string XmlElementNameEndTime = "EndTime";

		private const string XmlElementNameOutOfOfficeMessage = "OutOfOfficeMessage";

		private const string XmlElementNameMailboxFull = "MailboxFull";

		private const string XmlElementNameCustomMailTip = "CustomMailTip";

		private const string XmlElementNameTotalMemberCount = "TotalMemberCount";

		private const string XmlElementNameExternalMemberCount = "ExternalMemberCount";

		private const string XmlElementNameMaxMessageSize = "MaxMessageSize";

		private const string XmlElementNameDeliveryRestricted = "DeliveryRestricted";

		private const string XmlElementNameIsModerated = "IsModerated";

		private const string XmlElementNameInvalidRecipient = "InvalidRecipient";

		private const string XmlElementNameScope = "Scope";

		private const string XmlElementNameInvalidSmtpDomain = "InvalidSmtpDomain";

		private const string XmlAttributeNameXmlLang = "xml:lang";

		private ProxyAddress sendingAs;

		private ProxyAddress[] proxyAddresses;

		private Microsoft.Exchange.InfoWorker.Common.MailTips.MailTipTypes tipsRequested;

		private int lcid;
	}
}
