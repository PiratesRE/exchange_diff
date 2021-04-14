using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Core.LocStrings;

namespace Microsoft.Exchange.Configuration.Core
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class WinRMDataKeyNotFoundException : WinRMDataExchangeException
	{
		public WinRMDataKeyNotFoundException(string identity, string key) : base(Strings.WinRMDataKeyNotFound(identity, key))
		{
			this.identity = identity;
			this.key = key;
		}

		public WinRMDataKeyNotFoundException(string identity, string key, Exception innerException) : base(Strings.WinRMDataKeyNotFound(identity, key), innerException)
		{
			this.identity = identity;
			this.key = key;
		}

		protected WinRMDataKeyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.identity = (string)info.GetValue("identity", typeof(string));
			this.key = (string)info.GetValue("key", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("identity", this.identity);
			info.AddValue("key", this.key);
		}

		public string Identity
		{
			get
			{
				return this.identity;
			}
		}

		public string Key
		{
			get
			{
				return this.key;
			}
		}

		private readonly string identity;

		private readonly string key;
	}
}
