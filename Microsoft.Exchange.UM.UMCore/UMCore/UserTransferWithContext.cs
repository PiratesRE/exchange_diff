using System;
using System.Collections;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class UserTransferWithContext : ReferredByHeaderHandler
	{
		public UserTransferWithContext()
		{
		}

		public UserTransferWithContext(string referredByHostUri)
		{
			this.referredByHostUri = referredByHostUri;
		}

		public static bool TryParseReferredByHeader(string referredByHeader, UMDialPlan dialplan, out UMRecipient subscriber, out UserTransferWithContext.DeserializedReferredByHeader parsedHeader)
		{
			ValidateArgument.NotNullOrEmpty(referredByHeader, "ReferredByHeader");
			ValidateArgument.NotNull(dialplan, "UMDialPlan");
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "UserTransferWithContext::TryParseReferredByHeader() header value = [{0}]", new object[]
			{
				referredByHeader
			});
			UserTransferWithContext userTransferWithContext = new UserTransferWithContext();
			parsedHeader = null;
			subscriber = null;
			if (!userTransferWithContext.TryDeserialize(referredByHeader, out parsedHeader))
			{
				return false;
			}
			PIIMessage data = PIIMessage.Create(PIIType._PhoneNumber, parsedHeader.Extension);
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, data, "UserTransferWithContext::TryParseReferredByHeader() extension = _PhoneNumber, Calltype = {0}", new object[]
			{
				parsedHeader.TypeOfTransferredCall
			});
			try
			{
				subscriber = UMRecipient.Factory.FromExtension<UMRecipient>(parsedHeader.Extension, dialplan, null);
			}
			catch (LocalizedException ex)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, data, "Unable to validate entered mailbox digits '_PhoneNumber' due to exception e '{0}'", new object[]
				{
					ex
				});
			}
			if (subscriber == null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, data, "UserTransferWithContext::TryParseReferredByHeader Could not look up subscriber with extension = _PhoneNumber", new object[0]);
			}
			return subscriber != null;
		}

		internal bool TryDeserialize(string header, out UserTransferWithContext.DeserializedReferredByHeader headerValues)
		{
			Hashtable hashtable = base.ParseHeader(header);
			headerValues = new UserTransferWithContext.DeserializedReferredByHeader();
			if (hashtable.Count == 0)
			{
				return false;
			}
			if (!hashtable.ContainsKey("c"))
			{
				CallIdTracer.TraceWarning(ExTraceGlobals.CallSessionTracer, this, "TryDeserialize: Could not find a Command parameter", new object[0]);
				return false;
			}
			if (string.Equals((string)hashtable["c"], "ms-ova-data", StringComparison.OrdinalIgnoreCase))
			{
				headerValues.TypeOfTransferredCall = 3;
			}
			else
			{
				if (!string.Equals((string)hashtable["c"], "ms-ca-data", StringComparison.OrdinalIgnoreCase))
				{
					CallIdTracer.TraceWarning(ExTraceGlobals.CallSessionTracer, this, "TryDeserialize: Invalid Command value : '{0}'", new object[]
					{
						(string)hashtable["c"]
					});
					return false;
				}
				headerValues.TypeOfTransferredCall = 4;
			}
			if (!hashtable.ContainsKey("v") || !string.Equals((string)hashtable["v"], "1.0", StringComparison.OrdinalIgnoreCase))
			{
				CallIdTracer.TraceWarning(ExTraceGlobals.CallSessionTracer, this, "TryDeserialize: Could not find a Valid Version parameter", new object[0]);
				return false;
			}
			if (!hashtable.ContainsKey("extension") || string.IsNullOrEmpty((string)hashtable["extension"]))
			{
				CallIdTracer.TraceWarning(ExTraceGlobals.CallSessionTracer, this, "TryDeserialize: Could not find a Valid Extension parameter", new object[0]);
				return false;
			}
			headerValues.Extension = (string)hashtable["extension"];
			if (headerValues.TypeOfTransferredCall == 4)
			{
				if (!hashtable.ContainsKey("phone-context") || string.IsNullOrEmpty((string)hashtable["phone-context"]))
				{
					CallIdTracer.TraceWarning(ExTraceGlobals.CallSessionTracer, this, "TryDeserialize: Could not find a Valid PhoneContext parameter", new object[0]);
					return false;
				}
				headerValues.PhoneContext = (string)hashtable["phone-context"];
			}
			return true;
		}

		internal PlatformSipUri SerializeCACallTransferWithContextUri(string extension, string phoneContext)
		{
			if (string.IsNullOrEmpty(extension))
			{
				throw new ArgumentNullException("extension");
			}
			if (string.IsNullOrEmpty(phoneContext))
			{
				throw new ArgumentNullException("phoneContext");
			}
			return base.FrameHeader(new Hashtable
			{
				{
					"v",
					"1.0"
				},
				{
					"c",
					"ms-ca-data"
				},
				{
					"Extension",
					extension
				},
				{
					"phone-context",
					phoneContext
				}
			});
		}

		internal PlatformSipUri SerializeSACallTransferWithContextUri(string extension)
		{
			if (string.IsNullOrEmpty(extension))
			{
				throw new ArgumentNullException("extension");
			}
			return base.FrameHeader(new Hashtable
			{
				{
					"v",
					"1.0"
				},
				{
					"c",
					"ms-ova-data"
				},
				{
					"Extension",
					extension
				}
			});
		}

		private const string OvaData = "ms-ova-data";

		private const string CurrentVersion = "1.0";

		private const string CAData = "ms-ca-data";

		internal class DeserializedReferredByHeader
		{
			internal DeserializedReferredByHeader()
			{
				this.Extension = string.Empty;
				this.PhoneContext = string.Empty;
				this.TypeOfTransferredCall = 0;
			}

			internal string Extension { get; set; }

			internal string PhoneContext { get; set; }

			internal CallType TypeOfTransferredCall { get; set; }
		}
	}
}
