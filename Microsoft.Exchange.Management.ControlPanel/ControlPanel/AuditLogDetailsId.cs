using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class AuditLogDetailsId
	{
		public string Object { get; private set; }

		public string StartDate { get; private set; }

		public string EndDate { get; private set; }

		public AuditLogDetailsId(Identity identity)
		{
			this.Parse(identity.RawIdentity);
		}

		protected virtual void Parse(string rawIdentity)
		{
			string[] array = rawIdentity.Split(new char[]
			{
				';'
			});
			if (array.Length >= 3)
			{
				this.Object = array[0];
				this.StartDate = array[1];
				this.EndDate = array[2];
				return;
			}
			throw new FaultException(new ArgumentException("rawIdentity").Message);
		}
	}
}
