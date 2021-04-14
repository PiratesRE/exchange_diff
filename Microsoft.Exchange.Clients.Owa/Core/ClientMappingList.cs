using System;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class ClientMappingList
	{
		internal int Count
		{
			get
			{
				if (this.items == null)
				{
					return 0;
				}
				return this.items.Length;
			}
		}

		internal ClientMapping this[int index]
		{
			get
			{
				return this.items[index];
			}
		}

		internal ClientMappingList(ClientMapping[] items)
		{
			this.items = items;
		}

		internal bool FindMatchingRange(string application, string platform, ClientControl control, UserAgentParser.UserAgentVersion minVersion, out int firstMatch, out int lastMatch)
		{
			firstMatch = 0;
			lastMatch = this.Count - 1;
			if (this.Count == 0)
			{
				return false;
			}
			ClientMappingList.MatchingParameters parameters = new ClientMappingList.MatchingParameters(application, platform, control, minVersion);
			for (ClientMappingList.ClientAttribute clientAttribute = ClientMappingList.ClientAttribute.First; clientAttribute < ClientMappingList.ClientAttribute.Last; clientAttribute++)
			{
				if (!this.MatchClientAttribute(ref firstMatch, ref lastMatch, clientAttribute, parameters))
				{
					return false;
				}
			}
			return this.MatchClientMinimumVersion(firstMatch, ref lastMatch, parameters);
		}

		private bool MatchClientAttribute(ref int start, ref int end, ClientMappingList.ClientAttribute attribute, ClientMappingList.MatchingParameters parameters)
		{
			ExTraceGlobals.FormsRegistryCallTracer.TraceDebug((long)this.GetHashCode(), "ClientMappingList.MatchClientAttribute attribute = {0}, application = {1}, platform = {2}, control = {3}, start = {4}, end = {5}", new object[]
			{
				(int)attribute,
				parameters.Application,
				parameters.Platform,
				parameters.Control,
				start,
				end
			});
			if (start == end)
			{
				return this.IsMatchOrWildcard(start, attribute, parameters);
			}
			int num = end;
			if (!this.FindFirstMatch(start, ref num, attribute, parameters))
			{
				return false;
			}
			start = num;
			this.FindLastMatch(start, ref end, attribute, parameters);
			return true;
		}

		private bool FindFirstMatch(int first, ref int last, ClientMappingList.ClientAttribute attribute, ClientMappingList.MatchingParameters parameters)
		{
			int num = last;
			int num2 = first;
			first--;
			last++;
			while (last - first > 1)
			{
				int num3 = (first + last) / 2;
				bool flag = false;
				switch (attribute)
				{
				case ClientMappingList.ClientAttribute.First:
					flag = (string.CompareOrdinal(this.items[num3].Application, parameters.Application) < 0);
					break;
				case ClientMappingList.ClientAttribute.Platform:
					flag = (string.CompareOrdinal(this.items[num3].Platform, parameters.Platform) < 0);
					break;
				case ClientMappingList.ClientAttribute.Control:
					flag = (this.items[num3].Control < parameters.Control);
					break;
				}
				if (flag)
				{
					first = num3;
				}
				else
				{
					last = num3;
				}
			}
			if (last == num + 1 || !this.IsMatch(last, attribute, parameters))
			{
				if (!this.AttemptWildcardSearch(num2, attribute, parameters))
				{
					return false;
				}
				last = num2;
			}
			return true;
		}

		private void FindLastMatch(int first, ref int last, ClientMappingList.ClientAttribute attribute, ClientMappingList.MatchingParameters parameters)
		{
			if (attribute == ClientMappingList.ClientAttribute.Control)
			{
				while (first <= last && this.items[first].Control == parameters.Control)
				{
					first++;
				}
				last = first - 1;
				return;
			}
			first--;
			last++;
			while (last - first > 1)
			{
				int num = (first + last) / 2;
				bool flag = false;
				switch (attribute)
				{
				case ClientMappingList.ClientAttribute.First:
					flag = (string.CompareOrdinal(this.items[num].Application, parameters.Application) > 0);
					break;
				case ClientMappingList.ClientAttribute.Platform:
					flag = (string.CompareOrdinal(this.items[num].Platform, parameters.Platform) > 0);
					break;
				}
				if (flag)
				{
					last = num;
				}
				else
				{
					first = num;
				}
			}
			last = first;
		}

		private bool IsMatch(int index, ClientMappingList.ClientAttribute attribute, ClientMappingList.MatchingParameters parameters)
		{
			switch (attribute)
			{
			case ClientMappingList.ClientAttribute.First:
				return this.items[index].Application == parameters.Application;
			case ClientMappingList.ClientAttribute.Platform:
				return this.items[index].Platform == parameters.Platform;
			case ClientMappingList.ClientAttribute.Control:
				return this.items[index].Control == parameters.Control;
			default:
				return false;
			}
		}

		private bool IsMatchOrWildcard(int index, ClientMappingList.ClientAttribute attribute, ClientMappingList.MatchingParameters parameters)
		{
			switch (attribute)
			{
			case ClientMappingList.ClientAttribute.First:
				return this.IsMatch(index, attribute, parameters) || this.items[index].Application.Length == 0;
			case ClientMappingList.ClientAttribute.Platform:
				return this.IsMatch(index, attribute, parameters) || this.items[index].Platform.Length == 0;
			case ClientMappingList.ClientAttribute.Control:
				return this.IsMatch(index, attribute, parameters) || this.items[index].Control == ClientControl.None;
			default:
				return false;
			}
		}

		private bool AttemptWildcardSearch(int start, ClientMappingList.ClientAttribute attribute, ClientMappingList.MatchingParameters parameters)
		{
			ExTraceGlobals.FormsRegistryTracer.TraceDebug((long)this.GetHashCode(), "Attempting wildcard search");
			switch (attribute)
			{
			case ClientMappingList.ClientAttribute.First:
				if (this.items[start].Application.Length != 0)
				{
					return false;
				}
				parameters.Application = string.Empty;
				break;
			case ClientMappingList.ClientAttribute.Platform:
				if (this.items[start].Platform.Length != 0)
				{
					return false;
				}
				parameters.Platform = string.Empty;
				break;
			case ClientMappingList.ClientAttribute.Control:
				if (this.items[start].Control != ClientControl.None)
				{
					return false;
				}
				parameters.Control = ClientControl.None;
				break;
			}
			return true;
		}

		private bool MatchClientMinimumVersion(int start, ref int end, ClientMappingList.MatchingParameters parameters)
		{
			ExTraceGlobals.FormsRegistryCallTracer.TraceDebug<UserAgentParser.UserAgentVersion, int, int>((long)this.GetHashCode(), "ClientMappingList.MatchClientMinimumVersion version = {0}, start = {1}, end = {2}", parameters.Version, start, end);
			if (this.items[start].MinimumVersion.CompareTo(parameters.Version) > 0)
			{
				return false;
			}
			int num = start + 1;
			while (num <= end && this.items[num].MinimumVersion.CompareTo(parameters.Version) <= 0)
			{
				num++;
			}
			end = num - 1;
			return true;
		}

		private ClientMapping[] items;

		private struct MatchingParameters
		{
			public MatchingParameters(string application, string platform, ClientControl control, UserAgentParser.UserAgentVersion version)
			{
				this.Application = application;
				this.Platform = platform;
				this.Control = control;
				this.Version = version;
			}

			public string Application;

			public string Platform;

			public ClientControl Control;

			public UserAgentParser.UserAgentVersion Version;
		}

		internal enum ClientAttribute
		{
			First,
			Application = 0,
			Platform,
			Control,
			Last
		}
	}
}
