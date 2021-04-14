using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.WebServices.Autodiscover;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MigrationAutodiscoverGetUserSettingsRpcResult : MigrationProxyRpcResult
	{
		public MigrationAutodiscoverGetUserSettingsRpcResult() : base(MigrationProxyRpcType.GetUserSettings)
		{
		}

		public MigrationAutodiscoverGetUserSettingsRpcResult(byte[] resultBlob) : base(resultBlob, MigrationProxyRpcType.GetUserSettings)
		{
		}

		public ExchangeVersion? ExchangeVersion
		{
			get
			{
				object obj;
				if (this.PropertyCollection.TryGetValue(2433024003U, out obj) && obj is int)
				{
					return new ExchangeVersion?((ExchangeVersion)obj);
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					this.PropertyCollection[2433024003U] = value.Value;
					return;
				}
				this.PropertyCollection.Remove(2433024003U);
			}
		}

		public AutodiscoverClientStatus? Status
		{
			get
			{
				object obj;
				if (this.PropertyCollection.TryGetValue(2432958467U, out obj) && obj is int)
				{
					return new AutodiscoverClientStatus?((AutodiscoverClientStatus)obj);
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					this.PropertyCollection[2432958467U] = (int)value.Value;
					return;
				}
				this.PropertyCollection.Remove(2432958467U);
			}
		}

		public string MailboxDN
		{
			get
			{
				object obj;
				if (this.PropertyCollection.TryGetValue(2433089567U, out obj))
				{
					return obj as string;
				}
				return null;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					this.PropertyCollection[2433089567U] = value;
					return;
				}
				this.PropertyCollection.Remove(2433089567U);
			}
		}

		public string ExchangeServerDN
		{
			get
			{
				object obj;
				if (this.PropertyCollection.TryGetValue(2433155103U, out obj))
				{
					return obj as string;
				}
				return null;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					this.PropertyCollection[2433155103U] = value;
					return;
				}
				this.PropertyCollection.Remove(2433155103U);
			}
		}

		public string RpcProxyServer
		{
			get
			{
				object obj;
				if (this.PropertyCollection.TryGetValue(2433220639U, out obj))
				{
					string text = obj as string;
					if (!string.IsNullOrEmpty(text))
					{
						return text;
					}
				}
				return null;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					this.PropertyCollection[2433220639U] = value;
					return;
				}
				this.PropertyCollection.Remove(2433220639U);
			}
		}

		public string ExchangeServer
		{
			get
			{
				object obj;
				if (this.PropertyCollection.TryGetValue(2433286175U, out obj))
				{
					string text = obj as string;
					if (!string.IsNullOrEmpty(text))
					{
						return text;
					}
				}
				return null;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					this.PropertyCollection[2433286175U] = value;
					return;
				}
				this.PropertyCollection.Remove(2433286175U);
			}
		}

		public AuthenticationMethod? AuthenticationMethod
		{
			get
			{
				object obj;
				if (this.PropertyCollection.TryGetValue(2433351683U, out obj) && obj is int)
				{
					return new AuthenticationMethod?((AuthenticationMethod)obj);
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					this.PropertyCollection[2433351683U] = (int)value.Value;
					return;
				}
				this.PropertyCollection.Remove(2433351683U);
			}
		}

		public string AutodiscoverUrl
		{
			get
			{
				object obj;
				if (this.PropertyCollection.TryGetValue(2433417247U, out obj))
				{
					return obj as string;
				}
				return null;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					this.PropertyCollection[2433417247U] = value;
					return;
				}
				this.PropertyCollection.Remove(2433417247U);
			}
		}

		public AutodiscoverErrorCode? AutodiscoverErrorCode
		{
			get
			{
				object obj;
				if (this.PropertyCollection.TryGetValue(2433482755U, out obj) && obj is int)
				{
					return new AutodiscoverErrorCode?((AutodiscoverErrorCode)obj);
				}
				return null;
			}
			set
			{
				if (value != null)
				{
					this.PropertyCollection[2433482755U] = value.Value;
					return;
				}
				this.PropertyCollection.Remove(2433482755U);
			}
		}
	}
}
