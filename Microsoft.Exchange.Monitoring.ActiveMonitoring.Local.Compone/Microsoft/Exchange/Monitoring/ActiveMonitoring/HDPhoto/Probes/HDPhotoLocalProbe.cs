using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data.ApplicationLogic.UserPhotos;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Mapi;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HDPhoto.Probes
{
	public class HDPhotoLocalProbe : HDPhotoProbe
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			ExchangeServerRoleEndpoint exchangeServerRoleEndpoint = LocalEndpointManager.Instance.ExchangeServerRoleEndpoint;
			if (exchangeServerRoleEndpoint.IsCafeRoleInstalled)
			{
				base.DoWork(cancellationToken);
			}
			if (exchangeServerRoleEndpoint.IsMailboxRoleInstalled && base.Definition.Attributes.ContainsKey("DatabaseGuid"))
			{
				string text = base.Definition.Attributes["DatabaseGuid"];
				if (!string.IsNullOrEmpty(text) && DirectoryAccessor.Instance.IsDatabaseCopyActiveOnLocalServer(new Guid(text)))
				{
					base.DoWork(cancellationToken);
				}
			}
		}

		protected override bool UploadPhoto(string endpoint, NetworkCredential credential, MemoryStream stream, string identity)
		{
			bool result = false;
			try
			{
				identity = string.Format("{0}@{1}", credential.UserName, credential.Domain);
				IRecipientSession recipientSession;
				ADUser user;
				if (LocalEndpointManager.IsDataCenter)
				{
					base.Result.StateAttribute15 = "Probe is running locally on datacenter machine";
					user = CommonAccessTokenHelper.ResolveTenantUser(identity, out recipientSession);
				}
				else
				{
					base.Result.StateAttribute15 = "Probe is running locally on Enterprise machine";
					user = CommonAccessTokenHelper.ResolveRootOrgUser(identity, out recipientSession);
					recipientSession = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(false, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 97, "UploadPhoto", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HDPhoto\\hdphotolocalprobe.cs");
				}
				ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromADUser(user, null);
				using (MailboxSession mailboxSession = MailboxSession.OpenAsAdmin(exchangePrincipal, CultureInfo.InvariantCulture, "Client=Management;Action=Set-UserPhoto"))
				{
					PhotosConfiguration configuration = new PhotosConfiguration(ExchangeSetupContext.InstallPath);
					PhotoRequest request = this.CreateRequest(exchangePrincipal, false, stream);
					new PhotoUploadPipeline(configuration, mailboxSession, recipientSession, NullTracer.Instance).Upload(request, Stream.Null);
					WTFDiagnostics.TraceInformation(ExTraceGlobals.HDPhotoTracer, base.TraceContext, "Uploaded the picture stream as preview", null, "UploadPhoto", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HDPhoto\\hdphotolocalprobe.cs", 113);
					request = this.CreateRequest(exchangePrincipal, true, null);
					new PhotoUploadPipeline(configuration, mailboxSession, recipientSession, NullTracer.Instance).Upload(request, Stream.Null);
					WTFDiagnostics.TraceInformation(ExTraceGlobals.HDPhotoTracer, base.TraceContext, "Saved the preview", null, "UploadPhoto", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HDPhoto\\hdphotolocalprobe.cs", 119);
					result = true;
				}
			}
			catch (Exception ex)
			{
				if (!(ex is MailboxOfflineException) && !(ex is WrongServerException) && !(ex is StorageTransientException) && !(ex is MailboxCrossSiteFailoverException) && !(ex is MapiExceptionMailboxInTransit) && !(ex is MapiExceptionNetworkError))
				{
					throw;
				}
				WTFDiagnostics.TraceInformation(ExTraceGlobals.HDPhotoTracer, base.TraceContext, string.Format("Got Exception {0} and stack trace is {1}", ex.GetType().Name, ex.StackTrace), null, "UploadPhoto", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HDPhoto\\hdphotolocalprobe.cs", 134);
				base.LogExceptionInfoOnResult(ex);
			}
			return result;
		}

		protected override void ConfigureProbe()
		{
			CertificateValidationManager.RegisterCallback("HDPhoto_Local_Probe", (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true);
		}

		protected override HttpWebRequest GetEwsRequest(NetworkCredential credential, string publisherEmail)
		{
			string endpoint = base.Definition.Endpoint;
			base.Result.StateAttribute1 = endpoint;
			string text = string.Format("{0}/s/GetUserPhoto?email={1}&size=HR96x96&trace=1", endpoint, publisherEmail);
			base.Result.StateAttribute11 = text;
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(text);
			httpWebRequest.KeepAlive = false;
			httpWebRequest.PreAuthenticate = true;
			httpWebRequest.Headers.Add(CertificateValidationManager.ComponentIdHeaderName, "HDPhoto_Local_Probe");
			bool flag = endpoint.Contains("444");
			if (flag)
			{
				httpWebRequest.UseDefaultCredentials = true;
				string text2 = string.Format("{0}@{1}", credential.UserName, credential.Domain);
				CommonAccessToken commonAccessToken;
				if (LocalEndpointManager.IsDataCenter)
				{
					commonAccessToken = CommonAccessTokenHelper.CreateLiveIdBasic(text2);
					base.Result.StateAttribute15 = "Mailbox Probe running in Datacenter using Cafe Auth";
				}
				else
				{
					commonAccessToken = CommonAccessTokenHelper.CreateWindows(text2, credential.Password);
					base.Result.StateAttribute15 = "Mailbox Probe running in Enterprise using Cafe Auth";
				}
				httpWebRequest.Headers.Add("X-CommonAccessToken", commonAccessToken.Serialize());
				httpWebRequest.Headers.Add("X-IsFromCafe", "1");
			}
			else if (LocalEndpointManager.IsDataCenter)
			{
				string str = Convert.ToBase64String(Encoding.Default.GetBytes(string.Format("{0}@{1}:{2}", credential.UserName, credential.Domain, credential.Password)));
				httpWebRequest.Headers.Add("Authorization", "Basic " + str);
				base.Result.StateAttribute15 = "Cafe Probe running in Datacenter using Basic Auth";
			}
			else
			{
				httpWebRequest.Credentials = credential;
				base.Result.StateAttribute15 = "Cafe Probe running in Enterprise using Integrated Auth";
			}
			return httpWebRequest;
		}

		private PhotoRequest CreateRequest(ExchangePrincipal principal, bool isSave, MemoryStream pictureStream)
		{
			if (isSave)
			{
				return new PhotoRequest
				{
					TargetPrimarySmtpAddress = principal.MailboxInfo.PrimarySmtpAddress.ToString(),
					UploadTo = principal.ObjectId,
					Preview = false,
					UploadCommand = UploadCommand.Upload
				};
			}
			return new PhotoRequest
			{
				TargetPrimarySmtpAddress = principal.MailboxInfo.PrimarySmtpAddress.ToString(),
				UploadTo = principal.ObjectId,
				Preview = true,
				UploadCommand = UploadCommand.Upload,
				RawUploadedPhoto = pictureStream
			};
		}

		protected const string ComponentId = "HDPhoto_Local_Probe";
	}
}
