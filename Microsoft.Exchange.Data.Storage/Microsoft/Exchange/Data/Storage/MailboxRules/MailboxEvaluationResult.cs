using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MailboxEvaluationResult : IDisposable
	{
		public MailboxEvaluationResult(IRuleEvaluationContext context)
		{
			this.context = context;
			this.folderResults = new List<FolderEvaluationResult>();
			this.targetFolder = this.context.CurrentFolder;
		}

		public RuleAction.Bounce.BounceCode? BounceCode
		{
			get
			{
				if (this.folderResults.Count > 0)
				{
					return this.folderResults[this.folderResults.Count - 1].BounceCode;
				}
				return null;
			}
		}

		public IRuleEvaluationContext Context
		{
			get
			{
				return this.context;
			}
		}

		public Folder TargetFolder
		{
			get
			{
				return this.targetFolder;
			}
		}

		internal IList<FolderEvaluationResult> FolderResults
		{
			get
			{
				return new ReadOnlyCollection<FolderEvaluationResult>(this.folderResults);
			}
		}

		public void AddFolderResult(FolderEvaluationResult folderResult)
		{
			this.folderResults.Add(folderResult);
			this.targetFolder = folderResult.TargetFolder;
		}

		public void Dispose()
		{
			if (this.context != null)
			{
				this.context.Dispose();
			}
		}

		public void Execute(ExecutionStage stage)
		{
			this.context.DeliveryFolder = this.TargetFolder;
			this.context.ExecutionStage = stage;
			foreach (FolderEvaluationResult folderEvaluationResult in this.folderResults)
			{
				folderEvaluationResult.Execute(stage);
			}
		}

		private IRuleEvaluationContext context;

		private List<FolderEvaluationResult> folderResults;

		private Folder targetFolder;
	}
}
