using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Directory.TopologyDiscovery;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SuitabilityDirectoryException : SuitabilityException
	{
		public SuitabilityDirectoryException(string fqdn, int error, string errorMessage) : base(DirectoryStrings.SuitabilityDirectoryException(fqdn, error, errorMessage))
		{
			this.fqdn = fqdn;
			this.error = error;
			this.errorMessage = errorMessage;
		}

		public SuitabilityDirectoryException(string fqdn, int error, string errorMessage, Exception innerException) : base(DirectoryStrings.SuitabilityDirectoryException(fqdn, error, errorMessage), innerException)
		{
			this.fqdn = fqdn;
			this.error = error;
			this.errorMessage = errorMessage;
		}

		protected SuitabilityDirectoryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.fqdn = (string)info.GetValue("fqdn", typeof(string));
			this.error = (int)info.GetValue("error", typeof(int));
			this.errorMessage = (string)info.GetValue("errorMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("fqdn", this.fqdn);
			info.AddValue("error", this.error);
			info.AddValue("errorMessage", this.errorMessage);
		}

		public string Fqdn
		{
			get
			{
				return this.fqdn;
			}
		}

		public int Error
		{
			get
			{
				return this.error;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.errorMessage;
			}
		}

		private readonly string fqdn;

		private readonly int error;

		private readonly string errorMessage;
	}
}
