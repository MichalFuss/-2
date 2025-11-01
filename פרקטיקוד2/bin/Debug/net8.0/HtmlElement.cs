using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace פרקטיקוד2
{
    internal class HtmlElement
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<string> Attributes { get; set; }
        public List<string> Classes { get; set; }
        public string InnerHtml { get; set; }

        public HtmlElement Parent { get; set; }
        public List<HtmlElement> Children { get; set; }
        public HtmlElement() 
        {
            Attributes = new List<string>(); 
            Classes = new List<string>(); 
            Children = new List<HtmlElement>(); 
        }

        // פונקציית Descendants המממשת את הלוגיקה עם Queue
        public IEnumerable<HtmlElement> Descendants()
        {
            Queue<HtmlElement> queue = new Queue<HtmlElement>();
            queue.Enqueue(this);  // דחוף את האלמנט הנוכחי לתור

            while (queue.Count > 0)
            {
                HtmlElement current = queue.Dequeue();  // שלוף את האלמנט הראשון בתור
                yield return current;  // החזר את האלמנט הנוכחי

                foreach (var child in current.Children)
                {
                    queue.Enqueue(child);  // הוסף את הבנים של האלמנט לתור
                }
            }
        }

        // פונקציית Ancestors
        public IEnumerable<HtmlElement> Ancestors()
        {
            HtmlElement current = this.Parent;
            while (current != null)
            {
                yield return current;
                current = current.Parent;  // המשך להורה הבא
            }
        }

        public List<HtmlElement> FindElements(Selector selector)
        {
            HashSet<HtmlElement> uniqueResults = new HashSet<HtmlElement>();
            List<HtmlElement> results = new List<HtmlElement>();
            FindElementsRecursive(this, selector, uniqueResults, results);
            return results;
        }

        private void FindElementsRecursive(HtmlElement element, Selector selector, HashSet<HtmlElement> uniqueResults, List<HtmlElement> results)
        {
            if (MatchesSelector(element, selector) && uniqueResults.Add(element))  // אם הוספנו את האלמנט
            {
                results.Add(element);
            }

            foreach (var child in element.Children)
            {
                FindElementsRecursive(child, selector.Child, uniqueResults, results);
            }
        }
        // פונקציה שבודקת אם אלמנט תואם לסלקטור
        private bool MatchesSelector(HtmlElement element, Selector selector)
        {
            bool matches = true;

            if (!string.IsNullOrEmpty(selector.TagName) && element.Name != selector.TagName)
            {
                matches = false;
            }

            if (!string.IsNullOrEmpty(selector.Id) && element.Id != selector.Id)
            {
                matches = false;
            }

            if (selector.Classes.Count > 0 && !selector.Classes.TrueForAll(c => element.Classes.Contains(c)))
            {
                matches = false;
            }

            return matches;
        }
    }
}

