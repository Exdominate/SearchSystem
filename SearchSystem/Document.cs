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
        public string link;
        public string date;
        public string time;
        public int documentID;

        public Document() { }

        public Document(int id, string datetime, string _title, string _text, string _link)
        {
            documentID = id;
            date = datetime.Substring(0, 10);
            time = datetime.Substring(11, 8);
            title = _title;
            text = _text;
            link = _link;            
        }

       /*public bool addDocumentToBase()
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
        }*/

        public static Dictionary<int, double> getDocumentVector(int documentId)
        {
            Dictionary<int, double> result = new Dictionary<int, double>();            
            List<Word> lemms = Word.getWordListByDocId(documentId);
            
            foreach(var curLem in lemms)
            {
                double weight;
                if (getWordWeightInDocument(lemms, curLem, documentId, out weight))
                    result.Add(curLem.id, weight);
            }
            return result;
        }

        static bool getWordWeightInDocument(List<Word> docLemms, Word curLemm, int documentId, out double result)
        {
            try
            {
                double sqrtSumm = 0.0d;
                int amountOfDocuments = DbConn.getInstance().getAmountOfDocuments();

                foreach (Word lem in docLemms)
                {
                    sqrtSumm += Math.Pow(calcTDIDF(lem, documentId, amountOfDocuments), 2);
                }
                sqrtSumm = Math.Sqrt(sqrtSumm);

                result = calcTDIDF(curLemm, documentId, amountOfDocuments) / sqrtSumm;
                return true;
            }
            catch
            {
                result = -1.0d;
                return false;
            }
        }

        private static double calcTDIDF(Word lemmObj, int docId, int amountOfDocuments)
        {
            return lemmObj.refs[docId] * 
                Math.Log( amountOfDocuments / lemmObj.mentionings );
        }

        
    }
}
