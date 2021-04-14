using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class NonOwnerAccessDetailsId : AuditLogDetailsId
	{
		public string LogonTypes { get; private set; }

		public NonOwnerAccessDetailsId(Identity identity) : base(identity)
		{
			this.Parse(identity.RawIdentity);
		}

		protected override void Parse(string rawIdentity)
		{
			string[] array = rawIdentity.Split(new char[]
			{
				';'
			});
			if (array.Length == 4)
			{
				this.LogonTypes = array[3];
				base.Parse(rawIdentity);
				return;
			}
			throw new FaultException(new ArgumentException("rawIdentity").Message);
		}
	}
}
