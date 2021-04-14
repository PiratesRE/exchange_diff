using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[XmlInclude(typeof(DirectorySearch))]
	[WebServiceBinding(Name = "CompanyManagerProviderSoap", Namespace = "http://www.ccs.com/TestServices/")]
	[XmlInclude(typeof(DirectoryReference))]
	[XmlInclude(typeof(DirectoryProperty))]
	[DesignerCategory("code")]
	[XmlInclude(typeof(CompanyDomainValue))]
	[XmlInclude(typeof(DirectoryObject))]
	[DebuggerStepThrough]
	public class CompanyManagerProvider : SoapHttpClientProtocol
	{
		internal CompanyManagerProvider()
		{
			base.Url = "https://remotetestservice.msol-test.com/CompanyManagerProvider.asmx";
		}

		public event IsDomainAvailableCompletedEventHandler IsDomainAvailableCompleted;

		public event CreateCompanyCompletedEventHandler CreateCompanyCompleted;

		public event CreateSyndicatedCompanyCompletedEventHandler CreateSyndicatedCompanyCompleted;

		public event SetCompanyPartnershipCompletedEventHandler SetCompanyPartnershipCompleted;

		public event UpdateCompanyProfileCompletedEventHandler UpdateCompanyProfileCompleted;

		public event UpdateCompanyTagsCompletedEventHandler UpdateCompanyTagsCompleted;

		public event DeleteCompanyCompletedEventHandler DeleteCompanyCompleted;

		public event ForceDeleteCompanyCompletedEventHandler ForceDeleteCompanyCompleted;

		public event CreateAccountCompletedEventHandler CreateAccountCompleted;

		public event RenameAccountCompletedEventHandler RenameAccountCompleted;

		public event DeleteAccountCompletedEventHandler DeleteAccountCompleted;

		public event CreateUpdateDeleteSubscriptionCompletedEventHandler CreateUpdateDeleteSubscriptionCompleted;

		public event CreateCompanyWithSubscriptionsCompletedEventHandler CreateCompanyWithSubscriptionsCompleted;

		public event SignupCompletedEventHandler SignupCompleted;

		public event SignupWithCompanyTagsCompletedEventHandler SignupWithCompanyTagsCompleted;

		public event PromoteToPartnerCompletedEventHandler PromoteToPartnerCompleted;

		public event DemoteToCompanyCompletedEventHandler DemoteToCompanyCompleted;

		public event ForceDemoteToCompanyCompletedEventHandler ForceDemoteToCompanyCompleted;

		public event AddServiceTypeCompletedEventHandler AddServiceTypeCompleted;

		public event RemoveServiceTypeCompletedEventHandler RemoveServiceTypeCompleted;

		public event ListServicesForPartnershipCompletedEventHandler ListServicesForPartnershipCompleted;

		public event AssociateToPartnerCompletedEventHandler AssociateToPartnerCompleted;

		public event DeletePartnerContractCompletedEventHandler DeletePartnerContractCompleted;

		public event CreatePartnerCompletedEventHandler CreatePartnerCompleted;

		public event CreateMailboxAgentsGroupCompletedEventHandler CreateMailboxAgentsGroupCompleted;

		public event GetCompanyContextIdCompletedEventHandler GetCompanyContextIdCompleted;

		public event GetPartitionIdCompletedEventHandler GetPartitionIdCompleted;

		public event GetPartNumberFromSkuIdCompletedEventHandler GetPartNumberFromSkuIdCompleted;

		public event GetSkuIdFromPartNumberCompletedEventHandler GetSkuIdFromPartNumberCompleted;

		public event GetAccountSubscriptionsCompletedEventHandler GetAccountSubscriptionsCompleted;

		public event GetCompanyAccountsCompletedEventHandler GetCompanyAccountsCompleted;

		public event GetCompanyAssignedPlansCompletedEventHandler GetCompanyAssignedPlansCompleted;

		public event GetCompanyProvisionedPlansCompletedEventHandler GetCompanyProvisionedPlansCompleted;

		public event GetCompanySubscriptionsCompletedEventHandler GetCompanySubscriptionsCompleted;

		public event GetCompanyForeignPrincipalObjectsCompletedEventHandler GetCompanyForeignPrincipalObjectsCompleted;

		public event SetCompanyProvisioningStatusCompletedEventHandler SetCompanyProvisioningStatusCompleted;

		public event ForceRefreshSystemDataCompletedEventHandler ForceRefreshSystemDataCompleted;

		public event BuildRandomAccountCompletedEventHandler BuildRandomAccountCompleted;

		public event BuildRandomCompanyCompletedEventHandler BuildRandomCompanyCompleted;

		public event BuildRandomCompanyProfileCompletedEventHandler BuildRandomCompanyProfileCompleted;

		public event BuildRandomUserCompletedEventHandler BuildRandomUserCompleted;

		public event BuildRandomSubscriptionCompletedEventHandler BuildRandomSubscriptionCompleted;

		public event GetDefaultContractRoleMapCompletedEventHandler GetDefaultContractRoleMapCompleted;

		public event ForceTransitiveReplicationCompletedEventHandler ForceTransitiveReplicationCompleted;

		[SoapDocumentMethod("http://www.ccs.com/TestServices/IsDomainAvailable", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public bool IsDomainAvailable(string domainPrefix, string domainSuffix, Guid trackingId)
		{
			object[] array = base.Invoke("IsDomainAvailable", new object[]
			{
				domainPrefix,
				domainSuffix,
				trackingId
			});
			return (bool)array[0];
		}

		public IAsyncResult BeginIsDomainAvailable(string domainPrefix, string domainSuffix, Guid trackingId, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("IsDomainAvailable", new object[]
			{
				domainPrefix,
				domainSuffix,
				trackingId
			}, callback, asyncState);
		}

		public bool EndIsDomainAvailable(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (bool)array[0];
		}

		public void IsDomainAvailableAsync(string domainPrefix, string domainSuffix, Guid trackingId)
		{
			this.IsDomainAvailableAsync(domainPrefix, domainSuffix, trackingId, null);
		}

		public void IsDomainAvailableAsync(string domainPrefix, string domainSuffix, Guid trackingId, object userState)
		{
			if (this.IsDomainAvailableOperationCompleted == null)
			{
				this.IsDomainAvailableOperationCompleted = new SendOrPostCallback(this.OnIsDomainAvailableOperationCompleted);
			}
			base.InvokeAsync("IsDomainAvailable", new object[]
			{
				domainPrefix,
				domainSuffix,
				trackingId
			}, this.IsDomainAvailableOperationCompleted, userState);
		}

		private void OnIsDomainAvailableOperationCompleted(object arg)
		{
			if (this.IsDomainAvailableCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.IsDomainAvailableCompleted(this, new IsDomainAvailableCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/CreateCompany", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ProvisionInfo CreateCompany(Company company, User user, Account account, Guid trackingId)
		{
			object[] array = base.Invoke("CreateCompany", new object[]
			{
				company,
				user,
				account,
				trackingId
			});
			return (ProvisionInfo)array[0];
		}

		public IAsyncResult BeginCreateCompany(Company company, User user, Account account, Guid trackingId, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("CreateCompany", new object[]
			{
				company,
				user,
				account,
				trackingId
			}, callback, asyncState);
		}

		public ProvisionInfo EndCreateCompany(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (ProvisionInfo)array[0];
		}

		public void CreateCompanyAsync(Company company, User user, Account account, Guid trackingId)
		{
			this.CreateCompanyAsync(company, user, account, trackingId, null);
		}

		public void CreateCompanyAsync(Company company, User user, Account account, Guid trackingId, object userState)
		{
			if (this.CreateCompanyOperationCompleted == null)
			{
				this.CreateCompanyOperationCompleted = new SendOrPostCallback(this.OnCreateCompanyOperationCompleted);
			}
			base.InvokeAsync("CreateCompany", new object[]
			{
				company,
				user,
				account,
				trackingId
			}, this.CreateCompanyOperationCompleted, userState);
		}

		private void OnCreateCompanyOperationCompleted(object arg)
		{
			if (this.CreateCompanyCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CreateCompanyCompleted(this, new CreateCompanyCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/CreateSyndicatedCompany", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ProvisionInfo CreateSyndicatedCompany(Company company, User user, Account account, Guid trackingId, PartnershipValue[] partnerships)
		{
			object[] array = base.Invoke("CreateSyndicatedCompany", new object[]
			{
				company,
				user,
				account,
				trackingId,
				partnerships
			});
			return (ProvisionInfo)array[0];
		}

		public IAsyncResult BeginCreateSyndicatedCompany(Company company, User user, Account account, Guid trackingId, PartnershipValue[] partnerships, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("CreateSyndicatedCompany", new object[]
			{
				company,
				user,
				account,
				trackingId,
				partnerships
			}, callback, asyncState);
		}

		public ProvisionInfo EndCreateSyndicatedCompany(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (ProvisionInfo)array[0];
		}

		public void CreateSyndicatedCompanyAsync(Company company, User user, Account account, Guid trackingId, PartnershipValue[] partnerships)
		{
			this.CreateSyndicatedCompanyAsync(company, user, account, trackingId, partnerships, null);
		}

		public void CreateSyndicatedCompanyAsync(Company company, User user, Account account, Guid trackingId, PartnershipValue[] partnerships, object userState)
		{
			if (this.CreateSyndicatedCompanyOperationCompleted == null)
			{
				this.CreateSyndicatedCompanyOperationCompleted = new SendOrPostCallback(this.OnCreateSyndicatedCompanyOperationCompleted);
			}
			base.InvokeAsync("CreateSyndicatedCompany", new object[]
			{
				company,
				user,
				account,
				trackingId,
				partnerships
			}, this.CreateSyndicatedCompanyOperationCompleted, userState);
		}

		private void OnCreateSyndicatedCompanyOperationCompleted(object arg)
		{
			if (this.CreateSyndicatedCompanyCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CreateSyndicatedCompanyCompleted(this, new CreateSyndicatedCompanyCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/SetCompanyPartnership", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void SetCompanyPartnership(Guid contextId, PartnershipValue[] partnerships)
		{
			base.Invoke("SetCompanyPartnership", new object[]
			{
				contextId,
				partnerships
			});
		}

		public IAsyncResult BeginSetCompanyPartnership(Guid contextId, PartnershipValue[] partnerships, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("SetCompanyPartnership", new object[]
			{
				contextId,
				partnerships
			}, callback, asyncState);
		}

		public void EndSetCompanyPartnership(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void SetCompanyPartnershipAsync(Guid contextId, PartnershipValue[] partnerships)
		{
			this.SetCompanyPartnershipAsync(contextId, partnerships, null);
		}

		public void SetCompanyPartnershipAsync(Guid contextId, PartnershipValue[] partnerships, object userState)
		{
			if (this.SetCompanyPartnershipOperationCompleted == null)
			{
				this.SetCompanyPartnershipOperationCompleted = new SendOrPostCallback(this.OnSetCompanyPartnershipOperationCompleted);
			}
			base.InvokeAsync("SetCompanyPartnership", new object[]
			{
				contextId,
				partnerships
			}, this.SetCompanyPartnershipOperationCompleted, userState);
		}

		private void OnSetCompanyPartnershipOperationCompleted(object arg)
		{
			if (this.SetCompanyPartnershipCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SetCompanyPartnershipCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/UpdateCompanyProfile", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void UpdateCompanyProfile(Guid contextId, CompanyProfile companyProfile)
		{
			base.Invoke("UpdateCompanyProfile", new object[]
			{
				contextId,
				companyProfile
			});
		}

		public IAsyncResult BeginUpdateCompanyProfile(Guid contextId, CompanyProfile companyProfile, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("UpdateCompanyProfile", new object[]
			{
				contextId,
				companyProfile
			}, callback, asyncState);
		}

		public void EndUpdateCompanyProfile(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void UpdateCompanyProfileAsync(Guid contextId, CompanyProfile companyProfile)
		{
			this.UpdateCompanyProfileAsync(contextId, companyProfile, null);
		}

		public void UpdateCompanyProfileAsync(Guid contextId, CompanyProfile companyProfile, object userState)
		{
			if (this.UpdateCompanyProfileOperationCompleted == null)
			{
				this.UpdateCompanyProfileOperationCompleted = new SendOrPostCallback(this.OnUpdateCompanyProfileOperationCompleted);
			}
			base.InvokeAsync("UpdateCompanyProfile", new object[]
			{
				contextId,
				companyProfile
			}, this.UpdateCompanyProfileOperationCompleted, userState);
		}

		private void OnUpdateCompanyProfileOperationCompleted(object arg)
		{
			if (this.UpdateCompanyProfileCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.UpdateCompanyProfileCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/UpdateCompanyTags", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void UpdateCompanyTags(Guid contextId, string[] companyTags)
		{
			base.Invoke("UpdateCompanyTags", new object[]
			{
				contextId,
				companyTags
			});
		}

		public IAsyncResult BeginUpdateCompanyTags(Guid contextId, string[] companyTags, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("UpdateCompanyTags", new object[]
			{
				contextId,
				companyTags
			}, callback, asyncState);
		}

		public void EndUpdateCompanyTags(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void UpdateCompanyTagsAsync(Guid contextId, string[] companyTags)
		{
			this.UpdateCompanyTagsAsync(contextId, companyTags, null);
		}

		public void UpdateCompanyTagsAsync(Guid contextId, string[] companyTags, object userState)
		{
			if (this.UpdateCompanyTagsOperationCompleted == null)
			{
				this.UpdateCompanyTagsOperationCompleted = new SendOrPostCallback(this.OnUpdateCompanyTagsOperationCompleted);
			}
			base.InvokeAsync("UpdateCompanyTags", new object[]
			{
				contextId,
				companyTags
			}, this.UpdateCompanyTagsOperationCompleted, userState);
		}

		private void OnUpdateCompanyTagsOperationCompleted(object arg)
		{
			if (this.UpdateCompanyTagsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.UpdateCompanyTagsCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/DeleteCompany", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void DeleteCompany(Guid contextId)
		{
			base.Invoke("DeleteCompany", new object[]
			{
				contextId
			});
		}

		public IAsyncResult BeginDeleteCompany(Guid contextId, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("DeleteCompany", new object[]
			{
				contextId
			}, callback, asyncState);
		}

		public void EndDeleteCompany(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void DeleteCompanyAsync(Guid contextId)
		{
			this.DeleteCompanyAsync(contextId, null);
		}

		public void DeleteCompanyAsync(Guid contextId, object userState)
		{
			if (this.DeleteCompanyOperationCompleted == null)
			{
				this.DeleteCompanyOperationCompleted = new SendOrPostCallback(this.OnDeleteCompanyOperationCompleted);
			}
			base.InvokeAsync("DeleteCompany", new object[]
			{
				contextId
			}, this.DeleteCompanyOperationCompleted, userState);
		}

		private void OnDeleteCompanyOperationCompleted(object arg)
		{
			if (this.DeleteCompanyCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.DeleteCompanyCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/ForceDeleteCompany", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void ForceDeleteCompany(Guid contextId)
		{
			base.Invoke("ForceDeleteCompany", new object[]
			{
				contextId
			});
		}

		public IAsyncResult BeginForceDeleteCompany(Guid contextId, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("ForceDeleteCompany", new object[]
			{
				contextId
			}, callback, asyncState);
		}

		public void EndForceDeleteCompany(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void ForceDeleteCompanyAsync(Guid contextId)
		{
			this.ForceDeleteCompanyAsync(contextId, null);
		}

		public void ForceDeleteCompanyAsync(Guid contextId, object userState)
		{
			if (this.ForceDeleteCompanyOperationCompleted == null)
			{
				this.ForceDeleteCompanyOperationCompleted = new SendOrPostCallback(this.OnForceDeleteCompanyOperationCompleted);
			}
			base.InvokeAsync("ForceDeleteCompany", new object[]
			{
				contextId
			}, this.ForceDeleteCompanyOperationCompleted, userState);
		}

		private void OnForceDeleteCompanyOperationCompleted(object arg)
		{
			if (this.ForceDeleteCompanyCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ForceDeleteCompanyCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/CreateAccount", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void CreateAccount(Guid contextId, Account account)
		{
			base.Invoke("CreateAccount", new object[]
			{
				contextId,
				account
			});
		}

		public IAsyncResult BeginCreateAccount(Guid contextId, Account account, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("CreateAccount", new object[]
			{
				contextId,
				account
			}, callback, asyncState);
		}

		public void EndCreateAccount(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void CreateAccountAsync(Guid contextId, Account account)
		{
			this.CreateAccountAsync(contextId, account, null);
		}

		public void CreateAccountAsync(Guid contextId, Account account, object userState)
		{
			if (this.CreateAccountOperationCompleted == null)
			{
				this.CreateAccountOperationCompleted = new SendOrPostCallback(this.OnCreateAccountOperationCompleted);
			}
			base.InvokeAsync("CreateAccount", new object[]
			{
				contextId,
				account
			}, this.CreateAccountOperationCompleted, userState);
		}

		private void OnCreateAccountOperationCompleted(object arg)
		{
			if (this.CreateAccountCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CreateAccountCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/RenameAccount", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void RenameAccount(Guid contextId, Account account)
		{
			base.Invoke("RenameAccount", new object[]
			{
				contextId,
				account
			});
		}

		public IAsyncResult BeginRenameAccount(Guid contextId, Account account, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("RenameAccount", new object[]
			{
				contextId,
				account
			}, callback, asyncState);
		}

		public void EndRenameAccount(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void RenameAccountAsync(Guid contextId, Account account)
		{
			this.RenameAccountAsync(contextId, account, null);
		}

		public void RenameAccountAsync(Guid contextId, Account account, object userState)
		{
			if (this.RenameAccountOperationCompleted == null)
			{
				this.RenameAccountOperationCompleted = new SendOrPostCallback(this.OnRenameAccountOperationCompleted);
			}
			base.InvokeAsync("RenameAccount", new object[]
			{
				contextId,
				account
			}, this.RenameAccountOperationCompleted, userState);
		}

		private void OnRenameAccountOperationCompleted(object arg)
		{
			if (this.RenameAccountCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.RenameAccountCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/DeleteAccount", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void DeleteAccount(Guid contextId, Guid accountId)
		{
			base.Invoke("DeleteAccount", new object[]
			{
				contextId,
				accountId
			});
		}

		public IAsyncResult BeginDeleteAccount(Guid contextId, Guid accountId, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("DeleteAccount", new object[]
			{
				contextId,
				accountId
			}, callback, asyncState);
		}

		public void EndDeleteAccount(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void DeleteAccountAsync(Guid contextId, Guid accountId)
		{
			this.DeleteAccountAsync(contextId, accountId, null);
		}

		public void DeleteAccountAsync(Guid contextId, Guid accountId, object userState)
		{
			if (this.DeleteAccountOperationCompleted == null)
			{
				this.DeleteAccountOperationCompleted = new SendOrPostCallback(this.OnDeleteAccountOperationCompleted);
			}
			base.InvokeAsync("DeleteAccount", new object[]
			{
				contextId,
				accountId
			}, this.DeleteAccountOperationCompleted, userState);
		}

		private void OnDeleteAccountOperationCompleted(object arg)
		{
			if (this.DeleteAccountCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.DeleteAccountCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/CreateUpdateDeleteSubscription", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void CreateUpdateDeleteSubscription(Subscription subscription)
		{
			base.Invoke("CreateUpdateDeleteSubscription", new object[]
			{
				subscription
			});
		}

		public IAsyncResult BeginCreateUpdateDeleteSubscription(Subscription subscription, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("CreateUpdateDeleteSubscription", new object[]
			{
				subscription
			}, callback, asyncState);
		}

		public void EndCreateUpdateDeleteSubscription(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void CreateUpdateDeleteSubscriptionAsync(Subscription subscription)
		{
			this.CreateUpdateDeleteSubscriptionAsync(subscription, null);
		}

		public void CreateUpdateDeleteSubscriptionAsync(Subscription subscription, object userState)
		{
			if (this.CreateUpdateDeleteSubscriptionOperationCompleted == null)
			{
				this.CreateUpdateDeleteSubscriptionOperationCompleted = new SendOrPostCallback(this.OnCreateUpdateDeleteSubscriptionOperationCompleted);
			}
			base.InvokeAsync("CreateUpdateDeleteSubscription", new object[]
			{
				subscription
			}, this.CreateUpdateDeleteSubscriptionOperationCompleted, userState);
		}

		private void OnCreateUpdateDeleteSubscriptionOperationCompleted(object arg)
		{
			if (this.CreateUpdateDeleteSubscriptionCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CreateUpdateDeleteSubscriptionCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/CreateCompanyWithSubscriptions", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ProvisionInfo CreateCompanyWithSubscriptions(Company company, User user, Account account, Subscription[] subscriptions)
		{
			object[] array = base.Invoke("CreateCompanyWithSubscriptions", new object[]
			{
				company,
				user,
				account,
				subscriptions
			});
			return (ProvisionInfo)array[0];
		}

		public IAsyncResult BeginCreateCompanyWithSubscriptions(Company company, User user, Account account, Subscription[] subscriptions, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("CreateCompanyWithSubscriptions", new object[]
			{
				company,
				user,
				account,
				subscriptions
			}, callback, asyncState);
		}

		public ProvisionInfo EndCreateCompanyWithSubscriptions(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (ProvisionInfo)array[0];
		}

		public void CreateCompanyWithSubscriptionsAsync(Company company, User user, Account account, Subscription[] subscriptions)
		{
			this.CreateCompanyWithSubscriptionsAsync(company, user, account, subscriptions, null);
		}

		public void CreateCompanyWithSubscriptionsAsync(Company company, User user, Account account, Subscription[] subscriptions, object userState)
		{
			if (this.CreateCompanyWithSubscriptionsOperationCompleted == null)
			{
				this.CreateCompanyWithSubscriptionsOperationCompleted = new SendOrPostCallback(this.OnCreateCompanyWithSubscriptionsOperationCompleted);
			}
			base.InvokeAsync("CreateCompanyWithSubscriptions", new object[]
			{
				company,
				user,
				account,
				subscriptions
			}, this.CreateCompanyWithSubscriptionsOperationCompleted, userState);
		}

		private void OnCreateCompanyWithSubscriptionsOperationCompleted(object arg)
		{
			if (this.CreateCompanyWithSubscriptionsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CreateCompanyWithSubscriptionsCompleted(this, new CreateCompanyWithSubscriptionsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/Signup", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ProvisionInfo Signup(Company company, User user, Account account, Subscription subscription, Guid trackingId)
		{
			object[] array = base.Invoke("Signup", new object[]
			{
				company,
				user,
				account,
				subscription,
				trackingId
			});
			return (ProvisionInfo)array[0];
		}

		public IAsyncResult BeginSignup(Company company, User user, Account account, Subscription subscription, Guid trackingId, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("Signup", new object[]
			{
				company,
				user,
				account,
				subscription,
				trackingId
			}, callback, asyncState);
		}

		public ProvisionInfo EndSignup(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (ProvisionInfo)array[0];
		}

		public void SignupAsync(Company company, User user, Account account, Subscription subscription, Guid trackingId)
		{
			this.SignupAsync(company, user, account, subscription, trackingId, null);
		}

		public void SignupAsync(Company company, User user, Account account, Subscription subscription, Guid trackingId, object userState)
		{
			if (this.SignupOperationCompleted == null)
			{
				this.SignupOperationCompleted = new SendOrPostCallback(this.OnSignupOperationCompleted);
			}
			base.InvokeAsync("Signup", new object[]
			{
				company,
				user,
				account,
				subscription,
				trackingId
			}, this.SignupOperationCompleted, userState);
		}

		private void OnSignupOperationCompleted(object arg)
		{
			if (this.SignupCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SignupCompleted(this, new SignupCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/SignupWithCompanyTags", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ProvisionInfo SignupWithCompanyTags(Company company, string[] companyTags, User user, Account account, Subscription subscription, Guid trackingId)
		{
			object[] array = base.Invoke("SignupWithCompanyTags", new object[]
			{
				company,
				companyTags,
				user,
				account,
				subscription,
				trackingId
			});
			return (ProvisionInfo)array[0];
		}

		public IAsyncResult BeginSignupWithCompanyTags(Company company, string[] companyTags, User user, Account account, Subscription subscription, Guid trackingId, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("SignupWithCompanyTags", new object[]
			{
				company,
				companyTags,
				user,
				account,
				subscription,
				trackingId
			}, callback, asyncState);
		}

		public ProvisionInfo EndSignupWithCompanyTags(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (ProvisionInfo)array[0];
		}

		public void SignupWithCompanyTagsAsync(Company company, string[] companyTags, User user, Account account, Subscription subscription, Guid trackingId)
		{
			this.SignupWithCompanyTagsAsync(company, companyTags, user, account, subscription, trackingId, null);
		}

		public void SignupWithCompanyTagsAsync(Company company, string[] companyTags, User user, Account account, Subscription subscription, Guid trackingId, object userState)
		{
			if (this.SignupWithCompanyTagsOperationCompleted == null)
			{
				this.SignupWithCompanyTagsOperationCompleted = new SendOrPostCallback(this.OnSignupWithCompanyTagsOperationCompleted);
			}
			base.InvokeAsync("SignupWithCompanyTags", new object[]
			{
				company,
				companyTags,
				user,
				account,
				subscription,
				trackingId
			}, this.SignupWithCompanyTagsOperationCompleted, userState);
		}

		private void OnSignupWithCompanyTagsOperationCompleted(object arg)
		{
			if (this.SignupWithCompanyTagsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SignupWithCompanyTagsCompleted(this, new SignupWithCompanyTagsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/PromoteToPartner", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void PromoteToPartner(Guid partnerContextId, string[] serviceTypes, PartnerType partnerType)
		{
			base.Invoke("PromoteToPartner", new object[]
			{
				partnerContextId,
				serviceTypes,
				partnerType
			});
		}

		public IAsyncResult BeginPromoteToPartner(Guid partnerContextId, string[] serviceTypes, PartnerType partnerType, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("PromoteToPartner", new object[]
			{
				partnerContextId,
				serviceTypes,
				partnerType
			}, callback, asyncState);
		}

		public void EndPromoteToPartner(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void PromoteToPartnerAsync(Guid partnerContextId, string[] serviceTypes, PartnerType partnerType)
		{
			this.PromoteToPartnerAsync(partnerContextId, serviceTypes, partnerType, null);
		}

		public void PromoteToPartnerAsync(Guid partnerContextId, string[] serviceTypes, PartnerType partnerType, object userState)
		{
			if (this.PromoteToPartnerOperationCompleted == null)
			{
				this.PromoteToPartnerOperationCompleted = new SendOrPostCallback(this.OnPromoteToPartnerOperationCompleted);
			}
			base.InvokeAsync("PromoteToPartner", new object[]
			{
				partnerContextId,
				serviceTypes,
				partnerType
			}, this.PromoteToPartnerOperationCompleted, userState);
		}

		private void OnPromoteToPartnerOperationCompleted(object arg)
		{
			if (this.PromoteToPartnerCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.PromoteToPartnerCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/DemoteToCompany", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void DemoteToCompany(Guid partnerContextId)
		{
			base.Invoke("DemoteToCompany", new object[]
			{
				partnerContextId
			});
		}

		public IAsyncResult BeginDemoteToCompany(Guid partnerContextId, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("DemoteToCompany", new object[]
			{
				partnerContextId
			}, callback, asyncState);
		}

		public void EndDemoteToCompany(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void DemoteToCompanyAsync(Guid partnerContextId)
		{
			this.DemoteToCompanyAsync(partnerContextId, null);
		}

		public void DemoteToCompanyAsync(Guid partnerContextId, object userState)
		{
			if (this.DemoteToCompanyOperationCompleted == null)
			{
				this.DemoteToCompanyOperationCompleted = new SendOrPostCallback(this.OnDemoteToCompanyOperationCompleted);
			}
			base.InvokeAsync("DemoteToCompany", new object[]
			{
				partnerContextId
			}, this.DemoteToCompanyOperationCompleted, userState);
		}

		private void OnDemoteToCompanyOperationCompleted(object arg)
		{
			if (this.DemoteToCompanyCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.DemoteToCompanyCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/ForceDemoteToCompany", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void ForceDemoteToCompany(Guid partnerContextId)
		{
			base.Invoke("ForceDemoteToCompany", new object[]
			{
				partnerContextId
			});
		}

		public IAsyncResult BeginForceDemoteToCompany(Guid partnerContextId, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("ForceDemoteToCompany", new object[]
			{
				partnerContextId
			}, callback, asyncState);
		}

		public void EndForceDemoteToCompany(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void ForceDemoteToCompanyAsync(Guid partnerContextId)
		{
			this.ForceDemoteToCompanyAsync(partnerContextId, null);
		}

		public void ForceDemoteToCompanyAsync(Guid partnerContextId, object userState)
		{
			if (this.ForceDemoteToCompanyOperationCompleted == null)
			{
				this.ForceDemoteToCompanyOperationCompleted = new SendOrPostCallback(this.OnForceDemoteToCompanyOperationCompleted);
			}
			base.InvokeAsync("ForceDemoteToCompany", new object[]
			{
				partnerContextId
			}, this.ForceDemoteToCompanyOperationCompleted, userState);
		}

		private void OnForceDemoteToCompanyOperationCompleted(object arg)
		{
			if (this.ForceDemoteToCompanyCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ForceDemoteToCompanyCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/AddServiceType", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void AddServiceType(Guid partnerContextId, string serviceType)
		{
			base.Invoke("AddServiceType", new object[]
			{
				partnerContextId,
				serviceType
			});
		}

		public IAsyncResult BeginAddServiceType(Guid partnerContextId, string serviceType, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("AddServiceType", new object[]
			{
				partnerContextId,
				serviceType
			}, callback, asyncState);
		}

		public void EndAddServiceType(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void AddServiceTypeAsync(Guid partnerContextId, string serviceType)
		{
			this.AddServiceTypeAsync(partnerContextId, serviceType, null);
		}

		public void AddServiceTypeAsync(Guid partnerContextId, string serviceType, object userState)
		{
			if (this.AddServiceTypeOperationCompleted == null)
			{
				this.AddServiceTypeOperationCompleted = new SendOrPostCallback(this.OnAddServiceTypeOperationCompleted);
			}
			base.InvokeAsync("AddServiceType", new object[]
			{
				partnerContextId,
				serviceType
			}, this.AddServiceTypeOperationCompleted, userState);
		}

		private void OnAddServiceTypeOperationCompleted(object arg)
		{
			if (this.AddServiceTypeCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.AddServiceTypeCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/RemoveServiceType", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void RemoveServiceType(Guid partnerContextId, string serviceType)
		{
			base.Invoke("RemoveServiceType", new object[]
			{
				partnerContextId,
				serviceType
			});
		}

		public IAsyncResult BeginRemoveServiceType(Guid partnerContextId, string serviceType, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("RemoveServiceType", new object[]
			{
				partnerContextId,
				serviceType
			}, callback, asyncState);
		}

		public void EndRemoveServiceType(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void RemoveServiceTypeAsync(Guid partnerContextId, string serviceType)
		{
			this.RemoveServiceTypeAsync(partnerContextId, serviceType, null);
		}

		public void RemoveServiceTypeAsync(Guid partnerContextId, string serviceType, object userState)
		{
			if (this.RemoveServiceTypeOperationCompleted == null)
			{
				this.RemoveServiceTypeOperationCompleted = new SendOrPostCallback(this.OnRemoveServiceTypeOperationCompleted);
			}
			base.InvokeAsync("RemoveServiceType", new object[]
			{
				partnerContextId,
				serviceType
			}, this.RemoveServiceTypeOperationCompleted, userState);
		}

		private void OnRemoveServiceTypeOperationCompleted(object arg)
		{
			if (this.RemoveServiceTypeCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.RemoveServiceTypeCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/ListServicesForPartnership", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string[] ListServicesForPartnership()
		{
			object[] array = base.Invoke("ListServicesForPartnership", new object[0]);
			return (string[])array[0];
		}

		public IAsyncResult BeginListServicesForPartnership(AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("ListServicesForPartnership", new object[0], callback, asyncState);
		}

		public string[] EndListServicesForPartnership(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (string[])array[0];
		}

		public void ListServicesForPartnershipAsync()
		{
			this.ListServicesForPartnershipAsync(null);
		}

		public void ListServicesForPartnershipAsync(object userState)
		{
			if (this.ListServicesForPartnershipOperationCompleted == null)
			{
				this.ListServicesForPartnershipOperationCompleted = new SendOrPostCallback(this.OnListServicesForPartnershipOperationCompleted);
			}
			base.InvokeAsync("ListServicesForPartnership", new object[0], this.ListServicesForPartnershipOperationCompleted, userState);
		}

		private void OnListServicesForPartnershipOperationCompleted(object arg)
		{
			if (this.ListServicesForPartnershipCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ListServicesForPartnershipCompleted(this, new ListServicesForPartnershipCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/AssociateToPartner", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public Contract AssociateToPartner(Guid companyContextId, Guid partnerContextId, PartnerRoleMap[] roleMaps)
		{
			object[] array = base.Invoke("AssociateToPartner", new object[]
			{
				companyContextId,
				partnerContextId,
				roleMaps
			});
			return (Contract)array[0];
		}

		public IAsyncResult BeginAssociateToPartner(Guid companyContextId, Guid partnerContextId, PartnerRoleMap[] roleMaps, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("AssociateToPartner", new object[]
			{
				companyContextId,
				partnerContextId,
				roleMaps
			}, callback, asyncState);
		}

		public Contract EndAssociateToPartner(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (Contract)array[0];
		}

		public void AssociateToPartnerAsync(Guid companyContextId, Guid partnerContextId, PartnerRoleMap[] roleMaps)
		{
			this.AssociateToPartnerAsync(companyContextId, partnerContextId, roleMaps, null);
		}

		public void AssociateToPartnerAsync(Guid companyContextId, Guid partnerContextId, PartnerRoleMap[] roleMaps, object userState)
		{
			if (this.AssociateToPartnerOperationCompleted == null)
			{
				this.AssociateToPartnerOperationCompleted = new SendOrPostCallback(this.OnAssociateToPartnerOperationCompleted);
			}
			base.InvokeAsync("AssociateToPartner", new object[]
			{
				companyContextId,
				partnerContextId,
				roleMaps
			}, this.AssociateToPartnerOperationCompleted, userState);
		}

		private void OnAssociateToPartnerOperationCompleted(object arg)
		{
			if (this.AssociateToPartnerCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.AssociateToPartnerCompleted(this, new AssociateToPartnerCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/DeletePartnerContract", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void DeletePartnerContract(Guid partnerContextId, Guid contractId)
		{
			base.Invoke("DeletePartnerContract", new object[]
			{
				partnerContextId,
				contractId
			});
		}

		public IAsyncResult BeginDeletePartnerContract(Guid partnerContextId, Guid contractId, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("DeletePartnerContract", new object[]
			{
				partnerContextId,
				contractId
			}, callback, asyncState);
		}

		public void EndDeletePartnerContract(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void DeletePartnerContractAsync(Guid partnerContextId, Guid contractId)
		{
			this.DeletePartnerContractAsync(partnerContextId, contractId, null);
		}

		public void DeletePartnerContractAsync(Guid partnerContextId, Guid contractId, object userState)
		{
			if (this.DeletePartnerContractOperationCompleted == null)
			{
				this.DeletePartnerContractOperationCompleted = new SendOrPostCallback(this.OnDeletePartnerContractOperationCompleted);
			}
			base.InvokeAsync("DeletePartnerContract", new object[]
			{
				partnerContextId,
				contractId
			}, this.DeletePartnerContractOperationCompleted, userState);
		}

		private void OnDeletePartnerContractOperationCompleted(object arg)
		{
			if (this.DeletePartnerContractCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.DeletePartnerContractCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/CreatePartner", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ProvisionInfo CreatePartner(Company company, User user, Account account, string[] serviceTypes, PartnerType partnerType, Subscription[] subscriptions)
		{
			object[] array = base.Invoke("CreatePartner", new object[]
			{
				company,
				user,
				account,
				serviceTypes,
				partnerType,
				subscriptions
			});
			return (ProvisionInfo)array[0];
		}

		public IAsyncResult BeginCreatePartner(Company company, User user, Account account, string[] serviceTypes, PartnerType partnerType, Subscription[] subscriptions, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("CreatePartner", new object[]
			{
				company,
				user,
				account,
				serviceTypes,
				partnerType,
				subscriptions
			}, callback, asyncState);
		}

		public ProvisionInfo EndCreatePartner(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (ProvisionInfo)array[0];
		}

		public void CreatePartnerAsync(Company company, User user, Account account, string[] serviceTypes, PartnerType partnerType, Subscription[] subscriptions)
		{
			this.CreatePartnerAsync(company, user, account, serviceTypes, partnerType, subscriptions, null);
		}

		public void CreatePartnerAsync(Company company, User user, Account account, string[] serviceTypes, PartnerType partnerType, Subscription[] subscriptions, object userState)
		{
			if (this.CreatePartnerOperationCompleted == null)
			{
				this.CreatePartnerOperationCompleted = new SendOrPostCallback(this.OnCreatePartnerOperationCompleted);
			}
			base.InvokeAsync("CreatePartner", new object[]
			{
				company,
				user,
				account,
				serviceTypes,
				partnerType,
				subscriptions
			}, this.CreatePartnerOperationCompleted, userState);
		}

		private void OnCreatePartnerOperationCompleted(object arg)
		{
			if (this.CreatePartnerCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CreatePartnerCompleted(this, new CreatePartnerCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/CreateMailboxAgentsGroup", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void CreateMailboxAgentsGroup(Guid partnerContextId)
		{
			base.Invoke("CreateMailboxAgentsGroup", new object[]
			{
				partnerContextId
			});
		}

		public IAsyncResult BeginCreateMailboxAgentsGroup(Guid partnerContextId, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("CreateMailboxAgentsGroup", new object[]
			{
				partnerContextId
			}, callback, asyncState);
		}

		public void EndCreateMailboxAgentsGroup(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void CreateMailboxAgentsGroupAsync(Guid partnerContextId)
		{
			this.CreateMailboxAgentsGroupAsync(partnerContextId, null);
		}

		public void CreateMailboxAgentsGroupAsync(Guid partnerContextId, object userState)
		{
			if (this.CreateMailboxAgentsGroupOperationCompleted == null)
			{
				this.CreateMailboxAgentsGroupOperationCompleted = new SendOrPostCallback(this.OnCreateMailboxAgentsGroupOperationCompleted);
			}
			base.InvokeAsync("CreateMailboxAgentsGroup", new object[]
			{
				partnerContextId
			}, this.CreateMailboxAgentsGroupOperationCompleted, userState);
		}

		private void OnCreateMailboxAgentsGroupOperationCompleted(object arg)
		{
			if (this.CreateMailboxAgentsGroupCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CreateMailboxAgentsGroupCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/GetCompanyContextId", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[return: XmlElement(IsNullable = true)]
		public Guid? GetCompanyContextId(string domainOrPrincipalName)
		{
			object[] array = base.Invoke("GetCompanyContextId", new object[]
			{
				domainOrPrincipalName
			});
			return (Guid?)array[0];
		}

		public IAsyncResult BeginGetCompanyContextId(string domainOrPrincipalName, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetCompanyContextId", new object[]
			{
				domainOrPrincipalName
			}, callback, asyncState);
		}

		public Guid? EndGetCompanyContextId(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (Guid?)array[0];
		}

		public void GetCompanyContextIdAsync(string domainOrPrincipalName)
		{
			this.GetCompanyContextIdAsync(domainOrPrincipalName, null);
		}

		public void GetCompanyContextIdAsync(string domainOrPrincipalName, object userState)
		{
			if (this.GetCompanyContextIdOperationCompleted == null)
			{
				this.GetCompanyContextIdOperationCompleted = new SendOrPostCallback(this.OnGetCompanyContextIdOperationCompleted);
			}
			base.InvokeAsync("GetCompanyContextId", new object[]
			{
				domainOrPrincipalName
			}, this.GetCompanyContextIdOperationCompleted, userState);
		}

		private void OnGetCompanyContextIdOperationCompleted(object arg)
		{
			if (this.GetCompanyContextIdCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetCompanyContextIdCompleted(this, new GetCompanyContextIdCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/GetPartitionId", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public int GetPartitionId(Guid contextId)
		{
			object[] array = base.Invoke("GetPartitionId", new object[]
			{
				contextId
			});
			return (int)array[0];
		}

		public IAsyncResult BeginGetPartitionId(Guid contextId, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetPartitionId", new object[]
			{
				contextId
			}, callback, asyncState);
		}

		public int EndGetPartitionId(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (int)array[0];
		}

		public void GetPartitionIdAsync(Guid contextId)
		{
			this.GetPartitionIdAsync(contextId, null);
		}

		public void GetPartitionIdAsync(Guid contextId, object userState)
		{
			if (this.GetPartitionIdOperationCompleted == null)
			{
				this.GetPartitionIdOperationCompleted = new SendOrPostCallback(this.OnGetPartitionIdOperationCompleted);
			}
			base.InvokeAsync("GetPartitionId", new object[]
			{
				contextId
			}, this.GetPartitionIdOperationCompleted, userState);
		}

		private void OnGetPartitionIdOperationCompleted(object arg)
		{
			if (this.GetPartitionIdCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetPartitionIdCompleted(this, new GetPartitionIdCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/GetPartNumberFromSkuId", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string GetPartNumberFromSkuId(Guid skuId)
		{
			object[] array = base.Invoke("GetPartNumberFromSkuId", new object[]
			{
				skuId
			});
			return (string)array[0];
		}

		public IAsyncResult BeginGetPartNumberFromSkuId(Guid skuId, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetPartNumberFromSkuId", new object[]
			{
				skuId
			}, callback, asyncState);
		}

		public string EndGetPartNumberFromSkuId(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (string)array[0];
		}

		public void GetPartNumberFromSkuIdAsync(Guid skuId)
		{
			this.GetPartNumberFromSkuIdAsync(skuId, null);
		}

		public void GetPartNumberFromSkuIdAsync(Guid skuId, object userState)
		{
			if (this.GetPartNumberFromSkuIdOperationCompleted == null)
			{
				this.GetPartNumberFromSkuIdOperationCompleted = new SendOrPostCallback(this.OnGetPartNumberFromSkuIdOperationCompleted);
			}
			base.InvokeAsync("GetPartNumberFromSkuId", new object[]
			{
				skuId
			}, this.GetPartNumberFromSkuIdOperationCompleted, userState);
		}

		private void OnGetPartNumberFromSkuIdOperationCompleted(object arg)
		{
			if (this.GetPartNumberFromSkuIdCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetPartNumberFromSkuIdCompleted(this, new GetPartNumberFromSkuIdCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/GetSkuIdFromPartNumber", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public Guid GetSkuIdFromPartNumber(string partNumber)
		{
			object[] array = base.Invoke("GetSkuIdFromPartNumber", new object[]
			{
				partNumber
			});
			return (Guid)array[0];
		}

		public IAsyncResult BeginGetSkuIdFromPartNumber(string partNumber, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetSkuIdFromPartNumber", new object[]
			{
				partNumber
			}, callback, asyncState);
		}

		public Guid EndGetSkuIdFromPartNumber(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (Guid)array[0];
		}

		public void GetSkuIdFromPartNumberAsync(string partNumber)
		{
			this.GetSkuIdFromPartNumberAsync(partNumber, null);
		}

		public void GetSkuIdFromPartNumberAsync(string partNumber, object userState)
		{
			if (this.GetSkuIdFromPartNumberOperationCompleted == null)
			{
				this.GetSkuIdFromPartNumberOperationCompleted = new SendOrPostCallback(this.OnGetSkuIdFromPartNumberOperationCompleted);
			}
			base.InvokeAsync("GetSkuIdFromPartNumber", new object[]
			{
				partNumber
			}, this.GetSkuIdFromPartNumberOperationCompleted, userState);
		}

		private void OnGetSkuIdFromPartNumberOperationCompleted(object arg)
		{
			if (this.GetSkuIdFromPartNumberCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetSkuIdFromPartNumberCompleted(this, new GetSkuIdFromPartNumberCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/GetAccountSubscriptions", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public Subscription[] GetAccountSubscriptions(Guid contextId, Guid accountId)
		{
			object[] array = base.Invoke("GetAccountSubscriptions", new object[]
			{
				contextId,
				accountId
			});
			return (Subscription[])array[0];
		}

		public IAsyncResult BeginGetAccountSubscriptions(Guid contextId, Guid accountId, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetAccountSubscriptions", new object[]
			{
				contextId,
				accountId
			}, callback, asyncState);
		}

		public Subscription[] EndGetAccountSubscriptions(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (Subscription[])array[0];
		}

		public void GetAccountSubscriptionsAsync(Guid contextId, Guid accountId)
		{
			this.GetAccountSubscriptionsAsync(contextId, accountId, null);
		}

		public void GetAccountSubscriptionsAsync(Guid contextId, Guid accountId, object userState)
		{
			if (this.GetAccountSubscriptionsOperationCompleted == null)
			{
				this.GetAccountSubscriptionsOperationCompleted = new SendOrPostCallback(this.OnGetAccountSubscriptionsOperationCompleted);
			}
			base.InvokeAsync("GetAccountSubscriptions", new object[]
			{
				contextId,
				accountId
			}, this.GetAccountSubscriptionsOperationCompleted, userState);
		}

		private void OnGetAccountSubscriptionsOperationCompleted(object arg)
		{
			if (this.GetAccountSubscriptionsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetAccountSubscriptionsCompleted(this, new GetAccountSubscriptionsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/GetCompanyAccounts", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public Account[] GetCompanyAccounts(Guid contextId)
		{
			object[] array = base.Invoke("GetCompanyAccounts", new object[]
			{
				contextId
			});
			return (Account[])array[0];
		}

		public IAsyncResult BeginGetCompanyAccounts(Guid contextId, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetCompanyAccounts", new object[]
			{
				contextId
			}, callback, asyncState);
		}

		public Account[] EndGetCompanyAccounts(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (Account[])array[0];
		}

		public void GetCompanyAccountsAsync(Guid contextId)
		{
			this.GetCompanyAccountsAsync(contextId, null);
		}

		public void GetCompanyAccountsAsync(Guid contextId, object userState)
		{
			if (this.GetCompanyAccountsOperationCompleted == null)
			{
				this.GetCompanyAccountsOperationCompleted = new SendOrPostCallback(this.OnGetCompanyAccountsOperationCompleted);
			}
			base.InvokeAsync("GetCompanyAccounts", new object[]
			{
				contextId
			}, this.GetCompanyAccountsOperationCompleted, userState);
		}

		private void OnGetCompanyAccountsOperationCompleted(object arg)
		{
			if (this.GetCompanyAccountsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetCompanyAccountsCompleted(this, new GetCompanyAccountsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/GetCompanyAssignedPlans", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public AssignedPlanValue[] GetCompanyAssignedPlans(Guid contextId)
		{
			object[] array = base.Invoke("GetCompanyAssignedPlans", new object[]
			{
				contextId
			});
			return (AssignedPlanValue[])array[0];
		}

		public IAsyncResult BeginGetCompanyAssignedPlans(Guid contextId, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetCompanyAssignedPlans", new object[]
			{
				contextId
			}, callback, asyncState);
		}

		public AssignedPlanValue[] EndGetCompanyAssignedPlans(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (AssignedPlanValue[])array[0];
		}

		public void GetCompanyAssignedPlansAsync(Guid contextId)
		{
			this.GetCompanyAssignedPlansAsync(contextId, null);
		}

		public void GetCompanyAssignedPlansAsync(Guid contextId, object userState)
		{
			if (this.GetCompanyAssignedPlansOperationCompleted == null)
			{
				this.GetCompanyAssignedPlansOperationCompleted = new SendOrPostCallback(this.OnGetCompanyAssignedPlansOperationCompleted);
			}
			base.InvokeAsync("GetCompanyAssignedPlans", new object[]
			{
				contextId
			}, this.GetCompanyAssignedPlansOperationCompleted, userState);
		}

		private void OnGetCompanyAssignedPlansOperationCompleted(object arg)
		{
			if (this.GetCompanyAssignedPlansCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetCompanyAssignedPlansCompleted(this, new GetCompanyAssignedPlansCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/GetCompanyProvisionedPlans", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ProvisionedPlanValue[] GetCompanyProvisionedPlans(Guid contextId)
		{
			object[] array = base.Invoke("GetCompanyProvisionedPlans", new object[]
			{
				contextId
			});
			return (ProvisionedPlanValue[])array[0];
		}

		public IAsyncResult BeginGetCompanyProvisionedPlans(Guid contextId, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetCompanyProvisionedPlans", new object[]
			{
				contextId
			}, callback, asyncState);
		}

		public ProvisionedPlanValue[] EndGetCompanyProvisionedPlans(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (ProvisionedPlanValue[])array[0];
		}

		public void GetCompanyProvisionedPlansAsync(Guid contextId)
		{
			this.GetCompanyProvisionedPlansAsync(contextId, null);
		}

		public void GetCompanyProvisionedPlansAsync(Guid contextId, object userState)
		{
			if (this.GetCompanyProvisionedPlansOperationCompleted == null)
			{
				this.GetCompanyProvisionedPlansOperationCompleted = new SendOrPostCallback(this.OnGetCompanyProvisionedPlansOperationCompleted);
			}
			base.InvokeAsync("GetCompanyProvisionedPlans", new object[]
			{
				contextId
			}, this.GetCompanyProvisionedPlansOperationCompleted, userState);
		}

		private void OnGetCompanyProvisionedPlansOperationCompleted(object arg)
		{
			if (this.GetCompanyProvisionedPlansCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetCompanyProvisionedPlansCompleted(this, new GetCompanyProvisionedPlansCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/GetCompanySubscriptions", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public Subscription[] GetCompanySubscriptions(Guid contextId)
		{
			object[] array = base.Invoke("GetCompanySubscriptions", new object[]
			{
				contextId
			});
			return (Subscription[])array[0];
		}

		public IAsyncResult BeginGetCompanySubscriptions(Guid contextId, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetCompanySubscriptions", new object[]
			{
				contextId
			}, callback, asyncState);
		}

		public Subscription[] EndGetCompanySubscriptions(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (Subscription[])array[0];
		}

		public void GetCompanySubscriptionsAsync(Guid contextId)
		{
			this.GetCompanySubscriptionsAsync(contextId, null);
		}

		public void GetCompanySubscriptionsAsync(Guid contextId, object userState)
		{
			if (this.GetCompanySubscriptionsOperationCompleted == null)
			{
				this.GetCompanySubscriptionsOperationCompleted = new SendOrPostCallback(this.OnGetCompanySubscriptionsOperationCompleted);
			}
			base.InvokeAsync("GetCompanySubscriptions", new object[]
			{
				contextId
			}, this.GetCompanySubscriptionsOperationCompleted, userState);
		}

		private void OnGetCompanySubscriptionsOperationCompleted(object arg)
		{
			if (this.GetCompanySubscriptionsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetCompanySubscriptionsCompleted(this, new GetCompanySubscriptionsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/GetCompanyForeignPrincipalObjects", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public ForeignPrincipal[] GetCompanyForeignPrincipalObjects(Guid contextId)
		{
			object[] array = base.Invoke("GetCompanyForeignPrincipalObjects", new object[]
			{
				contextId
			});
			return (ForeignPrincipal[])array[0];
		}

		public IAsyncResult BeginGetCompanyForeignPrincipalObjects(Guid contextId, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetCompanyForeignPrincipalObjects", new object[]
			{
				contextId
			}, callback, asyncState);
		}

		public ForeignPrincipal[] EndGetCompanyForeignPrincipalObjects(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (ForeignPrincipal[])array[0];
		}

		public void GetCompanyForeignPrincipalObjectsAsync(Guid contextId)
		{
			this.GetCompanyForeignPrincipalObjectsAsync(contextId, null);
		}

		public void GetCompanyForeignPrincipalObjectsAsync(Guid contextId, object userState)
		{
			if (this.GetCompanyForeignPrincipalObjectsOperationCompleted == null)
			{
				this.GetCompanyForeignPrincipalObjectsOperationCompleted = new SendOrPostCallback(this.OnGetCompanyForeignPrincipalObjectsOperationCompleted);
			}
			base.InvokeAsync("GetCompanyForeignPrincipalObjects", new object[]
			{
				contextId
			}, this.GetCompanyForeignPrincipalObjectsOperationCompleted, userState);
		}

		private void OnGetCompanyForeignPrincipalObjectsOperationCompleted(object arg)
		{
			if (this.GetCompanyForeignPrincipalObjectsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetCompanyForeignPrincipalObjectsCompleted(this, new GetCompanyForeignPrincipalObjectsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/SetCompanyProvisioningStatus", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void SetCompanyProvisioningStatus(Guid contextId, ServicePlanProvisioningStatus[] servicePlanProvisioningStatus)
		{
			base.Invoke("SetCompanyProvisioningStatus", new object[]
			{
				contextId,
				servicePlanProvisioningStatus
			});
		}

		public IAsyncResult BeginSetCompanyProvisioningStatus(Guid contextId, ServicePlanProvisioningStatus[] servicePlanProvisioningStatus, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("SetCompanyProvisioningStatus", new object[]
			{
				contextId,
				servicePlanProvisioningStatus
			}, callback, asyncState);
		}

		public void EndSetCompanyProvisioningStatus(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void SetCompanyProvisioningStatusAsync(Guid contextId, ServicePlanProvisioningStatus[] servicePlanProvisioningStatus)
		{
			this.SetCompanyProvisioningStatusAsync(contextId, servicePlanProvisioningStatus, null);
		}

		public void SetCompanyProvisioningStatusAsync(Guid contextId, ServicePlanProvisioningStatus[] servicePlanProvisioningStatus, object userState)
		{
			if (this.SetCompanyProvisioningStatusOperationCompleted == null)
			{
				this.SetCompanyProvisioningStatusOperationCompleted = new SendOrPostCallback(this.OnSetCompanyProvisioningStatusOperationCompleted);
			}
			base.InvokeAsync("SetCompanyProvisioningStatus", new object[]
			{
				contextId,
				servicePlanProvisioningStatus
			}, this.SetCompanyProvisioningStatusOperationCompleted, userState);
		}

		private void OnSetCompanyProvisioningStatusOperationCompleted(object arg)
		{
			if (this.SetCompanyProvisioningStatusCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SetCompanyProvisioningStatusCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/ForceRefreshSystemData", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void ForceRefreshSystemData()
		{
			base.Invoke("ForceRefreshSystemData", new object[0]);
		}

		public IAsyncResult BeginForceRefreshSystemData(AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("ForceRefreshSystemData", new object[0], callback, asyncState);
		}

		public void EndForceRefreshSystemData(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void ForceRefreshSystemDataAsync()
		{
			this.ForceRefreshSystemDataAsync(null);
		}

		public void ForceRefreshSystemDataAsync(object userState)
		{
			if (this.ForceRefreshSystemDataOperationCompleted == null)
			{
				this.ForceRefreshSystemDataOperationCompleted = new SendOrPostCallback(this.OnForceRefreshSystemDataOperationCompleted);
			}
			base.InvokeAsync("ForceRefreshSystemData", new object[0], this.ForceRefreshSystemDataOperationCompleted, userState);
		}

		private void OnForceRefreshSystemDataOperationCompleted(object arg)
		{
			if (this.ForceRefreshSystemDataCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ForceRefreshSystemDataCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/BuildRandomAccount", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public Account BuildRandomAccount()
		{
			object[] array = base.Invoke("BuildRandomAccount", new object[0]);
			return (Account)array[0];
		}

		public IAsyncResult BeginBuildRandomAccount(AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("BuildRandomAccount", new object[0], callback, asyncState);
		}

		public Account EndBuildRandomAccount(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (Account)array[0];
		}

		public void BuildRandomAccountAsync()
		{
			this.BuildRandomAccountAsync(null);
		}

		public void BuildRandomAccountAsync(object userState)
		{
			if (this.BuildRandomAccountOperationCompleted == null)
			{
				this.BuildRandomAccountOperationCompleted = new SendOrPostCallback(this.OnBuildRandomAccountOperationCompleted);
			}
			base.InvokeAsync("BuildRandomAccount", new object[0], this.BuildRandomAccountOperationCompleted, userState);
		}

		private void OnBuildRandomAccountOperationCompleted(object arg)
		{
			if (this.BuildRandomAccountCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.BuildRandomAccountCompleted(this, new BuildRandomAccountCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/BuildRandomCompany", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public Company BuildRandomCompany(string domainSuffix)
		{
			object[] array = base.Invoke("BuildRandomCompany", new object[]
			{
				domainSuffix
			});
			return (Company)array[0];
		}

		public IAsyncResult BeginBuildRandomCompany(string domainSuffix, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("BuildRandomCompany", new object[]
			{
				domainSuffix
			}, callback, asyncState);
		}

		public Company EndBuildRandomCompany(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (Company)array[0];
		}

		public void BuildRandomCompanyAsync(string domainSuffix)
		{
			this.BuildRandomCompanyAsync(domainSuffix, null);
		}

		public void BuildRandomCompanyAsync(string domainSuffix, object userState)
		{
			if (this.BuildRandomCompanyOperationCompleted == null)
			{
				this.BuildRandomCompanyOperationCompleted = new SendOrPostCallback(this.OnBuildRandomCompanyOperationCompleted);
			}
			base.InvokeAsync("BuildRandomCompany", new object[]
			{
				domainSuffix
			}, this.BuildRandomCompanyOperationCompleted, userState);
		}

		private void OnBuildRandomCompanyOperationCompleted(object arg)
		{
			if (this.BuildRandomCompanyCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.BuildRandomCompanyCompleted(this, new BuildRandomCompanyCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/BuildRandomCompanyProfile", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public CompanyProfile BuildRandomCompanyProfile()
		{
			object[] array = base.Invoke("BuildRandomCompanyProfile", new object[0]);
			return (CompanyProfile)array[0];
		}

		public IAsyncResult BeginBuildRandomCompanyProfile(AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("BuildRandomCompanyProfile", new object[0], callback, asyncState);
		}

		public CompanyProfile EndBuildRandomCompanyProfile(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (CompanyProfile)array[0];
		}

		public void BuildRandomCompanyProfileAsync()
		{
			this.BuildRandomCompanyProfileAsync(null);
		}

		public void BuildRandomCompanyProfileAsync(object userState)
		{
			if (this.BuildRandomCompanyProfileOperationCompleted == null)
			{
				this.BuildRandomCompanyProfileOperationCompleted = new SendOrPostCallback(this.OnBuildRandomCompanyProfileOperationCompleted);
			}
			base.InvokeAsync("BuildRandomCompanyProfile", new object[0], this.BuildRandomCompanyProfileOperationCompleted, userState);
		}

		private void OnBuildRandomCompanyProfileOperationCompleted(object arg)
		{
			if (this.BuildRandomCompanyProfileCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.BuildRandomCompanyProfileCompleted(this, new BuildRandomCompanyProfileCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/BuildRandomUser", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public User BuildRandomUser()
		{
			object[] array = base.Invoke("BuildRandomUser", new object[0]);
			return (User)array[0];
		}

		public IAsyncResult BeginBuildRandomUser(AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("BuildRandomUser", new object[0], callback, asyncState);
		}

		public User EndBuildRandomUser(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (User)array[0];
		}

		public void BuildRandomUserAsync()
		{
			this.BuildRandomUserAsync(null);
		}

		public void BuildRandomUserAsync(object userState)
		{
			if (this.BuildRandomUserOperationCompleted == null)
			{
				this.BuildRandomUserOperationCompleted = new SendOrPostCallback(this.OnBuildRandomUserOperationCompleted);
			}
			base.InvokeAsync("BuildRandomUser", new object[0], this.BuildRandomUserOperationCompleted, userState);
		}

		private void OnBuildRandomUserOperationCompleted(object arg)
		{
			if (this.BuildRandomUserCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.BuildRandomUserCompleted(this, new BuildRandomUserCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/BuildRandomSubscription", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public Subscription BuildRandomSubscription(Guid contextId, Guid accountId, Guid skuId)
		{
			object[] array = base.Invoke("BuildRandomSubscription", new object[]
			{
				contextId,
				accountId,
				skuId
			});
			return (Subscription)array[0];
		}

		public IAsyncResult BeginBuildRandomSubscription(Guid contextId, Guid accountId, Guid skuId, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("BuildRandomSubscription", new object[]
			{
				contextId,
				accountId,
				skuId
			}, callback, asyncState);
		}

		public Subscription EndBuildRandomSubscription(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (Subscription)array[0];
		}

		public void BuildRandomSubscriptionAsync(Guid contextId, Guid accountId, Guid skuId)
		{
			this.BuildRandomSubscriptionAsync(contextId, accountId, skuId, null);
		}

		public void BuildRandomSubscriptionAsync(Guid contextId, Guid accountId, Guid skuId, object userState)
		{
			if (this.BuildRandomSubscriptionOperationCompleted == null)
			{
				this.BuildRandomSubscriptionOperationCompleted = new SendOrPostCallback(this.OnBuildRandomSubscriptionOperationCompleted);
			}
			base.InvokeAsync("BuildRandomSubscription", new object[]
			{
				contextId,
				accountId,
				skuId
			}, this.BuildRandomSubscriptionOperationCompleted, userState);
		}

		private void OnBuildRandomSubscriptionOperationCompleted(object arg)
		{
			if (this.BuildRandomSubscriptionCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.BuildRandomSubscriptionCompleted(this, new BuildRandomSubscriptionCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/GetDefaultContractRoleMap", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public RoleMap[] GetDefaultContractRoleMap()
		{
			object[] array = base.Invoke("GetDefaultContractRoleMap", new object[0]);
			return (RoleMap[])array[0];
		}

		public IAsyncResult BeginGetDefaultContractRoleMap(AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetDefaultContractRoleMap", new object[0], callback, asyncState);
		}

		public RoleMap[] EndGetDefaultContractRoleMap(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (RoleMap[])array[0];
		}

		public void GetDefaultContractRoleMapAsync()
		{
			this.GetDefaultContractRoleMapAsync(null);
		}

		public void GetDefaultContractRoleMapAsync(object userState)
		{
			if (this.GetDefaultContractRoleMapOperationCompleted == null)
			{
				this.GetDefaultContractRoleMapOperationCompleted = new SendOrPostCallback(this.OnGetDefaultContractRoleMapOperationCompleted);
			}
			base.InvokeAsync("GetDefaultContractRoleMap", new object[0], this.GetDefaultContractRoleMapOperationCompleted, userState);
		}

		private void OnGetDefaultContractRoleMapOperationCompleted(object arg)
		{
			if (this.GetDefaultContractRoleMapCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetDefaultContractRoleMapCompleted(this, new GetDefaultContractRoleMapCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://www.ccs.com/TestServices/ForceTransitiveReplication", RequestNamespace = "http://www.ccs.com/TestServices/", ResponseNamespace = "http://www.ccs.com/TestServices/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public bool ForceTransitiveReplication()
		{
			object[] array = base.Invoke("ForceTransitiveReplication", new object[0]);
			return (bool)array[0];
		}

		public IAsyncResult BeginForceTransitiveReplication(AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("ForceTransitiveReplication", new object[0], callback, asyncState);
		}

		public bool EndForceTransitiveReplication(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (bool)array[0];
		}

		public void ForceTransitiveReplicationAsync()
		{
			this.ForceTransitiveReplicationAsync(null);
		}

		public void ForceTransitiveReplicationAsync(object userState)
		{
			if (this.ForceTransitiveReplicationOperationCompleted == null)
			{
				this.ForceTransitiveReplicationOperationCompleted = new SendOrPostCallback(this.OnForceTransitiveReplicationOperationCompleted);
			}
			base.InvokeAsync("ForceTransitiveReplication", new object[0], this.ForceTransitiveReplicationOperationCompleted, userState);
		}

		private void OnForceTransitiveReplicationOperationCompleted(object arg)
		{
			if (this.ForceTransitiveReplicationCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ForceTransitiveReplicationCompleted(this, new ForceTransitiveReplicationCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		public new void CancelAsync(object userState)
		{
			base.CancelAsync(userState);
		}

		private SendOrPostCallback IsDomainAvailableOperationCompleted;

		private SendOrPostCallback CreateCompanyOperationCompleted;

		private SendOrPostCallback CreateSyndicatedCompanyOperationCompleted;

		private SendOrPostCallback SetCompanyPartnershipOperationCompleted;

		private SendOrPostCallback UpdateCompanyProfileOperationCompleted;

		private SendOrPostCallback UpdateCompanyTagsOperationCompleted;

		private SendOrPostCallback DeleteCompanyOperationCompleted;

		private SendOrPostCallback ForceDeleteCompanyOperationCompleted;

		private SendOrPostCallback CreateAccountOperationCompleted;

		private SendOrPostCallback RenameAccountOperationCompleted;

		private SendOrPostCallback DeleteAccountOperationCompleted;

		private SendOrPostCallback CreateUpdateDeleteSubscriptionOperationCompleted;

		private SendOrPostCallback CreateCompanyWithSubscriptionsOperationCompleted;

		private SendOrPostCallback SignupOperationCompleted;

		private SendOrPostCallback SignupWithCompanyTagsOperationCompleted;

		private SendOrPostCallback PromoteToPartnerOperationCompleted;

		private SendOrPostCallback DemoteToCompanyOperationCompleted;

		private SendOrPostCallback ForceDemoteToCompanyOperationCompleted;

		private SendOrPostCallback AddServiceTypeOperationCompleted;

		private SendOrPostCallback RemoveServiceTypeOperationCompleted;

		private SendOrPostCallback ListServicesForPartnershipOperationCompleted;

		private SendOrPostCallback AssociateToPartnerOperationCompleted;

		private SendOrPostCallback DeletePartnerContractOperationCompleted;

		private SendOrPostCallback CreatePartnerOperationCompleted;

		private SendOrPostCallback CreateMailboxAgentsGroupOperationCompleted;

		private SendOrPostCallback GetCompanyContextIdOperationCompleted;

		private SendOrPostCallback GetPartitionIdOperationCompleted;

		private SendOrPostCallback GetPartNumberFromSkuIdOperationCompleted;

		private SendOrPostCallback GetSkuIdFromPartNumberOperationCompleted;

		private SendOrPostCallback GetAccountSubscriptionsOperationCompleted;

		private SendOrPostCallback GetCompanyAccountsOperationCompleted;

		private SendOrPostCallback GetCompanyAssignedPlansOperationCompleted;

		private SendOrPostCallback GetCompanyProvisionedPlansOperationCompleted;

		private SendOrPostCallback GetCompanySubscriptionsOperationCompleted;

		private SendOrPostCallback GetCompanyForeignPrincipalObjectsOperationCompleted;

		private SendOrPostCallback SetCompanyProvisioningStatusOperationCompleted;

		private SendOrPostCallback ForceRefreshSystemDataOperationCompleted;

		private SendOrPostCallback BuildRandomAccountOperationCompleted;

		private SendOrPostCallback BuildRandomCompanyOperationCompleted;

		private SendOrPostCallback BuildRandomCompanyProfileOperationCompleted;

		private SendOrPostCallback BuildRandomUserOperationCompleted;

		private SendOrPostCallback BuildRandomSubscriptionOperationCompleted;

		private SendOrPostCallback GetDefaultContractRoleMapOperationCompleted;

		private SendOrPostCallback ForceTransitiveReplicationOperationCompleted;
	}
}
