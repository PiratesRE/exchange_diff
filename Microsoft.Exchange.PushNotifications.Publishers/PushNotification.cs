using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal abstract class PushNotification
	{
		protected PushNotification(string appId, OrganizationId tenantId)
		{
			this.AppId = appId;
			this.TenantId = tenantId;
			this.SequenceNumber = PushNotification.GetNextId();
		}

		public string AppId { get; private set; }

		public int SequenceNumber { get; private set; }

		public OrganizationId TenantId { get; private set; }

		public bool IsBackgroundSyncAvailable { get; protected set; }

		public string Identifier
		{
			get
			{
				if (this.identifier == null)
				{
					this.identifier = string.Format(PushNotification.IdTemplate, this.SequenceNumber.ToNullableString(null));
				}
				return this.identifier;
			}
		}

		public abstract string RecipientId { get; }

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

		public virtual bool IsMonitoring
		{
			get
			{
				return false;
			}
		}

		public void Validate()
		{
			if (!this.IsValid)
			{
				throw new InvalidPushNotificationException(this.validationErrors[0]);
			}
		}

		public sealed override string ToString()
		{
			if (this.toStringCache == null)
			{
				this.toStringCache = "id:" + this.Identifier.ToNullableString();
			}
			return this.toStringCache;
		}

		public string ToFullString()
		{
			if (this.toFullStringCache == null)
			{
				this.toFullStringCache = string.Format("{{{0}}}", this.InternalToFullString());
			}
			return this.toFullStringCache;
		}

		protected virtual string InternalToFullString()
		{
			return string.Format("appId:{0}; tenantId:{1}; id:{2}; isMonitoring:{3}", new object[]
			{
				this.AppId.ToNullableString(),
				this.TenantId.ToNullableString(null),
				this.Identifier.ToNullableString(),
				this.IsMonitoring.ToString()
			});
		}

		protected virtual void RunValidationCheck(List<LocalizedString> errors)
		{
			if (ExTraceGlobals.NotificationFormatTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.NotificationFormatTracer.TraceDebug<string>((long)this.GetHashCode(), "[PushNotification::RunValidationCheck] Validating notification '{0}'", this.ToFullString());
			}
			if (string.IsNullOrWhiteSpace(this.AppId))
			{
				errors.Add(Strings.InvalidAppId);
			}
		}

		private static int GetNextId()
		{
			return Interlocked.Increment(ref PushNotification.idCounter);
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

		private static readonly string IdTemplate = ExDateTime.UtcNow.ToString("yyyyMMdd-HHmmss-{0}");

		private static readonly char[] IdSeparator = new char[]
		{
			'-'
		};

		private static int idCounter;

		private string identifier;

		private string toStringCache;

		private string toFullStringCache;

		private bool? isValid;

		private List<LocalizedString> validationErrors;
	}
}
