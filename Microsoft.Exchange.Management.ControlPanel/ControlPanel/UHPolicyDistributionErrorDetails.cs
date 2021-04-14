using System;
using System.Runtime.Serialization;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.Tasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class UHPolicyDistributionErrorDetails
	{
		public UHPolicyDistributionErrorDetails(PolicyDistributionErrorDetails errorDetails)
		{
			this.Endpoint = errorDetails.Endpoint;
			this.ErrorCode = (int)errorDetails.ResultCode;
			this.ErrorMessage = errorDetails.ResultMessage;
			this.ObjectId = errorDetails.ObjectId;
			this.dateTime = errorDetails.LastResultTime;
			this.workload = errorDetails.Workload;
			this.objectType = errorDetails.ObjectType;
			if (errorDetails.ObjectType == ConfigurationObjectType.Scope)
			{
				this.Source = this.Endpoint;
				return;
			}
			this.Source = this.Workload;
		}

		[DataMember]
		public string Endpoint { get; private set; }

		[DataMember]
		public int ErrorCode { get; private set; }

		[DataMember]
		public string ErrorMessage { get; private set; }

		[DataMember]
		public string LastErrorTimeString
		{
			get
			{
				if (this.dateTime != null)
				{
					return this.dateTime.Value.ToString();
				}
				return null;
			}
			private set
			{
				this.dateTime = new DateTime?(DateTime.Parse(value));
			}
		}

		[DataMember]
		public DateTime LastErrorTimeDateTime
		{
			get
			{
				if (this.dateTime != null)
				{
					return this.dateTime.Value;
				}
				return DateTime.MinValue;
			}
			private set
			{
				this.dateTime = new DateTime?(value);
			}
		}

		[DataMember]
		public Guid ObjectId { get; private set; }

		[DataMember]
		public string Workload
		{
			get
			{
				switch (this.workload)
				{
				case Microsoft.Office.CompliancePolicy.PolicyConfiguration.Workload.Exchange:
					return Strings.CPPWorkloadExchange;
				case Microsoft.Office.CompliancePolicy.PolicyConfiguration.Workload.SharePoint:
					return Strings.CPPWorkloadSharePoint;
				case Microsoft.Office.CompliancePolicy.PolicyConfiguration.Workload.Intune:
					return Strings.CPPWorkloadIntune;
				}
				return Strings.CPPWorkloadNone;
			}
			private set
			{
				throw new NotImplementedException("The Workload property cannot be set at the moment; the getter returns the value of the private workload field, which is set in the constructor");
			}
		}

		[DataMember]
		public string ObjectType
		{
			get
			{
				switch (this.objectType)
				{
				case ConfigurationObjectType.Policy:
					return Strings.CPPConfigurationObjectTypePolicy;
				case ConfigurationObjectType.Rule:
					return Strings.CPPConfigurationObjectTypeRule;
				case ConfigurationObjectType.Association:
					return Strings.CPPConfigurationObjectTypeAssociation;
				case ConfigurationObjectType.Binding:
					return Strings.CPPConfigurationObjectTypeBinding;
				default:
					return Strings.CPPConfigurationObjectTypeScope;
				}
			}
			private set
			{
				throw new NotImplementedException("The ObjectType property cannot be set at the moment; the getter returns the value of the private objectType field, which is set in the constructor");
			}
		}

		[DataMember]
		public string Source { get; private set; }

		private DateTime? dateTime;

		private Workload workload;

		private ConfigurationObjectType objectType;
	}
}
