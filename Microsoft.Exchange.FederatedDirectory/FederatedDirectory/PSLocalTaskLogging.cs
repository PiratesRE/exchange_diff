using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.RecipientTasks;

namespace Microsoft.Exchange.FederatedDirectory
{
	internal static class PSLocalTaskLogging
	{
		internal struct CmdletStringBuilder
		{
			public void Append(string value)
			{
				this.InitializeIfNeeded();
				this.stringBuilder.Append(value);
			}

			public void Append(string parameterName, string parameterValue)
			{
				this.InitializeIfNeeded();
				if (!string.IsNullOrEmpty(parameterValue))
				{
					this.stringBuilder.Append(string.Concat(new string[]
					{
						" -",
						parameterName,
						":'",
						parameterValue,
						"'"
					}));
				}
			}

			public void Append(string parameterName, ADIdParameter parameterValue)
			{
				this.InitializeIfNeeded();
				if (parameterValue != null && !string.IsNullOrEmpty(parameterValue.RawIdentity))
				{
					this.stringBuilder.Append(string.Concat(new string[]
					{
						" -",
						parameterName,
						":'",
						parameterValue.RawIdentity,
						"'"
					}));
				}
			}

			public void Append(string parameterName, Uri parameterValue)
			{
				this.InitializeIfNeeded();
				if (parameterValue != null)
				{
					this.stringBuilder.Append(string.Concat(new string[]
					{
						" -",
						parameterName,
						":'",
						parameterValue.ToString(),
						"'"
					}));
				}
			}

			public void Append(string parameterName, RecipientIdParameter[] ids)
			{
				this.InitializeIfNeeded();
				if (ids != null && ids.Length > 0)
				{
					this.stringBuilder.Append(" -" + parameterName + ":");
					bool flag = true;
					foreach (RecipientIdParameter recipientIdParameter in ids)
					{
						if (flag)
						{
							flag = false;
						}
						else
						{
							this.stringBuilder.Append(",");
						}
						this.stringBuilder.Append("'" + recipientIdParameter.RawIdentity + "'");
					}
				}
			}

			public override string ToString()
			{
				this.InitializeIfNeeded();
				return this.stringBuilder.ToString();
			}

			private void InitializeIfNeeded()
			{
				if (this.stringBuilder == null)
				{
					this.stringBuilder = new StringBuilder(256);
				}
			}

			private StringBuilder stringBuilder;
		}

		internal sealed class NewGroupMailboxToString
		{
			public NewGroupMailboxToString(NewGroupMailbox cmdlet)
			{
				this.cmdlet = cmdlet;
			}

			public override string ToString()
			{
				PSLocalTaskLogging.CmdletStringBuilder cmdletStringBuilder = default(PSLocalTaskLogging.CmdletStringBuilder);
				cmdletStringBuilder.Append("New-GroupMailbox");
				cmdletStringBuilder.Append("ExecutingUser", this.cmdlet.ExecutingUser);
				cmdletStringBuilder.Append("Organization", this.cmdlet.Organization);
				cmdletStringBuilder.Append("ExternalDirectoryObjectId", this.cmdlet.ExternalDirectoryObjectId);
				cmdletStringBuilder.Append("Name", this.cmdlet.Name);
				cmdletStringBuilder.Append("ModernGroupType", this.cmdlet.ModernGroupType.ToString());
				cmdletStringBuilder.Append("Alias", this.cmdlet.Alias);
				cmdletStringBuilder.Append("Description", this.cmdlet.Description);
				cmdletStringBuilder.Append("Owners", this.cmdlet.Owners);
				cmdletStringBuilder.Append("Members", this.cmdlet.Members);
				return cmdletStringBuilder.ToString();
			}

			private readonly NewGroupMailbox cmdlet;
		}

		internal sealed class SetGroupMailboxToString
		{
			public SetGroupMailboxToString(SetGroupMailbox cmdlet)
			{
				this.cmdlet = cmdlet;
			}

			public override string ToString()
			{
				PSLocalTaskLogging.CmdletStringBuilder cmdletStringBuilder = default(PSLocalTaskLogging.CmdletStringBuilder);
				cmdletStringBuilder.Append("Set-GroupMailbox");
				cmdletStringBuilder.Append("ExecutingUser", this.cmdlet.ExecutingUser);
				cmdletStringBuilder.Append("Identity", this.cmdlet.Identity);
				cmdletStringBuilder.Append("Name", this.cmdlet.Name);
				cmdletStringBuilder.Append("DisplayName", this.cmdlet.DisplayName);
				cmdletStringBuilder.Append("Description", this.cmdlet.Description);
				cmdletStringBuilder.Append("SharePointUrl", this.cmdlet.SharePointUrl);
				cmdletStringBuilder.Append("Owners", this.cmdlet.Owners);
				cmdletStringBuilder.Append("AddOwners", this.cmdlet.AddOwners);
				cmdletStringBuilder.Append("RemoveOwners", this.cmdlet.RemoveOwners);
				cmdletStringBuilder.Append("AddedMembers", this.cmdlet.AddedMembers);
				cmdletStringBuilder.Append("RemovedMembers", this.cmdlet.RemovedMembers);
				if (this.cmdlet.RequireSenderAuthenticationEnabledChanged)
				{
					cmdletStringBuilder.Append("RequireSenderAuthenticationEnabled", this.cmdlet.RequireSenderAuthenticationEnabled.ToString());
				}
				return cmdletStringBuilder.ToString();
			}

			private readonly SetGroupMailbox cmdlet;
		}

		internal sealed class RemoveGroupMailboxToString
		{
			public RemoveGroupMailboxToString(RemoveGroupMailbox cmdlet)
			{
				this.cmdlet = cmdlet;
			}

			public override string ToString()
			{
				PSLocalTaskLogging.CmdletStringBuilder cmdletStringBuilder = default(PSLocalTaskLogging.CmdletStringBuilder);
				cmdletStringBuilder.Append("Remove-GroupMailbox");
				cmdletStringBuilder.Append("ExecutingUser", this.cmdlet.ExecutingUser);
				cmdletStringBuilder.Append("Identity", this.cmdlet.Identity);
				return cmdletStringBuilder.ToString();
			}

			private readonly RemoveGroupMailbox cmdlet;
		}

		internal sealed class TaskOutputToString
		{
			public TaskOutputToString(IList<PSLocalTaskIOData> container)
			{
				this.container = container;
			}

			public override string ToString()
			{
				if (this.container != null)
				{
					StringBuilder stringBuilder = new StringBuilder(1000);
					stringBuilder.AppendLine("Output:");
					foreach (PSLocalTaskIOData pslocalTaskIOData in this.container)
					{
						stringBuilder.AppendLine(pslocalTaskIOData.ToString());
					}
					return stringBuilder.ToString();
				}
				return "No output";
			}

			private readonly IList<PSLocalTaskIOData> container;
		}
	}
}
