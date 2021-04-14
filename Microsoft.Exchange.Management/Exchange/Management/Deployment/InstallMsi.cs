using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Cmdlet("Install", "MsiPackage")]
	public sealed class InstallMsi : MsiTaskBase
	{
		public InstallMsi()
		{
			base.Activity = Strings.UnpackingFiles;
			base.Fields["Reinstall"] = false;
			base.Fields["ReinstallMode"] = "vmous";
			base.Fields["NoActionIfInstalled"] = false;
		}

		[Parameter(Mandatory = false)]
		public LongPath UpdatesDir
		{
			get
			{
				return (LongPath)base.Fields["UpdatesDir"];
			}
			set
			{
				base.Fields["UpdatesDir"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string PackagePath
		{
			get
			{
				return (string)base.Fields["PackagePath"];
			}
			set
			{
				base.Fields["PackagePath"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath TargetDirectory
		{
			get
			{
				return (LocalLongFullPath)base.Fields["TargetDirectory"];
			}
			set
			{
				base.Fields["TargetDirectory"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool Reinstall
		{
			get
			{
				return (bool)base.Fields["Reinstall"];
			}
			set
			{
				base.Fields["Reinstall"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ReinstallMode
		{
			get
			{
				return (string)base.Fields["ReinstallMode"];
			}
			set
			{
				base.Fields["ReinstallMode"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter NoActionIfInstalled
		{
			get
			{
				return new SwitchParameter((bool)base.Fields["NoActionIfInstalled"]);
			}
			set
			{
				base.Fields["NoActionIfInstalled"] = value.ToBool();
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			try
			{
				this.newProduct = !MsiUtility.IsInstalled(this.PackagePath);
			}
			catch (ArgumentNullException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidArgument, null);
			}
			catch (ArgumentOutOfRangeException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidArgument, null);
			}
			catch (TaskException exception3)
			{
				base.WriteError(exception3, ErrorCategory.InvalidOperation, null);
			}
			if (!this.NoActionIfInstalled || this.newProduct)
			{
				if (!this.Reinstall)
				{
					if (this.newProduct && this.TargetDirectory != null && this.TargetDirectory.ToString() != string.Empty)
					{
						base.PropertyValues = string.Format("{0} TARGETDIR=\"{1}\"", base.PropertyValues, this.TargetDirectory);
					}
					if (base.Features != null && base.Features.Length != 0)
					{
						List<string> list = new List<string>(base.Features);
						bool flag = string.Equals("ExchangeServer.MSI", Path.GetFileName(this.PackagePath), StringComparison.OrdinalIgnoreCase);
						if (flag)
						{
							bool flag2 = false;
							bool flag3 = false;
							foreach (string b in list)
							{
								if (string.Equals("Gateway", b, StringComparison.OrdinalIgnoreCase))
								{
									flag2 = true;
								}
								else if (string.Equals("AdminTools", b, StringComparison.OrdinalIgnoreCase))
								{
									flag3 = true;
								}
							}
							if (flag2 && !flag3)
							{
								base.PropertyValues = string.Format("{0} REMOVE={1}", base.PropertyValues, "AdminToolsNonGateway");
							}
							if (!flag2 && flag3)
							{
								list.Add("AdminToolsNonGateway");
							}
						}
						base.PropertyValues = string.Format("{0} ADDLOCAL={1}", base.PropertyValues, string.Join(",", list.ToArray()));
					}
				}
				else
				{
					base.PropertyValues = string.Format("{0} REINSTALL=ALL REINSTALLMODE={1}", base.PropertyValues, this.ReinstallMode);
				}
				if (!base.Fields.IsChanged("LogFile"))
				{
					base.LogFile = Path.Combine(ConfigurationContext.Setup.SetupLoggingPath, Path.ChangeExtension(Path.GetFileName(this.PackagePath), ".msilog"));
				}
				string fullPath = Path.GetFullPath(this.PackagePath);
				base.WriteVerbose(Strings.MsiFullPackagePath(this.PackagePath, fullPath));
				this.PackagePath = fullPath;
			}
			else
			{
				base.WriteVerbose(Strings.MsiPackageAlreadyInstalled(this.PackagePath));
			}
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			if (!File.Exists(this.PackagePath))
			{
				base.WriteError(new FileNotFoundException(this.PackagePath), ErrorCategory.InvalidData, null);
			}
			if (this.Reinstall)
			{
				string packageName = null;
				try
				{
					packageName = MsiUtility.GetProductInfo(MsiUtility.GetProductCode(this.PackagePath), InstallProperty.PackageName);
				}
				catch (ArgumentOutOfRangeException exception)
				{
					base.WriteError(exception, ErrorCategory.InvalidArgument, null);
				}
				catch (TaskException exception2)
				{
					base.WriteError(exception2, ErrorCategory.InvalidOperation, null);
				}
				if (this.newProduct)
				{
					base.WriteError(new TaskException(Strings.MsiIsNotInstalled(packageName)), ErrorCategory.InvalidArgument, 0);
				}
				else if (base.Features != null && base.Features.Length != 0)
				{
					base.WriteError(new TaskException(Strings.MsiReinstallMustAll), ErrorCategory.InvalidArgument, 0);
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (!this.NoActionIfInstalled || this.newProduct)
			{
				base.UpdateProgress(0);
				this.InstallPackageFile(this.PackagePath);
			}
			TaskLogger.LogExit();
		}

		internal void InstallPackageFile(string packagePath)
		{
			TaskLogger.LogEnter();
			try
			{
				Guid productCode = MsiUtility.GetProductCode(this.PackagePath);
				base.SetLogging();
				MsiUtility.PushExternalUI(base.UIHandler, (InstallLogMode)10115);
				try
				{
					base.WriteVerbose(Strings.InstallingPackage(packagePath));
					List<MsiNativeMethods.PatchSequenceInfo> list = new List<MsiNativeMethods.PatchSequenceInfo>();
					uint num;
					if (this.UpdatesDir != null)
					{
						try
						{
							string[] files = Directory.GetFiles(this.UpdatesDir.PathName, "*.msp");
							foreach (string patchData in files)
							{
								MsiNativeMethods.PatchSequenceInfo item = new MsiNativeMethods.PatchSequenceInfo
								{
									patchData = patchData,
									patchDataType = PatchDataType.PatchFile,
									order = -1,
									status = 0U
								};
								list.Add(item);
								base.WriteVerbose(Strings.PatchFileFound(item.patchData));
							}
						}
						catch (DirectoryNotFoundException)
						{
							base.WriteVerbose(Strings.UpdatesDirectoryNotFound(this.UpdatesDir.PathName));
						}
						catch (IOException exception)
						{
							base.WriteError(exception, ErrorCategory.InvalidOperation, null);
						}
						if (list.Count > 0)
						{
							MsiNativeMethods.PatchSequenceInfo[] array2 = list.ToArray();
							num = MsiNativeMethods.DetermineApplicablePatches(this.PackagePath, array2.Length, array2);
							if (num != 0U)
							{
								base.WriteError(new TaskException(Strings.FailedToSortPatches(num)), ErrorCategory.InvalidOperation, null);
							}
							else
							{
								base.WriteVerbose(Strings.SortedAvailablePatches);
								List<MsiNativeMethods.PatchSequenceInfo> list2 = new List<MsiNativeMethods.PatchSequenceInfo>();
								foreach (MsiNativeMethods.PatchSequenceInfo item2 in array2)
								{
									base.WriteVerbose(Strings.PrintPatchOrderInfo(item2.patchData, item2.order, item2.status));
									if (item2.order >= 0)
									{
										list2.Add(item2);
									}
								}
								if (list2.Count > 0)
								{
									list2.Sort(new Comparison<MsiNativeMethods.PatchSequenceInfo>(MsiNativeMethods.ComparePatchSequence));
									StringBuilder stringBuilder = new StringBuilder();
									foreach (MsiNativeMethods.PatchSequenceInfo patchSequenceInfo in list2)
									{
										base.WriteVerbose(Strings.ValidPatch(patchSequenceInfo.patchData, patchSequenceInfo.order));
										stringBuilder.Append(patchSequenceInfo.patchData);
										stringBuilder.Append(";");
									}
									base.WriteVerbose(Strings.PatchAttributeValue(stringBuilder.ToString()));
									base.PropertyValues = string.Format("PATCH=\"{0}\" {1}", stringBuilder.ToString(), base.PropertyValues);
								}
							}
						}
					}
					else
					{
						base.WriteVerbose(Strings.NoUpdatesDirectorySpecified);
					}
					if (this.newProduct)
					{
						base.WriteVerbose(Strings.NewProductInstallation(packagePath, base.PropertyValues));
						num = MsiNativeMethods.InstallProduct(packagePath, base.PropertyValues);
					}
					else
					{
						string productCodeString = productCode.ToString("B").ToUpper(CultureInfo.InvariantCulture);
						base.WriteVerbose(Strings.ExistingProductConfiguration(productCodeString, base.PropertyValues));
						num = MsiNativeMethods.ConfigureProduct(productCodeString, InstallLevel.Default, InstallState.Default, base.PropertyValues);
					}
					if (num != 0U)
					{
						Win32Exception ex = new Win32Exception((int)num);
						if (num == 3010U)
						{
							throw new TaskException(Strings.MsiRebootRequiredToContinue(packagePath), ex);
						}
						if (string.IsNullOrEmpty(base.LastMsiError))
						{
							base.WriteError(new TaskException(Strings.MsiInstallFailed(packagePath, ex.Message, (int)num), ex), ErrorCategory.InvalidOperation, null);
						}
						else
						{
							base.WriteError(new TaskException(Strings.MsiInstallFailedDetailed(packagePath, ex.Message, (int)num, base.LastMsiError), ex), ErrorCategory.InvalidOperation, null);
						}
					}
				}
				finally
				{
					MsiUtility.PopExternalUI();
				}
			}
			catch (ArgumentOutOfRangeException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidArgument, null);
			}
			catch (TaskException exception3)
			{
				base.WriteError(exception3, ErrorCategory.InvalidOperation, null);
			}
			TaskLogger.LogExit();
		}

		private bool newProduct = true;
	}
}
