using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class MigrationProxyRpcArgs
	{
		protected MigrationProxyRpcArgs(string userName, string encryptedPassword, string userDomain, MigrationProxyRpcType type)
		{
			this.Type = type;
			this.PropertyCollection = new MdbefPropertyCollection();
			this.UserName = userName;
			this.EncryptedUserPassword = encryptedPassword;
			this.UserDomain = userDomain;
		}

		protected MigrationProxyRpcArgs(byte[] requestBlob, MigrationProxyRpcType type)
		{
			MigrationUtil.ThrowOnNullArgument(requestBlob, "requestBlob");
			this.Type = type;
			this.PropertyCollection = MdbefPropertyCollection.Create(requestBlob, 0, requestBlob.Length);
		}

		public string UserName
		{
			get
			{
				return this.GetProperty<string>(2416115743U);
			}
			set
			{
				this.SetPropertyAsString(2416115743U, value);
			}
		}

		public string UserDomain
		{
			get
			{
				return this.GetProperty<string>(2416181279U);
			}
			set
			{
				this.SetPropertyAsString(2416181279U, value);
			}
		}

		public string EncryptedUserPassword
		{
			get
			{
				return this.GetProperty<string>(2416246815U);
			}
			set
			{
				this.SetPropertyAsString(2416246815U, value);
			}
		}

		public byte[] GetBytes()
		{
			return this.PropertyCollection.GetBytes();
		}

		public virtual bool Validate(out string errorMsg)
		{
			if (string.IsNullOrEmpty(this.UserName) || string.IsNullOrEmpty(this.EncryptedUserPassword))
			{
				errorMsg = "User Name or password cannot be null or empty.";
				return false;
			}
			errorMsg = null;
			return true;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("<Request Type='{0}'>", this.Type);
			foreach (KeyValuePair<uint, object> keyValuePair in this.PropertyCollection)
			{
				object arg = keyValuePair.Value;
				if (keyValuePair.Key == 2416246815U)
				{
					arg = "*****";
				}
				else if (keyValuePair.Key == 2416447508U)
				{
					int num = (keyValuePair.Value == null) ? 0 : ((long[])keyValuePair.Value).Length;
					arg = string.Format("{0} PropTag(s)", num);
				}
				stringBuilder.AppendFormat("<Argument Key='{0}' Value='{1}' />", keyValuePair.Key, arg);
			}
			stringBuilder.AppendFormat("</Request>", new object[0]);
			return stringBuilder.ToString();
		}

		protected T GetProperty<T>(uint key) where T : class
		{
			object obj;
			if (this.PropertyCollection.TryGetValue(key, out obj))
			{
				return (T)((object)obj);
			}
			return default(T);
		}

		protected void SetProperty(uint key, object value)
		{
			if (value != null)
			{
				this.PropertyCollection[key] = value;
				return;
			}
			this.PropertyCollection.Remove(key);
		}

		protected void SetPropertyAsString(uint key, string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				this.SetProperty(key, null);
				return;
			}
			this.SetProperty(key, value);
		}

		public readonly MigrationProxyRpcType Type;

		protected readonly MdbefPropertyCollection PropertyCollection;
	}
}
