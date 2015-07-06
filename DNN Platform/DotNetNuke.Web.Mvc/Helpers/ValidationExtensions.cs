﻿#region Copyright
// 
// DotNetNuke® - http://www.dnnsoftware.com
// Copyright (c) 2002-2014
// by DNN Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
#endregion

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace DotNetNuke.Web.Mvc.Helpers
{
    public static class ValidationExtensions
    {
        // Validate

        public static void ValidateFor<TModel, TProperty>(this DnnHtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression)
        {
            var htmlHelper = html.HtmlHelper as HtmlHelper<TModel>;
            htmlHelper.ValidateFor(expression);
        }

        // ValidationMessage

        public static MvcHtmlString ValidationMessageFor<TModel, TProperty>(this DnnHtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression)
        {
            var htmlHelper = html.HtmlHelper as HtmlHelper<TModel>;
            return htmlHelper.ValidationMessageFor(expression);
        }

        public static MvcHtmlString ValidationMessageFor<TModel, TProperty>(this DnnHtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string validationMessage)
        {
            var htmlHelper = html.HtmlHelper as HtmlHelper<TModel>;
            return htmlHelper.ValidationMessageFor(expression, validationMessage);
        }

        public static MvcHtmlString ValidationMessageFor<TModel, TProperty>(this DnnHtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string validationMessage, object htmlAttributes)
        {
            var htmlHelper = html.HtmlHelper as HtmlHelper<TModel>;
            return htmlHelper.ValidationMessageFor(expression, validationMessage, htmlAttributes);
        }

        public static MvcHtmlString ValidationMessageFor<TModel, TProperty>(this DnnHtmlHelper<TModel> html, Expression<Func<TModel, TProperty>> expression, string validationMessage, IDictionary<string, object> htmlAttributes)
        {
            var htmlHelper = html.HtmlHelper as HtmlHelper<TModel>;
            return htmlHelper.ValidationMessageFor(expression, validationMessage, htmlAttributes);
        }

        // ValidationSummary

        public static MvcHtmlString ValidationSummary(this DnnHtmlHelper html)
        {
            return html.HtmlHelper.ValidationSummary();
        }

        public static MvcHtmlString ValidationSummary(this DnnHtmlHelper html, bool excludePropertyErrors)
        {
            return html.HtmlHelper.ValidationSummary(excludePropertyErrors);
        }

        public static MvcHtmlString ValidationSummary(this DnnHtmlHelper html, string message)
        {
            return html.HtmlHelper.ValidationSummary(message);
        }

        public static MvcHtmlString ValidationSummary(this DnnHtmlHelper html, bool excludePropertyErrors, string message)
        {
            return html.HtmlHelper.ValidationSummary(excludePropertyErrors, message);
        }

        public static MvcHtmlString ValidationSummary(this DnnHtmlHelper html, string message, object htmlAttributes)
        {
            return html.HtmlHelper.ValidationSummary(message, htmlAttributes);
        }

        public static MvcHtmlString ValidationSummary(this DnnHtmlHelper html, bool excludePropertyErrors, string message, object htmlAttributes)
        {
            return html.HtmlHelper.ValidationSummary(excludePropertyErrors, message, htmlAttributes);
        }

        public static MvcHtmlString ValidationSummary(this DnnHtmlHelper html, string message, IDictionary<string, object> htmlAttributes)
        {
            return html.HtmlHelper.ValidationSummary(message, htmlAttributes);
        }

        public static MvcHtmlString ValidationSummary(this DnnHtmlHelper html, bool excludePropertyErrors, string message, IDictionary<string, object> htmlAttributes)
        {
            return html.HtmlHelper.ValidationSummary(excludePropertyErrors, message, htmlAttributes);
        }
    }
}
