using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WikipediaNet;
using WikipediaNet.Objects;

namespace WikipediaIntegration
{
    public class Search
    {
        //obter retorno em json direto da wikipedia
        //a wikipedia tem um GET para JSON formatável
        //retornar a lista ==> list=search
        //https://en.wikipedia.org/w/api.php?format=json&action=query&generator=search&gsrnamespace=0&gsrsearch=jair%20bolsonaro&gsrlimit=10&list=search|extracts&pilimit=max&exintro&explaintext&exsentences=1&exlimit=max
        //retornar as imagens => prop=pageimages
        //https://en.wikipedia.org/w/api.php?format=json&action=query&generator=search&gsrnamespace=0&gsrsearch=jair%20bolsonaro&gsrlimit=10&prop=pageimages|extracts&pilimit=max&exintro&explaintext&exsentences=1&exlimit=max
        //retornar o conteudo => prop=extracts
        //https://en.wikipedia.org/w/api.php?format=json&action=query&generator=search&gsrnamespace=0&gsrsearch=jair%20bolsonaro&gsrlimit=10&prop=extracts|extracts

        //TODO: tem que ver o formato certo, como define linguagem, etc

        public static string SimpleSearch(string query)
        {
            Wikipedia wiki = new Wikipedia();
            wiki.Limit = 5;
            wiki.Language = WikipediaNet.Enums.Language.Portuguese;

            

            QueryResult results = wiki.Search(query);
            


            return JsonConvert.SerializeObject(results,  Formatting.Indented);

        }

    }
}
