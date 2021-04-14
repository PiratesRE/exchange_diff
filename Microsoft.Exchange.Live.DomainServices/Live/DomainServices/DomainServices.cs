using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Live.DomainServices
{
	[DebuggerStepThrough]
	[WebServiceBinding(Name = "DomainServicesSoap", Namespace = "http://domains.live.com/Service/DomainServices/V1.0")]
	[DesignerCategory("code")]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	public class DomainServices : SoapHttpClientProtocol
	{
		public DomainServices()
		{
			base.Url = "https://domains-tst.live-int.com/service/DomainServices.asmx";
		}

		public AdminPassportAuthHeader AdminPassportAuthHeaderValue
		{
			get
			{
				return this.adminPassportAuthHeaderValueField;
			}
			set
			{
				this.adminPassportAuthHeaderValueField = value;
			}
		}

		public ManagementCertificateAuthHeader ManagementCertificateAuthHeaderValue
		{
			get
			{
				return this.managementCertificateAuthHeaderValueField;
			}
			set
			{
				this.managementCertificateAuthHeaderValueField = value;
			}
		}

		public PartnerAuthHeader PartnerAuthHeaderValue
		{
			get
			{
				return this.partnerAuthHeaderValueField;
			}
			set
			{
				this.partnerAuthHeaderValueField = value;
			}
		}

		public event TestConnectionCompletedEventHandler TestConnectionCompleted;

		public event GetDomainAvailabilityCompletedEventHandler GetDomainAvailabilityCompleted;

		public event GetDomainInfoCompletedEventHandler GetDomainInfoCompleted;

		public event GetDomainInfoExCompletedEventHandler GetDomainInfoExCompleted;

		public event ReserveDomainCompletedEventHandler ReserveDomainCompleted;

		public event ReleaseDomainCompletedEventHandler ReleaseDomainCompleted;

		public event ProcessDomainCompletedEventHandler ProcessDomainCompleted;

		public event GetMemberTypeCompletedEventHandler GetMemberTypeCompleted;

		public event MemberNameToNetIdCompletedEventHandler MemberNameToNetIdCompleted;

		public event NetIdToMemberNameCompletedEventHandler NetIdToMemberNameCompleted;

		public event GetCountMembersCompletedEventHandler GetCountMembersCompleted;

		public event EnumMembersCompletedEventHandler EnumMembersCompleted;

		public event CreateMemberCompletedEventHandler CreateMemberCompleted;

		public event CreateMemberEncryptedCompletedEventHandler CreateMemberEncryptedCompleted;

		public event CreateMemberExCompletedEventHandler CreateMemberExCompleted;

		public event CreateMemberEncryptedExCompletedEventHandler CreateMemberEncryptedExCompleted;

		public event AddBrandInfoCompletedEventHandler AddBrandInfoCompleted;

		public event RemoveBrandInfoCompletedEventHandler RemoveBrandInfoCompleted;

		public event RenameMemberCompletedEventHandler RenameMemberCompleted;

		public event SetMemberPropertiesCompletedEventHandler SetMemberPropertiesCompleted;

		public event GetMemberPropertiesCompletedEventHandler GetMemberPropertiesCompleted;

		public event EvictMemberCompletedEventHandler EvictMemberCompleted;

		public event ResetMemberPasswordCompletedEventHandler ResetMemberPasswordCompleted;

		public event ResetMemberPasswordEncryptedCompletedEventHandler ResetMemberPasswordEncryptedCompleted;

		public event BlockMemberEmailCompletedEventHandler BlockMemberEmailCompleted;

		public event ImportUnmanagedMemberCompletedEventHandler ImportUnmanagedMemberCompleted;

		public event EvictUnmanagedMemberCompletedEventHandler EvictUnmanagedMemberCompleted;

		public event EvictAllUnmanagedMembersCompletedEventHandler EvictAllUnmanagedMembersCompleted;

		public event EnableOpenMembershipCompletedEventHandler EnableOpenMembershipCompleted;

		public event ProvisionMemberSubscriptionCompletedEventHandler ProvisionMemberSubscriptionCompleted;

		public event DeprovisionMemberSubscriptionCompletedEventHandler DeprovisionMemberSubscriptionCompleted;

		public event ConvertMemberSubscriptionCompletedEventHandler ConvertMemberSubscriptionCompleted;

		public event MemberHasSubscriptionCompletedEventHandler MemberHasSubscriptionCompleted;

		public event SuspendEmailCompletedEventHandler SuspendEmailCompleted;

		public event GetMxRecordsCompletedEventHandler GetMxRecordsCompleted;

		public event MemberHasMailboxCompletedEventHandler MemberHasMailboxCompleted;

		public event CompleteMemberEmailMigrationCompletedEventHandler CompleteMemberEmailMigrationCompleted;

		public event CompleteDomainEmailMigrationCompletedEventHandler CompleteDomainEmailMigrationCompleted;

		public event CreateByodRequestCompletedEventHandler CreateByodRequestCompleted;

		public event EnumByodDomainsCompletedEventHandler EnumByodDomainsCompleted;

		public event GetAdminsCompletedEventHandler GetAdminsCompleted;

		public event GetManagementCertificateCompletedEventHandler GetManagementCertificateCompleted;

		public event SetManagementCertificateCompletedEventHandler SetManagementCertificateCompleted;

		public event SetManagementPermissionsCompletedEventHandler SetManagementPermissionsCompleted;

		public event GetMaxMembersCompletedEventHandler GetMaxMembersCompleted;

		public event SetMaxMembersCompletedEventHandler SetMaxMembersCompleted;

		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/TestConnection", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("PartnerAuthHeaderValue")]
		[SoapHeader("AdminPassportAuthHeaderValue")]
		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		public string TestConnection(string input)
		{
			object[] array = base.Invoke("TestConnection", new object[]
			{
				input
			});
			return (string)array[0];
		}

		public IAsyncResult BeginTestConnection(string input, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("TestConnection", new object[]
			{
				input
			}, callback, asyncState);
		}

		public string EndTestConnection(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (string)array[0];
		}

		public void TestConnectionAsync(string input)
		{
			this.TestConnectionAsync(input, null);
		}

		public void TestConnectionAsync(string input, object userState)
		{
			if (this.TestConnectionOperationCompleted == null)
			{
				this.TestConnectionOperationCompleted = new SendOrPostCallback(this.OnTestConnectionOperationCompleted);
			}
			base.InvokeAsync("TestConnection", new object[]
			{
				input
			}, this.TestConnectionOperationCompleted, userState);
		}

		private void OnTestConnectionOperationCompleted(object arg)
		{
			if (this.TestConnectionCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.TestConnectionCompleted(this, new TestConnectionCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("PartnerAuthHeaderValue")]
		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/GetDomainAvailability", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public DomainAvailability GetDomainAvailability(string domainName)
		{
			object[] array = base.Invoke("GetDomainAvailability", new object[]
			{
				domainName
			});
			return (DomainAvailability)array[0];
		}

		public IAsyncResult BeginGetDomainAvailability(string domainName, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetDomainAvailability", new object[]
			{
				domainName
			}, callback, asyncState);
		}

		public DomainAvailability EndGetDomainAvailability(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (DomainAvailability)array[0];
		}

		public void GetDomainAvailabilityAsync(string domainName)
		{
			this.GetDomainAvailabilityAsync(domainName, null);
		}

		public void GetDomainAvailabilityAsync(string domainName, object userState)
		{
			if (this.GetDomainAvailabilityOperationCompleted == null)
			{
				this.GetDomainAvailabilityOperationCompleted = new SendOrPostCallback(this.OnGetDomainAvailabilityOperationCompleted);
			}
			base.InvokeAsync("GetDomainAvailability", new object[]
			{
				domainName
			}, this.GetDomainAvailabilityOperationCompleted, userState);
		}

		private void OnGetDomainAvailabilityOperationCompleted(object arg)
		{
			if (this.GetDomainAvailabilityCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetDomainAvailabilityCompleted(this, new GetDomainAvailabilityCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/GetDomainInfo", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		[SoapHeader("AdminPassportAuthHeaderValue")]
		[SoapHeader("PartnerAuthHeaderValue")]
		public DomainInfo GetDomainInfo(string domainName)
		{
			object[] array = base.Invoke("GetDomainInfo", new object[]
			{
				domainName
			});
			return (DomainInfo)array[0];
		}

		public IAsyncResult BeginGetDomainInfo(string domainName, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetDomainInfo", new object[]
			{
				domainName
			}, callback, asyncState);
		}

		public DomainInfo EndGetDomainInfo(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (DomainInfo)array[0];
		}

		public void GetDomainInfoAsync(string domainName)
		{
			this.GetDomainInfoAsync(domainName, null);
		}

		public void GetDomainInfoAsync(string domainName, object userState)
		{
			if (this.GetDomainInfoOperationCompleted == null)
			{
				this.GetDomainInfoOperationCompleted = new SendOrPostCallback(this.OnGetDomainInfoOperationCompleted);
			}
			base.InvokeAsync("GetDomainInfo", new object[]
			{
				domainName
			}, this.GetDomainInfoOperationCompleted, userState);
		}

		private void OnGetDomainInfoOperationCompleted(object arg)
		{
			if (this.GetDomainInfoCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetDomainInfoCompleted(this, new GetDomainInfoCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/GetDomainInfoEx", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("AdminPassportAuthHeaderValue")]
		[SoapHeader("PartnerAuthHeaderValue")]
		public DomainInfoEx GetDomainInfoEx(string domainName)
		{
			object[] array = base.Invoke("GetDomainInfoEx", new object[]
			{
				domainName
			});
			return (DomainInfoEx)array[0];
		}

		public IAsyncResult BeginGetDomainInfoEx(string domainName, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetDomainInfoEx", new object[]
			{
				domainName
			}, callback, asyncState);
		}

		public DomainInfoEx EndGetDomainInfoEx(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (DomainInfoEx)array[0];
		}

		public void GetDomainInfoExAsync(string domainName)
		{
			this.GetDomainInfoExAsync(domainName, null);
		}

		public void GetDomainInfoExAsync(string domainName, object userState)
		{
			if (this.GetDomainInfoExOperationCompleted == null)
			{
				this.GetDomainInfoExOperationCompleted = new SendOrPostCallback(this.OnGetDomainInfoExOperationCompleted);
			}
			base.InvokeAsync("GetDomainInfoEx", new object[]
			{
				domainName
			}, this.GetDomainInfoExOperationCompleted, userState);
		}

		private void OnGetDomainInfoExOperationCompleted(object arg)
		{
			if (this.GetDomainInfoExCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetDomainInfoExCompleted(this, new GetDomainInfoExCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/ReserveDomain", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("PartnerAuthHeaderValue")]
		public DomainInfoEx ReserveDomain(string domainName, string domainConfigId, bool processSynch)
		{
			object[] array = base.Invoke("ReserveDomain", new object[]
			{
				domainName,
				domainConfigId,
				processSynch
			});
			return (DomainInfoEx)array[0];
		}

		public IAsyncResult BeginReserveDomain(string domainName, string domainConfigId, bool processSynch, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("ReserveDomain", new object[]
			{
				domainName,
				domainConfigId,
				processSynch
			}, callback, asyncState);
		}

		public DomainInfoEx EndReserveDomain(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (DomainInfoEx)array[0];
		}

		public void ReserveDomainAsync(string domainName, string domainConfigId, bool processSynch)
		{
			this.ReserveDomainAsync(domainName, domainConfigId, processSynch, null);
		}

		public void ReserveDomainAsync(string domainName, string domainConfigId, bool processSynch, object userState)
		{
			if (this.ReserveDomainOperationCompleted == null)
			{
				this.ReserveDomainOperationCompleted = new SendOrPostCallback(this.OnReserveDomainOperationCompleted);
			}
			base.InvokeAsync("ReserveDomain", new object[]
			{
				domainName,
				domainConfigId,
				processSynch
			}, this.ReserveDomainOperationCompleted, userState);
		}

		private void OnReserveDomainOperationCompleted(object arg)
		{
			if (this.ReserveDomainCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ReserveDomainCompleted(this, new ReserveDomainCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/ReleaseDomain", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("PartnerAuthHeaderValue")]
		public void ReleaseDomain(string domainName)
		{
			base.Invoke("ReleaseDomain", new object[]
			{
				domainName
			});
		}

		public IAsyncResult BeginReleaseDomain(string domainName, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("ReleaseDomain", new object[]
			{
				domainName
			}, callback, asyncState);
		}

		public void EndReleaseDomain(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void ReleaseDomainAsync(string domainName)
		{
			this.ReleaseDomainAsync(domainName, null);
		}

		public void ReleaseDomainAsync(string domainName, object userState)
		{
			if (this.ReleaseDomainOperationCompleted == null)
			{
				this.ReleaseDomainOperationCompleted = new SendOrPostCallback(this.OnReleaseDomainOperationCompleted);
			}
			base.InvokeAsync("ReleaseDomain", new object[]
			{
				domainName
			}, this.ReleaseDomainOperationCompleted, userState);
		}

		private void OnReleaseDomainOperationCompleted(object arg)
		{
			if (this.ReleaseDomainCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ReleaseDomainCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("PartnerAuthHeaderValue")]
		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/ProcessDomain", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public DomainInfoEx ProcessDomain(string domainName, bool processSynch)
		{
			object[] array = base.Invoke("ProcessDomain", new object[]
			{
				domainName,
				processSynch
			});
			return (DomainInfoEx)array[0];
		}

		public IAsyncResult BeginProcessDomain(string domainName, bool processSynch, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("ProcessDomain", new object[]
			{
				domainName,
				processSynch
			}, callback, asyncState);
		}

		public DomainInfoEx EndProcessDomain(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (DomainInfoEx)array[0];
		}

		public void ProcessDomainAsync(string domainName, bool processSynch)
		{
			this.ProcessDomainAsync(domainName, processSynch, null);
		}

		public void ProcessDomainAsync(string domainName, bool processSynch, object userState)
		{
			if (this.ProcessDomainOperationCompleted == null)
			{
				this.ProcessDomainOperationCompleted = new SendOrPostCallback(this.OnProcessDomainOperationCompleted);
			}
			base.InvokeAsync("ProcessDomain", new object[]
			{
				domainName,
				processSynch
			}, this.ProcessDomainOperationCompleted, userState);
		}

		private void OnProcessDomainOperationCompleted(object arg)
		{
			if (this.ProcessDomainCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ProcessDomainCompleted(this, new ProcessDomainCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		[SoapHeader("AdminPassportAuthHeaderValue")]
		[SoapHeader("PartnerAuthHeaderValue")]
		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/GetMemberType", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public MemberType GetMemberType(string memberNameIn)
		{
			object[] array = base.Invoke("GetMemberType", new object[]
			{
				memberNameIn
			});
			return (MemberType)array[0];
		}

		public IAsyncResult BeginGetMemberType(string memberNameIn, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetMemberType", new object[]
			{
				memberNameIn
			}, callback, asyncState);
		}

		public MemberType EndGetMemberType(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (MemberType)array[0];
		}

		public void GetMemberTypeAsync(string memberNameIn)
		{
			this.GetMemberTypeAsync(memberNameIn, null);
		}

		public void GetMemberTypeAsync(string memberNameIn, object userState)
		{
			if (this.GetMemberTypeOperationCompleted == null)
			{
				this.GetMemberTypeOperationCompleted = new SendOrPostCallback(this.OnGetMemberTypeOperationCompleted);
			}
			base.InvokeAsync("GetMemberType", new object[]
			{
				memberNameIn
			}, this.GetMemberTypeOperationCompleted, userState);
		}

		private void OnGetMemberTypeOperationCompleted(object arg)
		{
			if (this.GetMemberTypeCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetMemberTypeCompleted(this, new GetMemberTypeCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/MemberNameToNetId", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("PartnerAuthHeaderValue")]
		public string MemberNameToNetId(string memberNameIn)
		{
			object[] array = base.Invoke("MemberNameToNetId", new object[]
			{
				memberNameIn
			});
			return (string)array[0];
		}

		public IAsyncResult BeginMemberNameToNetId(string memberNameIn, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("MemberNameToNetId", new object[]
			{
				memberNameIn
			}, callback, asyncState);
		}

		public string EndMemberNameToNetId(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (string)array[0];
		}

		public void MemberNameToNetIdAsync(string memberNameIn)
		{
			this.MemberNameToNetIdAsync(memberNameIn, null);
		}

		public void MemberNameToNetIdAsync(string memberNameIn, object userState)
		{
			if (this.MemberNameToNetIdOperationCompleted == null)
			{
				this.MemberNameToNetIdOperationCompleted = new SendOrPostCallback(this.OnMemberNameToNetIdOperationCompleted);
			}
			base.InvokeAsync("MemberNameToNetId", new object[]
			{
				memberNameIn
			}, this.MemberNameToNetIdOperationCompleted, userState);
		}

		private void OnMemberNameToNetIdOperationCompleted(object arg)
		{
			if (this.MemberNameToNetIdCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.MemberNameToNetIdCompleted(this, new MemberNameToNetIdCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("PartnerAuthHeaderValue")]
		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/NetIdToMemberName", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string NetIdToMemberName(string netIdIn)
		{
			object[] array = base.Invoke("NetIdToMemberName", new object[]
			{
				netIdIn
			});
			return (string)array[0];
		}

		public IAsyncResult BeginNetIdToMemberName(string netIdIn, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("NetIdToMemberName", new object[]
			{
				netIdIn
			}, callback, asyncState);
		}

		public string EndNetIdToMemberName(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (string)array[0];
		}

		public void NetIdToMemberNameAsync(string netIdIn)
		{
			this.NetIdToMemberNameAsync(netIdIn, null);
		}

		public void NetIdToMemberNameAsync(string netIdIn, object userState)
		{
			if (this.NetIdToMemberNameOperationCompleted == null)
			{
				this.NetIdToMemberNameOperationCompleted = new SendOrPostCallback(this.OnNetIdToMemberNameOperationCompleted);
			}
			base.InvokeAsync("NetIdToMemberName", new object[]
			{
				netIdIn
			}, this.NetIdToMemberNameOperationCompleted, userState);
		}

		private void OnNetIdToMemberNameOperationCompleted(object arg)
		{
			if (this.NetIdToMemberNameCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.NetIdToMemberNameCompleted(this, new NetIdToMemberNameCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/GetCountMembers", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("AdminPassportAuthHeaderValue")]
		[SoapHeader("PartnerAuthHeaderValue")]
		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		public int GetCountMembers(string domainName)
		{
			object[] array = base.Invoke("GetCountMembers", new object[]
			{
				domainName
			});
			return (int)array[0];
		}

		public IAsyncResult BeginGetCountMembers(string domainName, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetCountMembers", new object[]
			{
				domainName
			}, callback, asyncState);
		}

		public int EndGetCountMembers(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (int)array[0];
		}

		public void GetCountMembersAsync(string domainName)
		{
			this.GetCountMembersAsync(domainName, null);
		}

		public void GetCountMembersAsync(string domainName, object userState)
		{
			if (this.GetCountMembersOperationCompleted == null)
			{
				this.GetCountMembersOperationCompleted = new SendOrPostCallback(this.OnGetCountMembersOperationCompleted);
			}
			base.InvokeAsync("GetCountMembers", new object[]
			{
				domainName
			}, this.GetCountMembersOperationCompleted, userState);
		}

		private void OnGetCountMembersOperationCompleted(object arg)
		{
			if (this.GetCountMembersCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetCountMembersCompleted(this, new GetCountMembersCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("AdminPassportAuthHeaderValue")]
		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/EnumMembers", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("PartnerAuthHeaderValue")]
		[return: XmlArrayItem(IsNullable = false)]
		public Member[] EnumMembers(string domainName, string start, int count)
		{
			object[] array = base.Invoke("EnumMembers", new object[]
			{
				domainName,
				start,
				count
			});
			return (Member[])array[0];
		}

		public IAsyncResult BeginEnumMembers(string domainName, string start, int count, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("EnumMembers", new object[]
			{
				domainName,
				start,
				count
			}, callback, asyncState);
		}

		public Member[] EndEnumMembers(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (Member[])array[0];
		}

		public void EnumMembersAsync(string domainName, string start, int count)
		{
			this.EnumMembersAsync(domainName, start, count, null);
		}

		public void EnumMembersAsync(string domainName, string start, int count, object userState)
		{
			if (this.EnumMembersOperationCompleted == null)
			{
				this.EnumMembersOperationCompleted = new SendOrPostCallback(this.OnEnumMembersOperationCompleted);
			}
			base.InvokeAsync("EnumMembers", new object[]
			{
				domainName,
				start,
				count
			}, this.EnumMembersOperationCompleted, userState);
		}

		private void OnEnumMembersOperationCompleted(object arg)
		{
			if (this.EnumMembersCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.EnumMembersCompleted(this, new EnumMembersCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/CreateMember", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("AdminPassportAuthHeaderValue")]
		[SoapHeader("PartnerAuthHeaderValue")]
		public string CreateMember(string memberNameIn, string password, bool forceResetPassword, [XmlArrayItem(IsNullable = false)] Property[] properties)
		{
			object[] array = base.Invoke("CreateMember", new object[]
			{
				memberNameIn,
				password,
				forceResetPassword,
				properties
			});
			return (string)array[0];
		}

		public IAsyncResult BeginCreateMember(string memberNameIn, string password, bool forceResetPassword, Property[] properties, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("CreateMember", new object[]
			{
				memberNameIn,
				password,
				forceResetPassword,
				properties
			}, callback, asyncState);
		}

		public string EndCreateMember(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (string)array[0];
		}

		public void CreateMemberAsync(string memberNameIn, string password, bool forceResetPassword, Property[] properties)
		{
			this.CreateMemberAsync(memberNameIn, password, forceResetPassword, properties, null);
		}

		public void CreateMemberAsync(string memberNameIn, string password, bool forceResetPassword, Property[] properties, object userState)
		{
			if (this.CreateMemberOperationCompleted == null)
			{
				this.CreateMemberOperationCompleted = new SendOrPostCallback(this.OnCreateMemberOperationCompleted);
			}
			base.InvokeAsync("CreateMember", new object[]
			{
				memberNameIn,
				password,
				forceResetPassword,
				properties
			}, this.CreateMemberOperationCompleted, userState);
		}

		private void OnCreateMemberOperationCompleted(object arg)
		{
			if (this.CreateMemberCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CreateMemberCompleted(this, new CreateMemberCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/CreateMemberEncrypted", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		[SoapHeader("AdminPassportAuthHeaderValue")]
		[SoapHeader("PartnerAuthHeaderValue")]
		public string CreateMemberEncrypted(string memberNameIn, bool forceResetPassword, string SKI, string encryptedProperties, string version, [XmlArrayItem(IsNullable = false)] Property[] properties)
		{
			object[] array = base.Invoke("CreateMemberEncrypted", new object[]
			{
				memberNameIn,
				forceResetPassword,
				SKI,
				encryptedProperties,
				version,
				properties
			});
			return (string)array[0];
		}

		public IAsyncResult BeginCreateMemberEncrypted(string memberNameIn, bool forceResetPassword, string SKI, string encryptedProperties, string version, Property[] properties, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("CreateMemberEncrypted", new object[]
			{
				memberNameIn,
				forceResetPassword,
				SKI,
				encryptedProperties,
				version,
				properties
			}, callback, asyncState);
		}

		public string EndCreateMemberEncrypted(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (string)array[0];
		}

		public void CreateMemberEncryptedAsync(string memberNameIn, bool forceResetPassword, string SKI, string encryptedProperties, string version, Property[] properties)
		{
			this.CreateMemberEncryptedAsync(memberNameIn, forceResetPassword, SKI, encryptedProperties, version, properties, null);
		}

		public void CreateMemberEncryptedAsync(string memberNameIn, bool forceResetPassword, string SKI, string encryptedProperties, string version, Property[] properties, object userState)
		{
			if (this.CreateMemberEncryptedOperationCompleted == null)
			{
				this.CreateMemberEncryptedOperationCompleted = new SendOrPostCallback(this.OnCreateMemberEncryptedOperationCompleted);
			}
			base.InvokeAsync("CreateMemberEncrypted", new object[]
			{
				memberNameIn,
				forceResetPassword,
				SKI,
				encryptedProperties,
				version,
				properties
			}, this.CreateMemberEncryptedOperationCompleted, userState);
		}

		private void OnCreateMemberEncryptedOperationCompleted(object arg)
		{
			if (this.CreateMemberEncryptedCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CreateMemberEncryptedCompleted(this, new CreateMemberEncryptedCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("AdminPassportAuthHeaderValue")]
		[SoapHeader("PartnerAuthHeaderValue")]
		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/CreateMemberEx", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string CreateMemberEx(string memberNameIn, string password, bool forceResetPassword, string sq, string sa, bool resetSQ, string alternateEmail, int wlTOUVersion, bool noHip, bool forceHip, string brandInfo, [XmlArrayItem(IsNullable = false)] Property[] properties, bool needSlt, out string slt)
		{
			object[] array = base.Invoke("CreateMemberEx", new object[]
			{
				memberNameIn,
				password,
				forceResetPassword,
				sq,
				sa,
				resetSQ,
				alternateEmail,
				wlTOUVersion,
				noHip,
				forceHip,
				brandInfo,
				properties,
				needSlt
			});
			slt = (string)array[1];
			return (string)array[0];
		}

		public IAsyncResult BeginCreateMemberEx(string memberNameIn, string password, bool forceResetPassword, string sq, string sa, bool resetSQ, string alternateEmail, int wlTOUVersion, bool noHip, bool forceHip, string brandInfo, Property[] properties, bool needSlt, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("CreateMemberEx", new object[]
			{
				memberNameIn,
				password,
				forceResetPassword,
				sq,
				sa,
				resetSQ,
				alternateEmail,
				wlTOUVersion,
				noHip,
				forceHip,
				brandInfo,
				properties,
				needSlt
			}, callback, asyncState);
		}

		public string EndCreateMemberEx(IAsyncResult asyncResult, out string slt)
		{
			object[] array = base.EndInvoke(asyncResult);
			slt = (string)array[1];
			return (string)array[0];
		}

		public void CreateMemberExAsync(string memberNameIn, string password, bool forceResetPassword, string sq, string sa, bool resetSQ, string alternateEmail, int wlTOUVersion, bool noHip, bool forceHip, string brandInfo, Property[] properties, bool needSlt)
		{
			this.CreateMemberExAsync(memberNameIn, password, forceResetPassword, sq, sa, resetSQ, alternateEmail, wlTOUVersion, noHip, forceHip, brandInfo, properties, needSlt, null);
		}

		public void CreateMemberExAsync(string memberNameIn, string password, bool forceResetPassword, string sq, string sa, bool resetSQ, string alternateEmail, int wlTOUVersion, bool noHip, bool forceHip, string brandInfo, Property[] properties, bool needSlt, object userState)
		{
			if (this.CreateMemberExOperationCompleted == null)
			{
				this.CreateMemberExOperationCompleted = new SendOrPostCallback(this.OnCreateMemberExOperationCompleted);
			}
			base.InvokeAsync("CreateMemberEx", new object[]
			{
				memberNameIn,
				password,
				forceResetPassword,
				sq,
				sa,
				resetSQ,
				alternateEmail,
				wlTOUVersion,
				noHip,
				forceHip,
				brandInfo,
				properties,
				needSlt
			}, this.CreateMemberExOperationCompleted, userState);
		}

		private void OnCreateMemberExOperationCompleted(object arg)
		{
			if (this.CreateMemberExCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CreateMemberExCompleted(this, new CreateMemberExCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		[SoapHeader("AdminPassportAuthHeaderValue")]
		[SoapHeader("PartnerAuthHeaderValue")]
		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/CreateMemberEncryptedEx", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public string CreateMemberEncryptedEx(string memberNameIn, bool forceResetPassword, string sq, bool resetSQ, string alternateEmail, string SKI, string encryptedProperties, string version, int wlTOUVersion, bool noHip, bool forceHip, string brandInfo, [XmlArrayItem(IsNullable = false)] Property[] properties, bool needSlt, out string slt)
		{
			object[] array = base.Invoke("CreateMemberEncryptedEx", new object[]
			{
				memberNameIn,
				forceResetPassword,
				sq,
				resetSQ,
				alternateEmail,
				SKI,
				encryptedProperties,
				version,
				wlTOUVersion,
				noHip,
				forceHip,
				brandInfo,
				properties,
				needSlt
			});
			slt = (string)array[1];
			return (string)array[0];
		}

		public IAsyncResult BeginCreateMemberEncryptedEx(string memberNameIn, bool forceResetPassword, string sq, bool resetSQ, string alternateEmail, string SKI, string encryptedProperties, string version, int wlTOUVersion, bool noHip, bool forceHip, string brandInfo, Property[] properties, bool needSlt, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("CreateMemberEncryptedEx", new object[]
			{
				memberNameIn,
				forceResetPassword,
				sq,
				resetSQ,
				alternateEmail,
				SKI,
				encryptedProperties,
				version,
				wlTOUVersion,
				noHip,
				forceHip,
				brandInfo,
				properties,
				needSlt
			}, callback, asyncState);
		}

		public string EndCreateMemberEncryptedEx(IAsyncResult asyncResult, out string slt)
		{
			object[] array = base.EndInvoke(asyncResult);
			slt = (string)array[1];
			return (string)array[0];
		}

		public void CreateMemberEncryptedExAsync(string memberNameIn, bool forceResetPassword, string sq, bool resetSQ, string alternateEmail, string SKI, string encryptedProperties, string version, int wlTOUVersion, bool noHip, bool forceHip, string brandInfo, Property[] properties, bool needSlt)
		{
			this.CreateMemberEncryptedExAsync(memberNameIn, forceResetPassword, sq, resetSQ, alternateEmail, SKI, encryptedProperties, version, wlTOUVersion, noHip, forceHip, brandInfo, properties, needSlt, null);
		}

		public void CreateMemberEncryptedExAsync(string memberNameIn, bool forceResetPassword, string sq, bool resetSQ, string alternateEmail, string SKI, string encryptedProperties, string version, int wlTOUVersion, bool noHip, bool forceHip, string brandInfo, Property[] properties, bool needSlt, object userState)
		{
			if (this.CreateMemberEncryptedExOperationCompleted == null)
			{
				this.CreateMemberEncryptedExOperationCompleted = new SendOrPostCallback(this.OnCreateMemberEncryptedExOperationCompleted);
			}
			base.InvokeAsync("CreateMemberEncryptedEx", new object[]
			{
				memberNameIn,
				forceResetPassword,
				sq,
				resetSQ,
				alternateEmail,
				SKI,
				encryptedProperties,
				version,
				wlTOUVersion,
				noHip,
				forceHip,
				brandInfo,
				properties,
				needSlt
			}, this.CreateMemberEncryptedExOperationCompleted, userState);
		}

		private void OnCreateMemberEncryptedExOperationCompleted(object arg)
		{
			if (this.CreateMemberEncryptedExCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CreateMemberEncryptedExCompleted(this, new CreateMemberEncryptedExCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		[SoapHeader("AdminPassportAuthHeaderValue")]
		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/AddBrandInfo", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("PartnerAuthHeaderValue")]
		public void AddBrandInfo(string memberNameIn, string brandInfo)
		{
			base.Invoke("AddBrandInfo", new object[]
			{
				memberNameIn,
				brandInfo
			});
		}

		public IAsyncResult BeginAddBrandInfo(string memberNameIn, string brandInfo, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("AddBrandInfo", new object[]
			{
				memberNameIn,
				brandInfo
			}, callback, asyncState);
		}

		public void EndAddBrandInfo(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void AddBrandInfoAsync(string memberNameIn, string brandInfo)
		{
			this.AddBrandInfoAsync(memberNameIn, brandInfo, null);
		}

		public void AddBrandInfoAsync(string memberNameIn, string brandInfo, object userState)
		{
			if (this.AddBrandInfoOperationCompleted == null)
			{
				this.AddBrandInfoOperationCompleted = new SendOrPostCallback(this.OnAddBrandInfoOperationCompleted);
			}
			base.InvokeAsync("AddBrandInfo", new object[]
			{
				memberNameIn,
				brandInfo
			}, this.AddBrandInfoOperationCompleted, userState);
		}

		private void OnAddBrandInfoOperationCompleted(object arg)
		{
			if (this.AddBrandInfoCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.AddBrandInfoCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/RemoveBrandInfo", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		[SoapHeader("PartnerAuthHeaderValue")]
		[SoapHeader("AdminPassportAuthHeaderValue")]
		public void RemoveBrandInfo(string memberNameIn, string brandInfo)
		{
			base.Invoke("RemoveBrandInfo", new object[]
			{
				memberNameIn,
				brandInfo
			});
		}

		public IAsyncResult BeginRemoveBrandInfo(string memberNameIn, string brandInfo, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("RemoveBrandInfo", new object[]
			{
				memberNameIn,
				brandInfo
			}, callback, asyncState);
		}

		public void EndRemoveBrandInfo(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void RemoveBrandInfoAsync(string memberNameIn, string brandInfo)
		{
			this.RemoveBrandInfoAsync(memberNameIn, brandInfo, null);
		}

		public void RemoveBrandInfoAsync(string memberNameIn, string brandInfo, object userState)
		{
			if (this.RemoveBrandInfoOperationCompleted == null)
			{
				this.RemoveBrandInfoOperationCompleted = new SendOrPostCallback(this.OnRemoveBrandInfoOperationCompleted);
			}
			base.InvokeAsync("RemoveBrandInfo", new object[]
			{
				memberNameIn,
				brandInfo
			}, this.RemoveBrandInfoOperationCompleted, userState);
		}

		private void OnRemoveBrandInfoOperationCompleted(object arg)
		{
			if (this.RemoveBrandInfoCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.RemoveBrandInfoCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("PartnerAuthHeaderValue")]
		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/RenameMember", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("AdminPassportAuthHeaderValue")]
		public void RenameMember(string memberNameIn, string memberNameNewIn)
		{
			base.Invoke("RenameMember", new object[]
			{
				memberNameIn,
				memberNameNewIn
			});
		}

		public IAsyncResult BeginRenameMember(string memberNameIn, string memberNameNewIn, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("RenameMember", new object[]
			{
				memberNameIn,
				memberNameNewIn
			}, callback, asyncState);
		}

		public void EndRenameMember(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void RenameMemberAsync(string memberNameIn, string memberNameNewIn)
		{
			this.RenameMemberAsync(memberNameIn, memberNameNewIn, null);
		}

		public void RenameMemberAsync(string memberNameIn, string memberNameNewIn, object userState)
		{
			if (this.RenameMemberOperationCompleted == null)
			{
				this.RenameMemberOperationCompleted = new SendOrPostCallback(this.OnRenameMemberOperationCompleted);
			}
			base.InvokeAsync("RenameMember", new object[]
			{
				memberNameIn,
				memberNameNewIn
			}, this.RenameMemberOperationCompleted, userState);
		}

		private void OnRenameMemberOperationCompleted(object arg)
		{
			if (this.RenameMemberCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.RenameMemberCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/SetMemberProperties", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("AdminPassportAuthHeaderValue")]
		[SoapHeader("PartnerAuthHeaderValue")]
		public void SetMemberProperties(string memberNameIn, [XmlArrayItem(IsNullable = false)] Property[] properties)
		{
			base.Invoke("SetMemberProperties", new object[]
			{
				memberNameIn,
				properties
			});
		}

		public IAsyncResult BeginSetMemberProperties(string memberNameIn, Property[] properties, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("SetMemberProperties", new object[]
			{
				memberNameIn,
				properties
			}, callback, asyncState);
		}

		public void EndSetMemberProperties(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void SetMemberPropertiesAsync(string memberNameIn, Property[] properties)
		{
			this.SetMemberPropertiesAsync(memberNameIn, properties, null);
		}

		public void SetMemberPropertiesAsync(string memberNameIn, Property[] properties, object userState)
		{
			if (this.SetMemberPropertiesOperationCompleted == null)
			{
				this.SetMemberPropertiesOperationCompleted = new SendOrPostCallback(this.OnSetMemberPropertiesOperationCompleted);
			}
			base.InvokeAsync("SetMemberProperties", new object[]
			{
				memberNameIn,
				properties
			}, this.SetMemberPropertiesOperationCompleted, userState);
		}

		private void OnSetMemberPropertiesOperationCompleted(object arg)
		{
			if (this.SetMemberPropertiesCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SetMemberPropertiesCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/GetMemberProperties", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("PartnerAuthHeaderValue")]
		[SoapHeader("AdminPassportAuthHeaderValue")]
		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		[return: XmlArrayItem(IsNullable = false)]
		public Property[] GetMemberProperties(string memberNameIn)
		{
			object[] array = base.Invoke("GetMemberProperties", new object[]
			{
				memberNameIn
			});
			return (Property[])array[0];
		}

		public IAsyncResult BeginGetMemberProperties(string memberNameIn, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetMemberProperties", new object[]
			{
				memberNameIn
			}, callback, asyncState);
		}

		public Property[] EndGetMemberProperties(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (Property[])array[0];
		}

		public void GetMemberPropertiesAsync(string memberNameIn)
		{
			this.GetMemberPropertiesAsync(memberNameIn, null);
		}

		public void GetMemberPropertiesAsync(string memberNameIn, object userState)
		{
			if (this.GetMemberPropertiesOperationCompleted == null)
			{
				this.GetMemberPropertiesOperationCompleted = new SendOrPostCallback(this.OnGetMemberPropertiesOperationCompleted);
			}
			base.InvokeAsync("GetMemberProperties", new object[]
			{
				memberNameIn
			}, this.GetMemberPropertiesOperationCompleted, userState);
		}

		private void OnGetMemberPropertiesOperationCompleted(object arg)
		{
			if (this.GetMemberPropertiesCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetMemberPropertiesCompleted(this, new GetMemberPropertiesCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		[SoapHeader("PartnerAuthHeaderValue")]
		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/EvictMember", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("AdminPassportAuthHeaderValue")]
		public void EvictMember(string memberNameIn)
		{
			base.Invoke("EvictMember", new object[]
			{
				memberNameIn
			});
		}

		public IAsyncResult BeginEvictMember(string memberNameIn, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("EvictMember", new object[]
			{
				memberNameIn
			}, callback, asyncState);
		}

		public void EndEvictMember(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void EvictMemberAsync(string memberNameIn)
		{
			this.EvictMemberAsync(memberNameIn, null);
		}

		public void EvictMemberAsync(string memberNameIn, object userState)
		{
			if (this.EvictMemberOperationCompleted == null)
			{
				this.EvictMemberOperationCompleted = new SendOrPostCallback(this.OnEvictMemberOperationCompleted);
			}
			base.InvokeAsync("EvictMember", new object[]
			{
				memberNameIn
			}, this.EvictMemberOperationCompleted, userState);
		}

		private void OnEvictMemberOperationCompleted(object arg)
		{
			if (this.EvictMemberCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.EvictMemberCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/ResetMemberPassword", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("AdminPassportAuthHeaderValue")]
		[SoapHeader("PartnerAuthHeaderValue")]
		public void ResetMemberPassword(string memberNameIn, string newPassword, bool forceResetPassword)
		{
			base.Invoke("ResetMemberPassword", new object[]
			{
				memberNameIn,
				newPassword,
				forceResetPassword
			});
		}

		public IAsyncResult BeginResetMemberPassword(string memberNameIn, string newPassword, bool forceResetPassword, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("ResetMemberPassword", new object[]
			{
				memberNameIn,
				newPassword,
				forceResetPassword
			}, callback, asyncState);
		}

		public void EndResetMemberPassword(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void ResetMemberPasswordAsync(string memberNameIn, string newPassword, bool forceResetPassword)
		{
			this.ResetMemberPasswordAsync(memberNameIn, newPassword, forceResetPassword, null);
		}

		public void ResetMemberPasswordAsync(string memberNameIn, string newPassword, bool forceResetPassword, object userState)
		{
			if (this.ResetMemberPasswordOperationCompleted == null)
			{
				this.ResetMemberPasswordOperationCompleted = new SendOrPostCallback(this.OnResetMemberPasswordOperationCompleted);
			}
			base.InvokeAsync("ResetMemberPassword", new object[]
			{
				memberNameIn,
				newPassword,
				forceResetPassword
			}, this.ResetMemberPasswordOperationCompleted, userState);
		}

		private void OnResetMemberPasswordOperationCompleted(object arg)
		{
			if (this.ResetMemberPasswordCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ResetMemberPasswordCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/ResetMemberPasswordEncrypted", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("AdminPassportAuthHeaderValue")]
		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		[SoapHeader("PartnerAuthHeaderValue")]
		public void ResetMemberPasswordEncrypted(string memberNameIn, bool forceResetPassword, string SKI, string encryptedProperties, string version)
		{
			base.Invoke("ResetMemberPasswordEncrypted", new object[]
			{
				memberNameIn,
				forceResetPassword,
				SKI,
				encryptedProperties,
				version
			});
		}

		public IAsyncResult BeginResetMemberPasswordEncrypted(string memberNameIn, bool forceResetPassword, string SKI, string encryptedProperties, string version, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("ResetMemberPasswordEncrypted", new object[]
			{
				memberNameIn,
				forceResetPassword,
				SKI,
				encryptedProperties,
				version
			}, callback, asyncState);
		}

		public void EndResetMemberPasswordEncrypted(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void ResetMemberPasswordEncryptedAsync(string memberNameIn, bool forceResetPassword, string SKI, string encryptedProperties, string version)
		{
			this.ResetMemberPasswordEncryptedAsync(memberNameIn, forceResetPassword, SKI, encryptedProperties, version, null);
		}

		public void ResetMemberPasswordEncryptedAsync(string memberNameIn, bool forceResetPassword, string SKI, string encryptedProperties, string version, object userState)
		{
			if (this.ResetMemberPasswordEncryptedOperationCompleted == null)
			{
				this.ResetMemberPasswordEncryptedOperationCompleted = new SendOrPostCallback(this.OnResetMemberPasswordEncryptedOperationCompleted);
			}
			base.InvokeAsync("ResetMemberPasswordEncrypted", new object[]
			{
				memberNameIn,
				forceResetPassword,
				SKI,
				encryptedProperties,
				version
			}, this.ResetMemberPasswordEncryptedOperationCompleted, userState);
		}

		private void OnResetMemberPasswordEncryptedOperationCompleted(object arg)
		{
			if (this.ResetMemberPasswordEncryptedCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ResetMemberPasswordEncryptedCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("AdminPassportAuthHeaderValue")]
		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		[SoapHeader("PartnerAuthHeaderValue")]
		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/BlockMemberEmail", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void BlockMemberEmail(string memberNameIn, bool isBlocked)
		{
			base.Invoke("BlockMemberEmail", new object[]
			{
				memberNameIn,
				isBlocked
			});
		}

		public IAsyncResult BeginBlockMemberEmail(string memberNameIn, bool isBlocked, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("BlockMemberEmail", new object[]
			{
				memberNameIn,
				isBlocked
			}, callback, asyncState);
		}

		public void EndBlockMemberEmail(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void BlockMemberEmailAsync(string memberNameIn, bool isBlocked)
		{
			this.BlockMemberEmailAsync(memberNameIn, isBlocked, null);
		}

		public void BlockMemberEmailAsync(string memberNameIn, bool isBlocked, object userState)
		{
			if (this.BlockMemberEmailOperationCompleted == null)
			{
				this.BlockMemberEmailOperationCompleted = new SendOrPostCallback(this.OnBlockMemberEmailOperationCompleted);
			}
			base.InvokeAsync("BlockMemberEmail", new object[]
			{
				memberNameIn,
				isBlocked
			}, this.BlockMemberEmailOperationCompleted, userState);
		}

		private void OnBlockMemberEmailOperationCompleted(object arg)
		{
			if (this.BlockMemberEmailCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.BlockMemberEmailCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("PartnerAuthHeaderValue")]
		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/ImportUnmanagedMember", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("AdminPassportAuthHeaderValue")]
		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		public void ImportUnmanagedMember(string memberNameIn)
		{
			base.Invoke("ImportUnmanagedMember", new object[]
			{
				memberNameIn
			});
		}

		public IAsyncResult BeginImportUnmanagedMember(string memberNameIn, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("ImportUnmanagedMember", new object[]
			{
				memberNameIn
			}, callback, asyncState);
		}

		public void EndImportUnmanagedMember(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void ImportUnmanagedMemberAsync(string memberNameIn)
		{
			this.ImportUnmanagedMemberAsync(memberNameIn, null);
		}

		public void ImportUnmanagedMemberAsync(string memberNameIn, object userState)
		{
			if (this.ImportUnmanagedMemberOperationCompleted == null)
			{
				this.ImportUnmanagedMemberOperationCompleted = new SendOrPostCallback(this.OnImportUnmanagedMemberOperationCompleted);
			}
			base.InvokeAsync("ImportUnmanagedMember", new object[]
			{
				memberNameIn
			}, this.ImportUnmanagedMemberOperationCompleted, userState);
		}

		private void OnImportUnmanagedMemberOperationCompleted(object arg)
		{
			if (this.ImportUnmanagedMemberCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ImportUnmanagedMemberCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/EvictUnmanagedMember", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		[SoapHeader("AdminPassportAuthHeaderValue")]
		[SoapHeader("PartnerAuthHeaderValue")]
		public void EvictUnmanagedMember(string memberNameIn)
		{
			base.Invoke("EvictUnmanagedMember", new object[]
			{
				memberNameIn
			});
		}

		public IAsyncResult BeginEvictUnmanagedMember(string memberNameIn, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("EvictUnmanagedMember", new object[]
			{
				memberNameIn
			}, callback, asyncState);
		}

		public void EndEvictUnmanagedMember(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void EvictUnmanagedMemberAsync(string memberNameIn)
		{
			this.EvictUnmanagedMemberAsync(memberNameIn, null);
		}

		public void EvictUnmanagedMemberAsync(string memberNameIn, object userState)
		{
			if (this.EvictUnmanagedMemberOperationCompleted == null)
			{
				this.EvictUnmanagedMemberOperationCompleted = new SendOrPostCallback(this.OnEvictUnmanagedMemberOperationCompleted);
			}
			base.InvokeAsync("EvictUnmanagedMember", new object[]
			{
				memberNameIn
			}, this.EvictUnmanagedMemberOperationCompleted, userState);
		}

		private void OnEvictUnmanagedMemberOperationCompleted(object arg)
		{
			if (this.EvictUnmanagedMemberCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.EvictUnmanagedMemberCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("AdminPassportAuthHeaderValue")]
		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/EvictAllUnmanagedMembers", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		[SoapHeader("PartnerAuthHeaderValue")]
		public void EvictAllUnmanagedMembers(string domainName)
		{
			base.Invoke("EvictAllUnmanagedMembers", new object[]
			{
				domainName
			});
		}

		public IAsyncResult BeginEvictAllUnmanagedMembers(string domainName, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("EvictAllUnmanagedMembers", new object[]
			{
				domainName
			}, callback, asyncState);
		}

		public void EndEvictAllUnmanagedMembers(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void EvictAllUnmanagedMembersAsync(string domainName)
		{
			this.EvictAllUnmanagedMembersAsync(domainName, null);
		}

		public void EvictAllUnmanagedMembersAsync(string domainName, object userState)
		{
			if (this.EvictAllUnmanagedMembersOperationCompleted == null)
			{
				this.EvictAllUnmanagedMembersOperationCompleted = new SendOrPostCallback(this.OnEvictAllUnmanagedMembersOperationCompleted);
			}
			base.InvokeAsync("EvictAllUnmanagedMembers", new object[]
			{
				domainName
			}, this.EvictAllUnmanagedMembersOperationCompleted, userState);
		}

		private void OnEvictAllUnmanagedMembersOperationCompleted(object arg)
		{
			if (this.EvictAllUnmanagedMembersCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.EvictAllUnmanagedMembersCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("AdminPassportAuthHeaderValue")]
		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		[SoapHeader("PartnerAuthHeaderValue")]
		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/EnableOpenMembership", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void EnableOpenMembership(string domainName, bool isEnabled)
		{
			base.Invoke("EnableOpenMembership", new object[]
			{
				domainName,
				isEnabled
			});
		}

		public IAsyncResult BeginEnableOpenMembership(string domainName, bool isEnabled, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("EnableOpenMembership", new object[]
			{
				domainName,
				isEnabled
			}, callback, asyncState);
		}

		public void EndEnableOpenMembership(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void EnableOpenMembershipAsync(string domainName, bool isEnabled)
		{
			this.EnableOpenMembershipAsync(domainName, isEnabled, null);
		}

		public void EnableOpenMembershipAsync(string domainName, bool isEnabled, object userState)
		{
			if (this.EnableOpenMembershipOperationCompleted == null)
			{
				this.EnableOpenMembershipOperationCompleted = new SendOrPostCallback(this.OnEnableOpenMembershipOperationCompleted);
			}
			base.InvokeAsync("EnableOpenMembership", new object[]
			{
				domainName,
				isEnabled
			}, this.EnableOpenMembershipOperationCompleted, userState);
		}

		private void OnEnableOpenMembershipOperationCompleted(object arg)
		{
			if (this.EnableOpenMembershipCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.EnableOpenMembershipCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		[SoapHeader("AdminPassportAuthHeaderValue")]
		[SoapHeader("PartnerAuthHeaderValue")]
		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/ProvisionMemberSubscription", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void ProvisionMemberSubscription(string memberNameIn, string offerName)
		{
			base.Invoke("ProvisionMemberSubscription", new object[]
			{
				memberNameIn,
				offerName
			});
		}

		public IAsyncResult BeginProvisionMemberSubscription(string memberNameIn, string offerName, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("ProvisionMemberSubscription", new object[]
			{
				memberNameIn,
				offerName
			}, callback, asyncState);
		}

		public void EndProvisionMemberSubscription(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void ProvisionMemberSubscriptionAsync(string memberNameIn, string offerName)
		{
			this.ProvisionMemberSubscriptionAsync(memberNameIn, offerName, null);
		}

		public void ProvisionMemberSubscriptionAsync(string memberNameIn, string offerName, object userState)
		{
			if (this.ProvisionMemberSubscriptionOperationCompleted == null)
			{
				this.ProvisionMemberSubscriptionOperationCompleted = new SendOrPostCallback(this.OnProvisionMemberSubscriptionOperationCompleted);
			}
			base.InvokeAsync("ProvisionMemberSubscription", new object[]
			{
				memberNameIn,
				offerName
			}, this.ProvisionMemberSubscriptionOperationCompleted, userState);
		}

		private void OnProvisionMemberSubscriptionOperationCompleted(object arg)
		{
			if (this.ProvisionMemberSubscriptionCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ProvisionMemberSubscriptionCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("AdminPassportAuthHeaderValue")]
		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/DeprovisionMemberSubscription", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("PartnerAuthHeaderValue")]
		public void DeprovisionMemberSubscription(string memberNameIn, string offerName)
		{
			base.Invoke("DeprovisionMemberSubscription", new object[]
			{
				memberNameIn,
				offerName
			});
		}

		public IAsyncResult BeginDeprovisionMemberSubscription(string memberNameIn, string offerName, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("DeprovisionMemberSubscription", new object[]
			{
				memberNameIn,
				offerName
			}, callback, asyncState);
		}

		public void EndDeprovisionMemberSubscription(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void DeprovisionMemberSubscriptionAsync(string memberNameIn, string offerName)
		{
			this.DeprovisionMemberSubscriptionAsync(memberNameIn, offerName, null);
		}

		public void DeprovisionMemberSubscriptionAsync(string memberNameIn, string offerName, object userState)
		{
			if (this.DeprovisionMemberSubscriptionOperationCompleted == null)
			{
				this.DeprovisionMemberSubscriptionOperationCompleted = new SendOrPostCallback(this.OnDeprovisionMemberSubscriptionOperationCompleted);
			}
			base.InvokeAsync("DeprovisionMemberSubscription", new object[]
			{
				memberNameIn,
				offerName
			}, this.DeprovisionMemberSubscriptionOperationCompleted, userState);
		}

		private void OnDeprovisionMemberSubscriptionOperationCompleted(object arg)
		{
			if (this.DeprovisionMemberSubscriptionCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.DeprovisionMemberSubscriptionCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/ConvertMemberSubscription", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("AdminPassportAuthHeaderValue")]
		[SoapHeader("PartnerAuthHeaderValue")]
		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		public void ConvertMemberSubscription(string memberNameIn, string offerNameOld, string offerNameNew)
		{
			base.Invoke("ConvertMemberSubscription", new object[]
			{
				memberNameIn,
				offerNameOld,
				offerNameNew
			});
		}

		public IAsyncResult BeginConvertMemberSubscription(string memberNameIn, string offerNameOld, string offerNameNew, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("ConvertMemberSubscription", new object[]
			{
				memberNameIn,
				offerNameOld,
				offerNameNew
			}, callback, asyncState);
		}

		public void EndConvertMemberSubscription(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void ConvertMemberSubscriptionAsync(string memberNameIn, string offerNameOld, string offerNameNew)
		{
			this.ConvertMemberSubscriptionAsync(memberNameIn, offerNameOld, offerNameNew, null);
		}

		public void ConvertMemberSubscriptionAsync(string memberNameIn, string offerNameOld, string offerNameNew, object userState)
		{
			if (this.ConvertMemberSubscriptionOperationCompleted == null)
			{
				this.ConvertMemberSubscriptionOperationCompleted = new SendOrPostCallback(this.OnConvertMemberSubscriptionOperationCompleted);
			}
			base.InvokeAsync("ConvertMemberSubscription", new object[]
			{
				memberNameIn,
				offerNameOld,
				offerNameNew
			}, this.ConvertMemberSubscriptionOperationCompleted, userState);
		}

		private void OnConvertMemberSubscriptionOperationCompleted(object arg)
		{
			if (this.ConvertMemberSubscriptionCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ConvertMemberSubscriptionCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/MemberHasSubscription", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("AdminPassportAuthHeaderValue")]
		[SoapHeader("PartnerAuthHeaderValue")]
		public bool MemberHasSubscription(string memberNameIn, string offerName)
		{
			object[] array = base.Invoke("MemberHasSubscription", new object[]
			{
				memberNameIn,
				offerName
			});
			return (bool)array[0];
		}

		public IAsyncResult BeginMemberHasSubscription(string memberNameIn, string offerName, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("MemberHasSubscription", new object[]
			{
				memberNameIn,
				offerName
			}, callback, asyncState);
		}

		public bool EndMemberHasSubscription(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (bool)array[0];
		}

		public void MemberHasSubscriptionAsync(string memberNameIn, string offerName)
		{
			this.MemberHasSubscriptionAsync(memberNameIn, offerName, null);
		}

		public void MemberHasSubscriptionAsync(string memberNameIn, string offerName, object userState)
		{
			if (this.MemberHasSubscriptionOperationCompleted == null)
			{
				this.MemberHasSubscriptionOperationCompleted = new SendOrPostCallback(this.OnMemberHasSubscriptionOperationCompleted);
			}
			base.InvokeAsync("MemberHasSubscription", new object[]
			{
				memberNameIn,
				offerName
			}, this.MemberHasSubscriptionOperationCompleted, userState);
		}

		private void OnMemberHasSubscriptionOperationCompleted(object arg)
		{
			if (this.MemberHasSubscriptionCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.MemberHasSubscriptionCompleted(this, new MemberHasSubscriptionCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/SuspendEmail", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		[SoapHeader("AdminPassportAuthHeaderValue")]
		[SoapHeader("PartnerAuthHeaderValue")]
		public void SuspendEmail(string domainName, bool isSuspend)
		{
			base.Invoke("SuspendEmail", new object[]
			{
				domainName,
				isSuspend
			});
		}

		public IAsyncResult BeginSuspendEmail(string domainName, bool isSuspend, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("SuspendEmail", new object[]
			{
				domainName,
				isSuspend
			}, callback, asyncState);
		}

		public void EndSuspendEmail(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void SuspendEmailAsync(string domainName, bool isSuspend)
		{
			this.SuspendEmailAsync(domainName, isSuspend, null);
		}

		public void SuspendEmailAsync(string domainName, bool isSuspend, object userState)
		{
			if (this.SuspendEmailOperationCompleted == null)
			{
				this.SuspendEmailOperationCompleted = new SendOrPostCallback(this.OnSuspendEmailOperationCompleted);
			}
			base.InvokeAsync("SuspendEmail", new object[]
			{
				domainName,
				isSuspend
			}, this.SuspendEmailOperationCompleted, userState);
		}

		private void OnSuspendEmailOperationCompleted(object arg)
		{
			if (this.SuspendEmailCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SuspendEmailCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/GetMxRecords", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("AdminPassportAuthHeaderValue")]
		[SoapHeader("PartnerAuthHeaderValue")]
		public string[] GetMxRecords(string domainName)
		{
			object[] array = base.Invoke("GetMxRecords", new object[]
			{
				domainName
			});
			return (string[])array[0];
		}

		public IAsyncResult BeginGetMxRecords(string domainName, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetMxRecords", new object[]
			{
				domainName
			}, callback, asyncState);
		}

		public string[] EndGetMxRecords(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (string[])array[0];
		}

		public void GetMxRecordsAsync(string domainName)
		{
			this.GetMxRecordsAsync(domainName, null);
		}

		public void GetMxRecordsAsync(string domainName, object userState)
		{
			if (this.GetMxRecordsOperationCompleted == null)
			{
				this.GetMxRecordsOperationCompleted = new SendOrPostCallback(this.OnGetMxRecordsOperationCompleted);
			}
			base.InvokeAsync("GetMxRecords", new object[]
			{
				domainName
			}, this.GetMxRecordsOperationCompleted, userState);
		}

		private void OnGetMxRecordsOperationCompleted(object arg)
		{
			if (this.GetMxRecordsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetMxRecordsCompleted(this, new GetMxRecordsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("PartnerAuthHeaderValue")]
		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/MemberHasMailbox", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public bool MemberHasMailbox(string memberNameIn, EmailType emailType)
		{
			object[] array = base.Invoke("MemberHasMailbox", new object[]
			{
				memberNameIn,
				emailType
			});
			return (bool)array[0];
		}

		public IAsyncResult BeginMemberHasMailbox(string memberNameIn, EmailType emailType, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("MemberHasMailbox", new object[]
			{
				memberNameIn,
				emailType
			}, callback, asyncState);
		}

		public bool EndMemberHasMailbox(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (bool)array[0];
		}

		public void MemberHasMailboxAsync(string memberNameIn, EmailType emailType)
		{
			this.MemberHasMailboxAsync(memberNameIn, emailType, null);
		}

		public void MemberHasMailboxAsync(string memberNameIn, EmailType emailType, object userState)
		{
			if (this.MemberHasMailboxOperationCompleted == null)
			{
				this.MemberHasMailboxOperationCompleted = new SendOrPostCallback(this.OnMemberHasMailboxOperationCompleted);
			}
			base.InvokeAsync("MemberHasMailbox", new object[]
			{
				memberNameIn,
				emailType
			}, this.MemberHasMailboxOperationCompleted, userState);
		}

		private void OnMemberHasMailboxOperationCompleted(object arg)
		{
			if (this.MemberHasMailboxCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.MemberHasMailboxCompleted(this, new MemberHasMailboxCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("PartnerAuthHeaderValue")]
		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/CompleteMemberEmailMigration", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void CompleteMemberEmailMigration(string memberNameIn)
		{
			base.Invoke("CompleteMemberEmailMigration", new object[]
			{
				memberNameIn
			});
		}

		public IAsyncResult BeginCompleteMemberEmailMigration(string memberNameIn, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("CompleteMemberEmailMigration", new object[]
			{
				memberNameIn
			}, callback, asyncState);
		}

		public void EndCompleteMemberEmailMigration(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void CompleteMemberEmailMigrationAsync(string memberNameIn)
		{
			this.CompleteMemberEmailMigrationAsync(memberNameIn, null);
		}

		public void CompleteMemberEmailMigrationAsync(string memberNameIn, object userState)
		{
			if (this.CompleteMemberEmailMigrationOperationCompleted == null)
			{
				this.CompleteMemberEmailMigrationOperationCompleted = new SendOrPostCallback(this.OnCompleteMemberEmailMigrationOperationCompleted);
			}
			base.InvokeAsync("CompleteMemberEmailMigration", new object[]
			{
				memberNameIn
			}, this.CompleteMemberEmailMigrationOperationCompleted, userState);
		}

		private void OnCompleteMemberEmailMigrationOperationCompleted(object arg)
		{
			if (this.CompleteMemberEmailMigrationCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CompleteMemberEmailMigrationCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/CompleteDomainEmailMigration", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("PartnerAuthHeaderValue")]
		public void CompleteDomainEmailMigration(string domainName)
		{
			base.Invoke("CompleteDomainEmailMigration", new object[]
			{
				domainName
			});
		}

		public IAsyncResult BeginCompleteDomainEmailMigration(string domainName, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("CompleteDomainEmailMigration", new object[]
			{
				domainName
			}, callback, asyncState);
		}

		public void EndCompleteDomainEmailMigration(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void CompleteDomainEmailMigrationAsync(string domainName)
		{
			this.CompleteDomainEmailMigrationAsync(domainName, null);
		}

		public void CompleteDomainEmailMigrationAsync(string domainName, object userState)
		{
			if (this.CompleteDomainEmailMigrationOperationCompleted == null)
			{
				this.CompleteDomainEmailMigrationOperationCompleted = new SendOrPostCallback(this.OnCompleteDomainEmailMigrationOperationCompleted);
			}
			base.InvokeAsync("CompleteDomainEmailMigration", new object[]
			{
				domainName
			}, this.CompleteDomainEmailMigrationOperationCompleted, userState);
		}

		private void OnCompleteDomainEmailMigrationOperationCompleted(object arg)
		{
			if (this.CompleteDomainEmailMigrationCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CompleteDomainEmailMigrationCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("PartnerAuthHeaderValue")]
		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/CreateByodRequest", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public void CreateByodRequest(string domainName, string domainConfigId, string netId)
		{
			base.Invoke("CreateByodRequest", new object[]
			{
				domainName,
				domainConfigId,
				netId
			});
		}

		public IAsyncResult BeginCreateByodRequest(string domainName, string domainConfigId, string netId, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("CreateByodRequest", new object[]
			{
				domainName,
				domainConfigId,
				netId
			}, callback, asyncState);
		}

		public void EndCreateByodRequest(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void CreateByodRequestAsync(string domainName, string domainConfigId, string netId)
		{
			this.CreateByodRequestAsync(domainName, domainConfigId, netId, null);
		}

		public void CreateByodRequestAsync(string domainName, string domainConfigId, string netId, object userState)
		{
			if (this.CreateByodRequestOperationCompleted == null)
			{
				this.CreateByodRequestOperationCompleted = new SendOrPostCallback(this.OnCreateByodRequestOperationCompleted);
			}
			base.InvokeAsync("CreateByodRequest", new object[]
			{
				domainName,
				domainConfigId,
				netId
			}, this.CreateByodRequestOperationCompleted, userState);
		}

		private void OnCreateByodRequestOperationCompleted(object arg)
		{
			if (this.CreateByodRequestCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CreateByodRequestCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("PartnerAuthHeaderValue")]
		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/EnumByodDomains", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public DomainInfoEx[] EnumByodDomains(string netId)
		{
			object[] array = base.Invoke("EnumByodDomains", new object[]
			{
				netId
			});
			return (DomainInfoEx[])array[0];
		}

		public IAsyncResult BeginEnumByodDomains(string netId, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("EnumByodDomains", new object[]
			{
				netId
			}, callback, asyncState);
		}

		public DomainInfoEx[] EndEnumByodDomains(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (DomainInfoEx[])array[0];
		}

		public void EnumByodDomainsAsync(string netId)
		{
			this.EnumByodDomainsAsync(netId, null);
		}

		public void EnumByodDomainsAsync(string netId, object userState)
		{
			if (this.EnumByodDomainsOperationCompleted == null)
			{
				this.EnumByodDomainsOperationCompleted = new SendOrPostCallback(this.OnEnumByodDomainsOperationCompleted);
			}
			base.InvokeAsync("EnumByodDomains", new object[]
			{
				netId
			}, this.EnumByodDomainsOperationCompleted, userState);
		}

		private void OnEnumByodDomainsOperationCompleted(object arg)
		{
			if (this.EnumByodDomainsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.EnumByodDomainsCompleted(this, new EnumByodDomainsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/GetAdmins", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("AdminPassportAuthHeaderValue")]
		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		[SoapHeader("PartnerAuthHeaderValue")]
		public Admin[] GetAdmins(string domainName)
		{
			object[] array = base.Invoke("GetAdmins", new object[]
			{
				domainName
			});
			return (Admin[])array[0];
		}

		public IAsyncResult BeginGetAdmins(string domainName, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetAdmins", new object[]
			{
				domainName
			}, callback, asyncState);
		}

		public Admin[] EndGetAdmins(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (Admin[])array[0];
		}

		public void GetAdminsAsync(string domainName)
		{
			this.GetAdminsAsync(domainName, null);
		}

		public void GetAdminsAsync(string domainName, object userState)
		{
			if (this.GetAdminsOperationCompleted == null)
			{
				this.GetAdminsOperationCompleted = new SendOrPostCallback(this.OnGetAdminsOperationCompleted);
			}
			base.InvokeAsync("GetAdmins", new object[]
			{
				domainName
			}, this.GetAdminsOperationCompleted, userState);
		}

		private void OnGetAdminsOperationCompleted(object arg)
		{
			if (this.GetAdminsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetAdminsCompleted(this, new GetAdminsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/GetManagementCertificate", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		[SoapHeader("AdminPassportAuthHeaderValue")]
		[SoapHeader("PartnerAuthHeaderValue")]
		public CertData GetManagementCertificate(string domainName)
		{
			object[] array = base.Invoke("GetManagementCertificate", new object[]
			{
				domainName
			});
			return (CertData)array[0];
		}

		public IAsyncResult BeginGetManagementCertificate(string domainName, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetManagementCertificate", new object[]
			{
				domainName
			}, callback, asyncState);
		}

		public CertData EndGetManagementCertificate(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (CertData)array[0];
		}

		public void GetManagementCertificateAsync(string domainName)
		{
			this.GetManagementCertificateAsync(domainName, null);
		}

		public void GetManagementCertificateAsync(string domainName, object userState)
		{
			if (this.GetManagementCertificateOperationCompleted == null)
			{
				this.GetManagementCertificateOperationCompleted = new SendOrPostCallback(this.OnGetManagementCertificateOperationCompleted);
			}
			base.InvokeAsync("GetManagementCertificate", new object[]
			{
				domainName
			}, this.GetManagementCertificateOperationCompleted, userState);
		}

		private void OnGetManagementCertificateOperationCompleted(object arg)
		{
			if (this.GetManagementCertificateCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetManagementCertificateCompleted(this, new GetManagementCertificateCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/SetManagementCertificate", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("PartnerAuthHeaderValue")]
		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		[SoapHeader("AdminPassportAuthHeaderValue")]
		public void SetManagementCertificate(string domainName, CertData managementCertificate)
		{
			base.Invoke("SetManagementCertificate", new object[]
			{
				domainName,
				managementCertificate
			});
		}

		public IAsyncResult BeginSetManagementCertificate(string domainName, CertData managementCertificate, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("SetManagementCertificate", new object[]
			{
				domainName,
				managementCertificate
			}, callback, asyncState);
		}

		public void EndSetManagementCertificate(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void SetManagementCertificateAsync(string domainName, CertData managementCertificate)
		{
			this.SetManagementCertificateAsync(domainName, managementCertificate, null);
		}

		public void SetManagementCertificateAsync(string domainName, CertData managementCertificate, object userState)
		{
			if (this.SetManagementCertificateOperationCompleted == null)
			{
				this.SetManagementCertificateOperationCompleted = new SendOrPostCallback(this.OnSetManagementCertificateOperationCompleted);
			}
			base.InvokeAsync("SetManagementCertificate", new object[]
			{
				domainName,
				managementCertificate
			}, this.SetManagementCertificateOperationCompleted, userState);
		}

		private void OnSetManagementCertificateOperationCompleted(object arg)
		{
			if (this.SetManagementCertificateCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SetManagementCertificateCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/SetManagementPermissions", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("PartnerAuthHeaderValue")]
		public void SetManagementPermissions(PermissionFlags flags)
		{
			base.Invoke("SetManagementPermissions", new object[]
			{
				flags
			});
		}

		public IAsyncResult BeginSetManagementPermissions(PermissionFlags flags, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("SetManagementPermissions", new object[]
			{
				flags
			}, callback, asyncState);
		}

		public void EndSetManagementPermissions(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void SetManagementPermissionsAsync(PermissionFlags flags)
		{
			this.SetManagementPermissionsAsync(flags, null);
		}

		public void SetManagementPermissionsAsync(PermissionFlags flags, object userState)
		{
			if (this.SetManagementPermissionsOperationCompleted == null)
			{
				this.SetManagementPermissionsOperationCompleted = new SendOrPostCallback(this.OnSetManagementPermissionsOperationCompleted);
			}
			base.InvokeAsync("SetManagementPermissions", new object[]
			{
				flags
			}, this.SetManagementPermissionsOperationCompleted, userState);
		}

		private void OnSetManagementPermissionsOperationCompleted(object arg)
		{
			if (this.SetManagementPermissionsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SetManagementPermissionsCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("AdminPassportAuthHeaderValue")]
		[SoapHeader("PartnerAuthHeaderValue")]
		[SoapHeader("ManagementCertificateAuthHeaderValue")]
		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/GetMaxMembers", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		public int GetMaxMembers(string domainName)
		{
			object[] array = base.Invoke("GetMaxMembers", new object[]
			{
				domainName
			});
			return (int)array[0];
		}

		public IAsyncResult BeginGetMaxMembers(string domainName, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetMaxMembers", new object[]
			{
				domainName
			}, callback, asyncState);
		}

		public int EndGetMaxMembers(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (int)array[0];
		}

		public void GetMaxMembersAsync(string domainName)
		{
			this.GetMaxMembersAsync(domainName, null);
		}

		public void GetMaxMembersAsync(string domainName, object userState)
		{
			if (this.GetMaxMembersOperationCompleted == null)
			{
				this.GetMaxMembersOperationCompleted = new SendOrPostCallback(this.OnGetMaxMembersOperationCompleted);
			}
			base.InvokeAsync("GetMaxMembers", new object[]
			{
				domainName
			}, this.GetMaxMembersOperationCompleted, userState);
		}

		private void OnGetMaxMembersOperationCompleted(object arg)
		{
			if (this.GetMaxMembersCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetMaxMembersCompleted(this, new GetMaxMembersCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://domains.live.com/Service/DomainServices/V1.0/SetMaxMembers", RequestNamespace = "http://domains.live.com/Service/DomainServices/V1.0", ResponseNamespace = "http://domains.live.com/Service/DomainServices/V1.0", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
		[SoapHeader("PartnerAuthHeaderValue")]
		public void SetMaxMembers(string domainName, int maxUsers)
		{
			base.Invoke("SetMaxMembers", new object[]
			{
				domainName,
				maxUsers
			});
		}

		public IAsyncResult BeginSetMaxMembers(string domainName, int maxUsers, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("SetMaxMembers", new object[]
			{
				domainName,
				maxUsers
			}, callback, asyncState);
		}

		public void EndSetMaxMembers(IAsyncResult asyncResult)
		{
			base.EndInvoke(asyncResult);
		}

		public void SetMaxMembersAsync(string domainName, int maxUsers)
		{
			this.SetMaxMembersAsync(domainName, maxUsers, null);
		}

		public void SetMaxMembersAsync(string domainName, int maxUsers, object userState)
		{
			if (this.SetMaxMembersOperationCompleted == null)
			{
				this.SetMaxMembersOperationCompleted = new SendOrPostCallback(this.OnSetMaxMembersOperationCompleted);
			}
			base.InvokeAsync("SetMaxMembers", new object[]
			{
				domainName,
				maxUsers
			}, this.SetMaxMembersOperationCompleted, userState);
		}

		private void OnSetMaxMembersOperationCompleted(object arg)
		{
			if (this.SetMaxMembersCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SetMaxMembersCompleted(this, new AsyncCompletedEventArgs(invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		public new void CancelAsync(object userState)
		{
			base.CancelAsync(userState);
		}

		private AdminPassportAuthHeader adminPassportAuthHeaderValueField;

		private ManagementCertificateAuthHeader managementCertificateAuthHeaderValueField;

		private PartnerAuthHeader partnerAuthHeaderValueField;

		private SendOrPostCallback TestConnectionOperationCompleted;

		private SendOrPostCallback GetDomainAvailabilityOperationCompleted;

		private SendOrPostCallback GetDomainInfoOperationCompleted;

		private SendOrPostCallback GetDomainInfoExOperationCompleted;

		private SendOrPostCallback ReserveDomainOperationCompleted;

		private SendOrPostCallback ReleaseDomainOperationCompleted;

		private SendOrPostCallback ProcessDomainOperationCompleted;

		private SendOrPostCallback GetMemberTypeOperationCompleted;

		private SendOrPostCallback MemberNameToNetIdOperationCompleted;

		private SendOrPostCallback NetIdToMemberNameOperationCompleted;

		private SendOrPostCallback GetCountMembersOperationCompleted;

		private SendOrPostCallback EnumMembersOperationCompleted;

		private SendOrPostCallback CreateMemberOperationCompleted;

		private SendOrPostCallback CreateMemberEncryptedOperationCompleted;

		private SendOrPostCallback CreateMemberExOperationCompleted;

		private SendOrPostCallback CreateMemberEncryptedExOperationCompleted;

		private SendOrPostCallback AddBrandInfoOperationCompleted;

		private SendOrPostCallback RemoveBrandInfoOperationCompleted;

		private SendOrPostCallback RenameMemberOperationCompleted;

		private SendOrPostCallback SetMemberPropertiesOperationCompleted;

		private SendOrPostCallback GetMemberPropertiesOperationCompleted;

		private SendOrPostCallback EvictMemberOperationCompleted;

		private SendOrPostCallback ResetMemberPasswordOperationCompleted;

		private SendOrPostCallback ResetMemberPasswordEncryptedOperationCompleted;

		private SendOrPostCallback BlockMemberEmailOperationCompleted;

		private SendOrPostCallback ImportUnmanagedMemberOperationCompleted;

		private SendOrPostCallback EvictUnmanagedMemberOperationCompleted;

		private SendOrPostCallback EvictAllUnmanagedMembersOperationCompleted;

		private SendOrPostCallback EnableOpenMembershipOperationCompleted;

		private SendOrPostCallback ProvisionMemberSubscriptionOperationCompleted;

		private SendOrPostCallback DeprovisionMemberSubscriptionOperationCompleted;

		private SendOrPostCallback ConvertMemberSubscriptionOperationCompleted;

		private SendOrPostCallback MemberHasSubscriptionOperationCompleted;

		private SendOrPostCallback SuspendEmailOperationCompleted;

		private SendOrPostCallback GetMxRecordsOperationCompleted;

		private SendOrPostCallback MemberHasMailboxOperationCompleted;

		private SendOrPostCallback CompleteMemberEmailMigrationOperationCompleted;

		private SendOrPostCallback CompleteDomainEmailMigrationOperationCompleted;

		private SendOrPostCallback CreateByodRequestOperationCompleted;

		private SendOrPostCallback EnumByodDomainsOperationCompleted;

		private SendOrPostCallback GetAdminsOperationCompleted;

		private SendOrPostCallback GetManagementCertificateOperationCompleted;

		private SendOrPostCallback SetManagementCertificateOperationCompleted;

		private SendOrPostCallback SetManagementPermissionsOperationCompleted;

		private SendOrPostCallback GetMaxMembersOperationCompleted;

		private SendOrPostCallback SetMaxMembersOperationCompleted;
	}
}
