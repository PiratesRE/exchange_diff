using System;

namespace Microsoft.Exchange.Diagnostics
{
	[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
	public sealed class AccountingEnforcementAttribute : Attribute
	{
		public int RpcCount
		{
			get
			{
				return this.rpcCount;
			}
			set
			{
				this.rpcCount = value;
			}
		}

		public string Comments
		{
			get
			{
				return this.comments;
			}
			set
			{
				this.comments = value;
			}
		}

		private int rpcCount;

		private string comments;
	}
}
