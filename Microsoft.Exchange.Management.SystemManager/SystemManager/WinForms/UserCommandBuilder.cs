using System;
using System.Collections.Generic;
using System.Data;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.MonadDataProvider;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	internal class UserCommandBuilder : ProcedureBuilder
	{
		internal override MonadCommand InnerBuildProcedureCore(string commandText, DataRow row)
		{
			MonadCommand monadCommand = new LoggableMonadCommand(commandText);
			UserRange userRange = UserRange.AllUsersWithoutMailAttributes;
			string name = null;
			string displayName = null;
			string text = null;
			PSCredential pscredential = null;
			if (!DBNull.Value.Equals(row["UserRange"]))
			{
				userRange = (UserRange)row["UserRange"];
			}
			if (!DBNull.Value.Equals(row["NameForFilter"]))
			{
				name = (string)row["NameForFilter"];
			}
			if (!DBNull.Value.Equals(row["DisplayNameForFilter"]))
			{
				displayName = (string)row["DisplayNameForFilter"];
			}
			if (!DBNull.Value.Equals(row["DomainController"]))
			{
				text = (string)row["DomainController"];
			}
			if (!DBNull.Value.Equals(row["Credential"]))
			{
				pscredential = (PSCredential)row["Credential"];
			}
			string value = UserCommandBuilder.ConstructFilterParameterValue(name, displayName);
			if (!string.IsNullOrEmpty(value))
			{
				monadCommand.Parameters.AddWithValue("Filter", value);
			}
			monadCommand.Parameters.AddWithValue("RecipientTypeDetails", UserCommandBuilder.ConstructRecipientTypeDetailsArray(userRange));
			monadCommand.Parameters.AddWithValue("ResultSize", "Unlimited");
			if (text != null)
			{
				monadCommand.Parameters.AddWithValue("DomainController", text);
			}
			if (pscredential != null)
			{
				monadCommand.Parameters.AddWithValue("Credential", pscredential);
			}
			return monadCommand;
		}

		private static string ConstructFilterParameterValue(string name, string displayName)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string format = "SamAccountName -eq '{0}' -or DisplayName -eq '{0}'";
			if (!string.IsNullOrEmpty(name))
			{
				stringBuilder.AppendFormat(format, name.ToQuotationEscapedString());
			}
			if (!string.IsNullOrEmpty(displayName) && !displayName.Equals(name, StringComparison.OrdinalIgnoreCase))
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(" -or ");
				}
				stringBuilder.AppendFormat(format, displayName.ToQuotationEscapedString());
			}
			return stringBuilder.ToString();
		}

		private static RecipientTypeDetails[] ConstructRecipientTypeDetailsArray(UserRange userRange)
		{
			List<RecipientTypeDetails> list = new List<RecipientTypeDetails>();
			for (int i = 0; i < UserCommandBuilder.userRanges.Length; i++)
			{
				if ((short)(userRange & UserCommandBuilder.userRanges[i]) != 0)
				{
					list.AddRange(UserCommandBuilder.userRangeRecipientTypeDetails[i]);
				}
			}
			return list.ToArray();
		}

		private static readonly UserRange[] userRanges = new UserRange[]
		{
			UserRange.AccountEnabledUsers,
			UserRange.AccountDisabledUsers,
			UserRange.MailEnabledUsers,
			UserRange.MailboxUsers
		};

		private static readonly RecipientTypeDetails[][] userRangeRecipientTypeDetails = new RecipientTypeDetails[][]
		{
			new RecipientTypeDetails[]
			{
				RecipientTypeDetails.User
			},
			new RecipientTypeDetails[]
			{
				RecipientTypeDetails.DisabledUser
			},
			new RecipientTypeDetails[]
			{
				RecipientTypeDetails.MailUser,
				(RecipientTypeDetails)((ulong)int.MinValue),
				RecipientTypeDetails.RemoteRoomMailbox,
				RecipientTypeDetails.RemoteEquipmentMailbox,
				RecipientTypeDetails.RemoteSharedMailbox,
				RecipientTypeDetails.RemoteTeamMailbox
			},
			new RecipientTypeDetails[]
			{
				RecipientTypeDetails.UserMailbox,
				RecipientTypeDetails.LinkedMailbox,
				RecipientTypeDetails.SharedMailbox,
				RecipientTypeDetails.TeamMailbox,
				RecipientTypeDetails.LegacyMailbox,
				RecipientTypeDetails.RoomMailbox,
				RecipientTypeDetails.EquipmentMailbox
			}
		};
	}
}
