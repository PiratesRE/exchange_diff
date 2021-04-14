using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class RuleTableRestriction : IModifyTableRestriction
	{
		internal RuleTableRestriction(CoreFolder coreFolder)
		{
			Util.ThrowOnNullArgument(coreFolder, "coreFolder");
			this.coreFolder = coreFolder;
			this.session = coreFolder.Session;
		}

		void IModifyTableRestriction.Enforce(IModifyTable modifyTable, IEnumerable<ModifyTableOperation> modifyTableOperations)
		{
			if (modifyTableOperations == null)
			{
				return;
			}
			if (this.session != null && this.session.IsPublicFolderSession && !this.session.IsMoveUser && this.HasCreateOrModifyReplyRule(modifyTableOperations) && !this.coreFolder.IsMailEnabled())
			{
				throw new RuleNotSupportedException(ServerStrings.ReplyRuleNotSupportedOnNonMailPublicFolder);
			}
		}

		private bool HasCreateOrModifyReplyRule(IEnumerable<ModifyTableOperation> modifyTableOperations)
		{
			foreach (ModifyTableOperation modifyTableOperation in modifyTableOperations)
			{
				if (modifyTableOperation.Operation == ModifyTableOperationType.Add || modifyTableOperation.Operation == ModifyTableOperationType.Modify)
				{
					foreach (PropValue propValue in modifyTableOperation.Properties)
					{
						if (propValue.Property == InternalSchema.RuleActions)
						{
							RuleAction[] array = propValue.Value as RuleAction[];
							foreach (RuleAction ruleAction in array)
							{
								if (ruleAction.ActionType == RuleActionType.Reply)
								{
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}

		private readonly CoreFolder coreFolder;

		private readonly StoreSession session;
	}
}
