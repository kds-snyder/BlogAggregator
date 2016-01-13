using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class WordPressStringExtensions
    {
        // Adjust the input content (which should first have HTML scrubbing applied) as follows:
        // -Remove characters after [&#8230;], if the content contains:         
        //  "[&#8230;]\nThe post " and " appeared first on "
        // -If content does not have [&#8230;], but ends with &#8230; then add brackets
        //  so that content ends with [&#8230;] 
        // Note that &#8230; is displayed as ellipsis (...) 
        public static string AdjustContent(this string content)
        {
            // If content is empty, no adjustment is needed
            if (content.Length == 0)
            {
                return content;
            }
            // If "[&#8230;]\nThe post" and " appeared first on " are in content,
            //  remove all characters after "[&#8230;]"
            int indexBracket = content.IndexOf("[&#8230;]\nThe post");
            if (indexBracket > 0)
            {
                int indexAppeared = content.IndexOf(" appeared first on ");
                if (indexAppeared > indexBracket)
                {
                    string trimmedContent = content.Substring(0, indexBracket + 9);
                    return trimmedContent;
                }
            }
            // If content ends with "&#8230;" then add brackets
            // Insert a space before the left bracket if there is no space
            else if (content.Substring(content.Length - 7) == "&#8230;")
            {
                string leftBracket;
                if (content[content.Length - 8] != ' ')
                {
                    leftBracket = " [";
                }
                else
                {
                    leftBracket = "[";
                }
                string trimmedContent = content.Substring(0, content.Length - 7)
                                        + leftBracket + content.Substring(content.Length - 7) + "]";

                return trimmedContent;
            }

            // If content was not adjusted, return it
            return content;
        }
    }
}
