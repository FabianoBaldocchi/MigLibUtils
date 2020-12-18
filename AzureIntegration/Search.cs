using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Azure.CognitiveServices.Search.EntitySearch;
using Microsoft.Azure.CognitiveServices.Search.EntitySearch.Models;
using Newtonsoft.Json;

namespace AzureIntegration
{
    public class Search
    {
        public static string credential = "534e652ac5434b479e53ad7a9f02b4db";

        public static string endpoint = "https://tudo-sobre-prod.cognitiveservices.azure.com/bing/v7.0/entities";

        public static EntityData SearchEntity(string query, string languague = "pt-BR")
        {
            try
            {
                var client = new EntitySearchAPI(new ApiKeyServiceClientCredentials(credential));
                // client.BaseUri = new Uri(endpoint);

                var entityData = client.Entities.Search(query: query, market: languague);
                if (entityData.Entities == null)
                {

                    return new EntityData()
                    {
                        Name = new System.Globalization.CultureInfo("pt-BR", false).TextInfo.ToTitleCase(query)
                    };
                }
                else
                {
                    var mainEntity = entityData.Entities.Value.Where(thing => thing.EntityPresentationInfo.EntityScenario == EntityScenario.DominantEntity).FirstOrDefault();

                    if (mainEntity != null)
                    {
                        return new EntityData()
                        {
                            Description = mainEntity.Description,
                            Name = mainEntity.Name,
                            ImageUrlThumbnail = mainEntity.Image != null ? mainEntity.Image.ThumbnailUrl : null,
                            ImageUrl = (mainEntity.Image != null ? mainEntity.Image.Url : null) == null
                                  && mainEntity.Image != null
                                  && mainEntity.Image.HostPageUrl != null
                                  ? mainEntity.Image.HostPageUrl
                                  : null,
                            GeneralContent = Newtonsoft.Json.JsonConvert.SerializeObject(entityData)
                        };
                    }
                    else
                    {
                        return new EntityData()
                        {
                            Name = new System.Globalization.CultureInfo(languague, false).TextInfo.ToTitleCase(query)
                        };
                    }
                }
            }
            catch (Exception)
            {
                throw; //joga o erro pra cima
            }
        }

        public class EntityData
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string ImageUrl { get; set; }
            public string ImageUrlThumbnail { get; set; }
            public Dictionary<string, object> FrontParameters { get; set; } = new Dictionary<string, object>();
            public string GeneralContent { get; set; }

        }

    }
}
