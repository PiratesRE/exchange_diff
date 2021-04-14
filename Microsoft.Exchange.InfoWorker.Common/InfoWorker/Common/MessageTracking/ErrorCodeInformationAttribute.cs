using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class ErrorCodeInformationAttribute : Attribute
	{
		public bool IsTransientError { get; set; }

		public bool ShowToIWUser { get; set; }

		public bool ShowDetailToIW { get; set; }

		public Strings.IDs Message
		{
			get
			{
				return this.message;
			}
			set
			{
				this.message = value;
				if (value <= (Strings.IDs)2837247303U)
				{
					if (value <= Strings.IDs.TrackingErrorLogSearchServiceDown)
					{
						if (value == Strings.IDs.TrackingErrorInvalidADData)
						{
							goto IL_BC;
						}
						if (value != Strings.IDs.TrackingErrorLogSearchServiceDown)
						{
							goto IL_EC;
						}
						this.ErrorFormatter = delegate(TrackingError error, bool isMultiMessageSearch)
						{
							if (string.IsNullOrEmpty(error.Target))
							{
								return this.GetGenericErrorMessageIfMissingArguments(isMultiMessageSearch);
							}
							return string.Format(Strings.GetLocalizedString(this.message), error.Target);
						};
						return;
					}
					else if (value != Strings.IDs.TrackingErrorQueueViewerRpc && value != (Strings.IDs)2319913820U && value != (Strings.IDs)2837247303U)
					{
						goto IL_EC;
					}
				}
				else if (value <= (Strings.IDs)3376565836U)
				{
					if (value != (Strings.IDs)2960157992U && value != (Strings.IDs)3134958540U)
					{
						if (value != (Strings.IDs)3376565836U)
						{
							goto IL_EC;
						}
						goto IL_BC;
					}
				}
				else if (value != (Strings.IDs)3931032456U && value != (Strings.IDs)4118843607U)
				{
					if (value != (Strings.IDs)4287340460U)
					{
						goto IL_EC;
					}
					goto IL_BC;
				}
				this.ErrorFormatter = delegate(TrackingError error, bool isMultiMessageSearch)
				{
					if (string.IsNullOrEmpty(error.Target))
					{
						return this.GetGenericErrorMessageIfMissingArguments(isMultiMessageSearch);
					}
					Strings.IDs key = this.GetMessage(isMultiMessageSearch);
					return string.Format(Strings.GetLocalizedString(key), error.Server, error.Domain, error.Target);
				};
				return;
				IL_BC:
				this.ErrorFormatter = delegate(TrackingError error, bool isMultiMessageSearch)
				{
					if (string.IsNullOrEmpty(error.Data))
					{
						return this.GetGenericErrorMessageIfMissingArguments(isMultiMessageSearch);
					}
					Strings.IDs key = this.GetMessage(isMultiMessageSearch);
					return string.Format(Strings.GetLocalizedString(key), error.Server, error.Domain, error.Data);
				};
				return;
				IL_EC:
				this.ErrorFormatter = ((TrackingError error, bool isMultiMessageSearch) => Strings.GetLocalizedString(this.GetMessage(isMultiMessageSearch)));
			}
		}

		public Strings.IDs MultiMessageSearchMessage { get; set; }

		public ErrorCodeInformationAttribute.FormatterMethod ErrorFormatter { get; private set; }

		public RequiredProperty RequiredProperties { get; set; }

		private Strings.IDs GetMessage(bool isMultiMessageSearch)
		{
			if (!isMultiMessageSearch || (Strings.IDs)0U >= this.MultiMessageSearchMessage)
			{
				return this.message;
			}
			return this.MultiMessageSearchMessage;
		}

		private LocalizedString GetGenericErrorMessageIfMissingArguments(bool isMultiMessageSearch)
		{
			if (isMultiMessageSearch)
			{
				return Strings.GetLocalizedString(this.MultiMessageSearchMessage);
			}
			if (this.IsTransientError)
			{
				return Strings.TrackingTransientError;
			}
			return Strings.TrackingPermanentError;
		}

		private static readonly ErrorCodeInformationAttribute.FormatterMethod LogSearchDownFormatter = (TrackingError error, bool isMultiMessageSearch) => string.Format(Strings.TrackingErrorLogSearchServiceDown, error.Target);

		private Strings.IDs message;

		internal delegate string FormatterMethod(TrackingError error, bool isMultiMessageSearch);
	}
}
