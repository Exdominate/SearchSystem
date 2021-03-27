using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchSystem
{
    class Document
    {
        public string title;
        public string text;
        public string date;
        public string time;
        int documentID;

        public bool addDocumentToBase()
        {
            return true;
        }

        static bool deleteDocumentFromBase(int? documentID)
        {
            return true;
        }

        static bool getLemmInverseFrequency(int lemmId, out double result)
        {
            result = 0;
            return true;
        }

        static bool getLemmInverseFrequency(string lemmStr, out double result)
        {
            result = 0;
            return true;
        }

        static bool getWordWeightInDocument(string lemmStr, int documentId, out double result)
        {
            result = 0;
            return true;
        }

        static bool getLemmWeightInDocument(int lemmId, int documentId, out double result)
        {
            result = 0;
            return true;
        }

        static Dictionary<int, double> getDocumentVector(int documentId)
        {
            return new Dictionary<int, double>();
        }
    }
}
