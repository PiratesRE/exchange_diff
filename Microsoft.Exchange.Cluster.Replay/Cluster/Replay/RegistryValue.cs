using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class RegistryValue
	{
		public RegistryValue()
		{
			this.name = null;
			this.value = null;
			this.kind = RegistryValueKind.Unknown;
		}

		public RegistryValue(string name, object value, RegistryValueKind kind)
		{
			this.name = name;
			this.value = value;
			this.kind = kind;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public RegistryValueKind Kind
		{
			get
			{
				return this.kind;
			}
		}

		public object Value
		{
			get
			{
				return this.value;
			}
		}

		public bool Equals(RegistryValue other)
		{
			int num = 0;
			bool result = true;
			if (other == null || other.Value == null)
			{
				result = false;
			}
			else
			{
				if (this.Kind == other.Kind && string.Compare(this.Name, other.Name, StringComparison.OrdinalIgnoreCase) == 0)
				{
					switch (this.Kind)
					{
					case RegistryValueKind.String:
					case RegistryValueKind.ExpandString:
						return SharedHelper.StringIEquals((string)this.Value, (string)other.Value);
					case RegistryValueKind.Binary:
					{
						byte[] array = (byte[])other.Value;
						byte[] array2 = (byte[])this.Value;
						if (array.Length == array2.Length)
						{
							foreach (byte b in array2)
							{
								if (b != array[num])
								{
									result = false;
									break;
								}
								num++;
							}
							return result;
						}
						return false;
					}
					case RegistryValueKind.DWord:
						return (int)this.Value == (int)other.Value;
					case RegistryValueKind.MultiString:
					{
						string[] array4 = (string[])other.Value;
						string[] array5 = (string[])this.Value;
						if (array4.Length == array5.Length)
						{
							foreach (string str in array5)
							{
								if (!SharedHelper.StringIEquals(str, array4[num]))
								{
									result = false;
									break;
								}
								num++;
							}
							return result;
						}
						return false;
					}
					case RegistryValueKind.QWord:
						return (long)this.Value == (long)other.Value;
					}
					throw new NotImplementedException();
				}
				result = false;
			}
			return result;
		}

		private string name;

		private RegistryValueKind kind;

		private object value;
	}
}
