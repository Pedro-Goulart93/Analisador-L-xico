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
            Regex DelimiterRegex = new Regex(@"[\\}\\{\\)\\(\n\f\r\t]+");
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
            Regex ConditionalSmallerRegex = new Regex("\\<[^=]");
            Regex ConditionalBiggerRegex = new Regex("\\>[^=]");

            var ConditionalORRegex = new string[] { "||" };
            Regex ConditionalANDRegex = new Regex("\\&&");

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

            lista.AddRange(ConditionalANDRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "OP_AND", Token = m.Value })
                .ToList());

            foreach (var item in ConditionalORRegex)
            {
                if (input.Contains(item))
                {
                    lista.Add(new Pattern { Lexema = "OP_OR", Token = item });
                    break;
                }
            }

            return lista;
        }

        private static List<Pattern> Desvio(string input)
        {
            var DesvioIFRegex = new string[] { "if","IF","If"};

            var DesvioELSERegex = new string[] { "else","Else","ELSE" };

            List<Pattern> lista = new List<Pattern>();

            foreach (var item in DesvioIFRegex)
            {
                if (input.Contains(item)) {
                    lista.Add(new Pattern { Lexema = "CMD_IF", Token = item });
                    break;
                }
            }


            foreach (var item in DesvioELSERegex)
            {
                if (input.Contains(item)) { 
                    lista.Add(new Pattern { Lexema = "CMD_ELSE", Token = item });
                    break;
                }
            }


            return lista;
        }

        private static List<Pattern> Repetition(string input)
        {
            var RepetitionWhileRegex = new string[] { "while", "WHILE", "While" };
            var RepetitionForRegex = new string[] { "for", "For", "FOR" };

            List<Pattern> lista = new List<Pattern>();

            foreach (var item in RepetitionForRegex)
            {
                if (input.Contains(item))
                {
                    lista.Add(new Pattern { Lexema = "CMD_FOR", Token = item });
                    break;
                }
            }

            foreach (var item in RepetitionWhileRegex)
            {
                if (input.Contains(item))
                {
                    lista.Add(new Pattern { Lexema = "CMD_WHILE", Token = item });
                    break;
                }
            }


            return lista;
        }

        public static List<Pattern> Analisador(string input)
        {
            List<Pattern> result = new List<Pattern>();

            result.AddRange(Numbers(input));
            
            result.AddRange(Conditional(input));
            foreach (var item in result.Select(x => x.Token).ToArray())
            {
                input = input.Replace(item, "");
            }
            result.AddRange(Repetition(input));
            foreach (var item in result.Select(x => x.Token).ToArray())
            {
                input.Replace(item, "");
            }
            result.AddRange(Desvio(input));
            foreach (var item in result.Select(x => x.Token).ToArray())
            {
                input = input.Replace(item, "");
            }
            result.AddRange(Words(input));
            foreach (var item in result.Select(x => x.Token).ToArray())
            {
                input = input.Replace(item, "");
            }
            result.AddRange(Delimiter(input));
            foreach (var item in result.Select(x => x.Token).ToArray())
            {
                input = input.Replace(item, "");
            }
            result.AddRange(Attribution(input));
            foreach (var item in result.Select(x => x.Token).ToArray())
            {
                input = input.Replace(item, "");
            }
            result.AddRange(Operators(input));

            return result;
        }
    }
}