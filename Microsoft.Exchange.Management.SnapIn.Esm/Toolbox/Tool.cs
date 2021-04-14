using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Text;
using Microsoft.Exchange.Management.SystemManager;
using Microsoft.Exchange.ManagementGUI;
using Microsoft.Exchange.ManagementGUI.Resources;
using Microsoft.ManagementGUI.WinForms;

namespace Microsoft.Exchange.Management.SnapIn.Esm.Toolbox
{
	public class Tool : IComparable<Tool>
	{
		static Tool()
		{
			string[] value = new string[]
			{
				Strings.ConfigTools,
				Strings.DeveloperTools,
				Strings.MailflowTools,
				Strings.PerfTools,
				Strings.SecurityTools,
				Strings.UnifiedMessagingTools
			};
			Tool.basicGroups.AddRange(value);
			Tool.availableGroups.AddRange(value);
			Tool.safeToolsList.Add("Performance Monitor", "ExchPrf.msc");
			Tool.safeToolsList.Add("Queue Viewer", "Exchange Queue Viewer.msc");
			Tool.safeToolsList.Add("Details Templates Editor", "Details Templates Editor.msc");
			Tool.safeToolsList.Add("Public Folder Management Console", "Public Folder Management Console.msc");
			Tool.nonEdgeToolsList.Add("Details Templates Editor");
			Tool.nonEdgeToolsList.Add("Public Folder Management Console");
			Tool.nonEdgeToolsList.Add("Message Tracking");
			Tool.nonEdgeToolsList.Add("Rbac Assignment");
			Tool.nonEdgeToolsList.Add("Call Statistics");
			Tool.nonEdgeToolsList.Add("User Call Logs");
			Tool.cloudAndRemoteOnPremiseToolsList.Add("Message Tracking");
			Tool.cloudAndRemoteOnPremiseToolsList.Add("Rbac Assignment");
			Tool.cloudAndRemoteOnPremiseToolsList.Add("Call Statistics");
			Tool.cloudAndRemoteOnPremiseToolsList.Add("User Call Logs");
			Tool.iconLibrary = new IconLibrary();
			Tool.iconLibrary.Icons.Add("Error", Icons.Error);
		}

		public Tool()
		{
			this.errorMessage = new StringBuilder(Strings.ToolErrorHeader);
		}

		public static IconLibrary ToolIcons
		{
			get
			{
				return Tool.iconLibrary;
			}
		}

		public static StringCollection AvailableGroups
		{
			get
			{
				return Tool.availableGroups;
			}
		}

		public string WorkingFolder
		{
			get
			{
				return this.workingFolder;
			}
			set
			{
				this.workingFolder = value;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public string Type
		{
			get
			{
				return this.type;
			}
			set
			{
				this.type = value;
			}
		}

		public string Description
		{
			get
			{
				return this.description;
			}
			set
			{
				this.description = value;
			}
		}

		public string GroupName
		{
			get
			{
				return this.groupName;
			}
			set
			{
				this.groupName = value;
			}
		}

		public int GroupId
		{
			get
			{
				return this.groupId;
			}
			set
			{
				this.groupId = value;
			}
		}

		public string IconKey
		{
			get
			{
				return this.iconKey;
			}
			set
			{
				this.iconKey = value;
			}
		}

		public string Assembly
		{
			get
			{
				return this.assembly;
			}
			set
			{
				this.assembly = value;
			}
		}

		public bool ValidTool
		{
			get
			{
				return this.validTool;
			}
			set
			{
				this.validTool = value;
			}
		}

		public bool NonEdgeTool
		{
			get
			{
				return this.nonEdgeTool;
			}
		}

		public bool CloudAndRemoteOnPremiseTool
		{
			get
			{
				return this.cloudAndRemoteOnPremiseTool;
			}
		}

		public string ErrorMessage
		{
			get
			{
				return this.errorMessage.ToString();
			}
		}

		public int LocalizedName
		{
			get
			{
				return this.localizedName;
			}
			set
			{
				this.localizedName = value;
			}
		}

		public int LocalizedDescription
		{
			get
			{
				return this.localizedDescription;
			}
			set
			{
				this.localizedDescription = value;
			}
		}

		public int LocalizedGroupName
		{
			get
			{
				return this.localizedGroupName;
			}
			set
			{
				this.localizedGroupName = value;
			}
		}

		public string Version
		{
			get
			{
				return this.version;
			}
			set
			{
				this.version = value;
			}
		}

		public string Command
		{
			get
			{
				return this.command;
			}
			set
			{
				this.command = value;
			}
		}

		public string CommandFile
		{
			get
			{
				return this.commandFile;
			}
			set
			{
				this.commandFile = value;
			}
		}

		public string CommandParameters
		{
			get
			{
				return this.commandParameters;
			}
			set
			{
				this.commandParameters = value;
			}
		}

		public string DataSource
		{
			get
			{
				return this.dataSource;
			}
			set
			{
				this.dataSource = value;
			}
		}

		public static bool IsToolDuplicate(DataList<Tool> toolList, Tool tool)
		{
			bool result = false;
			for (int i = 0; i < toolList.Count; i++)
			{
				if (toolList[i].CompareTo(tool) == 0)
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public void AddErrorMessage(string error)
		{
			this.errorMessage.Append(Strings.ToolboxErrorMessageFormat(error));
		}

		public virtual void Initialize()
		{
			if (!this.validTool)
			{
				return;
			}
			if (string.IsNullOrEmpty(this.name))
			{
				this.validTool = false;
				this.AddErrorMessage(Strings.NameError);
			}
			if (string.IsNullOrEmpty(this.type))
			{
				this.validTool = false;
				this.AddErrorMessage(Strings.TypeError);
			}
			else if (string.Compare(this.type, "SnapIn", true) != 0 && string.Compare(this.type, "Executable", true) != 0 && string.Compare(this.type, "MonadScript", true) != 0 && string.Compare(this.type, "DynamicURL", true) != 0 && string.Compare(this.type, "StaticURL", true) != 0)
			{
				this.validTool = false;
				this.AddErrorMessage(Strings.InvalidType(this.type));
			}
			if (string.IsNullOrEmpty(this.command) && string.Compare(this.type, "DynamicURL", true) != 0 && string.IsNullOrEmpty(this.command) && string.Compare(this.type, "StaticURL", true) != 0)
			{
				this.validTool = false;
				this.AddErrorMessage(Strings.CommandError);
			}
			SafeLibraryHandle safeLibraryHandle = new SafeLibraryHandle();
			if (string.IsNullOrEmpty(this.assembly))
			{
				this.validTool = false;
				this.AddErrorMessage(Strings.AssemblyError);
			}
			else
			{
				string text = this.assembly;
				if (!Path.IsPathRooted(this.assembly))
				{
					if (PSConnectionInfoSingleton.GetInstance().Type == OrganizationType.Cloud)
					{
						text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Path.GetFileName(text));
					}
					else
					{
						text = Path.Combine(this.workingFolder, this.assembly);
					}
				}
				if (!File.Exists(text))
				{
					this.validTool = false;
					this.AddErrorMessage(Strings.AssemblyMissing(this.assembly));
				}
				else
				{
					if (safeLibraryHandle != null)
					{
						safeLibraryHandle.Dispose();
						safeLibraryHandle = null;
					}
					safeLibraryHandle = SafeLibraryHandle.LoadLibrary(text);
				}
			}
			if (string.Compare(this.type, "MonadScript", true) == 0 || string.Compare(this.type, "SnapIn", true) == 0)
			{
				if (string.IsNullOrEmpty(this.commandFile))
				{
					this.validTool = false;
					this.AddErrorMessage(Strings.CommandFileError(this.name));
				}
				else if (string.Compare(this.type, "SnapIn", true) == 0)
				{
					if (string.Compare(Path.GetFileName(this.command), "mmc.exe", true) != 0)
					{
						this.validTool = false;
						this.AddErrorMessage(Strings.InvalidSnapinTool(this.command));
					}
					else
					{
						string empty = string.Empty;
						if (!Tool.safeToolsList.TryGetValue(this.name, out empty))
						{
							this.validTool = false;
							this.AddErrorMessage(Strings.SnapInNotInSafeList(this.name));
						}
						else if (string.Compare(Path.GetFileName(this.commandFile.Replace("\"", "")), empty, true) != 0)
						{
							this.validTool = false;
							this.AddErrorMessage(Strings.SnapInCommandFileNotInSafeList(this.commandFile, this.name));
						}
					}
				}
				else if (string.Compare(Path.GetFileName(this.command), "PowerShell.exe", true) != 0)
				{
					this.validTool = false;
					this.AddErrorMessage(Strings.InvalidCmdletTool(this.command));
				}
				else
				{
					string empty2 = string.Empty;
					if (!Tool.safeToolsList.TryGetValue(this.name, out empty2))
					{
						this.validTool = false;
						this.AddErrorMessage(Strings.SnapInNotInSafeList(this.name));
					}
					else if (string.Compare(Path.GetFileName(this.commandFile.Replace("\"", "")), empty2, true) != 0)
					{
						this.validTool = false;
						this.AddErrorMessage(Strings.SnapInCommandFileNotInSafeList(this.commandFile, this.name));
					}
				}
			}
			else if (string.Compare(this.type, "Executable", true) == 0)
			{
				if (string.IsNullOrEmpty(this.command))
				{
					this.validTool = false;
					this.AddErrorMessage(Strings.CommandError);
				}
				else
				{
					string empty3 = string.Empty;
					if (!Tool.safeToolsList.TryGetValue(this.name, out empty3))
					{
						this.validTool = false;
						this.AddErrorMessage(Strings.ExecutableNotInSafeList(this.name));
					}
					else if (string.Compare(Path.GetFileName(this.command), empty3, true) != 0)
					{
						this.validTool = false;
						this.AddErrorMessage(Strings.ExecutableCommandNotInSafeList(this.command, this.name));
					}
				}
			}
			try
			{
				this.nonEdgeTool = Tool.nonEdgeToolsList.Contains(this.Name);
				this.cloudAndRemoteOnPremiseTool = Tool.cloudAndRemoteOnPremiseToolsList.Contains(this.Name);
				if (!safeLibraryHandle.IsInvalid)
				{
					if (this.localizedDescription != 0)
					{
						string @string = this.GetString(this.localizedDescription, safeLibraryHandle);
						if (!string.IsNullOrEmpty(@string))
						{
							this.description = @string;
						}
					}
					if (this.localizedGroupName != 0)
					{
						string @string = this.GetString(this.LocalizedGroupName, safeLibraryHandle);
						if (!string.IsNullOrEmpty(@string))
						{
							this.groupName = @string;
						}
					}
					if (this.localizedName != 0)
					{
						string @string = this.GetString(this.LocalizedName, safeLibraryHandle);
						if (!string.IsNullOrEmpty(@string))
						{
							this.name = @string;
						}
					}
				}
				this.LoadIcon(safeLibraryHandle);
			}
			finally
			{
				safeLibraryHandle.Dispose();
			}
			this.UpdateGroup();
		}

		private string GetString(int resourceId, SafeLibraryHandle moduleHandle)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = NativeMethods.LoadString(moduleHandle, resourceId, stringBuilder, 0);
			if (num != 0)
			{
				stringBuilder.EnsureCapacity(num + 1);
				NativeMethods.LoadString(moduleHandle, resourceId, stringBuilder, stringBuilder.Capacity);
			}
			return stringBuilder.ToString();
		}

		private void LoadIcon(SafeLibraryHandle moduleHandle)
		{
			Icon icon = null;
			int value;
			if (!string.IsNullOrEmpty(this.IconKey) && !moduleHandle.IsInvalid && int.TryParse(this.IconKey, out value))
			{
				IntPtr intPtr = NativeMethods.LoadIcon(moduleHandle, new IntPtr(value));
				if (intPtr != IntPtr.Zero)
				{
					icon = Icon.FromHandle(intPtr);
				}
			}
			if (icon != null)
			{
				if (Tool.ToolIcons.Icons[this.name] == null)
				{
					Tool.ToolIcons.Icons.Add(this.name, icon);
				}
				this.IconKey = this.name;
				return;
			}
			if (string.Compare(this.type, "SnapIn", true) == 0)
			{
				this.IconKey = "SnapIn";
				return;
			}
			if (string.Compare(this.type, "MonadScript", true) == 0)
			{
				this.IconKey = "MonadScript";
				return;
			}
			if (string.Compare(this.type, "Executable", true) == 0)
			{
				this.IconKey = "Executable";
				return;
			}
			this.IconKey = "Error";
		}

		private void UpdateGroup()
		{
			bool flag = false;
			if (this.GroupId == 0)
			{
				if (!string.IsNullOrEmpty(this.GroupName))
				{
					bool flag2 = false;
					for (int i = 0; i < Tool.availableGroups.Count; i++)
					{
						if (string.Compare(Tool.availableGroups[i], this.GroupName, true) == 0)
						{
							flag2 = true;
							this.GroupId = i + 1;
							break;
						}
					}
					if (!flag2)
					{
						Tool.availableGroups.Add(this.GroupName);
						this.GroupId = Tool.availableGroups.IndexOf(this.GroupName) + 1;
					}
					flag = true;
				}
				else
				{
					this.AddErrorMessage(Strings.GroupError);
				}
			}
			else if (Tool.basicGroups.Count >= this.GroupId)
			{
				this.GroupName = Tool.basicGroups[this.GroupId - 1];
				flag = true;
			}
			else
			{
				this.AddErrorMessage(Strings.InvalidGroupId(this.GroupId));
			}
			if (!flag)
			{
				this.validTool = false;
			}
		}

		public int CompareTo(Tool item)
		{
			return string.Compare(this.name, item.Name);
		}

		private static IconLibrary iconLibrary;

		private static StringCollection basicGroups = new StringCollection();

		private static StringCollection availableGroups = new StringCollection();

		private static Dictionary<string, string> safeToolsList = new Dictionary<string, string>();

		private static StringCollection nonEdgeToolsList = new StringCollection();

		private static StringCollection cloudAndRemoteOnPremiseToolsList = new StringCollection();

		private bool nonEdgeTool;

		private bool cloudAndRemoteOnPremiseTool;

		private string name = string.Empty;

		private string description;

		private string groupName;

		private int localizedName;

		private int localizedGroupName;

		private int localizedDescription;

		private StringBuilder errorMessage;

		private string command;

		private string commandFile;

		private string commandParameters;

		private string assembly;

		private string type;

		private string workingFolder;

		private string dataSource;

		private int groupId;

		private string version;

		private string iconKey;

		private bool validTool = true;
	}
}
