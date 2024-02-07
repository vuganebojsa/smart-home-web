using FluentResults;

namespace SmartHouse.Extensions
{
    public static class ResultExtensions
    {
        public static Result<T> FailBadRequest<T>(string errorMessage) => Result.Fail<T>(new Error(errorMessage).WithMetadata("StatusCode", 400));
        public static Result<T> FailNotFound<T>(string errorMessage) => Result.Fail<T>(new Error(errorMessage).WithMetadata("StatusCode", 404));
        public static Result FailNotFound(string errorMessage) => Result.Fail(new Error(errorMessage).WithMetadata("StatusCode", 404));
        public static Result FailBadRequest(string errorMessage) => Result.Fail(new Error(errorMessage).WithMetadata("StatusCode", 400));
    }
}
