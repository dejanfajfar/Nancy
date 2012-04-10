﻿namespace Nancy.ViewEngines.Razor
{
    using System;
    using System.IO;

    public static class HtmlHelpersExtensions
    {
        public static IHtmlString GetUrl<T>(this IHtmlHelpers<T> helpers)
        {
            //return new NonEncodedHtmlString(helpers.RenderContext.Context.Request.Url.ToString());
            return new NonEncodedHtmlString("Hello");
        }
    }

    /// <summary>
    /// Helpers to generate html content.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    public class HtmlHelpers<TModel> : IHtmlHelpers<TModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlHelpers{t}"/> class.
        /// </summary>
        /// <param name="engine">The razor view engine instance that the helpers are being used by.</param>
        /// <param name="renderContext">The <see cref="IRenderContext"/> that the helper are being used by.</param>
        /// <param name="model">The model that is used by the page where the helpers are invoked.</param>
        public HtmlHelpers(RazorViewEngine engine, IRenderContext renderContext, TModel model)
        {
            this.Engine = engine;
            this.RenderContext = renderContext;
            this.Model = model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public TModel Model { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public RazorViewEngine Engine { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public IRenderContext RenderContext { get; set; }

        /// <summary>
        /// Renders a partial with the given view name.
        /// </summary>
        /// <param name="viewName">Name of the view.</param>
        /// <returns>An <see cref="IHtmlString"/> representation of the partial.</returns>
        public IHtmlString Partial(string viewName)
        {
            return this.Partial(viewName, null);
        }

        /// <summary>
        /// Renders a partial with the given view name.
        /// </summary>
        /// <param name="viewName">Name of the partial view.</param>
        /// <param name="modelForPartial">The model that is passed to the partial.</param>
        /// <returns>An <see cref="IHtmlString"/> representation of the partial.</returns>
        public IHtmlString Partial(string viewName, dynamic modelForPartial)
        {
            var view = this.RenderContext.LocateView(viewName, modelForPartial);

            var response = this.Engine.RenderView(view, modelForPartial, this.RenderContext);
            Action<Stream> action = response.Contents;
            var mem = new MemoryStream();

            action.Invoke(mem);
            mem.Position = 0;

            var reader = new StreamReader(mem);

            return new NonEncodedHtmlString(reader.ReadToEnd());
        }

        /// <summary>
        /// Returns an html string composed of raw, non-encoded text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>An <see cref="IHtmlString"/> representation of the raw text.</returns>
        public IHtmlString Raw(string text)
        {
            return new NonEncodedHtmlString(text);
        }

        /// <summary>
        /// Creates an anti-forgery token.
        /// </summary>
        /// <returns>An <see cref="IHtmlString"/> representation of the anti forgery token.</returns>
        public IHtmlString AntiForgeryToken()
        {
            var tokenKeyValue = 
                this.RenderContext.GetCsrfToken();

            return new NonEncodedHtmlString(String.Format("<input type=\"hidden\" name=\"{0}\" value=\"{1}\"/>", tokenKeyValue.Key, tokenKeyValue.Value));
        }
    }
}