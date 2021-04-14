using System;
using System.Security;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.SnapIn.Esm.Toolbox
{
	public class RegistryDataProvider : DataProvider
	{
		public RegistryDataProvider(RegistryKey baseRegKey, string regKey, string workingFolder)
		{
			this.baseRegKey = baseRegKey;
			this.subKey = regKey;
			this.workingFolder = workingFolder;
		}

		public override void Query()
		{
			base.Tools.Clear();
			RegistryKey registryKey = null;
			try
			{
				registryKey = this.baseRegKey.OpenSubKey(this.subKey);
				if (registryKey != null && registryKey.SubKeyCount != 0)
				{
					string[] subKeyNames = registryKey.GetSubKeyNames();
					RegistryKey registryKey2 = null;
					for (int i = 0; i < subKeyNames.Length; i++)
					{
						Tool tool = new Tool();
						try
						{
							tool.DataSource = subKeyNames[i];
							tool.WorkingFolder = this.workingFolder;
							registryKey2 = registryKey.OpenSubKey(subKeyNames[i]);
							string[] valueNames = registryKey2.GetValueNames();
							if (valueNames == null || valueNames.Length == 0)
							{
								tool.ValidTool = false;
								tool.AddErrorMessage(Strings.InvalidRegistryEntry(subKeyNames[i]));
							}
							else
							{
								tool.Name = registryKey2.GetValue("Name", string.Empty).ToString();
								tool.Description = registryKey2.GetValue("Description", string.Empty).ToString();
								tool.Assembly = Environment.ExpandEnvironmentVariables(registryKey2.GetValue("Assembly", string.Empty).ToString());
								tool.Command = Environment.ExpandEnvironmentVariables(registryKey2.GetValue("Command", string.Empty).ToString());
								tool.CommandFile = Environment.ExpandEnvironmentVariables(registryKey2.GetValue("CommandFile", string.Empty).ToString());
								tool.CommandParameters = Environment.ExpandEnvironmentVariables(registryKey2.GetValue("CommandParameters", string.Empty).ToString());
								tool.GroupName = registryKey2.GetValue("GroupName", string.Empty).ToString();
								tool.Type = registryKey2.GetValue("Type", string.Empty).ToString();
								int num;
								if (int.TryParse(registryKey2.GetValue("LocalizedName", 0).ToString(), out num))
								{
									tool.LocalizedName = num;
								}
								if (int.TryParse(registryKey2.GetValue("LocalizedDescription", 0).ToString(), out num))
								{
									tool.LocalizedDescription = num;
								}
								if (int.TryParse(registryKey2.GetValue("LocalizedGroupName", 0).ToString(), out num))
								{
									tool.LocalizedGroupName = num;
								}
								if (int.TryParse(registryKey2.GetValue("GroupId", 0).ToString(), out num))
								{
									tool.GroupId = num;
								}
								if (int.TryParse(registryKey2.GetValue("Icon", 0).ToString(), out num))
								{
									tool.IconKey = num.ToString();
								}
								tool.Version = registryKey2.GetValue("Version", string.Empty).ToString();
							}
						}
						catch (SecurityException)
						{
							tool.ValidTool = false;
							tool.AddErrorMessage(Strings.InaccessibleRegistryEntry(subKeyNames[i]));
						}
						finally
						{
							if (registryKey2 != null)
							{
								registryKey2.Close();
							}
						}
						tool.Initialize();
						base.Tools.Add(tool);
					}
				}
			}
			catch (SecurityException)
			{
			}
			finally
			{
				if (registryKey != null)
				{
					registryKey.Close();
				}
			}
		}

		private string workingFolder;

		private RegistryKey baseRegKey;

		private string subKey;
	}
}
