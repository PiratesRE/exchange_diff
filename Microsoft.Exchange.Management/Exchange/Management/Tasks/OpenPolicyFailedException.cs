using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class OpenPolicyFailedException : LocalizedException
	{
		public OpenPolicyFailedException(uint err, string account, string dom) : base(Strings.OpenPolicyFailedException(err, account, dom))
		{
			this.err = err;
			this.account = account;
			this.dom = dom;
		}

		public OpenPolicyFailedException(uint err, string account, string dom, Exception innerException) : base(Strings.OpenPolicyFailedException(err, account, dom), innerException)
		{
			this.err = err;
			this.account = account;
			this.dom = dom;
		}

		protected OpenPolicyFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.err = (uint)info.GetValue("err", typeof(uint));
			this.account = (string)info.GetValue("account", typeof(string));
			this.dom = (string)info.GetValue("dom", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("err", this.err);
			info.AddValue("account", this.account);
			info.AddValue("dom", this.dom);
		}

		public uint Err
		{
			get
			{
				return this.err;
			}
		}

		public string Account
		{
			get
			{
				return this.account;
			}
		}

		public string Dom
		{
			get
			{
				return this.dom;
			}
		}

		private readonly uint err;

		private readonly string account;

		private readonly string dom;
	}
}
