using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchSystem
{
    class Word
    {
        private static Dictionary<List<int>, Word> wordsByDocCollection = null;
        private static bool _isCollectionUpToDate = false;
        public int id;
        public string lemm;
        public int mentionings; // count of documents where word is mentioned
        public Dictionary<int, int> refs; // <docId, times>

        public Word(string _lemm, int docId)
        {
            lemm = _lemm;
            mentionings = 1;
            refs = new Dictionary<int, int>();
            refs.Add(docId, 1);
        }

        public Word(int lemmId, string _lemm, int _mentionings)
        {
            id = lemmId;
            lemm = _lemm;
            mentionings = _mentionings;
            refs = new Dictionary<int, int>();
        }

        public static bool isCollectionUpToDate()
        {
            return _isCollectionUpToDate;
        }

        public static void setWordsCollection(Dictionary<List<int>, Word> collection)
        {
            wordsByDocCollection = collection;
            _isCollectionUpToDate = true;
        }

        public static List<Word> getWordListByDocId(int docId)
        {
            List<Word> result = new List<Word>();

            foreach(var docsLemmPair in wordsByDocCollection)
            {
                if (docsLemmPair.Key.Contains(docId))
                {
                    result.Add(docsLemmPair.Value);
                }
            }

            return result;
        }
    }
}
