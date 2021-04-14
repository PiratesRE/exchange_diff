using System;

namespace System.Runtime.Remoting.Activation
{
	internal class ActivationAttributeStack
	{
		internal ActivationAttributeStack()
		{
			this.activationTypes = new object[4];
			this.activationAttributes = new object[4];
			this.freeIndex = 0;
		}

		internal void Push(Type typ, object[] attr)
		{
			if (this.freeIndex == this.activationTypes.Length)
			{
				object[] destinationArray = new object[this.activationTypes.Length * 2];
				object[] destinationArray2 = new object[this.activationAttributes.Length * 2];
				Array.Copy(this.activationTypes, destinationArray, this.activationTypes.Length);
				Array.Copy(this.activationAttributes, destinationArray2, this.activationAttributes.Length);
				this.activationTypes = destinationArray;
				this.activationAttributes = destinationArray2;
			}
			this.activationTypes[this.freeIndex] = typ;
			this.activationAttributes[this.freeIndex] = attr;
			this.freeIndex++;
		}

		internal object[] Peek(Type typ)
		{
			if (this.freeIndex == 0 || this.activationTypes[this.freeIndex - 1] != typ)
			{
				return null;
			}
			return (object[])this.activationAttributes[this.freeIndex - 1];
		}

		internal void Pop(Type typ)
		{
			if (this.freeIndex != 0 && this.activationTypes[this.freeIndex - 1] == typ)
			{
				this.freeIndex--;
				this.activationTypes[this.freeIndex] = null;
				this.activationAttributes[this.freeIndex] = null;
			}
		}

		private object[] activationTypes;

		private object[] activationAttributes;

		private int freeIndex;
	}
}
