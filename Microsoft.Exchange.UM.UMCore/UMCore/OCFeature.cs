using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.ClientAccess.Messages;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class OCFeature
	{
		internal OCFeatureType FeatureType
		{
			get
			{
				return this.ocFeature;
			}
			set
			{
				this.ocFeature = value;
			}
		}

		internal bool SkipPin
		{
			get
			{
				return this.skipPin;
			}
		}

		internal string Subject
		{
			get
			{
				return this.subject;
			}
		}

		internal bool IsUrgent
		{
			get
			{
				return this.isUrgent;
			}
		}

		internal string ReferredBy
		{
			get
			{
				return this.referredBy;
			}
		}

		internal void Parse(CallContext context, IList<PlatformSignalingHeader> headers, string resourcePath)
		{
			string text = null;
			RouterUtils.TryGetHeaderValue(headers, "Referred-By", out this.referredBy);
			RouterUtils.TryGetHeaderValue(headers, "Subject", out this.subject);
			RouterUtils.TryGetHeaderValue(headers, "Priority", out text);
			this.isUrgent = (0 == string.Compare(text, "urgent", StringComparison.InvariantCultureIgnoreCase));
			string featureData = null;
			if (RouterUtils.TryGetHeaderValue(headers, "Ms-Exchange-Command", out text))
			{
				this.ParseMsExchangeCommandHeader(context, text, out this.skipPin, out featureData);
			}
			else
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "ProcessOCFeature: Ms-Exchange-Command header not specified", new object[0]);
			}
			if (string.IsNullOrEmpty(resourcePath))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "ProcessOCFeature: No local-resource-path found, no special OC feature requested", new object[0]);
				return;
			}
			try
			{
				this.SelectOCFeature(context, (OCFeatureType)Enum.Parse(typeof(OCFeatureType), resourcePath, true), featureData);
			}
			catch (ArgumentException ex)
			{
				CallIdTracer.TraceError(ExTraceGlobals.CallSessionTracer, this, "ProcessOCFeature: Enum.Parse - {0}", new object[]
				{
					ex
				});
				throw CallRejectedException.Create(Strings.OCFeatureDataValidation(Strings.OCFeatureInvalidLocalResourcePath(resourcePath)), CallEndingReason.OCFeatureInvalidLocalResourcePath, "local-resource-path: {0}.", new object[]
				{
					resourcePath
				});
			}
		}

		private void ParseMsExchangeCommandHeader(CallContext context, string headerValue, out bool skipPin, out string itemId)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "ParseMsExchangeCommandHeader: {0}", new object[]
			{
				headerValue
			});
			skipPin = false;
			itemId = null;
			string[] array = headerValue.Split(new char[]
			{
				';'
			}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text in array)
			{
				string text2 = text.Trim();
				if (string.Equals(text2, "skip-pin", StringComparison.InvariantCultureIgnoreCase))
				{
					skipPin = true;
					if (skipPin && !string.IsNullOrEmpty(context.Extension))
					{
						CallIdTracer.TraceError(ExTraceGlobals.CallSessionTracer, this, "Skip-pin specified on a non subscriber access call (diversion={0})", new object[]
						{
							context.Extension
						});
						skipPin = false;
					}
				}
				else
				{
					int num = text2.IndexOf("itemId=", StringComparison.InvariantCultureIgnoreCase);
					if (num >= 0)
					{
						itemId = text2.Substring(num + "itemId=".Length);
						if (itemId.Length >= 2 && itemId[0] == '"' && itemId[itemId.Length - 1] == '"')
						{
							itemId = itemId.Substring(1, itemId.Length - 2);
						}
					}
				}
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "ParseMsExchangeCommandHeader: skipPin={0} itemId={1}", new object[]
			{
				headerValue,
				skipPin,
				itemId
			});
		}

		private void SelectOCFeature(CallContext context, OCFeatureType feature, string featureData)
		{
			this.ocFeature = feature;
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "SelectOCFeature: Selecting OCFeature={0}, data={1}", new object[]
			{
				this.ocFeature,
				featureData
			});
			if (feature == OCFeatureType.None)
			{
				return;
			}
			bool flag = feature == OCFeatureType.Greeting || feature == OCFeatureType.Voicemail;
			if (flag)
			{
				if (!string.IsNullOrEmpty(context.Extension))
				{
					CallIdTracer.TraceError(ExTraceGlobals.CallSessionTracer, this, "SelectOCFeature: Subscriber access feature {0} cannot specify diversion {1}", new object[]
					{
						feature,
						context.Extension
					});
					throw CallRejectedException.Create(Strings.OCFeatureDataValidation(Strings.OCFeatureSACannotHaveDiversion(feature.ToString())), CallEndingReason.OCFeatureSACannotHaveDiversion, "Feature requested: {0}.", new object[]
					{
						feature
					});
				}
			}
			else if (string.IsNullOrEmpty(context.Extension))
			{
				CallIdTracer.TraceError(ExTraceGlobals.CallSessionTracer, this, "SelectOCFeature: Call answering feature {0} cannot have an empty diversion", new object[]
				{
					feature
				});
				throw CallRejectedException.Create(Strings.OCFeatureDataValidation(Strings.OCFeatureCAMustHaveDiversion(feature.ToString())), CallEndingReason.OCFeatureCAMustHaveDiversion, "Feature requested: {0}.", new object[]
				{
					feature.ToString()
				});
			}
			switch (this.ocFeature)
			{
			case OCFeatureType.Greeting:
				context.WebServiceRequest = new PlayOnPhoneGreetingRequest
				{
					GreetingType = UMGreetingType.NormalCustom,
					DialString = string.Empty
				};
				return;
			case OCFeatureType.Voicemail:
				if (!string.IsNullOrEmpty(featureData))
				{
					this.ValidateVoicemailItemId(featureData);
					this.ocFeature = OCFeatureType.SingleVoicemail;
					context.WebServiceRequest = new PlayOnPhoneMessageRequest
					{
						ObjectId = featureData,
						DialString = string.Empty
					};
					return;
				}
				break;
			default:
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "OCFeature={0} requires no additional preparation", new object[]
				{
					this.ocFeature
				});
				break;
			}
		}

		private void ValidateVoicemailItemId(string itemId)
		{
			try
			{
				Convert.FromBase64String(itemId);
			}
			catch (Exception ex)
			{
				if (ex is FormatException || ex is ArgumentException)
				{
					CallIdTracer.TraceError(ExTraceGlobals.CallSessionTracer, this, "ValidateVoicemailItemId: {0}", new object[]
					{
						ex
					});
					throw CallRejectedException.Create(Strings.OCFeatureDataValidation(Strings.OCFeatureInvalidItemId(itemId)), ex, CallEndingReason.OCFeatureInvalidItemId, "Item ID: {0}.", new object[]
					{
						itemId
					});
				}
				throw;
			}
		}

		private OCFeatureType ocFeature;

		private bool skipPin;

		private string subject;

		private bool isUrgent;

		private string referredBy;
	}
}
