

using eCommerce.SharedLibrary.Responses;

namespace eCommerce.SharedLibrary.Interface
{
    public interface IGenericInterface<in T> where T : class
    {
        Task<Response> CreateAsync(T entity);
    }
    
}
