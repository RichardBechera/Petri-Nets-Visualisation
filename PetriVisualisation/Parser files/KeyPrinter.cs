using System;
using System.Collections.Generic;
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
            var text = node.GetText();
            var ignoreTerminals = new List<string>
                {"}", "{", "subgraph", "--", "->", "[", "]", "=", ";", ","};
            if (!ignoreTerminals.Contains(text))
                OnTerminalFound(node.GetText());
            base.VisitTerminal(node);
        }

        public override void EnterGraph(DOTParser.GraphContext context)
        {
            OnRuleEnter(Rule.Graph, context.GetText());
            base.EnterGraph(context);
        }

        public override void ExitGraph(DOTParser.GraphContext context)
        {
            OnRuleLeave(Rule.Graph, context.GetText());
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
            OnRuleEnter(Rule.AttrStmt, context.GetText());
            base.EnterAttr_stmt(context);
        }

        public override void ExitAttr_stmt(DOTParser.Attr_stmtContext context)
        {
            OnRuleLeave(Rule.AttrStmt, context.GetText());
            base.ExitAttr_stmt(context);
        }

        public override void EnterAttr_list(DOTParser.Attr_listContext context)
        {
            OnRuleEnter(Rule.AttrList, context.GetText());
            base.EnterAttr_list(context);
        }

        public override void ExitAttr_list(DOTParser.Attr_listContext context)
        {
            base.ExitAttr_list(context);
        }

        public override void EnterA_list(DOTParser.A_listContext context)
        {
            OnRuleEnter(Rule.Alist, context.GetText());
            base.EnterA_list(context);
        }

        public override void ExitA_list(DOTParser.A_listContext context)
        {
            base.ExitA_list(context);
        }

        public override void EnterEdge_stmt(DOTParser.Edge_stmtContext context)
        {
            OnRuleEnter(Rule.EdgeStmt, context.GetText());
            base.EnterEdge_stmt(context);
        }

        public override void ExitEdge_stmt(DOTParser.Edge_stmtContext context)
        {
            base.ExitEdge_stmt(context);
        }

        public override void EnterEdgeRHS(DOTParser.EdgeRHSContext context)
        {
            OnRuleEnter(Rule.EdgeRhs, context.GetText());
            base.EnterEdgeRHS(context);
        }

        public override void ExitEdgeRHS(DOTParser.EdgeRHSContext context)
        {
            base.ExitEdgeRHS(context);
        }

        public override void EnterNode_stmt(DOTParser.Node_stmtContext context)
        {
            OnRuleEnter(Rule.Node, context.GetText());
            base.EnterNode_stmt(context);
        }

        public override void ExitNode_stmt(DOTParser.Node_stmtContext context)
        {
            OnRuleLeave(Rule.Node, context.GetText());
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
            OnRuleEnter(Rule.Subgraph, context.GetText());
            base.EnterSubgraph(context);
        }

        public override void ExitSubgraph(DOTParser.SubgraphContext context)
        {
            OnRuleLeave(Rule.Subgraph, context.GetText());
            base.ExitSubgraph(context);
        }

        public override void EnterId(DOTParser.IdContext context)
        {
            OnRuleEnter(Rule.Id, context.GetText());
            base.EnterId(context);
        }

        public override void ExitId(DOTParser.IdContext context)
        {
            base.ExitId(context);
        }
        
        protected virtual void OnRuleEnter(Rule rule, string context)
        {
            RuleMoved?.Invoke(this, new MoveRuleEventArgs()
            {
                Rule = rule, Context = context, Enter = Enter.Enter
            });
        }
        
        protected virtual void OnRuleLeave(Rule rule, string context)
        {
            RuleMoved?.Invoke(this, new MoveRuleEventArgs()
            {
                Rule = rule, Context = context, Enter = Enter.Leave
            });
        }
        
        protected virtual void OnTerminalFound(string contains)
        {
            Terminal?.Invoke(this, new TerminalEventArgs()
            {
                Contains = contains
            });
        }
        
        public event EventHandler<MoveRuleEventArgs> RuleMoved;
        public event EventHandler<TerminalEventArgs> Terminal;
    }
}