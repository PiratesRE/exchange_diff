using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class AsrContactsSearchContext : AsrSearchContext
	{
		internal AsrContactsSearchContext(AsrContactsManager manager)
		{
			this.manager = manager;
		}

		internal override bool CanShowExactMatches()
		{
			return this.manager.MatchedNameSelectionMethod != DisambiguationFieldEnum.PromptForAlias || this.manager.CurrentSearchTarget != SearchTarget.GlobalAddressList || this.manager.AuthenticatedCaller;
		}

		internal override void PrepareForNBestPhase2()
		{
			this.manager.PrepareForNBestPhase2();
		}

		internal override void PrepareForPromptForAliasQA(List<IUMRecognitionPhrase> alternates)
		{
			base.ResultsToPlay = new List<List<IUMRecognitionPhrase>>();
			base.ResultsToPlay.Add(alternates);
			this.manager.PrepareForPromptForAliasQA(alternates);
		}

		internal override void PrepareForCollisionQA(List<List<IUMRecognitionPhrase>> alternates)
		{
			base.ResultsToPlay = alternates;
			this.manager.PrepareForCollisionQA(alternates);
		}

		internal override void PrepareForConfirmViaListQA(List<List<IUMRecognitionPhrase>> alternates)
		{
			base.ResultsToPlay = alternates;
			this.manager.PrepareForConfirmViaListQA(alternates);
		}

		internal override bool PrepareForConfirmQA(List<List<IUMRecognitionPhrase>> alternates)
		{
			return this.manager.PrepareForConfirmQA(alternates);
		}

		internal override bool PrepareForConfirmAgainQA(List<List<IUMRecognitionPhrase>> alternates)
		{
			base.ResultsToPlay = alternates;
			return this.manager.PrepareForConfirmAgainQA(alternates);
		}

		internal override List<List<IUMRecognitionPhrase>> ProcessMultipleResults(List<List<IUMRecognitionPhrase>> results)
		{
			return this.manager.ProcessMultipleResults(results);
		}

		internal override void OnNameSpoken()
		{
			this.manager.OnNameSpoken();
		}

		private AsrContactsManager manager;
	}
}
