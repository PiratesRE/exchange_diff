using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability;
using Microsoft.Exchange.PowerSharp;
using Microsoft.Exchange.PowerSharp.Management;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;
using Microsoft.Online.Administration;
using Microsoft.Online.Administration.WebService;
using Microsoft.Online.Provisioning.CompanyManagement;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Provisioning.Probes
{
	public sealed class ForwardSyncCompanyProbe : ProbeWorkItem
	{
		public static ProbeDefinition CreateDefinition(string name, int companySla)
		{
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = ForwardSyncCompanyProbe.AssemblyPath;
			probeDefinition.TypeName = ForwardSyncCompanyProbe.TypeName;
			probeDefinition.Name = name;
			ForwardSyncCompanyProbe.companySlaSeconds = companySla;
			return probeDefinition;
		}

		public CompanyManagerFederatedServiceTestClient CreateCompanyManagerFederatedServiceTestClient(string endpointUrl, string certSubject, string certThumbprint = "")
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncCompanyProbe.CreateCompanyManagerFederatedServiceTestClient(): starting", null, "CreateCompanyManagerFederatedServiceTestClient", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ForwardSyncCompanyProbe.cs", 120);
			WSHttpBinding wshttpBinding = new WSHttpBinding();
			wshttpBinding.Security.Mode = SecurityMode.Transport;
			wshttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
			wshttpBinding.Security.Message.ClientCredentialType = MessageCredentialType.None;
			EndpointAddress remoteAddress = new EndpointAddress(endpointUrl);
			CompanyManagerFederatedServiceTestClient companyManagerFederatedServiceTestClient = new CompanyManagerFederatedServiceTestClient(wshttpBinding, remoteAddress);
			companyManagerFederatedServiceTestClient.Endpoint.Behaviors.Clear();
			ClientCredentials clientCredentials = new ClientCredentials();
			if (!string.IsNullOrEmpty(certThumbprint))
			{
				clientCredentials.ClientCertificate.SetCertificate(StoreLocation.LocalMachine, StoreName.My, X509FindType.FindByThumbprint, certThumbprint);
			}
			else
			{
				clientCredentials.ClientCertificate.SetCertificate(StoreLocation.LocalMachine, StoreName.My, X509FindType.FindBySubjectName, certSubject);
			}
			companyManagerFederatedServiceTestClient.Endpoint.Behaviors.Add(clientCredentials);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncCompanyProbe.CreateCompanyManagerFederatedServiceTestClient(): completed sucessfully", null, "CreateCompanyManagerFederatedServiceTestClient", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ForwardSyncCompanyProbe.cs", 149);
			return companyManagerFederatedServiceTestClient;
		}

		public void AddSubscription(Guid contextId, string serviceInstance, string partNumber, CompanyManagerFederatedServiceTestClient companyManagerClient)
		{
			WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncCompanyProbe.AddSubscription(): starting for company {0} with part number {1}", contextId.ToString(), partNumber, null, "AddSubscription", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ForwardSyncCompanyProbe.cs", 167);
			Microsoft.Online.Provisioning.CompanyManagement.Subscription subscription = new Microsoft.Online.Provisioning.CompanyManagement.Subscription();
			subscription.ContextId = contextId;
			subscription.AccountId = contextId;
			subscription.PartNumber = partNumber;
			subscription.PrepaidUnits = 10000;
			subscription.AllowedOverageUnits = 0;
			subscription.StartDate = (DateTime)ExDateTime.Now;
			subscription.LifecycleState = SubscriptionState.Active;
			subscription.LifecycleNextStateChangeDate = (DateTime)ExDateTime.Now.AddYears(10);
			subscription.SubscriptionId = Guid.NewGuid();
			companyManagerClient.FederatedServiceAddAuthorizedServiceInstanceToCompany(contextId, serviceInstance);
			companyManagerClient.FederatedServiceCreateUpdateDeleteSubscription(subscription);
			WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncCompanyProbe.AddSubscription() completed successfully for company {0} with part number {1}", contextId.ToString(), partNumber, null, "AddSubscription", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ForwardSyncCompanyProbe.cs", 183);
		}

		public void RemoveSubscription(Guid contextId, Guid subscriptionId, string partNumber, CompanyManagerFederatedServiceTestClient companyManagerClient)
		{
			WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncCompanyProbe.RemoveSubscription(): starting for company {0} with part number {1}", contextId.ToString(), partNumber, null, "RemoveSubscription", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ForwardSyncCompanyProbe.cs", 200);
			companyManagerClient.FederatedServiceCreateUpdateDeleteSubscription(new Microsoft.Online.Provisioning.CompanyManagement.Subscription
			{
				ContextId = contextId,
				AccountId = contextId,
				PartNumber = partNumber,
				StartDate = (DateTime)ExDateTime.Now,
				LifecycleState = SubscriptionState.Delete,
				SubscriptionId = subscriptionId
			});
			WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncCompanyProbe.RemoveSubscription() for company {0} with part number {1} completed successfully", contextId.ToString(), partNumber, null, "RemoveSubscription", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ForwardSyncCompanyProbe.cs", 211);
		}

		public Microsoft.Online.Administration.Subscription[] GetSubscription(string endpointUrl, string adminLiveId, string adminPassword)
		{
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncCompanyProbe.GetSubscription(): starting for {0}", adminLiveId, null, "GetSubscription", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ForwardSyncCompanyProbe.cs", 226);
			BecWebServiceProxy becWebServiceProxy = new BecWebServiceProxy(endpointUrl);
			becWebServiceProxy.SetCurrentUserCredential(adminLiveId, adminPassword);
			Request request = new Request();
			ListSubscriptionsResponse listSubscriptionsResponse = (ListSubscriptionsResponse)becWebServiceProxy.Invoke("ListSubscriptions", request);
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncCompanyProbe.GetSubscription(): completed for {0}", adminLiveId, null, "GetSubscription", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ForwardSyncCompanyProbe.cs", 234);
			return listSubscriptionsResponse.ReturnValue;
		}

		public ExchangeConfigurationUnit GetTestOrganizationCU(Guid externalDirectoryOrgId)
		{
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncCompanyProbe.GetTestOrganizationCU(): starting for {0}", externalDirectoryOrgId.ToString(), null, "GetTestOrganizationCU", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ForwardSyncCompanyProbe.cs", 245);
			this.AppendToLog(true, 0, " ForwardSyncCompanyProbe.GetTestOrganizationCU(): starting", "GetCU", "", "");
			ExchangeConfigurationUnit result = null;
			try
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromExternalDirectoryOrganizationId(externalDirectoryOrgId);
				ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(true, ConsistencyMode.PartiallyConsistent, sessionSettings, 253, "GetTestOrganizationCU", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ForwardSyncCompanyProbe.cs");
				result = tenantConfigurationSession.GetExchangeConfigurationUnitByExternalId(externalDirectoryOrgId.ToString());
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncCompanyProbe.GetTestOrganizationCU(): Org {0} exist.", externalDirectoryOrgId.ToString(), null, "GetTestOrganizationCU", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ForwardSyncCompanyProbe.cs", 258);
				this.AppendToLog(true, 0, " ForwardSyncCompanyProbe.GetTestOrganizationCU(): no exception", "GetCU", "", "");
			}
			catch (CannotResolveExternalDirectoryOrganizationIdException ex)
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncCompanyProbe.GetTestOrganizationCU(): Org {0} not exist.", externalDirectoryOrgId.ToString(), null, "GetTestOrganizationCU", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ForwardSyncCompanyProbe.cs", 264);
				this.AppendToLog(true, 2, ex.ToString(), "GetCU", "", "");
			}
			return result;
		}

		public void RemoveTestOrganization(ExchangeConfigurationUnit testOrgCU)
		{
			string text = string.Empty;
			Thread.Sleep(120000);
			try
			{
				text = testOrgCU.OrganizationalUnitRoot.ToString();
				if (text.ToLower().Contains("a830edad9050849exprv"))
				{
					WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncCompanyProbe.RemoveTestOrganization(): removing org {0}", text, null, "RemoveTestOrganization", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ForwardSyncCompanyProbe.cs", 287);
					this.AppendToLog(true, 0, " ForwardSyncCompanyProbe.RemoveTestOrganization(): removing org", "RemoveOrg", "", text);
					MonadRunspaceConfiguration.AddPSSnapInName("Microsoft.Exchange.Management.PowerShell.E2010");
					MonadRunspaceConfiguration.AddPSSnapInName("Microsoft.Exchange.Management.PowerShell.Setup");
					ExchangeManagementSessionFactory instance = ExchangeManagementSessionFactory.GetInstance();
					RunspaceCache runspaceCache = new RunspaceCache(instance.GetInitialSessionState());
					ExchangeManagementSession session = (ExchangeManagementSession)instance.CreateSession(runspaceCache, null, null);
					RemoveOrganizationCommand removeOrganizationCommand = new RemoveOrganizationCommand(session);
					removeOrganizationCommand.SetParameters(new RemoveOrganizationCommand.IdentityParameters
					{
						Identity = testOrgCU.ExternalDirectoryOrganizationId,
						Force = true,
						Confirm = false
					});
					removeOrganizationCommand.RunCommand();
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncCompanyProbe.RemoveTestOrganization(): for org {0} got exception {1}", text, ex.ToString(), null, "RemoveTestOrganization", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ForwardSyncCompanyProbe.cs", 309);
				this.RemoveTestOrganizationFromAd(testOrgCU);
				return;
			}
			try
			{
				Guid externalDirectoryOrgId = new Guid(testOrgCU.ExternalDirectoryOrganizationId);
				testOrgCU = this.GetTestOrganizationCU(externalDirectoryOrgId);
				if (testOrgCU != null)
				{
					this.AppendToLog(true, 2, " Org is not removed. Retry", "RemoveOrgFromAD", "", text);
					this.RemoveTestOrganizationFromAd(testOrgCU);
				}
			}
			catch (Exception ex2)
			{
				this.AppendToLog(false, 1, ex2.ToString(), "RemoveTestOrg2ndTry", "", text);
				return;
			}
			this.AppendToLog(true, 0, " ForwardSyncCompanyProbe.RemoveTestOrganization completed successfully", "RemoveOrg", "", text);
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncCompanyProbe.RemoveTestOrganization() for org {0} completed successfully", text, null, "RemoveTestOrganization", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ForwardSyncCompanyProbe.cs", 334);
		}

		public bool ShouldRun()
		{
			bool result = false;
			WTFDiagnostics.TraceInformation(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncCompanyProbe.ShouldRun(): starting", null, "ShouldRun", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ForwardSyncCompanyProbe.cs", 345);
			if (ExEnvironment.IsTest)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncCompanyProbe.ShouldRun(): returns true because this is test environment", null, "ShouldRun", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ForwardSyncCompanyProbe.cs", 349);
				return true;
			}
			try
			{
				IADServer localServer = CachedAdReader.Instance.LocalServer;
				if (localServer == null || localServer.DatabaseAvailabilityGroup == null)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncCompanyProbe.ShouldRun(): returns false because Local Server DAG is null.", null, "ShouldRun", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ForwardSyncCompanyProbe.cs", 358);
					return false;
				}
				IADDatabaseAvailabilityGroup localDAG = CachedAdReader.Instance.LocalDAG;
				if (localDAG != null)
				{
					AmServerName primaryActiveManagerNode = DagTaskHelper.GetPrimaryActiveManagerNode(localDAG);
					if (primaryActiveManagerNode != null)
					{
						result = primaryActiveManagerNode.IsLocalComputerName;
					}
				}
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncCompanyProbe.ShouldRun(): got exception {0}", ex.ToString(), null, "ShouldRun", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ForwardSyncCompanyProbe.cs", 375);
			}
			return result;
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (!this.ShouldRun())
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncCompanyProbe.DoWork(): Not run because local server is not PAM of the DAG.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ForwardSyncCompanyProbe.cs", 389);
				return;
			}
			Guid guid = new Guid(base.Definition.TargetResource);
			this.adminLiveId = base.Definition.Account;
			this.serviceInstance = base.Definition.Endpoint;
			this.partNumber = base.Definition.AccountDisplayName;
			if (this.partNumber.CompareTo("EXCHANGELITE") != 0 && this.partNumber.CompareTo("EXCHANGESTANDARD") != 0)
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncCompanyProbe.DoWork(): Not run because part number {0} is not supported.", this.partNumber, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ForwardSyncCompanyProbe.cs", 401);
				return;
			}
			WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncCompanyProbe.DoWork(): starting for company {0} with part number {1}", base.Definition.TargetResource, this.partNumber, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ForwardSyncCompanyProbe.cs", 405);
			string accountPassword = base.Definition.AccountPassword;
			string certSubject = null;
			string certThumbprint = null;
			string text = null;
			string text2 = null;
			AttributeHelper attributeHelper = new AttributeHelper(base.Definition);
			if (this.adminLiveId.EndsWith(".msol-test.com", StringComparison.OrdinalIgnoreCase))
			{
				certSubject = attributeHelper.GetString("TDCertificateSubject", true, "auth.exchangelabs.live-int.com");
				certThumbprint = attributeHelper.GetString("TDCertThumbprint", false, "");
				text = attributeHelper.GetString("TDCompanyManagerUrl", true, "https://companymanager.msol-test.com/CompanyManager.svc");
				text2 = attributeHelper.GetString("TDProvisioningApiUrl", true, "https://provisioningapi.msol-test.com/provisioningwebservice.svc");
			}
			else if (this.adminLiveId.EndsWith(".ccsctp.net", StringComparison.OrdinalIgnoreCase))
			{
				certSubject = attributeHelper.GetString("SDFCertificateSubject", true, "auth.outlook.com");
				certThumbprint = attributeHelper.GetString("SDFCertThumbprint", false, "");
				text = attributeHelper.GetString("SDFCompanyManagerUrl", true, "https://companymanager.ccsctp.com/CompanyManager.svc");
				text2 = attributeHelper.GetString("SDFProvisioningApiUrl", true, "https://provisioningapi.ccsctp.com/provisioningwebservice.svc");
			}
			else if (this.adminLiveId.EndsWith(".partner.onmschina.cn", StringComparison.OrdinalIgnoreCase))
			{
				certSubject = attributeHelper.GetString("GallatinCertificateSubject", true, "auth.partner.outlook.cn");
				certThumbprint = attributeHelper.GetString("GallatinCertThumbprint", false, "");
				text = attributeHelper.GetString("GallatinCompanyManagerUrl", true, "https://companymanager.partner.microsoftonline.cn/CompanyManager.svc");
				text2 = attributeHelper.GetString("GallatinProvisioningApiUrl", true, "https://provisioningapi.partner.microsoftonline.cn/ProvisioningWebService.svc");
			}
			else
			{
				certSubject = attributeHelper.GetString("ProdCertificateSubject", true, "auth.outlook.com");
				certThumbprint = attributeHelper.GetString("ProdCertThumbprint", false, "");
				text = attributeHelper.GetString("ProdCompanyManagerUrl", true, "https://companymanager.microsoftonline.com/CompanyManager.svc");
				text2 = attributeHelper.GetString("ProdProvisioningApiUrl", true, "https://provisioningapi.microsoftonline.com/provisioningwebservice.svc");
			}
			this.testOrgCU = this.GetTestOrganizationCU(guid);
			string[] array = null;
			Microsoft.Online.Administration.Subscription[] subscriptions = null;
			try
			{
				subscriptions = this.GetSubscription(text2, this.adminLiveId, accountPassword);
			}
			catch (Exception ex)
			{
				if (array != null)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "Received unexpected second redirection for provisioningApiUrl", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ForwardSyncCompanyProbe.cs", 456);
					throw;
				}
				Exception innerException = ex.InnerException;
				if (innerException == null)
				{
					throw;
				}
				TargetInvocationException ex2 = innerException as TargetInvocationException;
				if (ex2 == null)
				{
					throw;
				}
				Exception innerException2 = ex2.InnerException;
				if (innerException2 == null)
				{
					throw;
				}
				if (!(innerException2.GetType() == typeof(FaultException<BindingRedirectionException>)))
				{
					this.AppendToLog(false, 1, ex.Message, base.Result.StateAttribute1, "", "");
					throw;
				}
				FaultException<BindingRedirectionException> faultException = (FaultException<BindingRedirectionException>)innerException2;
				array = faultException.Detail.Locations;
				this.AppendToLog(true, 2, string.Format("Redirect ProvisioningUrl from: {0} to: {1}.", text2, array[0]), "HandleBindingRedirectionException to GetSubscription", "", "");
				WTFDiagnostics.TraceInformation(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "Binding redirection. Changing provisioningApiUrl private client binding to " + array[0], null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ForwardSyncCompanyProbe.cs", 480);
				subscriptions = this.GetSubscription(array[0], this.adminLiveId, accountPassword);
			}
			try
			{
				this.InvokeCompanyManagerProxy(text, certSubject, subscriptions, guid, certThumbprint);
			}
			catch (Exception ex3)
			{
				Exception innerException3 = ex3.InnerException;
				if (innerException3 == null)
				{
					throw;
				}
				TargetInvocationException ex4 = innerException3 as TargetInvocationException;
				if (ex4 == null)
				{
					throw;
				}
				Exception innerException4 = ex4.InnerException;
				if (innerException4 == null)
				{
					throw;
				}
				if (!(innerException4.GetType() == typeof(FaultException<BindingRedirectionFault>)))
				{
					this.AppendToLog(false, 1, ex3.Message, base.Result.StateAttribute1, "", "");
					throw;
				}
				FaultException<BindingRedirectionFault> faultException2 = (FaultException<BindingRedirectionFault>)innerException4;
				string location = faultException2.Detail.Location;
				this.AppendToLog(true, 2, string.Format("Redirect CompanyManagerUrl from: {0} to: {1}.", text, location), "HandleBindingRedirectionFault", "", "");
				WTFDiagnostics.TraceInformation(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "Binding redirection. Changing companyManagerUrl private client binding to " + location, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ForwardSyncCompanyProbe.cs", 520);
				this.InvokeCompanyManagerProxy(location, certSubject, subscriptions, guid, certThumbprint);
			}
		}

		private void InvokeCompanyManagerProxy(string companyManagerUrl, string certSubject, Microsoft.Online.Administration.Subscription[] subscriptions, Guid contextId, string certThumbprint = "")
		{
			StringBuilder stringBuilder = new StringBuilder();
			using (CompanyManagerFederatedServiceTestClient companyManagerFederatedServiceTestClient = this.CreateCompanyManagerFederatedServiceTestClient(companyManagerUrl, certSubject, certThumbprint))
			{
				if (subscriptions.Length == 1)
				{
					DateTime value = subscriptions[0].DateCreated.Value;
					Guid value2 = subscriptions[0].OcpSubscriptionId.Value;
					string skuPartNumber = subscriptions[0].SkuPartNumber;
					this.RemoveSubscription(contextId, value2, skuPartNumber, companyManagerFederatedServiceTestClient);
					base.Result.StateAttribute1 = "RemovedSubscription";
					if (this.testOrgCU != null)
					{
						if (this.testOrgCU.OrganizationStatus == OrganizationStatus.Active)
						{
							DateTime dateTime = this.testOrgCU.WhenOrganizationStatusSet.Value.ToUniversalTime();
							int num = (int)(dateTime - value).TotalSeconds;
							base.Result.SampleValue = (double)num;
							if (num > ForwardSyncCompanyProbe.companySlaSeconds)
							{
								string message = string.Format("Company {0} with admin {1} and serviceInstance {2} is added {3} subscription at {4} but synced to EXO at {5} which exceed SLA {6} seconds.", new object[]
								{
									contextId,
									this.adminLiveId,
									this.serviceInstance,
									skuPartNumber,
									value,
									dateTime,
									ForwardSyncCompanyProbe.companySlaSeconds
								});
								this.AppendToLog(true, 2, message, base.Result.StateAttribute1, this.testOrgCU.OrganizationStatus.ToString(), "");
								WTFDiagnostics.TraceError(ExTraceGlobals.ProvisioningTracer, base.TraceContext, message, null, "InvokeCompanyManagerProxy", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ForwardSyncCompanyProbe.cs", 579);
							}
							else
							{
								this.AppendToLog(true, 0, null, base.Result.StateAttribute1, "", this.testOrgCU.Name);
							}
						}
						else
						{
							stringBuilder.Append(string.Format("Company {0} with admin {1} and ServiceInstance {2} is added {3} subscription at {4} but not Active in EXO at {5}.", new object[]
							{
								contextId,
								this.adminLiveId,
								this.serviceInstance,
								skuPartNumber,
								value,
								DateTime.UtcNow
							}));
							this.AppendToLog(false, 1, stringBuilder.ToString(), base.Result.StateAttribute1, this.testOrgCU.OrganizationStatus.ToString(), "");
						}
						this.RemoveTestOrganization(this.testOrgCU);
					}
					else
					{
						stringBuilder.Append(string.Format("Company {0} with admin {1} and serviceInstance {2} is added {3} subscription at {4} but not synced to EXO at {5}.", new object[]
						{
							contextId,
							this.adminLiveId,
							this.serviceInstance,
							skuPartNumber,
							value,
							DateTime.UtcNow
						}));
						this.AppendToLog(false, 1, stringBuilder.ToString(), base.Result.StateAttribute1, "", "");
					}
				}
				else if (subscriptions.Length == 0)
				{
					string organizationName = "";
					if (this.testOrgCU != null)
					{
						organizationName = this.testOrgCU.Name;
						this.RemoveTestOrganization(this.testOrgCU);
						base.Result.StateAttribute1 = "RemoveTestOrganization";
						this.AppendToLog(true, 2, null, base.Result.StateAttribute1, "", organizationName);
					}
					this.AddSubscription(contextId, this.serviceInstance, this.partNumber, companyManagerFederatedServiceTestClient);
					base.Result.StateAttribute1 = "AddedSubscription";
					this.AppendToLog(true, 0, null, base.Result.StateAttribute1, "", organizationName);
				}
				else
				{
					foreach (Microsoft.Online.Administration.Subscription subscription in subscriptions)
					{
						Guid value3 = subscription.OcpSubscriptionId.Value;
						string skuPartNumber2 = subscription.SkuPartNumber;
						this.RemoveSubscription(contextId, value3, skuPartNumber2, companyManagerFederatedServiceTestClient);
					}
					base.Result.StateAttribute1 = "RemovedMultipleSubscriptions";
					this.AppendToLog(true, 2, string.Format("There are more than one subscription for context ID: {0}.", contextId), base.Result.StateAttribute1, "", "");
				}
			}
			if (stringBuilder.Length > 0)
			{
				throw new Exception(stringBuilder.ToString());
			}
		}

		private void AppendToLog(bool isProbeSucceed, int statusCode, string message, string action, string organizationStatus = "", string organizationName = "")
		{
			bool flag = Convert.ToBoolean(base.Definition.ExtensionAttributes);
			if (flag)
			{
				string hostName = Dns.GetHostName();
				string endpoint = base.Definition.Endpoint;
				StxLoggerBase.GetLoggerInstance(StxLogType.TestForwardSyncCompanyProbe).BeginAppend(hostName, isProbeSucceed, new TimeSpan(0L), statusCode, message, endpoint, "escalate", action, null, new List<string>
				{
					organizationStatus,
					organizationName
				});
			}
		}

		public void RemoveTestOrganizationFromAd(ExchangeConfigurationUnit testOrgCU)
		{
			if (testOrgCU == null)
			{
				base.Result.StateAttribute2 = "testOrgCU is already null in RemoveTestOrganizationFromAd. Return";
				return;
			}
			if (testOrgCU.OrganizationalUnitRoot != null)
			{
				string text = testOrgCU.OrganizationalUnitRoot.DistinguishedName;
				base.Result.StateAttribute2 = "TestOrg OU " + text;
				if (string.IsNullOrEmpty(text))
				{
					text = testOrgCU.OrganizationalUnitRoot.ToString();
				}
				string[] array = text.Split(new char[]
				{
					'/'
				});
				StringBuilder stringBuilder = new StringBuilder();
				if (array.Count<string>() == 3)
				{
					stringBuilder.Append("OU=" + array[2] + ",OU=" + array[1]);
					string[] array2 = array[0].Split(new char[]
					{
						'.'
					});
					foreach (string str in array2)
					{
						stringBuilder.Append(",DC=" + str);
					}
					text = stringBuilder.ToString();
					base.Result.StateAttribute3 = "Modified TestOrg OU " + text;
				}
				this.RemoveTestOrganizationUseLdap(text);
			}
			else
			{
				base.Result.StateAttribute2 = "testOrgCU.OrganizationalUnitRoot is null in RemoveTestOrganizationFromAd.";
			}
			if (testOrgCU.DistinguishedName != null)
			{
				string text2 = testOrgCU.DistinguishedName;
				base.Result.StateAttribute4 = "TestOrg CU " + text2;
				if (text2.Contains("CN=Configuration,"))
				{
					text2 = text2.Substring(text2.IndexOf("CN=Configuration,", StringComparison.OrdinalIgnoreCase) + "CN=Configuration,".Length);
				}
				base.Result.StateAttribute5 = "Modified TestOrg CU " + text2;
				this.RemoveTestOrganizationUseLdap(text2);
				return;
			}
			base.Result.StateAttribute4 = "testOrgCU.DistinguishedName is null in RemoveTestOrganizationFromAd.";
		}

		public void RemoveTestOrganizationUseLdap(string ldapPath)
		{
			if (string.IsNullOrEmpty(ldapPath))
			{
				base.Result.StateAttribute5 = "ForwardSyncCompanyProbe.RemoveTestOrganizationUseLdap(): LDAP path is empty";
				this.AppendToLog(true, 0, " ForwardSyncCompanyProbe.RemoveTestOrganizationUseLdap(): LDAP path is empty", "Tree", "", ldapPath);
				return;
			}
			if (!ldapPath.StartsWith("LDAP://", StringComparison.OrdinalIgnoreCase))
			{
				ldapPath = "LDAP://" + ldapPath;
			}
			this.AppendToLog(true, 0, " Will remove CU from AD", "DeleteTree", "", ldapPath);
			DirectoryEntry directoryEntry = null;
			try
			{
				directoryEntry = new DirectoryEntry(ldapPath);
				directoryEntry.DeleteTree();
				directoryEntry.CommitChanges();
				this.AppendToLog(true, 0, "The action succeeded", "DeleteTree", "", ldapPath);
			}
			catch (Exception ex)
			{
				WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.ProvisioningTracer, base.TraceContext, "ForwardSyncCompanyProbe.RemoveTestOrganizationUseLdap(): for ldap {0} got exception {1}", ldapPath, ex.ToString(), null, "RemoveTestOrganizationUseLdap", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Provisioning\\ForwardSyncCompanyProbe.cs", 798);
				base.Result.StateAttribute5 = string.Format("ForwardSyncCompanyProbe.RemoveTestOrganizationUseLdap(): for ldap {0} got exception {1}", ldapPath, ex);
				this.AppendToLog(false, 1, ex.ToString(), "DeleteTree", "", ldapPath);
			}
			finally
			{
				if (directoryEntry != null)
				{
					directoryEntry.Dispose();
				}
			}
		}

		private const int SleepingTime = 120000;

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(ForwardSyncCompanyProbe).FullName;

		private static int companySlaSeconds = 300;

		private ExchangeConfigurationUnit testOrgCU;

		private string adminLiveId = string.Empty;

		private string serviceInstance = string.Empty;

		private string partNumber = string.Empty;
	}
}
