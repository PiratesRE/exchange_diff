using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Data.Storage.UnifiedPolicy;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	internal static class AuditConfigUtility
	{
		internal static AuditConfigurationRule GetAuditConfigurationRule(Workload workload, OrganizationIdParameter organizationId, out IEnumerable<ErrorRecord> pipelineErrors)
		{
			Guid guid;
			if (!AuditPolicyUtility.GetRuleGuidFromWorkload(workload, out guid))
			{
				pipelineErrors = new List<ErrorRecord>();
				return null;
			}
			Command command = new Command("Get-AuditConfigurationRule");
			if (organizationId != null)
			{
				command.Parameters.Add("Organization", organizationId);
			}
			command.Parameters.Add("Identity", guid.ToString());
			return AuditConfigUtility.InvokeCommand(command, out pipelineErrors) as AuditConfigurationRule;
		}

		internal static AuditConfigurationPolicy GetAuditConfigurationPolicy(Workload workload, OrganizationIdParameter organizationId, out IEnumerable<ErrorRecord> pipelineErrors)
		{
			Guid guid;
			if (!AuditPolicyUtility.GetPolicyGuidFromWorkload(workload, out guid))
			{
				pipelineErrors = new List<ErrorRecord>();
				return null;
			}
			Command command = new Command("Get-AuditConfigurationPolicy");
			if (organizationId != null)
			{
				command.Parameters.Add("Organization", organizationId);
			}
			command.Parameters.Add("Identity", guid.ToString());
			return AuditConfigUtility.InvokeCommand(command, out pipelineErrors) as AuditConfigurationPolicy;
		}

		internal static AuditConfigurationPolicy NewAuditConfigurationPolicy(Workload workload, OrganizationIdParameter organizationId, out IEnumerable<ErrorRecord> pipelineErrors)
		{
			Command command = new Command("New-AuditConfigurationPolicy");
			if (organizationId != null)
			{
				command.Parameters.Add("Organization", organizationId);
			}
			command.Parameters.Add("Workload", workload);
			return AuditConfigUtility.InvokeCommand(command, out pipelineErrors) as AuditConfigurationPolicy;
		}

		internal static AuditConfigurationRule NewAuditConfigurationRule(Workload workload, OrganizationIdParameter organizationId, MultiValuedProperty<AuditableOperations> auditOpsToSet, out IEnumerable<ErrorRecord> pipelineErrors)
		{
			Command command = new Command("New-AuditConfigurationRule");
			if (organizationId != null)
			{
				command.Parameters.Add("Organization", organizationId);
			}
			command.Parameters.Add("Workload", workload);
			command.Parameters.Add("AuditOperation", auditOpsToSet);
			return AuditConfigUtility.InvokeCommand(command, out pipelineErrors) as AuditConfigurationRule;
		}

		internal static void SetAuditConfigurationRule(Workload workload, OrganizationIdParameter organizationId, MultiValuedProperty<AuditableOperations> auditOpsToSet, out IEnumerable<ErrorRecord> pipelineErrors)
		{
			Guid guid;
			if (!AuditPolicyUtility.GetRuleGuidFromWorkload(workload, out guid))
			{
				pipelineErrors = new List<ErrorRecord>();
				return;
			}
			Command command = new Command("Get-AuditConfigurationRule");
			if (organizationId != null)
			{
				command.Parameters.Add("Organization", organizationId);
			}
			command.Parameters.Add("Identity", guid.ToString());
			Command command2 = new Command("Set-AuditConfigurationRule");
			command2.Parameters.Add("AuditOperation", auditOpsToSet);
			AuditConfigUtility.InvokeCommands(new List<Command>
			{
				command,
				command2
			}, out pipelineErrors);
		}

		internal static Workload GetEffectiveWorkload(Workload auditWorkload)
		{
			if (auditWorkload != Workload.OneDriveForBusiness)
			{
				return auditWorkload;
			}
			return Workload.SharePoint;
		}

		internal static List<Workload> AuditableWorkloads
		{
			get
			{
				return AuditConfigUtility.auditableWorkloads;
			}
		}

		private static object InvokeCommand(Command cmd, out IEnumerable<ErrorRecord> pipelineErrors)
		{
			return AuditConfigUtility.InvokeCommands(new List<Command>
			{
				cmd
			}, out pipelineErrors);
		}

		private static object InvokeCommands(List<Command> cmds, out IEnumerable<ErrorRecord> pipelineErrors)
		{
			List<ErrorRecord> list = new List<ErrorRecord>();
			pipelineErrors = list;
			object result = null;
			using (Pipeline pipeline = Runspace.DefaultRunspace.CreateNestedPipeline())
			{
				foreach (Command item in cmds)
				{
					pipeline.Commands.Add(item);
				}
				Collection<PSObject> collection = pipeline.Invoke();
				if (collection != null && collection.Count == 1)
				{
					result = collection[0].BaseObject;
				}
				if (pipeline.Error.Count > 0)
				{
					while (!pipeline.Error.EndOfPipeline)
					{
						PSObject psobject = pipeline.Error.Read() as PSObject;
						if (psobject != null)
						{
							ErrorRecord errorRecord = psobject.BaseObject as ErrorRecord;
							if (errorRecord != null)
							{
								list.Add(errorRecord);
							}
						}
					}
				}
			}
			return result;
		}

		internal static bool ValidateErrorRecords(Cmdlet cmdLet, IEnumerable<ErrorRecord> errRecords)
		{
			return AuditConfigUtility.ValidateErrorRecords(cmdLet, errRecords, (ErrorRecord err) => true);
		}

		internal static bool ValidateErrorRecords(Cmdlet cmdLet, IEnumerable<ErrorRecord> errRecords, Func<ErrorRecord, bool> predicate)
		{
			bool result = true;
			foreach (ErrorRecord errorRecord in errRecords.Where(predicate))
			{
				cmdLet.WriteError(errorRecord);
				result = false;
			}
			return result;
		}

		internal static AuditSwitchStatus ValidateAuditConfigurationRule(Workload workload, AuditConfigurationRule auditRule)
		{
			MultiValuedProperty<AuditableOperations> auditOperation = auditRule.AuditOperation;
			Tuple<MultiValuedProperty<AuditableOperations>, MultiValuedProperty<AuditableOperations>> tuple = AuditConfigUtility.auditMap[workload];
			if (tuple != null)
			{
				if (auditOperation.Equals(tuple.Item1))
				{
					return AuditSwitchStatus.Off;
				}
				if (auditOperation.Equals(tuple.Item2))
				{
					return AuditSwitchStatus.On;
				}
			}
			return AuditSwitchStatus.None;
		}

		internal static MultiValuedProperty<AuditableOperations> GetAuditOperations(Workload workload, AuditSwitchStatus auditSwitch)
		{
			if (auditSwitch == AuditSwitchStatus.On)
			{
				return AuditConfigUtility.auditMap[workload].Item2;
			}
			return AuditConfigUtility.auditMap[workload].Item1;
		}

		private static readonly List<Workload> auditableWorkloads = new List<Workload>
		{
			Workload.Exchange,
			Workload.SharePoint,
			Workload.OneDriveForBusiness
		};

		private static readonly MultiValuedProperty<AuditableOperations> exchangeAuditOff = new MultiValuedProperty<AuditableOperations>
		{
			AuditableOperations.None
		};

		private static readonly MultiValuedProperty<AuditableOperations> exchangeAuditOn = new MultiValuedProperty<AuditableOperations>
		{
			AuditableOperations.Administrate,
			AuditableOperations.CreateUpdate,
			AuditableOperations.Forward,
			AuditableOperations.MoveCopy,
			AuditableOperations.Search,
			AuditableOperations.SendAsOthers
		};

		private static readonly MultiValuedProperty<AuditableOperations> sharePointAuditOff = new MultiValuedProperty<AuditableOperations>
		{
			AuditableOperations.None
		};

		private static readonly MultiValuedProperty<AuditableOperations> sharePointAuditOn = new MultiValuedProperty<AuditableOperations>
		{
			AuditableOperations.CheckIn,
			AuditableOperations.CheckOut,
			AuditableOperations.CreateUpdate,
			AuditableOperations.Delete,
			AuditableOperations.MoveCopy,
			AuditableOperations.PermissionChange,
			AuditableOperations.ProfileChange,
			AuditableOperations.SchemaChange,
			AuditableOperations.Search,
			AuditableOperations.Workflow
		};

		private static readonly MultiValuedProperty<AuditableOperations> oneDriveAuditOff = new MultiValuedProperty<AuditableOperations>
		{
			AuditableOperations.None
		};

		private static readonly MultiValuedProperty<AuditableOperations> oneDriveAuditOn = new MultiValuedProperty<AuditableOperations>
		{
			AuditableOperations.CreateUpdate,
			AuditableOperations.Delete,
			AuditableOperations.MoveCopy
		};

		private static readonly Dictionary<Workload, Tuple<MultiValuedProperty<AuditableOperations>, MultiValuedProperty<AuditableOperations>>> auditMap = new Dictionary<Workload, Tuple<MultiValuedProperty<AuditableOperations>, MultiValuedProperty<AuditableOperations>>>
		{
			{
				Workload.SharePoint,
				Tuple.Create<MultiValuedProperty<AuditableOperations>, MultiValuedProperty<AuditableOperations>>(AuditConfigUtility.sharePointAuditOff, AuditConfigUtility.sharePointAuditOn)
			},
			{
				Workload.Exchange,
				Tuple.Create<MultiValuedProperty<AuditableOperations>, MultiValuedProperty<AuditableOperations>>(AuditConfigUtility.exchangeAuditOff, AuditConfigUtility.exchangeAuditOn)
			},
			{
				Workload.OneDriveForBusiness,
				Tuple.Create<MultiValuedProperty<AuditableOperations>, MultiValuedProperty<AuditableOperations>>(AuditConfigUtility.oneDriveAuditOff, AuditConfigUtility.oneDriveAuditOn)
			}
		};
	}
}
