using System;
using System.Text;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class FolderSaveResult
	{
		public FolderSaveResult(OperationResult operationResult, LocalizedException exception, PropertyError[] propertyErrors)
		{
			EnumValidator.ThrowIfInvalid<OperationResult>(operationResult, "operationResult");
			this.OperationResult = operationResult;
			this.PropertyErrors = propertyErrors;
			this.Exception = exception;
		}

		public LocalizedException ToException()
		{
			int errorCount = 0;
			StringBuilder stringBuilder = new StringBuilder();
			if (this.PropertyErrors != null)
			{
				errorCount = this.PropertyErrors.Length;
				foreach (PropertyError propertyError in this.PropertyErrors)
				{
					stringBuilder.Append(Environment.NewLine);
					stringBuilder.Append(propertyError.ToLocalizedString());
					if (stringBuilder.Length >= 20000)
					{
						stringBuilder.Length = 20000 - "...".Length;
						stringBuilder.Append("...");
						break;
					}
				}
			}
			return this.ToException(ServerStrings.FolderSaveOperationResult(this.GetLocalizedResult(), errorCount, stringBuilder.ToString()));
		}

		public LocalizedException ToException(LocalizedString exceptionMessage)
		{
			if (this.IsErrorTransient)
			{
				return new FolderSaveTransientException(exceptionMessage, this);
			}
			return new FolderSaveException(exceptionMessage, this);
		}

		public override string ToString()
		{
			if (this.OperationResult == OperationResult.Succeeded)
			{
				return this.OperationResult.ToString();
			}
			StringBuilder stringBuilder = new StringBuilder(this.OperationResult.ToString());
			if (this.PropertyErrors != null)
			{
				foreach (PropertyError propertyError in this.PropertyErrors)
				{
					stringBuilder.Append(string.Format(", {0}", propertyError.ToString()));
				}
			}
			return stringBuilder.ToString();
		}

		private LocalizedString GetLocalizedResult()
		{
			switch (this.OperationResult)
			{
			case OperationResult.Succeeded:
				return ServerStrings.OperationResultSucceeded;
			case OperationResult.Failed:
				return ServerStrings.OperationResultFailed;
			case OperationResult.PartiallySucceeded:
				return ServerStrings.OperationResultPartiallySucceeded;
			default:
				throw new NotImplementedException(this.OperationResult.ToString());
			}
		}

		private bool IsErrorTransient
		{
			get
			{
				if (this.OperationResult != OperationResult.Succeeded)
				{
					foreach (PropertyError propertyError in this.PropertyErrors)
					{
						if (propertyError.PropertyErrorCode != PropertyErrorCode.TransientMapiCallFailed)
						{
							return false;
						}
					}
					return true;
				}
				return false;
			}
		}

		private const int MaxErrorMessageLength = 20000;

		public readonly OperationResult OperationResult;

		public readonly PropertyError[] PropertyErrors;

		public readonly LocalizedException Exception;
	}
}
