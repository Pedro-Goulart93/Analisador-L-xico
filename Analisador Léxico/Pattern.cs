using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Analisador_Léxico
{
    public class Pattern
    {
        public string Lexema { get; set; }

        public string Token { get; set; }
        
        private static List<Pattern> Numbers(string input)
        {
            Regex NumbersRegex = new Regex("\\d+(\\.\\d+)?");
            List<Pattern> lista = new List<Pattern>();

            lista = NumbersRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "NUM", Token = m.Value })
                .ToList();

            return lista;
        }

        private static List<Pattern> Words(string input)
        {
            Regex WordsRegex = new Regex("[A-Za-z]+");
            List<Pattern> lista = new List<Pattern>();

            lista = WordsRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "ID", Token = m.Value })
                .ToList();

            return lista;
        }

        private static List<Pattern> Delimiter(string input)
        {
            Regex DelimiterRegex = new Regex("[\\(\\)\\{\\}\' '\t\f\r\n]+");
            List<Pattern> lista = new List<Pattern>();

            lista = DelimiterRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "DELIM", Token = m.Value })
                .ToList();

            return lista;
        }

        private static List<Pattern> Attribution(string input)
            {
            Regex AttributionRegex = new Regex("\\=");
            List<Pattern> lista = new List<Pattern>();

            lista = AttributionRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "CMD_ATR", Token = m.Value })
                .ToList();

            return lista;
        }

        private static List<Pattern> Operators(string input)
        {
            Regex OperatorDIVRegex = new Regex("[\\/]+");
            Regex OperatorADDRegex = new Regex("[\\+]+");
            Regex OperatorSUBRegex = new Regex("[\\-]+");
            Regex OperatorMULRegex = new Regex("[\\*]+");

            List<Pattern> lista = new List<Pattern>();

            lista.AddRange(OperatorDIVRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "OP_DIV", Token = m.Value })
                .ToList());

            lista.AddRange(OperatorADDRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "OP_ADD", Token = m.Value })
                .ToList());

            lista.AddRange(OperatorSUBRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "OP_SUB", Token = m.Value })
                .ToList());

            lista.AddRange(OperatorMULRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "OP_MUL", Token = m.Value })
                .ToList());

            return lista;
        }

        private static List<Pattern> Conditional(string input)
        {
            Regex ConditionalEqualRegex = new Regex("\\==");
            Regex ConditionalNotEqualRegex = new Regex("\\<>");
            Regex ConditionalBiggerEqualRegex = new Regex("\\>=");
            Regex ConditionalSmallerEqualRegex = new Regex("\\<=");
            Regex ConditionalSmallerRegex = new Regex("\\<");
            Regex ConditionalBiggerRegex = new Regex("\\>");

            List<Pattern> lista = new List<Pattern>();

            lista.AddRange(ConditionalEqualRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "OP_EQ", Token = m.Value })
                .ToList());

            lista.AddRange(ConditionalNotEqualRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "OP_NEQ", Token = m.Value })
                .ToList());

            lista.AddRange(ConditionalBiggerEqualRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "OP_BEQ", Token = m.Value })
                .ToList());

            lista.AddRange(ConditionalSmallerEqualRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "OP_SEQ", Token = m.Value })
                .ToList());

            lista.AddRange(ConditionalSmallerRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "OP_SM", Token = m.Value })
                .ToList());

            lista.AddRange(ConditionalBiggerRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "OP_BG", Token = m.Value })
                .ToList());

            return lista;
        }

        private static List<Pattern> Desvio(string input)
        {
            Regex DesvioIFVRegex = new Regex("\bif\b");
            Regex DesvioELSERegex = new Regex("elseElseELSE");

            List<Pattern> lista = new List<Pattern>();

            lista.AddRange(DesvioIFVRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "CMD_IF", Token = m.Value })
                .ToList());

            lista.AddRange(DesvioELSERegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "CMD_ELSE", Token = m.Value })
                .ToList());

            

            return lista;
        }

        private static List<Pattern> Repetition(string input)
        {
            Regex RepetitionWhileRegex = new Regex("[WhileWHILEwhile]+");
            Regex RepetitionForRegex = new Regex("[ForforFOR]+");

            List<Pattern> lista = new List<Pattern>();

            lista.AddRange(RepetitionWhileRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "CMD_WHILE", Token = m.Value })
                .ToList());

            lista.AddRange(RepetitionForRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "CMD_FOR", Token = m.Value })
                .ToList());



            return lista;
        }

        public static List<Pattern> Analisador(string input)
        {
            List<Pattern> result = new List<Pattern>();

            result.AddRange(Numbers(input));
            result.AddRange(Conditional(input));
            //result.AddRange(Repetition(input));
            //result.AddRange(Desvio(input));
            result.AddRange(Words(input));
            result.AddRange(Delimiter(input));
            result.AddRange(Attribution(input));
            result.AddRange(Operators(input));

            return result;
        }
    }
}