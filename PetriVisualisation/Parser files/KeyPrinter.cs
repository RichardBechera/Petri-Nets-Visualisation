using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace PetriVisualisation.Parser_files
{
    public class KeyPrinter: DOTBaseListener
    {
        //! Those methods are entering and leaving rules, use it somehow ?! 
        public override void VisitErrorNode(IErrorNode node)
        {
            base.VisitErrorNode(node);
        }

        public override void VisitTerminal(ITerminalNode node)
        {
            base.VisitTerminal(node);
        }

        public override void EnterGraph(DOTParser.GraphContext context)
        {
            base.EnterGraph(context);
        }

        public override void ExitGraph(DOTParser.GraphContext context)
        {
            base.ExitGraph(context);
        }

        public override void EnterEveryRule(ParserRuleContext context)
        {
            base.EnterEveryRule(context);
        }

        public override void ExitEveryRule(ParserRuleContext context)
        {
            base.ExitEveryRule(context);
        }

        public override void EnterStmt_list(DOTParser.Stmt_listContext context)
        {
            base.EnterStmt_list(context);
        }

        public override void ExitStmt_list(DOTParser.Stmt_listContext context)
        {
            base.ExitStmt_list(context);
        }

        public override void EnterStmt(DOTParser.StmtContext context)
        {
            base.EnterStmt(context);
        }

        public override void ExitStmt(DOTParser.StmtContext context)
        {
            base.ExitStmt(context);
        }

        public override void EnterAttr_stmt(DOTParser.Attr_stmtContext context)
        {
            base.EnterAttr_stmt(context);
        }

        public override void ExitAttr_stmt(DOTParser.Attr_stmtContext context)
        {
            base.ExitAttr_stmt(context);
        }

        public override void EnterAttr_list(DOTParser.Attr_listContext context)
        {
            base.EnterAttr_list(context);
        }

        public override void ExitAttr_list(DOTParser.Attr_listContext context)
        {
            base.ExitAttr_list(context);
        }

        public override void EnterA_list(DOTParser.A_listContext context)
        {
            base.EnterA_list(context);
        }

        public override void ExitA_list(DOTParser.A_listContext context)
        {
            base.ExitA_list(context);
        }

        public override void EnterEdge_stmt(DOTParser.Edge_stmtContext context)
        {
            base.EnterEdge_stmt(context);
        }

        public override void ExitEdge_stmt(DOTParser.Edge_stmtContext context)
        {
            base.ExitEdge_stmt(context);
        }

        public override void EnterEdgeRHS(DOTParser.EdgeRHSContext context)
        {
            base.EnterEdgeRHS(context);
        }

        public override void ExitEdgeRHS(DOTParser.EdgeRHSContext context)
        {
            base.ExitEdgeRHS(context);
        }

        public override void EnterEdgeop(DOTParser.EdgeopContext context)
        {
            base.EnterEdgeop(context);
        }

        public override void ExitEdgeop(DOTParser.EdgeopContext context)
        {
            base.ExitEdgeop(context);
        }

        public override void EnterNode_stmt(DOTParser.Node_stmtContext context)
        {
            base.EnterNode_stmt(context);
        }

        public override void ExitNode_stmt(DOTParser.Node_stmtContext context)
        {
            base.ExitNode_stmt(context);
        }

        public override void EnterNode_id(DOTParser.Node_idContext context)
        {
            base.EnterNode_id(context);
        }

        public override void ExitNode_id(DOTParser.Node_idContext context)
        {
            base.ExitNode_id(context);
        }

        public override void EnterSubgraph(DOTParser.SubgraphContext context)
        {
            base.EnterSubgraph(context);
        }

        public override void ExitSubgraph(DOTParser.SubgraphContext context)
        {
            base.ExitSubgraph(context);
        }

        public override void EnterId(DOTParser.IdContext context)
        {
            base.EnterId(context);
        }

        public override void ExitId(DOTParser.IdContext context)
        {
            base.ExitId(context);
        }
    }
}