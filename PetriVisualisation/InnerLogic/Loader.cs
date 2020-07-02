using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.Controls;
using ReactiveUI;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using PetriVisualisation.Parser_files;


namespace PetriVisualisation
{

    public class Loader
    {
        //TODO method should take path to file or later add raw text or file as it is
        //TODO remake into async
        public void MyParseMethod()
        {
            ICharStream stream = CharStreams.fromPath("/home/richard/Downloads/petrinet18-02-2020_10-20-31.dot"); //random file for testing
            ITokenSource lexer = new DOTLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            DOTParser parser = new DOTParser(tokens);
            parser.BuildParseTree = true;
            IParseTree tree = parser.graph();
            KeyPrinter printer = new KeyPrinter();
            ParseTreeWalker.Default.Walk(printer, tree);
            var whatever = 5;
        }
    }
}