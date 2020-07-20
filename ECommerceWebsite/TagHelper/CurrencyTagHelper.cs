using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerceWebsite.TagHelper
{
    [HtmlTargetElement(Attributes = CurrencyAttributeName)]
    public class CurrencyTagHelper : TagHelperComponent
    {
        private const string CurrencyAttributeName = "currency";

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            string content = @"<span> Rs. </span>";

            output.PostContent.AppendHtml(content);

            if (output.Attributes.ContainsName(CurrencyAttributeName))
            {
                output.Attributes.RemoveAll(CurrencyAttributeName);
            }

            base.Process(context, output);
        }
    }
}
