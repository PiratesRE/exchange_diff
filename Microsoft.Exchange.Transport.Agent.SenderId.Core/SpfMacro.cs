using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.SenderId
{
	internal sealed class SpfMacro
	{
		private SpfMacro(SenderIdValidationContext context, MacroTermSpfNode domainSpec, bool exp, AsyncCallback asyncCallback, object asyncState)
		{
			this.context = context;
			this.currentMacroTerm = domainSpec;
			this.expanded = new StringBuilder();
			this.exp = exp;
			this.asyncResult = new SenderIdAsyncResult(asyncCallback, asyncState);
		}

		public static IAsyncResult BeginExpandDomainSpec(SenderIdValidationContext context, MacroTermSpfNode domainSpec, AsyncCallback asyncCallback, object asyncState)
		{
			SpfMacro spfMacro = new SpfMacro(context, domainSpec, false, asyncCallback, asyncState);
			return spfMacro.BeginExpandMacros();
		}

		public static SpfMacro.ExpandedMacro EndExpandDomainSpec(IAsyncResult ar)
		{
			SpfMacro.ExpandedMacro expandedMacro = SpfMacro.EndExpandMacros(ar);
			if (expandedMacro.IsValid && expandedMacro.Value.Length > 253)
			{
				int startIndex = expandedMacro.Value.Length - 253;
				int num = expandedMacro.Value.IndexOf(".", startIndex, StringComparison.OrdinalIgnoreCase);
				if (num >= 0)
				{
					expandedMacro = new SpfMacro.ExpandedMacro(true, expandedMacro.Value.Substring(num));
				}
				else
				{
					expandedMacro = new SpfMacro.ExpandedMacro(false, string.Empty);
				}
			}
			return expandedMacro;
		}

		public static IAsyncResult BeginExpandExp(SenderIdValidationContext context, string macro, AsyncCallback asyncCallback, object asyncState)
		{
			SpfMacro spfMacro = new SpfMacro(context, SpfParser.ParseExpMacro(macro), true, asyncCallback, asyncState);
			return spfMacro.BeginExpandMacros();
		}

		public static SpfMacro.ExpandedMacro EndExpandExp(IAsyncResult ar)
		{
			return SpfMacro.EndExpandMacros(ar);
		}

		private static SpfMacro.ExpandedMacro EndExpandMacros(IAsyncResult ar)
		{
			return (SpfMacro.ExpandedMacro)((SenderIdAsyncResult)ar).GetResult();
		}

		private static string DotFormattedIPv6Address(IPAddress address)
		{
			byte[] addressBytes = address.GetAddressBytes();
			StringBuilder stringBuilder = new StringBuilder(addressBytes.Length * 2);
			for (int i = 0; i < addressBytes.Length - 1; i++)
			{
				stringBuilder.AppendFormat("{0:x}.", (int)addressBytes[i]);
			}
			stringBuilder.AppendFormat("{0:x}", (int)addressBytes[addressBytes.Length - 1]);
			return stringBuilder.ToString();
		}

		private static string URLEscape(string s)
		{
			StringBuilder stringBuilder = new StringBuilder(s.Length * 2);
			for (int i = 0; i < s.Length; i++)
			{
				if (SpfMacro.UnreservedURLCharacters[(int)s[i]])
				{
					stringBuilder.Append(s[i]);
				}
				else
				{
					stringBuilder.AppendFormat("%{0:x}", (int)s[i]);
				}
			}
			return stringBuilder.ToString();
		}

		private static bool[] GetUnreservedURLCharacters()
		{
			bool[] array = new bool[255];
			for (char c = 'a'; c <= 'z'; c += '\u0001')
			{
				array[(int)c] = true;
				array[(int)char.ToUpperInvariant(c)] = true;
			}
			for (char c2 = '0'; c2 <= '9'; c2 += '\u0001')
			{
				array[(int)c2] = true;
			}
			array[45] = true;
			array[95] = true;
			array[46] = true;
			array[33] = true;
			array[126] = true;
			array[42] = true;
			array[39] = true;
			array[40] = true;
			array[41] = true;
			return array;
		}

		private IAsyncResult BeginExpandMacros()
		{
			if (this.currentMacroTerm == null)
			{
				this.asyncResult.InvokeCompleted(new SpfMacro.ExpandedMacro(false, null));
			}
			else
			{
				this.ExpandRemainingMacros();
			}
			return this.asyncResult;
		}

		private void ExpandRemainingMacros()
		{
			while (this.currentMacroTerm != null)
			{
				if (this.currentMacroTerm.IsLiteral)
				{
					MacroLiteralSpfNode macroLiteralSpfNode = (MacroLiteralSpfNode)this.currentMacroTerm;
					this.expanded.Append(macroLiteralSpfNode.Literal);
					this.currentMacroTerm = this.currentMacroTerm.Next;
				}
				else
				{
					if (this.currentMacroTerm.IsExpand)
					{
						MacroExpandSpfNode expandTerm = (MacroExpandSpfNode)this.currentMacroTerm;
						this.BeginExpandMacro(expandTerm, new AsyncCallback(this.ExpandMacroCallback), null);
						return;
					}
					this.asyncResult.InvokeCompleted(new SpfMacro.ExpandedMacro(false, null));
					return;
				}
			}
			this.asyncResult.InvokeCompleted(new SpfMacro.ExpandedMacro(true, this.expanded.ToString()));
		}

		private void ExpandMacroCallback(IAsyncResult ar)
		{
			string value = this.EndExpandMacro(ar);
			this.expanded.Append(value);
			this.currentMacroTerm = this.currentMacroTerm.Next;
			this.ExpandRemainingMacros();
		}

		private IAsyncResult BeginExpandMacro(MacroExpandSpfNode expandTerm, AsyncCallback asyncCallback, object asyncState)
		{
			SenderIdAsyncResult result = new SenderIdAsyncResult(asyncCallback, asyncState);
			SpfMacro.ExpandMacroAsyncState asyncState2 = new SpfMacro.ExpandMacroAsyncState(expandTerm, result);
			this.BeginExpandMacroCharacter(expandTerm.MacroCharacter, new AsyncCallback(this.ExpandMacroCharacterCallback), asyncState2);
			return result;
		}

		private string EndExpandMacro(IAsyncResult ar)
		{
			return (string)((SenderIdAsyncResult)ar).GetResult();
		}

		private void ExpandMacroCharacterCallback(IAsyncResult ar)
		{
			SpfMacro.ExpandMacroAsyncState expandMacroAsyncState = (SpfMacro.ExpandMacroAsyncState)ar.AsyncState;
			MacroExpandSpfNode macro = expandMacroAsyncState.Macro;
			string text = this.EndExpandMacroCharacter(ar);
			if (macro.TransformerDigits != 0 || macro.TransformerReverse || macro.Delimiters.Length > 0)
			{
				string[] array = text.Split(macro.Delimiters.ToCharArray());
				if (macro.TransformerReverse)
				{
					Array.Reverse(array);
				}
				int num = macro.TransformerDigits;
				if (num == 0 || num > array.Length)
				{
					num = array.Length;
				}
				text = string.Join(".", array, array.Length - num, num);
			}
			if (char.IsUpper(macro.MacroCharacter))
			{
				text = SpfMacro.URLEscape(text);
			}
			expandMacroAsyncState.AsyncResult.InvokeCompleted(text);
		}

		private IAsyncResult BeginExpandMacroCharacter(char macroCharacter, AsyncCallback asyncCallback, object asyncState)
		{
			SenderIdAsyncResult senderIdAsyncResult = new SenderIdAsyncResult(asyncCallback, asyncState);
			string text = null;
			macroCharacter = char.ToLowerInvariant(macroCharacter);
			char c = macroCharacter;
			if (c <= 'p')
			{
				if (c == 'd')
				{
					text = this.context.PurportedResponsibleDomain;
					goto IL_2C1;
				}
				switch (c)
				{
				case 'h':
					text = (this.context.BaseContext.HelloDomain ?? string.Empty);
					this.context.BaseContext.SetUncacheable();
					goto IL_2C1;
				case 'i':
					if (this.context.BaseContext.IPAddress.AddressFamily == AddressFamily.InterNetwork)
					{
						text = this.context.BaseContext.IPAddress.ToString();
						goto IL_2C1;
					}
					if (this.context.BaseContext.IPAddress.AddressFamily == AddressFamily.InterNetworkV6)
					{
						text = SpfMacro.DotFormattedIPv6Address(this.context.BaseContext.IPAddress);
						goto IL_2C1;
					}
					throw new InvalidOperationException("IP address has invalid address family");
				case 'l':
					text = this.context.BaseContext.PurportedResponsibleAddress.LocalPart;
					this.context.BaseContext.SetUncacheable();
					goto IL_2C1;
				case 'o':
					text = this.context.BaseContext.PurportedResponsibleAddress.DomainPart;
					this.context.BaseContext.SetUncacheable();
					goto IL_2C1;
				case 'p':
				{
					if (this.context.BaseContext.ExpandedPMacro != null)
					{
						text = this.context.BaseContext.ExpandedPMacro;
						goto IL_2C1;
					}
					SpfMacro.ExpandPMacroAsyncState asyncState2 = new SpfMacro.ExpandPMacroAsyncState(senderIdAsyncResult);
					this.BeginGetValidatedDomainName(new AsyncCallback(this.GetValidatedDomainNameCallback), asyncState2);
					return senderIdAsyncResult;
				}
				}
			}
			else
			{
				if (c == 's')
				{
					text = (string)this.context.BaseContext.PurportedResponsibleAddress;
					this.context.BaseContext.SetUncacheable();
					goto IL_2C1;
				}
				if (c == 'v')
				{
					if (this.context.BaseContext.IPAddress.AddressFamily == AddressFamily.InterNetwork)
					{
						text = "in-addr";
						goto IL_2C1;
					}
					if (this.context.BaseContext.IPAddress.AddressFamily == AddressFamily.InterNetworkV6)
					{
						text = "ip6";
						goto IL_2C1;
					}
					throw new InvalidOperationException("IP address has invalid address family");
				}
			}
			if (this.exp)
			{
				char c2 = macroCharacter;
				if (c2 != 'c')
				{
					switch (c2)
					{
					case 'r':
						text = this.context.BaseContext.Server.Name;
						break;
					case 't':
						text = ((int)DateTime.UtcNow.Subtract(SpfMacro.Epoch).TotalSeconds).ToString(CultureInfo.InvariantCulture);
						break;
					}
				}
				else
				{
					text = this.context.BaseContext.IPAddress.ToString();
				}
			}
			IL_2C1:
			if (text != null)
			{
				senderIdAsyncResult.InvokeCompleted(text);
			}
			else
			{
				this.context.ValidationCompleted(SenderIdStatus.PermError);
			}
			return senderIdAsyncResult;
		}

		private string EndExpandMacroCharacter(IAsyncResult ar)
		{
			return (string)((SenderIdAsyncResult)ar).GetResult();
		}

		private void GetValidatedDomainNameCallback(IAsyncResult ar)
		{
			SpfMacro.ExpandPMacroAsyncState expandPMacroAsyncState = (SpfMacro.ExpandPMacroAsyncState)ar.AsyncState;
			string invokeResult = this.EndGetValidatedDomainName(ar);
			expandPMacroAsyncState.AsyncResult.InvokeCompleted(invokeResult);
		}

		private IAsyncResult BeginGetValidatedDomainName(AsyncCallback asyncCallback, object asyncState)
		{
			SenderIdAsyncResult senderIdAsyncResult = new SenderIdAsyncResult(asyncCallback, asyncState);
			Util.AsyncDns.BeginPtrRecordQuery(this.context.BaseContext.IPAddress, new AsyncCallback(this.PtrCallback), senderIdAsyncResult);
			return senderIdAsyncResult;
		}

		private string EndGetValidatedDomainName(IAsyncResult ar)
		{
			return (string)((SenderIdAsyncResult)ar).GetResult();
		}

		private void PtrCallback(IAsyncResult ar)
		{
			string[] array;
			DnsStatus dnsStatus = Util.AsyncDns.EndPtrRecordQuery(ar, out array);
			SenderIdAsyncResult senderIdAsyncResult = (SenderIdAsyncResult)ar.AsyncState;
			string text2;
			if (dnsStatus == DnsStatus.Success)
			{
				string text = array[0];
				int num = 0;
				while (num < array.Length && num < 10)
				{
					if (array[num].EndsWith(this.context.PurportedResponsibleDomain, StringComparison.OrdinalIgnoreCase))
					{
						text = array[num];
						break;
					}
					num++;
				}
				text2 = text;
			}
			else
			{
				text2 = "unknown";
			}
			this.context.BaseContext.ExpandedPMacro = text2;
			senderIdAsyncResult.InvokeCompleted(text2);
		}

		private const int MaxDomainLength = 253;

		private const string UnknownDomainName = "unknown";

		private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		private static readonly bool[] UnreservedURLCharacters = SpfMacro.GetUnreservedURLCharacters();

		private SenderIdValidationContext context;

		private MacroTermSpfNode currentMacroTerm;

		private bool exp;

		private SenderIdAsyncResult asyncResult;

		private StringBuilder expanded;

		internal class ExpandedMacro
		{
			public ExpandedMacro(bool isValid, string value)
			{
				this.isValid = isValid;
				this.value = value;
			}

			public bool IsValid
			{
				get
				{
					return this.isValid;
				}
			}

			public string Value
			{
				get
				{
					return this.value;
				}
			}

			private readonly bool isValid;

			private readonly string value;
		}

		private class ExpandMacroAsyncState
		{
			public ExpandMacroAsyncState(MacroExpandSpfNode macro, SenderIdAsyncResult asyncResult)
			{
				this.Macro = macro;
				this.AsyncResult = asyncResult;
			}

			public readonly MacroExpandSpfNode Macro;

			public readonly SenderIdAsyncResult AsyncResult;
		}

		private class ExpandPMacroAsyncState
		{
			public ExpandPMacroAsyncState(SenderIdAsyncResult asyncResult)
			{
				this.AsyncResult = asyncResult;
			}

			public readonly SenderIdAsyncResult AsyncResult;
		}
	}
}
