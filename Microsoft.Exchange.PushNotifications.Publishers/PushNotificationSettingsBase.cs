using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal abstract class PushNotificationSettingsBase
	{
		public PushNotificationSettingsBase(string appId)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("appId", appId);
			this.AppId = appId;
		}

		public string AppId { get; private set; }

		public bool IsSuitable
		{
			get
			{
				if (this.isSuitable == null)
				{
					this.RunBaseSuitabilityCheck();
				}
				return this.isSuitable.Value;
			}
		}

		public bool IsValid
		{
			get
			{
				if (this.isValid == null)
				{
					this.RunBaseValidationCheck();
				}
				return this.isValid.Value;
			}
		}

		public List<LocalizedString> ValidationErrors
		{
			get
			{
				if (!this.IsValid)
				{
					return this.validationErrors;
				}
				throw new InvalidOperationException("ValidationErrors are not available when the instance is valid");
			}
		}

		public void Validate()
		{
			if (!this.IsValid)
			{
				throw new PushNotificationConfigurationException(this.validationErrors[0]);
			}
		}

		protected virtual void RunValidationCheck(List<LocalizedString> errors)
		{
		}

		protected virtual bool RunSuitabilityCheck()
		{
			return true;
		}

		private void RunBaseValidationCheck()
		{
			List<LocalizedString> list = new List<LocalizedString>();
			this.RunValidationCheck(list);
			if (list.Count == 0)
			{
				this.isValid = new bool?(true);
				return;
			}
			this.validationErrors = list;
			this.isValid = new bool?(false);
		}

		private void RunBaseSuitabilityCheck()
		{
			bool value;
			if (this.IsValid)
			{
				value = this.RunSuitabilityCheck();
			}
			else
			{
				string text = string.Join<LocalizedString>("\n\r", this.validationErrors.ToArray());
				PushNotificationsCrimsonEvents.PushNotificationPublisherConfigurationError.Log<string, string, string>(this.AppId, string.Empty, text);
				ExTraceGlobals.PublisherManagerTracer.TraceError<string, string>((long)this.GetHashCode(), "[PushNotificationSettingsBase:RunBaseSuitabilityCheck] Configuration for '{0}' has validation errors: {1}", this.AppId, text);
				value = false;
			}
			this.isSuitable = new bool?(value);
		}

		private bool? isValid;

		private List<LocalizedString> validationErrors;

		private bool? isSuitable;
	}
}
