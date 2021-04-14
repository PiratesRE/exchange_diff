using System;
using System.ServiceModel;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class ExternalAccessLogDetailsId
	{
		public string ObjectModified { get; private set; }

		public string Cmdlet { get; private set; }

		public string StartDate { get; private set; }

		public string EndDate { get; private set; }

		public ExternalAccessLogDetailsId(Identity identity)
		{
			this.Parse(identity.RawIdentity);
		}

		protected virtual void Parse(string rawIdentity)
		{
			string[] array = rawIdentity.Split(new char[]
			{
				';'
			});
			if (array.Length >= 4)
			{
				this.ObjectModified = array[0];
				this.Cmdlet = array[1];
				this.StartDate = array[2];
				this.EndDate = array[3];
				return;
			}
			throw new FaultException(new ArgumentException("rawIdentity").Message);
		}
	}
}
