using System;
using System.Globalization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class StaticDirectoryGrammarHandler : DirectoryGrammarHandler
	{
		public StaticDirectoryGrammarHandler(OrganizationId orgId) : base(orgId)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.UMGrammarGeneratorTracer, this, "Creating static directory grammar handler for org '{0}'", new object[]
			{
				base.OrgId
			});
		}

		public override bool DeleteFileAfterUse
		{
			get
			{
				return false;
			}
		}

		public override string ToString()
		{
			if (this.grammarIdentifier != null)
			{
				return this.grammarIdentifier.ToString();
			}
			return base.OrgId.ToString();
		}

		public override void PrepareGrammarAsync(CallContext callContext, DirectoryGrammarHandler.GrammarType grammarType)
		{
			ValidateArgument.NotNull(callContext, "callContext");
			this.grammarType = grammarType;
			switch (grammarType)
			{
			case DirectoryGrammarHandler.GrammarType.User:
				this.grammarIdentifier = GalGrammarFile.GetGrammarIdentifier(callContext);
				break;
			case DirectoryGrammarHandler.GrammarType.DL:
				this.grammarIdentifier = DistributionListGrammarFile.GetGrammarIdentifier(callContext);
				break;
			default:
				ExAssert.RetailAssert(false, "Unknown grammar type {0}", new object[]
				{
					grammarType
				});
				break;
			}
			this.grammarFetcher = new LargeGrammarFetcher(this.grammarIdentifier);
		}

		public override void PrepareGrammarAsync(ADRecipient recipient, CultureInfo culture)
		{
			ValidateArgument.NotNull(recipient, "recipient");
			ValidateArgument.NotNull(culture, "culture");
			this.grammarType = DirectoryGrammarHandler.GrammarType.User;
			this.grammarIdentifier = new GrammarIdentifier(base.OrgId, culture, GrammarFileNames.GetFileNameForGALUser());
			this.grammarFetcher = new LargeGrammarFetcher(this.grammarIdentifier);
		}

		public override SearchGrammarFile WaitForPrepareGrammarCompletion()
		{
			SearchGrammarFile result = null;
			switch (this.grammarType)
			{
			case DirectoryGrammarHandler.GrammarType.User:
			{
				LargeGrammarFetcher.FetchResult fetchResult = this.grammarFetcher.Wait(Constants.GalGrammarFetchTimeout);
				if (fetchResult.Status == LargeGrammarFetcher.FetchStatus.Success)
				{
					result = new GalGrammarFile(this.grammarIdentifier.Culture, fetchResult.FilePath);
				}
				break;
			}
			case DirectoryGrammarHandler.GrammarType.DL:
			{
				LargeGrammarFetcher.FetchResult fetchResult = this.grammarFetcher.Wait(Constants.DLGramamrFetchTimeout);
				if (fetchResult.Status == LargeGrammarFetcher.FetchStatus.Success)
				{
					result = new DistributionListGrammarFile(this.grammarIdentifier.Culture, fetchResult.FilePath);
				}
				break;
			}
			default:
				ExAssert.RetailAssert(false, "Unknown grammar type {0}", new object[]
				{
					this.grammarType
				});
				break;
			}
			return result;
		}

		private DirectoryGrammarHandler.GrammarType grammarType;

		private GrammarIdentifier grammarIdentifier;

		private LargeGrammarFetcher grammarFetcher;
	}
}
