using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ObscureUrlKey : SharingAnonymousIdentityCacheKey, IEquatable<ObscureUrlKey>
	{
		internal ObscureUrlKey(ObscureUrl url) : base(ObscureUrlKey.GetHashCode(url))
		{
			Util.ThrowOnNullArgument(url, "url");
			this.mailboxGuid = url.MailboxGuid;
			this.urlId = url.Identity;
			this.domain = url.Domain;
		}

		private static int GetHashCode(ObscureUrl url)
		{
			Util.ThrowOnNullArgument(url, "url");
			return (url.MailboxGuid.ToString() + url.Identity).GetHashCode();
		}

		public override string Lookup(out SecurityIdentifier sid)
		{
			sid = null;
			ADUser aduser;
			try
			{
				ADSessionSettings sessionSettings = ADSessionSettings.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(this.domain);
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.PartiallyConsistent, sessionSettings, 67, "Lookup", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Sharing\\ObscureUrlKey.cs");
				aduser = (tenantOrRootOrgRecipientSession.FindByExchangeGuid(this.mailboxGuid) as ADUser);
				if (aduser == null)
				{
					ExTraceGlobals.SharingTracer.TraceError<Guid>((long)this.GetHashCode(), "ObscureUrlKey.Lookup(): Cannot find User object for mailbox guid {0}.", this.mailboxGuid);
					return null;
				}
			}
			catch (CannotResolveTenantNameException arg)
			{
				ExTraceGlobals.SharingTracer.TraceError<string, Guid, CannotResolveTenantNameException>((long)this.GetHashCode(), "ObscureUrlKey.Lookup(): Cannot resolve tenant name {0} for mailbox guid {1}.Error: {2}", this.domain, this.mailboxGuid, arg);
				return null;
			}
			catch (ADTransientException arg2)
			{
				ExTraceGlobals.SharingTracer.TraceError<string, Guid, ADTransientException>((long)this.GetHashCode(), "ObscureUrlKey.Lookup(): Transient exception {0} for mailbox guid {1}.Error: {2}", this.domain, this.mailboxGuid, arg2);
				return null;
			}
			catch (DataSourceOperationException arg3)
			{
				ExTraceGlobals.SharingTracer.TraceError<string, Guid, DataSourceOperationException>((long)this.GetHashCode(), "ObscureUrlKey.Lookup(): DataSourceOperationException  {0} for mailbox guid {1}.Error: {2}", this.domain, this.mailboxGuid, arg3);
				return null;
			}
			sid = aduser.Sid;
			string folder = aduser.SharingAnonymousIdentities.GetFolder(this.urlId);
			if (string.IsNullOrEmpty(folder))
			{
				ExTraceGlobals.SharingTracer.TraceDebug<Guid, string>((long)this.GetHashCode(), "ObscureUrlKey.Lookup(): Not found sharing anonymous identity for mailbox guid {0} with url id {1}.", this.mailboxGuid, this.urlId);
			}
			else
			{
				ExTraceGlobals.SharingTracer.TraceDebug<Guid, string, string>((long)this.GetHashCode(), "ObscureUrlKey.Lookup(): Get sharing anonymous identity for mailbox guid {0} with url id {1}: Folder Id = {2}.", this.mailboxGuid, this.urlId, folder);
			}
			return folder;
		}

		public bool Equals(ObscureUrlKey other)
		{
			return !(other == null) && (object.ReferenceEquals(this, other) || (StringComparer.OrdinalIgnoreCase.Equals(this.urlId, other.urlId) && this.mailboxGuid == other.mailboxGuid));
		}

		protected override bool InternalEquals(object obj)
		{
			return this.Equals(obj as ObscureUrlKey);
		}

		private readonly Guid mailboxGuid;

		private readonly string urlId;

		private readonly string domain;
	}
}
