using Microsoft.Graph;
using System.Collections.Generic;

namespace day30Sample.Model
{
    public class SearchRequest
    {
        public List<SearchRequestObject> Requests {set;get;}

        public SearchAlterationOptions QueryAlterationOptions { set; get; }
    }
}
