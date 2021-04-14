using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public abstract class TaskInfo
	{
		public TaskInfo()
		{
		}

		[XmlIgnore]
		public string FileId { get; set; }

		[XmlAttribute]
		public string Id { get; set; }

		[XmlAttribute]
		public string Component { get; set; }

		[XmlAttribute]
		public bool ExcludeInDatacenterDedicated { get; set; }

		public string GetTask(InstallationModes mode, InstallationCircumstances circumstance)
		{
			TaskInfoBlock taskInfoBlock = null;
			if (this.blocks.TryGetValue(mode, out taskInfoBlock) && (mode == InstallationModes.BuildToBuildUpgrade || mode == InstallationModes.DisasterRecovery) && taskInfoBlock.UseInstallTasks)
			{
				this.blocks.TryGetValue(InstallationModes.Install, out taskInfoBlock);
			}
			if (taskInfoBlock != null)
			{
				return taskInfoBlock.GetTask(circumstance);
			}
			return string.Empty;
		}

		public string GetDescription(InstallationModes mode)
		{
			string text = string.Empty;
			TaskInfoBlock taskInfoBlock = null;
			if (this.blocks.TryGetValue(mode, out taskInfoBlock))
			{
				text = taskInfoBlock.DescriptionId;
				if ((mode == InstallationModes.BuildToBuildUpgrade || mode == InstallationModes.DisasterRecovery) && taskInfoBlock.UseInstallTasks && string.IsNullOrEmpty(text) && this.blocks.TryGetValue(InstallationModes.Install, out taskInfoBlock))
				{
					text = taskInfoBlock.DescriptionId;
				}
			}
			return text ?? string.Empty;
		}

		public int GetWeight(InstallationModes mode)
		{
			int num = 1;
			TaskInfoBlock taskInfoBlock = null;
			if (this.blocks.TryGetValue(mode, out taskInfoBlock))
			{
				num = taskInfoBlock.Weight;
				if ((mode == InstallationModes.BuildToBuildUpgrade || mode == InstallationModes.DisasterRecovery) && taskInfoBlock.UseInstallTasks && num == 1 && this.blocks.TryGetValue(InstallationModes.Install, out taskInfoBlock))
				{
					num = taskInfoBlock.Weight;
				}
			}
			return num;
		}

		public bool IsFatal(InstallationModes mode)
		{
			bool flag = true;
			TaskInfoBlock taskInfoBlock = null;
			if (this.blocks.TryGetValue(mode, out taskInfoBlock))
			{
				flag = taskInfoBlock.IsFatal;
				if ((mode == InstallationModes.BuildToBuildUpgrade || mode == InstallationModes.DisasterRecovery) && taskInfoBlock.UseInstallTasks && flag && this.blocks.TryGetValue(InstallationModes.Install, out taskInfoBlock))
				{
					flag = taskInfoBlock.IsFatal;
				}
			}
			return flag;
		}

		public virtual string GetID()
		{
			if (string.IsNullOrEmpty(this.Id))
			{
				string text = this.GetTask(InstallationModes.Install, InstallationCircumstances.Standalone) + this.GetTask(InstallationModes.BuildToBuildUpgrade, InstallationCircumstances.Standalone) + this.GetTask(InstallationModes.DisasterRecovery, InstallationCircumstances.Standalone) + this.GetTask(InstallationModes.Uninstall, InstallationCircumstances.Standalone);
				return text.GetHashCode().ToString();
			}
			return string.Format("{0}__{1}", this.FileId, this.Id);
		}

		internal TTaskInfoBlock GetBlock<TTaskInfoBlock>(InstallationModes mode) where TTaskInfoBlock : TaskInfoBlock, new()
		{
			TaskInfoBlock taskInfoBlock = null;
			this.blocks.TryGetValue(mode, out taskInfoBlock);
			switch (mode)
			{
			case InstallationModes.Install:
			case InstallationModes.Uninstall:
				if (taskInfoBlock == null)
				{
					taskInfoBlock = Activator.CreateInstance<TTaskInfoBlock>();
					this.blocks[mode] = taskInfoBlock;
				}
				break;
			case InstallationModes.BuildToBuildUpgrade:
			case InstallationModes.DisasterRecovery:
				if (taskInfoBlock == null)
				{
					taskInfoBlock = Activator.CreateInstance<TTaskInfoBlock>();
					this.blocks[mode] = taskInfoBlock;
				}
				else if (taskInfoBlock.UseInstallTasks)
				{
					taskInfoBlock = this.GetBlock<TTaskInfoBlock>(InstallationModes.Install);
				}
				break;
			}
			return (TTaskInfoBlock)((object)taskInfoBlock);
		}

		internal void SetBlock(InstallationModes mode, TaskInfoBlock infoBlock)
		{
			this.blocks[mode] = infoBlock;
		}

		private Dictionary<InstallationModes, TaskInfoBlock> blocks = new Dictionary<InstallationModes, TaskInfoBlock>();
	}
}
