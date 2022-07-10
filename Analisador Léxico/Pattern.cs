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

        public bool Aceito { get; set; }

        public string Erro { get; set; }

        public static List<Pattern> patterns = new List<Pattern>();

        private static void Validador(List<Pattern> input)
        {
            string[] delimDiscard = { "New Line", "TAB", "Form Feed"};
            string[] delimValid = { "(", "{" };

            var listLexema = input.Where(x => !delimDiscard.Contains(x.Token)).OrderBy(x => x.Ordem).ToList();
            for (int i = 0; i < listLexema.Count(); i++)
            {
                switch (listLexema[i].Lexema)
                {
                    case "ID":
                        if(i == 0)
                        {
                            listLexema[i].Aceito = true;
                        }
                        else if (delimValid.Contains(listLexema[i -1].Token) || listLexema[i - 1].Token == "=" 
                                || listLexema[i - 1].Token.Contains("OP") )
                        {
                            listLexema[i].Aceito = true;
                        }
                        else
                        {
                            listLexema[i].Aceito = false;
                            listLexema[i].Erro = "Erro Sintático na sentença. Ordem:" + listLexema[i].Ordem +
                                                ". Texto:" + listLexema[i].Token + ".";
                        }
                        break;
                    case "CMD_IF":
                        if (i == 0)
                        {
                            listLexema[i].Aceito = true;
                        }
                        else if (!delimValid.Contains(listLexema[i - 1].Token) || listLexema[i - 1].Token != "="
                                || !listLexema[i - 1].Token.Contains("OP"))
                        {
                            i++;
                            if(ProximoEstado(i, listLexema, delimValid))
                            {

                            }

                        }
                        break;
                    default:
                        break;
                }
            }
        }
        private static bool ProximoEstado(int i, List<Pattern> listPatterns, string[] delimValid) {
            for (int c = i; c < listPatterns.Count(); c++)
            {
                switch (listPatterns[c].Lexema)
                {
                    case "(":
                        if (listPatterns[c -1].Lexema == "CMD_IF")
                        {
                            listPatterns[c].Aceito = true;

                        }
                        else
                        {
                            listPatterns[c].Aceito = false;
                            listPatterns[c].Erro = "Erro Sintático na sentença. Ordem:" + listPatterns[c].Ordem +
                                                ". Texto:" + listPatterns[c].Token + ".";
                        }
                        break;
                    case "}":
                        if (delimValid.Contains(listLexema[i - 1].Token) || listLexema[i - 1].Token == "ID"
                                || listLexema[i - 1].Token == ")" || listLexema[i - 1].Lexema == "NUM")
                        {
                            listPatterns[c].Aceito = true;
                            return true;
                        }
                            break;
                    default:
                        break;

                }
                i++;
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
                            index = length + index;
                            length = 0;
                        }
                    }
                    else if (word.Length > 0)
                    {
                        patterns.Add(new Pattern { Lexema = "ID", Token = word, Ordem = patterns.Count() + 1 });
                        word = "";
                        index = length + index;
                        length = 0;
                    }
                }
                string auxText = "";
                if(text == "=")
                {
                    auxText = input.Substring(index, length + 1);
                    if (auxText == "==")
                    {
                        Conditional(auxText);
                        index = length + index + 1;
                        length = 0;
                    }
                    else
                    {
                        Attribution(text);
                        index = length + index;
                        length = 0;
                    }
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
                        index = length + index;
                        length = 0;
                    }
                    
                }
                if (Delimiter(text) || Operators(auxText != "" ? auxText:text) || Conditional(text))
                {
                   
                    index = length + index;
                                        
                    length = 0;
                }

                length++;
            }
        }


        private static bool Delimiter(string input)
        {
            Regex DelimiterRegex = new Regex(@"[\\}\\{\\)\\(\n\f\t]+");
            List<Pattern> lista = new List<Pattern>();
            string[] especiais = { "\n", "\f", "\r", "\t" };
            List<IdentifyDelimiter> identifies = IdentifyDelimiter.List();
            lista = DelimiterRegex.Matches(input)
                .Cast<Match>()
                .Select(m => new Pattern { Lexema = "DELIM", Token = identifies.Where(x => x.Delimiter == m.Value).Count() > 0 ?
                identifies.FirstOrDefault(x => x.Delimiter == m.Value).Identifier : m.Value, Ordem = patterns.Count() + 1 })
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
            string DesvioIFRegex = "if";

            string DesvioELSERegex = "else"; 


            List<Pattern> lista = new List<Pattern>();

           
            if (input.Contains(DesvioIFRegex)) {
                patterns.Add(new Pattern { Lexema = "CMD_IF", Token = input, Ordem = patterns.Count() + 1 });
                return true;
            }




            if (input.Contains(DesvioELSERegex))
            {
                patterns.Add(new Pattern { Lexema = "CMD_ELSE", Token = input, Ordem = patterns.Count() + 1 });
                return true;
            }


            return false;
        }

        private static bool Repetition(string input)
        {
            string RepetitionWhileRegex =  "while";
            string RepetitionForRegex = "for";

            List<Pattern> lista = new List<Pattern>();

           
            if (input.Contains(RepetitionWhileRegex))
            {
                patterns.Add(new Pattern { Lexema = "CMD_FOR", Token = input, Ordem = patterns.Count() + 1 });
                return true;
            }
            

            
            if (input.Contains(RepetitionForRegex))
            {
                patterns.Add(new Pattern { Lexema = "CMD_WHILE", Token = input, Ordem = patterns.Count() + 1 });
                return true;
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
            
            return result;
        }

        public static List<Pattern> AnalisadorSintatico(List<Pattern> lexemas_tokens)
        {
            List<Pattern> result = new List<Pattern>();
            
            //Valida entrada
            Validador(lexemas_tokens);

            result.AddRange(patterns);

            return result;
        }
    }
}