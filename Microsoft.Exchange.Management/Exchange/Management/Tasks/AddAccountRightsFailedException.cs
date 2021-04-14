using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AddAccountRightsFailedException : LocalizedException
	{
		public AddAccountRightsFailedException(string account, uint err) : base(Strings.AddAccountRightsFailedException(account, err))
		{
			this.account = account;
			this.err = err;
		}

		public AddAccountRightsFailedException(string account, uint err, Exception innerException) : base(Strings.AddAccountRightsFailedException(account, err), innerException)
		{
			this.account = account;
			this.err = err;
		}

		protected AddAccountRightsFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.account = (string)info.GetValue("account", typeof(string));
			this.err = (uint)info.GetValue("err", typeof(uint));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("account", this.account);
			info.AddValue("err", this.err);
		}

		public string Account
		{
			get
			{
				return this.account;
			}
		}

		public uint Err
		{
			get
			{
				return this.err;
			}
		}

		private readonly string account;

		private readonly uint err;
	}
}
