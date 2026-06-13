using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace project.Helpers;

public static class ViewRenderer
{
    public static async Task<string> RenderPartialToStringAsync<TModel>(
        this Controller controller,
        string viewName,
        TModel model)
    {
        var httpContext = controller.ControllerContext.HttpContext;
        var viewEngine = httpContext.RequestServices.GetRequiredService<IRazorViewEngine>();
        var tempDataProvider = httpContext.RequestServices.GetRequiredService<ITempDataProvider>();

        var actionContext = new ActionContext(
            httpContext,
            httpContext.GetRouteData(),
            controller.ControllerContext.ActionDescriptor
        );

        var viewResult = viewEngine.FindView(actionContext, viewName, false);

        if (!viewResult.Success)
        {
            throw new InvalidOperationException($"View '{viewName}' not found.");
        }

        await using var writer = new StringWriter();

        var viewData = new ViewDataDictionary<TModel>(
            new EmptyModelMetadataProvider(),
            controller.ModelState
        )
        {
            Model = model
        };

        var tempData = new TempDataDictionary(httpContext, tempDataProvider);

        var viewContext = new ViewContext(
            actionContext,
            viewResult.View,
            viewData,
            tempData,
            writer,
            new HtmlHelperOptions()
        );

        await viewResult.View.RenderAsync(viewContext);
        return writer.ToString();
    }
}
