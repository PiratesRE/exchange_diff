using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.RightsManagement;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Deployment;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.CompliancePrograms.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	internal class Utils
	{
		internal static string RuleCollectionNameFromRole()
		{
			if (!Utils.IsEdgeRoleInstalled())
			{
				return "TransportVersioned";
			}
			return "Edge";
		}

		internal static bool IsEdgeRoleInstalled()
		{
			return new GatewayRole().IsInstalled;
		}

		internal static ArgumentException ValidateEnabledConnectorById(ref OutboundConnectorIdParameter connectorIdentity, IConfigDataProvider session)
		{
			if (VariantConfiguration.InvariantNoFlightingSnapshot.CompliancePolicy.ValidateTenantOutboundConnector.Enabled)
			{
				IEnumerable<TenantOutboundConnector> objects = connectorIdentity.GetObjects<TenantOutboundConnector>(null, session);
				if (objects == null)
				{
					return new ArgumentException(Strings.OutboundConnectorIdNotFound(connectorIdentity.ToString()));
				}
				TenantOutboundConnector[] array = objects.ToArray<TenantOutboundConnector>();
				if (array.Length == 0)
				{
					return new ArgumentException(Strings.OutboundConnectorIdNotFound(connectorIdentity.ToString()));
				}
				if (array.Length > 1)
				{
					return new ArgumentException(Strings.ErrorObjectNotUnique(connectorIdentity.ToString()));
				}
				TenantOutboundConnector tenantOutboundConnector = array[0];
				if (!tenantOutboundConnector.Enabled)
				{
					return new ArgumentException(Strings.ErrorConnectorNotEnabled(tenantOutboundConnector.Name));
				}
				if (!tenantOutboundConnector.IsTransportRuleScoped)
				{
					return new ArgumentException(Strings.ErrorConnectorNotTransportRuleScoped(tenantOutboundConnector.Name));
				}
				connectorIdentity = OutboundConnectorIdParameter.Parse(tenantOutboundConnector.Name);
			}
			return null;
		}

		internal static bool IsRegexExecutionTimeWithinLimit(string regexPattern, long cpuTimeLimitInMs, bool failCheckTestHook = false)
		{
			if (string.IsNullOrEmpty(regexPattern))
			{
				throw new ArgumentNullException("regexPattern");
			}
			if (cpuTimeLimitInMs <= 0L)
			{
				throw new ArgumentException("cpuTimeLimitInMs");
			}
			if (failCheckTestHook)
			{
				return false;
			}
			string testContent = Utils.LoadTestContentForRegexPerformanceTest();
			return Utils.TimeRegexAgainstTestContent(regexPattern, testContent) < cpuTimeLimitInMs;
		}

		internal static bool TryGetTransportRules(IConfigDataProvider session, Utils.TransportRuleSelectionDelegate selectionDelegate, out IEnumerable<TransportRule> rules, object delegateContext = null)
		{
			IEnumerable<TransportRule> rules2;
			string text;
			if (!DlpUtils.TryGetTransportRules(session, out rules2, out text))
			{
				rules = null;
				return false;
			}
			rules = Utils.GetTransportRulesMatchingDelegate(rules2, selectionDelegate, delegateContext);
			return true;
		}

		internal static IEnumerable<TransportRule> GetTransportRulesMatchingDelegate(IConfigDataProvider session, Utils.TransportRuleSelectionDelegate selectionDelegate, object delegateContext = null)
		{
			IEnumerable<TransportRule> rules;
			string message;
			if (!DlpUtils.TryGetTransportRules(session, out rules, out message))
			{
				throw new InvalidOperationException(message);
			}
			return Utils.GetTransportRulesMatchingDelegate(rules, selectionDelegate, delegateContext);
		}

		private static IEnumerable<TransportRule> GetTransportRulesMatchingDelegate(IEnumerable<TransportRule> rules, Utils.TransportRuleSelectionDelegate selectionDelegate, object delegateContext = null)
		{
			foreach (TransportRule tr in rules)
			{
				Rule rule = null;
				try
				{
					rule = TransportRuleParser.Instance.GetRule(tr.Xml);
				}
				catch (ParserException)
				{
				}
				catch (XmlException)
				{
				}
				if (rule != null && selectionDelegate(rule, delegateContext))
				{
					yield return tr;
				}
			}
			yield break;
		}

		internal static bool RuleHasOutboundConnectorReference(Rule transportRule, object previousConnectorNameAsObject)
		{
			string a = previousConnectorNameAsObject as string;
			foreach (Action action in transportRule.Actions)
			{
				if (action is RouteMessageOutboundConnector)
				{
					RouteMessageOutboundConnectorAction routeMessageOutboundConnectorAction = RouteMessageOutboundConnectorAction.CreateFromInternalAction(action) as RouteMessageOutboundConnectorAction;
					if (routeMessageOutboundConnectorAction != null && string.Equals(a, routeMessageOutboundConnectorAction.ConnectorName, StringComparison.OrdinalIgnoreCase))
					{
						return true;
					}
				}
			}
			return false;
		}

		internal static ADObjectId GetClassificationADObjectId(string classificationId, IConfigDataProvider session)
		{
			if (string.IsNullOrEmpty(classificationId) || session == null)
			{
				return null;
			}
			MessageClassificationIdParameter messageClassificationIdParameter;
			try
			{
				messageClassificationIdParameter = MessageClassificationIdParameter.Parse(classificationId);
			}
			catch (ArgumentNullException)
			{
				messageClassificationIdParameter = null;
			}
			catch (ArgumentException)
			{
				messageClassificationIdParameter = null;
			}
			ADObjectId rootId = MessageClassificationIdParameter.DefaultRoot(session);
			if (messageClassificationIdParameter != null)
			{
				IEnumerable<MessageClassification> objects = messageClassificationIdParameter.GetObjects<MessageClassification>(rootId, session);
				using (IEnumerator<MessageClassification> enumerator = objects.GetEnumerator())
				{
					if (enumerator.MoveNext() && enumerator.Current != null)
					{
						return (ADObjectId)enumerator.Current.Identity;
					}
				}
			}
			Guid guid;
			try
			{
				guid = new Guid(classificationId);
			}
			catch (FormatException)
			{
				return null;
			}
			catch (OverflowException)
			{
				return null;
			}
			bool isSharedConfigChecked = ((IConfigurationSession)session).SessionSettings.IsSharedConfigChecked;
			bool isRedirectedToSharedConfig = ((IConfigurationSession)session).SessionSettings.IsRedirectedToSharedConfig;
			((IConfigurationSession)session).SessionSettings.IsSharedConfigChecked = true;
			((IConfigurationSession)session).SessionSettings.IsRedirectedToSharedConfig = false;
			ADObjectId result;
			try
			{
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ClassificationSchema.ClassificationID, guid);
				MessageClassification[] array = ((IConfigurationSession)session).Find<MessageClassification>(rootId, QueryScope.SubTree, filter, null, 1);
				if (array == null || array.Length != 1)
				{
					result = null;
				}
				else
				{
					result = (ADObjectId)array[0].Identity;
				}
			}
			finally
			{
				((IConfigurationSession)session).SessionSettings.IsSharedConfigChecked = isSharedConfigChecked;
				((IConfigurationSession)session).SessionSettings.IsRedirectedToSharedConfig = isRedirectedToSharedConfig;
			}
			return result;
		}

		internal static string GetClassificationId(ADObjectId adObjectId, IConfigDataProvider session)
		{
			if (adObjectId == null)
			{
				return null;
			}
			MessageClassification messageClassification = (MessageClassification)session.Read<MessageClassification>(adObjectId);
			if (messageClassification == null || messageClassification.ClassificationID == Guid.Empty)
			{
				return null;
			}
			return messageClassification.ClassificationID.ToString();
		}

		internal static string GetClassificationDisplayName(ADObjectId adObjectId, IConfigDataProvider session)
		{
			if (adObjectId == null)
			{
				return string.Empty;
			}
			MessageClassification messageClassification = (MessageClassification)session.Read<MessageClassification>(adObjectId);
			if (messageClassification == null || string.IsNullOrEmpty(messageClassification.DisplayName))
			{
				return string.Empty;
			}
			return messageClassification.DisplayName;
		}

		internal static bool IsValidModerator(string smtpAddress, IRecipientSession recipientSession)
		{
			MailEnabledRecipient recipientMailEnabledRecipient = Utils.GetRecipientMailEnabledRecipient(smtpAddress, recipientSession);
			return recipientMailEnabledRecipient != null && (recipientMailEnabledRecipient is MailContact || recipientMailEnabledRecipient is Mailbox || recipientMailEnabledRecipient is MailUser);
		}

		internal static MailEnabledRecipient GetRecipientMailEnabledRecipient(string smtpAddress, IRecipientSession recipientSession)
		{
			ProxyAddress proxyAddress = ProxyAddress.Parse("smtp:" + smtpAddress);
			ADRecipient adrecipient;
			try
			{
				adrecipient = recipientSession.FindByProxyAddress(proxyAddress);
			}
			catch (NonUniqueRecipientException)
			{
				return null;
			}
			if (adrecipient == null)
			{
				return null;
			}
			return Utils.GetRecipientMailEnabledRecipient(adrecipient);
		}

		internal static void GetOrgTransportRuleLimits(OrganizationId orgId, IConfigurationSession configSession, out int transportRuleCollectionAddedRecipientsLimit, out int transportRuleLimit, out ByteQuantifiedSize transportRuleCollectionRegexCharsLimit, out ByteQuantifiedSize transportRuleSizeLimit)
		{
			if (!(orgId != OrganizationId.ForestWideOrgId))
			{
				transportRuleCollectionAddedRecipientsLimit = int.MaxValue;
				transportRuleLimit = int.MaxValue;
				transportRuleCollectionRegexCharsLimit = ByteQuantifiedSize.FromBytes(2147483647UL);
				transportRuleSizeLimit = ByteQuantifiedSize.FromBytes(2147483647UL);
				return;
			}
			TransportConfigContainer transportConfigContainer = configSession.Find<TransportConfigContainer>(orgId.ConfigurationUnit, QueryScope.SubTree, null, null, 1).FirstOrDefault<TransportConfigContainer>();
			if (transportConfigContainer == null)
			{
				throw new ArgumentException(Strings.ErrorAccessingTransportSettings);
			}
			transportRuleCollectionAddedRecipientsLimit = transportConfigContainer.TransportRuleCollectionAddedRecipientsLimit;
			transportRuleLimit = transportConfigContainer.TransportRuleLimit;
			transportRuleCollectionRegexCharsLimit = transportConfigContainer.TransportRuleCollectionRegexCharsLimit;
			transportRuleSizeLimit = transportConfigContainer.TransportRuleSizeLimit;
		}

		internal static InvalidOperationException CheckRuleForOrganizationLimits(IConfigurationSession dataSession, IRecipientSession recipientSession, ADRuleStorageManager ruleStorageManager, OrganizationId orgId, TransportRule internalRule, bool isNewRule)
		{
			int num;
			int num2;
			ByteQuantifiedSize byteQuantifiedSize;
			ByteQuantifiedSize byteQuantifiedSize2;
			Utils.GetOrgTransportRuleLimits(orgId, dataSession, out num, out num2, out byteQuantifiedSize, out byteQuantifiedSize2);
			if (isNewRule)
			{
				if (ruleStorageManager.Count >= num2)
				{
					return new InvalidOperationException(Strings.ErrorTooManyTransportRules(num2));
				}
			}
			else if (ruleStorageManager.Count > num2)
			{
				return new InvalidOperationException(Strings.ErrorTooManyTransportRules(num2));
			}
			string text = TransportRuleSerializer.Instance.SaveRuleToString(internalRule);
			if (text.Length > (int)byteQuantifiedSize2.ToBytes())
			{
				return new InvalidOperationException(Strings.ErrorRuleXmlTooBig(text.Length, (long)byteQuantifiedSize2.ToBytes()));
			}
			int num3 = 0;
			foreach (Rule rule in ruleStorageManager.GetRuleCollection())
			{
				TransportRule transportRule = (TransportRule)rule;
				if (!transportRule.Name.Equals(internalRule.Name, StringComparison.InvariantCulture))
				{
					num3 += transportRule.RegexCharacterCount;
				}
			}
			num3 += internalRule.RegexCharacterCount;
			if (num3 > (int)byteQuantifiedSize.ToBytes())
			{
				return new InvalidOperationException(Strings.ErrorTooManyRegexCharsInRuleCollection(num3, (long)byteQuantifiedSize.ToBytes()));
			}
			List<string> list = new List<string>();
			foreach (Rule rule2 in ruleStorageManager.GetRuleCollection())
			{
				TransportRule transportRule2 = (TransportRule)rule2;
				if (!transportRule2.Name.Equals(internalRule.Name, StringComparison.InvariantCulture))
				{
					foreach (string text2 in transportRule2.RecipientsAddedByActions)
					{
						MailEnabledRecipient recipientMailEnabledRecipient = Utils.GetRecipientMailEnabledRecipient(text2, recipientSession);
						if (recipientMailEnabledRecipient != null && (recipientMailEnabledRecipient is DistributionGroup || recipientMailEnabledRecipient is DynamicDistributionGroup))
						{
							return new InvalidOperationException(Strings.ErrorExistingRecipientInActionsCannotBeDistributionGroup(transportRule2.Name, text2));
						}
						list.Add(text2);
					}
				}
			}
			foreach (string item in internalRule.RecipientsAddedByActions)
			{
				list.Add(item);
			}
			foreach (string text3 in list)
			{
				MailEnabledRecipient recipientMailEnabledRecipient2 = Utils.GetRecipientMailEnabledRecipient(text3, recipientSession);
				if (recipientMailEnabledRecipient2 != null && (recipientMailEnabledRecipient2 is DistributionGroup || recipientMailEnabledRecipient2 is DynamicDistributionGroup))
				{
					return new InvalidOperationException(Strings.ErrorAddedRecipientCannotBeDistributionGroup(text3));
				}
			}
			if (list.Count > num)
			{
				return new InvalidOperationException(Strings.ErrorTooManyAddedRecipientsInRuleCollection(list.Count, num));
			}
			return null;
		}

		internal static MailEnabledRecipient GetRecipientMailEnabledRecipient(ADRecipient recipient)
		{
			switch (recipient.RecipientType)
			{
			case RecipientType.UserMailbox:
				return new Mailbox((ADUser)recipient);
			case RecipientType.MailUser:
				return new MailUser((ADUser)recipient);
			case RecipientType.MailContact:
				return new MailContact((ADContact)recipient);
			case RecipientType.MailUniversalDistributionGroup:
			case RecipientType.MailUniversalSecurityGroup:
			case RecipientType.MailNonUniversalGroup:
				return new DistributionGroup((ADGroup)recipient);
			case RecipientType.DynamicDistributionGroup:
				return new DynamicDistributionGroup((ADDynamicGroup)recipient);
			}
			return null;
		}

		internal static string GetRecipientAddressString(MailEnabledRecipient recipient)
		{
			if (recipient == null)
			{
				return null;
			}
			SmtpAddress primarySmtpAddress = recipient.PrimarySmtpAddress;
			if (SmtpAddress.Empty == primarySmtpAddress || !primarySmtpAddress.IsValidAddress)
			{
				return null;
			}
			return primarySmtpAddress.ToString();
		}

		internal static bool ValidateServerVersions(Version minVer, Task task, string name)
		{
			if (minVer == new Version("14.00.0000.000"))
			{
				return true;
			}
			IConfigDataProvider configDataProvider = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 1272, "ValidateServerVersions", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\TransportRule\\Utils.cs");
			IEnumerable<Server> enumerable = configDataProvider.FindPaged<Server>(null, null, true, null, 0);
			foreach (Server server in enumerable)
			{
				if (server.IsHubTransportServer)
				{
					if (null == server.AdminDisplayVersion)
					{
						task.WriteWarning(Strings.ServerVersionNull(server.Name ?? string.Empty, name));
					}
					else if (!(server.AdminDisplayVersion < new Version("14.0.574.0")) && minVer > server.AdminDisplayVersion)
					{
						return false;
					}
				}
			}
			return true;
		}

		internal static bool ValidateGccJournalRuleParameters(Task task, bool isNewRule)
		{
			if (task.Fields.IsModified("LawfulInterception"))
			{
				if (isNewRule && !task.Fields.IsModified("RecipientProperty"))
				{
					task.WriteError(new InvalidOperationException(Strings.JournalingParameterErrorGccWithoutRecipient), ErrorCategory.InvalidOperation, null);
					return false;
				}
				if (task.Fields.IsModified("RecipientProperty") && task.Fields["RecipientProperty"] == null)
				{
					task.WriteError(new InvalidOperationException(Strings.JournalingParameterErrorGccWithoutRecipient), ErrorCategory.InvalidOperation, null);
					return false;
				}
				if (task.Fields.IsModified("Scope"))
				{
					task.WriteError(new InvalidOperationException(Strings.JournalingParameterErrorGccWithScope), ErrorCategory.InvalidOperation, null);
					return false;
				}
				if (task.Fields.IsModified("Organization"))
				{
					task.WriteError(new InvalidOperationException(Strings.JournalingParameterErrorGccWithOrganization), ErrorCategory.InvalidOperation, null);
					return false;
				}
			}
			else
			{
				if (task.Fields.IsModified("FullReport"))
				{
					task.WriteError(new InvalidOperationException(Strings.JournalingParameterErrorFullReportWithoutGcc), ErrorCategory.InvalidOperation, null);
					return false;
				}
				if (task.Fields.IsModified("ExpiryDate"))
				{
					task.WriteError(new InvalidOperationException(Strings.JournalingParameterErrorExpiryDateWithoutGcc), ErrorCategory.InvalidOperation, null);
					return false;
				}
			}
			return true;
		}

		internal static bool Exchange12HubServersExist(Task task)
		{
			IConfigDataProvider configDataProvider = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 1398, "Exchange12HubServersExist", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\TransportRule\\Utils.cs");
			IEnumerable<Server> enumerable = configDataProvider.FindPaged<Server>(null, null, true, null, 0);
			Version v = new Version("14.0");
			foreach (Server server in enumerable)
			{
				if (server.IsHubTransportServer)
				{
					if (null == server.AdminDisplayVersion)
					{
						task.WriteWarning(Strings.HubServerVersionNotFound((server.Name == null) ? string.Empty : server.Name));
					}
					else if (server.AdminDisplayVersion < v)
					{
						return true;
					}
				}
			}
			return false;
		}

		internal static bool IsCustomizedDsnConfigured(string enhancedStatus)
		{
			if (enhancedStatus.Equals("5.7.1", StringComparison.Ordinal))
			{
				return true;
			}
			IConfigurationSession configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 1438, "IsCustomizedDsnConfigured", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\TransportRule\\Utils.cs");
			ADObjectId orgContainerId = configurationSession.GetOrgContainerId();
			ObjectId dsnCustomizationContainer = SystemMessage.GetDsnCustomizationContainer(orgContainerId);
			QueryFilter filter = new TextFilter(ADObjectSchema.Name, enhancedStatus, MatchOptions.FullString, MatchFlags.Default);
			SystemMessage[] array = (SystemMessage[])configurationSession.Find<SystemMessage>(filter, dsnCustomizationContainer, true, null);
			return array.Length > 0;
		}

		internal static bool IsRuleIdInList(ADObjectId id, IEnumerable<TransportRule> rules)
		{
			foreach (TransportRule transportRule in rules)
			{
				if (id.Equals(transportRule.Id))
				{
					return true;
				}
			}
			return false;
		}

		internal static bool ValidateRuleComments(string chars, out ArgumentException error)
		{
			bool result = true;
			error = null;
			if (!string.IsNullOrEmpty(chars))
			{
				int index;
				if (chars.Length > 1024)
				{
					error = new ArgumentException(Strings.CommentsTooLong(1024, chars.Length), "Comments");
					result = false;
				}
				else if (!Utils.CheckIsUnicodeStringWellFormed(chars, out index))
				{
					error = new ArgumentException(Strings.CommentsHaveInvalidChars((int)chars[index]), "Comments");
					result = false;
				}
			}
			return result;
		}

		internal static bool ValidateRestrictedHeaders(PropertyBag fields, bool validateForMultitenancyOnly, out ArgumentException error, out string parameterName)
		{
			error = null;
			parameterName = null;
			if (validateForMultitenancyOnly && !Utils.IsMultiTeancyEnabled())
			{
				return true;
			}
			string text = null;
			string text2 = null;
			if (fields.IsModified("SetHeaderName") && fields["SetHeaderName"] != null)
			{
				text = fields["SetHeaderName"].ToString();
			}
			if (fields.IsModified("RemoveHeader") && fields["RemoveHeader"] != null)
			{
				text2 = fields["RemoveHeader"].ToString();
			}
			string[] tenantProhibitedHeaderPrefixes = Utils.TenantProhibitedHeaderPrefixes;
			int i = 0;
			while (i < tenantProhibitedHeaderPrefixes.Length)
			{
				string value = tenantProhibitedHeaderPrefixes[i];
				bool result;
				if (!string.IsNullOrEmpty(text) && text.StartsWith(value, StringComparison.InvariantCultureIgnoreCase) && string.Compare(text, "X-Ms-Exchange-Organization-Dlp-SenderOverrideJustification", StringComparison.InvariantCultureIgnoreCase) != 0 && string.Compare(text, "X-Ms-Exchange-Organization-Dlp-FalsePositive", StringComparison.InvariantCultureIgnoreCase) != 0)
				{
					error = new ArgumentException(Strings.HeaderNameNotAllowed(text), "SetHeaderName");
					parameterName = "SetHeaderName";
					result = false;
				}
				else
				{
					if (string.IsNullOrEmpty(text2) || !text2.StartsWith(value, StringComparison.InvariantCultureIgnoreCase))
					{
						i++;
						continue;
					}
					error = new ArgumentException(Strings.HeaderNameNotAllowed(text2), "RemoveHeader");
					parameterName = "RemoveHeader";
					result = false;
				}
				return result;
			}
			if (string.Compare(text, "X-MS-Exchange-Transport-Rules-IncidentReport", StringComparison.InvariantCultureIgnoreCase) == 0 || string.Compare(text, "X-MS-Exchange-Transport-Rules-Notification", StringComparison.InvariantCultureIgnoreCase) == 0)
			{
				error = new ArgumentException(Strings.HeaderNameNotAllowed(text), "SetHeaderName");
				parameterName = "SetHeaderName";
				return false;
			}
			return true;
		}

		internal static bool ValidateMessageClassification(PropertyBag fields, out ArgumentException error, out string parameterName, IConfigDataProvider session)
		{
			if (fields.IsModified("HasClassification"))
			{
				string text = (string)fields["HasClassification"];
				if (!string.IsNullOrEmpty(text) && Utils.GetClassificationADObjectId(text, session) == null)
				{
					error = new ArgumentException(Strings.InvalidMessageClassification(text));
					parameterName = "HasClassification";
					return false;
				}
			}
			if (fields.IsModified("ExceptIfHasClassification"))
			{
				string text2 = (string)fields["ExceptIfHasClassification"];
				if (!string.IsNullOrEmpty(text2) && Utils.GetClassificationADObjectId(text2, session) == null)
				{
					error = new ArgumentException(Strings.InvalidMessageClassification(text2));
					parameterName = "ExceptIfHasClassification";
					return false;
				}
			}
			if (fields.IsModified("ApplyClassification"))
			{
				string text3 = (string)fields["ApplyClassification"];
				if (!string.IsNullOrEmpty(text3) && Utils.GetClassificationADObjectId(text3, session) == null)
				{
					error = new ArgumentException(Strings.InvalidMessageClassification(text3));
					parameterName = "ApplyClassification";
					return false;
				}
			}
			error = null;
			parameterName = null;
			return true;
		}

		internal static bool ValidateAuditSeverityLevel(PropertyBag fields, out ArgumentException error, out string parameterName)
		{
			if (fields.IsModified("SetAuditSeverity"))
			{
				string text = (string)fields["SetAuditSeverity"];
				AuditSeverityLevel auditSeverityLevel;
				if (!string.IsNullOrEmpty(text) && !Enum.TryParse<AuditSeverityLevel>(text, out auditSeverityLevel))
				{
					error = new ArgumentException(Strings.InvalidAuditSeverityLevel(text));
					parameterName = "SetAuditSeverity";
					return false;
				}
			}
			error = null;
			parameterName = null;
			return true;
		}

		internal static bool ValidateMessageDataClassification(PropertyBag fields, OrganizationId orgId, out ArgumentException error, out string parameterName)
		{
			if (fields["MessageContainsDataClassifications"] != null)
			{
				Hashtable[] array = Utils.UppercaseHashtableKeys((Hashtable[])fields["MessageContainsDataClassifications"]).ToArray<Hashtable>();
				error = MessageContainsDataClassificationsPredicate.ValidateDataClassificationParameters(orgId, array);
				if (error != null)
				{
					parameterName = "MessageContainsDataClassifications";
					return false;
				}
				fields["MessageContainsDataClassifications"] = array;
			}
			if (fields["ExceptIfMessageContainsDataClassifications"] != null)
			{
				Hashtable[] array2 = Utils.UppercaseHashtableKeys((Hashtable[])fields["ExceptIfMessageContainsDataClassifications"]).ToArray<Hashtable>();
				error = MessageContainsDataClassificationsPredicate.ValidateDataClassificationParameters(orgId, array2);
				if (error != null)
				{
					parameterName = "ExceptIfMessageContainsDataClassifications";
					return false;
				}
				fields["ExceptIfMessageContainsDataClassifications"] = array2;
			}
			error = null;
			parameterName = null;
			return true;
		}

		internal static IEnumerable<Hashtable> UppercaseHashtableKeys(IEnumerable<Hashtable> tables)
		{
			List<Hashtable> list = new List<Hashtable>();
			foreach (Hashtable hashtable in tables)
			{
				Hashtable hashtable2 = new Hashtable();
				foreach (object obj in hashtable.Keys)
				{
					string text = (string)obj;
					hashtable2.Add(text.ToUpper(), hashtable[text]);
				}
				list.Add(hashtable2);
			}
			return list;
		}

		internal static bool ValidateDlpPolicy(IConfigDataProvider dataSession, PropertyBag fields, out ArgumentException error, out string parameterName)
		{
			if (fields.IsModified("DlpPolicy"))
			{
				string text = (string)fields["DlpPolicy"];
				if (!string.IsNullOrEmpty(text) && !DlpUtils.GetInstalledTenantDlpPolicies(dataSession, text).Any<ADComplianceProgram>())
				{
					error = new ArgumentException(Strings.InvalidDlpPolicy(text));
					parameterName = "DlpPolicy";
					return false;
				}
			}
			error = null;
			parameterName = null;
			return true;
		}

		internal static bool ValidateRuleAndDlpPolicyStateConsistency(IConfigDataProvider dataSession, TransportRule rule, bool ruleEnabled, out ArgumentException error)
		{
			error = null;
			Guid guid;
			string text;
			return !rule.TryGetDlpPolicyId(out guid) || Guid.Empty.Equals(guid) || Utils.ValidateRuleAndDlpPolicyStateConsistency(dataSession, new PropertyBag(1)
			{
				{
					"DlpPolicy",
					guid
				}
			}, ruleEnabled, out error, out text);
		}

		internal static bool ValidateRuleAndDlpPolicyStateConsistency(IConfigDataProvider dataSession, PropertyBag fields, bool ruleEnabled, out ArgumentException error, out string parameterName)
		{
			string text = (string)fields["DlpPolicy"];
			ADComplianceProgram adcomplianceProgram = DlpUtils.GetInstalledTenantDlpPolicies(dataSession, text).FirstOrDefault<ADComplianceProgram>();
			if (!string.IsNullOrEmpty(text) && adcomplianceProgram == null)
			{
				error = new ArgumentException(Strings.InvalidDlpPolicy(text));
				parameterName = "DlpPolicy";
				return false;
			}
			RuleState ruleState = ruleEnabled ? RuleState.Enabled : RuleState.Disabled;
			if (ruleState != DlpUtils.DlpStateToRuleState(adcomplianceProgram.State).Item1)
			{
				string state = (ruleState == RuleState.Enabled) ? Strings.RuleStateEnabled : Strings.RuleStateDisabled;
				string requiredState = (ruleState == RuleState.Enabled) ? Strings.RuleStateDisabled : Strings.RuleStateEnabled;
				error = new ArgumentException(Strings.ErrorRuleStateInconsistentWithComplianceProgram(state, text, requiredState));
				parameterName = "DlpPolicy";
				return false;
			}
			error = null;
			parameterName = null;
			return true;
		}

		internal static bool ValidateConnectorParameter(PropertyBag fields, IConfigDataProvider session, out ArgumentException error, out string errorParameterName)
		{
			if (fields.IsModified("RouteMessageOutboundConnector"))
			{
				OutboundConnectorIdParameter outboundConnectorIdParameter = (OutboundConnectorIdParameter)fields["RouteMessageOutboundConnector"];
				if (outboundConnectorIdParameter != null)
				{
					error = Utils.ValidateEnabledConnectorById(ref outboundConnectorIdParameter, session);
					if (error != null)
					{
						errorParameterName = "RouteMessageOutboundConnector";
						return false;
					}
					fields["RouteMessageOutboundConnector"] = outboundConnectorIdParameter;
				}
			}
			error = null;
			errorParameterName = null;
			return true;
		}

		internal static void ValidateGenerateIncidentReportParameters(Task task, string cmdletName)
		{
			if (task.Fields.IsModified("IncidentReportContent") && task.Fields.IsModified("IncidentReportOriginalMail"))
			{
				task.WriteError(new ArgumentException(RulesTasksStrings.IncidentReportConflictingParameters("IncidentReportOriginalMail", "IncidentReportContent")), ErrorCategory.InvalidArgument, cmdletName);
			}
			if (task.Fields.IsModified("IncidentReportOriginalMail") && !task.Fields.IsModified("GenerateIncidentReport"))
			{
				task.WriteError(new ArgumentException(Strings.InvalidIncidentReportOriginalMail), ErrorCategory.InvalidArgument, cmdletName);
			}
			if (task.Fields.IsModified("IncidentReportContent") && !task.Fields.IsModified("GenerateIncidentReport"))
			{
				task.WriteError(new ArgumentException(Strings.InvalidIncidentReportContent), ErrorCategory.InvalidArgument, cmdletName);
			}
		}

		internal static bool CheckIsUnicodeStringWellFormed(string chars, out int position)
		{
			for (int i = 0; i < chars.Length; i++)
			{
				char c = chars[i];
				position = i;
				if (c < ' ' || (c > '퟿' && c < '') || c > '�')
				{
					if (c != '\t' && c != '\n' && c != '\r')
					{
						return false;
					}
				}
				else if (char.IsHighSurrogate(c))
				{
					if (i + 1 >= chars.Length || !char.IsLowSurrogate(chars[i + 1]))
					{
						return false;
					}
					i++;
				}
				else if (char.IsLowSurrogate(c))
				{
					return false;
				}
			}
			position = -1;
			return true;
		}

		internal static bool ValidateParametersForRole(PropertyBag fields, out Exception error, out string parameterName)
		{
			if (Utils.IsEdgeRoleInstalled())
			{
				foreach (string text in Utils.HubSpecificParameters)
				{
					if (fields.IsModified(text))
					{
						error = new ArgumentException(Strings.HubParameterNotValidOnEdgeRole);
						parameterName = text;
						return false;
					}
				}
			}
			else
			{
				foreach (string text2 in Utils.EdgeSpecificParameters)
				{
					if (fields.IsModified(text2))
					{
						error = new ArgumentException(Strings.EdgeParameterNotValidOnHubRole);
						parameterName = text2;
						return false;
					}
				}
				if (fields.IsModified("Quarantine") && !VariantConfiguration.InvariantNoFlightingSnapshot.CompliancePolicy.QuarantineAction.Enabled)
				{
					error = new ArgumentException(Strings.QuarantineActionNotAvailable);
					parameterName = "Quarantine";
					return false;
				}
			}
			error = null;
			parameterName = null;
			return true;
		}

		internal static bool ValidateRecipientIdParameters(PropertyBag fields, IRecipientSession recipientSession, DataAccessHelper.GetDataObjectDelegate getRecipientObject, out Exception error, out string parameterName)
		{
			foreach (string text in Utils.RecipientIdParameters)
			{
				if (fields.IsModified(text))
				{
					bool flag = false;
					bool flag2 = false;
					RecipientIdParameter[] array = (RecipientIdParameter[])fields[text];
					if (array != null)
					{
						int j = 0;
						while (j < array.Length)
						{
							RecipientIdParameter recipientIdParameter = array[j];
							bool result;
							if (!Utils.ValidateRecipient(getRecipientObject, recipientSession, ref recipientIdParameter, ref flag, out flag2, out error))
							{
								parameterName = text;
								result = false;
							}
							else
							{
								array[j] = recipientIdParameter;
								flag = true;
								if ((!(text == "SentTo") && !(text == "ExceptIfSentTo")) || !flag2)
								{
									j++;
									continue;
								}
								parameterName = text;
								error = new ArgumentException(Strings.ErrorRecipientInSentToConditionCannotBeDistributionGroup(array[j].ToString()));
								result = false;
							}
							return result;
						}
					}
					if (flag)
					{
						fields[text] = array;
					}
				}
			}
			if (fields.IsModified("GenerateIncidentReport") && fields["GenerateIncidentReport"] != null)
			{
				bool flag = false;
				bool flag2 = false;
				RecipientIdParameter value = (RecipientIdParameter)fields["GenerateIncidentReport"];
				if (!Utils.ValidateRecipient(getRecipientObject, recipientSession, ref value, ref flag, out flag2, out error))
				{
					parameterName = "GenerateIncidentReport";
					return false;
				}
				if (flag)
				{
					fields["GenerateIncidentReport"] = value;
				}
			}
			error = null;
			parameterName = string.Empty;
			return true;
		}

		private static bool ValidateRecipient(DataAccessHelper.GetDataObjectDelegate getRecipientObject, IRecipientSession recipientSession, ref RecipientIdParameter recipientId, ref bool recipientUpdated, out bool isDistributionGroup, out Exception error)
		{
			bool flag = false;
			try
			{
				ADRecipient recipient = (ADRecipient)getRecipientObject(recipientId, recipientSession, null, null, new LocalizedString?(Strings.ErrorRecipientNotFound(recipientId.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(recipientId.ToString())));
				MailEnabledRecipient recipientMailEnabledRecipient = Utils.GetRecipientMailEnabledRecipient(recipient);
				if (recipientMailEnabledRecipient != null && (recipientMailEnabledRecipient is DistributionGroup || recipientMailEnabledRecipient is DynamicDistributionGroup))
				{
					flag = true;
				}
			}
			catch (ManagementObjectNotFoundException ex)
			{
				MimeRecipient mimeRecipient = null;
				try
				{
					mimeRecipient = MimeRecipient.Parse(recipientId.RawIdentity, AddressParserFlags.IgnoreComments | AddressParserFlags.AllowSquareBrackets);
				}
				catch (MimeException)
				{
				}
				if (mimeRecipient == null || string.IsNullOrEmpty(mimeRecipient.Email) || !SmtpAddress.IsValidSmtpAddress(mimeRecipient.Email))
				{
					error = ex;
					isDistributionGroup = false;
					return false;
				}
				recipientId = new RecipientIdParameter(mimeRecipient.Email);
				recipientUpdated = true;
			}
			isDistributionGroup = flag;
			error = null;
			return true;
		}

		internal static bool ValidateParameterGroups(PropertyBag fields, out ArgumentException error, out string parameterName)
		{
			if (fields.IsModified("BetweenMemberOf1") && !fields.IsModified("BetweenMemberOf2"))
			{
				error = new ArgumentException(Strings.IncompleteParameterGroup("BetweenMemberOf1", "BetweenMemberOf2"));
				parameterName = "BetweenMemberOf1";
				return false;
			}
			if (fields.IsModified("BetweenMemberOf2") && !fields.IsModified("BetweenMemberOf1"))
			{
				error = new ArgumentException(Strings.IncompleteParameterGroup("BetweenMemberOf2", "BetweenMemberOf1"));
				parameterName = "BetweenMemberOf2";
				return false;
			}
			if (fields.IsModified("ManagerForEvaluatedUser") && !fields.IsModified("ManagerAddresses"))
			{
				error = new ArgumentException(Strings.IncompleteParameterGroup("ManagerForEvaluatedUser", "ManagerAddresses"));
				parameterName = "ManagerForEvaluatedUser";
				return false;
			}
			if (fields.IsModified("ADComparisonOperator") && !fields.IsModified("ADComparisonAttribute"))
			{
				error = new ArgumentException(Strings.IncompleteParameterGroup("ADComparisonOperator", "ADComparisonAttribute"));
				parameterName = "ADComparisonOperator";
				return false;
			}
			if (fields.IsModified("HeaderContainsMessageHeader") && !fields.IsModified("HeaderContainsWords"))
			{
				error = new ArgumentException(Strings.IncompleteParameterGroup("HeaderContainsMessageHeader", "HeaderContainsWords"));
				parameterName = "HeaderContainsMessageHeader";
				return false;
			}
			if (fields.IsModified("HeaderContainsWords") && !fields.IsModified("HeaderContainsMessageHeader"))
			{
				error = new ArgumentException(Strings.IncompleteParameterGroup("HeaderContainsWords", "HeaderContainsMessageHeader"));
				parameterName = "HeaderContainsWords";
				return false;
			}
			if (fields.IsModified("HeaderMatchesMessageHeader") && !fields.IsModified("HeaderMatchesPatterns"))
			{
				error = new ArgumentException(Strings.IncompleteParameterGroup("HeaderMatchesMessageHeader", "HeaderMatchesPatterns"));
				parameterName = "HeaderMatchesMessageHeader";
				return false;
			}
			if (fields.IsModified("HeaderMatchesPatterns") && !fields.IsModified("HeaderMatchesMessageHeader"))
			{
				error = new ArgumentException(Strings.IncompleteParameterGroup("HeaderMatchesPatterns", "HeaderMatchesMessageHeader"));
				parameterName = "HeaderMatchesPatterns";
				return false;
			}
			if (fields.IsModified("ExceptIfBetweenMemberOf1") && !fields.IsModified("ExceptIfBetweenMemberOf2"))
			{
				error = new ArgumentException(Strings.IncompleteParameterGroup("ExceptIfBetweenMemberOf1", "ExceptIfBetweenMemberOf2"));
				parameterName = "ExceptIfBetweenMemberOf1";
				return false;
			}
			if (fields.IsModified("ExceptIfBetweenMemberOf2") && !fields.IsModified("ExceptIfBetweenMemberOf1"))
			{
				error = new ArgumentException(Strings.IncompleteParameterGroup("ExceptIfBetweenMemberOf2", "ExceptIfBetweenMemberOf1"));
				parameterName = "ExceptIfBetweenMemberOf2";
				return false;
			}
			if (fields.IsModified("ExceptIfManagerForEvaluatedUser") && !fields.IsModified("ExceptIfManagerAddresses"))
			{
				error = new ArgumentException(Strings.IncompleteParameterGroup("ExceptIfManagerForEvaluatedUser", "ExceptIfManagerAddresses"));
				parameterName = "ExceptIfManagerForEvaluatedUser";
				return false;
			}
			if (fields.IsModified("ExceptIfADComparisonOperator") && !fields.IsModified("ExceptIfADComparisonAttribute"))
			{
				error = new ArgumentException(Strings.IncompleteParameterGroup("ExceptIfADComparisonOperator", "ExceptIfADComparisonAttribute"));
				parameterName = "ExceptIfADComparisonOperator";
				return false;
			}
			if (fields.IsModified("ExceptIfHeaderContainsMessageHeader") && !fields.IsModified("ExceptIfHeaderContainsWords"))
			{
				error = new ArgumentException(Strings.IncompleteParameterGroup("ExceptIfHeaderContainsMessageHeader", "ExceptIfHeaderContainsWords"));
				parameterName = "ExceptIfHeaderContainsMessageHeader";
				return false;
			}
			if (fields.IsModified("ExceptIfHeaderContainsWords") && !fields.IsModified("ExceptIfHeaderContainsMessageHeader"))
			{
				error = new ArgumentException(Strings.IncompleteParameterGroup("ExceptIfHeaderContainsWords", "ExceptIfHeaderContainsMessageHeader"));
				parameterName = "ExceptIfHeaderContainsWords";
				return false;
			}
			if (fields.IsModified("ExceptIfHeaderMatchesMessageHeader") && !fields.IsModified("ExceptIfHeaderMatchesPatterns"))
			{
				error = new ArgumentException(Strings.IncompleteParameterGroup("ExceptIfHeaderMatchesMessageHeader", "ExceptIfHeaderMatchesPatterns"));
				parameterName = "ExceptIfHeaderMatchesMessageHeader";
				return false;
			}
			if (fields.IsModified("ExceptIfHeaderMatchesPatterns") && !fields.IsModified("ExceptIfHeaderMatchesMessageHeader"))
			{
				error = new ArgumentException(Strings.IncompleteParameterGroup("ExceptIfHeaderMatchesPatterns", "ExceptIfHeaderMatchesMessageHeader"));
				parameterName = "ExceptIfHeaderMatchesPatterns";
				return false;
			}
			if (fields.IsModified("ApplyHtmlDisclaimerLocation") && !fields.IsModified("ApplyHtmlDisclaimerText"))
			{
				error = new ArgumentException(Strings.IncompleteParameterGroup("ApplyHtmlDisclaimerLocation", string.Format("{0}, {1}", "ApplyHtmlDisclaimerLocation", "ApplyHtmlDisclaimerText")));
				parameterName = "ApplyHtmlDisclaimerLocation";
				return false;
			}
			if (fields.IsModified("ApplyHtmlDisclaimerFallbackAction") && !fields.IsModified("ApplyHtmlDisclaimerText"))
			{
				error = new ArgumentException(Strings.IncompleteParameterGroup("ApplyHtmlDisclaimerFallbackAction", string.Format("{0}, {1}", "ApplyHtmlDisclaimerFallbackAction", "ApplyHtmlDisclaimerText")));
				parameterName = "ApplyHtmlDisclaimerFallbackAction";
				return false;
			}
			if (fields.IsModified("SetHeaderName") && !fields.IsModified("SetHeaderValue"))
			{
				error = new ArgumentException(Strings.IncompleteParameterGroup("SetHeaderName", "SetHeaderValue"));
				parameterName = "SetHeaderName";
				return false;
			}
			if (fields.IsModified("SetHeaderValue") && !fields.IsModified("SetHeaderName"))
			{
				error = new ArgumentException(Strings.IncompleteParameterGroup("SetHeaderValue", "SetHeaderName"));
				parameterName = "SetHeaderValue";
				return false;
			}
			if ((fields.IsModified("Conditions") || fields.IsModified("Actions") || fields.IsModified("Exceptions")) && (fields.IsModified("From") || fields.IsModified("FromMemberOf") || fields.IsModified("FromScope") || fields.IsModified("SentTo") || fields.IsModified("SentToMemberOf") || fields.IsModified("SentToScope") || fields.IsModified("BetweenMemberOf1") || fields.IsModified("BetweenMemberOf2") || fields.IsModified("ManagerAddresses") || fields.IsModified("ManagerForEvaluatedUser") || fields.IsModified("SenderManagementRelationship") || fields.IsModified("ADComparisonAttribute") || fields.IsModified("ADComparisonOperator") || fields.IsModified("SenderADAttributeContainsWords") || fields.IsModified("SenderADAttributeMatchesPatterns") || fields.IsModified("RecipientADAttributeContainsWords") || fields.IsModified("RecipientADAttributeMatchesPatterns") || fields.IsModified("AnyOfToHeader") || fields.IsModified("AnyOfToHeaderMemberOf") || fields.IsModified("AnyOfCcHeader") || fields.IsModified("AnyOfCcHeaderMemberOf") || fields.IsModified("AnyOfToCcHeader") || fields.IsModified("AnyOfToCcHeaderMemberOf") || fields.IsModified("HasClassification") || fields.IsModified("HasNoClassification") || fields.IsModified("SubjectContainsWords") || fields.IsModified("SubjectOrBodyContainsWords") || fields.IsModified("HeaderContainsMessageHeader") || fields.IsModified("HeaderContainsWords") || fields.IsModified("FromAddressContainsWords") || fields.IsModified("SenderDomainIs") || fields.IsModified("RecipientDomainIs") || fields.IsModified("SubjectMatchesPatterns") || fields.IsModified("SubjectOrBodyMatchesPatterns") || fields.IsModified("HeaderMatchesMessageHeader") || fields.IsModified("HeaderMatchesPatterns") || fields.IsModified("FromAddressMatchesPatterns") || fields.IsModified("AttachmentNameMatchesPatterns") || fields.IsModified("AttachmentExtensionMatchesWords") || fields.IsModified("AttachmentHasExecutableContent") || fields.IsModified("AttachmentIsUnsupported") || fields.IsModified("AttachmentProcessingLimitExceeded") || fields.IsModified("AttachmentIsPasswordProtected") || fields.IsModified("AttachmentPropertyContainsWords") || fields.IsModified("ContentCharacterSetContainsWords") || fields.IsModified("SCLOver") || fields.IsModified("AttachmentSizeOver") || fields.IsModified("MessageSizeOver") || fields.IsModified("WithImportance") || fields.IsModified("MessageTypeMatches") || fields.IsModified("RecipientAddressContainsWords") || fields.IsModified("RecipientAddressMatchesPatterns") || fields.IsModified("HasSenderOverride") || fields.IsModified("MessageContainsDataClassifications") || fields.IsModified("ExceptIfFrom") || fields.IsModified("ExceptIfFromMemberOf") || fields.IsModified("ExceptIfFromScope") || fields.IsModified("ExceptIfSentTo") || fields.IsModified("ExceptIfSentToMemberOf") || fields.IsModified("ExceptIfSentToScope") || fields.IsModified("ExceptIfBetweenMemberOf1") || fields.IsModified("ExceptIfBetweenMemberOf2") || fields.IsModified("ExceptIfManagerAddresses") || fields.IsModified("ExceptIfManagerForEvaluatedUser") || fields.IsModified("ExceptIfSenderManagementRelationship") || fields.IsModified("ExceptIfADComparisonAttribute") || fields.IsModified("ExceptIfADComparisonOperator") || fields.IsModified("ExceptIfSenderADAttributeContainsWords") || fields.IsModified("ExceptIfSenderADAttributeMatchesPatterns") || fields.IsModified("ExceptIfRecipientADAttributeContainsWords") || fields.IsModified("ExceptIfRecipientADAttributeMatchesPatterns") || fields.IsModified("ExceptIfAnyOfToHeader") || fields.IsModified("ExceptIfAnyOfToHeaderMemberOf") || fields.IsModified("ExceptIfAnyOfCcHeader") || fields.IsModified("ExceptIfAnyOfCcHeaderMemberOf") || fields.IsModified("ExceptIfAnyOfToCcHeader") || fields.IsModified("ExceptIfAnyOfToCcHeaderMemberOf") || fields.IsModified("ExceptIfHasClassification") || fields.IsModified("ExceptIfHasNoClassification") || fields.IsModified("ExceptIfSubjectContainsWords") || fields.IsModified("ExceptIfSubjectOrBodyContainsWords") || fields.IsModified("ExceptIfHeaderContainsMessageHeader") || fields.IsModified("ExceptIfHeaderContainsWords") || fields.IsModified("ExceptIfFromAddressContainsWords") || fields.IsModified("ExceptIfSenderDomainIs") || fields.IsModified("ExceptIfRecipientDomainIs") || fields.IsModified("ExceptIfSubjectMatchesPatterns") || fields.IsModified("ExceptIfSubjectOrBodyMatchesPatterns") || fields.IsModified("ExceptIfHeaderMatchesMessageHeader") || fields.IsModified("ExceptIfHeaderMatchesPatterns") || fields.IsModified("ExceptIfFromAddressMatchesPatterns") || fields.IsModified("ExceptIfAttachmentNameMatchesPatterns") || fields.IsModified("ExceptIfAttachmentExtensionMatchesWords") || fields.IsModified("ExceptIfAttachmentIsUnsupported") || fields.IsModified("ExceptIfAttachmentProcessingLimitExceeded") || fields.IsModified("ExceptIfAttachmentHasExecutableContent") || fields.IsModified("ExceptIfAttachmentIsPasswordProtected") || fields.IsModified("ExceptIfAttachmentPropertyContainsWords") || fields.IsModified("ExceptIfContentCharacterSetContainsWords") || fields.IsModified("ExceptIfSCLOver") || fields.IsModified("ExceptIfAttachmentSizeOver") || fields.IsModified("ExceptIfMessageSizeOver") || fields.IsModified("ExceptIfWithImportance") || fields.IsModified("ExceptIfMessageTypeMatches") || fields.IsModified("ExceptIfRecipientAddressContainsWords") || fields.IsModified("ExceptIfRecipientAddressMatchesPatterns") || fields.IsModified("ExceptIfHasSenderOverride") || fields.IsModified("ExceptIfMessageContainsDataClassifications") || fields.IsModified("PrependSubject") || fields.IsModified("SetAuditSeverity") || fields.IsModified("ApplyClassification") || fields.IsModified("ApplyHtmlDisclaimerLocation") || fields.IsModified("ApplyHtmlDisclaimerText") || fields.IsModified("ApplyHtmlDisclaimerFallbackAction") || fields.IsModified("ApplyRightsProtectionTemplate") || fields.IsModified("SetSCL") || fields.IsModified("SetHeaderName") || fields.IsModified("SetHeaderValue") || fields.IsModified("RemoveHeader") || fields.IsModified("AddToRecipients") || fields.IsModified("CopyTo") || fields.IsModified("BlindCopyTo") || fields.IsModified("AddManagerAsRecipientType") || fields.IsModified("NotifySender") || fields.IsModified("SetAuditSeverity") || fields.IsModified("ModerateMessageByManager") || fields.IsModified("ModerateMessageByUser") || fields.IsModified("RedirectMessageTo") || fields.IsModified("RejectMessageEnhancedStatusCode") || fields.IsModified("RejectMessageReasonText") || fields.IsModified("DeleteMessage") || fields.IsModified("GenerateIncidentReport") || fields.IsModified("GenerateNotification") || fields.IsModified("StopRuleProcessing") || fields.IsModified("RouteMessageOutboundConnector") || fields.IsModified("RouteMessageOutboundRequireTls") || fields.IsModified("ApplyOME") || fields.IsModified("RemoveOME")))
			{
				error = new ArgumentException(Strings.ParameterVersionMismatch);
				if (fields.IsModified("Conditions"))
				{
					parameterName = "Conditions";
				}
				else if (fields.IsModified("Actions"))
				{
					parameterName = "Actions";
				}
				else
				{
					parameterName = "Exceptions";
				}
				return false;
			}
			error = null;
			parameterName = null;
			return true;
		}

		internal static void BuildConditionsAndExceptionsFromParameters(PropertyBag fields, IRecipientSession recipientSession, IConfigDataProvider dataSession, bool useLegacyRegex, out List<Type> conditionTypesSpecified, out List<Type> exceptionTypesSpecified, out TransportRulePredicate[] conditions, out TransportRulePredicate[] exceptions)
		{
			TypeMapping[] availablePredicateMappings = TransportRulePredicate.GetAvailablePredicateMappings();
			conditionTypesSpecified = new List<Type>();
			exceptionTypesSpecified = new List<Type>();
			List<TransportRulePredicate> list = new List<TransportRulePredicate>();
			List<TransportRulePredicate> list2 = new List<TransportRulePredicate>();
			Utils.InitializeAndAddNewPredicateForArrayParameter<FromPredicate, RecipientIdParameter>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "From", "ExceptIfFrom", delegate(FromPredicate predicate, RecipientIdParameter[] addresses)
			{
				predicate.Addresses = Utils.BuildSmtpAddressArray(addresses, recipientSession);
			});
			Utils.InitializeAndAddNewPredicateForArrayParameter<FromMemberOfPredicate, RecipientIdParameter>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "FromMemberOf", "ExceptIfFromMemberOf", delegate(FromMemberOfPredicate predicate, RecipientIdParameter[] addresses)
			{
				predicate.Addresses = Utils.BuildSmtpAddressArray(addresses, recipientSession);
			});
			Utils.InitializeAndAddNewPredicateForNullableParameter<FromScopePredicate, FromUserScope>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "FromScope", "ExceptIfFromScope", delegate(FromScopePredicate predicate, FromUserScope scope)
			{
				predicate.Scope = scope;
			});
			Utils.InitializeAndAddNewPredicateForArrayParameter<SentToPredicate, RecipientIdParameter>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "SentTo", "ExceptIfSentTo", delegate(SentToPredicate predicate, RecipientIdParameter[] addresses)
			{
				predicate.Addresses = Utils.BuildSmtpAddressArray(addresses, recipientSession);
			});
			Utils.InitializeAndAddNewPredicateForArrayParameter<SentToMemberOfPredicate, RecipientIdParameter>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "SentToMemberOf", "ExceptIfSentToMemberOf", delegate(SentToMemberOfPredicate predicate, RecipientIdParameter[] addresses)
			{
				predicate.Addresses = Utils.BuildSmtpAddressArray(addresses, recipientSession);
			});
			Utils.InitializeAndAddNewPredicateForNullableParameter<SentToScopePredicate, ToUserScope>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "SentToScope", "ExceptIfSentToScope", delegate(SentToScopePredicate predicate, ToUserScope scope)
			{
				predicate.Scope = scope;
			});
			if (fields.IsModified("BetweenMemberOf1"))
			{
				RecipientIdParameter[] array = (RecipientIdParameter[])fields["BetweenMemberOf1"];
				RecipientIdParameter[] array2 = (RecipientIdParameter[])fields["BetweenMemberOf2"];
				if (array != null && array.Length > 0 && array2 != null && array2.Length > 0)
				{
					BetweenMemberOfPredicate betweenMemberOfPredicate = new BetweenMemberOfPredicate();
					betweenMemberOfPredicate.Initialize(availablePredicateMappings);
					betweenMemberOfPredicate.Addresses = Utils.BuildSmtpAddressArray(array, recipientSession);
					betweenMemberOfPredicate.Addresses2 = Utils.BuildSmtpAddressArray(array2, recipientSession);
					Utils.InsertPredicateSorted(betweenMemberOfPredicate, list);
				}
				conditionTypesSpecified.Add(typeof(BetweenMemberOfPredicate));
			}
			if (fields.IsModified("ExceptIfBetweenMemberOf1"))
			{
				RecipientIdParameter[] array3 = (RecipientIdParameter[])fields["ExceptIfBetweenMemberOf1"];
				RecipientIdParameter[] array4 = (RecipientIdParameter[])fields["ExceptIfBetweenMemberOf2"];
				if (array3 != null && array3.Length > 0 && array4 != null && array4.Length > 0)
				{
					BetweenMemberOfPredicate betweenMemberOfPredicate2 = new BetweenMemberOfPredicate();
					betweenMemberOfPredicate2.Initialize(availablePredicateMappings);
					betweenMemberOfPredicate2.Addresses = Utils.BuildSmtpAddressArray(array3, recipientSession);
					betweenMemberOfPredicate2.Addresses2 = Utils.BuildSmtpAddressArray(array4, recipientSession);
					Utils.InsertPredicateSorted(betweenMemberOfPredicate2, list2);
				}
				exceptionTypesSpecified.Add(typeof(BetweenMemberOfPredicate));
			}
			if (fields.IsModified("ManagerAddresses"))
			{
				RecipientIdParameter[] array5 = (RecipientIdParameter[])fields["ManagerAddresses"];
				EvaluatedUser? evaluatedUser;
				if (fields.IsModified("ManagerForEvaluatedUser"))
				{
					evaluatedUser = (EvaluatedUser?)fields["ManagerForEvaluatedUser"];
				}
				else
				{
					evaluatedUser = new EvaluatedUser?(EvaluatedUser.Sender);
				}
				if (array5 != null && array5.Length > 0 && evaluatedUser != null)
				{
					ManagerIsPredicate managerIsPredicate = new ManagerIsPredicate();
					managerIsPredicate.Initialize(availablePredicateMappings);
					managerIsPredicate.Addresses = Utils.BuildSmtpAddressArray(array5, recipientSession);
					managerIsPredicate.EvaluatedUser = evaluatedUser.Value;
					Utils.InsertPredicateSorted(managerIsPredicate, list);
				}
				conditionTypesSpecified.Add(typeof(ManagerIsPredicate));
			}
			if (fields.IsModified("ExceptIfManagerAddresses"))
			{
				RecipientIdParameter[] array6 = (RecipientIdParameter[])fields["ExceptIfManagerAddresses"];
				EvaluatedUser? evaluatedUser2;
				if (fields.IsModified("ExceptIfManagerForEvaluatedUser"))
				{
					evaluatedUser2 = (EvaluatedUser?)fields["ExceptIfManagerForEvaluatedUser"];
				}
				else
				{
					evaluatedUser2 = new EvaluatedUser?(EvaluatedUser.Sender);
				}
				if (array6 != null && array6.Length > 0 && evaluatedUser2 != null)
				{
					ManagerIsPredicate managerIsPredicate2 = new ManagerIsPredicate();
					managerIsPredicate2.Initialize(availablePredicateMappings);
					managerIsPredicate2.Addresses = Utils.BuildSmtpAddressArray(array6, recipientSession);
					managerIsPredicate2.EvaluatedUser = evaluatedUser2.Value;
					Utils.InsertPredicateSorted(managerIsPredicate2, list2);
				}
				exceptionTypesSpecified.Add(typeof(ManagerIsPredicate));
			}
			Utils.InitializeAndAddNewPredicateForNullableParameter<ManagementRelationshipPredicate, ManagementRelationship>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "SenderManagementRelationship", "ExceptIfSenderManagementRelationship", delegate(ManagementRelationshipPredicate predicate, ManagementRelationship managementRelationship)
			{
				predicate.ManagementRelationship = managementRelationship;
			});
			if (fields.IsModified("ADComparisonAttribute"))
			{
				ADAttribute? adattribute = (ADAttribute?)fields["ADComparisonAttribute"];
				Evaluation? evaluation;
				if (!fields.IsModified("ADComparisonOperator"))
				{
					evaluation = new Evaluation?(Evaluation.Equal);
				}
				else
				{
					evaluation = (Evaluation?)fields["ADComparisonOperator"];
				}
				if (adattribute != null && evaluation != null)
				{
					ADAttributeComparisonPredicate adattributeComparisonPredicate = new ADAttributeComparisonPredicate();
					adattributeComparisonPredicate.Initialize(availablePredicateMappings);
					adattributeComparisonPredicate.ADAttribute = adattribute.Value;
					adattributeComparisonPredicate.Evaluation = evaluation.Value;
					Utils.InsertPredicateSorted(adattributeComparisonPredicate, list);
				}
				conditionTypesSpecified.Add(typeof(ADAttributeComparisonPredicate));
			}
			if (fields.IsModified("ExceptIfADComparisonAttribute"))
			{
				ADAttribute? adattribute2 = (ADAttribute?)fields["ExceptIfADComparisonAttribute"];
				Evaluation? evaluation2;
				if (!fields.IsModified("ExceptIfADComparisonOperator"))
				{
					evaluation2 = new Evaluation?(Evaluation.Equal);
				}
				else
				{
					evaluation2 = (Evaluation?)fields["ExceptIfADComparisonOperator"];
				}
				if (adattribute2 != null && evaluation2 != null)
				{
					ADAttributeComparisonPredicate adattributeComparisonPredicate2 = new ADAttributeComparisonPredicate();
					adattributeComparisonPredicate2.Initialize(availablePredicateMappings);
					adattributeComparisonPredicate2.ADAttribute = adattribute2.Value;
					adattributeComparisonPredicate2.Evaluation = evaluation2.Value;
					Utils.InsertPredicateSorted(adattributeComparisonPredicate2, list2);
				}
				exceptionTypesSpecified.Add(typeof(ADAttributeComparisonPredicate));
			}
			Utils.InitializeAndAddNewPredicateForArrayParameter<SenderAttributeContainsPredicate, Word>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "SenderADAttributeContainsWords", "ExceptIfSenderADAttributeContainsWords", delegate(SenderAttributeContainsPredicate predicate, Word[] words)
			{
				predicate.Words = words;
			});
			Utils.InitializeAndAddNewPredicateForArrayParameter<SenderAttributeMatchesPredicate, Pattern>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "SenderADAttributeMatchesPatterns", "ExceptIfSenderADAttributeMatchesPatterns", delegate(SenderAttributeMatchesPredicate predicate, Pattern[] patterns)
			{
				predicate.UseLegacyRegex = useLegacyRegex;
				predicate.Patterns = patterns;
			});
			Utils.InitializeAndAddNewPredicateForArrayParameter<RecipientAttributeContainsPredicate, Word>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "RecipientADAttributeContainsWords", "ExceptIfRecipientADAttributeContainsWords", delegate(RecipientAttributeContainsPredicate predicate, Word[] words)
			{
				predicate.Words = words;
			});
			Utils.InitializeAndAddNewPredicateForArrayParameter<RecipientAttributeMatchesPredicate, Pattern>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "RecipientADAttributeMatchesPatterns", "ExceptIfRecipientADAttributeMatchesPatterns", delegate(RecipientAttributeMatchesPredicate predicate, Pattern[] patterns)
			{
				predicate.UseLegacyRegex = useLegacyRegex;
				predicate.Patterns = patterns;
			});
			Utils.InitializeAndAddNewPredicateForArrayParameter<AnyOfToHeaderPredicate, RecipientIdParameter>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "AnyOfToHeader", "ExceptIfAnyOfToHeader", delegate(AnyOfToHeaderPredicate predicate, RecipientIdParameter[] addresses)
			{
				predicate.Addresses = Utils.BuildSmtpAddressArray(addresses, recipientSession);
			});
			Utils.InitializeAndAddNewPredicateForArrayParameter<AnyOfToHeaderMemberOfPredicate, RecipientIdParameter>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "AnyOfToHeaderMemberOf", "ExceptIfAnyOfToHeaderMemberOf", delegate(AnyOfToHeaderMemberOfPredicate predicate, RecipientIdParameter[] addresses)
			{
				predicate.Addresses = Utils.BuildSmtpAddressArray(addresses, recipientSession);
			});
			Utils.InitializeAndAddNewPredicateForArrayParameter<AnyOfCcHeaderPredicate, RecipientIdParameter>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "AnyOfCcHeader", "ExceptIfAnyOfCcHeader", delegate(AnyOfCcHeaderPredicate predicate, RecipientIdParameter[] addresses)
			{
				predicate.Addresses = Utils.BuildSmtpAddressArray(addresses, recipientSession);
			});
			Utils.InitializeAndAddNewPredicateForArrayParameter<AnyOfCcHeaderMemberOfPredicate, RecipientIdParameter>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "AnyOfCcHeaderMemberOf", "ExceptIfAnyOfCcHeaderMemberOf", delegate(AnyOfCcHeaderMemberOfPredicate predicate, RecipientIdParameter[] addresses)
			{
				predicate.Addresses = Utils.BuildSmtpAddressArray(addresses, recipientSession);
			});
			Utils.InitializeAndAddNewPredicateForArrayParameter<AnyOfToCcHeaderPredicate, RecipientIdParameter>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "AnyOfToCcHeader", "ExceptIfAnyOfToCcHeader", delegate(AnyOfToCcHeaderPredicate predicate, RecipientIdParameter[] addresses)
			{
				predicate.Addresses = Utils.BuildSmtpAddressArray(addresses, recipientSession);
			});
			Utils.InitializeAndAddNewPredicateForArrayParameter<AnyOfToCcHeaderMemberOfPredicate, RecipientIdParameter>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "AnyOfToCcHeaderMemberOf", "ExceptIfAnyOfToCcHeaderMemberOf", delegate(AnyOfToCcHeaderMemberOfPredicate predicate, RecipientIdParameter[] addresses)
			{
				predicate.Addresses = Utils.BuildSmtpAddressArray(addresses, recipientSession);
			});
			if (fields.IsModified("HasClassification"))
			{
				string classificationId = (string)fields["HasClassification"];
				ADObjectId classificationADObjectId = Utils.GetClassificationADObjectId(classificationId, dataSession);
				if (classificationADObjectId != null)
				{
					HasClassificationPredicate hasClassificationPredicate = new HasClassificationPredicate(dataSession);
					hasClassificationPredicate.Initialize(availablePredicateMappings);
					hasClassificationPredicate.Classification = classificationADObjectId;
					Utils.InsertPredicateSorted(hasClassificationPredicate, list);
				}
				conditionTypesSpecified.Add(typeof(HasClassificationPredicate));
			}
			if (fields.IsModified("ExceptIfHasClassification"))
			{
				string classificationId2 = (string)fields["ExceptIfHasClassification"];
				ADObjectId classificationADObjectId2 = Utils.GetClassificationADObjectId(classificationId2, dataSession);
				if (classificationADObjectId2 != null)
				{
					HasClassificationPredicate hasClassificationPredicate2 = new HasClassificationPredicate(dataSession);
					hasClassificationPredicate2.Initialize(availablePredicateMappings);
					hasClassificationPredicate2.Classification = classificationADObjectId2;
					Utils.InsertPredicateSorted(hasClassificationPredicate2, list2);
				}
				exceptionTypesSpecified.Add(typeof(HasClassificationPredicate));
			}
			if (fields.IsModified("HasNoClassification"))
			{
				if ((bool)fields["HasNoClassification"])
				{
					HasNoClassificationPredicate hasNoClassificationPredicate = new HasNoClassificationPredicate();
					hasNoClassificationPredicate.Initialize(availablePredicateMappings);
					Utils.InsertPredicateSorted(hasNoClassificationPredicate, list);
				}
				conditionTypesSpecified.Add(typeof(HasNoClassificationPredicate));
			}
			if (fields.IsModified("ExceptIfHasNoClassification"))
			{
				if ((bool)fields["ExceptIfHasNoClassification"])
				{
					HasNoClassificationPredicate hasNoClassificationPredicate2 = new HasNoClassificationPredicate();
					hasNoClassificationPredicate2.Initialize(availablePredicateMappings);
					Utils.InsertPredicateSorted(hasNoClassificationPredicate2, list2);
				}
				exceptionTypesSpecified.Add(typeof(HasNoClassificationPredicate));
			}
			Utils.InitializeAndAddNewPredicateForArrayParameter<SubjectContainsPredicate, Word>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "SubjectContainsWords", "ExceptIfSubjectContainsWords", new Utils.ArrayPredicateInitializer<SubjectContainsPredicate, Word>(Utils.InitializeContainsWordsPredicateWithNormalization));
			Utils.InitializeAndAddNewPredicateForArrayParameter<SubjectOrBodyContainsPredicate, Word>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "SubjectOrBodyContainsWords", "ExceptIfSubjectOrBodyContainsWords", new Utils.ArrayPredicateInitializer<SubjectOrBodyContainsPredicate, Word>(Utils.InitializeContainsWordsPredicateWithNormalization));
			if (fields.IsModified("HeaderContainsMessageHeader"))
			{
				HeaderName? headerName = (HeaderName?)fields["HeaderContainsMessageHeader"];
				Word[] array7 = (Word[])fields["HeaderContainsWords"];
				if (headerName != null && array7 != null && array7.Length > 0)
				{
					HeaderContainsPredicate headerContainsPredicate = new HeaderContainsPredicate();
					headerContainsPredicate.Initialize(availablePredicateMappings);
					headerContainsPredicate.MessageHeader = headerName.Value;
					headerContainsPredicate.Words = array7;
					Utils.InsertPredicateSorted(headerContainsPredicate, list);
				}
				conditionTypesSpecified.Add(typeof(HeaderContainsPredicate));
			}
			if (fields.IsModified("ExceptIfHeaderContainsMessageHeader"))
			{
				HeaderName? headerName2 = (HeaderName?)fields["ExceptIfHeaderContainsMessageHeader"];
				Word[] array8 = (Word[])fields["ExceptIfHeaderContainsWords"];
				if (headerName2 != null && array8 != null && array8.Length > 0)
				{
					HeaderContainsPredicate headerContainsPredicate2 = new HeaderContainsPredicate();
					headerContainsPredicate2.Initialize(availablePredicateMappings);
					headerContainsPredicate2.MessageHeader = headerName2.Value;
					headerContainsPredicate2.Words = array8;
					Utils.InsertPredicateSorted(headerContainsPredicate2, list2);
				}
				exceptionTypesSpecified.Add(typeof(HeaderContainsPredicate));
			}
			Utils.InitializeAndAddNewPredicateForArrayParameter<FromAddressContainsPredicate, Word>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "FromAddressContainsWords", "ExceptIfFromAddressContainsWords", new Utils.ArrayPredicateInitializer<FromAddressContainsPredicate, Word>(Utils.InitializeContainsWordsPredicateWithNormalization));
			Utils.InitializeAndAddNewPredicateForArrayParameter<SubjectMatchesPredicate, Pattern>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "SubjectMatchesPatterns", "ExceptIfSubjectMatchesPatterns", delegate(SubjectMatchesPredicate predicate, Pattern[] patterns)
			{
				Utils.InitializeMatchesPatternsPredicateWithNormalization(predicate, patterns, useLegacyRegex);
			});
			Utils.InitializeAndAddNewPredicateForArrayParameter<SenderDomainIsPredicate, Word>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "SenderDomainIs", "ExceptIfSenderDomainIs", delegate(SenderDomainIsPredicate predicate, Word[] words)
			{
				predicate.Words = Utils.NormalizeWords(words);
			});
			Utils.InitializeAndAddNewPredicateForArrayParameter<RecipientDomainIsPredicate, Word>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "RecipientDomainIs", "ExceptIfRecipientDomainIs", delegate(RecipientDomainIsPredicate predicate, Word[] words)
			{
				predicate.Words = Utils.NormalizeWords(words);
			});
			Utils.InitializeAndAddNewPredicateForArrayParameter<SubjectOrBodyMatchesPredicate, Pattern>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "SubjectOrBodyMatchesPatterns", "ExceptIfSubjectOrBodyMatchesPatterns", delegate(SubjectOrBodyMatchesPredicate predicate, Pattern[] patterns)
			{
				Utils.InitializeMatchesPatternsPredicateWithNormalization(predicate, patterns, useLegacyRegex);
			});
			if (fields.IsModified("HeaderMatchesMessageHeader"))
			{
				HeaderName? headerName3 = (HeaderName?)fields["HeaderMatchesMessageHeader"];
				Pattern[] array9 = (Pattern[])fields["HeaderMatchesPatterns"];
				if (headerName3 != null && array9 != null && array9.Length > 0)
				{
					HeaderMatchesPredicate headerMatchesPredicate = new HeaderMatchesPredicate(useLegacyRegex);
					headerMatchesPredicate.Initialize(availablePredicateMappings);
					headerMatchesPredicate.MessageHeader = headerName3.Value;
					headerMatchesPredicate.Patterns = array9;
					Utils.InsertPredicateSorted(headerMatchesPredicate, list);
				}
				conditionTypesSpecified.Add(typeof(HeaderMatchesPredicate));
			}
			if (fields.IsModified("ExceptIfHeaderMatchesMessageHeader"))
			{
				HeaderName? headerName4 = (HeaderName?)fields["ExceptIfHeaderMatchesMessageHeader"];
				Pattern[] array10 = (Pattern[])fields["ExceptIfHeaderMatchesPatterns"];
				if (headerName4 != null && array10 != null && array10.Length > 0)
				{
					HeaderMatchesPredicate headerMatchesPredicate2 = new HeaderMatchesPredicate(useLegacyRegex);
					headerMatchesPredicate2.Initialize(availablePredicateMappings);
					headerMatchesPredicate2.MessageHeader = headerName4.Value;
					headerMatchesPredicate2.Patterns = array10;
					Utils.InsertPredicateSorted(headerMatchesPredicate2, list2);
				}
				exceptionTypesSpecified.Add(typeof(HeaderMatchesPredicate));
			}
			Utils.InitializeAndAddNewPredicateForArrayParameter<FromAddressMatchesPredicate, Pattern>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "FromAddressMatchesPatterns", "ExceptIfFromAddressMatchesPatterns", delegate(FromAddressMatchesPredicate predicate, Pattern[] patterns)
			{
				Utils.InitializeMatchesPatternsPredicateWithNormalization(predicate, patterns, useLegacyRegex);
			});
			Utils.InitializeAndAddNewPredicateForArrayParameter<AttachmentNameMatchesPredicate, Pattern>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "AttachmentNameMatchesPatterns", "ExceptIfAttachmentNameMatchesPatterns", delegate(AttachmentNameMatchesPredicate predicate, Pattern[] patterns)
			{
				predicate.UseLegacyRegex = useLegacyRegex;
				predicate.Patterns = patterns;
			});
			Utils.InitializeAndAddNewPredicateForArrayParameter<AttachmentExtensionMatchesWordsPredicate, Word>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "AttachmentExtensionMatchesWords", "ExceptIfAttachmentExtensionMatchesWords", delegate(AttachmentExtensionMatchesWordsPredicate predicate, Word[] words)
			{
				predicate.Words = words;
			});
			Utils.InitializeAndAddNewPredicateForArrayParameter<ContentCharacterSetContainsWordsPredicate, Word>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "ContentCharacterSetContainsWords", "ExceptIfContentCharacterSetContainsWords", delegate(ContentCharacterSetContainsWordsPredicate predicate, Word[] words)
			{
				predicate.Words = words;
			});
			Utils.InitializeAndAddNewPredicateForNullableParameter<SclOverPredicate, SclValue>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "SCLOver", "ExceptIfSCLOver", delegate(SclOverPredicate predicate, SclValue sclValue)
			{
				predicate.SclValue = sclValue;
			});
			Utils.InitializeAndAddNewPredicateForNullableParameter<AttachmentSizeOverPredicate, ByteQuantifiedSize>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "AttachmentSizeOver", "ExceptIfAttachmentSizeOver", delegate(AttachmentSizeOverPredicate predicate, ByteQuantifiedSize size)
			{
				predicate.Size = size;
			});
			Utils.InitializeAndAddNewPredicateForNullableParameter<MessageSizeOverPredicate, ByteQuantifiedSize>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "MessageSizeOver", "ExceptIfMessageSizeOver", delegate(MessageSizeOverPredicate predicate, ByteQuantifiedSize size)
			{
				predicate.Size = size;
			});
			Utils.InitializeAndAddNewPredicateForNullableParameter<WithImportancePredicate, Importance>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "WithImportance", "ExceptIfWithImportance", delegate(WithImportancePredicate predicate, Importance importance)
			{
				predicate.Importance = importance;
			});
			Utils.InitializeAndAddNewPredicateForNullableParameter<MessageTypeMatchesPredicate, MessageType>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "MessageTypeMatches", "ExceptIfMessageTypeMatches", delegate(MessageTypeMatchesPredicate predicate, MessageType messageType)
			{
				predicate.MessageType = messageType;
			});
			Utils.InitializeAndAddNewPredicateForArrayParameter<RecipientAddressContainsWordsPredicate, Word>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "RecipientAddressContainsWords", "ExceptIfRecipientAddressContainsWords", delegate(RecipientAddressContainsWordsPredicate predicate, Word[] words)
			{
				predicate.Words = Utils.NormalizeWords(words);
			});
			Utils.InitializeAndAddNewPredicateForArrayParameter<RecipientAddressMatchesPatternsPredicate, Pattern>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "RecipientAddressMatchesPatterns", "ExceptIfRecipientAddressMatchesPatterns", delegate(RecipientAddressMatchesPatternsPredicate predicate, Pattern[] patterns)
			{
				predicate.UseLegacyRegex = useLegacyRegex;
				predicate.Patterns = Utils.NormalizePatterns(patterns);
			});
			Utils.InitializeAndAddNewPredicateForArrayParameter<SenderInRecipientListPredicate, Word>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "SenderInRecipientList", "ExceptIfSenderInRecipientList", delegate(SenderInRecipientListPredicate predicate, Word[] lists)
			{
				predicate.Lists = lists;
			});
			if (fields.IsModified("SenderIpRanges"))
			{
				IEnumerable<IPRange> enumerable = (IEnumerable<IPRange>)fields["SenderIpRanges"];
				if (enumerable != null && enumerable.Any<IPRange>())
				{
					SenderIpRangesPredicate senderIpRangesPredicate = new SenderIpRangesPredicate();
					senderIpRangesPredicate.Initialize(availablePredicateMappings);
					senderIpRangesPredicate.IpRanges = enumerable.ToList<IPRange>();
					Utils.InsertPredicateSorted(senderIpRangesPredicate, list);
				}
				conditionTypesSpecified.Add(typeof(SenderIpRangesPredicate));
			}
			if (fields.IsModified("ExceptIfSenderIpRanges"))
			{
				IEnumerable<IPRange> enumerable2 = (IEnumerable<IPRange>)fields["ExceptIfSenderIpRanges"];
				if (enumerable2 != null && enumerable2.Any<IPRange>())
				{
					SenderIpRangesPredicate senderIpRangesPredicate2 = new SenderIpRangesPredicate();
					senderIpRangesPredicate2.Initialize(availablePredicateMappings);
					senderIpRangesPredicate2.IpRanges = enumerable2.ToList<IPRange>();
					Utils.InsertPredicateSorted(senderIpRangesPredicate2, list2);
				}
				exceptionTypesSpecified.Add(typeof(SenderIpRangesPredicate));
			}
			Utils.InitializeAndAddNewPredicateForArrayParameter<RecipientInSenderListPredicate, Word>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "RecipientInSenderList", "ExceptIfRecipientInSenderList", delegate(RecipientInSenderListPredicate predicate, Word[] lists)
			{
				predicate.Lists = lists;
			});
			Utils.InitializeAndAddNewPredicateForArrayParameter<AttachmentContainsWordsPredicate, Word>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "AttachmentContainsWords", "ExceptIfAttachmentContainsWords", new Utils.ArrayPredicateInitializer<AttachmentContainsWordsPredicate, Word>(Utils.InitializeContainsWordsPredicateWithNormalization));
			Utils.InitializeAndAddNewPredicateForArrayParameter<AttachmentMatchesPatternsPredicate, Pattern>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "AttachmentMatchesPatterns", "ExceptIfAttachmentMatchesPatterns", delegate(AttachmentMatchesPatternsPredicate predicate, Pattern[] patterns)
			{
				Utils.InitializeMatchesPatternsPredicateWithNormalization(predicate, patterns, useLegacyRegex);
			});
			Utils.InitializeAndAddNewPredicateForBoolParameter<AttachmentIsUnsupportedPredicate>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "AttachmentIsUnsupported", "ExceptIfAttachmentIsUnsupported");
			Utils.InitializeAndAddNewPredicateForBoolParameter<AttachmentProcessingLimitExceededPredicate>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "AttachmentProcessingLimitExceeded", "ExceptIfAttachmentProcessingLimitExceeded");
			Utils.InitializeAndAddNewPredicateForBoolParameter<AttachmentHasExecutableContentPredicate>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "AttachmentHasExecutableContent", "ExceptIfAttachmentHasExecutableContent");
			Utils.InitializeAndAddNewPredicateForBoolParameter<AttachmentIsPasswordProtectedPredicate>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "AttachmentIsPasswordProtected", "ExceptIfAttachmentIsPasswordProtected");
			Utils.InitializeAndAddNewPredicateForArrayParameter<AttachmentPropertyContainsWordsPredicate, Word>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "AttachmentPropertyContainsWords", "ExceptIfAttachmentPropertyContainsWords", delegate(AttachmentPropertyContainsWordsPredicate predicate, Word[] words)
			{
				predicate.Words = words;
			});
			Utils.InitializeAndAddNewPredicateForArrayParameter<AnyOfRecipientAddressContainsPredicate, Word>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "AnyOfRecipientAddressContainsWords", "ExceptIfAnyOfRecipientAddressContainsWords", delegate(AnyOfRecipientAddressContainsPredicate predicate, Word[] words)
			{
				predicate.Words = Utils.NormalizeWords(words);
			});
			Utils.InitializeAndAddNewPredicateForArrayParameter<AnyOfRecipientAddressMatchesPredicate, Pattern>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "AnyOfRecipientAddressMatchesPatterns", "ExceptIfAnyOfRecipientAddressMatchesPatterns", delegate(AnyOfRecipientAddressMatchesPredicate predicate, Pattern[] patterns)
			{
				predicate.UseLegacyRegex = useLegacyRegex;
				predicate.Patterns = Utils.NormalizePatterns(patterns);
			});
			Utils.InitializeAndAddNewPredicateForBoolParameter<HasSenderOverridePredicate>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "HasSenderOverride", "ExceptIfHasSenderOverride");
			Utils.InitializeAndAddNewPredicateForArrayParameter<MessageContainsDataClassificationsPredicate, Hashtable>(fields, availablePredicateMappings, list, list2, conditionTypesSpecified, exceptionTypesSpecified, "MessageContainsDataClassifications", "ExceptIfMessageContainsDataClassifications", delegate(MessageContainsDataClassificationsPredicate predicate, Hashtable[] dataClassifications)
			{
				predicate.DataClassifications = dataClassifications;
			});
			conditions = list.ToArray();
			exceptions = list2.ToArray();
		}

		internal static TransportRuleAction[] BuildActionsFromParameters(PropertyBag fields, IRecipientSession recipientSession, IConfigDataProvider dataSession, out List<Type> actionTypesSpecified)
		{
			TypeMapping[] availableActionMappings = TransportRuleAction.GetAvailableActionMappings();
			List<TransportRuleAction> list = new List<TransportRuleAction>();
			actionTypesSpecified = new List<Type>();
			if (fields.IsModified("PrependSubject"))
			{
				SubjectPrefix? subjectPrefix = (SubjectPrefix?)fields["PrependSubject"];
				if (subjectPrefix != null && !string.IsNullOrEmpty(subjectPrefix.Value.ToString()))
				{
					PrependSubjectAction prependSubjectAction = new PrependSubjectAction();
					prependSubjectAction.Initialize(availableActionMappings);
					prependSubjectAction.Prefix = subjectPrefix.Value;
					Utils.InsertActionSorted(prependSubjectAction, list);
				}
				actionTypesSpecified.Add(typeof(PrependSubjectAction));
			}
			if (fields.IsModified("SetAuditSeverity"))
			{
				string text = (string)fields["SetAuditSeverity"];
				AuditSeverityLevel auditSeverityLevel;
				if (Enum.TryParse<AuditSeverityLevel>(text, out auditSeverityLevel))
				{
					SetAuditSeverityAction setAuditSeverityAction = new SetAuditSeverityAction();
					setAuditSeverityAction.Initialize(availableActionMappings);
					setAuditSeverityAction.SeverityLevel = text;
					Utils.InsertActionSorted(setAuditSeverityAction, list);
				}
				actionTypesSpecified.Add(typeof(SetAuditSeverityAction));
			}
			if (fields.IsModified("ApplyClassification"))
			{
				string classificationId = (string)fields["ApplyClassification"];
				ADObjectId classificationADObjectId = Utils.GetClassificationADObjectId(classificationId, dataSession);
				if (classificationADObjectId != null)
				{
					ApplyClassificationAction applyClassificationAction = new ApplyClassificationAction(dataSession);
					applyClassificationAction.Initialize(availableActionMappings);
					applyClassificationAction.Classification = classificationADObjectId;
					Utils.InsertActionSorted(applyClassificationAction, list);
				}
				actionTypesSpecified.Add(typeof(ApplyClassificationAction));
			}
			if (fields.IsModified("ApplyHtmlDisclaimerText"))
			{
				DisclaimerText? disclaimerText = (DisclaimerText?)fields["ApplyHtmlDisclaimerText"];
				if (disclaimerText != null)
				{
					ApplyHtmlDisclaimerAction applyHtmlDisclaimerAction = new ApplyHtmlDisclaimerAction();
					applyHtmlDisclaimerAction.Initialize(availableActionMappings);
					applyHtmlDisclaimerAction.Text = disclaimerText.Value;
					if (fields.IsModified("ApplyHtmlDisclaimerLocation"))
					{
						applyHtmlDisclaimerAction.Location = ((DisclaimerLocation?)fields["ApplyHtmlDisclaimerLocation"]).Value;
					}
					else
					{
						applyHtmlDisclaimerAction.Location = DisclaimerLocation.Append;
					}
					if (fields.IsModified("ApplyHtmlDisclaimerFallbackAction"))
					{
						applyHtmlDisclaimerAction.FallbackAction = ((DisclaimerFallbackAction?)fields["ApplyHtmlDisclaimerFallbackAction"]).Value;
					}
					else
					{
						applyHtmlDisclaimerAction.FallbackAction = DisclaimerFallbackAction.Wrap;
					}
					Utils.InsertActionSorted(applyHtmlDisclaimerAction, list);
				}
				actionTypesSpecified.Add(typeof(ApplyHtmlDisclaimerAction));
			}
			if (fields.IsModified("ApplyRightsProtectionTemplate"))
			{
				RmsTemplateIdentity rmsTemplateIdentity = (RmsTemplateIdentity)fields["ResolvedRmsTemplateIdentity"];
				if (rmsTemplateIdentity != null && rmsTemplateIdentity.TemplateId != Guid.Empty && !string.IsNullOrEmpty(rmsTemplateIdentity.TemplateName))
				{
					RightsProtectMessageAction rightsProtectMessageAction = new RightsProtectMessageAction();
					rightsProtectMessageAction.Initialize(availableActionMappings);
					rightsProtectMessageAction.Template = rmsTemplateIdentity;
					Utils.InsertActionSorted(rightsProtectMessageAction, list);
				}
				actionTypesSpecified.Add(typeof(RightsProtectMessageAction));
			}
			if (fields.IsModified("SetSCL"))
			{
				SclValue? sclValue = (SclValue?)fields["SetSCL"];
				if (sclValue != null)
				{
					SetSclAction setSclAction = new SetSclAction();
					setSclAction.Initialize(availableActionMappings);
					setSclAction.SclValue = sclValue.Value;
					Utils.InsertActionSorted(setSclAction, list);
				}
				actionTypesSpecified.Add(typeof(SetSclAction));
			}
			if (fields.IsModified("SetHeaderName"))
			{
				HeaderName? headerName = (HeaderName?)fields["SetHeaderName"];
				HeaderValue? headerValue = (HeaderValue?)fields["SetHeaderValue"];
				if (headerName != null && headerValue != null)
				{
					SetHeaderAction setHeaderAction = new SetHeaderAction();
					setHeaderAction.Initialize(availableActionMappings);
					setHeaderAction.MessageHeader = headerName.Value;
					setHeaderAction.HeaderValue = headerValue.Value;
					Utils.InsertActionSorted(setHeaderAction, list);
				}
				actionTypesSpecified.Add(typeof(SetHeaderAction));
			}
			if (fields.IsModified("RemoveHeader"))
			{
				HeaderName? headerName2 = (HeaderName?)fields["RemoveHeader"];
				if (headerName2 != null)
				{
					RemoveHeaderAction removeHeaderAction = new RemoveHeaderAction();
					removeHeaderAction.Initialize(availableActionMappings);
					removeHeaderAction.MessageHeader = headerName2.Value;
					Utils.InsertActionSorted(removeHeaderAction, list);
				}
				actionTypesSpecified.Add(typeof(RemoveHeaderAction));
			}
			Utils.InitializeAndAddNewActionForParameter<AddToRecipientAction, RecipientIdParameter>(fields, availableActionMappings, list, actionTypesSpecified, "AddToRecipients", delegate(AddToRecipientAction action, RecipientIdParameter[] addresses)
			{
				action.Addresses = Utils.BuildSmtpAddressArray(addresses, recipientSession);
			});
			Utils.InitializeAndAddNewActionForParameter<CopyToAction, RecipientIdParameter>(fields, availableActionMappings, list, actionTypesSpecified, "CopyTo", delegate(CopyToAction action, RecipientIdParameter[] addresses)
			{
				action.Addresses = Utils.BuildSmtpAddressArray(addresses, recipientSession);
			});
			Utils.InitializeAndAddNewActionForParameter<BlindCopyToAction, RecipientIdParameter>(fields, availableActionMappings, list, actionTypesSpecified, "BlindCopyTo", delegate(BlindCopyToAction action, RecipientIdParameter[] addresses)
			{
				action.Addresses = Utils.BuildSmtpAddressArray(addresses, recipientSession);
			});
			if (fields.IsModified("AddManagerAsRecipientType"))
			{
				AddedRecipientType? addedRecipientType = (AddedRecipientType?)fields["AddManagerAsRecipientType"];
				if (addedRecipientType != null)
				{
					AddManagerAsRecipientTypeAction addManagerAsRecipientTypeAction = new AddManagerAsRecipientTypeAction();
					addManagerAsRecipientTypeAction.Initialize(availableActionMappings);
					addManagerAsRecipientTypeAction.RecipientType = addedRecipientType.Value;
					Utils.InsertActionSorted(addManagerAsRecipientTypeAction, list);
				}
				actionTypesSpecified.Add(typeof(AddManagerAsRecipientTypeAction));
			}
			if (fields.IsModified("ModerateMessageByUser"))
			{
				RecipientIdParameter[] array = (RecipientIdParameter[])fields["ModerateMessageByUser"];
				if (array != null && array.Length > 0)
				{
					int num = Utils.IsMultiTeancyEnabled() ? 10 : 25;
					if (array.Length > num)
					{
						throw new ArgumentException(Strings.ErrorTooManyModerators(num));
					}
					SmtpAddress[] array2 = Utils.BuildSmtpAddressArray(array, recipientSession);
					foreach (SmtpAddress smtpAddress in array2)
					{
						if (!Utils.IsValidModerator(smtpAddress.ToString(), recipientSession))
						{
							throw new ArgumentException(Strings.InvalidRecipientForModerationAction(smtpAddress.ToString()));
						}
					}
					ModerateMessageByUserAction moderateMessageByUserAction = new ModerateMessageByUserAction();
					moderateMessageByUserAction.Initialize(availableActionMappings);
					moderateMessageByUserAction.Addresses = array2;
					Utils.InsertActionSorted(moderateMessageByUserAction, list);
				}
				actionTypesSpecified.Add(typeof(ModerateMessageByUserAction));
			}
			if (fields.IsModified("ModerateMessageByManager"))
			{
				if ((bool)fields["ModerateMessageByManager"])
				{
					ModerateMessageByManagerAction moderateMessageByManagerAction = new ModerateMessageByManagerAction();
					moderateMessageByManagerAction.Initialize(availableActionMappings);
					Utils.InsertActionSorted(moderateMessageByManagerAction, list);
				}
				actionTypesSpecified.Add(typeof(ModerateMessageByManagerAction));
			}
			Utils.InitializeAndAddNewActionForParameter<RedirectMessageAction, RecipientIdParameter>(fields, availableActionMappings, list, actionTypesSpecified, "RedirectMessageTo", delegate(RedirectMessageAction action, RecipientIdParameter[] addresses)
			{
				action.Addresses = Utils.BuildSmtpAddressArray(addresses, recipientSession);
			});
			DsnText? dsnText;
			if (fields.IsModified("RejectMessageReasonText"))
			{
				dsnText = (DsnText?)fields["RejectMessageReasonText"];
			}
			else
			{
				dsnText = new DsnText?(new DsnText(Utils.DefaultRejectText.Value));
			}
			RejectEnhancedStatus? rejectEnhancedStatus;
			if (fields.IsModified("RejectMessageEnhancedStatusCode"))
			{
				rejectEnhancedStatus = (RejectEnhancedStatus?)fields["RejectMessageEnhancedStatusCode"];
			}
			else
			{
				rejectEnhancedStatus = Utils.DefaultEnhancedStatusCode;
			}
			if (fields.IsModified("NotifySender"))
			{
				NotifySenderType? notifySenderType = (NotifySenderType?)fields["NotifySender"];
				if (notifySenderType != null)
				{
					TypeMapping[] supportedActions = availableActionMappings;
					NotifySenderType? notifySenderType2 = notifySenderType;
					DsnText? rejectText = new DsnText?(dsnText ?? new DsnText(Utils.DefaultRejectText.Value));
					RejectEnhancedStatus? rejectEnhancedStatus2 = rejectEnhancedStatus;
					Utils.BuildNotifySenderActionFromParameters(supportedActions, notifySenderType2, rejectText, (rejectEnhancedStatus2 != null) ? new RejectEnhancedStatus?(rejectEnhancedStatus2.GetValueOrDefault()) : Utils.DefaultEnhancedStatusCode, list);
				}
				else if (fields.IsModified("RejectMessageReasonText") || fields.IsModified("RejectMessageEnhancedStatusCode"))
				{
					if (dsnText != null && rejectEnhancedStatus != null)
					{
						Utils.BuildRejectMessageActionFromParameters(availableActionMappings, dsnText, rejectEnhancedStatus, list);
					}
					actionTypesSpecified.Add(typeof(RejectMessageAction));
				}
				actionTypesSpecified.Add(typeof(NotifySenderAction));
			}
			else if (fields.IsModified("RejectMessageReasonText") || fields.IsModified("RejectMessageEnhancedStatusCode"))
			{
				if (dsnText != null && rejectEnhancedStatus != null)
				{
					Utils.BuildRejectMessageActionFromParameters(availableActionMappings, dsnText, rejectEnhancedStatus, list);
				}
				actionTypesSpecified.Add(typeof(RejectMessageAction));
			}
			if (fields.IsModified("DeleteMessage"))
			{
				if ((bool)fields["DeleteMessage"])
				{
					DeleteMessageAction deleteMessageAction = new DeleteMessageAction();
					deleteMessageAction.Initialize(availableActionMappings);
					Utils.InsertActionSorted(deleteMessageAction, list);
				}
				actionTypesSpecified.Add(typeof(DeleteMessageAction));
			}
			if (fields.IsModified("SmtpRejectMessageRejectText") || fields.IsModified("SmtpRejectMessageRejectStatusCode"))
			{
				RejectText? rejectText2;
				if (fields.IsModified("SmtpRejectMessageRejectText"))
				{
					rejectText2 = (RejectText?)fields["SmtpRejectMessageRejectText"];
				}
				else
				{
					rejectText2 = new RejectText?(Utils.DefaultRejectText);
				}
				RejectStatusCode? rejectStatusCode;
				if (fields.IsModified("SmtpRejectMessageRejectStatusCode"))
				{
					rejectStatusCode = (RejectStatusCode?)fields["SmtpRejectMessageRejectStatusCode"];
				}
				else
				{
					rejectStatusCode = new RejectStatusCode?(Utils.DefaultRejectStatusCode);
				}
				if (rejectText2 != null && rejectStatusCode != null)
				{
					SmtpRejectMessageAction smtpRejectMessageAction = new SmtpRejectMessageAction();
					smtpRejectMessageAction.Initialize(availableActionMappings);
					smtpRejectMessageAction.RejectReason = rejectText2.Value;
					smtpRejectMessageAction.StatusCode = rejectStatusCode.Value;
					Utils.InsertActionSorted(smtpRejectMessageAction, list);
				}
				actionTypesSpecified.Add(typeof(SmtpRejectMessageAction));
			}
			if (fields.IsModified("Disconnect"))
			{
				if ((bool)fields["Disconnect"])
				{
					DisconnectAction disconnectAction = new DisconnectAction();
					disconnectAction.Initialize(availableActionMappings);
					Utils.InsertActionSorted(disconnectAction, list);
				}
				actionTypesSpecified.Add(typeof(DisconnectAction));
			}
			if (fields.IsModified("Quarantine"))
			{
				if ((bool)fields["Quarantine"])
				{
					QuarantineAction quarantineAction = new QuarantineAction();
					quarantineAction.Initialize(availableActionMappings);
					Utils.InsertActionSorted(quarantineAction, list);
				}
				actionTypesSpecified.Add(typeof(QuarantineAction));
			}
			if (fields.IsModified("LogEventText"))
			{
				EventLogText? eventLogText = (EventLogText?)fields["LogEventText"];
				if (eventLogText != null)
				{
					LogEventAction logEventAction = new LogEventAction();
					logEventAction.Initialize(availableActionMappings);
					logEventAction.EventMessage = eventLogText.Value;
					Utils.InsertActionSorted(logEventAction, list);
				}
				actionTypesSpecified.Add(typeof(LogEventAction));
			}
			if (fields.IsModified("StopRuleProcessing"))
			{
				if ((bool)fields["StopRuleProcessing"])
				{
					StopRuleProcessingAction stopRuleProcessingAction = new StopRuleProcessingAction();
					stopRuleProcessingAction.Initialize(availableActionMappings);
					Utils.InsertActionSorted(stopRuleProcessingAction, list);
				}
				actionTypesSpecified.Add(typeof(StopRuleProcessingAction));
			}
			if (fields.IsModified("GenerateIncidentReport"))
			{
				RecipientIdParameter recipientIdParameter = (RecipientIdParameter)fields["GenerateIncidentReport"];
				if (recipientIdParameter != null)
				{
					GenerateIncidentReportAction generateIncidentReportAction = new GenerateIncidentReportAction();
					generateIncidentReportAction.Initialize(availableActionMappings);
					RecipientIdParameter[] recipientIds = new RecipientIdParameter[]
					{
						recipientIdParameter
					};
					generateIncidentReportAction.ReportDestination = Utils.BuildSmtpAddressArray(recipientIds, recipientSession)[0];
					if (fields.IsModified("IncidentReportContent") && fields["IncidentReportContent"] != null)
					{
						generateIncidentReportAction.IncidentReportContent = (IncidentReportContent[])fields["IncidentReportContent"];
						generateIncidentReportAction.IncidentReportOriginalMail = (generateIncidentReportAction.IncidentReportContent.Contains(IncidentReportContent.AttachOriginalMail) ? IncidentReportOriginalMail.IncludeOriginalMail : IncidentReportOriginalMail.DoNotIncludeOriginalMail);
					}
					else if (fields.IsModified("IncidentReportOriginalMail") && fields["IncidentReportOriginalMail"] != null)
					{
						generateIncidentReportAction.IncidentReportOriginalMail = (IncidentReportOriginalMail)fields["IncidentReportOriginalMail"];
						generateIncidentReportAction.IncidentReportContent = GenerateIncidentReport.GetLegacyContentItems(generateIncidentReportAction.IncidentReportOriginalMail).ToArray();
					}
					else
					{
						generateIncidentReportAction.IncidentReportOriginalMail = IncidentReportOriginalMail.DoNotIncludeOriginalMail;
						generateIncidentReportAction.IncidentReportContent = GenerateIncidentReport.GetLegacyContentItems(generateIncidentReportAction.IncidentReportOriginalMail).ToArray();
					}
					Utils.InsertActionSorted(generateIncidentReportAction, list);
				}
				actionTypesSpecified.Add(typeof(GenerateIncidentReportAction));
			}
			if (fields.IsModified("GenerateNotification"))
			{
				DisclaimerText? disclaimerText2 = (DisclaimerText?)fields["GenerateNotification"];
				if (disclaimerText2 != null && !string.IsNullOrEmpty(disclaimerText2.Value.ToString()))
				{
					GenerateNotificationAction generateNotificationAction = new GenerateNotificationAction(disclaimerText2.Value);
					generateNotificationAction.Initialize(availableActionMappings);
					Utils.InsertActionSorted(generateNotificationAction, list);
				}
				actionTypesSpecified.Add(typeof(GenerateNotificationAction));
			}
			if (fields.IsModified("RouteMessageOutboundConnector"))
			{
				OutboundConnectorIdParameter outboundConnectorIdParameter = (OutboundConnectorIdParameter)fields["RouteMessageOutboundConnector"];
				if (outboundConnectorIdParameter != null && !string.IsNullOrEmpty(outboundConnectorIdParameter.RawIdentity))
				{
					RouteMessageOutboundConnectorAction routeMessageOutboundConnectorAction = new RouteMessageOutboundConnectorAction();
					routeMessageOutboundConnectorAction.Initialize(availableActionMappings);
					routeMessageOutboundConnectorAction.ConnectorName = outboundConnectorIdParameter.RawIdentity;
					Utils.InsertActionSorted(routeMessageOutboundConnectorAction, list);
				}
				actionTypesSpecified.Add(typeof(RouteMessageOutboundConnectorAction));
			}
			if (fields.IsModified("RouteMessageOutboundRequireTls"))
			{
				if ((bool)fields["RouteMessageOutboundRequireTls"])
				{
					RouteMessageOutboundRequireTlsAction routeMessageOutboundRequireTlsAction = new RouteMessageOutboundRequireTlsAction();
					routeMessageOutboundRequireTlsAction.Initialize(availableActionMappings);
					Utils.InsertActionSorted(routeMessageOutboundRequireTlsAction, list);
				}
				actionTypesSpecified.Add(typeof(RouteMessageOutboundRequireTlsAction));
			}
			if (fields.IsModified("ApplyOME"))
			{
				if ((bool)fields["ApplyOME"])
				{
					ApplyOMEAction applyOMEAction = new ApplyOMEAction();
					applyOMEAction.Initialize(availableActionMappings);
					Utils.InsertActionSorted(applyOMEAction, list);
				}
				actionTypesSpecified.Add(typeof(ApplyOMEAction));
			}
			if (fields.IsModified("RemoveOME"))
			{
				if ((bool)fields["RemoveOME"])
				{
					RemoveOMEAction removeOMEAction = new RemoveOMEAction();
					removeOMEAction.Initialize(availableActionMappings);
					Utils.InsertActionSorted(removeOMEAction, list);
				}
				actionTypesSpecified.Add(typeof(RemoveOMEAction));
			}
			return list.ToArray();
		}

		internal static void BuildRejectMessageActionFromParameters(TypeMapping[] supportedActions, DsnText? rejectText, RejectEnhancedStatus? rejectEnhancedStatus, List<TransportRuleAction> newActions)
		{
			RejectMessageAction rejectMessageAction = new RejectMessageAction();
			rejectMessageAction.Initialize(supportedActions);
			rejectMessageAction.RejectReason = rejectText.Value;
			rejectMessageAction.EnhancedStatusCode = rejectEnhancedStatus.Value;
			Utils.InsertActionSorted(rejectMessageAction, newActions);
		}

		internal static void BuildNotifySenderActionFromParameters(TypeMapping[] supportedActions, NotifySenderType? notifySenderType, DsnText? rejectText, RejectEnhancedStatus? rejectEnhancedStatus, List<TransportRuleAction> newActions)
		{
			NotifySenderAction notifySenderAction = new NotifySenderAction();
			notifySenderAction.Initialize(supportedActions);
			notifySenderAction.SenderNotificationType = notifySenderType.Value;
			notifySenderAction.RejectReason = rejectText.Value;
			notifySenderAction.EnhancedStatusCode = rejectEnhancedStatus.Value;
			Utils.InsertActionSorted(notifySenderAction, newActions);
		}

		internal static void InsertPredicateSorted(TransportRulePredicate predicate, List<TransportRulePredicate> predicateList)
		{
			int rank = predicate.Rank;
			int num = 0;
			while (num < predicateList.Count && rank >= predicateList[num].Rank)
			{
				num++;
			}
			predicateList.Insert(num, predicate);
		}

		internal static void InsertActionSorted(TransportRuleAction action, List<TransportRuleAction> actionList)
		{
			int rank = action.Rank;
			int num = 0;
			while (num < actionList.Count && rank >= actionList[num].Rank)
			{
				num++;
			}
			actionList.Insert(num, action);
		}

		internal static RecipientIdParameter[] BuildRecipientIdArray(MailEnabledRecipient[] recipients)
		{
			List<RecipientIdParameter> list = new List<RecipientIdParameter>();
			foreach (MailEnabledRecipient recipient in recipients)
			{
				string recipientAddressString = Utils.GetRecipientAddressString(recipient);
				if (!string.IsNullOrEmpty(recipientAddressString))
				{
					list.Add(new RecipientIdParameter(recipientAddressString));
				}
			}
			return list.ToArray();
		}

		internal static RecipientIdParameter[] BuildRecipientIdArray(SmtpAddress[] recipients)
		{
			List<RecipientIdParameter> list = new List<RecipientIdParameter>();
			if (recipients != null)
			{
				foreach (SmtpAddress smtpAddress in recipients)
				{
					list.Add(new RecipientIdParameter(smtpAddress.ToString()));
				}
			}
			return list.ToArray();
		}

		internal static MailEnabledRecipient[] BuildMailEnabledRecipientArray(RecipientIdParameter[] recipientIds, IRecipientSession recipientSession)
		{
			List<MailEnabledRecipient> list = new List<MailEnabledRecipient>();
			foreach (RecipientIdParameter recipientIdParameter in recipientIds)
			{
				IEnumerable<ADRecipient> objects = recipientIdParameter.GetObjects<ADRecipient>(null, recipientSession);
				ADRecipient adrecipient;
				using (IEnumerator<ADRecipient> enumerator = objects.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						throw new ArgumentException(Strings.NoRecipientsForRecipientId(recipientIdParameter.ToString()));
					}
					adrecipient = enumerator.Current;
					if (enumerator.MoveNext())
					{
						throw new ArgumentException(Strings.MoreThanOneRecipientForRecipientId(recipientIdParameter.ToString()));
					}
				}
				if (adrecipient is IADOrgPerson && adrecipient.PrimarySmtpAddress.Equals(SmtpAddress.Empty))
				{
					throw new ArgumentException(Strings.NoSmtpAddressForRecipientId(recipientIdParameter.ToString()));
				}
				list.Add(Utils.GetRecipientMailEnabledRecipient(adrecipient));
			}
			return list.ToArray();
		}

		internal static SmtpAddress[] BuildSmtpAddressArray(RecipientIdParameter[] recipientIds, IRecipientSession recipientSession)
		{
			List<SmtpAddress> list = new List<SmtpAddress>();
			int i = 0;
			while (i < recipientIds.Length)
			{
				RecipientIdParameter recipientIdParameter = recipientIds[i];
				IEnumerable<ADRecipient> objects = recipientIdParameter.GetObjects<ADRecipient>(null, recipientSession);
				ADRecipient adrecipient;
				using (IEnumerator<ADRecipient> enumerator = objects.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						SmtpAddress unicodeSmtpAddress = new SmtpAddress(recipientIdParameter.RawIdentity);
						if (unicodeSmtpAddress.IsValidAddress)
						{
							list.Add(Utils.NormalizeSmtpAddress(unicodeSmtpAddress));
							goto IL_E9;
						}
						throw new ArgumentException(Strings.NoRecipientsForRecipientId(recipientIdParameter.ToString()));
					}
					else
					{
						adrecipient = enumerator.Current;
						if (enumerator.MoveNext())
						{
							throw new ArgumentException(Strings.MoreThanOneRecipientForRecipientId(recipientIdParameter.ToString()));
						}
					}
				}
				goto IL_A4;
				IL_E9:
				i++;
				continue;
				IL_A4:
				IADOrgPerson iadorgPerson = adrecipient as IADOrgPerson;
				if (iadorgPerson != null && iadorgPerson.PrimarySmtpAddress.Equals(SmtpAddress.Empty))
				{
					throw new ArgumentException(Strings.NoSmtpAddressForRecipientId(recipientIdParameter.ToString()));
				}
				list.Add(adrecipient.PrimarySmtpAddress);
				goto IL_E9;
			}
			return list.ToArray();
		}

		internal static string[] BuildRecipientStringList(MailEnabledRecipient[] recipients)
		{
			List<string> list = new List<string>();
			foreach (MailEnabledRecipient mailEnabledRecipient in recipients)
			{
				list.Add(mailEnabledRecipient.ToString());
			}
			return list.ToArray();
		}

		internal static string[] BuildWordStringList(Word[] words)
		{
			List<string> list = new List<string>();
			foreach (Word word in words)
			{
				list.Add(word.ToString());
			}
			return list.ToArray();
		}

		internal static string[] BuildPatternStringList(Pattern[] patterns)
		{
			List<string> list = new List<string>();
			foreach (Pattern pattern in patterns)
			{
				list.Add(pattern.ToString());
			}
			return list.ToArray();
		}

		internal static string[] BuildSmtpAddressStringList(SmtpAddress[] addresses, bool shouldBeQuoted = true)
		{
			if (!shouldBeQuoted)
			{
				return (from address in addresses
				select address.ToString()).ToArray<string>();
			}
			return (from address in addresses
			select Utils.QuoteCmdletParameter(address.ToString())).ToArray<string>();
		}

		internal static RuleDescription BuildRuleDescription(Rule rule, int maxDescriptionListLength = 200)
		{
			TransportRulePredicate[] conditions = rule.Conditions;
			TransportRulePredicate[] exceptions = rule.Exceptions;
			TransportRuleAction[] actions = rule.Actions;
			RuleDescription ruleDescription = new RuleDescription();
			if (conditions != null && conditions.Length > 0)
			{
				foreach (TransportRulePredicate transportRulePredicate in conditions)
				{
					transportRulePredicate.MaxDescriptionListLength = maxDescriptionListLength;
					ruleDescription.ConditionDescriptions.Add(transportRulePredicate.Description);
				}
			}
			if (actions != null && actions.Length > 0)
			{
				foreach (TransportRuleAction transportRuleAction in actions)
				{
					transportRuleAction.MaxDescriptionListLength = maxDescriptionListLength;
					ruleDescription.ActionDescriptions.Add(transportRuleAction.Description);
				}
			}
			if (exceptions != null && exceptions.Length > 0)
			{
				foreach (TransportRulePredicate transportRulePredicate2 in exceptions)
				{
					transportRulePredicate2.MaxDescriptionListLength = maxDescriptionListLength;
					ruleDescription.ExceptionDescriptions.Add(transportRulePredicate2.Description);
				}
			}
			if (rule.ActivationDate != null)
			{
				ruleDescription.ActivationDescription = rule.ActivationDate.Value.ToString(Thread.CurrentThread.CurrentCulture.DateTimeFormat);
			}
			if (rule.ExpiryDate != null)
			{
				ruleDescription.ExpiryDescription = rule.ExpiryDate.Value.ToString(Thread.CurrentThread.CurrentCulture.DateTimeFormat);
			}
			return ruleDescription;
		}

		internal static bool IsChildOfRuleContainer(RuleIdParameter ruleId, string ruleCollectionName)
		{
			return ruleId == null || ruleId.InternalADObjectId == null || ruleId.InternalADObjectId.Parent == null || string.Equals(ruleId.InternalADObjectId.Parent.Name, ruleCollectionName, StringComparison.OrdinalIgnoreCase);
		}

		internal static bool IsLegacyRegexPredicate(TransportRulePredicate predicate)
		{
			MatchesPatternsPredicate matchesPatternsPredicate = predicate as MatchesPatternsPredicate;
			if (matchesPatternsPredicate != null && matchesPatternsPredicate.UseLegacyRegex)
			{
				return true;
			}
			BifurcationInfoMatchesPatternsPredicate bifurcationInfoMatchesPatternsPredicate = predicate as BifurcationInfoMatchesPatternsPredicate;
			return bifurcationInfoMatchesPatternsPredicate != null && bifurcationInfoMatchesPatternsPredicate.UseLegacyRegex;
		}

		internal static bool ValidateSingletonAction(TransportRuleAction[] actions)
		{
			if (actions == null)
			{
				return true;
			}
			if (actions.Any((TransportRuleAction action) => action.GetType() == typeof(NotifySenderAction)))
			{
				if (actions.Any((TransportRuleAction action) => action.GetType() == typeof(DeleteMessageAction)))
				{
					return false;
				}
			}
			int num = 1;
			num += (actions.Any((TransportRuleAction action) => action.GetType() == typeof(SetAuditSeverityAction)) ? 1 : 0);
			num += (actions.Any((TransportRuleAction action) => action.GetType() == typeof(GenerateIncidentReportAction)) ? 1 : 0);
			num += (actions.Any((TransportRuleAction action) => action.GetType() == typeof(GenerateNotificationAction)) ? 1 : 0);
			num += (actions.Any((TransportRuleAction action) => action.GetType() == typeof(NotifySenderAction)) ? 1 : 0);
			num += (actions.Any((TransportRuleAction action) => action.GetType() == typeof(StopRuleProcessingAction)) ? 1 : 0);
			if (actions.Length > num)
			{
				if (actions.Any((TransportRuleAction action) => Utils.ActionWhichMustBeSingleton.ContainsKey(action.GetType())))
				{
					return false;
				}
			}
			return true;
		}

		internal static bool ValidateSubtypes(RuleSubType ruleSubType, IEnumerable<TransportRulePredicate> conditions, IEnumerable<TransportRulePredicate> exceptions, IEnumerable<TransportRuleAction> actions, out ArgumentException argumentException)
		{
			argumentException = null;
			List<TransportRulePredicate> list = new List<TransportRulePredicate>();
			if (conditions != null)
			{
				list.AddRange(conditions);
			}
			if (exceptions != null)
			{
				list.AddRange(exceptions);
			}
			TransportRuleAction[] array = (actions == null) ? null : actions.ToArray<TransportRuleAction>();
			foreach (TransportRulePredicate transportRulePredicate in list)
			{
				if (!transportRulePredicate.RuleSubTypes.Any((RuleSubType subtype) => subtype == ruleSubType))
				{
					argumentException = new ArgumentException(Strings.ConditionIncompatibleWithTheSubtype(transportRulePredicate.Name, Enum.GetName(typeof(RuleSubType), ruleSubType)));
					return false;
				}
				if (array != null)
				{
					if (array.Any((TransportRuleAction action) => action is NotifySenderAction))
					{
						if (!transportRulePredicate.RuleSubTypes.Any((RuleSubType subtype) => subtype == RuleSubType.Dlp))
						{
							argumentException = new ArgumentException(Strings.ConditionIncompatibleWithNotifySenderAction(transportRulePredicate.Name));
							return false;
						}
					}
				}
			}
			if (RuleSubType.Dlp == ruleSubType)
			{
				if (!list.Any((TransportRulePredicate condition) => condition.GetType() == typeof(MessageContainsDataClassificationsPredicate)))
				{
					argumentException = new ArgumentException(Strings.DlpRuleMustContainMessageContainsDataClassificationPredicate);
					return false;
				}
			}
			return true;
		}

		internal static bool ValidateNotifySender(IEnumerable<TransportRulePredicate> conditions, IEnumerable<TransportRulePredicate> exceptions, IEnumerable<TransportRuleAction> actions, Action<LocalizedString> warningDelegate, out ArgumentException argumentException)
		{
			argumentException = null;
			if (actions == null)
			{
				return true;
			}
			TransportRuleAction transportRuleAction = actions.FirstOrDefault((TransportRuleAction action) => action is NotifySenderAction);
			if (transportRuleAction != null)
			{
				NotifySenderAction notifySenderAction = transportRuleAction as NotifySenderAction;
				switch (notifySenderAction.SenderNotificationType)
				{
				case NotifySenderType.RejectMessage:
					if (!actions.All((TransportRuleAction action) => action is NotifySenderAction || Utils.ActionsWhichCouldAlwaysCoExist.Contains(action.GetType())))
					{
						argumentException = new ArgumentException(Strings.NotifySenderAndRejectMessageShouldBeSingletonAction);
						return false;
					}
					break;
				case NotifySenderType.RejectUnlessFalsePositiveOverride:
				case NotifySenderType.RejectUnlessSilentOverride:
				case NotifySenderType.RejectUnlessExplicitOverride:
					warningDelegate(Strings.WarningActionWillOnlyBeAppliedIfMessageIsNotRejected);
					break;
				}
				if (conditions != null)
				{
					if (conditions.Any((TransportRulePredicate condition) => condition is MessageContainsDataClassificationsPredicate))
					{
						return true;
					}
				}
				if (exceptions != null)
				{
					if (exceptions.Any((TransportRulePredicate exception) => exception is MessageContainsDataClassificationsPredicate))
					{
						return true;
					}
				}
				argumentException = new ArgumentException(Strings.NotifySenderActionRequiresMcdcPredicate);
				return false;
			}
			return true;
		}

		internal static bool IsUserScopeValid(string parameterToValidate, PropertyBag fields, out ArgumentException argumentException)
		{
			argumentException = null;
			if (!fields.IsModified(parameterToValidate))
			{
				return true;
			}
			ToUserScope? toUserScope = (ToUserScope?)fields[parameterToValidate];
			if (toUserScope != null && Utils.IsMultiTeancyEnabled() && (toUserScope == ToUserScope.ExternalPartner || toUserScope == ToUserScope.ExternalNonPartner))
			{
				argumentException = new ArgumentException(Strings.ExternalScopeInvalidInDc(Enum.GetName(typeof(ToUserScope), toUserScope), Enum.GetName(typeof(ToUserScope), ToUserScope.InOrganization), Enum.GetName(typeof(ToUserScope), ToUserScope.NotInOrganization)));
				return false;
			}
			return true;
		}

		internal static bool ValidateSentToScope(PropertyBag fields, out ArgumentException argumentException, out string errorParameterName)
		{
			errorParameterName = string.Empty;
			if (!Utils.IsUserScopeValid("SentToScope", fields, out argumentException))
			{
				errorParameterName = "SentToScope";
				return false;
			}
			if (!Utils.IsUserScopeValid("ExceptIfSentToScope", fields, out argumentException))
			{
				errorParameterName = "ExceptIfSentToScope";
				return false;
			}
			return true;
		}

		internal static bool IsAttachmentExtensionParameterValid(string parameterToValidate, PropertyBag fields, out ArgumentException argumentException)
		{
			argumentException = null;
			if (fields.IsModified(parameterToValidate))
			{
				Word[] array = (Word[])fields[parameterToValidate];
				if (array != null)
				{
					List<string> list = (from word in array
					where word.ToString().StartsWith(".")
					select word.ToString()).ToList<string>();
					if (list.Any<string>())
					{
						argumentException = new ArgumentException(Strings.AtatchmentExtensionMatchesWordsParameterContainsWordsThatStartWithDot(parameterToValidate, string.Join(",", list)));
						return false;
					}
				}
			}
			return true;
		}

		internal static bool ValidateAttachmentExtensionMatchesWordParameter(PropertyBag fields, out ArgumentException argumentException, out string errorParameterName)
		{
			errorParameterName = string.Empty;
			if (!Utils.IsAttachmentExtensionParameterValid("AttachmentExtensionMatchesWords", fields, out argumentException))
			{
				errorParameterName = "AttachmentExtensionMatchesWords";
				return false;
			}
			if (!Utils.IsAttachmentExtensionParameterValid("ExceptIfAttachmentExtensionMatchesWords", fields, out argumentException))
			{
				errorParameterName = "ExceptIfAttachmentExtensionMatchesWords";
				return false;
			}
			return true;
		}

		internal static bool GetNotifySenderParameterValue(PropertyBag fields, out NotifySenderType notifySender)
		{
			notifySender = NotifySenderType.NotifyOnly;
			if (fields.IsModified("NotifySender"))
			{
				NotifySenderType? notifySenderType = (NotifySenderType?)fields["NotifySender"];
				if (notifySenderType != null)
				{
					notifySender = notifySenderType.Value;
					return true;
				}
			}
			return false;
		}

		internal static bool IsNotifySenderIgnoringRejectParameters(PropertyBag fields)
		{
			NotifySenderType notifySenderType;
			if (Utils.GetNotifySenderParameterValue(fields, out notifySenderType) && NotifySenderType.NotifyOnly == notifySenderType)
			{
				if (fields.IsModified("RejectMessageReasonText") && (DsnText?)fields["RejectMessageReasonText"] != null)
				{
					return true;
				}
				if (fields.IsModified("RejectMessageEnhancedStatusCode") && (RejectEnhancedStatus?)fields["RejectMessageEnhancedStatusCode"] != null)
				{
					return true;
				}
			}
			return false;
		}

		internal static string GetActionName(TransportRuleAction action)
		{
			foreach (TypeMapping typeMapping in TransportRuleAction.GetAvailableActionMappings())
			{
				if (typeMapping.Type.IsInstanceOfType(action))
				{
					return typeMapping.Name;
				}
			}
			return null;
		}

		internal static string QuoteCmdletParameter(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return "''";
			}
			return string.Format("'{0}'", Utils.EscapeCmdletParameter(input));
		}

		internal static string EscapeCmdletParameter(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return string.Empty;
			}
			string text = input.Replace("'", "''");
			text = text.Replace("‘", "‘‘");
			text = text.Replace("’", "’’");
			text = text.Replace("‚", "‚‚");
			return text.Replace("‛", "‛‛");
		}

		private static void InitializeAndAddNewPredicateForBoolParameter<PredicateType>(PropertyBag fields, TypeMapping[] supportedPredicates, List<TransportRulePredicate> conditionList, List<TransportRulePredicate> exceptionList, ICollection<Type> conditionTypesSpecified, ICollection<Type> exceptionTypesSpecified, string conditionParameterName, string exceptionParameterName) where PredicateType : TransportRulePredicate, new()
		{
			Utils.InitializeAndAddNewPredicateForBoolParameter<PredicateType>(fields, supportedPredicates, conditionList, conditionTypesSpecified, conditionParameterName);
			Utils.InitializeAndAddNewPredicateForBoolParameter<PredicateType>(fields, supportedPredicates, exceptionList, exceptionTypesSpecified, exceptionParameterName);
		}

		private static void InitializeAndAddNewPredicateForBoolParameter<PredicateType>(PropertyBag fields, TypeMapping[] supportedPredicates, List<TransportRulePredicate> predicateList, ICollection<Type> predicateTypesSpecified, string parameterName) where PredicateType : TransportRulePredicate, new()
		{
			if (fields.IsModified(parameterName))
			{
				if ((bool)fields[parameterName])
				{
					PredicateType predicateType = Activator.CreateInstance<PredicateType>();
					predicateType.Initialize(supportedPredicates);
					Utils.InsertPredicateSorted(predicateType, predicateList);
				}
				predicateTypesSpecified.Add(typeof(PredicateType));
			}
		}

		internal static void InitializeMatchesPatternsPredicateWithNormalization(MatchesPatternsPredicate predicate, IEnumerable<Pattern> patterns, bool useLegacyRegex)
		{
			predicate.UseLegacyRegex = useLegacyRegex;
			predicate.Patterns = Utils.NormalizePatterns(patterns);
		}

		internal static void InitializeContainsWordsPredicateWithNormalization(ContainsWordsPredicate predicate, IEnumerable<Word> words)
		{
			predicate.Words = Utils.NormalizeWords(words);
		}

		private static void InitializeAndAddNewPredicateForArrayParameter<PredicateType, ParameterType>(PropertyBag fields, TypeMapping[] supportedPredicates, List<TransportRulePredicate> conditionList, List<TransportRulePredicate> exceptionList, ICollection<Type> conditionTypesSpecified, ICollection<Type> exceptionTypesSpecified, string conditionParameterName, string exceptionParameterName, Utils.ArrayPredicateInitializer<PredicateType, ParameterType> predicateInitializer) where PredicateType : TransportRulePredicate, new()
		{
			Utils.InitializeAndAddNewPredicateForArrayParameter<PredicateType, ParameterType>(fields, supportedPredicates, conditionList, conditionTypesSpecified, conditionParameterName, predicateInitializer);
			Utils.InitializeAndAddNewPredicateForArrayParameter<PredicateType, ParameterType>(fields, supportedPredicates, exceptionList, exceptionTypesSpecified, exceptionParameterName, predicateInitializer);
		}

		private static void InitializeAndAddNewPredicateForArrayParameter<PredicateType, ParameterType>(PropertyBag fields, TypeMapping[] supportedPredicates, List<TransportRulePredicate> predicateList, ICollection<Type> predicateTypesSpecified, string parameterName, Utils.ArrayPredicateInitializer<PredicateType, ParameterType> predicateInitializer) where PredicateType : TransportRulePredicate, new()
		{
			if (fields.IsModified(parameterName))
			{
				ParameterType[] array = fields[parameterName] as ParameterType[];
				if (array != null && array.Length > 0)
				{
					PredicateType predicateType = Activator.CreateInstance<PredicateType>();
					predicateType.Initialize(supportedPredicates);
					predicateInitializer(predicateType, array);
					Utils.InsertPredicateSorted(predicateType, predicateList);
				}
				predicateTypesSpecified.Add(typeof(PredicateType));
			}
		}

		private static void InitializeAndAddNewPredicateForNullableParameter<PredicateType, ParameterType>(PropertyBag fields, TypeMapping[] supportedPredicates, List<TransportRulePredicate> conditionList, List<TransportRulePredicate> exceptionList, ICollection<Type> conditionTypesSpecified, ICollection<Type> exceptionTypesSpecified, string conditionParameterName, string exceptionParameterName, Utils.NonArrayPredicateInitializer<PredicateType, ParameterType> predicateInitializer) where PredicateType : TransportRulePredicate, new() where ParameterType : struct
		{
			Utils.InitializeAndAddNewPredicateForNullableParameter<PredicateType, ParameterType>(fields, supportedPredicates, conditionList, conditionTypesSpecified, conditionParameterName, predicateInitializer);
			Utils.InitializeAndAddNewPredicateForNullableParameter<PredicateType, ParameterType>(fields, supportedPredicates, exceptionList, exceptionTypesSpecified, exceptionParameterName, predicateInitializer);
		}

		private static void InitializeAndAddNewPredicateForNullableParameter<PredicateType, ParameterType>(PropertyBag fields, TypeMapping[] supportedPredicates, List<TransportRulePredicate> predicateList, ICollection<Type> predicateTypesSpecified, string parameterName, Utils.NonArrayPredicateInitializer<PredicateType, ParameterType> predicateInitializer) where PredicateType : TransportRulePredicate, new() where ParameterType : struct
		{
			if (fields.IsModified(parameterName))
			{
				ParameterType? parameterType = fields[parameterName] as ParameterType?;
				if (parameterType != null)
				{
					PredicateType predicateType = Activator.CreateInstance<PredicateType>();
					predicateType.Initialize(supportedPredicates);
					predicateInitializer(predicateType, parameterType.Value);
					Utils.InsertPredicateSorted(predicateType, predicateList);
				}
				predicateTypesSpecified.Add(typeof(PredicateType));
			}
		}

		private static void InitializeAndAddNewActionForParameter<ActionType, ParameterType>(PropertyBag fields, TypeMapping[] supportedActions, List<TransportRuleAction> actionList, ICollection<Type> actionTypesSpecified, string parameterName, Utils.ActionInitializer<ActionType, ParameterType> actionInitializer) where ActionType : TransportRuleAction, new()
		{
			if (fields.IsModified(parameterName))
			{
				ParameterType[] array = (ParameterType[])fields[parameterName];
				if (array != null && array.Length > 0)
				{
					ActionType actionType = Activator.CreateInstance<ActionType>();
					actionType.Initialize(supportedActions);
					actionInitializer(actionType, array);
					Utils.InsertActionSorted(actionType, actionList);
				}
				actionTypesSpecified.Add(typeof(ActionType));
			}
		}

		private static string LoadTestContentForRegexPerformanceTest()
		{
			string result;
			using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("RegexPerformanceTestContent.txt"))
			{
				if (manifestResourceStream == null)
				{
					throw new InvalidOperationException("Unable to load regex performance test content");
				}
				manifestResourceStream.Seek(0L, SeekOrigin.Begin);
				StreamReader streamReader = new StreamReader(manifestResourceStream);
				result = streamReader.ReadToEnd();
			}
			return result;
		}

		private static long TimeRegexAgainstTestContent(string regexPattern, string testContent)
		{
			if (string.IsNullOrEmpty(regexPattern))
			{
				throw new ArgumentNullException("regexPattern");
			}
			if (string.IsNullOrEmpty(testContent))
			{
				throw new ArgumentNullException("testContent");
			}
			CpuStopwatch cpuStopwatch = new CpuStopwatch();
			try
			{
				cpuStopwatch.Start();
				Regex.IsMatch(testContent, regexPattern);
			}
			catch (ArgumentException)
			{
			}
			cpuStopwatch.Stop();
			return cpuStopwatch.ElapsedMilliseconds;
		}

		private static string RedactNameValuePair(string value)
		{
			int num = value.IndexOf(':');
			if (num < 0 || num >= value.Length - 1)
			{
				return null;
			}
			string arg = value.Substring(0, num);
			string text = value.Substring(num);
			text = SuppressingPiiData.Redact(text);
			return arg + ':' + text;
		}

		internal static string GetOrganizationParameterValue(PropertyBag fields)
		{
			if (fields["Organization"] != null)
			{
				return fields["Organization"].ToString();
			}
			return string.Empty;
		}

		internal static IEnumerable<string> AddOrganizationScopeToCmdlets(IEnumerable<string> policyCommands, string organizationParameterValue)
		{
			return (from command in policyCommands
			select CommandHelper.AddOrganizationScope(command, organizationParameterValue)).ToList<string>();
		}

		internal static string AddOrganizationScopeToCmdlet(string policyCommand, string organizationParameterValue)
		{
			return CommandHelper.AddOrganizationScope(policyCommand, organizationParameterValue);
		}

		internal static void ValidateTransportRuleRegexCpuTimeLimit(IConfigurationSession configSession, PropertyBag propertyBag)
		{
			List<Pattern> list = new List<Pattern>();
			foreach (object obj in from object val in propertyBag.Values
			where val != null && val is Pattern[]
			select val)
			{
				if (obj is Pattern[])
				{
					list.AddRange(obj as Pattern[]);
				}
			}
			if (list.Count == 0)
			{
				return;
			}
			TransportConfigContainer transportConfigContainer = configSession.Find<TransportConfigContainer>(null, QueryScope.SubTree, null, null, 1).FirstOrDefault<TransportConfigContainer>();
			if (transportConfigContainer == null)
			{
				throw new ArgumentException(Strings.ErrorAccessingTransportSettings);
			}
			foreach (Pattern pattern in list)
			{
				pattern.ValidatePatternDoesNotExceedCpuTimeLimit((long)transportConfigContainer.TransportRuleRegexValidationTimeout.TotalMilliseconds);
			}
		}

		internal static bool ValidateActivationAndExpiryDates(Action<LocalizedString> warningDelegate, TransportRule existingRule, PropertyBag fields, out ArgumentException error, out string parameterName)
		{
			error = null;
			parameterName = null;
			DateTime? dateTime = fields.IsModified("ActivationDate") ? ((DateTime?)fields["ActivationDate"]) : ((existingRule == null) ? null : existingRule.ActivationDate);
			DateTime? dateTime2 = fields.IsModified("ExpiryDate") ? ((DateTime?)fields["ExpiryDate"]) : ((existingRule == null) ? null : existingRule.ExpiryDate);
			if (dateTime2 != null && dateTime != null && dateTime2.Value.ToUniversalTime() <= dateTime.Value.ToUniversalTime())
			{
				error = new ArgumentException(Strings.ExpiryDateSameOrBeforeActivationDate);
				parameterName = "ExpiryDate";
				return false;
			}
			if (fields.IsModified("ActivationDate") && dateTime != null && dateTime.Value.ToUniversalTime() < DateTime.UtcNow)
			{
				warningDelegate(Strings.WarningActivationDateBeforeCurrentDate("ActivationDate"));
			}
			if (fields.IsModified("ExpiryDate") && dateTime2 != null && dateTime2.Value.ToUniversalTime() <= DateTime.UtcNow)
			{
				warningDelegate(Strings.WarningExpiryDateBeforeCurrentDate);
			}
			return true;
		}

		internal static Exception ValidateTransportRuleRegexesForMigratingTenants(OrganizationId organizationId)
		{
			IConfigurationSession session = TransportUtils.CreateSession(organizationId);
			ADRuleStorageManager adruleStorageManager;
			try
			{
				adruleStorageManager = new ADRuleStorageManager(Utils.RuleCollectionNameFromRole(), session);
			}
			catch (RuleCollectionNotInAdException)
			{
				return null;
			}
			try
			{
				adruleStorageManager.LoadRuleCollection();
			}
			catch (ParserException result)
			{
				return result;
			}
			return null;
		}

		internal static bool IsMessageClassificationUsedByTransportRule(IConfigurationSession session, MessageClassification classification)
		{
			Func<Rule, bool> selector = (Rule rule) => (rule.ApplyClassification != null && rule.ApplyClassification.Equals(classification.Id)) || (rule.HasClassification != null && rule.HasClassification.Equals(classification.Id)) || (rule.ExceptIfHasClassification != null && rule.ExceptIfHasClassification.Equals(classification.Id));
			bool result;
			try
			{
				result = DlpUtils.GetTransportRules(session, selector).Any<Rule>();
			}
			catch (RuleCollectionNotInAdException)
			{
				result = false;
			}
			return result;
		}

		internal static RecipientIdParameter[] RemoveDuplicateRecipients(RecipientIdParameter[] recipients, out int duplicateCount)
		{
			if (recipients == null)
			{
				throw new ArgumentNullException("recipients");
			}
			duplicateCount = 0;
			if (recipients.Length < 2)
			{
				return recipients;
			}
			HashSet<string> addresses = new HashSet<string>();
			List<RecipientIdParameter> list = (from recipient in recipients
			where addresses.Add(recipient.RawIdentity.ToLower())
			select recipient).ToList<RecipientIdParameter>();
			duplicateCount = recipients.Length - list.Count;
			if (duplicateCount != 0)
			{
				return list.ToArray();
			}
			return recipients;
		}

		internal static int GetHashCodeForArray<ArrayType>(IEnumerable<ArrayType> array)
		{
			Func<int, ArrayType, int> func = null;
			if (array == null)
			{
				return 0;
			}
			int seed = 17;
			if (func == null)
			{
				func = ((int current, ArrayType item) => current * 23 + item.GetHashCode());
			}
			return array.Aggregate(seed, func);
		}

		internal static Pattern[] RedactPatterns(IEnumerable<Pattern> patterns)
		{
			if (patterns == null)
			{
				return null;
			}
			Pattern[] array = patterns.ToArray<Pattern>();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SuppressPiiData();
			}
			return array;
		}

		internal static Pattern[] RedactNameValuePairPatterns(Pattern[] patterns)
		{
			if (patterns == null)
			{
				return null;
			}
			Pattern[] array = new Pattern[patterns.Length];
			for (int i = 0; i < patterns.Length; i++)
			{
				array[i] = new Pattern(Utils.RedactNameValuePair(patterns[i].Value));
			}
			return array;
		}

		internal static Word[] RedactNameValuePairWords(Word[] words)
		{
			if (words == null)
			{
				return null;
			}
			List<Word> list = new List<Word>();
			for (int i = 0; i < words.Length; i++)
			{
				list.Add(new Word(Utils.RedactNameValuePair(words[i].Value)));
			}
			return list.ToArray();
		}

		private static bool IsMultiTeancyEnabled()
		{
			return VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled || Datacenter.IsPartnerHostedOnly(true);
		}

		internal static bool ValidateDomainIsPredicates(PropertyBag fields, out ArgumentException argumentException, out string parameterName)
		{
			argumentException = null;
			parameterName = null;
			if (fields.IsModified("SenderDomainIs") && !Utils.ValidateDomainNames((Word[])fields["SenderDomainIs"], out argumentException))
			{
				parameterName = "SenderDomainIs";
				return false;
			}
			if (fields.IsModified("RecipientDomainIs") && !Utils.ValidateDomainNames((Word[])fields["RecipientDomainIs"], out argumentException))
			{
				parameterName = "RecipientDomainIs";
				return false;
			}
			if (fields.IsModified("ExceptIfSenderDomainIs") && !Utils.ValidateDomainNames((Word[])fields["ExceptIfSenderDomainIs"], out argumentException))
			{
				parameterName = "ExceptIfSenderDomainIs";
				return false;
			}
			if (fields.IsModified("ExceptIfRecipientDomainIs") && !Utils.ValidateDomainNames((Word[])fields["ExceptIfRecipientDomainIs"], out argumentException))
			{
				parameterName = "ExceptIfRecipientDomainIs";
				return false;
			}
			return true;
		}

		internal static bool ValidateDomainNames(Word[] domains, out ArgumentException argumentException)
		{
			argumentException = null;
			if (domains == null)
			{
				return true;
			}
			List<Word> list = new List<Word>();
			foreach (Word item in domains)
			{
				SmtpDomain smtpDomain;
				if (!SmtpDomain.TryParse(item.ToString(), out smtpDomain))
				{
					list.Add(item);
				}
			}
			if (!list.Any<Word>())
			{
				return true;
			}
			argumentException = new ArgumentException(Strings.DomainIsParameterInvalid(string.Join<Word>(",", list)));
			return false;
		}

		internal static bool ValidateContainsWordsPredicate(PropertyBag fields, out ArgumentException argumentException, out string invalidParameterName)
		{
			invalidParameterName = null;
			argumentException = null;
			foreach (string text in Utils.ContainsWordsParameters)
			{
				if (fields.IsModified(text) && (Word[])fields[text] != null)
				{
					if (((Word[])fields[text]).Any((Word word) => Utils.StringContainsLeadingTrailingWhiteSpace(word.ToString())))
					{
						invalidParameterName = text;
						argumentException = new ArgumentException(Strings.WordIsWhiteSpaceOrContainsWhiteSpacePrefixOrSuffix);
						return false;
					}
				}
			}
			return true;
		}

		internal static bool ValidateMatchesPatternsPredicate(PropertyBag fields, out ArgumentException argumentException, out string invalidParameterName)
		{
			invalidParameterName = null;
			argumentException = null;
			foreach (string text in Utils.MatchesPatternsParameters)
			{
				if (fields.IsModified(text) && fields[text] != null && ((Pattern[])fields[text]).Any(new Func<Pattern, bool>(Utils.PatternContainsLeadingTrailingProhibitedRegex)))
				{
					invalidParameterName = text;
					argumentException = new ArgumentException(Strings.PatternContainsProhibitedLeadingOrTrailingRegexCharacters);
					return false;
				}
			}
			return true;
		}

		internal static bool ValidateAdAttributePredicate(PropertyBag fields, out ArgumentException argumentException, out string invalidParameterName)
		{
			invalidParameterName = null;
			argumentException = null;
			foreach (string text in Utils.AdAttributeContainsWordsParameters)
			{
				if (fields.IsModified(text) && (Word[])fields[text] != null)
				{
					if (((Word[])fields[text]).Any((Word parameter) => !Utils.IsValidAdAttributeParameter(parameter)))
					{
						invalidParameterName = text;
						argumentException = new ArgumentException(Strings.ADAttributeIsWhiteSpaceOrContainsWhiteSpacePrefixOrSuffix);
						return false;
					}
				}
			}
			foreach (string text2 in Utils.AdAttributeMatchesPatternsParameters)
			{
				if (fields.IsModified(text2) && fields[text2] != null)
				{
					Pattern[] source = (Pattern[])fields[text2];
					if (source.Any((Pattern parameter) => !Utils.IsValidAdAttributeParameter(parameter)))
					{
						invalidParameterName = text2;
						argumentException = new ArgumentException(Strings.ADAttributeIsWhiteSpaceOrContainsWhiteSpacePrefixOrSuffix);
						return false;
					}
					if (source.Any(new Func<Pattern, bool>(Utils.PatternContainsLeadingTrailingProhibitedRegex)))
					{
						invalidParameterName = text2;
						argumentException = new ArgumentException(Strings.PatternContainsProhibitedLeadingOrTrailingRegexCharacters);
						return false;
					}
				}
			}
			return true;
		}

		internal static bool IsValidAdAttributeParameter(object parameter)
		{
			string text = null;
			if (parameter is Pattern)
			{
				text = ((Pattern)parameter).Value;
			}
			else if (parameter is Word)
			{
				text = ((Word)parameter).ToString();
			}
			return !string.IsNullOrEmpty(text) && !Utils.StringContainsLeadingTrailingWhiteSpace(text);
		}

		internal static bool ValidatePropertyContainsWordsPredicates(PropertyBag fields, out ArgumentException argumentException, out string invalidParameterName)
		{
			invalidParameterName = null;
			argumentException = null;
			foreach (string text in Utils.PropertyContainsWordsParameters)
			{
				if (fields.IsModified(text) && (Word[])fields[text] != null)
				{
					if (((Word[])fields[text]).Any((Word parameter) => !Utils.IsValidAdAttributeParameter(parameter)))
					{
						invalidParameterName = text;
						argumentException = new ArgumentException(Strings.MetadataPropertyIsWhiteSpaceOrContainsWhiteSpacePrefixOrSuffix);
						return false;
					}
				}
			}
			return true;
		}

		internal static bool StringContainsLeadingTrailingWhiteSpace(string input)
		{
			ArgumentValidator.ThrowIfNull("input", input);
			return !input.Length.Equals(input.Trim().Length);
		}

		internal static bool PatternContainsLeadingTrailingProhibitedRegex(Pattern input)
		{
			ArgumentValidator.ThrowIfNull("input", input);
			string text = input.ToString();
			string[] prohibitedLeadingAndTrailingCharactersInPatterns = Utils.ProhibitedLeadingAndTrailingCharactersInPatterns;
			int i = 0;
			while (i < prohibitedLeadingAndTrailingCharactersInPatterns.Length)
			{
				string text2 = prohibitedLeadingAndTrailingCharactersInPatterns[i];
				bool result;
				if (text.StartsWith(text2))
				{
					result = true;
				}
				else
				{
					if (!new Regex(string.Format(".*{0}(\\*|\\+|{1})?$", Utils.EscapeProhibitedRegexCharacters(text2), Pattern.BoundedRepeaterPattern)).Match(text).Success)
					{
						i++;
						continue;
					}
					result = true;
				}
				return result;
			}
			return false;
		}

		internal static string EscapeProhibitedRegexCharacters(string input)
		{
			string text = Regex.Escape(input);
			if (!text.StartsWith("\\"))
			{
				return text;
			}
			return "(?<!\\\\)" + text;
		}

		internal static PiiMap GetSessionPiiMap(ExchangeRunspaceConfiguration config)
		{
			if (config != null && config.PiiMapId != null)
			{
				return PiiMapManager.Instance.GetOrAdd(config.PiiMapId);
			}
			return null;
		}

		internal static SmtpAddress NormalizeSmtpAddress(SmtpAddress unicodeSmtpAddress)
		{
			return new SmtpAddress(unicodeSmtpAddress.ToString().Normalize(NormalizationForm.FormKC));
		}

		internal static Word[] NormalizeWords(IEnumerable<Word> words)
		{
			return (from x in words
			select new Word(x.Value.Normalize(NormalizationForm.FormKC))).ToArray<Word>();
		}

		internal static Pattern[] NormalizePatterns(IEnumerable<Pattern> patterns)
		{
			return (from x in patterns
			select new Pattern(x.Value.Normalize(NormalizationForm.FormKC))).ToArray<Pattern>();
		}

		internal const string MigratedRuleCommentSuffix = "FOPEPolicyMigration";

		internal const string Edge = "Edge";

		internal const string Transport = "Transport";

		internal const string Transport14 = "Transport14";

		internal const string TransportVersioned = "TransportVersioned";

		internal const string TransportRule = "TransportRule";

		internal const string TransportRuleCollection = "TransportRuleCollection";

		internal const string New = "New";

		internal const string Remove = "Remove";

		internal const string Get = "Get";

		internal const string Set = "Set";

		internal const string Enable = "Enable";

		internal const string Disable = "Disable";

		internal const string Import = "Import";

		internal const string Export = "Export";

		internal const string Conditions = "Conditions";

		internal const string Actions = "Actions";

		internal const string Exceptions = "Exceptions";

		internal const string Name = "Name";

		internal const string ApplyHtmlDisclaimer = "ApplyHtmlDisclaimer";

		internal const string ApplyDisclaimer = "ApplyDisclaimer";

		internal const string AddManagerAsRecipientType = "AddManagerAsRecipientType";

		internal const string ModerateMessageByUser = "ModerateMessageByUser";

		internal const string ModerateMessageByManager = "ModerateMessageByManager";

		internal const string ApplyDisclaimerWithSeparator = "ApplyDisclaimerWithSeparator";

		internal const string ApplyDisclaimerWithSeparatorAndReadingOrder = "ApplyDisclaimerWithSeparatorAndReadingOrder";

		internal const string LogEvent = "LogEvent";

		internal const string Description = "Description";

		internal const string RightsProtectMessage = "RightsProtectMessage";

		internal const string CrLfString = "\r\n";

		internal const string TabString = "\t";

		internal const string ParameterPrefix = "-";

		internal const string ParameterSeparator = ", ";

		internal const string ParameterTrueValue = "$true";

		internal const string ResolvedRmsTemplateIdentity = "ResolvedRmsTemplateIdentity";

		internal const string UseLegacyRegexParameterName = "UseLegacyRegex";

		internal const string DlpPolicyParameterName = "DlpPolicy";

		internal const string EnabledParameterName = "Enabled";

		internal const string FromParameterName = "From";

		internal const string FromMemberOfParameterName = "FromMemberOf";

		internal const string FromScopeParameterName = "FromScope";

		internal const string SentToParameterName = "SentTo";

		internal const string SentToMemberOfParameterName = "SentToMemberOf";

		internal const string SentToScopeParameterName = "SentToScope";

		internal const string BetweenMemberOf1ParameterName = "BetweenMemberOf1";

		internal const string BetweenMemberOf2ParameterName = "BetweenMemberOf2";

		internal const string ManagerAddressesParameterName = "ManagerAddresses";

		internal const string ManagerForEvaluatedUserParameterName = "ManagerForEvaluatedUser";

		internal const string SenderManagementRelationshipParameterName = "SenderManagementRelationship";

		internal const string ADComparisonAttributeParameterName = "ADComparisonAttribute";

		internal const string ADComparisonOperatorParameterName = "ADComparisonOperator";

		internal const string SenderADAttributeContainsWordsParameterName = "SenderADAttributeContainsWords";

		internal const string SenderADAttributeMatchesPatternsParameterName = "SenderADAttributeMatchesPatterns";

		internal const string RecipientADAttributeContainsWordsParameterName = "RecipientADAttributeContainsWords";

		internal const string RecipientADAttributeMatchesPatternsParameterName = "RecipientADAttributeMatchesPatterns";

		internal const string AnyOfToHeaderParameterName = "AnyOfToHeader";

		internal const string AnyOfToHeaderMemberOfParameterName = "AnyOfToHeaderMemberOf";

		internal const string AnyOfCcHeaderParameterName = "AnyOfCcHeader";

		internal const string AnyOfCcHeaderMemberOfParameterName = "AnyOfCcHeaderMemberOf";

		internal const string AnyOfToCcHeaderParameterName = "AnyOfToCcHeader";

		internal const string AnyOfToCcHeaderMemberOfParameterName = "AnyOfToCcHeaderMemberOf";

		internal const string HasClassificationParameterName = "HasClassification";

		internal const string HasNoClassificationParameterName = "HasNoClassification";

		internal const string SubjectContainsWordsParameterName = "SubjectContainsWords";

		internal const string SubjectOrBodyContainsWordsParameterName = "SubjectOrBodyContainsWords";

		internal const string HeaderContainsMessageHeaderParameterName = "HeaderContainsMessageHeader";

		internal const string HeaderContainsWordsParameterName = "HeaderContainsWords";

		internal const string FromAddressContainsWordsParameterName = "FromAddressContainsWords";

		internal const string SenderDomainIsParameterName = "SenderDomainIs";

		internal const string RecipientDomainIsParameterName = "RecipientDomainIs";

		internal const string SubjectMatchesPatternsParameterName = "SubjectMatchesPatterns";

		internal const string SubjectOrBodyMatchesPatternsParameterName = "SubjectOrBodyMatchesPatterns";

		internal const string HeaderMatchesMessageHeaderParameterName = "HeaderMatchesMessageHeader";

		internal const string HeaderMatchesPatternsParameterName = "HeaderMatchesPatterns";

		internal const string FromAddressMatchesPatternsParameterName = "FromAddressMatchesPatterns";

		internal const string AttachmentNameMatchesPatternsParameterName = "AttachmentNameMatchesPatterns";

		internal const string AttachmentExtensionMatchesWordsParameterName = "AttachmentExtensionMatchesWords";

		internal const string SCLOverParameterName = "SCLOver";

		internal const string AttachmentSizeOverParameterName = "AttachmentSizeOver";

		internal const string MessageSizeOverParameterName = "MessageSizeOver";

		internal const string WithImportanceParameterName = "WithImportance";

		internal const string MessageTypeMatchesParameterName = "MessageTypeMatches";

		internal const string RecipientAddressContainsWordsParameterName = "RecipientAddressContainsWords";

		internal const string RecipientAddressMatchesPatternsParameterName = "RecipientAddressMatchesPatterns";

		internal const string AttachmentMatchesPatternsParameterName = "AttachmentMatchesPatterns";

		internal const string AttachmentContainsWordsParameterName = "AttachmentContainsWords";

		internal const string AttachmentIsUnsupportedParameterName = "AttachmentIsUnsupported";

		internal const string AttachmentHasExecutableContentParameterName = "AttachmentHasExecutableContent";

		internal const string AttachmentProcessingLimitExceededParameterName = "AttachmentProcessingLimitExceeded";

		internal const string AttachmentPropertyContainsWordsParameterName = "AttachmentPropertyContainsWords";

		internal const string AnyOfRecipientAddressContainsWordsParameterName = "AnyOfRecipientAddressContainsWords";

		internal const string AnyOfRecipientAddressMatchesPatternsParameterName = "AnyOfRecipientAddressMatchesPatterns";

		internal const string HasSenderOverrideParameterName = "HasSenderOverride";

		internal const string MessageContainsDataClassificationsParameterName = "MessageContainsDataClassifications";

		internal const string ExceptIfFromParameterName = "ExceptIfFrom";

		internal const string ExceptIfFromMemberOfParameterName = "ExceptIfFromMemberOf";

		internal const string ExceptIfFromScopeParameterName = "ExceptIfFromScope";

		internal const string ExceptIfSentToParameterName = "ExceptIfSentTo";

		internal const string ExceptIfSentToMemberOfParameterName = "ExceptIfSentToMemberOf";

		internal const string ExceptIfSentToScopeParameterName = "ExceptIfSentToScope";

		internal const string ExceptIfBetweenMemberOf1ParameterName = "ExceptIfBetweenMemberOf1";

		internal const string ExceptIfBetweenMemberOf2ParameterName = "ExceptIfBetweenMemberOf2";

		internal const string ExceptIfManagerAddressesParameterName = "ExceptIfManagerAddresses";

		internal const string ExceptIfManagerForEvaluatedUserParameterName = "ExceptIfManagerForEvaluatedUser";

		internal const string ExceptIfSenderManagementRelationshipParameterName = "ExceptIfSenderManagementRelationship";

		internal const string ExceptIfADComparisonAttributeParameterName = "ExceptIfADComparisonAttribute";

		internal const string ExceptIfADComparisonOperatorParameterName = "ExceptIfADComparisonOperator";

		internal const string ExceptIfSenderADAttributeContainsWordsParameterName = "ExceptIfSenderADAttributeContainsWords";

		internal const string ExceptIfSenderADAttributeMatchesPatternsParameterName = "ExceptIfSenderADAttributeMatchesPatterns";

		internal const string ExceptIfRecipientADAttributeContainsWordsParameterName = "ExceptIfRecipientADAttributeContainsWords";

		internal const string ExceptIfRecipientADAttributeMatchesPatternsParameterName = "ExceptIfRecipientADAttributeMatchesPatterns";

		internal const string ExceptIfAnyOfToHeaderParameterName = "ExceptIfAnyOfToHeader";

		internal const string ExceptIfAnyOfToHeaderMemberOfParameterName = "ExceptIfAnyOfToHeaderMemberOf";

		internal const string ExceptIfAnyOfCcHeaderParameterName = "ExceptIfAnyOfCcHeader";

		internal const string ExceptIfAnyOfCcHeaderMemberOfParameterName = "ExceptIfAnyOfCcHeaderMemberOf";

		internal const string ExceptIfAnyOfToCcHeaderParameterName = "ExceptIfAnyOfToCcHeader";

		internal const string ExceptIfAnyOfToCcHeaderMemberOfParameterName = "ExceptIfAnyOfToCcHeaderMemberOf";

		internal const string ExceptIfHasClassificationParameterName = "ExceptIfHasClassification";

		internal const string ExceptIfHasNoClassificationParameterName = "ExceptIfHasNoClassification";

		internal const string ExceptIfSubjectContainsWordsParameterName = "ExceptIfSubjectContainsWords";

		internal const string ExceptIfSubjectOrBodyContainsWordsParameterName = "ExceptIfSubjectOrBodyContainsWords";

		internal const string ExceptIfHeaderContainsMessageHeaderParameterName = "ExceptIfHeaderContainsMessageHeader";

		internal const string ExceptIfHeaderContainsWordsParameterName = "ExceptIfHeaderContainsWords";

		internal const string ExceptIfFromAddressContainsWordsParameterName = "ExceptIfFromAddressContainsWords";

		internal const string ExceptIfSenderDomainIsParameterName = "ExceptIfSenderDomainIs";

		internal const string ExceptIfRecipientDomainIsParameterName = "ExceptIfRecipientDomainIs";

		internal const string ExceptIfSubjectMatchesPatternsParameterName = "ExceptIfSubjectMatchesPatterns";

		internal const string ExceptIfSubjectOrBodyMatchesPatternsParameterName = "ExceptIfSubjectOrBodyMatchesPatterns";

		internal const string ExceptIfHeaderMatchesMessageHeaderParameterName = "ExceptIfHeaderMatchesMessageHeader";

		internal const string ExceptIfHeaderMatchesPatternsParameterName = "ExceptIfHeaderMatchesPatterns";

		internal const string ExceptIfFromAddressMatchesPatternsParameterName = "ExceptIfFromAddressMatchesPatterns";

		internal const string ExceptIfAttachmentNameMatchesPatternsParameterName = "ExceptIfAttachmentNameMatchesPatterns";

		internal const string ExceptIfAttachmentExtensionMatchesWordsParameterName = "ExceptIfAttachmentExtensionMatchesWords";

		internal const string ExceptIfSCLOverParameterName = "ExceptIfSCLOver";

		internal const string ExceptIfAttachmentSizeOverParameterName = "ExceptIfAttachmentSizeOver";

		internal const string ExceptIfMessageSizeOverParameterName = "ExceptIfMessageSizeOver";

		internal const string ExceptIfWithImportanceParameterName = "ExceptIfWithImportance";

		internal const string ExceptIfMessageTypeMatchesParameterName = "ExceptIfMessageTypeMatches";

		internal const string ExceptIfRecipientAddressContainsWordsParameterName = "ExceptIfRecipientAddressContainsWords";

		internal const string ExceptIfRecipientAddressMatchesPatternsParameterName = "ExceptIfRecipientAddressMatchesPatterns";

		internal const string ExceptIfAttachmentMatchesPatternsParameterName = "ExceptIfAttachmentMatchesPatterns";

		internal const string ExceptIfAttachmentContainsWordsParameterName = "ExceptIfAttachmentContainsWords";

		internal const string ExceptIfAttachmentIsUnsupportedParameterName = "ExceptIfAttachmentIsUnsupported";

		internal const string ExceptIfAttachmentHasExecutableContentParameterName = "ExceptIfAttachmentHasExecutableContent";

		internal const string ExceptIfAttachmentProcessingLimitExceededParameterName = "ExceptIfAttachmentProcessingLimitExceeded";

		internal const string ExceptIfAttachmentPropertyContainsWordsParameterName = "ExceptIfAttachmentPropertyContainsWords";

		internal const string ExceptIfAnyOfRecipientAddressContainsWordsParameterName = "ExceptIfAnyOfRecipientAddressContainsWords";

		internal const string ExceptIfAnyOfRecipientAddressMatchesPatternsParameterName = "ExceptIfAnyOfRecipientAddressMatchesPatterns";

		internal const string PrependSubjectParameterName = "PrependSubject";

		internal const string SetAuditSeverityParameterName = "SetAuditSeverity";

		internal const string ApplyClassificationParameterName = "ApplyClassification";

		internal const string ApplyHtmlDisclaimerLocationParameterName = "ApplyHtmlDisclaimerLocation";

		internal const string ApplyHtmlDisclaimerTextParameterName = "ApplyHtmlDisclaimerText";

		internal const string ApplyHtmlDisclaimerFallbackActionParameterName = "ApplyHtmlDisclaimerFallbackAction";

		internal const string ApplyRightsProtectionTemplateParameterName = "ApplyRightsProtectionTemplate";

		internal const string SetSCLParameterName = "SetSCL";

		internal const string SetHeaderNameParameterName = "SetHeaderName";

		internal const string SetHeaderValueParameterName = "SetHeaderValue";

		internal const string RemoveHeaderParameterName = "RemoveHeader";

		internal const string AddToRecipientsParameterName = "AddToRecipients";

		internal const string CopyToParameterName = "CopyTo";

		internal const string BlindCopyToParameterName = "BlindCopyTo";

		internal const string AddManagerAsRecipientTypeParameterName = "AddManagerAsRecipientType";

		internal const string ModerateMessageByUserParameterName = "ModerateMessageByUser";

		internal const string ModerateMessageByManagerParameterName = "ModerateMessageByManager";

		internal const string RedirectMessageToParameterName = "RedirectMessageTo";

		internal const string RejectMessageEnhancedStatusCodeParameterName = "RejectMessageEnhancedStatusCode";

		internal const string RejectMessageReasonTextParameterName = "RejectMessageReasonText";

		internal const string DeleteMessageParameterName = "DeleteMessage";

		internal const string DisconnectParameterName = "Disconnect";

		internal const string QuarantineParameterName = "Quarantine";

		internal const string LogEventParameterName = "LogEventText";

		internal const string StopRuleProcessingParameterName = "StopRuleProcessing";

		internal const string SmtpRejectMessageRejectStatusCodeParameterName = "SmtpRejectMessageRejectStatusCode";

		internal const string SmtpRejectMessageRejectTextParameterName = "SmtpRejectMessageRejectText";

		internal const string SenderInRecipientListParameterName = "SenderInRecipientList";

		internal const string SenderIpRangesParameterName = "SenderIpRanges";

		internal const string ExceptIfSenderIpRangesParameterName = "ExceptIfSenderIpRanges";

		internal const string RecipientInSenderListParameterName = "RecipientInSenderList";

		internal const string ExceptIfSenderInRecipientListParameterName = "ExceptIfSenderInRecipientList";

		internal const string ExceptIfRecipientInSenderListParameterName = "ExceptIfRecipientInSenderList";

		internal const string ExceptIfHasSenderOverrideParameterName = "ExceptIfHasSenderOverride";

		internal const string ExceptIfMessageContainsDataClassificationsParameterName = "ExceptIfMessageContainsDataClassifications";

		internal const string ActivationDateParameterName = "ActivationDate";

		internal const string PriorityParameterName = "Priority";

		internal const string CommentsParameterName = "Comments";

		internal const string ExpiryDateParameterName = "ExpiryDate";

		internal const string NotifySenderParameterName = "NotifySender";

		internal const string RuleSubTypeParameterName = "RuleSubType";

		internal const string RuleModeParameterName = "Mode";

		internal const string GenerateIncidentReportParameterName = "GenerateIncidentReport";

		internal const string IncidentReportOriginalMailParameterName = "IncidentReportOriginalMail";

		internal const string IncidentReportContentParameterName = "IncidentReportContent";

		internal const string RouteMessageOutboundConnectorParameterName = "RouteMessageOutboundConnector";

		internal const string RouteMessageOutboundRequireTlsParameterName = "RouteMessageOutboundRequireTls";

		internal const string ApplyOMEParameterName = "ApplyOME";

		internal const string RemoveOMEParameterName = "RemoveOME";

		internal const string RuleErrorActionParameterName = "RuleErrorAction";

		internal const string SenderAddressLocationParameterName = "SenderAddressLocation";

		internal const string OrganizationParameterName = "Organization";

		internal const string IdentityParameterName = "Identity";

		internal const string ContentCharacterSetContainsWordsParameterName = "ContentCharacterSetContainsWords";

		internal const string ExceptIfContentCharacterSetContainsWordsParameterName = "ExceptIfContentCharacterSetContainsWords";

		internal const string AttachmentIsPasswordProtectedParameterName = "AttachmentIsPasswordProtected";

		internal const string ExceptIfAttachmentIsPasswordProtectedParameterName = "ExceptIfAttachmentIsPasswordProtected";

		internal const string FilterParameterName = "Filter";

		internal const string GenerateNotificationParameterName = "GenerateNotification";

		internal static readonly RejectText DefaultRejectText = new RejectText("Delivery not authorized, message refused");

		internal static readonly RejectEnhancedStatus? DefaultEnhancedStatusCode = new RejectEnhancedStatus?(new RejectEnhancedStatus("5.7.1"));

		private static readonly RejectStatusCode DefaultRejectStatusCode = new RejectStatusCode("550");

		public static readonly string[] RecipientIdParameters = new string[]
		{
			"From",
			"FromMemberOf",
			"SentTo",
			"SentToMemberOf",
			"BetweenMemberOf1",
			"BetweenMemberOf2",
			"ManagerAddresses",
			"AnyOfToHeader",
			"AnyOfToHeaderMemberOf",
			"AnyOfCcHeader",
			"AnyOfCcHeaderMemberOf",
			"AnyOfToCcHeader",
			"AnyOfToCcHeaderMemberOf",
			"ExceptIfFrom",
			"ExceptIfFromMemberOf",
			"ExceptIfSentTo",
			"ExceptIfSentToMemberOf",
			"ExceptIfBetweenMemberOf1",
			"ExceptIfBetweenMemberOf2",
			"ExceptIfManagerAddresses",
			"ExceptIfAnyOfToHeader",
			"ExceptIfAnyOfToHeaderMemberOf",
			"ExceptIfAnyOfCcHeader",
			"ExceptIfAnyOfCcHeaderMemberOf",
			"ExceptIfAnyOfToCcHeader",
			"ExceptIfAnyOfToCcHeaderMemberOf",
			"AddToRecipients",
			"CopyTo",
			"BlindCopyTo",
			"ModerateMessageByUser",
			"RedirectMessageTo"
		};

		public static readonly string[] EdgeSpecificParameters = new string[]
		{
			"Disconnect",
			"LogEventText",
			"SmtpRejectMessageRejectStatusCode",
			"SmtpRejectMessageRejectText"
		};

		public static readonly string[] HubSpecificParameters = new string[]
		{
			"From",
			"FromMemberOf",
			"SentTo",
			"SentToMemberOf",
			"SentToScope",
			"BetweenMemberOf1",
			"BetweenMemberOf2",
			"ManagerAddresses",
			"ManagerForEvaluatedUser",
			"SenderDomainIs",
			"SenderManagementRelationship",
			"ADComparisonAttribute",
			"ADComparisonOperator",
			"SenderADAttributeContainsWords",
			"SenderADAttributeMatchesPatterns",
			"RecipientADAttributeContainsWords",
			"RecipientADAttributeMatchesPatterns",
			"AnyOfToHeader",
			"AnyOfToHeaderMemberOf",
			"AnyOfCcHeader",
			"AnyOfCcHeaderMemberOf",
			"AnyOfToCcHeader",
			"AnyOfToCcHeaderMemberOf",
			"HasClassification",
			"HasNoClassification",
			"AttachmentContainsWords",
			"AttachmentIsUnsupported",
			"AttachmentNameMatchesPatterns",
			"AttachmentExtensionMatchesWords",
			"AttachmentHasExecutableContent",
			"AttachmentMatchesPatterns",
			"AttachmentProcessingLimitExceeded",
			"AttachmentPropertyContainsWords",
			"ContentCharacterSetContainsWords",
			"WithImportance",
			"MessageTypeMatches",
			"RecipientAddressContainsWords",
			"RecipientAddressMatchesPatterns",
			"HasSenderOverride",
			"MessageContainsDataClassifications",
			"ExceptIfFrom",
			"ExceptIfFromMemberOf",
			"ExceptIfSentTo",
			"ExceptIfSentToMemberOf",
			"ExceptIfSentToScope",
			"ExceptIfBetweenMemberOf1",
			"ExceptIfBetweenMemberOf2",
			"ExceptIfManagerAddresses",
			"ExceptIfManagerForEvaluatedUser",
			"ExceptIfSenderDomainIs",
			"ExceptIfSenderManagementRelationship",
			"ExceptIfADComparisonAttribute",
			"ExceptIfADComparisonOperator",
			"ExceptIfSenderADAttributeContainsWords",
			"ExceptIfSenderADAttributeMatchesPatterns",
			"ExceptIfRecipientADAttributeContainsWords",
			"ExceptIfRecipientADAttributeMatchesPatterns",
			"ExceptIfAnyOfToHeader",
			"ExceptIfAnyOfToHeaderMemberOf",
			"ExceptIfAnyOfCcHeader",
			"ExceptIfAnyOfCcHeaderMemberOf",
			"ExceptIfAnyOfToCcHeader",
			"ExceptIfAnyOfToCcHeaderMemberOf",
			"ExceptIfHasClassification",
			"ExceptIfHasNoClassification",
			"ExceptIfAttachmentContainsWords",
			"ExceptIfAttachmentIsUnsupported",
			"ExceptIfAttachmentNameMatchesPatterns",
			"ExceptIfAttachmentExtensionMatchesWords",
			"ExceptIfAttachmentHasExecutableContent",
			"ExceptIfAttachmentMatchesPatterns",
			"ExceptIfAttachmentProcessingLimitExceeded",
			"ExceptIfAttachmentPropertyContainsWords",
			"ExceptIfContentCharacterSetContainsWords",
			"ExceptIfWithImportance",
			"ExceptIfMessageTypeMatches",
			"ExceptIfRecipientAddressContainsWords",
			"ExceptIfRecipientAddressMatchesPatterns",
			"SetAuditSeverity",
			"NotifySender",
			"ApplyClassification",
			"ApplyHtmlDisclaimerLocation",
			"ApplyHtmlDisclaimerText",
			"ApplyHtmlDisclaimerFallbackAction",
			"ApplyRightsProtectionTemplate",
			"AddManagerAsRecipientType",
			"ModerateMessageByUser",
			"ModerateMessageByManager",
			"RejectMessageEnhancedStatusCode",
			"RejectMessageReasonText",
			"SenderInRecipientList",
			"SenderIpRanges",
			"ExceptIfSenderIpRanges",
			"RecipientInSenderList",
			"ExceptIfSenderInRecipientList",
			"ExceptIfRecipientInSenderList",
			"ExceptIfHasSenderOverride",
			"ExceptIfMessageContainsDataClassifications",
			"RouteMessageOutboundConnector",
			"RouteMessageOutboundRequireTls",
			"ApplyOME",
			"RemoveOME",
			"GenerateNotification",
			"GenerateIncidentReport",
			"IncidentReportOriginalMail",
			"IncidentReportContent",
			"Quarantine"
		};

		public static readonly string[] ContainsWordsParameters = new string[]
		{
			"AttachmentContainsWords",
			"AttachmentPropertyContainsWords",
			"AttachmentExtensionMatchesWords",
			"ContentCharacterSetContainsWords",
			"HeaderContainsWords",
			"FromAddressContainsWords",
			"AnyOfRecipientAddressContainsWords",
			"SubjectContainsWords",
			"SubjectOrBodyContainsWords",
			"RecipientAddressContainsWords",
			"ExceptIfAttachmentContainsWords",
			"ExceptIfAttachmentPropertyContainsWords",
			"ExceptIfAttachmentExtensionMatchesWords",
			"ExceptIfContentCharacterSetContainsWords",
			"ExceptIfHeaderContainsWords",
			"ExceptIfFromAddressContainsWords",
			"ExceptIfAnyOfRecipientAddressContainsWords",
			"ExceptIfSubjectContainsWords",
			"ExceptIfSubjectOrBodyContainsWords",
			"ExceptIfRecipientAddressContainsWords"
		};

		public static readonly string[] MatchesPatternsParameters = new string[]
		{
			"FromAddressMatchesPatterns",
			"RecipientAddressMatchesPatterns",
			"SubjectMatchesPatterns",
			"SubjectOrBodyMatchesPatterns",
			"AttachmentMatchesPatterns",
			"AttachmentNameMatchesPatterns",
			"HeaderMatchesPatterns",
			"ExceptIfFromAddressMatchesPatterns",
			"ExceptIfRecipientAddressMatchesPatterns",
			"ExceptIfSubjectMatchesPatterns",
			"ExceptIfSubjectOrBodyMatchesPatterns",
			"ExceptIfAttachmentMatchesPatterns",
			"ExceptIfAttachmentNameMatchesPatterns",
			"ExceptIfHeaderMatchesPatterns"
		};

		public static readonly string[] AdAttributeContainsWordsParameters = new string[]
		{
			"RecipientADAttributeContainsWords",
			"SenderADAttributeContainsWords",
			"ExceptIfRecipientADAttributeContainsWords",
			"ExceptIfSenderADAttributeContainsWords"
		};

		public static readonly string[] AdAttributeMatchesPatternsParameters = new string[]
		{
			"RecipientADAttributeMatchesPatterns",
			"SenderADAttributeMatchesPatterns",
			"ExceptIfRecipientADAttributeMatchesPatterns",
			"ExceptIfSenderADAttributeMatchesPatterns"
		};

		public static readonly string[] PropertyContainsWordsParameters = new string[]
		{
			"AttachmentPropertyContainsWords",
			"ExceptIfAttachmentPropertyContainsWords"
		};

		public static readonly string[] TenantProhibitedHeaderPrefixes = new string[]
		{
			"X-MS-Exchange-Organization-",
			"X-MS-Exchange-Forest-"
		};

		public static readonly Dictionary<Type, string> ActionWhichMustBeSingleton = new Dictionary<Type, string>
		{
			{
				typeof(ModerateMessageByUserAction),
				Strings.ModerateActionMustBeTheOnlyAction
			},
			{
				typeof(ModerateMessageByManagerAction),
				Strings.ModerateActionMustBeTheOnlyAction
			},
			{
				typeof(DeleteMessageAction),
				Strings.DeleteMessageActionMustBeTheOnlyAction
			},
			{
				typeof(RejectMessageAction),
				Strings.RejectMessageActionMustBeTheOnlyAction
			},
			{
				typeof(SmtpRejectMessageAction),
				Strings.RejectMessageActionMustBeTheOnlyAction
			}
		};

		public static readonly Type[] ActionsWhichCouldAlwaysCoExist = new Type[]
		{
			typeof(SetAuditSeverityAction),
			typeof(GenerateIncidentReportAction),
			typeof(GenerateNotificationAction),
			typeof(StopRuleProcessingAction)
		};

		internal static readonly string[] ProhibitedLeadingAndTrailingCharactersInPatterns = new string[]
		{
			"\\S"
		};

		public static class MessageContainsDataClassificationsParameters
		{
			internal const string Name = "Name";

			internal const string MinCount = "MinCount";

			internal const string MaxCount = "MaxCount";

			internal const string MinConfidence = "MinConfidence";

			internal const string MaxConfidence = "MaxConfidence";
		}

		internal delegate bool TransportRuleSelectionDelegate(Rule transportRule, object delegateContext);

		private delegate void ArrayPredicateInitializer<in PredicateType, in ParameterType>(PredicateType predicate, ParameterType[] parameters);

		private delegate void NonArrayPredicateInitializer<in PredicateType, in ParameterType>(PredicateType predicate, ParameterType parameter);

		private delegate void ActionInitializer<in ActionType, in ParameterType>(ActionType action, ParameterType[] parameters);
	}
}
