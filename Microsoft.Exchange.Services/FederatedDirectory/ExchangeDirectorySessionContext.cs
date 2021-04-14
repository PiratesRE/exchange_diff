using System;
using System.Diagnostics;
using System.Net;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Office.Server.Directory;

namespace Microsoft.Exchange.FederatedDirectory
{
	internal sealed class ExchangeDirectorySessionContext : IDirectorySessionContext
	{
		public ExchangeDirectorySessionContext(ADUser accessingUser, ExchangePrincipal accessingPrincipal)
		{
			ArgumentValidator.ThrowIfNull("accessingUser", accessingUser);
			ArgumentValidator.ThrowIfNull("accessingPrincipal", accessingPrincipal);
			this.AccessingUser = accessingUser;
			this.AccessingPrincipal = accessingPrincipal;
			this.TenantContextId = ((accessingUser.OrganizationId == OrganizationId.ForestWideOrgId) ? Guid.Empty : new Guid(accessingUser.OrganizationId.ToExternalDirectoryOrganizationId()));
			this.UserId = new Guid(accessingUser.ExternalDirectoryObjectId);
		}

		internal ADUser AccessingUser { get; private set; }

		internal ExchangePrincipal AccessingPrincipal { get; private set; }

		public Guid TenantContextId { get; private set; }

		public bool MockEnternalDirectoryObjectId { get; set; }

		public Guid UserId { get; private set; }

		public string UserPrincipalName
		{
			get
			{
				return this.AccessingUser.UserPrincipalName;
			}
		}

		public ICredentials ActAsUserCredentials
		{
			get
			{
				if (this.actAsUserCredentials == null)
				{
					this.actAsUserCredentials = OAuthCredentials.GetOAuthCredentialsForAppActAsToken(this.AccessingUser.OrganizationId, this.AccessingUser, null);
					LogWriter.TraceAndLog(new LogWriter.TraceMethod(ExchangeDirectorySessionContext.Tracer.TraceDebug), 4, this.GetHashCode(), "Created user credentials for {0}: {1}", new object[]
					{
						this.AccessingUser.UserPrincipalName,
						this.actAsUserCredentials
					});
				}
				return this.actAsUserCredentials;
			}
		}

		public ICredentials AppCredentials
		{
			get
			{
				if (this.appCredentials == null)
				{
					this.appCredentials = OAuthCredentials.GetOAuthCredentialsForAppToken(this.AccessingUser.OrganizationId, "dummyRealm");
					LogWriter.TraceAndLog(new LogWriter.TraceMethod(ExchangeDirectorySessionContext.Tracer.TraceDebug), 4, this.GetHashCode(), "Created app credentials for {0}: {1}", new object[]
					{
						this.AccessingUser.OrganizationId,
						this.appCredentials
					});
				}
				return this.appCredentials;
			}
		}

		public bool IsInTenantAdministrationRole
		{
			get
			{
				return true;
			}
		}

		public ExchangeDirectorySessionContext.NewGroupDiagnostics CreationDiagnostics
		{
			get
			{
				if (this.creationDiagnostics == null)
				{
					this.creationDiagnostics = new ExchangeDirectorySessionContext.NewGroupDiagnostics();
				}
				return this.creationDiagnostics;
			}
		}

		public override string ToString()
		{
			return "ExchangeDirectorySessionContext: " + this.AccessingUser.UserPrincipalName;
		}

		private static readonly Microsoft.Exchange.Diagnostics.Trace Tracer = ExTraceGlobals.FederatedDirectoryTracer;

		private ICredentials appCredentials;

		private ICredentials actAsUserCredentials;

		private ExchangeDirectorySessionContext.NewGroupDiagnostics creationDiagnostics;

		public class NewGroupDiagnostics
		{
			public NewGroupDiagnostics()
			{
				this.stopWatch = new Stopwatch();
			}

			public TimeSpan? MailboxCreationTime { get; private set; }

			public TimeSpan? AADIdentityCreationTime { get; private set; }

			public TimeSpan? GroupCreationTime { get; private set; }

			public bool MailboxCreatedSuccessfully { get; set; }

			public Guid CmdletLogCorrelationId { get; set; }

			public void Start()
			{
				this.stopWatch.Start();
			}

			public void Stop()
			{
				this.GroupCreationTime = new TimeSpan?(this.stopWatch.Elapsed);
				this.stopWatch.Stop();
			}

			public void RecordAADTime()
			{
				this.AADIdentityCreationTime = new TimeSpan?(this.stopWatch.Elapsed);
			}

			public void RecordMailboxTime()
			{
				this.MailboxCreationTime = new TimeSpan?(this.stopWatch.Elapsed.Subtract(this.AADIdentityCreationTime.Value));
			}

			private readonly Stopwatch stopWatch;
		}
	}
}
