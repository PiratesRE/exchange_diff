using System;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.Protocols;

namespace Microsoft.Exchange.Security.X509CertAuth
{
	internal class X509CertUserCache : LazyLookupTimeoutCache<X509Identifier, X509CertUserCache.ResolvedX509CertUser>
	{
		private X509CertUserCache() : base(1, X509CertUserCache.cacheSize.Value, false, X509CertUserCache.cacheTimeToLive.Value)
		{
		}

		public static X509CertUserCache Singleton
		{
			get
			{
				if (X509CertUserCache.singleton == null)
				{
					lock (X509CertUserCache.lockObj)
					{
						if (X509CertUserCache.singleton == null)
						{
							X509CertUserCache.singleton = new X509CertUserCache();
						}
					}
				}
				return X509CertUserCache.singleton;
			}
		}

		protected override X509CertUserCache.ResolvedX509CertUser CreateOnCacheMiss(X509Identifier key, ref bool shouldAdd)
		{
			shouldAdd = true;
			string subject = key.Subject;
			string anchorMailbox = null;
			string text = null;
			if (HttpContext.Current != null && HttpContext.Current.Request != null)
			{
				anchorMailbox = (text = HttpContext.Current.Request.Headers[WellKnownHeader.AnchorMailbox]);
			}
			if (string.IsNullOrEmpty(text))
			{
				int num = subject.IndexOf("=");
				if (num != -1)
				{
					int num2 = subject.IndexOf(",", num);
					if (num2 != -1)
					{
						text = subject.Substring(num + 1, num2 - num - 1).Trim();
					}
					else
					{
						text = subject.Substring(num + 1).Trim();
					}
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				return X509CertUserCache.ResolvedX509CertUser.CannotFindUserAddress(subject, anchorMailbox);
			}
			if (!SmtpAddress.IsValidSmtpAddress(text))
			{
				return X509CertUserCache.ResolvedX509CertUser.InvalidUserAddress(text);
			}
			string domain = new SmtpAddress(text).Domain;
			ADSessionSettings adsessionSettings = null;
			ADTransientException ex = null;
			for (int i = 0; i < X509CertUserCache.lookupRetryMax.Value; i++)
			{
				try
				{
					adsessionSettings = ADSessionSettings.FromTenantAcceptedDomain(domain);
				}
				catch (CannotResolveTenantNameException)
				{
					return X509CertUserCache.ResolvedX509CertUser.CannotResolveTenantName(domain);
				}
				catch (ADTransientException ex2)
				{
					ex = ex2;
				}
			}
			if (adsessionSettings == null)
			{
				shouldAdd = false;
				return X509CertUserCache.ResolvedX509CertUser.ADTransientException(ex);
			}
			for (int j = 0; j < X509CertUserCache.lookupRetryMax.Value; j++)
			{
				try
				{
					IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, adsessionSettings, 163, "CreateOnCacheMiss", "f:\\15.00.1497\\sources\\dev\\Security\\src\\Authentication\\X509CertAuth\\X509CertUserCache.cs");
					ADUser aduser = (ADUser)tenantOrRootOrgRecipientSession.FindByCertificate(key);
					ExTraceGlobals.X509CertAuthTracer.TraceDebug<ADUser, X509Identifier>(0L, "[X509CertCache::CreateOnCacheMiss] FindByCertificate returns '{0}' for X509Identifier {1}", aduser, key);
					return new X509CertUserCache.ResolvedX509CertUser
					{
						OrganizationId = aduser.OrganizationId,
						Sid = aduser.Sid,
						UserPrincipalName = aduser.UserPrincipalName,
						ImplicitUserPrincipalName = string.Format("{0}@{1}", aduser.SamAccountName, aduser.Id.GetPartitionId().ForestFQDN)
					};
				}
				catch (NonUniqueRecipientException arg)
				{
					ExTraceGlobals.X509CertAuthTracer.TraceError<NonUniqueRecipientException>(0L, "[X509CertCache::CreateOnCacheMiss] FindByCertificate throws NonUniqueRecipientException {0}", arg);
					return X509CertUserCache.ResolvedX509CertUser.NonUniqueRecipient(key);
				}
				catch (ADTransientException ex3)
				{
					ExTraceGlobals.X509CertAuthTracer.TraceDebug<int, ADTransientException>(0L, "[X509CertCache::CreateOnCacheMiss] FindByCertificate will retry for {0} times, for ADTransientException {1}", j, ex3);
					ex = ex3;
				}
			}
			ExTraceGlobals.X509CertAuthTracer.TraceWarning<X509Identifier>(0L, "[X509CertCache::CreateOnCacheMiss] FindByCertificate returns null for X509Identifier {0}", key);
			shouldAdd = false;
			return X509CertUserCache.ResolvedX509CertUser.ADTransientException(ex);
		}

		private static TimeSpanAppSettingsEntry cacheTimeToLive = new TimeSpanAppSettingsEntry("X509CertCacheTimeToLive", TimeSpanUnit.Seconds, TimeSpan.FromMinutes(15.0), ExTraceGlobals.X509CertAuthTracer);

		private static IntAppSettingsEntry cacheSize = new IntAppSettingsEntry("X509CertCacheMaxItems", 500, ExTraceGlobals.X509CertAuthTracer);

		private static IntAppSettingsEntry lookupRetryMax = new IntAppSettingsEntry("X509CertCacheLookupRetryMax", 3, ExTraceGlobals.X509CertAuthTracer);

		private static readonly object lockObj = new object();

		private static X509CertUserCache singleton = null;

		internal class ResolvedX509CertUser
		{
			public OrganizationId OrganizationId { get; set; }

			public string ImplicitUserPrincipalName { get; set; }

			public string UserPrincipalName { get; set; }

			public SecurityIdentifier Sid { get; set; }

			public string ErrorString { get; set; }

			public static X509CertUserCache.ResolvedX509CertUser CannotFindUserAddress(string subject, string anchorMailbox)
			{
				return X509CertUserCache.ResolvedX509CertUser.CreateErrorObject("Cannot find user email address in cert subject '{0}' or anchor mailbox header '{1}'", new object[]
				{
					subject,
					anchorMailbox
				});
			}

			public static X509CertUserCache.ResolvedX509CertUser InvalidUserAddress(string userAddress)
			{
				return X509CertUserCache.ResolvedX509CertUser.CreateErrorObject("'{0}' is not valid email address", new object[]
				{
					userAddress
				});
			}

			public static X509CertUserCache.ResolvedX509CertUser CannotResolveTenantName(string domain)
			{
				return X509CertUserCache.ResolvedX509CertUser.CreateErrorObject("Cannot resolve tenant name '{0}'", new object[]
				{
					domain
				});
			}

			public static X509CertUserCache.ResolvedX509CertUser NonUniqueRecipient(X509Identifier identifier)
			{
				return X509CertUserCache.ResolvedX509CertUser.CreateErrorObject("FindByCertificate hit non-unique recipient for subject '{0}', issuer '{1}'", new object[]
				{
					identifier.Subject,
					identifier.Issuer
				});
			}

			public static X509CertUserCache.ResolvedX509CertUser ADTransientException(Exception ex)
			{
				return X509CertUserCache.ResolvedX509CertUser.CreateErrorObject("Hitting transient exception with '{0}'", new object[]
				{
					ex
				});
			}

			private static X509CertUserCache.ResolvedX509CertUser CreateErrorObject(string format, params object[] args)
			{
				return new X509CertUserCache.ResolvedX509CertUser
				{
					ErrorString = string.Format(format, args)
				};
			}
		}
	}
}
