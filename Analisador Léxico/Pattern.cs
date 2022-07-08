using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Analisador_Léxico
{
    public class Pattern
    {
        public string Lexema { get; set; }

        public string Token { get; set; }

        public int Ordem { get; set; }

        public static List<Pattern> patterns = new List<Pattern>();

        private static bool Numbers(string input)
        {
            Regex NumbersRegex = new Regex("\\d+(\\.\\d+)?");
            List<Pattern> lista = new List<Pattern>();

            lista = NumbersRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "NUM", Token = m.Value, Ordem = patterns.Count() + 1 })
                .ToList();

            if (lista.Count() > 0)
            {
                patterns.AddRange(lista);
                return true;
            }

            return false;
        }

        private static void Tokens(string input)
        {
            Regex WordsRegex = new Regex("[A-Za-z]+");
            Regex NumbersRegex = new Regex("\\d+(\\.\\d+)?");
            Regex DelimiterRegex = new Regex(@"[\\}\\{\\)\\(\n\f\r\t]+");
            Regex Espaco = new Regex("\\d+(\\.\\s+)?");
            List<Pattern> lista = new List<Pattern>();
            string extractText = "";
            string extractNumber = "";
            string word = "", number = "";
            int index = 0;
            int length = 1;
            string text;

            for(int i = 0; i < input.Length; i++)
            {
                var tamText = extractText != null ? extractText.Length : 0;
                if (index < input.Length)
                {
                    text = input.Substring(index, length);
                }
                else
                {
                    continue;
                }

                extractText = WordsRegex.Matches(text)
                .Cast<Match>()
                .Select(m => m.Value).FirstOrDefault();

                if (extractText != null)
                {
                    if (tamText < extractText.Length) {
                        if (!Desvio(extractText) && !Repetition(extractText))
                        {
                            word = extractText;

                        }
                        else
                        {
                            word = "";
                            index = i + 1;
                            length = 0;
                        }
                    }
                    else if (word.Length > 0)
                    {
                        patterns.Add(new Pattern { Lexema = "ID", Token = word, Ordem = patterns.Count() + 1 });
                        word = "";
                        index = i + 1;
                        length = 0;
                    }
                }
                if(text == "=")
                {
                    var auxText = input.Substring(index, length + 1);
                    if (auxText == "==")
                    {
                        Conditional(auxText);
                        index = i + 2;
                        length = 0;
                    }
                    else
                    {
                        Attribution(text);
                        index = i + 1;
                        length = 0;
                    }
                }
                if (Delimiter(text) || Operators(text) || Conditional(text))
                {
                    index = i + 1;
                    length = 0;
                }

                //Verifica se possui numero na entrada
                var tamNumber = extractNumber != null ? extractNumber.Length : 0;

               
                 extractNumber = NumbersRegex.Matches(text)
                                .Cast<Match>()
                                .Select(m => m.Value).FirstOrDefault();

                if (extractNumber != null)
                {
                    text = text.TrimStart();
                    if (!text.Contains(" ") && DelimiterRegex.Matches(text).Count <= 0)
                    {

                        number = extractNumber;

                    }
                    else if (number.Length > 0)
                    {
                        patterns.Add(new Pattern { Lexema = "NUM", Token = number, Ordem = patterns.Count() + 1 });
                        number = "";
                        index = i + 1;
                        length = 0;
                    }
                    
                }
                

                length++;
            }
        }


        private static bool Delimiter(string input)
        {
            Regex DelimiterRegex = new Regex(@"[\\}\\{\\)\\(\n\f\r\t]+");
            List<Pattern> lista = new List<Pattern>();

            lista = DelimiterRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "DELIM", Token = m.Value, Ordem = patterns.Count() + 1 })
                .ToList();

            if (lista.Count() > 0)
            {
                patterns.AddRange(lista);
                return true;
            }

            return false;
        }

        private static bool Attribution(string input)
        {
            Regex AttributionRegex = new Regex("\\=");
            var lista = new List<Pattern>();

            lista = AttributionRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "CMD_ATR", Token = m.Value, Ordem = patterns.Count() + 1 })
                .ToList();

            if (lista.Count() > 0)
            {
               patterns.AddRange(lista);
               return true;
            }

            return false;
        }

        private static bool Operators(string input)
        {
            Regex OperatorDIVRegex = new Regex("[\\/]+");
            Regex OperatorADDRegex = new Regex("[\\+]+");
            Regex OperatorSUBRegex = new Regex("[\\-]+");
            Regex OperatorMULRegex = new Regex("[\\*]+");

            List<Pattern> lista = new List<Pattern>();

            lista.AddRange(OperatorDIVRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "OP_DIV", Token = m.Value, Ordem = patterns.Count() + 1 })
                .ToList());

            lista.AddRange(OperatorADDRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "OP_ADD", Token = m.Value, Ordem = patterns.Count() + lista.Count() + 1 })
                .ToList());

            lista.AddRange(OperatorSUBRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "OP_SUB", Token = m.Value, Ordem = patterns.Count() + lista.Count() + 1 })
                .ToList());

            lista.AddRange(OperatorMULRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "OP_MUL", Token = m.Value, Ordem = patterns.Count() + lista.Count() + 1 })
                .ToList());

            if (lista.Count() > 0)
            {
                patterns.AddRange(lista);
                return true;
            }

            return false;
        }

        private static bool Conditional(string input)
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
                .Select(m => new Pattern { Lexema = "OP_EQ", Token = m.Value, Ordem = patterns.Count() + 1 })
                .ToList());

            lista.AddRange(ConditionalNotEqualRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "OP_NEQ", Token = m.Value, Ordem = patterns.Count() + lista.Count() + 1 })
                .ToList());

            lista.AddRange(ConditionalBiggerEqualRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "OP_BEQ", Token = m.Value, Ordem = patterns.Count() + lista.Count() + 1 })
                .ToList());

            lista.AddRange(ConditionalSmallerEqualRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "OP_SEQ", Token = m.Value, Ordem = patterns.Count() + lista.Count() + 1 })
                .ToList());

            lista.AddRange(ConditionalSmallerRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "OP_SM", Token = m.Value, Ordem = patterns.Count() + lista.Count() + 1 })
                .ToList());

            lista.AddRange(ConditionalBiggerRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "OP_BG", Token = m.Value, Ordem = patterns.Count() + lista.Count() + 1 })
                .ToList());

            lista.AddRange(ConditionalANDRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "OP_AND", Token = m.Value, Ordem = patterns.Count() + lista.Count() + 1 })
                .ToList());

            foreach (var item in ConditionalORRegex)
            {
                if (input.Contains(item))
                {
                    lista.Add(new Pattern { Lexema = "OP_OR", Token = item, Ordem = patterns.Count() + lista.Count() + 1 });
                    break;
                }
            }

            if (lista.Count() > 0)
            {
                patterns.AddRange(lista);
                return true;
            }

            return false;
        }

        private static bool Desvio(string input)
        {
            var DesvioIFRegex = new string[] { "if","IF","If"};

            var DesvioELSERegex = new string[] { "else","Else","ELSE" };

            List<Pattern> lista = new List<Pattern>();

            foreach (var item in DesvioIFRegex)
            {
                if (input.Contains(item)) {
                    patterns.Add(new Pattern { Lexema = "CMD_IF", Token = item, Ordem = patterns.Count() + 1 });
                    return true;
                }
            }


            foreach (var item in DesvioELSERegex)
            {
                if (input.Contains(item)) {
                    patterns.Add(new Pattern { Lexema = "CMD_ELSE", Token = item, Ordem = patterns.Count() + 1 });
                    return true;
                }
            }


            return false;
        }

        private static bool Repetition(string input)
        {
            var RepetitionWhileRegex = new string[] { "while", "WHILE", "While" };
            var RepetitionForRegex = new string[] { "for", "For", "FOR" };

            List<Pattern> lista = new List<Pattern>();

            foreach (var item in RepetitionForRegex)
            {
                if (input.Contains(item))
                {
                    patterns.Add(new Pattern { Lexema = "CMD_FOR", Token = item, Ordem = patterns.Count() + 1 });
                    return true;
                }
            }

            foreach (var item in RepetitionWhileRegex)
            {
                if (input.Contains(item))
                {
                    patterns.Add(new Pattern { Lexema = "CMD_WHILE", Token = item, Ordem = patterns.Count() + 1 });
                    return true;
                }
            }


            return false;
        }

        public static List<Pattern> AnalisadorLexico(string input)
        {
            List<Pattern> result = new List<Pattern>();

            patterns.Clear();
            //Gera os tokens
            Tokens(input);

            result.AddRange(patterns);
            //foreach (var item in result.Select(x => x.Token).ToArray())
            //{
            //    input = input.Replace(item, "");
            //}

            //result.AddRange(Numbers(input));
            
            //result.AddRange(Conditional(input));
            //foreach (var item in result.Select(x => x.Token).ToArray())
            //{
            //    input = input.Replace(item, "");
            //}
            //result.AddRange(Repetition(input));
            //foreach (var item in result.Select(x => x.Token).ToArray())
            //{
            //    input.Replace(item, "");
            //}
            //result.AddRange(Desvio(input));
            //foreach (var item in result.Select(x => x.Token).ToArray())
            //{
            //    input = input.Replace(item, "");
            //}
            
            //result.AddRange(Delimiter(input));
            //foreach (var item in result.Select(x => x.Token).ToArray())
            //{
            //    input = input.Replace(item, "");
            //}
            //result.AddRange(Attribution(input));
            //foreach (var item in result.Select(x => x.Token).ToArray())
            //{
            //    input = input.Replace(item, "");
            //}
            //result.AddRange(Operators(input));

            return result;
        }
    }
}