using Microsoft.AspNetCore.Http;

namespace SmileProject.Services
{
    public interface IResultService
    {
        ResultService Success(string content , object? obj = null);
        ResultService Failed(string content);
        ResultService NotFound(string content);
    }
    public class ResultService : IResultService
    {
        public string Content { get; set; }
        public int StatusCode { get; set; }
        public object? Object { get; set; }
        public ResultService Success(string content , object? obj = null)
        {
            //System.IO.File.AppendAllText("Result.txt",
            //    $"Result : {DateTime.Now} | Content = {content} | Successfull Login{Environment.NewLine}"
            //    );
            return new ResultService
            {
                Content = content,
                StatusCode = 200,
                Object = obj
            };

        }
        public ResultService Failed(string content)
        {
            /*System.IO.File.AppendAllText("Result.txt",
                $"Result : {DateTime.Now} | Content = {content} | Failed Operation{Environment.NewLine}"
                );*/
            return new ResultService
            {
                Content = content,
                StatusCode = 500
            };

        }

        public ResultService NotFound(string content)
        {
            //; System.IO.File.AppendAllText("Result.txt",
            //    $"Result : {DateTime.Now} | Content = {content} | NotFound Operation{Environment.NewLine}"
            //    );
            return new ResultService
            {
                Content = content,
                StatusCode = 404
            };
        }
    }
}
