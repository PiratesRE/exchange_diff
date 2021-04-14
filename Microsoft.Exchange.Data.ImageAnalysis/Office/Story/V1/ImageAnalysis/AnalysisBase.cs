using System;
using System.Runtime.Serialization;

namespace Microsoft.Office.Story.V1.ImageAnalysis
{
	[DataContract]
	[Serializable]
	internal abstract class AnalysisBase
	{
		[DataMember]
		public ImageAnalysisState State { get; protected set; }

		[DataMember]
		public Exception Error { get; protected set; }

		protected void PerformAnalysis()
		{
			this.State = ImageAnalysisState.NotProcessed;
			if (!this.CanAnalyze())
			{
				this.State = ImageAnalysisState.UnsatisfactorySubject;
				this.CreateDefaultResults();
				return;
			}
			try
			{
				this.AnalysisImplementation();
				this.State = ImageAnalysisState.SuccessfullyProcessed;
			}
			catch (Exception error)
			{
				this.Error = error;
				this.State = ImageAnalysisState.ErrorProcessing;
			}
			finally
			{
				this.Lock();
			}
		}

		protected virtual void Lock()
		{
		}

		protected virtual void CreateDefaultResults()
		{
		}

		protected abstract bool CanAnalyze();

		protected abstract void AnalysisImplementation();
	}
}
