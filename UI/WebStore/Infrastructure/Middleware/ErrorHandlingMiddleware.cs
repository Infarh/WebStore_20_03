using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WebStore.Infrastructure.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _Next;
        private readonly ILogger<ErrorHandlingMiddleware> _Logger;

        public ErrorHandlingMiddleware(RequestDelegate Next, ILogger<ErrorHandlingMiddleware> Logger)
        {
            _Next = Next;
            _Logger = Logger;
        }

        //public async Task Invoke(HttpContext Context)
        //{
        //    // Действие, выполняемое в нашу очередь в конвейере.
        //    var next_taks = _Next(Context); //  Запускаем следующее звено конвейера асинхронно
        //    // Можем параллельно выполнять какие-то другие действия
        //    await next_taks; // Ожидаем завершения оставшейся части конвейера
        //    // Действия, Выполняемые после завершения работы оставшейся части конвейера
        //}

        public async Task Invoke(HttpContext Context)
        {
            try
            {
                await _Next(Context);
            }
            catch (Exception e)
            {
                HandleException(Context, e);
                //throw e; // Неправильно. Повреждает стек вызова. Так делать не надо!!!
                throw;
            }
        }

        private void HandleException(HttpContext Context, Exception Error)
        {
            _Logger.LogError(Error, "Ошибка при обработке запроса {0}", Context.Request.Path);
        }
    }
}
