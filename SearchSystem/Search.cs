using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordNetClasses;

namespace SearchSystem
{
    class Search
    {
        bool allWordsTogether;
        string dateStartString;
        string dateEndString;
        string searchQuery;
        Dictionary<int, double> sqVect = null;

        public Search(string query, string dateStart = null, string dateEnd = null)
        {
            searchQuery = query;
            dateStartString = dateStart;
            dateEndString = dateEnd;
        }

        public double scalarProduct(Dictionary<int, double> docVector)
        {
            if (this.sqVect == null) this.sqVect = this.getSearchQueryVector();

            double result = 0.0d;

            foreach(var searchDocWeight in sqVect)
            {
                foreach(var docWeight in docVector)
                {
                    if (docWeight.Key == searchDocWeight.Key)
                    {
                        result += docWeight.Value * searchDocWeight.Value;
                    }
                }
            }
            result /= euclidNorm(docVector);

            return result;
        }

        protected Dictionary<int, double> getSearchQueryVector()
        {
            Dictionary<int, double> result = new Dictionary<int, double>();
            List<Word> lemmas = DbConn.getInstance().getWordObjectsForLemms(Parser.getAllLemms(this.searchQuery));

            foreach(var lem in lemmas)
            {
                result.Add(lem.id, 1.0d);
            }

            return result;
        }

        protected double euclidNorm(Dictionary<int, double> docVector)
        {
            double result = 0.0d;

            foreach(var wordWeight in docVector)
            {
                result += Math.Pow(wordWeight.Value, 2);
            }
            result = Math.Sqrt(result);

            return result;
        }

       /* IOrderedEnumerable <SearchResult> getSearchResult()
        {
            return new IOrderedEnumerable <SearchResult>();
        }*/
    }
}
