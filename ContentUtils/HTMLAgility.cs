using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContentUtils
{
    internal class HTMLAgility
    {
        public string RemoveUnwantedHtmlTags(string html, string encode, List<string> unwantedTags)
        {
            if (string.IsNullOrEmpty(html))
                return html;


            var document = GetHtmlDocument(html, encode);
            HtmlNodeCollection tryGetNodes = document.DocumentNode.SelectNodes("./*|./text()");


            if (tryGetNodes == null || !tryGetNodes.Any())
                return html;


            var nodes = new Queue<HtmlNode>(tryGetNodes);


            while (nodes.Count > 0)
            {
                var node = nodes.Dequeue();
                var parentNode = node.ParentNode;
                var childNodes = node.SelectNodes("./*|./text()");


                if (childNodes != null)
                {
                    foreach (var child in childNodes)
                        nodes.Enqueue(child);
                }


                if (unwantedTags.Any(tag => tag == node.Name))
                {
                    if (childNodes != null)
                    {
                        foreach (var child in childNodes)
                            parentNode.InsertBefore(child, node);
                    }


                    parentNode.RemoveChild(node);
                }
            }


            return document.DocumentNode.InnerHtml;
        }


        public string RemoveUnwantedHtmlAttributes(string html, string encode, string Tag, List<string> unwantedAttributes)
        {
            if (string.IsNullOrEmpty(html))
                return html;
            var document = GetHtmlDocument(html, encode);
            var tags = document.DocumentNode.SelectNodes("//" + Tag);
            if (tags != null)
            {
                foreach (var node in tags)
                {
                    foreach (var attr in unwantedAttributes)
                        node.Attributes.Remove(attr);
                }
            }
            return document.DocumentNode.InnerHtml;
        }

        public string RemoveUnwantedHtmlStyle(string html, string encode, string Tag, List<string> unwantedStyles)
        {
            if (string.IsNullOrEmpty(html))
                return html;

            unwantedStyles.ForEach(s => s = s.Replace(" ", ""));

            var document = GetHtmlDocument(html, encode);
            var tags = document.DocumentNode.SelectNodes("//" + Tag);
            if (tags != null)
            {
                foreach (var node in tags)
                {
                    var style = node.GetAttributeValue("style", "");
                    var stylestokeep = new List<string>();
                    if (style != "")
                    {
                        var styles = style.Split(';');
                        foreach (var st in styles)
                        {
                            if (!unwantedStyles.Contains(st.Replace(" ", "")))
                            {
                                var efective_style = st.Split(':')[0].Trim();
                                if (!unwantedStyles.Contains(efective_style))
                                    stylestokeep.Add(st);
                            }
                        }
                        node.SetAttributeValue("style", string.Join(";", stylestokeep.ToArray()));
                    }

                }
            }
            return document.DocumentNode.InnerHtml;
        }

        public string ConsolidateRepeatedTags(string html, string encode, string Tag = "span")
        {
            if (string.IsNullOrEmpty(html))
                return null;

            var document = GetHtmlDocument(html, encode);

            HtmlNodeCollection tryGetNodes = document.DocumentNode.SelectNodes("./*|./text()");


            if (tryGetNodes == null || !tryGetNodes.Any())
                return html;


            var nodes = new Queue<HtmlNode>(tryGetNodes);

            HtmlNode lastnode = null;
            while (nodes.Count > 0)
            {
                var node = nodes.Dequeue();
                var parentNode = node.ParentNode;
                var childNodes = node.SelectNodes("./*|./text()");


                if (childNodes != null)
                {
                    foreach (var child in childNodes)
                        nodes.Enqueue(child);
                }

                if (node.Name.ToUpper() == Tag.ToUpper() && lastnode != null && lastnode.Name == node.Name && 
                    lastnode.ParentNode != null && node.ParentNode != null &&
                    lastnode.ParentNode.XPath == node.ParentNode.XPath)
                {

                    if (SerializedAttributes(lastnode).ToUpper() == SerializedAttributes(node).ToUpper())
                    {
                        lastnode.InnerHtml += node.InnerHtml;
                        parentNode.RemoveChild(node);
                    }
                    else
                    {
                        lastnode = node;
                    }
                }
                else
                {
                    lastnode = node;
                }
            }


            return document.DocumentNode.InnerHtml;


        }

        public List<KeyValuePair<string, string>> GetTagContent(string html, string encode, string Tag)
        {
            if (string.IsNullOrEmpty(html))
                return null;

            var document = GetHtmlDocument(html, encode);

            var ret = new List<KeyValuePair<string, string>>();
            var tags = document.DocumentNode.SelectNodes("//" + Tag);
            if (tags != null)
            {
                foreach (var node in tags)
                {
                    ret.Add(new KeyValuePair<string, string>(SerializedAttributes(node), node.InnerHtml));
                }
            }
            return ret;
        }

        private static string SerializedAttributes(HtmlNode node)
        {
            return string.Join(" ", node.Attributes.Select(a => a.Name + "=\"" + a.Value + "\"").OrderBy(a => a).ToArray());
        }

        private HtmlDocument GetHtmlDocument(string htmlContent, string encoding = "windows-1252")
        {
            var doc = new HtmlDocument()
            {
                OptionOutputAsXml = false,
                OptionReadEncoding = true,
                OptionDefaultStreamEncoding = Encoding.GetEncoding(encoding)
            };
            doc.LoadHtml(htmlContent);
            return doc;
        }

    }
}
