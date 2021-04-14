using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring.MicrosoftOnline
{
	[XmlInclude(typeof(Company1))]
	[XmlType(Namespace = "http://schemas.microsoft.com/online/directoryservices/change/2008/11")]
	[XmlInclude(typeof(StockKeepingUnit))]
	[XmlInclude(typeof(ServicePlan))]
	[DesignerCategory("code")]
	[XmlInclude(typeof(FeatureDescriptor))]
	[XmlInclude(typeof(KeyGroup))]
	[XmlInclude(typeof(SliceInstance))]
	[XmlInclude(typeof(ServicePrincipal))]
	[XmlInclude(typeof(Datacenter))]
	[XmlInclude(typeof(User1))]
	[XmlInclude(typeof(ThrottlePolicy))]
	[XmlInclude(typeof(TaskSet))]
	[XmlInclude(typeof(Task))]
	[XmlInclude(typeof(Subscription1))]
	[XmlInclude(typeof(SubscribedPlan))]
	[XmlInclude(typeof(ServiceInstance))]
	[XmlInclude(typeof(Service))]
	[XmlInclude(typeof(Scope))]
	[XmlInclude(typeof(RoleTemplate))]
	[XmlInclude(typeof(Role))]
	[XmlInclude(typeof(Region))]
	[XmlInclude(typeof(Group))]
	[XmlInclude(typeof(ForeignPrincipal))]
	[XmlInclude(typeof(Contract))]
	[XmlInclude(typeof(Contact))]
	[XmlInclude(typeof(Account1))]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[DebuggerStepThrough]
	[Serializable]
	public abstract class DirectoryObject
	{
		public DirectoryObject()
		{
			this.allField = false;
			this.deletedField = false;
		}

		[XmlAttribute]
		[DefaultValue(false)]
		public bool All
		{
			get
			{
				return this.allField;
			}
			set
			{
				this.allField = value;
			}
		}

		[XmlAttribute]
		public string ContextId
		{
			get
			{
				return this.contextIdField;
			}
			set
			{
				this.contextIdField = value;
			}
		}

		[XmlAttribute]
		public string ObjectId
		{
			get
			{
				return this.objectIdField;
			}
			set
			{
				this.objectIdField = value;
			}
		}

		[DefaultValue(false)]
		[XmlAttribute]
		public bool Deleted
		{
			get
			{
				return this.deletedField;
			}
			set
			{
				this.deletedField = value;
			}
		}

		private bool allField;

		private string contextIdField;

		private string objectIdField;

		private bool deletedField;
	}
}
