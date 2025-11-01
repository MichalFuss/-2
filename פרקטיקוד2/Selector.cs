using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace פרקטיקוד2
{
    public class Selector
    {
        public string TagName { get; set; }
        public string Id { get; set; }
        public List<string> Classes { get; set; } = new List<string>();
        public Selector Parent { get; set; }
        public Selector Child { get; set; }

        // פונקציה סטטית שממירה מחרוזת של סלקטור לאובייקט Selector
        public static Selector FromQuery(string query)
        {
            // 1. חיתוך המחרוזת לפי רווחים - כל רמה בשאילתה מופרדת ברווח
            var segments = query.Split(' ');

            // 2. יצירת אובייקט Selector שיתפקד כשורש
            Selector root = new Selector();
            Selector current = root;

            // 3. לולאה על כל החלקים במחרוזת
            foreach (var segment in segments)
            {
                var selector = new Selector();

                // 4. פרק את החלק של המחרוזת לפי המפרידים # ו-. (נקודה)
                if (segment.Contains("#"))
                {
                    selector.Id = segment.Split('#')[1];  // חיתוך ה-id
                }

                if (segment.Contains("."))
                {
                    selector.Classes.Add(segment.Split('.')[1]);  // חיתוך ה-class
                }
                else if (!string.IsNullOrEmpty(segment))
                {
                    // אם אין # או ., אז שם התגית (TagName)
                    selector.TagName = segment;
                }

                // 5. צור אובייקט Selector חדש, הוסף אותו כילד של הסלקטור הנוכחי
                current.Child = selector;
                current = selector;  // עדכון הסלקטור הנוכחי
            }

            // 6. החזר את אובייקט ה-root (השורש) שמייצג את הסלקטור
            return root;
        }
    }
}