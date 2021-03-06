﻿using System;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CLanguage.Tests
{
	[TestClass]
	public class ParserTests
	{
		TranslationUnit Parse (string code)
		{
			var report = new Report (new TestPrinter ());
			var pp = new Preprocessor (report);
			pp.AddCode ("stdin", code);
			var lexer = new Lexer (pp);
			var parser = new CParser ();
			return parser.ParseTranslationUnit (lexer, report);
		}

		[TestMethod]
		public void BadFunction ()
		{
			var failed = false;
			try {
				var tu = Parse (@"void setup()
{
pinMode (4, OUTPUT);
}

void loop()
{
pinMode
sleep(1000);
}");
			}
			catch {
				failed = true;
			}
			if (!failed) {
				Assert.Fail ("Shouldn't have compiled");
			}
		}

		[TestMethod]
		public void ForLoopWithThreeInits ()
		{
			var tu = Parse (@"
void f () {
	int acc;
	int i;
	int j;
	for (i = -10, acc = 0, j = 42; i <= 10; i += 2) {
		acc = acc + 1;
	}
	assertAreEqual (11, acc);
}");
			var f = tu.Functions.First (x => x.Name == "f");
			var forS = (ForStatement)f.Body.Statements[0];
			var init = forS.InitBlock;
			Assert.AreEqual (init.Statements.Count, 1);
			var expr = (SequenceExpression)((ExpressionStatement)init.Statements[0]).Expression;
			Assert.IsInstanceOfType (expr.First, typeof(SequenceExpression));
			var sexpr = (SequenceExpression)expr.First;
			Assert.AreEqual (((VariableExpression)((AssignExpression)sexpr.First).Left).VariableName, "i");
			Assert.AreEqual (((VariableExpression)((AssignExpression)sexpr.Second).Left).VariableName, "acc");
			Assert.IsInstanceOfType (expr.Second, typeof(AssignExpression));
			Assert.AreEqual (((VariableExpression)((AssignExpression)expr.Second).Left).VariableName, "j");
		}
	}
}

