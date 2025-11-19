using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StarterKit.Application.Abstractions.Services;
using StarterKit.Application.Consts;
using StarterKit.WebApi.Attributes;

namespace StarterKit.WebApi.Filters
{
    public class LocalizeResponseFilter : IAsyncResultFilter
    {
        private readonly ILocalizationService _localizer;

        public LocalizeResponseFilter(ILocalizationService localizer)
        {
            _localizer = localizer;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            bool hasAttribute =
                context.ActionDescriptor.EndpointMetadata.OfType<LocalizeResponseAttribute>().Any();

            if (!hasAttribute)
            {
                await next();
                return;
            }

            if (context.Result is ObjectResult objectResult &&
                objectResult.Value is ResponseDto dto &&
                dto.Message is not null)
            {
                var lang = context.HttpContext.Request.Headers["Accept-Language"].ToString().ToLower();

                if (string.IsNullOrEmpty(lang) || !(lang == "az" || lang == "en" || lang == "ru"))
                    lang = "az";

                dto.Message = _localizer.GetMessage(dto.Message, lang);
            }

            await next();
        }
    }

}
