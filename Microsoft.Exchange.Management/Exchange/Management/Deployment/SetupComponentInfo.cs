using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[XmlInclude(typeof(SetupComponentInfo))]
	public class SetupComponentInfo
	{
		[XmlAttribute]
		public string DescriptionId
		{
			get
			{
				if (this.descriptionId == null)
				{
					this.descriptionId = string.Empty;
				}
				return this.descriptionId;
			}
			set
			{
				this.descriptionId = value;
			}
		}

		public ServerTaskInfoCollection ServerTasks
		{
			get
			{
				if (this.serverTasks == null)
				{
					this.serverTasks = new ServerTaskInfoCollection();
				}
				return this.serverTasks;
			}
			set
			{
				this.serverTasks = value;
			}
		}

		public OrgTaskInfoCollection OrgTasks
		{
			get
			{
				if (this.orgTasks == null)
				{
					this.orgTasks = new OrgTaskInfoCollection();
				}
				return this.orgTasks;
			}
			set
			{
				this.orgTasks = value;
			}
		}

		public ServicePlanTaskInfoCollection ServicePlanOrgTasks
		{
			get
			{
				if (this.servicePlanOrgTasks == null)
				{
					this.servicePlanOrgTasks = new ServicePlanTaskInfoCollection();
				}
				return this.servicePlanOrgTasks;
			}
			set
			{
				this.servicePlanOrgTasks = value;
			}
		}

		public TaskInfoCollection Tasks
		{
			get
			{
				if (this.tasks == null)
				{
					this.tasks = new TaskInfoCollection();
				}
				return this.tasks;
			}
		}

		internal void PopulateTasksProperty(string fileId)
		{
			if (this.tasks == null)
			{
				this.tasks = new TaskInfoCollection();
			}
			if (this.tasks.Count == 0)
			{
				foreach (ServerTaskInfo serverTaskInfo in this.ServerTasks)
				{
					serverTaskInfo.FileId = fileId;
					this.tasks.Add(serverTaskInfo);
				}
				foreach (OrgTaskInfo orgTaskInfo in this.OrgTasks)
				{
					orgTaskInfo.FileId = fileId;
					this.tasks.Add(orgTaskInfo);
				}
				foreach (ServicePlanTaskInfo item in this.ServicePlanOrgTasks)
				{
					this.tasks.Add(item);
				}
			}
		}

		internal void ValidateDatacenterAttributes()
		{
			if (this.DatacenterMode == DatacenterMode.ExO && !this.IsDatacenterOnly)
			{
				throw new LocalizedException(Strings.ErrorCannotBeExOWithoutDatacenter(this.Name), null);
			}
			if (this.DatacenterMode == DatacenterMode.Ffo && !this.IsDatacenterOnly)
			{
				throw new LocalizedException(Strings.ErrorCannotBeFfoWithoutDatacenter(this.Name), null);
			}
		}

		[XmlAttribute]
		public string Name;

		private string descriptionId;

		[XmlAttribute]
		public bool AlwaysExecute;

		[XmlAttribute]
		public bool IsDatacenterOnly;

		[XmlAttribute]
		public bool IsPartnerHostedOnly;

		[XmlAttribute]
		[DefaultValue(DatacenterMode.Common)]
		public DatacenterMode DatacenterMode;

		[XmlAttribute]
		public bool IsDatacenterDedicatedOnly;

		private ServerTaskInfoCollection serverTasks;

		private OrgTaskInfoCollection orgTasks;

		private ServicePlanTaskInfoCollection servicePlanOrgTasks;

		private TaskInfoCollection tasks;
	}
}
