using System;
using System.Collections;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon.Exceptions;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class XsoPasswordImpl : IPassword
	{
		internal XsoPasswordImpl(UMMailboxRecipient mailbox)
		{
			this.mailbox = mailbox;
		}

		public int LockoutCount
		{
			get
			{
				object obj = this.Dictionary["LockoutCount"];
				if (obj == null)
				{
					return 0;
				}
				if (!(obj is int))
				{
					this.DeleteCorruptedPassword();
					return 0;
				}
				return (int)obj;
			}
			set
			{
				this.Dictionary["LockoutCount"] = value;
			}
		}

		public PasswordBlob CurrentPassword
		{
			get
			{
				if (null == this.cachedCurrentPwd)
				{
					object obj = this.Dictionary["Password"];
					if (obj == null)
					{
						return null;
					}
					if (!(obj is string))
					{
						this.DeleteCorruptedPassword();
						return null;
					}
					string s = (string)this.Dictionary["Password"];
					try
					{
						byte[] blobdata = Convert.FromBase64String(s);
						this.cachedCurrentPwd = new PasswordBlob(blobdata);
					}
					catch (FormatException)
					{
						this.DeleteCorruptedPassword();
						return null;
					}
					catch (UserConfigurationException)
					{
						this.DeleteCorruptedPassword();
						return null;
					}
				}
				return this.cachedCurrentPwd;
			}
			set
			{
				this.Dictionary["Password"] = Convert.ToBase64String(value.Blob);
				this.cachedCurrentPwd = value;
			}
		}

		public ExDateTime TimeSet
		{
			get
			{
				object obj = this.Dictionary["PasswordSetTime"];
				if (obj == null)
				{
					return ExDateTime.UtcNow.AddDays(-36501.0);
				}
				if (!(obj is ExDateTime))
				{
					this.DeleteCorruptedPassword();
					return ExDateTime.UtcNow.AddDays(-36501.0);
				}
				return (ExDateTime)obj;
			}
			set
			{
				this.Dictionary["PasswordSetTime"] = value;
			}
		}

		public ArrayList OldPasswords
		{
			get
			{
				if (this.cachedOldPwdList == null)
				{
					object obj = this.Dictionary["PreviousPasswords"];
					ArrayList arrayList = null;
					if (obj == null)
					{
						arrayList = new ArrayList();
					}
					else
					{
						if (!(obj is string))
						{
							this.DeleteCorruptedPassword();
							return null;
						}
						try
						{
							byte[] flat = Convert.FromBase64String((string)obj);
							arrayList = this.DeSerializeBlobs(flat);
						}
						catch (FormatException)
						{
							this.DeleteCorruptedPassword();
						}
						catch (UserConfigurationException)
						{
							this.DeleteCorruptedPassword();
						}
					}
					this.cachedOldPwdList = arrayList;
				}
				return this.cachedOldPwdList;
			}
			set
			{
				this.Dictionary["PreviousPasswords"] = Convert.ToBase64String(XsoPasswordImpl.SerializeBlobs(value));
				this.cachedOldPwdList = value;
			}
		}

		private string ConfigurationName
		{
			get
			{
				return "Um.Password";
			}
		}

		private IDictionary Dictionary
		{
			get
			{
				if (this.dictionary == null)
				{
					try
					{
						this.dictionary = this.CopyFromUserConfig();
					}
					catch (CorruptDataException)
					{
						this.DeleteCorruptedPassword();
					}
					catch (InvalidOperationException)
					{
						this.DeleteCorruptedPassword();
					}
				}
				return this.dictionary;
			}
		}

		public void Commit()
		{
			this.CopyToUserConfig(this.Dictionary);
			this.cachedCurrentPwd = null;
			this.cachedOldPwdList = null;
		}

		private static byte[] SerializeBlobs(ArrayList blobs)
		{
			int num = 0;
			int num2 = 0;
			foreach (object obj in blobs)
			{
				PasswordBlob passwordBlob = (PasswordBlob)obj;
				num2 += passwordBlob.Blob.Length;
			}
			byte[] array = new byte[num2];
			foreach (object obj2 in blobs)
			{
				PasswordBlob passwordBlob2 = (PasswordBlob)obj2;
				byte[] blob = passwordBlob2.Blob;
				blob.CopyTo(array, num);
				num += blob.Length;
			}
			return array;
		}

		private UserConfiguration GetConfig(MailboxSession session)
		{
			UserConfiguration result = null;
			try
			{
				result = session.UserConfigurationManager.GetMailboxConfiguration(this.ConfigurationName, UserConfigurationTypes.Dictionary);
			}
			catch (CorruptDataException)
			{
				this.DeleteCorruptedPassword();
			}
			catch (InvalidOperationException)
			{
				this.DeleteCorruptedPassword();
			}
			catch (ObjectNotFoundException)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.AuthenticationTracer, this, "Password file doesn't exist...creating.", new object[0]);
				result = session.UserConfigurationManager.CreateMailboxConfiguration(this.ConfigurationName, UserConfigurationTypes.Dictionary);
			}
			return result;
		}

		private ArrayList DeSerializeBlobs(byte[] flat)
		{
			ArrayList arrayList = new ArrayList();
			int i;
			PasswordBlob passwordBlob;
			for (i = 0; i < flat.Length; i += passwordBlob.Blob.Length)
			{
				passwordBlob = new PasswordBlob(flat, i);
				arrayList.Add(passwordBlob);
			}
			if (i != flat.Length)
			{
				throw new UserConfigurationException(Strings.CorruptedPasswordField(this.mailbox.ToString()));
			}
			return arrayList;
		}

		private void DeleteCorruptedPassword()
		{
			PIIMessage data = PIIMessage.Create(PIIType._EmailAddress, this.mailbox.MailAddress);
			CallIdTracer.TraceDebug(ExTraceGlobals.AuthenticationTracer, this, data, "Found a corrupted password file for user=_EmailAddress! Deleting!.", new object[0]);
			UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_CorruptedPIN, null, new object[]
			{
				this.mailbox
			});
			UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.mailbox.CreateSessionLock();
			try
			{
				mailboxSessionLock.Session.UserConfigurationManager.DeleteMailboxConfigurations(new string[]
				{
					this.ConfigurationName
				});
				throw new UserConfigurationException(Strings.CorruptedPIN(this.mailbox.MailAddress));
			}
			finally
			{
				if (mailboxSessionLock != null)
				{
					((IDisposable)mailboxSessionLock).Dispose();
					goto IL_9E;
				}
				goto IL_9E;
				IL_9E:;
			}
		}

		private IDictionary CopyFromUserConfig()
		{
			Hashtable hashtable = new Hashtable();
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.mailbox.CreateSessionLock())
			{
				using (UserConfiguration config = this.GetConfig(mailboxSessionLock.Session))
				{
					IDictionary dictionary = config.GetDictionary();
					foreach (object key in dictionary.Keys)
					{
						hashtable[key] = dictionary[key];
					}
				}
			}
			return hashtable;
		}

		private void CopyToUserConfig(IDictionary srcDictionary)
		{
			using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = this.mailbox.CreateSessionLock())
			{
				using (UserConfiguration config = this.GetConfig(mailboxSessionLock.Session))
				{
					IDictionary dictionary = config.GetDictionary();
					foreach (object key in srcDictionary.Keys)
					{
						dictionary[key] = srcDictionary[key];
					}
					config.Save();
				}
			}
		}

		private UMMailboxRecipient mailbox;

		private IDictionary dictionary;

		private PasswordBlob cachedCurrentPwd;

		private ArrayList cachedOldPwdList;
	}
}
