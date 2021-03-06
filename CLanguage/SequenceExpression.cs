using System;

namespace CLanguage
{
	public class SequenceExpression : Expression
	{
		public Expression First { get; set; }
		public Expression Second { get; set; }

		public SequenceExpression (Expression first, Expression second)
		{
			First = first;
			Second = second;
		}

		public override CType GetEvaluatedCType (EmitContext ec)
		{
			return Second.GetEvaluatedCType (ec);
		}

		protected override void DoEmit (EmitContext ec)
		{
			First.Emit (ec);
			ec.Emit (OpCode.Pop);
			Second.Emit (ec);
		}

		public override string ToString ()
		{
			return "(" + First + ", " + Second + ")";
		}
	}
}

