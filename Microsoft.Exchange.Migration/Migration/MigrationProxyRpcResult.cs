using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class MigrationProxyRpcResult
	{
		protected MigrationProxyRpcResult(MigrationProxyRpcType type)
		{
			this.Type = type;
			this.PropertyCollection = new MdbefPropertyCollection();
		}

		protected MigrationProxyRpcResult(byte[] resultBlob, MigrationProxyRpcType type)
		{
			MigrationUtil.ThrowOnNullArgument(resultBlob, "resultBlob");
			this.Type = type;
			this.PropertyCollection = MdbefPropertyCollection.Create(resultBlob, 0, resultBlob.Length);
		}

		public string ErrorMessage
		{
			get
			{
				object obj;
				if (this.PropertyCollection.TryGetValue(2432761887U, out obj))
				{
					return obj as string;
				}
				return null;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					this.PropertyCollection[2432761887U] = value;
					return;
				}
				this.PropertyCollection.Remove(2432761887U);
			}
		}

		public int RpcErrorCode
		{
			get
			{
				object obj;
				if (this.PropertyCollection.TryGetValue(2433548291U, out obj) && obj is int)
				{
					return (int)obj;
				}
				return 0;
			}
			set
			{
				if (value != 0)
				{
					this.PropertyCollection[2433548291U] = value;
					return;
				}
				this.PropertyCollection.Remove(2433548291U);
			}
		}

		public byte[] GetBytes()
		{
			return this.PropertyCollection.GetBytes();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("<Response Type='{0}'>", this.Type);
			foreach (KeyValuePair<uint, object> keyValuePair in this.PropertyCollection)
			{
				uint key = keyValuePair.Key;
				if (key <= 2433089567U)
				{
					if (key <= 2432892959U)
					{
						if (key == 2432761887U)
						{
							stringBuilder.AppendFormat("<Result Key='Exception' Value='{0}' />", keyValuePair.Value);
							continue;
						}
						if (key == 2432827395U)
						{
							stringBuilder.AppendFormat("<Result Key='NSPI Total Size' Value='{0}' />", keyValuePair.Value);
							continue;
						}
						if (key == 2432892959U)
						{
							stringBuilder.AppendFormat("<Result Key='NSPI Server' Value='{0}' />", keyValuePair.Value);
							continue;
						}
					}
					else
					{
						if (key == 2432958467U)
						{
							stringBuilder.AppendFormat("<Result Key='Autodiscover Status' Value='{0}' />", keyValuePair.Value);
							continue;
						}
						if (key == 2433024003U)
						{
							stringBuilder.AppendFormat("<Result Key='Autodiscover Exchange Version' Value='{0}' />", keyValuePair.Value);
							continue;
						}
						if (key == 2433089567U)
						{
							stringBuilder.AppendFormat("<Result Key='Autodiscover Mailbox DN' Value='{0}' />", keyValuePair.Value);
							continue;
						}
					}
				}
				else if (key <= 2433286175U)
				{
					if (key == 2433155103U)
					{
						stringBuilder.AppendFormat("<Result Key='Autodiscover Exchange Server DN' Value='{0}' />", keyValuePair.Value);
						continue;
					}
					if (key == 2433220639U)
					{
						stringBuilder.AppendFormat("<Result Key='Http Proxy Server' Value='{0}' />", keyValuePair.Value);
						continue;
					}
					if (key == 2433286175U)
					{
						stringBuilder.AppendFormat("<Result Key='Autodiscover Exchange Server' Value='{0}' />", keyValuePair.Value);
						continue;
					}
				}
				else if (key <= 2433417247U)
				{
					if (key == 2433351683U)
					{
						stringBuilder.AppendFormat("<Result Key='Autodiscover Authentication Method' Value='{0}' />", keyValuePair.Value);
						continue;
					}
					if (key == 2433417247U)
					{
						stringBuilder.AppendFormat("<Result Key='Autodiscover Url' Value='{0}' />", keyValuePair.Value);
						continue;
					}
				}
				else
				{
					if (key == 2433482755U)
					{
						stringBuilder.AppendFormat("<Result Key='Autodiscover Error Code' Value='{0}' />", keyValuePair.Value);
						continue;
					}
					if (key == 2433548291U)
					{
						stringBuilder.AppendFormat("<Result Key='RpcErrorCode' Value='{0}' />", keyValuePair.Value);
						continue;
					}
				}
				stringBuilder.AppendFormat("<InvalidResult/>", new object[0]);
			}
			stringBuilder.AppendFormat("</Response>", new object[0]);
			return stringBuilder.ToString();
		}

		public readonly MigrationProxyRpcType Type;

		protected readonly MdbefPropertyCollection PropertyCollection;
	}
}
