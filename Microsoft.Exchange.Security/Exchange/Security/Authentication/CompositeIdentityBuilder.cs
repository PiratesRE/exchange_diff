using System;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Security.Authentication
{
	internal class CompositeIdentityBuilder
	{
		private CompositeIdentityBuilder(HttpContext context)
		{
			this.isCompositeIdentityFlightEnabled = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Cafe.CompositeIdentity.Enabled;
			this.isMsaIdentityPrimary = CompositeIdentityBuilder.IsMsaIdentityRequestedToBePrimary(context);
		}

		public static bool IsLiveIdAuthStillNeededForOwa(HttpContext context)
		{
			CompositeIdentityBuilder compositeIdentityBuilder;
			return CompositeIdentityBuilder.TryGetInstance(context, out compositeIdentityBuilder) && compositeIdentityBuilder.isCompositeIdentityFlightEnabled && compositeIdentityBuilder.orgIdIdentity == null;
		}

		public static void AddIdentity(HttpContext context, GenericIdentity identity, AuthenticationAuthority authAuthority)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context", "The current HTTP context must not be null!");
			}
			if (identity == null)
			{
				throw new ArgumentNullException("identity", "You cannot add a null identity to the collection!");
			}
			CompositeIdentityBuilder instance = CompositeIdentityBuilder.GetInstance(context);
			if (instance.isCompositeIdentityFlightEnabled)
			{
				instance.AddIdentity(identity, authAuthority);
			}
		}

		public static IIdentity GetUserIdentity(HttpContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context", "The current HTTP context must not be null!");
			}
			IIdentity result = null;
			CompositeIdentity compositeIdentity;
			if (!CompositeIdentityBuilder.ExecuteIfIdentitiesPresent(context, delegate(CompositeIdentityBuilder builder)
			{
				if (builder.TryGetCompositeIdentity(out compositeIdentity))
				{
					result = compositeIdentity;
					return;
				}
				result = (builder.msaIdentity ?? builder.orgIdIdentity).Identity;
			}))
			{
				result = ((context.User != null) ? context.User.Identity : null);
			}
			return result;
		}

		public static bool TryGetMsaNoAdUserIdentity(HttpContext context, out MSAIdentity msaIdentity)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context", "The current HTTP context must not be null!");
			}
			MSAIdentity result = null;
			if (!CompositeIdentityBuilder.ExecuteIfIdentitiesPresent(context, delegate(CompositeIdentityBuilder builder)
			{
				result = ((builder.msaIdentity != null) ? (builder.msaIdentity.Identity as MSAIdentity) : null);
			}))
			{
				result = ((context.User != null) ? (context.User.Identity as MSAIdentity) : null);
			}
			msaIdentity = result;
			return msaIdentity != null;
		}

		public static bool TryHandleRehydratedIdentity(HttpContext context, IIdentity rehydratedIdentity)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context", "The current HTTP context must not be null!");
			}
			if (rehydratedIdentity == null)
			{
				throw new ArgumentNullException("rehydratedIdentity", "The rehydratedIdentity cannot be null!");
			}
			CompositeIdentityBuilder instance = CompositeIdentityBuilder.GetInstance(context);
			if (!instance.isCompositeIdentityFlightEnabled)
			{
				return true;
			}
			CompositeIdentity compositeIdentity = rehydratedIdentity as CompositeIdentity;
			if (compositeIdentity != null)
			{
				instance.SetIdentityCollection(compositeIdentity);
				return true;
			}
			MSAIdentity msaidentity = rehydratedIdentity as MSAIdentity;
			if (msaidentity != null)
			{
				instance.AddIdentity(msaidentity, AuthenticationAuthority.MSA);
				return true;
			}
			GenericIdentity genericIdentity = rehydratedIdentity as GenericIdentity;
			if (genericIdentity != null)
			{
				instance.AddIdentity(genericIdentity, AuthenticationAuthority.ORGID);
				return true;
			}
			return false;
		}

		private static bool TryGetInstance(HttpContext context, out CompositeIdentityBuilder instance)
		{
			instance = (context.Items["CompositeIdentityAuthenticationHelper"] as CompositeIdentityBuilder);
			return instance != null;
		}

		private static CompositeIdentityBuilder GetInstance(HttpContext context)
		{
			CompositeIdentityBuilder compositeIdentityBuilder;
			if (!CompositeIdentityBuilder.TryGetInstance(context, out compositeIdentityBuilder))
			{
				compositeIdentityBuilder = new CompositeIdentityBuilder(context);
				context.Items["CompositeIdentityAuthenticationHelper"] = compositeIdentityBuilder;
			}
			return compositeIdentityBuilder;
		}

		private static bool ExecuteIfIdentitiesPresent(HttpContext context, Action<CompositeIdentityBuilder> actionToExecute)
		{
			CompositeIdentityBuilder compositeIdentityBuilder;
			if (CompositeIdentityBuilder.TryGetInstance(context, out compositeIdentityBuilder) && compositeIdentityBuilder.isCompositeIdentityFlightEnabled && compositeIdentityBuilder.IdentitiesCount > 0)
			{
				actionToExecute(compositeIdentityBuilder);
				return true;
			}
			return false;
		}

		private static string GetPrimaryIdentityHeaderValue(HttpContext context)
		{
			string text = context.Request.Headers[WellKnownHeader.PrimaryIdentity];
			if (string.IsNullOrWhiteSpace(text))
			{
				text = context.Request.Headers["X-PrimaryIdentityId"];
			}
			if (!string.IsNullOrWhiteSpace(text))
			{
				return text;
			}
			return null;
		}

		private static bool IsMsaIdentityRequestedToBePrimary(HttpContext context)
		{
			string primaryIdentityHeaderValue = CompositeIdentityBuilder.GetPrimaryIdentityHeaderValue(context);
			return string.Equals(primaryIdentityHeaderValue, "MSA", StringComparison.OrdinalIgnoreCase);
		}

		private int IdentitiesCount
		{
			get
			{
				return Convert.ToInt32(this.msaIdentity != null) + Convert.ToInt32(this.orgIdIdentity != null);
			}
		}

		private bool TryGetCompositeIdentity(out CompositeIdentity compositeIdentity)
		{
			compositeIdentity = null;
			if (this.isMsaIdentityPrimary && this.msaIdentity == null)
			{
				throw new MissingIdentityException(CompositeIdentityBuilder.UnifiedMailboxMarker, "The primary identity (MSA) is missing.");
			}
			if (this.IdentitiesCount > 1)
			{
				if (this.isMsaIdentityPrimary)
				{
					compositeIdentity = new CompositeIdentity(this.msaIdentity, new IdentityRef[]
					{
						this.orgIdIdentity
					});
				}
				else
				{
					if (this.orgIdIdentity == null)
					{
						throw new MissingIdentityException(CompositeIdentityBuilder.OrgIdMailboxMarker, "The primary identity (ORGID) is missing.");
					}
					compositeIdentity = new CompositeIdentity(this.orgIdIdentity, (this.msaIdentity == null) ? null : new IdentityRef[]
					{
						this.msaIdentity
					});
				}
				return true;
			}
			return false;
		}

		private void AddIdentity(GenericIdentity identity, AuthenticationAuthority authAuthority)
		{
			IdentityRef identityRef = new IdentityRef(identity, authAuthority, authAuthority != AuthenticationAuthority.MSA);
			if (authAuthority == AuthenticationAuthority.MSA)
			{
				this.msaIdentity = identityRef;
				return;
			}
			this.orgIdIdentity = identityRef;
		}

		private void SetIdentityCollection(CompositeIdentity ci)
		{
			foreach (IdentityRef identityRef in ci)
			{
				if (identityRef.Authority == AuthenticationAuthority.MSA)
				{
					this.msaIdentity = identityRef;
				}
				else
				{
					this.orgIdIdentity = identityRef;
				}
			}
		}

		private const string BuilderInstanceItemName = "CompositeIdentityAuthenticationHelper";

		public static readonly Guid UnifiedMailboxMarker = new Guid("6b08da0b-2146-48b1-9b7d-712991891aef");

		public static readonly Guid OrgIdMailboxMarker = new Guid("000f9a33-4461-46f3-bf99-c183947612a5");

		private readonly bool isMsaIdentityPrimary;

		private readonly bool isCompositeIdentityFlightEnabled;

		private IdentityRef msaIdentity;

		private IdentityRef orgIdIdentity;
	}
}
