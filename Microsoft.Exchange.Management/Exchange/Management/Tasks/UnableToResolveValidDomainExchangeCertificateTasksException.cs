using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnableToResolveValidDomainExchangeCertificateTasksException : LocalizedException
	{
		public UnableToResolveValidDomainExchangeCertificateTasksException(string hostName, string physicalName, string fullyQualifiedName, string physicalFullyQualifiedName) : base(Strings.UnableToResolveValidDomainExchangeCertificateTasksException(hostName, physicalName, fullyQualifiedName, physicalFullyQualifiedName))
		{
			this.hostName = hostName;
			this.physicalName = physicalName;
			this.fullyQualifiedName = fullyQualifiedName;
			this.physicalFullyQualifiedName = physicalFullyQualifiedName;
		}

		public UnableToResolveValidDomainExchangeCertificateTasksException(string hostName, string physicalName, string fullyQualifiedName, string physicalFullyQualifiedName, Exception innerException) : base(Strings.UnableToResolveValidDomainExchangeCertificateTasksException(hostName, physicalName, fullyQualifiedName, physicalFullyQualifiedName), innerException)
		{
			this.hostName = hostName;
			this.physicalName = physicalName;
			this.fullyQualifiedName = fullyQualifiedName;
			this.physicalFullyQualifiedName = physicalFullyQualifiedName;
		}

		protected UnableToResolveValidDomainExchangeCertificateTasksException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.hostName = (string)info.GetValue("hostName", typeof(string));
			this.physicalName = (string)info.GetValue("physicalName", typeof(string));
			this.fullyQualifiedName = (string)info.GetValue("fullyQualifiedName", typeof(string));
			this.physicalFullyQualifiedName = (string)info.GetValue("physicalFullyQualifiedName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("hostName", this.hostName);
			info.AddValue("physicalName", this.physicalName);
			info.AddValue("fullyQualifiedName", this.fullyQualifiedName);
			info.AddValue("physicalFullyQualifiedName", this.physicalFullyQualifiedName);
		}

		public string HostName
		{
			get
			{
				return this.hostName;
			}
		}

		public string PhysicalName
		{
			get
			{
				return this.physicalName;
			}
		}

		public string FullyQualifiedName
		{
			get
			{
				return this.fullyQualifiedName;
			}
		}

		public string PhysicalFullyQualifiedName
		{
			get
			{
				return this.physicalFullyQualifiedName;
			}
		}

		private readonly string hostName;

		private readonly string physicalName;

		private readonly string fullyQualifiedName;

		private readonly string physicalFullyQualifiedName;
	}
}
