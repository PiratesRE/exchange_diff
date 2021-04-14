using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class RoleAssigneeIdParameter : ADIdParameter
	{
		public RoleAssigneeIdParameter()
		{
		}

		public RoleAssigneeIdParameter(string identity) : base(identity)
		{
		}

		public RoleAssigneeIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public RoleAssigneeIdParameter(Mailbox mailbox) : base(mailbox.Id)
		{
			this.spParameter = new SecurityPrincipalIdParameter(mailbox);
		}

		public RoleAssigneeIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public RoleAssigneeIdParameter(SecurityIdentifier sid) : base(sid.ToString())
		{
			this.spParameter = new SecurityPrincipalIdParameter(sid);
		}

		public RoleAssigneeIdParameter(RoleGroup group) : base(group.Id)
		{
			this.spParameter = new SecurityPrincipalIdParameter(group.Id);
		}

		public RoleAssigneeIdParameter(RoleAssignmentPolicy policy) : base(policy.Id)
		{
			this.policyParameter = new MailboxPolicyIdParameter(policy);
		}

		public static RoleAssigneeIdParameter Parse(string identity)
		{
			return new RoleAssigneeIdParameter(identity);
		}

		internal static ADObject GetRawRoleAssignee(RoleAssigneeIdParameter user, IConfigurationSession configSession, IRecipientSession recipientSession)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			if (recipientSession == null)
			{
				throw new ArgumentNullException("recipientSession");
			}
			if (configSession == null)
			{
				throw new ArgumentNullException("configSession");
			}
			IEnumerable<ADObject> enumerable = user.GetObjects<RoleAssignmentPolicy>(null, configSession).Cast<ADObject>();
			EnumerableWrapper<ADObject> wrapper = EnumerableWrapper<ADObject>.GetWrapper(enumerable);
			if (!wrapper.HasElements())
			{
				SecurityIdentifier securityIdentifier = SecurityPrincipalIdParameter.TryParseToSID(user.RawIdentity);
				if (null != securityIdentifier)
				{
					ADRecipient adrecipient = recipientSession.FindBySid(securityIdentifier);
					if (adrecipient != null)
					{
						enumerable = new ADObject[]
						{
							adrecipient
						};
						wrapper = EnumerableWrapper<ADObject>.GetWrapper(enumerable);
					}
				}
				else
				{
					enumerable = user.GetObjects<ADRecipient>(null, recipientSession).Cast<ADObject>();
					wrapper = EnumerableWrapper<ADObject>.GetWrapper(enumerable);
				}
			}
			ADObject result = null;
			using (IEnumerator<ADObject> enumerator = wrapper.GetEnumerator())
			{
				if (!enumerator.MoveNext())
				{
					throw new ManagementObjectNotFoundException(Strings.ErrorPolicyUserOrSecurityGroupNotFound(user.ToString()));
				}
				result = enumerator.Current;
				if (enumerator.MoveNext())
				{
					throw new ManagementObjectAmbiguousException(Strings.ErrorPolicyUserOrSecurityGroupNotUnique(user.ToString()));
				}
			}
			return result;
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (subTreeSession == null)
			{
				throw new ArgumentNullException("subTreeSession");
			}
			if (!typeof(T).IsAssignableFrom(typeof(ADRecipient)) && !typeof(T).IsAssignableFrom(typeof(RoleAssignmentPolicy)))
			{
				throw new ArgumentException(Strings.ErrorInvalidType(typeof(T).Name), "type");
			}
			bool flag = session is IConfigurationSession && subTreeSession is IConfigurationSession;
			bool flag2 = session is IRecipientSession && subTreeSession is IRecipientSession;
			if (flag)
			{
				if (typeof(T).IsAssignableFrom(typeof(RoleAssignmentPolicy)))
				{
					if (this.policyParameter == null)
					{
						this.policyParameter = ((base.InternalADObjectId != null) ? new MailboxPolicyIdParameter(base.InternalADObjectId) : new MailboxPolicyIdParameter(base.RawIdentity));
					}
					return this.policyParameter.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason);
				}
				throw new ArgumentException("Argument Mismatch. Type T is located on Config NC and sessions aren't of IConfigurationSession type.");
			}
			else
			{
				if (!flag2)
				{
					throw new ArgumentException(string.Format("Invalid Session Type. Session isn't of type 'RecipientSession' or 'SystemConfigurationSession'. Session type is '{0}'", session.GetType().Name), "session");
				}
				if (typeof(T).IsAssignableFrom(typeof(ADRecipient)))
				{
					if (this.spParameter == null)
					{
						this.spParameter = ((base.InternalADObjectId != null) ? new SecurityPrincipalIdParameter(base.InternalADObjectId) : new SecurityPrincipalIdParameter(base.RawIdentity));
					}
					return this.spParameter.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason);
				}
				throw new ArgumentException("Argument Mismatch. Type T is located on Domain NC and sessions aren't of IRecipientSession type.");
			}
		}

		private SecurityPrincipalIdParameter spParameter;

		private MailboxPolicyIdParameter policyParameter;
	}
}
