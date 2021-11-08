
using System.Web.Mvc;

namespace CV.Web
{
    public static class RazorHelpers
    {
        /// <summary>
        /// Puts a group (div) for validation messages
        /// </summary>
        /// <param name="helper">Html helper instance</param>
        /// <param name="putStar">Puts a red star (by default) to define it is required</param>
        /// <param name="messages">Message list, created by PutValidationMessage() methods</param>
        /// <returns>Mvc Html string back</returns>
        public static MvcHtmlString PutValidationMessageGroup(this HtmlHelper helper, bool putStar, params MvcHtmlString[] messages)
        {
            return helper.PutValidationMessageGroup(putStar, "validation", messages);
        }

        /// <summary>
        /// Puts a group (div) for validation messages
        /// </summary>
        /// <param name="helper">Html helper instance</param>
        /// <param name="putStar">Puts a red star (by default) to define it is required</param>
        /// <param name="messages">Message list, created by PutValidationMessage() methods</param>
        /// <returns>Mvc Html string back</returns>
        public static MvcHtmlString PutValidationMessageGroup(this HtmlHelper helper, bool putStar, string classForMessage, params MvcHtmlString[] messages)
        {
            TagBuilder div = new TagBuilder("div");
            div.AddCssClass(classForMessage);
            if (putStar) div.InnerHtml = helper.PutValidationRequiredStar().ToString();
            foreach (MvcHtmlString mvcMessage in messages)
                div.InnerHtml = div.InnerHtml + mvcMessage;
            return new MvcHtmlString(div.ToString(TagRenderMode.Normal));
        }

        /// <summary>
        /// Generates a span for given message with localized values
        /// </summary>
        /// <param name="helper">Html helper instance</param>
        /// <param name="messageId">Message item id, required to be valid for span[@id]</param>
        /// <param name="text">Message</param>
        /// <param name="classForMessageTag">Span tag class name</param>
        /// <returns>Mvc html string back</returns>
        public static MvcHtmlString PutValidationMessage(this HtmlHelper helper, string messageId, string text, string classForMessageTag)
        {
            TagBuilder span = new TagBuilder("span");
            span.SetInnerText(text);
            span.MergeAttribute("id", messageId, true);
            span.AddCssClass(classForMessageTag);
            return new MvcHtmlString(span.ToString(TagRenderMode.Normal));
        }

        /// <summary>
        /// Generates a span for given message with localized values
        /// </summary>
        /// <param name="helper">Html helper instance</param>
        /// <param name="messageId">Message item id, required to be valid for span[@id]</param>
        /// <param name="localizationId">Localization item name for message</param>
        /// <returns>Mvc html string back</returns>
        public static MvcHtmlString PutValidationMessage(this HtmlHelper helper, string messageId, string text)
        {
            return helper.PutValidationMessage(messageId, text, "validation");
        }

        /// <summary>
        /// Generates a span with red asterix
        /// </summary>
        /// <param name="helper">Html helper instance</param>
        /// <returns>Mvc html string back</returns>
        public static MvcHtmlString PutValidationRequiredStar(this HtmlHelper helper)
        {
            TagBuilder span = new TagBuilder("span");
            span.SetInnerText("*");
            span.AddCssClass("required");
            return new MvcHtmlString(span.ToString(TagRenderMode.Normal));
        }
    }
}