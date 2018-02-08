using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TestTaskFromAstral.Models;

namespace TestTaskFromAstral.HtmlHelpers
{
    public static class PagingHelper
    {
        public static MvcHtmlString PageLinks(this HtmlHelper html, MainObjectViewModel mainObject, Func<int, string> pageUrl)
        {

            StringBuilder result = new StringBuilder();

            if(mainObject != null)
            {
                if(mainObject.Items != null)
                {
                    for (int i = 1; i < Int32.Parse(mainObject.Pages); i++)
                    {
                        TagBuilder tag = new TagBuilder("a");//создаем объект для генерации Ссылки
                        tag.MergeAttribute("href", pageUrl(i));//определяем атрибут
                        tag.InnerHtml = i.ToString();//определяем внутренний текст
                        if (i == Int32.Parse(mainObject.Page))//если страница текущая то
                        {
                            tag.AddCssClass("selected");
                            tag.AddCssClass("btn-primary");
                        }
                        tag.AddCssClass("btn btn-default");

                        if ((i < (Int32.Parse(mainObject.Page) - 5)))//если страница текущая то
                        {
                            tag.AddCssClass("hidden");
                        }

                        if ((i > (Int32.Parse(mainObject.Page) + 5)))//если страница текущая то
                        {
                            tag.AddCssClass("hidden");
                        }

                        result.Append(tag.ToString());
                    }
                }
            }

            return MvcHtmlString.Create(result.ToString());
        }
    }
}
