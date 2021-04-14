using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;

namespace Microsoft.Exchange.Security.Authentication
{
	public class CompositeIdentity : GenericSidIdentity, IIdentity, IEnumerable<IdentityRef>, IEnumerable
	{
		public CompositeIdentity(IdentityRef primaryIdentity, IEnumerable<IdentityRef> secondaryIdentities) : base(primaryIdentity.Identity.GetSafeName(true), primaryIdentity.Identity.AuthenticationType, primaryIdentity.Identity.GetSecurityIdentifier())
		{
			this.ValidateIdentities(primaryIdentity, secondaryIdentities);
			this.primaryIdentity = primaryIdentity;
			this.secondaryIdentities = (secondaryIdentities ?? ((IEnumerable<IdentityRef>)new IdentityRef[0])).ToArray<IdentityRef>();
		}

		public GenericIdentity PrimaryIdentity
		{
			get
			{
				return this.primaryIdentity.Identity;
			}
		}

		public int SecondaryIdentitiesCount
		{
			get
			{
				return this.secondaryIdentities.Length;
			}
		}

		public IEnumerable<GenericIdentity> SecondaryIdentities
		{
			get
			{
				return from ir in this.secondaryIdentities
				select ir.Identity;
			}
		}

		public SecurityIdentifier CanarySid
		{
			get
			{
				IdentityRef identityRef;
				if (!this.primaryIdentity.IsUsedForCanary)
				{
					identityRef = this.secondaryIdentities.FirstOrDefault((IdentityRef si) => si.IsUsedForCanary);
				}
				else
				{
					identityRef = this.primaryIdentity;
				}
				IdentityRef identityRef2 = identityRef;
				if (identityRef2 == null)
				{
					identityRef2 = this.primaryIdentity;
				}
				return identityRef2.Identity.GetSecurityIdentifier();
			}
		}

		public GenericIdentity GetSecondaryIdentityAt(int index)
		{
			if (index < 0 || index >= this.SecondaryIdentitiesCount)
			{
				throw new ArgumentOutOfRangeException("index", string.Format(CultureInfo.InvariantCulture, "Invalid index specified ({0}). There are {1} secondary identities available.", new object[]
				{
					index,
					this.SecondaryIdentitiesCount
				}));
			}
			return this.secondaryIdentities[index].Identity;
		}

		public IEnumerator<IdentityRef> GetEnumerator()
		{
			return this.GetAllIdentities().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private void ValidateIdentities(IdentityRef primaryIdentity, IEnumerable<IdentityRef> secondaryIdentities)
		{
			if (primaryIdentity == null)
			{
				throw new ArgumentNullException("primaryIdentity", "The primary identity must not be null!");
			}
			int num = Convert.ToInt32(primaryIdentity.Authority == AuthenticationAuthority.MSA);
			if (secondaryIdentities != null)
			{
				foreach (IdentityRef identityRef in secondaryIdentities)
				{
					num += Convert.ToInt32(identityRef.Authority == AuthenticationAuthority.MSA);
				}
			}
			if (num > 1)
			{
				throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "There must not be more than one {0} identity present.", new object[]
				{
					AuthenticationAuthority.MSA
				}));
			}
		}

		private IEnumerable<IdentityRef> GetAllIdentities()
		{
			yield return this.primaryIdentity;
			if (this.secondaryIdentities != null)
			{
				foreach (IdentityRef secondaryIdentity in this.secondaryIdentities)
				{
					yield return secondaryIdentity;
				}
			}
			yield break;
		}

		private readonly IdentityRef primaryIdentity;

		private readonly IdentityRef[] secondaryIdentities;
	}
}
