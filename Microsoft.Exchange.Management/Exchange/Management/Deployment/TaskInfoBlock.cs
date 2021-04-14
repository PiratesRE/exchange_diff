using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public abstract class TaskInfoBlock
	{
		public TaskInfoBlock()
		{
			this.useInstallTasks = false;
			this.isFatal = true;
			this.weight = 1;
		}

		[XmlAttribute]
		public string DescriptionId
		{
			get
			{
				if (this.descriptionId != null)
				{
					return this.descriptionId;
				}
				return string.Empty;
			}
			set
			{
				this.descriptionId = value;
			}
		}

		[XmlAttribute]
		public bool UseInstallTasks
		{
			get
			{
				return this.useInstallTasks;
			}
			set
			{
				this.useInstallTasks = value;
			}
		}

		[XmlAttribute]
		public int Weight
		{
			get
			{
				return this.weight;
			}
			set
			{
				this.weight = value;
			}
		}

		[XmlAttribute]
		public bool IsFatal
		{
			get
			{
				return this.isFatal;
			}
			set
			{
				this.isFatal = value;
			}
		}

		internal abstract string GetTask(InstallationCircumstances circumstance);

		private string descriptionId;

		private bool useInstallTasks;

		private int weight;

		private bool isFatal;
	}
}
