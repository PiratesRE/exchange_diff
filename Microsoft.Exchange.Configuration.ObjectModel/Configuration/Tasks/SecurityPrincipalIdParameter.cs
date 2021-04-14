using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Management.Automation;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Common.LocStrings;
using Microsoft.Exchange.Configuration.ObjectModel;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[Serializable]
	public class SecurityPrincipalIdParameter : RecipientIdParameter, IUrlTokenEncode
	{
		public SecurityPrincipalIdParameter(string identity) : base(identity)
		{
		}

		public SecurityPrincipalIdParameter()
		{
		}

		public SecurityPrincipalIdParameter(INamedIdentity namedIdentity) : base(namedIdentity)
		{
		}

		public SecurityPrincipalIdParameter(Mailbox mailbox) : base(mailbox.Id)
		{
		}

		public SecurityPrincipalIdParameter(ADObjectId adObjectId) : base(adObjectId)
		{
		}

		public SecurityPrincipalIdParameter(SecurityIdentifier sid) : this(SecurityPrincipalIdParameter.GetFriendlyUserName(sid, null))
		{
			this.securityIdentifier = sid;
		}

		public SecurityPrincipalIdParameter(SecurityIdentifier sid, string friendlyName) : this(friendlyName)
		{
			this.securityIdentifier = sid;
		}

		public SecurityIdentifier SecurityIdentifier
		{
			get
			{
				return this.securityIdentifier;
			}
			internal set
			{
				this.securityIdentifier = value;
			}
		}

		internal override RecipientType[] RecipientTypes
		{
			get
			{
				return SecurityPrincipalIdParameter.AllowedRecipientTypes;
			}
		}

		public bool ReturnUrlTokenEncodedString
		{
			get
			{
				return this.returnUrlTokenEncodedString;
			}
			set
			{
				this.returnUrlTokenEncodedString = value;
			}
		}

		private static Dictionary<string, string> SidToAliasMap
		{
			get
			{
				if (SecurityPrincipalIdParameter.sidToAlias == null)
				{
					lock (SecurityPrincipalIdParameter.syncLock)
					{
						Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
						Type typeFromHandle = typeof(WellKnownSids);
						Type typeFromHandle2 = typeof(DescriptionAttribute);
						BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.Public;
						foreach (FieldInfo fieldInfo in typeFromHandle.GetFields(bindingAttr))
						{
							SecurityIdentifier securityIdentifier = fieldInfo.GetValue(null) as SecurityIdentifier;
							if (!(securityIdentifier == null))
							{
								DescriptionAttribute[] array = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeFromHandle2, true);
								if (array != null)
								{
									try
									{
										dictionary.Add(securityIdentifier.ToString(), array[0].Description);
									}
									catch (ArgumentException)
									{
									}
									SecurityPrincipalIdParameter.sidToAlias = dictionary;
								}
							}
						}
					}
				}
				return SecurityPrincipalIdParameter.sidToAlias;
			}
		}

		public new static SecurityPrincipalIdParameter Parse(string identity)
		{
			return new SecurityPrincipalIdParameter(identity);
		}

		public override string ToString()
		{
			string text = SecurityPrincipalIdParameter.MapSidToAlias(base.RawIdentity);
			string text2 = (!string.IsNullOrEmpty(text)) ? text : base.ToString();
			if (this.ReturnUrlTokenEncodedString && text2 != null)
			{
				text2 = UrlTokenConverter.UrlTokenEncode(text2);
			}
			return text2;
		}

		internal static string GetFriendlyUserName(IdentityReference sid, Task.TaskVerboseLoggingDelegate verboseLogger)
		{
			if (null == sid)
			{
				throw new ArgumentNullException("sid");
			}
			string result;
			try
			{
				result = sid.Translate(typeof(NTAccount)).ToString();
			}
			catch (IdentityNotMappedException ex)
			{
				TaskLogger.Trace("Couldn't resolve the following sid '{0}': {1}", new object[]
				{
					sid.ToString(),
					ex.Message
				});
				if (verboseLogger != null)
				{
					verboseLogger(Strings.VerboseCannotResolveSid(sid.ToString(), ex.Message));
				}
				result = sid.ToString();
			}
			catch (SystemException ex2)
			{
				TaskLogger.Trace("Couldn't resolve the following sid '{0}': {1}", new object[]
				{
					sid.ToString(),
					ex2.Message
				});
				if (verboseLogger != null)
				{
					verboseLogger(Strings.VerboseCannotResolveSid(sid.ToString(), ex2.Message));
				}
				result = sid.ToString();
			}
			return result;
		}

		internal static IADSecurityPrincipal GetSecurityPrincipal(IRecipientSession session, SecurityPrincipalIdParameter user, Task.TaskErrorLoggingDelegate logError, Task.TaskVerboseLoggingDelegate logVerbose)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			if (logError == null)
			{
				throw new ArgumentNullException("logError");
			}
			if (logVerbose == null)
			{
				throw new ArgumentNullException("logVerbose");
			}
			ADRecipient adrecipient = null;
			logVerbose(Strings.CheckIfUserIsASID(user.ToString()));
			SecurityIdentifier securityIdentifier = SecurityPrincipalIdParameter.TryParseToSID(user.RawIdentity);
			if (null == securityIdentifier)
			{
				securityIdentifier = SecurityPrincipalIdParameter.GetUserSidAsSAMAccount(user, logError, logVerbose);
			}
			IEnumerable<ADRecipient> objects = user.GetObjects<ADRecipient>(null, session);
			foreach (ADRecipient adrecipient2 in objects)
			{
				if (adrecipient == null)
				{
					adrecipient = adrecipient2;
				}
				else
				{
					logError(new ManagementObjectAmbiguousException(Strings.ErrorUserNotUnique(user.ToString())), ErrorCategory.InvalidData, null);
				}
			}
			if (adrecipient == null && null != securityIdentifier)
			{
				adrecipient = new ADUser();
				adrecipient.propertyBag.SetField(IADSecurityPrincipalSchema.Sid, securityIdentifier);
			}
			if (adrecipient == null)
			{
				logError(new ManagementObjectNotFoundException(Strings.ErrorUserNotFound(user.ToString())), ErrorCategory.InvalidData, null);
			}
			return (IADSecurityPrincipal)adrecipient;
		}

		internal static SecurityIdentifier GetUserSid(IRecipientSession session, SecurityPrincipalIdParameter user, Task.TaskErrorLoggingDelegate logError, Task.TaskVerboseLoggingDelegate logVerbose)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			if (logError == null)
			{
				throw new ArgumentNullException("logError");
			}
			if (logVerbose == null)
			{
				throw new ArgumentNullException("logVerbose");
			}
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			logVerbose(Strings.CheckIfUserIsASID(user.ToString()));
			SecurityIdentifier securityIdentifier = SecurityPrincipalIdParameter.TryParseToSID(user.RawIdentity);
			if (null != securityIdentifier)
			{
				return securityIdentifier;
			}
			logVerbose(Strings.LookupUserAsDomainUser(user.ToString()));
			IEnumerable<ADRecipient> objects = user.GetObjects<ADRecipient>(null, session);
			using (IEnumerator<ADRecipient> enumerator = objects.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					securityIdentifier = ((IADSecurityPrincipal)enumerator.Current).Sid;
					if (enumerator.MoveNext())
					{
						logError(new ManagementObjectAmbiguousException(Strings.ErrorUserNotUnique(user.ToString())), ErrorCategory.InvalidData, null);
					}
					return securityIdentifier;
				}
			}
			securityIdentifier = SecurityPrincipalIdParameter.GetUserSidAsSAMAccount(user, logError, logVerbose);
			if (null == securityIdentifier)
			{
				logError(new ManagementObjectNotFoundException(Strings.ErrorUserNotFound(user.ToString())), ErrorCategory.InvalidData, null);
			}
			return securityIdentifier;
		}

		internal static SecurityIdentifier TryParseToSID(string sid)
		{
			if (sid == null)
			{
				throw new ArgumentNullException("sid");
			}
			SecurityIdentifier result;
			try
			{
				result = new SecurityIdentifier(sid);
			}
			catch (ArgumentException)
			{
				result = null;
			}
			return result;
		}

		internal override IEnumerable<T> GetObjects<T>(ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason)
		{
			notFoundReason = new LocalizedString?(LocalizedString.Empty);
			EnumerableWrapper<T> enumerableWrapper = EnumerableWrapper<T>.Empty;
			SecurityIdentifier sid = SecurityPrincipalIdParameter.TryParseToSID(base.RawIdentity);
			string userAccountNameFromSid = SecurityPrincipalIdParameter.GetUserAccountNameFromSid(sid, this.ToString(), null);
			if (!string.IsNullOrEmpty(userAccountNameFromSid))
			{
				enumerableWrapper = base.GetEnumerableWrapper<T>(enumerableWrapper, base.GetObjectsByAccountName<T>(userAccountNameFromSid, rootId, (IRecipientSession)session, optionalData));
				if (enumerableWrapper.HasElements())
				{
					return enumerableWrapper;
				}
			}
			enumerableWrapper = base.GetEnumerableWrapper<T>(enumerableWrapper, base.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason));
			if (enumerableWrapper.HasElements())
			{
				return enumerableWrapper;
			}
			sid = SecurityPrincipalIdParameter.GetUserSidAsSAMAccount(this, null, null);
			userAccountNameFromSid = SecurityPrincipalIdParameter.GetUserAccountNameFromSid(sid, this.ToString(), null);
			if (!string.IsNullOrEmpty(userAccountNameFromSid))
			{
				enumerableWrapper = base.GetEnumerableWrapper<T>(EnumerableWrapper<T>.Empty, base.GetObjectsByAccountName<T>(userAccountNameFromSid, rootId, (IRecipientSession)session, optionalData));
			}
			return enumerableWrapper;
		}

		protected override LocalizedString GetErrorMessageForWrongType(string id)
		{
			return Strings.WrongTypeSecurityPrincipal(id);
		}

		protected virtual SecurityPrincipalIdParameter CreateSidParameter(string identity)
		{
			return new SecurityPrincipalIdParameter(identity);
		}

		internal static SecurityIdentifier GetUserSidAsSAMAccount(SecurityPrincipalIdParameter user, Task.TaskErrorLoggingDelegate logError, Task.TaskVerboseLoggingDelegate logVerbose)
		{
			SecurityIdentifier securityIdentifier = null;
			if (logVerbose != null)
			{
				logVerbose(Strings.LookupUserAsSAMAccount(user.ToString()));
			}
			NTAccount ntaccount;
			try
			{
				ntaccount = new NTAccount(user.RawIdentity);
			}
			catch (ArgumentException)
			{
				if (logVerbose != null)
				{
					logVerbose(Strings.UserNotSAMAccount(user.ToString()));
				}
				return null;
			}
			try
			{
				securityIdentifier = (SecurityIdentifier)ntaccount.Translate(typeof(SecurityIdentifier));
			}
			catch (IdentityNotMappedException)
			{
			}
			catch (SystemException innerException)
			{
				if (logError != null)
				{
					logError(new LocalizedException(Strings.ForeignForestTrustFailedException(user.ToString()), innerException), ErrorCategory.InvalidOperation, null);
				}
				return null;
			}
			if (securityIdentifier == null)
			{
				securityIdentifier = SecurityPrincipalIdParameter.MapAliasToSid(user.RawIdentity);
			}
			return securityIdentifier;
		}

		private static string MapSidToAlias(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				return null;
			}
			string result;
			if (!SecurityPrincipalIdParameter.SidToAliasMap.TryGetValue(key, out result))
			{
				return null;
			}
			return result;
		}

		private static SecurityIdentifier MapAliasToSid(string alias)
		{
			foreach (KeyValuePair<string, string> keyValuePair in SecurityPrincipalIdParameter.SidToAliasMap)
			{
				if (string.Equals(alias, keyValuePair.Value, StringComparison.OrdinalIgnoreCase))
				{
					return new SecurityIdentifier(keyValuePair.Key);
				}
			}
			return null;
		}

		[OnSerializing]
		private void OnSerializing(StreamingContext context)
		{
			if (null != this.securityIdentifier)
			{
				this.binarySecurityIdentifier = ValueConvertor.ConvertValueToBinary(this.securityIdentifier, null);
			}
		}

		[OnSerialized]
		private void OnSerialized(StreamingContext context)
		{
			this.binarySecurityIdentifier = null;
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			if (this.binarySecurityIdentifier != null)
			{
				this.securityIdentifier = (SecurityIdentifier)ValueConvertor.ConvertValueFromBinary(this.binarySecurityIdentifier, typeof(SecurityIdentifier), null);
				this.binarySecurityIdentifier = null;
			}
		}

		private static string GetUserAccountNameFromSid(SecurityIdentifier sid, string user, Task.TaskErrorLoggingDelegate logError)
		{
			string result = null;
			if (sid != null && sid.IsAccountSid())
			{
				try
				{
					string text = sid.Translate(typeof(NTAccount)).ToString();
					string[] array = text.Split(new char[]
					{
						'\\'
					});
					if (array.Length == 2 && string.Compare(array[0], Environment.MachineName, StringComparison.OrdinalIgnoreCase) == 0 && !ADSession.IsBoundToAdam && logError != null)
					{
						logError(new CannotHaveLocalAccountException(user), ErrorCategory.InvalidData, null);
					}
					result = text;
				}
				catch (IdentityNotMappedException)
				{
				}
				catch (SystemException innerException)
				{
					if (logError != null)
					{
						logError(new LocalizedException(Strings.ForeignForestTrustFailedException(user), innerException), ErrorCategory.InvalidOperation, null);
					}
				}
			}
			return result;
		}

		private IEnumerable<T> GetUserAccountFromSid<T>(SecurityIdentifier sid, string user, Task.TaskErrorLoggingDelegate logError, ADObjectId rootId, IDirectorySession session, IDirectorySession subTreeSession, OptionalIdentityData optionalData, out LocalizedString? notFoundReason) where T : IConfigurable, new()
		{
			notFoundReason = null;
			string userAccountNameFromSid = SecurityPrincipalIdParameter.GetUserAccountNameFromSid(sid, user, logError);
			if (!string.IsNullOrEmpty(userAccountNameFromSid))
			{
				SecurityPrincipalIdParameter securityPrincipalIdParameter = this.CreateSidParameter(userAccountNameFromSid);
				return securityPrincipalIdParameter.GetObjects<T>(rootId, session, subTreeSession, optionalData, out notFoundReason);
			}
			return null;
		}

		internal new static readonly RecipientType[] AllowedRecipientTypes = new RecipientType[]
		{
			RecipientType.User,
			RecipientType.UserMailbox,
			RecipientType.MailUser,
			RecipientType.Group,
			RecipientType.MailUniversalSecurityGroup,
			RecipientType.MailNonUniversalGroup,
			RecipientType.Computer
		};

		private static object syncLock = new object();

		private static Dictionary<string, string> sidToAlias = null;

		[NonSerialized]
		private SecurityIdentifier securityIdentifier;

		private byte[] binarySecurityIdentifier;

		[NonSerialized]
		private bool returnUrlTokenEncodedString;
	}
}
