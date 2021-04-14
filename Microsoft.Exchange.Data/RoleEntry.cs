using System;
using System.Collections.Generic;
using System.Management.Automation.Runspaces;
using System.Text;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics.Components.Authorization;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public abstract class RoleEntry
	{
		protected RoleEntry(string name, string[] parameters)
		{
			RoleEntry.ValidateName(name);
			this.SetParameters(parameters, true);
			this.name = name;
		}

		protected RoleEntry()
		{
		}

		public static RoleEntry Parse(string input)
		{
			if (input == null)
			{
				throw new ArgumentNullException("input");
			}
			if (input.Length < 3)
			{
				throw new FormatException(DataStrings.RoleEntryNameTooShort);
			}
			if (input[1] != ',')
			{
				throw new FormatException(DataStrings.RoleEntryStringMustBeCommaSeparated);
			}
			RoleEntry roleEntry = null;
			if (RoleEntry.roleEntryCache.TryGetValue(input, out roleEntry))
			{
				return roleEntry;
			}
			char c = input[0];
			switch (c)
			{
			case 'a':
				roleEntry = new ApplicationPermissionRoleEntry(input);
				goto IL_A9;
			case 'b':
				break;
			case 'c':
				roleEntry = new CmdletRoleEntry(input);
				goto IL_A9;
			default:
				if (c == 's')
				{
					roleEntry = new ScriptRoleEntry(input);
					goto IL_A9;
				}
				if (c == 'w')
				{
					roleEntry = new WebServiceRoleEntry(input);
					goto IL_A9;
				}
				break;
			}
			roleEntry = new UnknownRoleEntry(input);
			IL_A9:
			RoleEntry.roleEntryCache.Add(input, roleEntry);
			return roleEntry;
		}

		internal SessionStateCommandEntry CachedIssEntry
		{
			get
			{
				return this.sessionStateCommandEntry;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("CachedIssEntry");
				}
				this.sessionStateCommandEntry = value;
			}
		}

		internal int CachedIssEntryParameterCount
		{
			get
			{
				return this.cachedIssEntryParameterCount;
			}
			set
			{
				this.cachedIssEntryParameterCount = value;
			}
		}

		protected int ExtractAndSetName(string entryString)
		{
			int num = 2;
			int num2 = entryString.IndexOf(',', num);
			int num3 = ((num2 < 0) ? entryString.Length : num2) - num;
			if (num3 < 1)
			{
				throw new FormatException(DataStrings.RoleEntryNameTooShort);
			}
			string text = entryString.Substring(num, num3);
			RoleEntry.ValidateName(text);
			this.name = text;
			return num2 + 1;
		}

		protected void ExtractAndSetParameters(string entryString, int paramIndex)
		{
			if (paramIndex <= 0)
			{
				this.parameterCollection = RoleEntry.noParameters;
				this.parameters = RoleEntry.emptyArray;
				return;
			}
			string text = entryString.Substring(paramIndex);
			string[] value = text.Split(new char[]
			{
				','
			});
			this.SetParameters(value, true);
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public ICollection<string> Parameters
		{
			get
			{
				return this.parameterCollection;
			}
		}

		internal static void FormatParameters(string[] parameters)
		{
			if (parameters == null)
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < parameters.Length; i++)
			{
				if (parameters[i].Length < 1)
				{
					throw new FormatException(DataStrings.ParameterNameEmptyException);
				}
				if (RoleEntry.ContainsInvalidChars(parameters[i]))
				{
					throw new FormatException(DataStrings.ParameterNameInvalidCharException(parameters[i]));
				}
				if (!flag && i < parameters.Length - 1 && string.Compare(parameters[i], parameters[i + 1], StringComparison.OrdinalIgnoreCase) >= 0)
				{
					flag = true;
				}
			}
			if (flag)
			{
				Array.Sort<string>(parameters, StringComparer.OrdinalIgnoreCase);
				for (int j = 0; j < parameters.Length - 1; j++)
				{
					if (string.Equals(parameters[j], parameters[j + 1], StringComparison.OrdinalIgnoreCase))
					{
						throw new FormatException(DataStrings.DuplicateParameterException(parameters[j]));
					}
				}
			}
		}

		protected void SetParameters(ICollection<string> value, bool performValidation)
		{
			if (value == null)
			{
				this.parameterCollection = RoleEntry.noParameters;
				this.parameters = RoleEntry.emptyArray;
				return;
			}
			string[] array = new string[value.Count];
			value.CopyTo(array, 0);
			if (performValidation)
			{
				RoleEntry.FormatParameters(array);
			}
			this.parameters = array;
			this.parameterCollection = new ReadOnlyCollection<string>(array);
		}

		internal static RoleEntry MergeParameters(IList<RoleEntry> entries)
		{
			if (entries == null)
			{
				throw new ArgumentNullException("entries");
			}
			if (entries.Count == 0)
			{
				throw new ArgumentException("entries");
			}
			if (entries.Count == 1)
			{
				return entries[0];
			}
			int num = 0;
			int count = entries[0].Parameters.Count;
			int index = 0;
			string b = entries[0].Name;
			CmdletRoleEntry cmdletRoleEntry = entries[0] as CmdletRoleEntry;
			string text = (cmdletRoleEntry == null) ? null : cmdletRoleEntry.PSSnapinName;
			int[] array = new int[entries.Count];
			for (int i = 0; i < entries.Count; i++)
			{
				RoleEntry roleEntry = entries[i];
				if (i > 0 && object.ReferenceEquals(roleEntry, entries[i - 1]))
				{
					array[i] = -1;
				}
				else
				{
					if (!string.Equals(roleEntry.Name, b, StringComparison.OrdinalIgnoreCase))
					{
						throw new ArgumentException("entries");
					}
					if (text != null)
					{
						CmdletRoleEntry cmdletRoleEntry2 = roleEntry as CmdletRoleEntry;
						if (roleEntry == null)
						{
							throw new ArgumentException("entries");
						}
						if (!string.Equals(cmdletRoleEntry2.PSSnapinName, text, StringComparison.OrdinalIgnoreCase))
						{
							throw new ArgumentException("entries");
						}
					}
					else if (roleEntry == null)
					{
						throw new ArgumentException("entries");
					}
					if (count < roleEntry.Parameters.Count)
					{
						count = roleEntry.Parameters.Count;
						index = i;
					}
					num += roleEntry.Parameters.Count;
					if (roleEntry.Parameters.Count == 0)
					{
						array[i] = -1;
					}
				}
			}
			if (num == entries[0].Parameters.Count)
			{
				return entries[0];
			}
			List<string> list = new List<string>(num);
			string b2 = string.Empty;
			string text2 = null;
			for (;;)
			{
				for (int j = 0; j < entries.Count; j++)
				{
					if (array[j] != -1)
					{
						string text3 = entries[j].parameters[array[j]];
						if (string.Equals(text3, b2, StringComparison.OrdinalIgnoreCase))
						{
							int num2 = ++array[j];
							if (num2 >= entries[j].parameters.Length)
							{
								array[j] = -1;
								goto IL_242;
							}
							text3 = entries[j].parameters[array[j]];
						}
						if (text2 == null || string.Compare(text3, text2, StringComparison.OrdinalIgnoreCase) < 0)
						{
							text2 = text3;
						}
					}
					IL_242:;
				}
				if (text2 == null)
				{
					break;
				}
				list.Add(text2);
				b2 = text2;
				text2 = null;
			}
			if (list.Count == count)
			{
				return entries[index];
			}
			RoleEntry roleEntry2 = entries[0].Clone(list, false, null);
			string token = roleEntry2.ToADString();
			RoleEntry result = null;
			if (RoleEntry.roleEntryCache.TryGetValue(token, out result))
			{
				return result;
			}
			RoleEntry.roleEntryCache.Add(token, roleEntry2);
			return roleEntry2;
		}

		internal RoleEntry IntersectParameters(RoleEntry entry)
		{
			List<string> list = null;
			bool flag = false;
			int i = 0;
			for (int j = 0; j < this.Parameters.Count; j++)
			{
				string text = this.parameters[j];
				int num = 1;
				while (i < entry.Parameters.Count)
				{
					num = string.Compare(text, entry.parameters[i], StringComparison.OrdinalIgnoreCase);
					if (num >= 0)
					{
						i++;
					}
					if (num <= 0)
					{
						break;
					}
				}
				if (num == 0)
				{
					if (flag)
					{
						list.Add(text);
					}
				}
				else if (!flag)
				{
					flag = true;
					list = new List<string>(this.Parameters.Count);
					for (int k = 0; k < j; k++)
					{
						list.Add(this.parameters[k]);
					}
				}
			}
			if (!flag)
			{
				return this;
			}
			CmdletRoleEntry cmdletRoleEntry = this as CmdletRoleEntry;
			if (cmdletRoleEntry != null)
			{
				return new CmdletRoleEntry(cmdletRoleEntry.Name, cmdletRoleEntry.PSSnapinName, list.ToArray());
			}
			throw new NotSupportedException("Parameter intersection for RoleEntry other than CmdletRoleEntry is not supported.");
		}

		internal bool ContainsAllParameters(IList<string> parametersToCheck)
		{
			if (parametersToCheck == null)
			{
				throw new ArgumentNullException("parametersToCheck");
			}
			for (int i = 0; i < parametersToCheck.Count; i++)
			{
				if (!this.ContainsParameter(parametersToCheck[i]))
				{
					ExTraceGlobals.AccessDeniedTracer.TraceError<string, string>(0L, "Role entry {0} does not contain parameter {1}", this.Name, parametersToCheck[i]);
					return false;
				}
			}
			return true;
		}

		internal bool ContainsAllParametersFromRoleEntry(RoleEntry roleEntryToCheck, out ICollection<string> missingParameters)
		{
			missingParameters = null;
			if (roleEntryToCheck == null)
			{
				throw new ArgumentNullException("roleEntryToCheck");
			}
			for (int i = 0; i < roleEntryToCheck.parameters.Length; i++)
			{
				if (!this.ContainsParameter(roleEntryToCheck.parameters[i]))
				{
					missingParameters = (missingParameters ?? new List<string>());
					missingParameters.Add(roleEntryToCheck.parameters[i]);
				}
			}
			return missingParameters == null;
		}

		internal bool ContainsAnyParameter(IList<string> parametersToCheck)
		{
			if (parametersToCheck == null)
			{
				throw new ArgumentNullException("parametersToCheck");
			}
			for (int i = 0; i < parametersToCheck.Count; i++)
			{
				if (this.ContainsParameter(parametersToCheck[i]))
				{
					return true;
				}
			}
			ExTraceGlobals.AccessDeniedTracer.TraceError<string, int>(0L, "Role entry {0} does not contain any parameters from the specified list of {1}", this.Name, parametersToCheck.Count);
			return false;
		}

		internal bool ContainsParameter(string parameterToCheck)
		{
			return Array.BinarySearch<string>(this.parameters, parameterToCheck, StringComparer.OrdinalIgnoreCase) >= 0;
		}

		internal bool TryMatchRoleEntryToArrayAndRemoveKnownParameters(RoleEntry[] availableEntries, out RoleEntry filteredEntry)
		{
			int num = Array.BinarySearch<RoleEntry>(availableEntries, this, RoleEntry.NameComparer);
			if (num < 0)
			{
				filteredEntry = this;
				return false;
			}
			new List<string>(this.Parameters);
			RoleEntry roleEntry = availableEntries[num];
			ICollection<string> collection;
			if (roleEntry.ContainsAllParametersFromRoleEntry(this, out collection))
			{
				filteredEntry = null;
			}
			else
			{
				filteredEntry = this.Clone(collection);
			}
			return true;
		}

		internal RoleEntry FindAndRemoveMatchingParameters(RoleEntry[] availableEntries)
		{
			int num = Array.BinarySearch<RoleEntry>(availableEntries, this, RoleEntry.NameComparer);
			if (num < 0)
			{
				return this;
			}
			new List<string>(this.Parameters);
			RoleEntry roleEntry = availableEntries[num];
			ICollection<string> collection;
			if (roleEntry.ContainsAllParametersFromRoleEntry(this, out collection))
			{
				return null;
			}
			return this.Clone(collection);
		}

		internal RoleEntry FindAndIntersectWithMatchingParameters(RoleEntry[] availableEntries)
		{
			int num = Array.BinarySearch<RoleEntry>(availableEntries, this, RoleEntry.NameComparer);
			if (num < 0)
			{
				return null;
			}
			new List<string>(this.Parameters);
			RoleEntry entry = availableEntries[num];
			return this.IntersectParameters(entry);
		}

		protected bool Equals(RoleEntry other)
		{
			if (other == null)
			{
				return false;
			}
			if (!this.Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase) || this.Parameters.Count != other.Parameters.Count)
			{
				return false;
			}
			for (int i = 0; i < this.parameters.Length; i++)
			{
				if (!this.parameters[i].Equals(other.parameters[i], StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
			}
			return true;
		}

		public static bool operator ==(RoleEntry left, RoleEntry right)
		{
			return object.Equals(left, right);
		}

		public static bool operator !=(RoleEntry left, RoleEntry right)
		{
			return !(left == right);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as RoleEntry);
		}

		public override int GetHashCode()
		{
			return this.Name.GetHashCode();
		}

		internal int GetInstanceHashCode()
		{
			return base.GetHashCode();
		}

		internal static int CompareRoleEntriesByName(RoleEntry a, RoleEntry b)
		{
			CmdletRoleEntry cmdletRoleEntry = a as CmdletRoleEntry;
			CmdletRoleEntry cmdletRoleEntry2 = b as CmdletRoleEntry;
			if (a == null || b == null)
			{
				if (a == null && b == null)
				{
					return 0;
				}
				if (!(a == null))
				{
					return 1;
				}
				return -1;
			}
			else if (cmdletRoleEntry == null != (cmdletRoleEntry2 == null))
			{
				if (!(cmdletRoleEntry == null))
				{
					return 1;
				}
				return -1;
			}
			else
			{
				if (cmdletRoleEntry != null)
				{
					return string.Compare(cmdletRoleEntry.FullName, cmdletRoleEntry2.FullName, StringComparison.OrdinalIgnoreCase);
				}
				return string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase);
			}
		}

		internal static int CompareRoleEntriesByNameAndInstanceHashCode(RoleEntry a, RoleEntry b)
		{
			int num = RoleEntry.CompareRoleEntriesByName(a, b);
			if (num != 0 || (a == null && b == null))
			{
				return num;
			}
			return a.GetInstanceHashCode().CompareTo(b.GetInstanceHashCode());
		}

		protected string ToString(string snapin)
		{
			StringBuilder stringBuilder = new StringBuilder(128);
			if (snapin != null)
			{
				stringBuilder.Append("(");
				stringBuilder.Append(snapin);
				stringBuilder.Append(") ");
			}
			stringBuilder.Append(this.Name);
			foreach (string value in this.Parameters)
			{
				stringBuilder.Append(" -");
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}

		public override string ToString()
		{
			return this.ToString(null);
		}

		protected string ToADString(char typeHint, string snapin, string versionInfo)
		{
			StringBuilder stringBuilder = new StringBuilder(128);
			stringBuilder.Append(typeHint);
			stringBuilder.Append(",");
			stringBuilder.Append(this.Name);
			if (versionInfo != null)
			{
				stringBuilder.Append(",");
				stringBuilder.Append(versionInfo);
			}
			if (snapin != null)
			{
				stringBuilder.Append(",");
				stringBuilder.Append(snapin);
			}
			foreach (string value in this.Parameters)
			{
				stringBuilder.Append(",");
				stringBuilder.Append(value);
			}
			return stringBuilder.ToString();
		}

		public abstract string ToADString();

		internal RoleEntry Clone(ICollection<string> parameters)
		{
			return this.Clone(parameters, true, null);
		}

		internal RoleEntry Clone(ICollection<string> parameters, string newName)
		{
			return this.Clone(parameters, true, newName);
		}

		private RoleEntry Clone(ICollection<string> parameters, bool performValidation, string newName = null)
		{
			RoleEntry roleEntry;
			if (this is CmdletRoleEntry)
			{
				roleEntry = new CmdletRoleEntry((CmdletRoleEntry)this, newName);
			}
			else if (this is ScriptRoleEntry)
			{
				roleEntry = new ScriptRoleEntry();
			}
			else if (this is ApplicationPermissionRoleEntry)
			{
				roleEntry = new ApplicationPermissionRoleEntry();
			}
			else if (this is WebServiceRoleEntry)
			{
				roleEntry = new WebServiceRoleEntry();
			}
			else
			{
				roleEntry = new UnknownRoleEntry();
			}
			if (string.IsNullOrWhiteSpace(newName))
			{
				roleEntry.name = this.Name;
			}
			else
			{
				roleEntry.name = newName;
			}
			roleEntry.SetParameters(parameters, performValidation);
			return roleEntry;
		}

		protected static bool ContainsInvalidChars(string valueToCheck)
		{
			return RoleEntry.ContainsInvalidChars(valueToCheck, 0, valueToCheck.Length);
		}

		protected static bool ContainsInvalidChars(string valueToCheck, int startIndex, int count)
		{
			return -1 != valueToCheck.IndexOfAny(RoleEntry.invalidCharacters, startIndex, count);
		}

		internal static void ValidateName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new FormatException(DataStrings.RoleEntryNameInvalidException(name ?? string.Empty));
			}
			if (RoleEntry.ContainsInvalidChars(name))
			{
				throw new FormatException(DataStrings.RoleEntryNameInvalidException(name));
			}
		}

		internal RoleEntry MapToPreviousVersion()
		{
			return this;
		}

		internal string MapParameterToPreviousVersion(string newParameter)
		{
			return newParameter;
		}

		internal const char Separator = ',';

		internal const char VersionPrefix = 'v';

		internal const string SeparatorString = ",";

		private static ReadOnlyCollection<string> noParameters = new ReadOnlyCollection<string>(new string[0]);

		private static string[] emptyArray = new string[0];

		private static MruDictionaryCache<string, RoleEntry> roleEntryCache = new MruDictionaryCache<string, RoleEntry>(5000, 1440);

		private string name;

		private ReadOnlyCollection<string> parameterCollection;

		private string[] parameters;

		[NonSerialized]
		private SessionStateCommandEntry sessionStateCommandEntry;

		private int cachedIssEntryParameterCount;

		private static readonly char[] invalidCharacters = new char[]
		{
			','
		};

		internal static readonly IComparer<RoleEntry> NameComparer = new RoleEntry.NameRoleEntryComparer();

		private class NameRoleEntryComparer : IComparer<RoleEntry>
		{
			public int Compare(RoleEntry a, RoleEntry b)
			{
				return RoleEntry.CompareRoleEntriesByName(a, b);
			}
		}
	}
}
