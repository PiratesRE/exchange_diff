using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Net;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.Tasks;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Search.Query;

namespace Microsoft.Office.CompliancePolicy.Validators
{
	internal class SharepointValidator : SourceValidator
	{
		public SharepointValidator(Uri spSiteContextUrl, ICredentials credentials, bool validateUsingSearch, Task.TaskErrorLoggingDelegate writeErrorDelegate, Action<LocalizedString> writeWarningDelegate, Func<LocalizedString, bool> shouldContinueDelegate, int maxSitesLimit, ExecutionLog logger, string logTag, string tenantId, SourceValidator.Clients client) : base(writeErrorDelegate, writeWarningDelegate, shouldContinueDelegate, logger, logTag, tenantId, client)
		{
			ArgumentValidator.ThrowIfNull("spSiteContextUrl", spSiteContextUrl);
			ArgumentValidator.ThrowIfNull("credentials", credentials);
			this.spSiteContextUrl = spSiteContextUrl;
			this.credentials = credentials;
			this.validateUsingSearch = validateUsingSearch;
			this.maxSitesLimit = maxSitesLimit;
		}

		public MultiValuedProperty<BindingMetadata> ValidateLocations(IEnumerable<string> locations)
		{
			ArgumentValidator.ThrowIfNull("locations", locations);
			MultiValuedProperty<BindingMetadata> multiValuedProperty = new MultiValuedProperty<BindingMetadata>();
			if (locations.Count<string>() > this.maxSitesLimit)
			{
				base.LogOneEntry(ExecutionLog.EventType.Error, "InvalidArgument: {0}", new object[]
				{
					Strings.ErrorMaxSiteLimit(this.maxSitesLimit, locations.Count<string>())
				});
				base.WriteError(new SpValidatorException(Strings.ErrorMaxSiteLimit(this.maxSitesLimit, locations.Count<string>())), ErrorCategory.InvalidArgument);
			}
			foreach (string location in locations)
			{
				SharepointValidationResult validationResult = this.ValidateLocation(location);
				if (validationResult.IsValid)
				{
					if (validationResult.IsTopLevelSiteCollection)
					{
						base.WriteWarning(validationResult.ValidationText);
					}
					if (!multiValuedProperty.Any((BindingMetadata p) => string.Equals(validationResult.SharepointSource.Identity, p.ImmutableIdentity, StringComparison.OrdinalIgnoreCase)))
					{
						multiValuedProperty.Add(new BindingMetadata(validationResult.SharepointSource.Title, validationResult.SharepointSource.SiteUrl, validationResult.SharepointSource.Identity, SourceValidator.GetBindingType(validationResult.SharepointSource.Identity)));
					}
				}
				else
				{
					base.WriteError(new SpValidatorException(validationResult.ValidationText), ErrorCategory.InvalidArgument);
				}
			}
			return multiValuedProperty;
		}

		public SharepointValidationResult ValidateLocation(string location)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("location", location);
			if (SourceValidator.IsWideScope(location))
			{
				base.LogOneEntry(ExecutionLog.EventType.Verbose, "Skipping validation for wide scoped location '{0}", new object[]
				{
					location
				});
				return new SharepointValidationResult
				{
					IsTopLevelSiteCollection = false,
					IsValid = true,
					SharepointSource = new SharepointSource(location, location, Guid.Empty, Guid.Empty)
				};
			}
			SharepointValidationResult result = null;
			try
			{
				if (this.validateUsingSearch)
				{
					result = this.ValidateLocationBySearch(location);
				}
				else
				{
					result = this.ValidateLocationByLoad(location);
				}
			}
			catch (Exception ex)
			{
				base.LogOneEntry(ExecutionLog.EventType.Error, ex, "Unexpected Exception occurred when Validating the location {0}. ValidatingUsingSearch: {1}", new object[]
				{
					location,
					this.validateUsingSearch
				});
				EventNotificationItem.Publish(ExchangeComponent.UnifiedComplianceSourceValidation.Name, "SharepointValidatorUnexpectedError", base.Client.ToString(), string.Format("Unexpected exception occured when validating the sites(using search:{0}). Exception:{1}", this.validateUsingSearch, ex), ResultSeverityLevel.Error, false);
				throw;
			}
			return result;
		}

		internal static ISharepointCsomProvider CsomProvider
		{
			get
			{
				if (SharepointValidator.csomProviderInstance == null)
				{
					ISharepointCsomProvider sharepointCsomProvider;
					if (Utils.TryGetMockCsomProvider(out sharepointCsomProvider))
					{
						SharepointValidator.csomProviderInstance = sharepointCsomProvider;
					}
					else
					{
						SharepointValidator.csomProviderInstance = new SharepointCsomProvider();
					}
				}
				return SharepointValidator.csomProviderInstance;
			}
			set
			{
				SharepointValidator.csomProviderInstance = value;
			}
		}

		private SharepointValidationResult ValidateLocationByLoad(string location)
		{
			SharepointValidationResult validationResult = null;
			try
			{
				Uri uri = new Uri(location, UriKind.Absolute);
				if (this.spSiteContextUrl.IsBaseOf(uri))
				{
					Utils.WrapSharePointCsomCall(uri, this.credentials, delegate(ClientContext context)
					{
						string siteUrl;
						string text;
						Guid siteId;
						Guid webId;
						SharepointValidator.CsomProvider.LoadWebInfo(context, out siteUrl, out text, out siteId, out webId);
						if (string.IsNullOrWhiteSpace(text))
						{
							this.LogOneEntry(ExecutionLog.EventType.Error, "The site is invalid as it missing the title: {0}", new object[]
							{
								Strings.SpLocationValidationFailed(location)
							});
							validationResult = SharepointValidator.CreateFailedResult(Strings.SpLocationValidationFailed(location));
							return;
						}
						SharepointSource sharepointSource = new SharepointSource(siteUrl, text, siteId, webId);
						validationResult = new SharepointValidationResult
						{
							SharepointSource = sharepointSource,
							IsValid = true
						};
						if (this.spSiteContextUrl.Equals(sharepointSource.SiteUrl))
						{
							validationResult.IsTopLevelSiteCollection = true;
							validationResult.ValidationText = Strings.SpLocationHasMultipleSites(location);
							this.LogOneEntry(ExecutionLog.EventType.Warning, "Found top level site collection. {0}", new object[]
							{
								validationResult.ValidationText
							});
						}
					});
				}
				else
				{
					base.LogOneEntry(ExecutionLog.EventType.Error, "InvalidArgument: {0}, the url does not start with the tenant root url {1}", new object[]
					{
						Strings.SpLocationValidationFailed(location),
						this.spSiteContextUrl
					});
					validationResult = SharepointValidator.CreateFailedResult(Strings.SpLocationValidationFailed(location));
				}
			}
			catch (UriFormatException exception)
			{
				base.LogOneEntry(ExecutionLog.EventType.Error, exception, "InvalidArgument: {0}", new object[]
				{
					Strings.SpLocationValidationFailed(location)
				});
				validationResult = SharepointValidator.CreateFailedResult(Strings.SpLocationValidationFailed(location));
			}
			catch (SpCsomCallException ex)
			{
				if (!(ex.InnerException is WebException) && !(ex.InnerException is ClientRequestException))
				{
					throw;
				}
				base.LogOneEntry(ExecutionLog.EventType.Error, ex, "InvalidArgument: {0}", new object[]
				{
					Strings.SpLocationValidationFailed(location)
				});
				validationResult = SharepointValidator.CreateFailedResult(Strings.SpLocationValidationFailed(location));
			}
			return validationResult;
		}

		private SharepointValidationResult ValidateLocationBySearch(string location)
		{
			SharepointValidationResult validationResult = null;
			Utils.WrapSharePointCsomCall(this.spSiteContextUrl, this.credentials, delegate(ClientContext context)
			{
				ResultTableCollection queryResults = SharepointValidator.CsomProvider.ExecuteSearch(context, location, false);
				validationResult = this.GetValidationResult(queryResults, context);
			});
			if (validationResult == null)
			{
				base.LogOneEntry(ExecutionLog.EventType.Error, "InvalidArgument: {0}", new object[]
				{
					Strings.SpLocationValidationFailed(location)
				});
				validationResult = SharepointValidator.CreateFailedResult(Strings.SpLocationValidationFailed(location));
			}
			return validationResult;
		}

		public static SharepointValidator Create(IConfigurationSession configurationSession, ADObjectId executingUserId, ExecutionLog logger)
		{
			return SharepointValidator.Create(configurationSession, executingUserId, null, null, null, "SitePicker validation", SourceValidator.Clients.UccPolicyUI, 0, logger);
		}

		public static SharepointValidator Create(IConfigurationSession configurationSession, ExchangeRunspaceConfiguration exchangeRunspaceConfig, Task.TaskErrorLoggingDelegate writeErrorDelegate, Action<LocalizedString> writeWarningDelegate, Func<LocalizedString, bool> shouldContinueDelegate, string logTag, SourceValidator.Clients client, int existingSitesCount, ExecutionLog logger)
		{
			bool boolFromConfig = Utils.GetBoolFromConfig("ValidateSharepointUsingSearch", true);
			ADObjectId executingUserId;
			if (boolFromConfig)
			{
				if (exchangeRunspaceConfig == null || !exchangeRunspaceConfig.TryGetExecutingUserId(out executingUserId))
				{
					throw new SpValidatorException(Strings.FailedToGetExecutingUser);
				}
			}
			else
			{
				executingUserId = null;
			}
			return SharepointValidator.Create(configurationSession, executingUserId, writeErrorDelegate, writeWarningDelegate, shouldContinueDelegate, logTag, client, existingSitesCount, logger);
		}

		private static SharepointValidator Create(IConfigurationSession configurationSession, ADObjectId executingUserId, Task.TaskErrorLoggingDelegate writeErrorDelegate, Action<LocalizedString> writeWarningDelegate, Func<LocalizedString, bool> shouldContinueDelegate, string logTag, SourceValidator.Clients client, int existingSitesCount, ExecutionLog logger)
		{
			ArgumentValidator.ThrowIfNull("configurationSession", configurationSession);
			Uri uri = null;
			Uri uri2 = null;
			OrganizationId organizationId = configurationSession.GetOrgContainer().OrganizationId;
			UnifiedPolicyConfiguration.GetInstance().GetTenantSharePointUrls(configurationSession, out uri, out uri2);
			if (uri == null)
			{
				EventNotificationItem.Publish(ExchangeComponent.UnifiedComplianceSourceValidation.Name, "SharepointValidatorUnexpectedError", client.ToString(), string.Format("Tenant {0}, Error:{1}", organizationId.ToExternalDirectoryOrganizationId(), Strings.FailedToGetSpSiteUrlForTenant), ResultSeverityLevel.Error, false);
				throw new SpValidatorException(Strings.FailedToGetSpSiteUrlForTenant);
			}
			ADUser actAsUser = null;
			if (executingUserId != null)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromExternalDirectoryOrganizationId(new Guid(organizationId.ToExternalDirectoryOrganizationId()));
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 520, "Create", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\UnifiedPolicy\\Validators\\SharepointValidator.cs");
				actAsUser = tenantOrRootOrgRecipientSession.FindADUserByObjectId(executingUserId);
			}
			ICredentials credentials = UnifiedPolicyConfiguration.GetInstance().GetCredentials(configurationSession, actAsUser);
			if (credentials == null)
			{
				EventNotificationItem.Publish(ExchangeComponent.UnifiedComplianceSourceValidation.Name, "SharepointValidatorUnexpectedError", client.ToString(), string.Format("Tenant {0}, Error:{1}", organizationId.ToExternalDirectoryOrganizationId(), Strings.FailedToGetCredentialsForTenant), ResultSeverityLevel.Error, false);
				throw new SpValidatorException(Strings.FailedToGetCredentialsForTenant);
			}
			int maxLimitFromConfig = SourceValidator.GetMaxLimitFromConfig("MaxSitesLimit", 100, existingSitesCount);
			return new SharepointValidator(uri, credentials, executingUserId != null, writeErrorDelegate, writeWarningDelegate, shouldContinueDelegate, maxLimitFromConfig, logger, logTag, organizationId.ToExternalDirectoryOrganizationId(), client);
		}

		private static SharepointValidationResult CreateFailedResult(LocalizedString message)
		{
			return new SharepointValidationResult
			{
				IsValid = false,
				ValidationText = message
			};
		}

		private SharepointValidationResult ValidateLocation(Guid webId, Guid siteId, ClientContext context)
		{
			ResultTableCollection queryResults = SharepointValidator.CsomProvider.ExecuteSearch(context, webId, siteId);
			return this.GetValidationResult(webId, siteId, queryResults, context);
		}

		private bool HasMultipleSitesUnderLocation(string location, ClientContext context)
		{
			ResultTableCollection resultTableCollection = SharepointValidator.CsomProvider.ExecuteSearch(context, location, true);
			foreach (ResultTable resultTable in resultTableCollection)
			{
				if (resultTable.TableType == KnownTableTypes.RelevantResults && resultTable.RowCount > 1)
				{
					return true;
				}
			}
			return false;
		}

		private SharepointValidationResult GetValidationResult(ResultTableCollection queryResults, ClientContext context)
		{
			SharepointValidationResult result = null;
			foreach (ResultTable resultTable in queryResults)
			{
				if (resultTable.TableType == KnownTableTypes.RelevantResults)
				{
					foreach (IDictionary<string, object> dictionary in resultTable.ResultRows)
					{
						string text = null;
						string text2 = null;
						if (dictionary.ContainsKey("WebId") && dictionary.ContainsKey("SiteId"))
						{
							text = (dictionary["WebId"] as string);
							text2 = (dictionary["SiteId"] as string);
						}
						if (!string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(text2))
						{
							Guid webId = new Guid(text);
							Guid siteId = new Guid(text2);
							string text3 = dictionary["contentclass"] as string;
							if (text3 == "STS_Web" || text3 == "STS_Site")
							{
								result = this.GetValidationResult(webId, siteId, text3, dictionary, context);
								break;
							}
							result = this.ValidateLocation(webId, siteId, context);
							break;
						}
					}
				}
			}
			return result;
		}

		private SharepointValidationResult GetValidationResult(Guid webId, Guid siteId, string contentClass, IDictionary<string, object> resultRow, ClientContext context)
		{
			SharepointSource sharepointSource = new SharepointSource(resultRow["Path"] as string, resultRow["Title"] as string, siteId, webId);
			SharepointValidationResult sharepointValidationResult = new SharepointValidationResult();
			sharepointValidationResult.SharepointSource = sharepointSource;
			sharepointValidationResult.IsValid = true;
			if (contentClass == "STS_Site" && this.HasMultipleSitesUnderLocation(sharepointSource.SiteUrl, context))
			{
				sharepointValidationResult.ValidationText = Strings.SpLocationHasMultipleSites(sharepointSource.SiteUrl);
				sharepointValidationResult.IsTopLevelSiteCollection = true;
				base.LogOneEntry(ExecutionLog.EventType.Warning, "Found top level site collection. {0}", new object[]
				{
					sharepointValidationResult.ValidationText
				});
			}
			return sharepointValidationResult;
		}

		private SharepointValidationResult GetValidationResult(Guid webId, Guid siteId, ResultTableCollection queryResults, ClientContext context)
		{
			SharepointValidationResult result = null;
			foreach (ResultTable resultTable in queryResults)
			{
				if (resultTable.TableType == KnownTableTypes.RelevantResults)
				{
					foreach (IDictionary<string, object> dictionary in resultTable.ResultRows)
					{
						if (dictionary.ContainsKey("Path"))
						{
							string contentClass = dictionary["contentclass"] as string;
							result = this.GetValidationResult(webId, siteId, contentClass, dictionary, context);
							break;
						}
					}
				}
			}
			return result;
		}

		private const string WebContentClass = "STS_Web";

		private const string SiteContentClass = "STS_Site";

		private const int MaxSitesDefaultLimit = 100;

		private const string MaxSitesLimitKey = "MaxSitesLimit";

		private const string ValidateSharepointUsingSearchKey = "ValidateSharepointUsingSearch";

		private const string UnexpectedErrorEvent = "SharepointValidatorUnexpectedError";

		private readonly int maxSitesLimit;

		private readonly ICredentials credentials;

		private readonly Uri spSiteContextUrl;

		private readonly bool validateUsingSearch;

		private static ISharepointCsomProvider csomProviderInstance;
	}
}
