using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.InfoWorker.Common.MailTips;
using Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.MessagingPolicies.Rules.OutlookProtection;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.PolicyNudges;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.UM.ClientAccess;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class GetServiceConfiguration : SingleStepServiceCommand<GetServiceConfigurationRequest, ServiceConfigurationResponseMessage[]>
	{
		public GetServiceConfiguration(CallContext callContext, GetServiceConfigurationRequest request) : base(callContext, request)
		{
			this.traceId = this.GetHashCode();
		}

		private static ExchangePrincipal GetPrincipal(CallContext callContext, EmailAddressWrapper sendingAs)
		{
			ExchangePrincipal exchangePrincipal;
			if (sendingAs != null)
			{
				string emailAddress = sendingAs.EmailAddress;
				exchangePrincipal = ExchangePrincipalCache.GetFromCache(emailAddress, callContext.ADRecipientSessionContext);
			}
			else
			{
				exchangePrincipal = callContext.AccessingPrincipal;
				if (exchangePrincipal == null)
				{
					throw new NonExistentMailboxException((CoreResources.IDs)4088802584U, string.Empty);
				}
			}
			return exchangePrincipal;
		}

		internal override ServiceResult<ServiceConfigurationResponseMessage[]> Execute()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			ServiceResult<ServiceConfigurationResponseMessage[]> result;
			try
			{
				result = this.InternalExecute();
			}
			finally
			{
				stopwatch.Stop();
			}
			PerfCounterHelper.UpdateServiceConfigurationResponseTimePerformanceCounter(stopwatch.ElapsedMilliseconds);
			return result;
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			return new GetServiceConfigurationResponseMessage(base.Result.Code, base.Result.Error, base.Result.Value);
		}

		private ServiceResult<ServiceConfigurationResponseMessage[]> InternalExecute()
		{
			foreach (string value in base.Request.ConfigurationTypes)
			{
				if (!Enum.IsDefined(typeof(ServiceConfigurationType), value))
				{
					throw new ServiceArgumentException((CoreResources.IDs)3640136612U);
				}
				ServiceConfigurationType serviceConfigurationType = (ServiceConfigurationType)Enum.Parse(typeof(ServiceConfigurationType), value);
				this.configurationTypes |= serviceConfigurationType;
			}
			ExTraceGlobals.GetOrganizationConfigurationCallTracer.TraceDebug<ServiceConfigurationType>((long)this.GetHashCode(), "Getting organization configuration {0}", this.configurationTypes);
			CachedOrganizationConfiguration cachedOrganizationConfiguration = null;
			if (this.CallerRequested(ServiceConfigurationType.UnifiedMessagingConfiguration))
			{
				if (base.Request.ActingAs != null)
				{
					throw new ServiceArgumentException((CoreResources.IDs)2476021338U);
				}
				this.unifiedMessagingPrincipal = base.CallContext.AccessingPrincipal;
				if (this.unifiedMessagingPrincipal == null)
				{
					throw new NonExistentMailboxException((CoreResources.IDs)4088802584U, string.Empty);
				}
			}
			if (this.CallerRequested(ServiceConfigurationType.MailTips | ServiceConfigurationType.ProtectionRules | ServiceConfigurationType.PolicyNudges))
			{
				ExTraceGlobals.GetOrganizationConfigurationCallTracer.TraceDebug((long)this.GetHashCode(), "Getting organization configuration instance");
				this.GetActor();
				OrganizationId organizationId = (OrganizationId)this.actingAsRecipient[ADObjectSchema.OrganizationId];
				cachedOrganizationConfiguration = CachedOrganizationConfiguration.GetInstance(organizationId, CachedOrganizationConfiguration.ConfigurationTypes.All);
			}
			List<XmlElement> list = new List<XmlElement>();
			if (this.CallerRequested(ServiceConfigurationType.MailTips))
			{
				MailTipsConfiguration mailTipsConfiguration = new MailTipsConfiguration(this.traceId);
				mailTipsConfiguration.Initialize(cachedOrganizationConfiguration, this.actingAsRecipient);
				XmlElement item = this.SerializeMailTipsConfiguration(cachedOrganizationConfiguration.OrganizationConfiguration.Configuration.MailTipsAllTipsEnabled, mailTipsConfiguration, cachedOrganizationConfiguration.Domains);
				list.Add(item);
			}
			if (this.CallerRequested(ServiceConfigurationType.UnifiedMessagingConfiguration))
			{
				XmlElement item2;
				using (UMClientCommon umclientCommon = new UMClientCommon(this.unifiedMessagingPrincipal))
				{
					if (!umclientCommon.IsUMEnabled())
					{
						item2 = this.SerializeUnifiedMessageConfiguration(false, new UMPropertiesEx
						{
							PlayOnPhoneDialString = string.Empty,
							PlayOnPhoneEnabled = false
						});
					}
					else
					{
						UMPropertiesEx umproperties = umclientCommon.GetUMProperties();
						item2 = this.SerializeUnifiedMessageConfiguration(true, umproperties);
					}
				}
				list.Add(item2);
			}
			if (this.CallerRequested(ServiceConfigurationType.ProtectionRules))
			{
				XmlElement item3 = this.SerializeProtectionRulesConfiguration(cachedOrganizationConfiguration.ProtectionRules, cachedOrganizationConfiguration.Domains);
				list.Add(item3);
			}
			if (this.CallerRequested(ServiceConfigurationType.PolicyNudges))
			{
				using (XmlNodeReader xmlNodeReader = new XmlNodeReader(base.Request.ConfigurationRequestDetails))
				{
					xmlNodeReader.MoveToContent();
					PolicyNudgeConfiguration policyNudgeConfiguration = PolicyNudgeConfigurationFactory.Create();
					XElement root = XDocument.Load(xmlNodeReader).Root;
					foreach (XElement xelement in root.DescendantsAndSelf())
					{
						xelement.Name = XNamespace.None.GetName(xelement.Name.LocalName);
						xelement.ReplaceAttributes(xelement.Attributes().Select(delegate(XAttribute a)
						{
							if (!a.IsNamespaceDeclaration)
							{
								return new XAttribute(XNamespace.None.GetName(a.Name.LocalName), a.Value);
							}
							return null;
						}));
					}
					list.Add(policyNudgeConfiguration.SerializeConfiguration(root.Element("PolicyNudges"), cachedOrganizationConfiguration, this.actingAsRecipient.Id, base.XmlDocument));
				}
			}
			return this.CreateResultArray(list);
		}

		private void GetActor()
		{
			if (base.Request.ActingAs == null)
			{
				throw new ServiceArgumentException(CoreResources.IDs.ErrorActingAsRequired);
			}
			if (string.IsNullOrEmpty(base.Request.ActingAs.RoutingType))
			{
				throw new ServiceArgumentException((CoreResources.IDs)2292082652U);
			}
			if (string.IsNullOrEmpty(base.Request.ActingAs.EmailAddress))
			{
				throw new ServiceArgumentException((CoreResources.IDs)3538999938U);
			}
			this.actingAsAddress = ProxyAddress.Parse(base.Request.ActingAs.RoutingType ?? string.Empty, base.Request.ActingAs.EmailAddress ?? string.Empty);
			if (this.actingAsAddress is InvalidProxyAddress)
			{
				throw new ServiceArgumentException((CoreResources.IDs)2886782397U);
			}
			try
			{
				this.transportPrincipal = GetServiceConfiguration.GetPrincipal(base.CallContext, base.Request.ActingAs);
				ExTraceGlobals.GetOrganizationConfigurationCallTracer.TraceDebug<ProxyAddress>((long)this.GetHashCode(), "Looking up the actor ", this.actingAsAddress);
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(this.transportPrincipal.MailboxInfo.OrganizationId), 387, "GetActor", "f:\\15.00.1497\\sources\\dev\\services\\src\\Core\\servicecommands\\GetServiceConfiguration.cs");
				this.actingAsRecipient = MailTipsUtility.GetSender(tenantOrRootOrgRecipientSession, this.actingAsAddress, this.callerProperties);
			}
			catch (SenderNotUniqueException)
			{
				throw new ServiceArgumentException(CoreResources.IDs.ErrorActingAsUserNotUnique);
			}
			catch (SenderNotFoundException)
			{
				throw new ServiceArgumentException(CoreResources.IDs.ErrorActingAsUserNotFound);
			}
			catch (ServicePermanentException)
			{
				throw new ServiceArgumentException(CoreResources.IDs.ErrorActingAsUserNotFound);
			}
		}

		private bool CallerRequested(ServiceConfigurationType type)
		{
			return (this.configurationTypes & type) != ServiceConfigurationType.None;
		}

		private ServiceResult<ServiceConfigurationResponseMessage[]> CreateResultArray(List<XmlElement> responseElements)
		{
			ServiceConfigurationResponseMessage serviceConfigurationResponseMessage = new ServiceConfigurationResponseMessage(ServiceResultCode.Success, null, responseElements.ToArray());
			ServiceConfigurationResponseMessage[] value = new ServiceConfigurationResponseMessage[]
			{
				serviceConfigurationResponseMessage
			};
			return new ServiceResult<ServiceConfigurationResponseMessage[]>(value);
		}

		private string GetOutlookDateTimeString(DateTime dt)
		{
			return dt.ToString("yyyy-MM-dd") + "T" + dt.ToString("HH:mm:ss") + "Z";
		}

		private XmlElement SerializeMailTipsConfiguration(bool mailTipsEnabled, MailTipsConfiguration mailTipsConfiguration, OrganizationDomains domains)
		{
			XmlElement xmlElement = ServiceXml.CreateElement(base.XmlDocument, "MailTipsConfiguration", "http://schemas.microsoft.com/exchange/services/2006/messages");
			ServiceXml.CreateTextElement(xmlElement, "MailTipsEnabled", XmlConvert.ToString(mailTipsEnabled), "http://schemas.microsoft.com/exchange/services/2006/types");
			ServiceXml.CreateTextElement(xmlElement, "MaxRecipientsPerGetMailTipsRequest", XmlConvert.ToString(50), "http://schemas.microsoft.com/exchange/services/2006/types");
			ServiceXml.CreateTextElement(xmlElement, "MaxMessageSize", XmlConvert.ToString(mailTipsConfiguration.MaxMessageSize), "http://schemas.microsoft.com/exchange/services/2006/types");
			ServiceXml.CreateTextElement(xmlElement, "LargeAudienceThreshold", XmlConvert.ToString(mailTipsConfiguration.LargeAudienceThreshold), "http://schemas.microsoft.com/exchange/services/2006/types");
			ServiceXml.CreateTextElement(xmlElement, "ShowExternalRecipientCount", XmlConvert.ToString(mailTipsConfiguration.ShowExternalRecipientCount), "http://schemas.microsoft.com/exchange/services/2006/types");
			XmlElement parentElement = ServiceXml.CreateElement(xmlElement, "InternalDomains", "http://schemas.microsoft.com/exchange/services/2006/types");
			foreach (SmtpDomainWithSubdomains smtpDomainWithSubdomains in domains.InternalDomains)
			{
				XmlElement xmlElement2 = ServiceXml.CreateTextElement(parentElement, "Domain", string.Empty, "http://schemas.microsoft.com/exchange/services/2006/types");
				xmlElement2.SetAttribute("Name", smtpDomainWithSubdomains.Domain);
				xmlElement2.SetAttribute("IncludeSubdomains", XmlConvert.ToString(smtpDomainWithSubdomains.IncludeSubDomains));
			}
			if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2012))
			{
				ServiceXml.CreateTextElement(xmlElement, "PolicyTipsEnabled", XmlConvert.ToString(mailTipsConfiguration.PolicyTipsEnabled), "http://schemas.microsoft.com/exchange/services/2006/types");
				ServiceXml.CreateTextElement(xmlElement, "LargeAudienceCap", XmlConvert.ToString(1000), "http://schemas.microsoft.com/exchange/services/2006/types");
			}
			return xmlElement;
		}

		private XmlElement SerializeUnifiedMessageConfiguration(bool umEnabled, UMPropertiesEx umConfiguration)
		{
			XmlElement xmlElement = ServiceXml.CreateElement(base.XmlDocument, "UnifiedMessagingConfiguration", "http://schemas.microsoft.com/exchange/services/2006/messages");
			ServiceXml.CreateTextElement(xmlElement, "UmEnabled", XmlConvert.ToString(umEnabled), "http://schemas.microsoft.com/exchange/services/2006/types");
			ServiceXml.CreateTextElement(xmlElement, "PlayOnPhoneDialString", umConfiguration.PlayOnPhoneDialString, "http://schemas.microsoft.com/exchange/services/2006/types");
			ServiceXml.CreateTextElement(xmlElement, "PlayOnPhoneEnabled", XmlConvert.ToString(umConfiguration.PlayOnPhoneEnabled), "http://schemas.microsoft.com/exchange/services/2006/types");
			return xmlElement;
		}

		private XmlElement SerializeProtectionRulesConfiguration(IEnumerable<OutlookProtectionRule> rules, OrganizationDomains domains)
		{
			XmlElement xmlElement = ServiceXml.CreateElement(base.XmlDocument, "ProtectionRulesConfiguration", "http://schemas.microsoft.com/exchange/services/2006/messages");
			xmlElement.SetAttribute("RefreshInterval", XmlConvert.ToString(24));
			GetServiceConfiguration.ProtectionRulesSerializer.SerializeRules(xmlElement, rules);
			XmlElement parentElement = ServiceXml.CreateElement(xmlElement, "InternalDomains", "http://schemas.microsoft.com/exchange/services/2006/types");
			string domain = this.transportPrincipal.MailboxInfo.PrimarySmtpAddress.Domain;
			bool flag = false;
			if (rules != null)
			{
				foreach (OutlookProtectionRule outlookProtectionRule in rules)
				{
					if (outlookProtectionRule.Enabled == RuleState.Enabled)
					{
						flag = true;
						break;
					}
				}
			}
			foreach (SmtpDomainWithSubdomains smtpDomainWithSubdomains in domains.InternalDomains)
			{
				if (flag || string.Compare(smtpDomainWithSubdomains.Address, domain, StringComparison.OrdinalIgnoreCase) == 0)
				{
					XmlElement xmlElement2 = ServiceXml.CreateTextElement(parentElement, "Domain", string.Empty, "http://schemas.microsoft.com/exchange/services/2006/types");
					xmlElement2.SetAttribute("Name", smtpDomainWithSubdomains.Domain);
					xmlElement2.SetAttribute("IncludeSubdomains", XmlConvert.ToString(smtpDomainWithSubdomains.IncludeSubDomains));
				}
			}
			return xmlElement;
		}

		private const string XmlElementNameServiceConfiguration = "ServiceConfiguration";

		private const string XmlElementNameMailTipsEnabled = "MailTipsEnabled";

		private const string XmlElementNameMailTipsConfiguration = "MailTipsConfiguration";

		private const string XmlElementNameMaxRecipientsPerGetMailTipsRequest = "MaxRecipientsPerGetMailTipsRequest";

		private const string XmlElementNameMaxMessageSize = "MaxMessageSize";

		private const string XmlElementNameLargeAudienceThreshold = "LargeAudienceThreshold";

		private const string XmlElementNameLargeAudienceCap = "LargeAudienceCap";

		private const string XmlElementNameShowExternalRecipientCount = "ShowExternalRecipientCount";

		private const string XmlElementNamePolicyTipsEnabled = "PolicyTipsEnabled";

		private const string XmlElementNameInternalDomains = "InternalDomains";

		private const string XmlElementNameDomain = "Domain";

		private const string XmlAttributeNameName = "Name";

		private const string XmlAttributeNameIncludeSubdomains = "IncludeSubdomains";

		private const string XmlElementNameUmEnabled = "UmEnabled";

		private const string XmlElementNamePlayOnPhoneDialString = "PlayOnPhoneDialString";

		private const string XmlElementNamePlayOnPhoneEnabled = "PlayOnPhoneEnabled";

		private const string XmlElementNameProtectionRulesConfiguration = "ProtectionRulesConfiguration";

		private const string XmlAttributeNameRefreshInterval = "RefreshInterval";

		internal const string XmlElementNameUnifiedMessagingConfiguration = "UnifiedMessagingConfiguration";

		private const string XmlElementNamePolicyNudges = "PolicyNudges";

		private readonly ADPropertyDefinition[] callerProperties = new ADPropertyDefinition[]
		{
			ADObjectSchema.OrganizationId,
			ADRecipientSchema.MaxSendSize,
			ADObjectSchema.Id
		};

		private ProxyAddress actingAsAddress;

		private ADRawEntry actingAsRecipient;

		private ServiceConfigurationType configurationTypes;

		private ExchangePrincipal transportPrincipal;

		private ExchangePrincipal unifiedMessagingPrincipal;

		private int traceId;

		private static class ProtectionRulesSerializer
		{
			public static void SerializeRules(XmlElement parent, IEnumerable<OutlookProtectionRule> rules)
			{
				XmlElement parent2 = ServiceXml.CreateElement(parent, "Rules", "http://schemas.microsoft.com/exchange/services/2006/types");
				if (rules == null)
				{
					return;
				}
				int num = 1;
				foreach (OutlookProtectionRule outlookProtectionRule in rules)
				{
					if (outlookProtectionRule.Enabled == RuleState.Enabled)
					{
						GetServiceConfiguration.ProtectionRulesSerializer.SerializeRule(parent2, outlookProtectionRule, num);
						num++;
					}
				}
			}

			private static void SerializeRule(XmlElement parent, OutlookProtectionRule rule, int priority)
			{
				XmlElement xmlElement = ServiceXml.CreateElement(parent, "Rule", "http://schemas.microsoft.com/exchange/services/2006/types");
				xmlElement.SetAttribute("Name", rule.Name);
				xmlElement.SetAttribute("Priority", XmlConvert.ToString(priority));
				xmlElement.SetAttribute("UserOverridable", XmlConvert.ToString(rule.UserOverridable));
				GetServiceConfiguration.ProtectionRulesSerializer.SerializeCondition(xmlElement, rule);
				GetServiceConfiguration.ProtectionRulesSerializer.SerializeAction(xmlElement, rule);
			}

			private static void SerializeCondition(XmlElement parent, OutlookProtectionRule rule)
			{
				XmlElement parentElement = ServiceXml.CreateElement(parent, "Condition", "http://schemas.microsoft.com/exchange/services/2006/types");
				XmlElement xmlElement = ServiceXml.CreateElement(parentElement, "And", "http://schemas.microsoft.com/exchange/services/2006/types");
				GetServiceConfiguration.ProtectionRulesSerializer.SerializeAllInternalPredicate(xmlElement, rule);
				XmlElement parent2 = ServiceXml.CreateElement(xmlElement, "And", "http://schemas.microsoft.com/exchange/services/2006/types");
				GetServiceConfiguration.ProtectionRulesSerializer.SerializeRecipientIsPredicate(parent2, rule);
				GetServiceConfiguration.ProtectionRulesSerializer.SerializeSenderDepartmentPredicate(parent2, rule);
			}

			private static void SerializeAllInternalPredicate(XmlElement parent, OutlookProtectionRule rule)
			{
				if (rule.GetAllInternalPredicate() == null)
				{
					ServiceXml.CreateElement(parent, "True", "http://schemas.microsoft.com/exchange/services/2006/types");
					return;
				}
				ServiceXml.CreateElement(parent, "AllInternal", "http://schemas.microsoft.com/exchange/services/2006/types");
			}

			private static void SerializeRecipientIsPredicate(XmlElement parent, OutlookProtectionRule rule)
			{
				PredicateCondition recipientIsPredicate = rule.GetRecipientIsPredicate();
				if (recipientIsPredicate == null)
				{
					ServiceXml.CreateElement(parent, "True", "http://schemas.microsoft.com/exchange/services/2006/types");
					return;
				}
				XmlElement parent2 = ServiceXml.CreateElement(parent, "RecipientIs", "http://schemas.microsoft.com/exchange/services/2006/types");
				GetServiceConfiguration.ProtectionRulesSerializer.SerializeValues(parent2, recipientIsPredicate.Value);
			}

			private static void SerializeSenderDepartmentPredicate(XmlElement parent, OutlookProtectionRule rule)
			{
				PredicateCondition senderDepartmentPredicate = rule.GetSenderDepartmentPredicate();
				if (senderDepartmentPredicate == null)
				{
					ServiceXml.CreateElement(parent, "True", "http://schemas.microsoft.com/exchange/services/2006/types");
					return;
				}
				XmlElement parent2 = ServiceXml.CreateElement(parent, "SenderDepartments", "http://schemas.microsoft.com/exchange/services/2006/types");
				GetServiceConfiguration.ProtectionRulesSerializer.SerializeValues(parent2, senderDepartmentPredicate.Value);
			}

			private static void SerializeValues(XmlElement parent, Value values)
			{
				if (values == null)
				{
					return;
				}
				foreach (string textValue in values.RawValues)
				{
					ServiceXml.CreateTextElement(parent, "Value", textValue, "http://schemas.microsoft.com/exchange/services/2006/types");
				}
			}

			private static void SerializeAction(XmlElement parent, OutlookProtectionRule rule)
			{
				RightsProtectMessageAction rightsProtectMessageAction = rule.GetRightsProtectMessageAction();
				if (rightsProtectMessageAction == null)
				{
					return;
				}
				XmlElement xmlElement = ServiceXml.CreateElement(parent, "Action", "http://schemas.microsoft.com/exchange/services/2006/types");
				xmlElement.SetAttribute("Name", "RightsProtectMessage");
				XmlElement xmlElement2 = ServiceXml.CreateElement(xmlElement, "Argument", "http://schemas.microsoft.com/exchange/services/2006/types");
				xmlElement2.SetAttribute("Value", rightsProtectMessageAction.TemplateId);
			}

			private static class ElementNames
			{
				public const string Action = "Action";

				public const string AllInternal = "AllInternal";

				public const string And = "And";

				public const string Argument = "Argument";

				public const string Condition = "Condition";

				public const string RecipientIs = "RecipientIs";

				public const string Rule = "Rule";

				public const string Rules = "Rules";

				public const string SenderDepartments = "SenderDepartments";

				public const string True = "True";

				public const string Value = "Value";
			}

			private static class AttributeNames
			{
				public const string Name = "Name";

				public const string Priority = "Priority";

				public const string UserOverridable = "UserOverridable";

				public const string Value = "Value";
			}

			private static class ActionNames
			{
				public const string RightsProtectMessage = "RightsProtectMessage";
			}
		}
	}
}
