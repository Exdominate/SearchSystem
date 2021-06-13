using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeepMorphy;

namespace SearchSystem
{
    class Parser
    {
        public static string removeTagsAndSymbols(string text)
        {
            StringBuilder stB = new StringBuilder(text);
            for (int i = 0; i < stB.Length; )
            {
                if (stB[i] == '\t' || stB[i] == '\n')
                {
                    stB.Remove(i, 1);
                }
                else if (stB[i] == '<')
                {
                    var j = i;
                    while (stB[j] != '>' && j < stB.Length) j++;
                    if (j != stB.Length)
                    {
                        stB.Remove(i, j - i + 1);
                    }
                    else
                    {
                        i++;
                    }
                }
                else
                {
                    i++;
                }
            }            
            return stB.ToString();
        }

        public static DeepMorphy.Model.MorphInfo[] getAllLemms(string text)
        {
            string[] separators = { ",", ".", "!", "?", ";", ":", " ", "(", ")", "[", "]", "\"", "\'", "-", "–", "—", "»", "«", "•", "{", "}" };
            var words = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            MorphAnalyzer MA = new MorphAnalyzer(withLemmatization: true, withTrimAndLower: true);
            return MA.Parse(words).ToArray();     
        }
    }
}
