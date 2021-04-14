using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Web;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.ControlPanel;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.PolicyEvaluation;
using Microsoft.Office.CompliancePolicy.Tasks;

namespace Microsoft.Exchange.Management.DDIService
{
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class DDIService : IDDIService
	{
		public static bool UseDDIService(WebServiceReference service)
		{
			return service != null && !string.IsNullOrEmpty(service.ServiceUrl) && service.ServiceUrl.IndexOf("DDIService.svc", StringComparison.OrdinalIgnoreCase) != -1;
		}

		internal static void AddKnownType(ICustomAttributeProvider provider, Type type)
		{
			List<Type> list = (List<Type>)DDIService.GetKnownTypes(provider);
			list.Add(type);
		}

		private static IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider provider)
		{
			return DDIService.KnownTypes.Value;
		}

		public PowerShellResults<JsonDictionary<object>> GetList(DDIParameters filter, SortOptions sort)
		{
			DDIServiceHelper ddiserviceHelper = this.CreateDDIServiceHelper();
			return ddiserviceHelper.GetList(filter, sort);
		}

		public PowerShellResults<JsonDictionary<object>> GetObject(Identity identity)
		{
			DDIServiceHelper ddiserviceHelper = this.CreateDDIServiceHelper();
			return ddiserviceHelper.GetObject(identity);
		}

		public PowerShellResults<JsonDictionary<object>> GetObjectOnDemand(Identity identity, string workflow)
		{
			DDIServiceHelper ddiserviceHelper = this.CreateDDIServiceHelper(workflow);
			return ddiserviceHelper.GetObjectOnDemand(identity);
		}

		public PowerShellResults<JsonDictionary<object>> GetObjectForNew(Identity identity)
		{
			DDIServiceHelper ddiserviceHelper = this.CreateDDIServiceHelper();
			return ddiserviceHelper.GetObjectForNew(identity);
		}

		public PowerShellResults<JsonDictionary<object>> SetObject(Identity identity, DDIParameters properties)
		{
			DDIServiceHelper ddiserviceHelper = this.CreateDDIServiceHelper();
			return ddiserviceHelper.SetObject(identity, properties);
		}

		public PowerShellResults<JsonDictionary<object>> GetProgress(string progressId)
		{
			DDIServiceHelper ddiserviceHelper = this.CreateDDIServiceHelper();
			return ddiserviceHelper.GetProgress(progressId, HttpContext.Current != null && "GetList".Equals(HttpContext.Current.Request.QueryString["AyncMethod"], StringComparison.OrdinalIgnoreCase));
		}

		public PowerShellResults Cancel(string progressId)
		{
			DDIServiceHelper ddiserviceHelper = this.CreateDDIServiceHelper();
			return ddiserviceHelper.Cancel(progressId);
		}

		public PowerShellResults<JsonDictionary<object>> NewObject(DDIParameters properties)
		{
			DDIServiceHelper ddiserviceHelper = this.CreateDDIServiceHelper();
			return ddiserviceHelper.NewObject(properties);
		}

		public PowerShellResults RemoveObjects(Identity[] identities, DDIParameters parameters)
		{
			DDIServiceHelper ddiserviceHelper = this.CreateDDIServiceHelper();
			return ddiserviceHelper.RemoveObjects(identities, parameters);
		}

		public PowerShellResults<JsonDictionary<object>> MultiObjectExecute(Identity[] identities, DDIParameters parameters)
		{
			DDIServiceHelper ddiserviceHelper = this.CreateDDIServiceHelper();
			return ddiserviceHelper.MultiObjectExecute(identities, parameters);
		}

		public PowerShellResults<JsonDictionary<object>> SingleObjectExecute(Identity identity, DDIParameters properties)
		{
			DDIServiceHelper ddiserviceHelper = this.CreateDDIServiceHelper();
			return ddiserviceHelper.SingleObjectExecute(identity, properties);
		}

		public void InitializeOperationContext(string serviceUrl)
		{
			if (string.IsNullOrEmpty(serviceUrl))
			{
				throw new ArgumentNullException("serviceUrl");
			}
			Uri uri = new Uri(serviceUrl, UriKind.RelativeOrAbsolute);
			if (!uri.IsAbsoluteUri)
			{
				uri = new Uri(new Uri("http://localhost"), serviceUrl);
			}
			UriTemplate uriTemplate = new UriTemplate("*");
			Uri baseAddress = new Uri(uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped));
			UriTemplateMatch uriTemplateMatch = uriTemplate.Match(baseAddress, uri);
			this.schema = uriTemplateMatch.QueryParameters["schema"];
			this.workflow = uriTemplateMatch.QueryParameters["workflow"];
		}

		private DDIServiceHelper CreateDDIServiceHelper()
		{
			return this.CreateDDIServiceHelper(null);
		}

		private DDIServiceHelper CreateDDIServiceHelper(string workflow)
		{
			this.GetSchemaAndWorkflowFromRequest();
			return new DDIServiceHelper(this.schema, workflow ?? this.workflow);
		}

		private void GetSchemaAndWorkflowFromRequest()
		{
			if (WebOperationContext.Current != null)
			{
				UriTemplateMatch uriTemplateMatch = WebOperationContext.Current.IncomingRequest.UriTemplateMatch;
				this.schema = uriTemplateMatch.QueryParameters["schema"];
				if (string.IsNullOrEmpty(this.schema))
				{
					throw new FaultException(new ArgumentNullException("schema").Message);
				}
				this.workflow = uriTemplateMatch.QueryParameters["workflow"];
			}
		}

		internal const string GetListString = "GetList";

		private const string AyncMethodString = "AyncMethod";

		private const string DDIServiceSVC = "DDIService.svc";

		public const string SchemaParameter = "schema";

		public const string WorkflowParameter = "workflow";

		public static readonly string DDIPath = Path.Combine(ConfigurationContext.Setup.InstallPath, "ClientAccess\\ecp\\DDI");

		private string schema;

		private string workflow;

		internal static LazilyInitialized<List<Type>> KnownTypes = new LazilyInitialized<List<Type>>(delegate()
		{
			List<Type> list = new List<Type>();
			foreach (Type type in typeof(DDIService).Assembly.GetTypes())
			{
				if (type.IsDataContract() || type.GetCustomAttributes(typeof(SerializableAttribute), false).Length > 0)
				{
					if (!type.IsGenericTypeDefinition)
					{
						list.Add(type);
					}
					else
					{
						Type[] genericArguments = type.GetGenericArguments();
						Type[] array = new Type[genericArguments.Length];
						int num = 0;
						while (num < genericArguments.Length && !(genericArguments[num].BaseType != typeof(object)))
						{
							array[num] = typeof(object);
							num++;
						}
						if (num == genericArguments.Length - 1)
						{
							list.Add(type.MakeGenericType(array));
						}
					}
				}
			}
			list.Add(typeof(ADObjectId));
			list.Add(typeof(List<OwaMailboxPolicyFeatureInfo>));
			list.Add(typeof(List<AdObjectResolverRow>));
			list.Add(typeof(List<RecipientObjectResolverRow>));
			list.Add(typeof(List<AcePermissionRecipientRow>));
			list.Add(typeof(List<MailCertificate>));
			list.Add(typeof(List<SharingDomain>));
			list.Add(typeof(List<object>));
			list.Add(typeof(ValidatorInfo[]));
			list.Add(typeof(Identity[]));
			list.Add(typeof(ErrorRecord[]));
			list.Add(typeof(string[]));
			list.Add(typeof(ProxyAddressCollection));
			list.Add(typeof(SmtpProxyAddress));
			list.Add(typeof(SmtpProxyAddressPrefix));
			list.Add(typeof(EumProxyAddress));
			list.Add(typeof(EumProxyAddressPrefix));
			list.Add(typeof(CustomProxyAddress));
			list.Add(typeof(CustomProxyAddressPrefix));
			list.Add(typeof(ProxyAddress));
			list.Add(typeof(CountryInfo));
			list.Add(typeof(ProxyAddressTemplate));
			list.Add(typeof(List<SharingRuleEntry>));
			list.Add(typeof(List<PublicFolderPermissionInfo>));
			list.Add(typeof(SmtpDomain));
			list.Add(typeof(PowerShellResults<JsonDictionary<object>>));
			list.Add(typeof(List<IPAddressEntry>));
			list.Add(typeof(List<DomainEntry>));
			list.Add(typeof(MultiValuedProperty<IPRange>));
			list.Add(typeof(ADMultiValuedProperty<ADObjectId>));
			list.Add(typeof(MultiValuedProperty<SmtpDomain>));
			list.Add(typeof(bool[]));
			list.Add(typeof(EnhancedTimeSpan));
			list.Add(typeof(List<RetentionPolicyTagResolverRow>));
			list.Add(typeof(List<ServerResolverRow>));
			list.Add(typeof(List<MigrationEndpointObject>));
			list.Add(typeof(string[][]));
			list.Add(typeof(List<RecipientObjectResolverRow>));
			list.Add(typeof(MigrationReportGroupDetails[]));
			list.Add(typeof(PeopleIdentity[]));
			list.Add(typeof(PeopleIdentity));
			list.Add(typeof(List<DropDownItemData>));
			list.Add(typeof(UMDialingRuleEntry[]));
			list.Add(typeof(UMDialingRuleGroupRow[]));
			list.Add(typeof(MultiValuedProperty<UMNumberingPlanFormat>));
			list.Add(typeof(MultiValuedProperty<CustomMenuKeyMapping>));
			list.Add(typeof(MultiValuedProperty<HolidaySchedule>));
			list.Add(typeof(List<UMAAHolidaySchedule>));
			list.Add(typeof(List<UMAAMenuKeyMapping>));
			list.Add(typeof(ScheduleInterval[]));
			list.Add(typeof(DataClassificationService.LanguageSetting[]));
			list.Add(typeof(Fingerprint[]));
			list.Add(typeof(JsonDictionary<object>[]));
			list.Add(typeof(SecurityPrincipalPickerObject[]));
			list.Add(typeof(PolicyApplyStatus));
			list.Add(typeof(MultiValuedProperty<PolicyDistributionErrorDetails>));
			list.Add(typeof(List<UHPolicyDistributionErrorDetails>));
			list.Add(typeof(MultiValuedProperty<BindingMetadata>));
			list.Add(typeof(List<UHExchangeBinding>));
			list.Add(typeof(List<UHSharepointBinding>));
			list.Add(typeof(HoldDurationHint));
			list.Add(typeof(DateTime?));
			list.Add(typeof(Unlimited<int>?));
			return list;
		});
	}
}
