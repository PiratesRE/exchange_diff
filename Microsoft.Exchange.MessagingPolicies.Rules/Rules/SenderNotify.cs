using System;
using System.Text;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class SenderNotify : TransportAction
	{
		public SenderNotify(ShortList<Argument> arguments) : base(arguments)
		{
		}

		public override Type[] ArgumentsType
		{
			get
			{
				return SenderNotify.ArgumentTypes;
			}
		}

		public override TransportActionType Type
		{
			get
			{
				return TransportActionType.RecipientRelated;
			}
		}

		public override string Name
		{
			get
			{
				return "SenderNotify";
			}
		}

		public override Version MinimumVersion
		{
			get
			{
				return SenderNotify.SenderNotifyActionVersion;
			}
		}

		protected override ExecutionControl OnExecute(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			if (!TransportUtils.IsInternalMail(transportRulesEvaluationContext))
			{
				return ExecutionControl.Execute;
			}
			SenderNotify.SenderNotifyType senderNotifyType = SenderNotify.SenderNotifyType.NotifyOnly;
			try
			{
				senderNotifyType = (SenderNotify.SenderNotifyType)Enum.Parse(typeof(SenderNotify.SenderNotifyType), (string)base.Arguments[0].GetValue(transportRulesEvaluationContext));
			}
			catch (ArgumentOutOfRangeException)
			{
				string message = TransportRulesStrings.InvalidNotifySenderTypeArgument(base.Arguments[0].GetValue(transportRulesEvaluationContext));
				ExTraceGlobals.TransportRulesEngineTracer.TraceError(0L, message);
				throw new TransportRulePermanentException(message);
			}
			catch (ArgumentException)
			{
				string message2 = TransportRulesStrings.InvalidNotifySenderTypeArgument(base.Arguments[0].GetValue(transportRulesEvaluationContext));
				ExTraceGlobals.TransportRulesEngineTracer.TraceError(0L, message2);
				throw new TransportRulePermanentException(message2);
			}
			catch (OverflowException)
			{
				string message3 = TransportRulesStrings.InvalidNotifySenderTypeArgument(base.Arguments[0].GetValue(transportRulesEvaluationContext));
				ExTraceGlobals.TransportRulesEngineTracer.TraceError(0L, message3);
				throw new TransportRulePermanentException(message3);
			}
			string status = (string)base.Arguments[1].GetValue(transportRulesEvaluationContext);
			string text = (string)base.Arguments[2].GetValue(transportRulesEvaluationContext);
			string reason = (string)base.Arguments[3].GetValue(transportRulesEvaluationContext);
			bool flag = SenderNotify.IsFpHeaderSet(transportRulesEvaluationContext);
			string senderOverrideJustification;
			bool senderOverrideHeader = SenderNotify.GetSenderOverrideHeader(transportRulesEvaluationContext, out senderOverrideJustification);
			switch (senderNotifyType)
			{
			case SenderNotify.SenderNotifyType.NotifyOnly:
				if (flag)
				{
					transportRulesEvaluationContext.FpOverriden = true;
				}
				break;
			case SenderNotify.SenderNotifyType.RejectMessage:
				transportRulesEvaluationContext.ShouldExecuteActions = false;
				return RejectMessage.Reject(transportRulesEvaluationContext, status, flag ? string.Empty : text, reason);
			case SenderNotify.SenderNotifyType.RejectUnlessFalsePositiveOverride:
				if (!flag)
				{
					transportRulesEvaluationContext.ShouldExecuteActions = false;
					return RejectMessage.Reject(transportRulesEvaluationContext, status, text, reason);
				}
				transportRulesEvaluationContext.FpOverriden = true;
				break;
			case SenderNotify.SenderNotifyType.RejectUnlessSilentOverride:
			case SenderNotify.SenderNotifyType.RejectUnlessExplicitOverride:
				if (!flag && !senderOverrideHeader)
				{
					transportRulesEvaluationContext.ShouldExecuteActions = false;
					return RejectMessage.Reject(transportRulesEvaluationContext, status, text, reason);
				}
				if (senderOverrideHeader)
				{
					transportRulesEvaluationContext.SenderOverridden = true;
					transportRulesEvaluationContext.SenderOverrideJustification = senderOverrideJustification;
				}
				if (flag)
				{
					transportRulesEvaluationContext.FpOverriden = true;
				}
				break;
			}
			return ExecutionControl.Execute;
		}

		internal static bool IsFpHeaderSet(TransportRulesEvaluationContext context)
		{
			return context.Message.Headers["X-Ms-Exchange-Organization-Dlp-FalsePositive"].Count > 0;
		}

		internal static bool GetSenderOverrideHeader(TransportRulesEvaluationContext context, out string senderOverrideHeaderValue)
		{
			bool flag = context.Message.Headers["X-Ms-Exchange-Organization-Dlp-SenderOverrideJustification"].Count > 0;
			senderOverrideHeaderValue = string.Empty;
			if (flag && !SenderNotify.TryGetBase64EncodedValue(context.Message.Headers["X-Ms-Exchange-Organization-Dlp-SenderOverrideJustification"][0], SenderNotify.MaxJustificationLength, out senderOverrideHeaderValue))
			{
				ExTraceGlobals.TransportRulesEngineTracer.TraceError<string, string>(0L, "Header {0} is not a valid base-64 encoded text \"{1}\".", "X-Ms-Exchange-Organization-Dlp-SenderOverrideJustification", context.Message.Headers["X-Ms-Exchange-Organization-Dlp-SenderOverrideJustification"][0]);
			}
			return flag;
		}

		internal static bool TryGetBase64EncodedValue(string base64EncodedString, int maxLength, out string value)
		{
			value = string.Empty;
			try
			{
				byte[] bytes = Convert.FromBase64String(base64EncodedString);
				value = Encoding.Unicode.GetString(bytes);
				if (value.Length > maxLength)
				{
					value = value.Substring(0, maxLength);
				}
			}
			catch (FormatException)
			{
				return false;
			}
			return true;
		}

		private static readonly int MaxJustificationLength = 100;

		public static readonly Version SenderNotifyActionVersion = new Version("15.00.0002.000");

		private static readonly Type[] ArgumentTypes = new Type[]
		{
			typeof(string),
			typeof(string),
			typeof(string),
			typeof(string)
		};

		public enum SenderNotifyType
		{
			NotifyOnly = 1,
			RejectMessage,
			RejectUnlessFalsePositiveOverride,
			RejectUnlessSilentOverride,
			RejectUnlessExplicitOverride
		}
	}
}
