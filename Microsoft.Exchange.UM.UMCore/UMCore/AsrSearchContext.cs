using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class AsrSearchContext
	{
		internal List<List<IUMRecognitionPhrase>> RejectedResults
		{
			get
			{
				return this.rejectedResults;
			}
			set
			{
				this.rejectedResults = value;
			}
		}

		internal List<List<IUMRecognitionPhrase>> ResultsToPlay
		{
			get
			{
				return this.resultsToPlay;
			}
			set
			{
				this.resultsToPlay = value;
			}
		}

		internal abstract bool CanShowExactMatches();

		internal abstract void PrepareForNBestPhase2();

		internal abstract void PrepareForPromptForAliasQA(List<IUMRecognitionPhrase> alternates);

		internal abstract void PrepareForCollisionQA(List<List<IUMRecognitionPhrase>> alternates);

		internal abstract void PrepareForConfirmViaListQA(List<List<IUMRecognitionPhrase>> results);

		internal abstract bool PrepareForConfirmQA(List<List<IUMRecognitionPhrase>> results);

		internal abstract bool PrepareForConfirmAgainQA(List<List<IUMRecognitionPhrase>> results);

		internal abstract List<List<IUMRecognitionPhrase>> ProcessMultipleResults(List<List<IUMRecognitionPhrase>> results);

		internal abstract void OnNameSpoken();

		private List<List<IUMRecognitionPhrase>> resultsToPlay;

		private List<List<IUMRecognitionPhrase>> rejectedResults;
	}
}
