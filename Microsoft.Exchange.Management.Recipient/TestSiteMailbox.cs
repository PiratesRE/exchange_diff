using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Management.Automation;
using System.Net;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.SharePoint.Client;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Test", "SiteMailbox", SupportsShouldProcess = true)]
	public sealed class TestSiteMailbox : TeamMailboxDiagnosticsBase
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageTestTeamMailbox((this.Identity != null) ? this.Identity.ToString() : this.SharePointUrl.AbsoluteUri.ToString());
			}
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public new RecipientIdParameter Identity
		{
			get
			{
				return (RecipientIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "TeamMailboxITPro")]
		public Uri SharePointUrl
		{
			get
			{
				return (Uri)base.Fields["SharePointUrl"];
			}
			set
			{
				base.Fields["SharePointUrl"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public RecipientIdParameter RequestorIdentity
		{
			get
			{
				return (RecipientIdParameter)base.Fields["RequestorIdentity"];
			}
			set
			{
				base.Fields["RequestorIdentity"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter UseAppTokenOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields["UseAppTokenOnly"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["UseAppTokenOnly"] = value;
			}
		}

		protected override void InternalValidate()
		{
			if (this.Identity != null && this.SharePointUrl != null)
			{
				base.WriteError(new InvalidOperationException(Strings.TestTeamMailboxConstraintError("Identity", "SharePointUrl")), ErrorCategory.InvalidOperation, null);
			}
			else if (this.Identity == null && this.SharePointUrl == null)
			{
				base.WriteError(new InvalidOperationException(Strings.TestTeamMailboxConstraintError("Identity", "SharePointUrl")), ErrorCategory.InvalidOperation, null);
			}
			if (this.UseAppTokenOnly && this.RequestorIdentity != null)
			{
				base.WriteError(new InvalidOperationException(Strings.TestTeamMailboxConstraintError("UseAppTokenOnly", "RequestorIdentity")), ErrorCategory.InvalidOperation, null);
			}
			ADObjectId adobjectId = null;
			base.TryGetExecutingUserId(out adobjectId);
			if (this.RequestorIdentity == null)
			{
				if (adobjectId == null)
				{
					base.WriteError(new InvalidOperationException(Strings.CouldNotGetExecutingUser), ErrorCategory.InvalidOperation, null);
				}
				try
				{
					this.requestor = (ADUser)base.GetDataObject(new RecipientIdParameter(adobjectId));
					goto IL_145;
				}
				catch (ManagementObjectNotFoundException)
				{
					if (this.UseAppTokenOnly && base.Organization != null)
					{
						this.requestor = null;
						goto IL_145;
					}
					throw;
				}
			}
			this.requestor = (ADUser)base.GetDataObject(this.RequestorIdentity);
			if (adobjectId != this.requestor.Id)
			{
				this.additionalConstrainedIdentity = this.requestor.Id;
			}
			IL_145:
			if (this.Identity != null)
			{
				base.InternalValidate();
				if (base.TMPrincipals.Count > 1)
				{
					base.WriteError(new InvalidOperationException(Strings.MoreThanOneTeamMailboxes), ErrorCategory.InvalidOperation, null);
				}
				using (Dictionary<ADUser, ExchangePrincipal>.KeyCollection.Enumerator enumerator = base.TMPrincipals.Keys.GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						ADUser aduser = enumerator.Current;
						this.tmADObject = aduser;
						if (this.tmADObject.SharePointUrl == null)
						{
							base.WriteError(new InvalidOperationException(Strings.TeamMailboxSharePointUrlMissing), ErrorCategory.InvalidOperation, null);
						}
					}
				}
			}
		}

		protected override void InternalProcessRecord()
		{
			ValidationResultCollector validationResultCollector = new ValidationResultCollector();
			LocalConfiguration localConfiguration = LocalConfiguration.Load(validationResultCollector);
			foreach (ValidationResultNode sendToPipeline in validationResultCollector.Results)
			{
				base.WriteObject(sendToPipeline);
			}
			SharePointException ex = null;
			Uri uri = this.SharePointUrl ?? this.tmADObject.SharePointUrl;
			OAuthCredentials oauthCredentials = null;
			try
			{
				using (ClientContext clientContext = new ClientContext(uri))
				{
					bool flag = false;
					ICredentials credentialAndConfigureClientContext = TeamMailboxHelper.GetCredentialAndConfigureClientContext(this.requestor, (this.requestor != null) ? this.requestor.OrganizationId : base.CurrentOrganizationId, clientContext, this.UseAppTokenOnly, out flag);
					if (!flag)
					{
						base.WriteError(new InvalidOperationException(Strings.OauthIsTurnedOff), ErrorCategory.InvalidOperation, null);
					}
					oauthCredentials = (credentialAndConfigureClientContext as OAuthCredentials);
					oauthCredentials.Tracer = new TestSiteMailbox.TaskOauthOutboundTracer();
					oauthCredentials.LocalConfiguration = localConfiguration;
					Web web = clientContext.Web;
					clientContext.Load<Web>(web, new Expression<Func<Web, object>>[0]);
					clientContext.ExecuteQuery();
				}
			}
			catch (ClientRequestException e)
			{
				ex = new SharePointException(uri.AbsoluteUri, e);
			}
			catch (ServerException e2)
			{
				ex = new SharePointException(uri.AbsoluteUri, e2);
			}
			catch (IOException ex2)
			{
				ex = new SharePointException(uri.AbsoluteUri, new LocalizedString(ex2.Message));
			}
			catch (WebException e3)
			{
				ex = new SharePointException(uri.AbsoluteUri, e3, true);
			}
			if (ex != null)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine(Strings.TestTeamMailboxOutboundOauthLog);
				stringBuilder.AppendLine(oauthCredentials.Tracer.ToString());
				stringBuilder.AppendLine(Strings.TestTeamMailboxSharePointResponseDetails);
				stringBuilder.AppendLine(ex.DiagnosticInfo);
				ValidationResultNode sendToPipeline2 = new ValidationResultNode(Strings.TestTeamMailboxSharepointCallUnderOauthTask, new LocalizedString(stringBuilder.ToString()), ResultType.Error);
				base.WriteObject(sendToPipeline2);
				return;
			}
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.AppendLine(Strings.TestTeamMailboxSharepointCallUnderOauthSuccess(uri.AbsoluteUri));
			stringBuilder2.AppendLine(Strings.TestTeamMailboxOutboundOauthLog);
			stringBuilder2.AppendLine(oauthCredentials.Tracer.ToString());
			ValidationResultNode sendToPipeline3 = new ValidationResultNode(Strings.TestTeamMailboxSharepointCallUnderOauthTask, new LocalizedString(stringBuilder2.ToString()), ResultType.Success);
			base.WriteObject(sendToPipeline3);
		}

		private ADUser requestor;

		private ADUser tmADObject;

		private sealed class TaskOauthOutboundTracer : IOutboundTracer
		{
			public void LogInformation(int hashCode, string formatString, params object[] args)
			{
				this.result.Append("Information:");
				this.result.AppendLine(string.Format(formatString, args));
			}

			public void LogWarning(int hashCode, string formatString, params object[] args)
			{
				this.result.Append("Warning:");
				this.result.AppendLine(string.Format(formatString, args));
			}

			public void LogError(int hashCode, string formatString, params object[] args)
			{
				this.result.Append("Error:");
				this.result.AppendLine(string.Format(formatString, args));
			}

			public void LogToken(int hashCode, string tokenString)
			{
				this.result.Append("Token:");
				this.result.AppendLine(tokenString);
			}

			public override string ToString()
			{
				return this.result.ToString();
			}

			private readonly StringBuilder result = new StringBuilder();
		}
	}
}
