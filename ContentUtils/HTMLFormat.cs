using Ganss.XSS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TidyManaged;

namespace ContentUtils
{
    public class HTMLFormat
    {
        public static string CleanAndRepair(string content, bool contentisfile = false,
                                            string encodeinput = null, string encodecontent = null, string encodeoutput = null,
                                            bool onlybody = true,
                                            bool outputxhtml = false,
                                            bool outputhtml = false,
                                            bool indent = false,
                                            string contentencode = "windows-1252")
        {
            using (var m = new MemoryStream(contentisfile ? File.ReadAllBytes(content) : System.Text.Encoding.GetEncoding(contentencode).GetBytes(content)))
            {
                using (Document doc = Document.FromStream(m))
                {
                    if (encodecontent != null && Enum.TryParse(encodecontent, out EncodingType ec))
                        doc.CharacterEncoding = ec;
                    else
                    {
                        if (encodeinput != null && Enum.TryParse(encodeinput, out EncodingType ei))
                            doc.InputCharacterEncoding = ei;

                        if (encodeoutput != null && Enum.TryParse(encodeoutput, out EncodingType eo))
                            doc.OutputCharacterEncoding = eo;
                    }

                    doc.OutputBodyOnly = onlybody ? AutoBool.Yes : AutoBool.Auto;

                    doc.ShowWarnings = true;
                    doc.Quiet = false;
                    doc.IndentAttributes = indent;
                    doc.IndentBlockElements = indent ? AutoBool.Yes : AutoBool.Auto;
                    doc.OutputXhtml = outputxhtml;
                    doc.OutputHtml = outputhtml;

                    doc.CleanAndRepair();

                    string parsed = doc.Save();


                    return parsed;
                }
            }
        }

        public static string CleanUnwantedHtml(string html, string encode, params HtmlTag[] Tags)
        {
            var tags = Tags.ToList();

            var ha = new HTMLAgility();

            html = ha.RemoveUnwantedHtmlTags(html, encode, tags.Where(t => t.RemoveAllTag).Select(t => t.Tag).ToList());



            tags.Where(t => t.RemoveAllTag == false && t.AttributeToRemove != null && t.AttributeToRemove.Length > 0).ToList()
                       .ForEach(tag =>
                       {
                           html = ha.RemoveUnwantedHtmlAttributes(html, encode, tag.Tag, tag.AttributeToRemove.ToList());
                       });

            tags.Where(t => t.RemoveAllTag == false && t.StyleToRemove != null && t.StyleToRemove.Length > 0).ToList()
                        .ForEach(tag =>
                        {
                            html = ha.RemoveUnwantedHtmlStyle(html, encode, tag.Tag, tag.StyleToRemove.ToList());
                        });

            return html;
        }

        public static List<KeyValuePair<string, string>> GetTagContent(string html, string encode, string Tag)
        {
            var ha = new HTMLAgility();
            return ha.GetTagContent(html, encode, Tag);
        }

        public static string ConsolidateRepeatedTags(string html, string encode, string Tag = "span")
        {
            var ha = new HTMLAgility();
            return ha.ConsolidateRepeatedTags(html, encode, Tag);
        }

        public static string CleanHtml(string html)
        {
            var sanitizer = new HtmlSanitizer();
            return sanitizer.Sanitize(html);
        }

        public static string HtmlToAmp(string html)
        {
            var converter = new Html2Amp.HtmlToAmpConverter();
            var result = converter.ConvertFromHtml(html);
            return result.AmpHtml;
        }


        public static string CleanHtmlFromWord(string conteudo, string encode = "utf-8", string encode_return = "windows-1252", bool return_only_body = true, List<HTMLFormat.HtmlTag> tags = null)
        {
            if (tags == null)
            {
                tags = new List<HTMLFormat.HtmlTag>();

                tags.Add(new HTMLFormat.HtmlTag()
                {
                    Tag = "p",
                    StyleToRemove = new string[] { "page-break-inside", "page-break-after", "page-break-before", "border-top-style:none",
                                                   "border-bottom-style:none", "border-left-style:none", "border-right-style:none",
                                                   "margin-top:0pt","margin-bottom:0pt","margin-left:0pt","text-indent:0pt","margin-right:0pt",
                                                   "mso-bidi-font-family", "mso-fareast-font-family", "mso-fareast-language", "color:#000000", "background-color:#FFFFFF",
                                                   "line-height", "position" },
                    AttributeToRemove = new string[] { "class" }
                });
                tags.Add(new HTMLFormat.HtmlTag()
                {
                    Tag = "span",
                    StyleToRemove = new string[] { "page-break-inside", "page-break-after", "page-break-before", "border-top-style:none",
                                                   "border-bottom-style:none", "border-left-style:none", "border-right-style:none",
                                                   "margin-top:0pt","margin-bottom:0pt","margin-left:0pt","text-indent:0pt","margin-right:0pt",
                                                   "mso-bidi-font-family", "mso-fareast-font-family", "mso-fareast-language", "color:#000000", "background-color:#FFFFFF",
                                                   "line-height", "position" },
                    AttributeToRemove = new string[] { "class" }
                });
                tags.Add(new HTMLFormat.HtmlTag()
                {
                    Tag = "a",
                    StyleToRemove = new string[] { "page-break-inside", "page-break-after", "page-break-before", "border-top-style:none",
                                                   "border-bottom-style:none", "border-left-style:none", "border-right-style:none",
                                                   "margin-top:0pt","margin-bottom:0pt","margin-left:0pt","text-indent:0pt","margin-right:0pt",
                                                   "mso-bidi-font-family", "mso-fareast-font-family", "mso-fareast-language", "color:#000000", "background-color:#FFFFFF",
                                                   "line-height", "position" },
                    AttributeToRemove = new string[] { "class" }
                });
                tags.Add(new HTMLFormat.HtmlTag()
                {
                    Tag = "table",
                    StyleToRemove = new string[] { "page-break-inside", "page-break-after", "page-break-before", "border-top-style:none",
                                                   "border-bottom-style:none", "border-left-style:none", "border-right-style:none",
                                                   "margin-top:0pt","margin-bottom:0pt","margin-left:0pt","text-indent:0pt","margin-right:0pt",
                                                   "mso-bidi-font-family", "mso-fareast-font-family", "mso-fareast-language", "color:#000000", "background-color:#FFFFFF",
                                                   "line-height", "position" },
                    AttributeToRemove = new string[] { "class" }
                });
                tags.Add(new HTMLFormat.HtmlTag()
                {
                    Tag = "div",
                    RemoveAllTag = true
                });
            }

            conteudo = HTMLFormat.CleanUnwantedHtml(conteudo, encode, tags.ToArray());
            conteudo = HTMLFormat.ConsolidateRepeatedTags(conteudo, encode, "span"); //remove as repetições de SPAN desnecessárias do Word

            //obtem o body do Word
            var body = HTMLFormat.GetTagContent(conteudo, encode, "body");
            if (body != null && body.Where(d => true).Count() > 0)
            {
                conteudo = string.Join("<BR/>", body.Where(d => true).Select(d => d.Value).ToArray());
            }

            if (encode.ToUpper() != encode_return.ToUpper())
                conteudo = Encoding.GetEncoding(encode_return).GetString(Encoding.UTF8.GetBytes(conteudo));

            if (!return_only_body)
            {
                conteudo = "<html><head></head><body>" + conteudo + "</body></html>";
            }

            //conteudo = HTMLFormat.CleanAndRepair(conteudo, onlybody: true, outputxhtml: true, indent: true);

            return conteudo;


        }

        public class HtmlTag
        {
            public string Tag { get; set; }

            public bool RemoveAllTag { get; set; } = false;


            public string[] AttributeToRemove { get; set; } = null;
            public string[] AttributeToKeep { get; set; } = null;

            public string[] StyleToRemove { get; set; } = null;
            public string[] StyleToKeep { get; set; } = null;



        }

    }
}
