using System;
using System.Security;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.Net.XropService;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class WebServiceServerSessionProvider : DisposeTrackableBase, IServerSessionProvider
	{
		public IServerSession Create(TokenValidationResults tokenValidationResults)
		{
			if (tokenValidationResults == null)
			{
				ExTraceGlobals.ConnectXropTracer.TraceDebug((long)this.GetHashCode(), "WebServiceServerSessionProvider.Create: TokenValidationResults are null.");
				return null;
			}
			string emailAddress = tokenValidationResults.EmailAddress;
			string text = null;
			string organization = null;
			IServerSession result;
			try
			{
				if (!SmtpAddress.IsValidSmtpAddress(emailAddress))
				{
					WebServiceEndPoint.LogFailure(string.Format("Connection failed. Address in authentication token is not a valid SMTP address: {0}", emailAddress), null, emailAddress, "<unknown>", null, ExTraceGlobals.ConnectXropTracer);
					result = null;
				}
				else
				{
					SmtpAddress userSmtpAddress = new SmtpAddress(emailAddress);
					text = userSmtpAddress.Domain;
					OrganizationId organizationId = DomainToOrganizationIdCache.Singleton.Get(new SmtpDomain(text));
					if (organizationId == null)
					{
						WebServiceEndPoint.LogFailure(string.Format("Connection failed. No AcceptedDomain matches domain from EmailAddress in authentication token: {0}", text), null, emailAddress, text, null, ExTraceGlobals.ConnectXropTracer);
						result = null;
					}
					else
					{
						organization = ((organizationId.OrganizationalUnit != null) ? organizationId.OrganizationalUnit.ToCanonicalName() : string.Empty);
						string text2;
						try
						{
							IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), 138, "Create", "f:\\15.00.1497\\sources\\dev\\mapimt\\src\\Server\\WebServiceServerSessionProvider.cs");
							ADRecipient adrecipient = tenantOrRootOrgRecipientSession.FindByProxyAddress(new SmtpProxyAddress(emailAddress, false));
							if (adrecipient == null)
							{
								WebServiceEndPoint.LogFailure(string.Format("Connection failed. Unable to find ADRecipient based on proxy SMTP address: {0}", emailAddress), null, emailAddress, text, organization, ExTraceGlobals.ConnectXropTracer);
								return null;
							}
							ADUser aduser = adrecipient as ADUser;
							if (aduser == null)
							{
								WebServiceEndPoint.LogFailure(string.Format("Connection failed. ADRecipient found is not an ADUser: {0}", emailAddress), null, emailAddress, text, organization, ExTraceGlobals.ConnectXropTracer);
								return null;
							}
							text2 = string.Format("{0}@{1}", aduser.SamAccountName, organizationId.PartitionId.ForestFQDN);
						}
						catch (DataValidationException exception)
						{
							WebServiceEndPoint.LogFailure(string.Format("Connection failed. Cannot find an AD object for proxy SMTP address: {0}", emailAddress), exception, emailAddress, text, organization, ExTraceGlobals.ConnectXropTracer);
							return null;
						}
						catch (DataSourceOperationException exception2)
						{
							WebServiceEndPoint.LogFailure(string.Format("Connection failed. Failure occurred during AD lookup for proxy SMTP address: {0}", emailAddress), exception2, emailAddress, text, organization, ExTraceGlobals.ConnectXropTracer);
							return null;
						}
						catch (DataSourceTransientException exception3)
						{
							WebServiceEndPoint.LogFailure(string.Format("Connection failed. Failure occurred during AD lookup for proxy SMTP address: {0}", emailAddress), exception3, emailAddress, text, organization, ExTraceGlobals.ConnectXropTracer);
							return null;
						}
						WebServiceServerSession webServiceServerSession;
						using (DisposeGuard disposeGuard = default(DisposeGuard))
						{
							WindowsIdentity windowsIdentity;
							try
							{
								windowsIdentity = new WindowsIdentity(text2);
							}
							catch (SecurityException exception4)
							{
								WebServiceEndPoint.LogFailure(string.Format("Connection failed. Unable to create WindowsIdentity for user: {0}, implicitUpn={1}", emailAddress, text2), exception4, emailAddress, text, organization, ExTraceGlobals.ConnectXropTracer);
								return null;
							}
							catch (UnauthorizedAccessException exception5)
							{
								WebServiceEndPoint.LogFailure(string.Format("Connection failed. Unable to create WindowsIdenity for user (unauthorized): {0}, implicitUpn={1}", emailAddress, text2), exception5, emailAddress, text, organization, ExTraceGlobals.ConnectXropTracer);
								return null;
							}
							disposeGuard.Add<WindowsIdentity>(windowsIdentity);
							webServiceServerSession = new WebServiceServerSession(windowsIdentity, new WebServiceUserInformation(userSmtpAddress, organization), WebServiceEndPoint.Dispatch);
							disposeGuard.Success();
						}
						result = webServiceServerSession;
					}
				}
			}
			catch (Exception exception6)
			{
				WebServiceEndPoint.LogFailure("Connection failed. Unhandled exception thrown.", exception6, emailAddress, text ?? "<unknown>", organization, ExTraceGlobals.ConnectXropTracer);
				throw;
			}
			return result;
		}

		protected override void InternalDispose(bool disposing)
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<WebServiceServerSessionProvider>(this);
		}
	}
}
