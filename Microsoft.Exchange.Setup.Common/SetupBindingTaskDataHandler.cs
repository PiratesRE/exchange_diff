using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Deployment;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SetupBindingTaskDataHandler : SetupSingleTaskDataHandler
	{
		public SetupBindingTaskDataHandler(BindingCategory category, ISetupContext context, MonadConnection connection) : base(context, "Start-" + category.ToString(), connection)
		{
			this.SelectedInstallableUnits = null;
			base.ImplementsDatacenterMode = true;
			base.ImplementsDatacenterDedicatedMode = true;
			base.ImplementsPartnerHostedMode = true;
			switch (category)
			{
			case BindingCategory.PreSetup:
				base.WorkUnit.Text = Strings.PreSetupText;
				break;
			case BindingCategory.PreFileCopy:
				base.WorkUnit.Text = Strings.PreFileCopyText;
				break;
			case BindingCategory.MidFileCopy:
				base.WorkUnit.Text = Strings.MidFileCopyText;
				break;
			case BindingCategory.PostFileCopy:
				base.WorkUnit.Text = Strings.PostFileCopyText;
				break;
			case BindingCategory.PostSetup:
				base.WorkUnit.Text = Strings.PostSetupText;
				break;
			}
			base.SetupContext = context;
		}

		public InstallationModes Mode
		{
			get
			{
				return this.mode;
			}
			set
			{
				this.mode = value;
			}
		}

		public List<string> SelectedInstallableUnits { get; set; }

		public Version PreviousVersion { get; set; }

		protected override void AddParameters()
		{
			base.AddParameters();
			if (base.SetupContext.IsDatacenter && base.SetupContext.IsFfo)
			{
				base.Parameters.AddWithValue("IsFfo", true);
			}
			if (this.PreviousVersion != null)
			{
				base.Parameters.AddWithValue("PreviousVersion", this.PreviousVersion);
			}
			base.Parameters.AddWithValue("Mode", this.Mode);
			base.Parameters.AddWithValue("Roles", this.GetRoleNames());
		}

		private string[] GetRoleNames()
		{
			List<string> list = new List<string>();
			foreach (string roleName in this.SelectedInstallableUnits)
			{
				Role roleByName = RoleManager.GetRoleByName(roleName);
				if (roleByName != null && (!(roleByName.RoleName == LanguagePacksRole.ClassRoleName) || (!base.SetupContext.IsCleanMachine && InstallationModes.Install == this.Mode)))
				{
					list.Add(roleByName.RoleName);
				}
			}
			return list.ToArray();
		}

		private InstallationModes mode;
	}
}
