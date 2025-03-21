﻿using System.ComponentModel.DataAnnotations;

namespace OrderApi.Application.DTOs
{
    public record OrderDetailsDto
    (
        [Required] int OrderId,
        [Required] int ProductId,
        [Required] int Client,
        [Required] string Name,
        [Required, EmailAddress] string Email,
        [Required, EmailAddress] string Address,
        [Required] string TelephoneNumber,
        [Required] string ProductName,
        [Required] int ProductQuantity,
        [Required, DataType(DataType.Currency)] decimal UnitPrice,
        [Required, DataType(DataType.Currency)] decimal TotalPrice,
        [Required] DateTime OrderDate
        );
}
