using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class BaseUMOfflineTranscriber : DisposableBase
	{
		internal abstract event EventHandler<BaseUMOfflineTranscriber.TranscribeCompletedEventArgs> TranscribeCompleted;

		internal TopNData TopN { get; set; }

		protected internal ContactInfo CallerInfo
		{
			protected get
			{
				return this.callerInfo;
			}
			set
			{
				this.callerInfo = value;
			}
		}

		protected internal UMSubscriber TranscriptionUser
		{
			protected get
			{
				return this.transcriptionUser;
			}
			set
			{
				this.transcriptionUser = value;
				this.calleeInfo = new ADContactInfo((IADOrgPerson)value.ADRecipient);
			}
		}

		protected internal string CallingLineId
		{
			protected get
			{
				return this.callingLineId;
			}
			set
			{
				this.callingLineId = value;
			}
		}

		protected ContactInfo CalleeInfo
		{
			get
			{
				return this.calleeInfo;
			}
		}

		internal abstract void TranscribeFile(string audioFilePath);

		internal abstract void CancelTranscription();

		internal abstract List<KeyValuePair<string, int>> FilterWordsInLexion(List<KeyValuePair<string, int>> rawList, int maxNumberToKeep);

		internal abstract string TestHook_GenerateCustomGrammars();

		private ContactInfo callerInfo;

		private ContactInfo calleeInfo;

		private UMSubscriber transcriptionUser;

		private string callingLineId;

		internal class TranscribeCompletedEventArgs : AsyncCompletedEventArgs
		{
			internal TranscribeCompletedEventArgs(List<IUMTranscriptionResult> transcriptionResults, Exception error, bool cancelled, object userState) : base(error, cancelled, userState)
			{
				this.transcriptionResults = transcriptionResults;
			}

			internal List<IUMTranscriptionResult> TranscriptionResults
			{
				get
				{
					return this.transcriptionResults;
				}
			}

			private List<IUMTranscriptionResult> transcriptionResults;
		}
	}
}
