using System;
using System.ServiceModel;
using Microsoft.Exchange.Management.ControlPanel;

namespace Microsoft.Exchange.Management.DDIService
{
	internal class AuditLogIdentity
	{
		internal AuditLogIdentity(Identity identity)
		{
			this.Parse(identity.RawIdentity);
		}

		public string StartDate { get; set; }

		public string EndDate { get; set; }

		public string ObjectId { get; set; }

		public string Cmdlet { get; set; }

		protected virtual void Parse(string rawIdentity)
		{
			string[] array = rawIdentity.Split(new char[]
			{
				'\n'
			});
			if (array.Length >= 3)
			{
				this.ObjectId = array[0];
				this.StartDate = array[1];
				this.EndDate = array[2];
				this.Cmdlet = array[3];
				return;
			}
			throw new FaultException(new ArgumentException("rawIdentity").Message);
		}
	}
}
