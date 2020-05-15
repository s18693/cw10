using System.Collections.Generic;
using cw3.Models;

namespace cw3.DAL
{
    public interface IDbService
    {
        //httpContext.Request.Method
        //[..].Path
        //[..].Body
        //[..].QueryString

        bool checkIndex(string index);
    }
}
