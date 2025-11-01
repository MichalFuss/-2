using System.Text.RegularExpressions;
using פרקטיקוד2;

async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}
string url = "https://www.example.com";
var html = await Load(url);
var cleanHtml = new Regex("\\s").Replace(html, "");
var HtmlLines = new Regex("<(.*?)>").Split(cleanHtml).Where(s => s.Length > 0);
HtmlElement root = new HtmlElement();
HtmlElement currentElement = root;
פרקטיקוד2\Program.cs
using System.Text.RegularExpressions;
using System.Linq;
using פרקטיקוד2;

async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}
string url = "https://www.example.com";
var html = await Load(url);
var cleanHtml = new Regex("\\s").Replace(html, "");
var HtmlLines = new Regex("<(.*?)>").Split(cleanHtml).Where(s => s.Length > 0);

HtmlElement root = new HtmlElement();
HtmlElement currentElement = root;

foreach (var line in HtmlLines)
{
    
    var tagMatch = Regex.Match(line, @"^\/?([^\s\/>]+)");
    if (!tagMatch.Success)
    {
       
        if (currentElement != null)
            currentElement.InnerHtml = line;
        continue;
    }

    string tagName = tagMatch.Groups[1].Value;
    bool isClosingTag = line.StartsWith("/");
    bool isHtmlClosing = isClosingTag && string.Equals(tagName, "html", StringComparison.OrdinalIgnoreCase);

    if (isHtmlClosing)
        break;

    if (isClosingTag)
    {
        if (currentElement?.Parent != null)
            currentElement = currentElement.Parent;
        continue;
    }

    if (!HtmlHelper.Tags.AllTags.Contains(tagName, StringComparer.OrdinalIgnoreCase))
    {
        if (currentElement != null)
            currentElement.InnerHtml = line;
        continue;
    }

    HtmlElement newElement = new HtmlElement
    {
        Name = tagName,
        Parent = currentElement
    };

    currentElement.Children.Add(newElement);

    var remainder = line.Substring(tagMatch.Length).Trim();

    bool endsWithSlash = remainder.EndsWith("/");
    bool listedSelfClosing = HtmlHelper.Tags.SelfClosingTags
        .Contains(tagName, StringComparer.OrdinalIgnoreCase);
    bool isSelfClosing = endsWithSlash || listedSelfClosing;

    var attrRegex = new Regex(@"([^\s=]+)(?:=(?:""([^""]*)""|'([^']*)'|([^\s""]+)))?");
    foreach (Match m in attrRegex.Matches(remainder))
    {
        if (!m.Success) continue;
        var attrName = m.Groups[1].Value;
        string attrValue = null;
        if (m.Groups[2].Success) attrValue = m.Groups[2].Value;
        else if (m.Groups[3].Success) attrValue = m.Groups[3].Value;
        else if (m.Groups[4].Success) attrValue = m.Groups[4].Value;
        else attrValue = string.Empty; // boolean attribute

        // store attribute in the Attributes list (as raw "name=value" or "name" for boolean)
        if (attrValue.Length > 0)
            newElement.Attributes.Add($"{attrName}={attrValue}");
        else
            newElement.Attributes.Add(attrName);

        // handle special attributes
        if (string.Equals(attrName, "class", StringComparison.OrdinalIgnoreCase))
        {
            // classes separated by spaces (if any). If no spaces (because whitespace was stripped earlier),
            // this will still add the single value.
            var classes = attrValue.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var c in classes)
                newElement.Classes.Add(c);
        }
        else if (string.Equals(attrName, "id", StringComparison.OrdinalIgnoreCase))
        {
            // HtmlElement.Id is an int in the model; try to parse numeric id, otherwise ignore.
            if (int.TryParse(attrValue, out var numericId))
                newElement.Id = numericId;
            // if Id is not numeric, we do not set it because model's Id is an int.
        }
    }

    // if the element is NOT self-closing, descend into it (it becomes the current element)
    if (!isSelfClosing)
    {
        currentElement = newElement;
    }
    // else: leave currentElement unchanged
}