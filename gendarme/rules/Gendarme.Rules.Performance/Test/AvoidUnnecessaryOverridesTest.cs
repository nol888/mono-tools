//
// Unit tests for AvoidUnnecessaryOverridesRule
//
// Authors:
//	N Lum <nol888@gmail.com
//
// Copyright (C) 2010 N Lum
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.Security.Permissions;

using Gendarme.Rules.Performance;

using NUnit.Framework;

using Test.Rules.Definitions;
using Test.Rules.Fixtures;

namespace Tests.Rules.Performance {

	[TestFixture]
	public class AvoidUnnecessaryOverridesTest : TypeRuleTestFixture <AvoidUnnecessaryOverridesRule> {

		private class TestBaseClass {
			public virtual string DoSomething(string s)
			{
				return s;
			}
		}

		private class TestClassGood : TestBaseClass {
			[STAThread]
			public override string DoSomething(string s)
			{
				return base.DoSomething (s);
			}
			[FileIOPermission (SecurityAction.Demand)]
			public override string ToString()
			{
				return base.ToString ();
			}
			public override bool Equals(object obj)
			{
				if (obj == null)
					return false;
				else
					return base.Equals (obj);
			}
		}

		private class TestClassBad : TestBaseClass {
			public override string ToString()
			{
				return base.ToString ();
			}
			public override string DoSomething(string s)
			{
				return base.DoSomething (s);
			}
		}

		private Mono.Cecil.TypeDefinition SimpleClassNoMethods;

		[SetUp]
		public void SetUp()
		{
			// Classes always have a constuctor so we hack our own methodless class.
			SimpleClassNoMethods = SimpleTypes.Class;
			SimpleClassNoMethods.Methods.Clear ();
		}

		[Test]
		public void Good()
		{
			AssertRuleSuccess<TestClassGood> ();
		}

		[Test]
		public void Bad()
		{
			AssertRuleFailure<TestClassBad> (2);
		}

		[Test]
		public void DoesNotApply()
		{
			AssertRuleDoesNotApply (SimpleClassNoMethods);
		}
	}
}
