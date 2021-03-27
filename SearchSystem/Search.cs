using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchSystem
{
    class Search
    {
        bool allWordsTogether;
        string dateStartString;
        string dateEndString;
        string searchQuery;

        Dictionary<int, double> getSearchQueryVector(string query, Dictionary<int, double> docVector)
        {
            return new Dictionary<int, double>();
        }

        double scalarProduct(Dictionary<int, double> val)
        {
            return 0;
        }

        double euclidNorm(Dictionary<int, double> val)
        {
            return 0;
        }

       /* IOrderedEnumerable <SearchResult> getSearchResult()
        {
            return new IOrderedEnumerable <SearchResult>();
        }*/
    }
}
