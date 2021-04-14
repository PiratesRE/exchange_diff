using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	[Serializable]
	public sealed class InterceptorAgentAction : IEquatable<InterceptorAgentAction>
	{
		public InterceptorAgentAction(InterceptorAgentRuleBehavior action)
		{
			if (action != InterceptorAgentRuleBehavior.Drop && action != InterceptorAgentRuleBehavior.NoOp)
			{
				throw new InvalidOperationException("Only the drop or no-op actions have no additional parameters");
			}
			this.action = action;
		}

		public InterceptorAgentAction(InterceptorAgentRuleBehavior action, SmtpResponse response)
		{
			if (SmtpResponse.Empty.Equals(response))
			{
				throw new ArgumentException("Empty SmtpResponse", "response");
			}
			switch (action)
			{
			case InterceptorAgentRuleBehavior.PermanentReject:
			case InterceptorAgentRuleBehavior.TransientReject:
				this.response = response;
				this.action = action;
				return;
			default:
				throw new InvalidOperationException(string.Format("Wrong constructor for the specified action '{0}'", action));
			}
		}

		public InterceptorAgentAction(InterceptorAgentRuleBehavior action, TimeSpan delay)
		{
			if (action != InterceptorAgentRuleBehavior.Delay && action != InterceptorAgentRuleBehavior.Defer)
			{
				throw new InvalidOperationException("This constructor can only be used by Delay or Defer Behavior");
			}
			this.delay = delay;
			if (this.delay.Ticks > 0L)
			{
				this.DelaySpecified = true;
			}
			this.action = action;
		}

		public InterceptorAgentAction(InterceptorAgentRuleBehavior action, string path, SmtpResponse response)
		{
			if (!InterceptorAgentAction.IsArchivingBehavior(action))
			{
				throw new InvalidOperationException(string.Format("Wrong constructor for the specified action '{0}'", action));
			}
			this.path = path;
			this.action = action;
			if (action == InterceptorAgentRuleBehavior.ArchiveAndPermanentReject || action == InterceptorAgentRuleBehavior.ArchiveAndTransientReject)
			{
				this.response = response;
			}
		}

		private InterceptorAgentAction()
		{
		}

		[XmlAttribute]
		public InterceptorAgentRuleBehavior Action
		{
			get
			{
				return this.action;
			}
			set
			{
				this.action = value;
			}
		}

		[XmlAttribute]
		public string ResponseString
		{
			get
			{
				return this.response.ToString();
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.response = SmtpResponse.Empty;
					return;
				}
				if (!SmtpResponse.TryParse(value, out this.response))
				{
					throw new ArgumentException(string.Format("Cannot parse smtp response from {0}", value), "Response");
				}
			}
		}

		[XmlAttribute("Delay", DataType = "duration")]
		public string DelayString
		{
			get
			{
				return this.delay.ToString();
			}
			set
			{
				this.delay = TimeSpan.Parse(value);
				if (this.delay.Ticks > 0L)
				{
					this.DelaySpecified = true;
				}
			}
		}

		[XmlAttribute]
		public string Path
		{
			get
			{
				return this.path;
			}
			set
			{
				this.path = value;
			}
		}

		internal static string[] AllActions
		{
			get
			{
				return InterceptorAgentAction.allActionNames;
			}
		}

		[XmlIgnore]
		internal SmtpResponse Response
		{
			get
			{
				return this.response;
			}
		}

		[XmlIgnore]
		internal TimeSpan Delay
		{
			get
			{
				return this.delay;
			}
		}

		private bool DelaySpecified { get; set; }

		public static bool IsValidRuleBehavior(InterceptorAgentRuleBehavior behavior)
		{
			return Enum.GetValues(typeof(InterceptorAgentRuleBehavior)).Cast<object>().Any((object val) => behavior == (InterceptorAgentRuleBehavior)val);
		}

		public static bool IsArchivingBehavior(InterceptorAgentRuleBehavior behavior)
		{
			return (ushort)(behavior & (InterceptorAgentRuleBehavior.Archive | InterceptorAgentRuleBehavior.ArchiveHeaders)) != 0;
		}

		public static bool IsRejectingBehavior(InterceptorAgentRuleBehavior behavior)
		{
			return (ushort)(behavior & (InterceptorAgentRuleBehavior.PermanentReject | InterceptorAgentRuleBehavior.TransientReject)) != 0;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.ToString(stringBuilder);
			return stringBuilder.ToString();
		}

		public void ToString(StringBuilder result)
		{
			if (result == null)
			{
				throw new ArgumentNullException("result");
			}
			result.Append(this.Action.ToString());
			if (!SmtpResponse.Empty.Equals(this.response))
			{
				result.AppendFormat(" rejectResponse=\"{0}\"", this.response);
			}
			if ((ushort)(this.Action & InterceptorAgentRuleBehavior.Archive) != 0 || (ushort)(this.Action & InterceptorAgentRuleBehavior.ArchiveHeaders) != 0)
			{
				result.AppendFormat(" path=\"{0}\"", this.Path);
			}
			if (this.DelaySpecified && this.delay.Ticks > 0L)
			{
				result.AppendFormat(" delay=\"{0}\"", this.Delay);
			}
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as InterceptorAgentAction);
		}

		public override int GetHashCode()
		{
			return this.Action.GetHashCode() ^ this.response.ToString().GetHashCode() ^ this.Delay.GetHashCode() ^ ((this.Path == null) ? 0 : this.Path.GetHashCode());
		}

		public bool Equals(InterceptorAgentAction other)
		{
			return !object.ReferenceEquals(other, null) && (object.ReferenceEquals(this, other) || (this.Action == other.Action && !(this.Delay != other.Delay) && string.Equals(this.Path, other.Path) && this.response.ToString().Equals(other.response.ToString())));
		}

		internal InterceptorAgentRuleBehavior PerformAction(InterceptorAgentRule matchedRule, MailItem mailItem, Action drop, Action<SmtpResponse> reject, Action<TimeSpan> defer = null)
		{
			InterceptorAgentRuleBehavior interceptorAgentRuleBehavior = InterceptorAgentRuleBehavior.NoOp;
			if (this.action == InterceptorAgentRuleBehavior.NoOp)
			{
				return interceptorAgentRuleBehavior;
			}
			if (matchedRule == null)
			{
				throw new ArgumentNullException("matchedRule");
			}
			bool flag = false;
			if ((ushort)(this.action & InterceptorAgentRuleBehavior.Archive) != 0)
			{
				if (mailItem == null)
				{
					throw new ArgumentNullException("mailItem");
				}
				if (!Archiver.Instance.TryArchiveMessage(mailItem, this.path ?? string.Empty))
				{
					ExTraceGlobals.InterceptorAgentTracer.TraceError((long)this.GetHashCode(), "Failed to archive mail item; will not reject or drop it");
					flag = true;
				}
				else
				{
					interceptorAgentRuleBehavior |= InterceptorAgentRuleBehavior.Archive;
				}
			}
			else if ((ushort)(this.action & InterceptorAgentRuleBehavior.ArchiveHeaders) != 0)
			{
				if (mailItem == null)
				{
					throw new ArgumentNullException("mailItem");
				}
				if (!Archiver.Instance.TryArchiveHeaders(mailItem, this.path ?? string.Empty))
				{
					ExTraceGlobals.InterceptorAgentTracer.TraceError((long)this.GetHashCode(), "Failed to archive mail item headers");
				}
				else
				{
					interceptorAgentRuleBehavior |= InterceptorAgentRuleBehavior.ArchiveHeaders;
				}
			}
			if (!flag && (ushort)(this.action & InterceptorAgentRuleBehavior.Drop) != 0)
			{
				if (drop == null)
				{
					throw new ArgumentNullException("drop");
				}
				drop();
				interceptorAgentRuleBehavior |= InterceptorAgentRuleBehavior.Drop;
			}
			else if (!flag && ((ushort)(this.action & InterceptorAgentRuleBehavior.PermanentReject) != 0 || (ushort)(this.action & InterceptorAgentRuleBehavior.TransientReject) != 0))
			{
				if (reject == null)
				{
					throw new ArgumentNullException("reject");
				}
				if ((ushort)(this.action & InterceptorAgentRuleBehavior.PermanentReject) != 0)
				{
					if (!this.RuleAlreadyApplied(matchedRule.Guid, mailItem))
					{
						reject(this.response);
						interceptorAgentRuleBehavior |= InterceptorAgentRuleBehavior.PermanentReject;
					}
				}
				else
				{
					reject(this.response);
					interceptorAgentRuleBehavior |= InterceptorAgentRuleBehavior.TransientReject;
				}
			}
			else if ((ushort)(this.action & InterceptorAgentRuleBehavior.Defer) != 0)
			{
				if (defer == null)
				{
					throw new ArgumentNullException("defer");
				}
				if (!this.RuleAlreadyApplied(matchedRule.Guid, mailItem))
				{
					defer(this.delay);
					interceptorAgentRuleBehavior |= InterceptorAgentRuleBehavior.Defer;
				}
			}
			if ((ushort)(this.action & InterceptorAgentRuleBehavior.Delay) != 0 && this.delay > TimeSpan.Zero && this.delay < TimeSpan.FromMilliseconds(2147483647.0))
			{
				ExTraceGlobals.InterceptorAgentTracer.TraceInformation<string>(0, (long)this.GetHashCode(), "Delaying message by blocking thread for {0}", this.Delay.ToString());
				Thread.Sleep(this.delay);
				interceptorAgentRuleBehavior |= InterceptorAgentRuleBehavior.Delay;
			}
			return interceptorAgentRuleBehavior;
		}

		internal void Verify()
		{
			if (!InterceptorAgentAction.IsValidRuleBehavior(this.action))
			{
				throw new InvalidOperationException(string.Format("Invalid action '{0}'", this.action));
			}
			if ((this.action == InterceptorAgentRuleBehavior.Delay || this.action == InterceptorAgentRuleBehavior.Defer) && this.Delay == TimeSpan.Zero)
			{
				throw new InvalidOperationException("No delay specified for delay or defer action");
			}
			if (this.action == InterceptorAgentRuleBehavior.TransientReject && this.Response.SmtpResponseType != SmtpResponseType.TransientError)
			{
				throw new InvalidOperationException("TransientReject must have a transient error response code");
			}
			if (this.action == InterceptorAgentRuleBehavior.PermanentReject && this.Response.SmtpResponseType != SmtpResponseType.PermanentError)
			{
				throw new InvalidOperationException("PermanentReject must have a permanent error response code");
			}
		}

		private bool RuleAlreadyApplied(Guid matchedRuleGuid, MailItem mailItem)
		{
			if (mailItem == null || mailItem.MimeDocument == null || mailItem.MimeDocument.RootPart == null || mailItem.MimeDocument.RootPart.Headers == null)
			{
				return false;
			}
			HeaderList headers = mailItem.MimeDocument.RootPart.Headers;
			Header[] array = headers.FindAll("X-MS-Exchange-Organization-Matched-Interceptor-Rule");
			foreach (Header header in array)
			{
				if (header.Value.Equals(matchedRuleGuid.ToString(), StringComparison.InvariantCultureIgnoreCase))
				{
					ExTraceGlobals.InterceptorAgentTracer.TraceInformation<Guid>(0, (long)this.GetHashCode(), "interceptor rule with guid {0} has already been applied on this message", matchedRuleGuid);
					return true;
				}
			}
			headers.AppendChild(new AsciiTextHeader("X-MS-Exchange-Organization-Matched-Interceptor-Rule", matchedRuleGuid.ToString()));
			return false;
		}

		private static readonly string[] allActionNames = Enum.GetNames(typeof(InterceptorAgentRuleBehavior));

		private SmtpResponse response;

		private TimeSpan delay = TimeSpan.Zero;

		private InterceptorAgentRuleBehavior action;

		private string path = string.Empty;
	}
}
