namespace Scrummy.GameService.Api;

public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
{
    public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context) { }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is ActorMethodInvocationException actorMethodInvocationException &&
            actorMethodInvocationException.InnerException is ActorInvokeException actorInvokeException)
        {
            if (actorInvokeException.ActualExceptionType.Equals(typeof(UnauthorizedAccessException).FullName))
            {
                context.Result = new ObjectResult(actorInvokeException.Message)
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };

                context.ExceptionHandled = true;
                return;
            }

            if (actorInvokeException.ActualExceptionType.Equals(typeof(InvalidOperationException).FullName))
            {
                context.Result = new ObjectResult(actorInvokeException.Message)
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };

                context.ExceptionHandled = true;
                return;
            }
        }
    }
}
