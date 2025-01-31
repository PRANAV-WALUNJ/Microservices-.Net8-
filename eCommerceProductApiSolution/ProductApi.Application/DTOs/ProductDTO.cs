using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductApi.Application.DTOs
{
    public record ProductDTO
    (
        int Id,
        string Name,
        [Required,Range(1,int.MaxValue)] int Quntity,
        [Required, DataType(DataType.Currency)] decimal Price
    );
}
