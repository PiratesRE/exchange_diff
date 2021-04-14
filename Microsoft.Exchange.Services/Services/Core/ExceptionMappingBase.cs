using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.EventLogs;

namespace Microsoft.Exchange.Services.Core
{
	internal abstract class ExceptionMappingBase
	{
		public ExceptionMappingBase(Type exceptionType, ExceptionMappingBase.Attributes attributes)
		{
			this.ExceptionType = exceptionType;
			this.attributes = attributes;
		}

		public Type ExceptionType { get; private set; }

		public virtual ServiceError GetServiceError(LocalizedException exception, ExchangeVersion requestVersion)
		{
			ExTraceGlobals.ExceptionTracer.TraceError<LocalizedException>((long)this.GetHashCode(), "ExceptionMappingBase.GetServiceError called for exception: {0}", exception);
			LocalizedString localizedMessage = this.GetLocalizedMessage(exception);
			if (this.ReportException)
			{
				ServiceDiagnostics.ReportException(exception, ServicesEventLogConstants.Tuple_ErrorMappingNotFound, this, "Error mapping not found. Internal server error used for exception: {0}");
			}
			else
			{
				ExTraceGlobals.ExceptionTracer.TraceError<LocalizedString, LocalizedException>((long)this.GetHashCode(), "Mapping '{0}' used for exception '{1}'.", localizedMessage, exception);
			}
			ServiceError serviceError = new ServiceError(exception, localizedMessage, this.GetResponseCode(exception), 0, requestVersion, this.GetEffectiveVersion(exception), (this.attributes & ExceptionMappingBase.Attributes.StopsBatchProcessing) == ExceptionMappingBase.Attributes.StopsBatchProcessing, this.GetPropertyPaths(exception), this.GetConstantValues(exception));
			this.DoServiceErrorPostProcessing(exception, serviceError);
			return serviceError;
		}

		protected virtual PropertyPath[] GetPropertyPaths(LocalizedException exception)
		{
			return null;
		}

		public virtual LocalizedString GetLocalizedMessage(LocalizedException exception)
		{
			return this.GetLocalizedMessage(this.GetResourceId(exception));
		}

		protected virtual LocalizedString GetLocalizedMessage(CoreResources.IDs messageId)
		{
			return CoreResources.GetLocalizedString(messageId);
		}

		protected virtual IDictionary<string, string> GetConstantValues(LocalizedException exception)
		{
			return null;
		}

		protected virtual void DoServiceErrorPostProcessing(LocalizedException exception, ServiceError error)
		{
		}

		protected abstract ResponseCodeType GetResponseCode(LocalizedException exception);

		protected abstract ExchangeVersion GetEffectiveVersion(LocalizedException exception);

		protected abstract CoreResources.IDs GetResourceId(LocalizedException exception);

		public bool StopsBatchProcessing
		{
			get
			{
				return this.IsAttributeSet(ExceptionMappingBase.Attributes.StopsBatchProcessing);
			}
		}

		public bool IsUnmappedException
		{
			get
			{
				return this.IsAttributeSet(ExceptionMappingBase.Attributes.IsUnmappedException);
			}
		}

		public bool ReportException
		{
			get
			{
				return this.IsAttributeSet(ExceptionMappingBase.Attributes.ReportException);
			}
		}

		public bool TryInnerExceptionForExceptionMapping
		{
			get
			{
				return this.IsAttributeSet(ExceptionMappingBase.Attributes.TryInnerExceptionForExceptionMapping);
			}
		}

		protected T VerifyExceptionType<T>(LocalizedException exception) where T : LocalizedException
		{
			return exception as T;
		}

		private bool IsAttributeSet(ExceptionMappingBase.Attributes checkAttributes)
		{
			return (this.attributes & checkAttributes) == checkAttributes;
		}

		private ExceptionMappingBase.Attributes attributes;

		[Flags]
		public enum Attributes
		{
			None = 0,
			IsUnmappedException = 1,
			ReportException = 2,
			TryInnerExceptionForExceptionMapping = 4,
			StopsBatchProcessing = 8
		}
	}
}
