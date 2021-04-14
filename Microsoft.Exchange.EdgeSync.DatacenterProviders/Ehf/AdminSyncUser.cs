using System;

namespace Microsoft.Exchange.EdgeSync.Ehf
{
	internal class AdminSyncUser
	{
		public AdminSyncUser(string distinguishedName, Guid objectGuid)
		{
			if (objectGuid == Guid.Empty)
			{
				throw new ArgumentException("objectGuid cannot be empty");
			}
			this.userGuid = objectGuid;
			this.distinguishedName = distinguishedName;
		}

		public Guid UserGuid
		{
			get
			{
				return this.userGuid;
			}
		}

		public string DistinguishedName
		{
			get
			{
				return this.distinguishedName;
			}
		}

		private Guid userGuid;

		private string distinguishedName;
	}
}
