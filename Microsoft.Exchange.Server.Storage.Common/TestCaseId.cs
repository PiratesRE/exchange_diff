using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public struct TestCaseId : IComparable<TestCaseId>, IEquatable<TestCaseId>
	{
		private TestCaseId(int value)
		{
			this.value = value;
		}

		public static TestCaseId Null
		{
			get
			{
				return TestCaseId.nullId;
			}
		}

		public static TestCaseId ExpandedConversationViewTestCaseId
		{
			get
			{
				return TestCaseId.expandedConversationViewTestCaseId;
			}
		}

		public int Value
		{
			get
			{
				return this.value;
			}
		}

		public bool IsNull
		{
			get
			{
				return this.value == -1;
			}
		}

		public bool IsNotNull
		{
			get
			{
				return this.value != -1;
			}
		}

		public static bool operator ==(TestCaseId left, TestCaseId right)
		{
			return left.Value == right.Value;
		}

		public static bool operator !=(TestCaseId left, TestCaseId right)
		{
			return left.Value != right.Value;
		}

		public static explicit operator TestCaseId(int value)
		{
			return new TestCaseId(value);
		}

		public static TestCaseId GetInProcessTestCaseId()
		{
			if (!DefaultSettings.Get.EnableTestCaseIdLookup)
			{
				return TestCaseId.Null;
			}
			return TestCaseId.InternalGetTestCaseId();
		}

		public override int GetHashCode()
		{
			return this.value;
		}

		public override string ToString()
		{
			if (!this.IsNull)
			{
				return this.value.ToString();
			}
			return "Null";
		}

		public override bool Equals(object other)
		{
			return other is TestCaseId && this.Equals((TestCaseId)other);
		}

		public bool Equals(TestCaseId other)
		{
			return this.Value == other.Value;
		}

		public int CompareTo(TestCaseId other)
		{
			return this.Value - other.Value;
		}

		private static TestCaseId InternalGetTestCaseId()
		{
			int num;
			if (int.TryParse(Environment.GetEnvironmentVariable("PerseusActiveTestCaseTCMID", EnvironmentVariableTarget.Process), out num))
			{
				return new TestCaseId(num);
			}
			return TestCaseId.Null;
		}

		private const int NullTestCaseId = -1;

		private static readonly TestCaseId nullId = new TestCaseId(-1);

		private static readonly TestCaseId expandedConversationViewTestCaseId = new TestCaseId(380796);

		private int value;
	}
}
