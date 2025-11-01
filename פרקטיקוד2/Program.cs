using System.Text.RegularExpressions;
using פרקטיקוד2;

string url = "https://www.example.com";
var html = await Load(url);
var cleanHtml = new Regex("\\s").Replace(html, "");
var htmlLines = new Regex("<(.*?)>").Split(cleanHtml).Where(s => s.Length > 0);

HtmlElement root = new HtmlElement();
HtmlElement currentElement = root;

foreach (var line in htmlLines)
{
    var tagMatch = Regex.Match(line, @"^\/?([^\s\/>]+)");

    // אם לא מצאנו תגית (מקרה של טקסט פנימי)
    if (!tagMatch.Success)
    {
        if (currentElement != null)
            currentElement.InnerHtml = line;
        continue;
    }

    string tagName = tagMatch.Groups[1].Value;
    bool isClosingTag = line.StartsWith("/");
    bool isHtmlClosing = isClosingTag && string.Equals(tagName, "html", StringComparison.OrdinalIgnoreCase);

    // אם הגענו לתגית סגירה של html, אז עצור
    if (isHtmlClosing)
        break;

    if (isClosingTag)
    {
        // אם מדובר בתגית סגירה, חזור לאבא
        if (currentElement?.Parent != null)
            currentElement = currentElement.Parent;
        continue;
    }

    // אם התגית לא מוכרת, תחשב אותה כטקסט פנימי
    if (!HtmlHelper.Tags.AllTags.Contains(tagName, StringComparer.OrdinalIgnoreCase))
    {
        if (currentElement != null)
            currentElement.InnerHtml = line;
        continue;
    }

    // יצירת אלמנט חדש
    HtmlElement newElement = new HtmlElement
    {
        Name = tagName,
        Parent = currentElement
    };

    // הוספת את האלמנט החדש ל-Children של האלמנט הנוכחי
    currentElement.Children.Add(newElement);

    // פרק את השארית של המחרוזת אחרי שם התגית
    var remainder = line.Substring(tagMatch.Length).Trim();

    // בדוק אם זה אלמנט עצמאי (כמו <br />)
    bool endsWithSlash = remainder.EndsWith("/");
    bool isSelfClosing = endsWithSlash || HtmlHelper.Tags.SelfClosingTags
        .Contains(tagName, StringComparer.OrdinalIgnoreCase);

    // אם זה לא אלמנט עצמאי, יש צורך לשמור את האלמנט הנוכחי כאלמנט אובייקט
    if (!isSelfClosing)
    {
        currentElement = newElement;
    }

    // חילוץ Attributes מתוך התגית
    var attrMatches = Regex.Matches(remainder, @"([^\s=]+)(?:=(?:""([^""]*)""|'([^']*)'|([^\s""]+)))?");
    foreach (Match match in attrMatches)
    {
        string attributeName = match.Groups[1].Value;
        string attributeValue = match.Groups[2].Value ?? match.Groups[3].Value ?? match.Groups[4].Value;

        if (attributeName.Equals("class", StringComparison.OrdinalIgnoreCase))
        {
            // אם יש את Attribute "class", פרק אותו לכמה חלקים
            currentElement.Classes.AddRange(attributeValue.Split(' '));
        }
        else if (attributeName.Equals("id", StringComparison.OrdinalIgnoreCase))
        {
            // אם יש את Attribute "id", שמור אותו ב-Id
            currentElement.Id = (int.TryParse(attributeValue, out int id) ? id : 0).ToString();
        }

        // הוסף את ה-Attribute לרשימת ה-Attributes
        currentElement.Attributes.Add(attributeName);
    }
}


// אפשר להוסיף כאן קוד נוסף להדפסת העץ או פעולות נוספות
PrintElementTree(root);


// פונקציה להורדת ה-HTML מאתר
static async Task<string> Load(string url)
{
    HttpClient client = new HttpClient();
    var response = await client.GetAsync(url);
    var html = await response.Content.ReadAsStringAsync();
    return html;
}

// פונקציה להדפסת העץ
static void PrintElementTree(HtmlElement element, string indent = "")
{
    Console.WriteLine($"{indent}<{element.Name}>");

    if (!string.IsNullOrEmpty(element.InnerHtml))
        Console.WriteLine($"{indent}  {element.InnerHtml}");

    foreach (var child in element.Children)
    {
        PrintElementTree(child, indent + "  ");
    }

    Console.WriteLine($"{indent}</{element.Name}>");
}
    